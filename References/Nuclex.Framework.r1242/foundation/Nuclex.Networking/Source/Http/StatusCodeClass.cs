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

namespace Nuclex.Networking.Http {

  /// <summary>Classes of status codes that can be returned by an HTTP server</summary>
  public enum StatusCodeClass {

    /// <summary>Unknown status code class</summary>
    Unknown = 0,

    /// <summary>
    ///   This class of status code indicates a provisional response, consisting only of
    ///   the Status-Line and optional headers, and is terminated by an empty line
    /// </summary>
    C1xx_Informational = 100,

    /// <summary>
    ///   This class of status code indicates that the client's request was successfully
    ///   received, understood, and accepted
    /// </summary>
    C2xx_Successful = 200,

    /// <summary>
    ///   This class of status code indicates that further action needs to be taken
    ///   by the user agent in order to fulfill the request
    /// </summary>
    C3xx_Redirection = 300,

    /// <summary>
    ///   The 4xx class of status code is intended for cases in which the client seems
    ///   to have erred
    /// </summary>
    C4xx_Client_Error = 400,

    /// <summary>
    ///   Response status codes beginning with the digit "5" indicate cases in which
    ///   the server is aware that it has erred or is incapable of performing
    ///   the request
    /// </summary>
    C5xx_Server_Error = 500

  }

} // namespace Nuclex.Networking.Http
