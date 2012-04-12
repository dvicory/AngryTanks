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

using Nuclex.Support.Collections;

namespace Nuclex.Networking.Http {

  /// <summary>Stores the informations about an HTTP request</summary>
  public class Request {

    /// <summary></summary>
    enum UriOption {
      /// <summary>Request is targeted at the server as a whole</summary>
      Asterisk,
      /// <summary>Request contains absolute URI (including the host)</summary>
      AbsoluteUri,
      /// <summary>Request only contains the resource path</summary>
      AbsolutePath,
      /// <summary>Request only specifies the host</summary>
      Authority
    }

    /// <summary>Initializes a new HTTP request container</summary>
    /// <param name="method">Request method being used by the client</param>
    /// <param name="uri">URI being accessed by the client</param>
    /// <param name="version">Version of the HTTP protocol being used</param>
    /// <param name="headers">Headers with additional options from the client</param>
    /// <remarks>
    ///   Takes ownership of the headers collection. This collection should not be
    ///   used or modified by the original owner after the constructor has completed.
    ///   If you need to keep the dictionary, pass a cloned dictionary to
    ///   this constructor instead.
    /// </remarks>
    internal Request(
      string method, string uri, string version, IDictionary<string, string> headers
    ) {
      this.method = method;
      this.uri = uri;
      this.version = version;
      this.headers = new ReadOnlyDictionary<string, string>(headers);
    }

    /// <summary>Method of the request</summary>
    /// <remarks>
    ///   Requests can use several "methods" to obtain data from the server. The
    ///   methods defined by HTTP/1.1 are "OPTIONS", "GET", "HEAD", "POST", "PUT",
    ///   "DELETE", "TRACE" and "CONNECT"
    /// </remarks>
    public string Method {
      get { return this.method; }
    }

    /// <summary>URI of the resource the client tries to access</summary>
    public string Uri {
      get { return this.uri; }
    }

    /// <summary>Version number of the HTTP protocol used by the client</summary>
    public string Version {
      get { return this.version; }
    }

    /// <summary>Headers provided with the HTTP request by the client</summary>
    public ReadOnlyDictionary<string, string> Headers {
      get { return this.headers; }
    }
    
    /// <summary>HTTP request method used by the client</summary>
    private string method;
    /// <summary>URI of the resource accessed by the client</summary>
    private string uri;
    /// <summary>Version of the HTTP protocol used</summary>
    private string version;
    /// <summary>
    ///   Headers providing additional informations about the client's preferences
    /// </summary>
    private ReadOnlyDictionary<string, string> headers;

  }

} // namespace Nuclex.Networking.Http
