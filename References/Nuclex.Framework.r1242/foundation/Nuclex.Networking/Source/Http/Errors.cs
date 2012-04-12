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

  /// <summary>Helper class for generating exceptions relevant to the HTTP server</summary>
  internal static class Errors {

    /// <summary>
    ///   Generates an exception used to report that a request ist malformed
    /// </summary>
    /// <returns>The new exception</returns>
    public static Exception BadRequest() {
      return new Exceptions.BadRequestException();
    }

    /// <summary>
    ///   Generates an exception used to report that a request ist malformed
    /// </summary>
    /// <param name="reason">Reason the server considers the request malformed</param>
    /// <returns>The new exception</returns>
    public static Exception BadRequest(string reason) {
      return new Exceptions.BadRequestException("Bad Request - " + reason);
    }

    /// <summary>
    ///   Generates an exception used to report that a complete request is too large
    /// </summary>
    /// <param name="maximumSize">Maximum request size the server will accept</param>
    /// <returns>The new exception</returns>
    public static Exception RequestEntityTooLarge(int maximumSize) {
      return new Exceptions.RequestEntityTooLargeException(
        string.Format("Message entity exceeds maximum size of {0} bytes", maximumSize)
      );
    }

    /// <summary>
    ///   Generates an exception used to report that a request URI is too large
    /// </summary>
    /// <param name="maximumLength">Maximum URI length the server will accept</param>
    /// <returns>The new exception</returns>
    public static Exception RequestUriTooLong(int maximumLength) {
      return new Exceptions.RequestUriTooLongException(
        string.Format("Request URI exceeds maximum length of {0} bytes", maximumLength)
      );
    }

    /// <summary>
    ///   Generates an exception used to report that the server does not supported
    ///   the version of the protocol used
    /// </summary>
    /// <returns>The new exception</returns>
    public static Exception UnsupportedProtocolVersion() {
      throw new Exceptions.VersionNotSupportedException();
    }

  }

} // namespace Nuclex.Networking.Http
