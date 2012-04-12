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

namespace Nuclex.Networking {

  // Idea:
  // Refactor some of the functionality into a separate class so besides this
  // socket receiver, an adapter can be built that allows cancellation of
  // asynchronous socket requests 

  /// <summary>Asynchronously receives data from a socket</summary>
  /// <remarks>
  ///   This class simplifies the implementation of servers that need to receive
  ///   unsolicited data from sockets asynchronously. It uses a lock-free
  ///   receiving loop to ensure maximum performance for the receiver.
  /// </remarks>
  public class SocketReceiver : IDisposable {

    /// <summary>Initializes a new connection from a HTTP client</summary>
    /// <param name="socket">Socket of the connected client</param>
    /// <remarks>
    ///   This object takes ownership of the socket and will close it upon being
    ///   disposed. This cannot be avoided because the only way to stop a waiting
    ///   receive request in the .NET framework is to close the receiving socket.
    /// </remarks>
    public SocketReceiver(Socket socket) : this(socket, 256) { }

    /// <summary>Initializes a new connection from a HTTP client</summary>
    /// <param name="socket">Socket of the connected client</param>
    /// <param name="bufferSize">Size of the receive buffer</param>
    /// <remarks>
    ///   This object takes ownership of the socket and will close it upon being
    ///   disposed. This cannot be avoided because the only way to stop a waiting
    ///   receive request in the .NET framework is to close the receiving socket.
    /// </remarks>
    public SocketReceiver(Socket socket, int bufferSize) {
      this.dataReceivedDelegate = new AsyncCallback(this.dataReceived);
      this.socket = socket;
      this.buffer = new byte[bufferSize];

      // Make sure the Socket and its receive buffer are written to memory. Since
      // the receive loop cannot be running at this time (no call to Start() yet),
      // we can be sure that it will (no matter which thread it runs in) either pull
      // all three fields from memory or run on the same CPU cache as the constructor
      // -- thus we can avoid declaring the above three fields as volatile!
      Thread.MemoryBarrier();
    }

    /// <summary>Begins receiving data from the socket</summary>
    /// <remarks>
    ///   Normally, this method is to be called in the constructor of the deriving class
    ///   to begin the asynchronous receive loop after you initialized any additional
    ///   fields you are using in the OnDataReceived() callback.
    /// </remarks>
    protected void Start() {

      lock(this) {

        // If a shutdown was requested (eg. Dispose() runs, possible in another
        // thread), do not start. This is checked before wasStarted is set because
        // otherwise, the Dispose() thread might see wasStarted being true and
        // try to wait for the end of a non-existing thread.
        if(Thread.VolatileRead(ref this.shutdownRequested) != 0) {
          return;
        }

        // If we were already started, do nothing. Multiple calls to Start() should
        // not happen, but are tolerated and should not result in multiple asynchronous
        // receive loop competing to get data from the socket.
        if(this.wasStarted) {
          System.Diagnostics.Trace.Assert(
            !this.wasStarted, "Start called on an already started socket receiver"
          );
          return;
        }

        // Remember that we were started so Dispose() knows that it needs to
        // stop the receive loop and to catch multiple Start() calls.
        this.wasStarted = true;

      } // lock(this)

      // Begin processing data from the socket asynchronously. This is done in a
      // ThreadPool thread to eliminate the risk of the socket doing several synchronous
      // runs of returning data, thereby blocking the calling thread.
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.startReceiving));

    }

    /// <summary>Immediately stops the receive loop and releases all resources</summary>
    public void Dispose() {

      // Required to a) prevent races with threads calling the Start() method and
      // to b) avoid chaos when two threads try to call Dispose() at the same time.
      lock(this) {

        // If we're already disposed, do nothing
        if(Thread.VolatileRead(ref this.shutdownRequested) != 0) {
          return;
        }

        // If the asynchronous receive loop has already started, we need to shut
        // it down before we can safely release our resources
        if(this.wasStarted) {

          // The asynchronous receive loop will check the 'shutdownRequested' variable
          // without synchronization (in order to avoid locking in the receive loop).
          // However, when it detects the shutdownRequested flag, it will set this
          // event and shut down. Thus we need to create the event before setting the
          // shutdown flag!
          this.shutdownCompleteEvent = new ManualResetEvent(false);
          try {
            // Cache the socket in case this thread is preempted by the socket receiver
            // thread, setting the socket to null right after we have set the
            // shutdownRequested field
            Socket openSocket = this.socket;

            // Make the receive loop shut down. If it is currently waiting for incoming
            // data, the only way to get it out of this state is to close the socket.
            Thread.VolatileWrite(ref this.shutdownRequested, 1);
            openSocket.Close();

            // Wait for the asynchronous receive loop to end
            this.shutdownCompleteEvent.WaitOne();
          }
          finally {
            this.shutdownCompleteEvent.Close();
          }

        } else {

          // Asynchronous receive loop was not started yet. In case we preempted the
          // Start() method directly at the top, wasStarted will never be set once the
          // thread that runs Start() enters the lock and sees the shutdown flag.
          Thread.VolatileWrite(ref this.shutdownRequested, 1);

        }

        // Helps the GC on Compact Framework builds
        this.socket = null;

      } // lock(this)

    }

    /// <summary>Shuts down the connection graciously</summary>
    /// <remarks>
    ///   Using this method is preferrable to simply disposing the SocketReceiver
    ///   because it gives the other side a chance to be notified when the connection
    ///   goes down and cleanly stops the transmission of data.
    /// </remarks>
    protected void Shutdown() {
      lock(this) {

        // If we're already disposed, do nothing
        if(Thread.VolatileRead(ref this.shutdownRequested) != 0) {
          return;
        }

        // The socket can be disposed at any time, so make sure we're not running into
        // a race condition for the reference here and store it locally
        Socket socket = this.socket;
        if(socket == null)
          return;

        // If the connection still appears to be active, we'll attempt a gracious
        // disconnect before closing the socket for good. Of course, this leads to
        // a race condition (between our if and a possible connection shutdown
        // outside of our control) that we have to simply accept due to the design
        // of the .NET Socket library, thus the try..finally
        try {
          if(this.socket.Connected) {
            this.socket.Shutdown(SocketShutdown.Both);
          }
        }
        finally {
          // CHECK: OnConnectionDropped() may be called here
          // OnConnectionDropped() might be called when the other side closes
          // its connection before we enter the Dispose() method (i.e. _real_ fast).
          // Is this a problem for the user?
          Dispose();
        }

      } // lock(this)
    }

    /// <summary>Called whenever data is received on the socket</summary>
    /// <param name="buffer">Buffer containing the received data</param>
    /// <param name="receivedByteCount">Number of bytes that have been received</param>
    protected virtual void OnDataReceived(byte[] buffer, int receivedByteCount) { }

    /// <summary>Called when the connection has been dropped by the peer</summary>
    protected virtual void OnConnectionDropped() { }

    /// <summary>Begins receiving data from the socket in the background</summary>
    /// <param name="state">Not used</param>
    private void startReceiving(object state) {

      // Take a reference to the socket so we don't run race conditions with
      // Dispose() setting the socket to null and us accessing it.
      Socket socket = this.socket;
      if(ReferenceEquals(socket, null)) {
        safelyEndReceivingLoop();
        return;
      }

      // Since the BeginReceive() call might complete synchronously, we may have to
      // repeat the call several times. If we just invoked this method again from the
      // dataReceived() callback, we would go deeper and deeper in the call stack,
      // possibly causing a StackOverflowException in extreme cases.
      for(; ; ) {

        // Stop the receive loop if the socket receiver was requested to shut down
        if(Thread.VolatileRead(ref this.shutdownRequested) != 0) {
          this.shutdownCompleteEvent.Set();
          return;
        }

        IAsyncResult asyncResult;
        try {
          // Begin receiving data from the socket. This call can either result in a
          // synchronous invocation of the callback (if data was already in the socket's
          // internal buffer) or in an asynchronous invocation (if the socket has not
          // yet received any data).
          asyncResult = this.socket.BeginReceive(
            this.buffer,
            0,
            this.buffer.Length,
            SocketFlags.None,
            this.dataReceivedDelegate,
            null
          );
        }
        catch(ObjectDisposedException) {
          // This can happen during shutdown due to the design of the socket class
          // in the .NET framework. We try our best to avoid it, but there's no
          // guarantee that the socket isn't closed by another thread just when it
          // received some data.
          safelyEndReceivingLoop();
          return;
        }

        // If the request is running asynchronously (meaning no data was in the
        // socket's internal buffer yet; see comment above), exit this thread.
        // The ThreadPool thread on which the socket called the dataReceived callback
        // will initiate next startReceiving() call as soon as data arrives.
        if(!asyncResult.CompletedSynchronously) {
          break;
        }

      }
    }

    /// <summary>Called when data has been received on the socket</summary>
    /// <param name="asyncResult">Handle of the asynchronous request</param>
    private void dataReceived(IAsyncResult asyncResult) {

#if DEBUG
      // In debug mode, give the thread a reasonable name so the user can clearly
      // see the purpose of threads in the debugger's thread view.
      Thread.CurrentThread.Name = "HttpClientConnection socket receiver";
#endif

      // If the socket is being closed by another thead, it will be set to null in
      // order to ensure it's no longer used to avoid multiple close attempts if
      // dispose is called more than once. To prevent the race condition that could
      // result, we take a copy of the socket and 
      Socket socket = this.socket;
      if(ReferenceEquals(socket, null)) {
        safelyEndReceivingLoop();
        return;
      }

      int receivedBytes;
      try {

        // If the socket has been closed, try to get out of here without causing
        // an exception. EndReceive() does not have to be called when the socket has
        // been disposed already (you generally shouldn't cease any communication with
        // disposed objects)
        if(!this.socket.Connected) {
          safelyEndReceivingLoop();
          return;
        }

        // Now we've got a good chance that the socket has not been closed yet, so
        // try and hope for the best. The design of the socket class doesn't provide us
        // with a better way for the standard async pattern, so we have to accept and
        // eat up the ObjectDisposedException if it occurs.
        receivedBytes = this.socket.EndReceive(asyncResult);

      }
      catch(ObjectDisposedException) {
        // This can happen during shutdown due to the design of the socket class
        // in the .NET framework. We try our best to avoid it, but there's no
        // guarantee that the socket isn't closed by another thread just when it
        // received some data.
        safelyEndReceivingLoop();
        return;
      }

      // If the we're being asked to shut down, do so without invoking the callbacks.
      // We will only call OnConnectionDropped() when _the_other_side_ closes the
      // connection. If the user called Dispose(), we don't want to invoke the callback!
      if(Thread.VolatileRead(ref this.shutdownRequested) != 0) {
        this.shutdownCompleteEvent.Set();
        return;
      }

      // Receiving 0 bytes indicates that the connection has been dropped by the peer
      // using the graceful connection shutdown procedure, so in that case, we're done
      if(receivedBytes == 0) {
        safelyEndReceivingLoop();
        OnConnectionDropped();
        return;
      }

      // Finally, we can process the data we received on the socket
      OnDataReceived(this.buffer, receivedBytes);

      // Resume listening for incoming data. This is done after the processReceivedData()
      // intentionally - the idea is the processReceivedData() should not throw. If it
      // does, it obviously coughed on the data received and we don't know if it processed
      // part of it, all of it, and whether it will cough again, so what's the use on
      // giving it the next slice of data, cut at an arbitrary point?
      if(!asyncResult.CompletedSynchronously) {
        startReceiving(null);
      }

    }

    /// <summary>Ends the receiving loop in a safe manner</summary>
    /// <remarks>
    ///   Called when the asynchronous receiving loop is terminated unexpectedly. This
    ///   happens when the other side closes the connection or a socket error occurs.
    ///   On the remote chance that someone is running the Dispose() method just now,
    ///   we have to guarantee that Dispose() won't sit there, forever waiting for its
    ///   shutdownCompleteEvent() to become set.
    /// </remarks>
    private void safelyEndReceivingLoop() {
      int wasShuttingDown = Interlocked.Exchange(ref this.shutdownRequested, 1);
      if(wasShuttingDown != 0) {
        this.shutdownCompleteEvent.Set();
      } else {
        this.socket.Close();
        this.socket = null;
      }
    }

    /// <summary>Delegate for the DataReceived callback method</summary>
    private AsyncCallback dataReceivedDelegate;
    /// <summary>Socket of the connected client</summary>
    private Socket socket;
    /// <summary>Buffer into which received data is written by the socket</summary>
    private byte[] buffer;

    /// <summary>Whether the Start() method has been called</summary>
    private bool wasStarted;
    /// <summary>True if the receive loop should stop</summary>
    private int shutdownRequested;
    /// <summary>Used by Dispose() to wait for the receive loop to end</summary>
    private volatile ManualResetEvent shutdownCompleteEvent;

  }

} // namespace Nuclex.Networking
