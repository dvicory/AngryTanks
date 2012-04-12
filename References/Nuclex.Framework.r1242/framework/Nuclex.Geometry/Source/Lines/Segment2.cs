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

  /// <summary>Segment of a line (Typical line with starting and ending location)</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Segment2 : ILine2 {

    /// <summary>Initializes a new line segment</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public Segment2() {
      Start = Vector2.Zero;
      End = Vector2.Zero;
    }

    /// <summary>Constructs a new line as copy of an existing instance</summary>
    /// <param name="other">Existing instance to copy</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Segment2(Segment2 other) {
      Start = other.Start;
      End = other.End;
    }

    /// <summary>Initializes a new line segment</summary>
    /// <param name="start">Starting location of the line segment</param>
    /// <param name="end">Ending location of the line segment</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Segment2(Vector2 start, Vector2 end) {
      Start = start;
      End = end;
    }

    /// <summary>Locates the nearest point on the line to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point on the line to the specified location</returns>
    public Vector2 ClosestPointTo(Vector2 location) {

      // If the line's start and end location are identical we would be dividing by
      // zero further down, so we'll handle this case seperately
      if(Start == End)
        return Start;

      // Calculate the position of an orthogonal vector on the line pointing
      // towards the location that the caller specified 
      Vector2 direction = Vector2.Normalize(End - Start);
      float position = Vector2.Dot(location - Start, direction);

      // Clip the position onto the length of the line segment
      return Start + direction * Math.Min(Math.Max(position, 0.0f), 1.0f);

    }

    /// <summary>Checks two segment instances for inequality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if the instances differ or exactly one reference is set to null</returns>
    public static bool operator !=(Segment2 first, Segment2 second) {
      return !(first == second);
    }

    /// <summary>Checks two segment instances for equality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if both instances are equal or both references are null</returns>
    public static bool operator ==(Segment2 first, Segment2 second) {
      if(ReferenceEquals(first, null))
        return ReferenceEquals(second, null);

      return first.Equals(second);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public override bool Equals(object other) {
      return Equals(other as Segment2);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public virtual bool Equals(Segment2 other) {
      if(other == null)
        return false;
      else
        return (this.Start == other.Start) && (this.End == other.End);
    }

    /// <summary>Obtains a hash code of this instance</summary>
    /// <returns>The hash code of the instance</returns>
    public override int GetHashCode() {
      unchecked { return Start.GetHashCode() + End.GetHashCode(); }
    }

    /// <summary>Locate which side of a line a point is on</summary>
    /// <param name="start">Starting point of a line segment that is interpreted as line</param>
    /// <param name="end">Ending point of a line segment that is interpreted as line</param>
    /// <param name="point">Location which is checked for the side it is on</param>
    /// <returns>1 if it is on the positive side, -1 for negative, 0 for on the line</returns>
    public static Side Orientation(Vector2 start, Vector2 end, Vector2 point) {
      double determinant =
        (end.X - start.X) * (point.Y - start.Y) -
        (point.X - start.X) * (end.Y - start.Y);

      if(determinant >= 0.0)
        return Side.Positive;
      else
        return Side.Negative;
    }

    /// <summary>The starting point of the line segment</summary>
    public Vector2 Start;
    /// <summary>The ending point of the line segment</summary>
    public Vector2 End;
  }

} // namespace Nuclex.Geometry.Ranges
