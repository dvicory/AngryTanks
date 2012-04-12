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
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Nuclex.Networking {

  /// <summary>Listens for incoming TCP/IP connections on a specific port</summary>
  /// <remarks>
  ///   <para>
  ///     This listener can handle moderate loads. It doesn't use the new support for
  ///     IO completion ports in .NET 2.0 SP1 for the listening socket, but the
  ///     user is free to use them for the actual transmissions on accepted connections.
  ///   </para>
  ///   <para>
  ///     Using the .NET asynchronous pattern, this listener will be able to accept
  ///     incoming connections until the thread pool reaches full capacity (which will
  ///     take quite a beating), at which point the listening socket's queue of pending
  ///     connections (backlog) will start to load up, meaning that unless another
  ///     method of limiting the number of allowed client connections is implemented,
  ///     a maximum of ThreadPool.GetMaxThreads() + Socket Backlog Size connections
  ///     can be made before the listener will begin rejecting incoming connections.
  ///   </para>
  /// </remarks>
  public class SocketListener : IDisposable {

    /// <summary>Triggered whenever a client connects</summary>
    public event EventHandler<SocketEventArgs> ClientConnected;

    /// <summary>Initializes a new incoming TCP/IP connection listener</summary>
    /// <param name="port">Port to listen on</param>
    public SocketListener(short port) {
      setupSocket(port);
    }

    /// <summary>Shuts down the TCP/IP connection listener</summary>
    public void Dispose() {

      // This variable is mainly used by the acceptIncomingConnection() callback to
      // distinguish whether it was called as an effect of a new incoming connection
      // or due to the socket being shut down.
      int wasShuttingDown = Interlocked.Exchange(ref this.shutdownState, 1);

      // We can also use this to avoid double shut downs a little more efficiently :)
      if(wasShuttingDown != 1) {
        if(this.listeningSocket != null) {
          this.listeningSocket.Close();
          this.listeningSocket = null;
        }
      }

    }

    /// <summary>Begins listening for incoming TCP/IP connections</summary>
    /// <remarks>
    ///   This method is provided so you have the chance to subscribe to the
    ///   <see cref="ClientConnected" /> event before incoming connection attempts
    ///   start being accepted by the listener.
    /// </remarks>
    public void StartListening() {

      // Make sure we're still alive
      if(this.shutdownState == 1) {
        throw new ObjectDisposedException("Socket listener has already been disposed");
      }

      // If we're already listening, do nothing (redundant StartListening() calls
      // are accepted by this library)
      int wasListening = Interlocked.Exchange(ref this.listeningState, 1);
      if(wasListening == 1) {
        return;
      }

      // Bring the socket into a listening state. This is a seperate function since,
      // after accepting a connection, this has to be repeated
      internalStartListening();

    }

    /// <summary>
    ///   Fires the ClientConnected event when a client connects to the listener
    /// </summary>
    /// <param name="connectedClient">Socket of the client that has connected</param>
    protected virtual void OnClientConnected(Socket connectedClient) {
      EventHandler<SocketEventArgs> copy = ClientConnected;
      if(copy != null) {
        copy(this, new SocketEventArgs(connectedClient));
      }
    }

    /// <summary>Internal listening start method for use in the accept callback</summary>
    /// <remarks>
    ///   This method is called once to initiate listening when the user calls
    ///   <see cref="StartListening" /> and then by the connection callback to resume
    ///   listening after an incoming connection is accepted.
    /// </remarks>
    private void internalStartListening() {
      for(; ; ) {

        // Asynchronously wait for the next client to connect to the socket. If there's
        // already a client in the socket's pending connection queue, there's a chance
        // that the call will complete synchronously and, thus, invoke the callback
        // from this thread.
        IAsyncResult asyncAcceptResult = this.listeningSocket.BeginAccept(
          acceptIncomingConnectionDelegate, null
        );

        // If the callback didn't run synchronously, we can safely exit here. When the
        // callback is made, it will be from a ThreadPool thread. We loop here otherwise
        // to avoid calling BeginAccept() again from the callback, which theoretically
        // could cause a stack overflow.
        if(!asyncAcceptResult.CompletedSynchronously) {
          break;
        }

      }
    }

    /// <summary>
    ///   Callback that will be invoked by the socket when a client connects
    /// </summary>
    /// <param name="asyncResult">Handle of the asynchronous call</param>
    private void acceptIncomingConnection(IAsyncResult asyncResult) {

      // Take a copy of the listening socket so we don't run into a NullReferenceException
      // when the socket is being destroyed at the same time a connection arrives
      // (or the callback is invoked because the socket is being destroyed and the
      // destroying thread was faster than us)
      Socket listeningSocket = this.listeningSocket;
      if((this.shutdownState == 1) || (this.listeningSocket == null)) {
        return;
      }

      // The call was not tiggered by the socket shutting down, so we have
      // a connection to accept!
      Socket acceptedConnection = this.listeningSocket.EndAccept(asyncResult);

      // Register and process the new client connection
      OnClientConnected(acceptedConnection);

      // If this callback was made from a ThreadPool thread, we can resume listening here.
      // Otherwise, we're still in the thread that called BeginReceive() and will let
      // that thread call BeginReceive() another time to avoid going deeper in the stack.
      if(!asyncResult.CompletedSynchronously) {
        internalStartListening();
      }

    }

    /// <summary>
    ///   Constructs the socket we're using to listen for incoming connections
    /// </summary>
    /// <param name="port">Port the socket is to listen on</param>
    private void setupSocket(short port) {
      this.acceptIncomingConnectionDelegate = new AsyncCallback(
        acceptIncomingConnection
      );

      this.listeningSocket = new Socket(
        AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp
      );

      try {
        IPEndPoint localEndPoint = new IPEndPoint(
          IPAddress.Parse("127.0.0.1"), port
        );

        this.listeningSocket.Bind(localEndPoint);
        this.listeningSocket.Listen(-1);
      }
      catch(Exception) {
        this.listeningSocket.Close();
        this.listeningSocket = null;
        throw;
      }
    }

    /// <summary>Socket we're using to listen for incoming connection attempts</summary>
    private volatile Socket listeningSocket;
    /// <summary>Delegate for the acceptIncomingConnection() method</summary>
    private AsyncCallback acceptIncomingConnectionDelegate;
    /// <summary>Whether the socket receiver is currently shutting down</summary>
    private int shutdownState;
    /// <summary>Whether the socket receiver is currently listening</summary>
    private int listeningState;

  }

} // namespace Nuclex.Networking
