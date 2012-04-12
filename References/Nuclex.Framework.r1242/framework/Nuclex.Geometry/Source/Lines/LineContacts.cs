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

namespace Nuclex.Geometry.Lines {

  /// <summary>
  ///   Stores the times of first and last contact determined in an intersection test
  /// </summary>
  public struct LineContacts {

    /// <summary>Initializes a new contact point</summary>
    /// <param name="touchTime">Time the contact was made at</param>
    public LineContacts(float touchTime) :
      this(touchTime, touchTime) { }

    /// <summary>Initializes a new contact point</summary>
    /// <param name="entryTime">Time the first contact was made at</param>
    /// <param name="exitTime">Time the last contact has occured</param>
    public LineContacts(float entryTime, float exitTime) {
      this.EntryTime = entryTime;
      this.ExitTime = exitTime;
    }

    // TEMP!
    /// <summary>Converts a line contact record into a floating point array</summary>
    /// <param name="contacts">Contact record that will be converted</param>
    /// <returns>The resulting floating point array</returns>
    public static implicit operator float[](LineContacts contacts) {
      if(float.IsNaN(contacts.EntryTime)) {
        return null;
      } else {
        return new float[2] { contacts.EntryTime, contacts.ExitTime };
      }
    }

    /// <summary>Whether a contact is stored in the line contacts instance</summary>
    public bool HasContact {
      get { return !float.IsNaN(EntryTime); }
    }

    /// <summary>
    ///   Determines whether this instance is identical to another instance
    /// </summary>
    /// <param name="otherObject">Other instance of compare against</param>
    /// <returns>True if both instances are identical</returns>
    public override bool Equals(object otherObject) {
      if(!(otherObject is LineContacts)) {
        return false;
      }

      LineContacts other = (LineContacts)otherObject;
      return
        (this.EntryTime == other.EntryTime) &&
        (this.ExitTime == other.ExitTime);
    }

    /// <summary>Returns a hash code for the instance</summary>
    /// <returns>The instance's hash code</returns>
    public override int GetHashCode() {
      return this.EntryTime.GetHashCode() ^ this.ExitTime.GetHashCode();
    }

    /// <summary>A line contacts instance for reporting no contacts</summary>
    public static readonly LineContacts None = new LineContacts(float.NaN, float.NaN);

    /// <summary>Time the first contact was made</summary>
    public float EntryTime;
    /// <summary>Time the last contact occurred</summary>
    public float ExitTime;

  }

} // namespace Nuclex.Geometry.Lines
