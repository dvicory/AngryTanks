#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using Nuclex.Support.Collections;

namespace Nuclex.Networking.Http {

  /// <summary>Simple HTTP server that answers requests by clients</summary>
  public class HttpServer : IDisposable {

    #region class ConnectionEntry

    /// <summary>Encapsulates an HTTP client connection managed by the server</summary>
    private class ConnectionEntry {

      /// <summary>Initializes a new connection entry</summary>
      /// <param name="connection">Connected HTTP client to store in the entry</param>
      public ConnectionEntry(ClientConnection connection) {
        this.Connection = connection;
      }

      /// <summary>Connected HTTP client managed by this entry</summary>
      public volatile ClientConnection Connection;

    }

    #endregion // class ConnectionEntry

    /// <summary>Initializes a new HTTP server on port 80</summary>
    public HttpServer() : this(80) { }

    /// <summary>Initializes a new HTTP server</summary>
    /// <param name="port">
    ///   TCP port the server should liston on for incoming connections
    /// </param>
    public HttpServer(short port) {
      this.port = port;
      this.clientConnectedDelegate = new EventHandler<SocketEventArgs>(clientConnected);

      this.connectedClients = new Dictionary<ClientConnection, ConnectionEntry>();
      this.clientCleanupQueue = new PairPriorityQueue<DateTime, ConnectionEntry>(
        new ReverseComparer<DateTime>()
      );
      this.idleConnectionDropTime = new TimeSpan(0, 0, 30);
      this.idleConnectionCleanupThreadWakeupEvent = new ManualResetEvent(false);
    }

    /// <summary>Starts the http server</summary>
    public void Start() {
      lock(this) {

        // If the listener does not exist that indicates that we're not running. Do nothing
        // otherwise (we allow multiple Start()s - the redundant calls will simply do nothing)
        if(this.listener == null) {

          // Start the idle connection cleanup thread. We use an explicit thread here
          // instead of a thread pool wait request in order to guarantee connection cleanup.
          // If the server becomes very loaded (to the point of running out of thread pool
          // threads), reliability would otherwise suffer because the idle connection
          // cleaner might in itself be queued in the thread pool amongst all the connection
          // processing tasks.
          //
          // This is not a protection against a DoS attack since it's still possible to
          // flood the server with countless connections. Flooding still has to be detected
          // and and resolved by the implementer if so desired.
          this.stopIdleConnectionCleanupThread = false;
          this.idleConnectionCleanupThread = new Thread(
            new ThreadStart(runIdleConnectionCleanupLoop)
          );
          this.idleConnectionCleanupThread.Name = "HTTP Server Idle Connection Cleaner";
          this.idleConnectionCleanupThread.IsBackground = true;
          this.idleConnectionCleanupThread.Start();

          // Start the socket listener
          this.listener = new SocketListener(this.port);
          this.listener.ClientConnected += this.clientConnectedDelegate;
          this.listener.StartListening();

        }

      }
    }

    /// <summary>Stops the http server</summary>
    public void Stop() {
      lock(this) {

        // The existence of the listener indicates that we're still running. Do nothing
        // otherwise (we allow multiple Stop()s - the redundant calls will simply do nothing)
        if(this.listener != null) {

          // Stop the listener first to ensure we will receive no more connections
          this.listener.ClientConnected -= this.clientConnectedDelegate;
          this.listener.Dispose();
          this.listener = null;

          // Now stop the idle connection cleanup thread - we're going to drop all connections
          // and don't want that thread messing things up for us
          this.stopIdleConnectionCleanupThread = true;
          this.idleConnectionCleanupThreadWakeupEvent.Set();
          this.idleConnectionCleanupThread.Join();
          this.idleConnectionCleanupThread = null;

          // No more incoming connections will be accepted and the idle connection cleanup
          // thread is stopped, now we can safely drop all client connections
          DropAllClients();

        }

      }
    }

    /// <summary>Drops all clients currently connected to the server</summary>
    public void DropAllClients() {
      int connectionCount;
      ClientConnection[] clientConnections;

      lock(this.connectedClients) {
        connectionCount = this.connectedClients.Count;
        clientConnections = new ClientConnection[connectionCount];
        this.connectedClients.Keys.CopyTo(clientConnections, 0);
      }

      for(; connectionCount > 0; --connectionCount) {
        clientConnections[connectionCount - 1].Drop();
      }
    }

    /// <summary>Immediately releases all resources used by the instance</summary>
    public void Dispose() {
      Stop();
    }

    /// <summary>Time after which the server will drop inactive connections</summary>
    /// <remarks>
    ///   This is mainly a safeguard against faulty clients. If a client doesn't close
    ///   his connection after a reasonable idle time (internet standards suggest
    ///   15-30 seconds), the server will take action. Connectivity breakdowns in
    ///   clients can also result in connections dying without proper termination and
    ///   would result in the server slowly building up more and more connections.
    /// </remarks>
    public TimeSpan IdleConnectionDropTime {
      get { return this.idleConnectionDropTime; }
      set {
        lock(this) {
          this.idleConnectionDropTime = value;
          this.idleConnectionCleanupThreadWakeupEvent.Set();
        }
      }
    }

    /// <summary>Called to accepts incoming client connections</summary>
    /// <param name="connectedSocket">Socket of the connected client</param>
    /// <returns>
    ///   A new client connection responsible for managing requests by the
    ///   connected client.
    /// </returns>
    protected virtual ClientConnection AcceptClientConnection(Socket connectedSocket) {
      return new ClientConnection(this, connectedSocket);
    }

    /// <summary>Notifies the server that a client has disconnected</summary>
    /// <param name="client">Client that has disconnected</param>
    internal void NotifyClientDisconnected(ClientConnection client) {
      lock(this.connectedClients) {
        ConnectionEntry entry;
        if(this.connectedClients.TryGetValue(client, out entry)) {
          ClientConnection connection = entry.Connection;
          entry.Connection = null;
          this.connectedClients.Remove(connection);
        }
      }
    }

    /// <summary>Cleans up idle connections after they have timed out</summary>
    private void runIdleConnectionCleanupLoop() {
      for(; ; ) {

        DateTime cleanupCycleStartTime = DateTime.UtcNow;
        TimeSpan timeToNextCleanup;

        // Clean up all connections that have been idling for longer than they
        // are allowed to
        for(; ; ) {

          // If the HTTP server wants us to stop, break the loop here!
          if(this.stopIdleConnectionCleanupThread) {
            return;
          }

          // Take out the next item that needs to be cleaned. We take the lock only for
          // this moment to avoid stalling the acceptance of new connections while we're
          // cleaning up this connection.
          PriorityItemPair<DateTime, ConnectionEntry> nextConnectionToClean;
          lock(this.clientCleanupQueue) {
            if(this.clientCleanupQueue.Count == 0) {
              timeToNextCleanup = this.idleConnectionDropTime;
              break;
            }

            nextConnectionToClean = this.clientCleanupQueue.Peek();
          }

          // See if this connection is a candidate for cleanup
          TimeSpan idleTime = cleanupCycleStartTime - nextConnectionToClean.Priority;
          if(idleTime > this.idleConnectionDropTime) {

            // The connection was idle for too long - clean it up. The call to Drop()
            // will call back on us to take the connection out of the normal connection
            // list, we only need to bother with our idle connection cleanup queue.
            lock(this.clientCleanupQueue) {
              this.clientCleanupQueue.Dequeue();
              ClientConnection connection = nextConnectionToClean.Item.Connection;
              if(!ReferenceEquals(connection, null)) {
                connection.Drop();
              }
            }

          } else {

            // The connection has not yet reached the idle timeout. Since we're using a
            // priority queue, we can be sure that this is the connection that was idle
            // the longest and that there will be no other timeouts occuring before it.
            idleTime = DateTime.UtcNow - nextConnectionToClean.Priority;
            timeToNextCleanup = idleTime + OneSecondTimeSpan;
            break;

          }

        } // for(;;)

        // Wait until the next connection becomes idle or we're waken up
        this.idleConnectionCleanupThreadWakeupEvent.WaitOne(timeToNextCleanup, false);

      } // for(;;)
    }

    /// <summary>Called when a client connects to the http server</summary>
    /// <param name="sender">The socket listener reporting the new connection</param>
    /// <param name="arguments">Contains the socket of the connecting client</param>
    private void clientConnected(object sender, SocketEventArgs arguments) {
      ClientConnection clientConnection;

      // An exception from the AcceptClientConnection method would end up in the
      // ThreadPool. In .NET 2.0, the behavior in this case is clearly defined
      // (ThreadPool prints exception to console and ignores it), but since this
      // would leave the client running into the timeout, we try to at least
      // shut down the socket gracefully and to pester the developer
      try {
        clientConnection = AcceptClientConnection(arguments.Socket);
      }
      catch(Exception exception) {
        System.Diagnostics.Trace.WriteLine(
          "AcceptClientConnection() threw an exception, dropping connection"
        );
        System.Diagnostics.Trace.WriteLine(
          "Exception from AcceptClientConnection(): " + exception.ToString()
        );
        System.Diagnostics.Trace.Assert(
          false, "AcceptClientConnection() threw an exception, connection will be dropped"
        );

        // Send 500 internal server error here!

        try {
          arguments.Socket.Shutdown(SocketShutdown.Both);
        }
        finally {
          arguments.Socket.Close();
        }

        throw;
      }

      try {
        ConnectionEntry entry = new ConnectionEntry(clientConnection);
        lock(this.clientCleanupQueue) {
          this.clientCleanupQueue.Enqueue(DateTime.UtcNow, entry);
        }
        lock(this.connectedClients) {
          this.connectedClients.Add(clientConnection, entry);
        }
      }
      catch(Exception) {
        clientConnection.Drop();
        throw;
      }
    }

    /// <summary>Timespan with a length of 1 second</summary>
    private static readonly TimeSpan OneSecondTimeSpan = new TimeSpan(0, 0, 1);

    /// <summary>Listener used to accept incoming connections</summary>
    private SocketListener listener;
    /// <summary>Port the http server is listening on</summary>
    private short port;
    /// <summary>Delegate for the clientConnectd() method</summary>
    private EventHandler<SocketEventArgs> clientConnectedDelegate;
    /// <summary>clients sorted by time of their last transmission</summary>
    /// <remarks>
    ///   This queue will also retain entries for connections that have been closed due
    ///   to means other than a timeout. In this case, the additional indirection provided
    ///   by the ConnectionEntry will allow the queue item's connection to be set to
    ///   null and thus, be silently discarded when its lifetime expires.
    /// </remarks>
    private PairPriorityQueue<DateTime, ConnectionEntry> clientCleanupQueue;
    /// <summary>Time until an idle connection will be dropped by the server</summary>
    private TimeSpan idleConnectionDropTime;
    /// <summary>Set to true to stop the idle connection cleanup thread</summary>
    private volatile bool stopIdleConnectionCleanupThread;
    /// <summary>Thread used to clean up idle connections from the server</summary>
    private Thread idleConnectionCleanupThread;
    /// <summary>Wakeup event for the idle connection cleanup thread</summary>
    private ManualResetEvent idleConnectionCleanupThreadWakeupEvent;
    /// <summary>Dictionary containing the clients currently connected to the server</summary>
    private Dictionary<ClientConnection, ConnectionEntry> connectedClients;

  }

} // namespace Nuclex.Networking.Http
