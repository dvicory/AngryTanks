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

using Nuclex.Networking.Exceptions;

namespace Nuclex.Audio.Metadata.Exceptions {

  /// <summary>
  ///   Thrown to indicate that the requested database entry was notfound
  /// </summary>
  [Serializable]
  public class EntryNotFoundException : CddbException {

    /// <summary>Status code this exception is generated for</summary>
    public const int Code = 401;

    /// <summary>Initializes the exception</summary>
    public EntryNotFoundException() :
      base(Code, "Specified CDDB entry not found") { }

    /// <summary>Initializes the exception with an error message</summary>
    /// <param name="message">Error message describing the cause of the exception</param>
    public EntryNotFoundException(string message) :
      base(Code, message) { }

    /// <summary>Initializes the exception as a followup exception</summary>
    /// <param name="message">Error message describing the cause of the exception</param>
    /// <param name="inner">Preceding exception that has caused this exception</param>
    public EntryNotFoundException(string message, Exception inner) :
      base(Code, message, inner) { }

    /// <summary>Initializes the exception from its serialized state</summary>
    /// <param name="info">Contains the serialized fields of the exception</param>
    /// <param name="context">Additional environmental informations</param>
    protected EntryNotFoundException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context
    ) :
      base(info, context) { }

  }

} // namespace Nuclex.Audio.Metadata.Exceptions
