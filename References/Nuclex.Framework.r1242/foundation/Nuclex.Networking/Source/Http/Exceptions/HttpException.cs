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

namespace Nuclex.Networking.Http.Exceptions {

  /// <summary>Base class for exceptions indicating HTTP errors</summary>
  /// <remarks>
  ///   Please do not use this class directly, it is only intended to server as a base
  ///   class for specialized exceptions matching the HTTP error status codes.
  /// </remarks>
  [Serializable]
  public class HttpException : Exception {

    /// <summary>Initializes the exception</summary>
    /// <param name="statusCode">HTTP status code to provide with the exception</param>
    protected HttpException(StatusCode statusCode) {
      this.statusCode = statusCode;
    }

    /// <summary>Initializes the exception with an error message</summary>
    /// <param name="statusCode">HTTP status code to provide with the exception</param>
    /// <param name="message">Error message describing the cause of the exception</param>
    protected HttpException(StatusCode statusCode, string message) :
      base(message) {
      this.statusCode = statusCode;
    }

    /// <summary>Initializes the exception as a followup exception</summary>
    /// <param name="statusCode">HTTP status code to provide with the exception</param>
    /// <param name="message">Error message describing the cause of the exception</param>
    /// <param name="inner">Preceding exception that has caused this exception</param>
    protected HttpException(StatusCode statusCode, string message, Exception inner) :
      base(message, inner) {
      this.statusCode = statusCode;
    }

    /// <summary>Initializes the exception from its serialized state</summary>
    /// <param name="info">Contains the serialized fields of the exception</param>
    /// <param name="context">Additional environmental informations</param>
    protected HttpException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context
    ) :
      base(info, context) {

      this.statusCode = (StatusCode)info.GetValue("statusCode", typeof(StatusCode));
    }

    /// <summary>
    ///   Provides the System.Runtime.Serialization.SerializationInfo instance
    ///   with information about the exception
    /// </summary>
    /// <param name="info">
    ///   The System.Runtime.Serialization.SerializationInfo instance that holds
    ///   the serialized object data about the exception being thrown
    /// </param>
    /// <param name="context">
    ///   The System.Runtime.Serialization.StreamingContext instance that contains
    ///   contextual information about the source or destination.
    /// </param>
    public override void GetObjectData(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context
    ) {
      base.GetObjectData(info, context);

      info.AddValue("statusCode", this.statusCode, typeof(StatusCode));
    }

    /// <summary>
    ///   HTTP status code that indicates the problem reported by the exception
    /// </summary>
    public StatusCode StatusCode {
      get { return this.statusCode; }
    }

    /// <summary>HTTP status code for the problem reported by the exception</summary>
    private StatusCode statusCode;

  }

} // namespace Nuclex.Networking.Http.Exceptions
