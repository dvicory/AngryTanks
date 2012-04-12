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
using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Lines {

  /// <summary>Line (extending to infinity on both directions)</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Line2 : ILine2 {

    /// <summary>Constructs a new line</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public Line2() : this(Vector2.Zero, Vector2.Zero) { }

    /// <summary>Constructs a new line as copy of an existing instance</summary>
    /// <param name="other">Existing instance to copy</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Line2(Line2 other) : this(other.Offset, other.Direction) { }

    /// <summary>Initializes a new line</summary>
    /// <param name="offset">Offset of the line from the coordinate system's center</param>
    /// <param name="direction">Vector the defines the direction of the line</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Line2(Vector2 offset, Vector2 direction) {
      this.Offset = offset;
      this.Direction = direction;
    }

    /// <summary>Locates the nearest point on the line to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point on the line to the specified location</returns>
    public Vector2 ClosestPointTo(Vector2 location) {
      Vector2 distance = location - this.Offset;

      float ratio = Vector2.Dot(this.Direction, distance) / this.Direction.LengthSquared();

      return this.Offset + this.Direction * ratio;
    }

    /// <summary>Checks two line instances for inequality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if the instances differ or exactly one reference is set to null</returns>
    public static bool operator !=(Line2 first, Line2 second) {
      return !(first == second);
    }

    /// <summary>Checks two line instances for equality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if both instances are equal or both references are null</returns>
    public static bool operator ==(Line2 first, Line2 second) {
      if(ReferenceEquals(first, null))
        return ReferenceEquals(second, null);

      return first.Equals(second);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public override bool Equals(object other) {
      return Equals(other as Line2);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public virtual bool Equals(Line2 other) {
      if(other == null)
        return false;
      else
        return (this.Offset == other.Offset) && (this.Direction == other.Direction);
    }

    /// <summary>Obtains a hash code of this instance</summary>
    /// <returns>The hash code of the instance</returns>
    public override int GetHashCode() {
      unchecked { return Offset.GetHashCode() + Direction.GetHashCode(); }
    }

    /// <summary>Offset of the line from the coordinate system's center</summary>
    public Vector2 Offset;
    /// <summary>Direction into which the line extends</summary>
    public Vector2 Direction;
  }

} // namespace Nuclex.Geometry.Ranges
