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

namespace Nuclex.Networking.Http {

  /// <summary>Carries a response the HTTP server can send to a client</summary>
  public class Response {

    /// <summary>Initializes a new HTTP server response</summary>
    public Response() {
      this.Version = "HTTP/1.1";
      this.headers = new Dictionary<string, string>();
    }

    /// <summary>Initializes a new HTTP server response</summary>
    /// <param name="statusCode">
    ///   Status code that will be reported back to the client
    /// </param>
    /// <param name="statusMessage">
    ///   Status message to deliver together with the status code
    /// </param>
    public Response(StatusCode statusCode, string statusMessage)
      : this() {
      this.StatusCode = statusCode;
    }

    /// <summary>Initializes a new HTTP server response</summary>
    /// <param name="statusCode">
    ///   Status code that will be reported back to the client
    /// </param>
    public Response(StatusCode statusCode) : this(statusCode, null) { }

    /// <summary>Status code of the client's request</summary>
    /// <remarks>
    ///   HTTP status codes are designed to be extensible - clients are required to
    ///   understand status codes based on their numeric range (eg. clients will assume
    ///   any status code in the 200-299 range means success). If you want to return
    ///   a custom status code, simply cast any integer to the <see cref="StatusCode" />
    ///   enumeration.
    /// </remarks>
    public StatusCode StatusCode;

    /// <summary>HTTP protocol version used by the server</summary>
    public string Version;

    /// <summary>Message returned to the client together with the status code</summary>
    /// <remarks>
    ///   <para>
    ///     This message is generally not intepreted by a client itself but can be
    ///     shown to the user if the request failed or has an otherwise unexpected
    ///     outcome. You should provide a short but meaningful response telling what
    ///     went wrong and why in case of an error.
    ///   </para>
    ///   <para>
    ///     You can leave this string set to null to have the HTTP server return the
    ///     default status message for known status codes. For custom status codes,
    ///     this will result in the status message being omitted (same as setting it
    ///     to String.Empty).
    ///   </para>
    /// </remarks>
    public string StatusMessage;

    /// <summary>
    ///   Attaches a stream to the response that will be send back to the client</summary>
    /// <param name="stream">Stream the data will be read from</param>
    public void AttachStream(Stream stream) {
      this.attachedStream = stream;
    }

    /// <summary>Headers provided to the client by the server</summary>
    public Dictionary<string, string> Headers {
      get { return this.headers; }
    }

    /// <summary>Stream that has been attached to the response</summary>
    internal Stream AttachedStream {
      get { return this.attachedStream; }
    }

    /// <summary>Headers being returned to the client</summary>
    private Dictionary<string, string> headers;
    /// <summary>The stream attached to the server response</summary>
    private Stream attachedStream;

    // Some responses by widely used HTTP servers for reference until this works
#if false
    private static readonly string googleRedirectResponse =
      "HTTP/1.1 302 Found\r\n" +
      "Location: http://www.google.de/\r\n" +
      "Cache-Control: private\r\n" +
      "Content-Type: text/html; charset=UTF-8\r\n" +
      "Date: Wed, 30 Jul 2008 14:01:06 GMT\r\n" +
      "Server: gws\r\n" +
      "Content-Length: 218\r\n" +
      "\r\n";

    private static readonly string googleUncompressedResponse =
      "HTTP/1.1 200 OK\r\n" +
      "Cache-Control: private\r\n" +
      "Content-Type: text/html; charset=UTF-8\r\n" +
      "Date: Wed, 30 Jul 2008 14:01:06 GMT\r\n" +
      "Server: gws\r\n" +
      "Content-Length: 13\r\n" +
      "\r\n" +
      "Hello World\r\n";

    private static readonly string googleCompressedResponse =
      "HTTP/1.1 200 OK\r\n" +
      "Cache-Control: private, max-age=0\r\n" +
      "Date: Wed, 30 Jul 2008 13:27:24 GMT\r\n" +
      "Expires: -1\r\n" +
      "Content-Type: text/html; charset=UTF-8\r\n" +
      "Content-Encoding: gzip\r\n" +
      "Server: gws\r\n" +
      "Content-Length: 2749\r\n" +
      "\r\n";

    private static readonly string apacheCompressedResponse =
      "HTTP/1.1 200 OK\r\n" +
      "Date: Wed, 30 Jul 2008 13:44:43 GMT\r\n" +
      "Server: Apache\r\n" +
      "X-Powered-By: PHP/5.2.6-pl2-gentoo\r\n" +
      "X-Pingback: http://cygon.nuclex.org/xmlrpc.php\r\n" +
      "Content-Encoding: gzip\r\n" +
      "Vary: Accept-Encoding\r\n" +
      "Keep-Alive: timeout=15, max=100\r\n" +
      "Connection: Keep-Alive\r\n" +
      "Transfer-Encoding: chunked\r\n" +
      "Content-Type: text/html; charset=UTF-8\r\n" +
      "\r\n";

    private static readonly string apacheUncompressedResponse =
      "HTTP/1.1 200 OK\r\n" +
      "Date: Wed, 30 Jul 2008 13:48:03 GMT\r\n" +
      "Server: Apache\r\n" +
      "X-Powered-By: PHP/5.2.6-pl2-gentoo\r\n" +
      "X-Pingback: http://cygon.nuclex.org/xmlrpc.php\r\n" +
      "Keep-Alive: timeout=15, max=100\r\n" +
      "Connection: Keep-Alive\r\n" +
      "Transfer-Encoding: chunked\r\n" +
      "Content-Type: text/html; charset=UTF-8\r\n" +
      "\r\n";
#endif

  }

} // namespace Nuclex.Networking.Http
