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
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Nuclex.Networking.Http {

  /// <summary>Handles a connection from an HTTP client</summary>
  public class ClientConnection : SocketReceiver {

    /// <summary>Default size for the incoming data buffer</summary>
    private const int DefaultBufferSize = 256;

    /// <summary>Initializes a new connection from a HTTP client</summary>
    /// <param name="server">Server the client is connected to</param>
    /// <param name="socket">Socket of the connected client</param>
    public ClientConnection(HttpServer server, Socket socket) :
      this(server, socket, DefaultBufferSize) {
      this.socket = socket;
    }

    /// <summary>Initializes a new connection from a HTTP client</summary>
    /// <param name="server">Server the client is connected to</param>
    /// <param name="socket">Socket of the connected client</param>
    /// <param name="bufferSize">Size of the receive buffer</param>
    public ClientConnection(HttpServer server, Socket socket, int bufferSize) :
      base(socket, bufferSize) {
      this.server = server;
      this.parser = new RequestParser(1024);
      Start();
    }

    /// <summary>Drops the client from the server</summary>
    public void Drop() {
      try {
        this.server.NotifyClientDisconnected(this);
      }
      finally {
        Shutdown();
      }
    }

    /// <summary>Called when the connection has been dropped by the peer</summary>
    protected virtual void OnPeerDisconnected() {
      this.server.NotifyClientDisconnected(this);
    }

    /// <summary>Called whenever data is received on the socket</summary>
    /// <param name="buffer">Buffer containing the received data</param>
    /// <param name="receivedByteCount">Number of bytes that have been received</param>
    protected override void OnDataReceived(byte[] buffer, int receivedByteCount) {
      parseRequest(buffer, receivedByteCount);
    }

    /// <summary>Processes the provided request and generates a server response</summary>
    /// <param name="request">Request to be processed by the server</param>
    /// <returns>The response to the server request</returns>
    protected virtual Response ProcessRequest(Request request) {
#if true // GENERATE_DUMMY_RESPONSE

      Console.WriteLine(
        DateTime.Now.ToString() + " Processed request for " + request.Uri
      );

      // Here's the HTML document we want to send to the client
      MemoryStream messageMemory = new MemoryStream();
      StreamWriter writer = new StreamWriter(messageMemory, Encoding.UTF8);
      writer.WriteLine("<html>");
      writer.WriteLine("<head><title>Hello World</title></head>");
      writer.WriteLine("<body><small>Hello World from the Nuclex Web Server</small></body>");
      writer.WriteLine("</html>");
      writer.Flush();

      // Prepare the response message
      Response theResponse = new Response(StatusCode.S200_OK);

      // Add some random headers web server's like to provider
      theResponse.Headers.Add("Cache-Control", "private");
      theResponse.Headers.Add("Content-Type", "text/html; charset=UTF-8");
      theResponse.Headers.Add("Date", "Wed, 30 Jul 2008 14:01:06 GMT");
      theResponse.Headers.Add("Server", "Nuclex");

      // Attach the HTML document to our response
      theResponse.AttachStream(messageMemory);

      // Now comes the important part, specify how many bytes we're going to
      // transmit.
      // TODO: This should be done by parseRequest()
      //       Whether it's needed or not depends on the transport protocol used
      messageMemory.Position = 0;
      theResponse.Headers.Add("Content-Length", messageMemory.Length.ToString());

#endif

      return theResponse;
    }

    /// <summary>Parses incoming data into an HTTP request</summary>
    /// <param name="buffer">Buffer containing the received data</param>
    /// <param name="receivedByteCount">Number of bytes in the receive buffer</param>
    private void parseRequest(byte[] buffer, int receivedByteCount) {

      ArraySegment<byte> data = new ArraySegment<byte>(buffer, 0, receivedByteCount);

      while(data.Count > 0) {

        Response response; // Response that will be delivered to the client
        bool dropConnection = false; // Whether to close the connection

        try {

          // Try to parse a complete request from the bytes the client has sent to us.
          // If there isn't enough data available yet, we exit here and hopefully the
          // next time data arrives it will be enough to complete the request entity.
          Request request = this.parser.ProcessBytes(data.Array, data.Offset, data.Count);
          if(request == null) {
            break;
          }

          // We've got an actual request. Now let's handle it using the implementation
          // provided by the deriving class from the user!
          response = ProcessRequest(request);

        }
        catch(Exceptions.HttpException httpException) {
          response = new Response(httpException.StatusCode, httpException.Message);
          dropConnection = true;
        }
        catch(Exception exception) {
          response = new Response(StatusCode.S500_Internal_Server_Error, exception.Message);
          dropConnection = true;
        }

        // Transform the response entity into a stream of bytes and send it back to
        // the client. If any additional data has to be transmitted, this will be handled
        // by the attachment transmitter.
        byte[] responseBytes = ResponseFormatter.Format(response);
        this.socket.Send(responseBytes); // TODO: Can get an ObjectDisposedException here!

        // TODO: Evil hack!
        if(response.AttachedStream != null) {
          this.socket.Send(((MemoryStream)response.AttachedStream).GetBuffer());
        }

        // TODO: Respect the keep-alive request header here
        //       If keep-alive is not set, we should close the connection here!

        if(dropConnection) {
          Drop();
        } else {
          // Now take the remaining data out of the parser and bring the parser back into
          // its initial state so it can begin parsing the next request
          data = this.parser.GetRemainingData();
          this.parser.Reset();
        }

      }

    }

    /// <summary>Server to which the client is connected</summary>
    private HttpServer server;
    /// <summary>HTTP request parser we use for interpreting client requests</summary>
    private RequestParser parser;
    /// <summary>Socket the client is connected by</summary>
    private Socket socket;

  }

} // namespace Nuclex.Networking.Http
