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

  /// <summary>A Ray from some origin to infinity</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Ray2 : ILine2 {

    /// <summary>Initializes a new ray</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public Ray2() {
      Origin = Vector2.Zero;
      Direction = Vector2.UnitX;
    }

    /// <summary>Constructs a new line as copy of an existing instance</summary>
    /// <param name="other">Existing instance to copy</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Ray2(Ray2 other) {
      Origin = other.Origin;
      Direction = other.Direction;
    }

    /// <summary>Initializes a new ray</summary>
    /// <param name="origin">Location from which the ray originates</param>
    /// <param name="direction">Direction into which the ray goes</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Ray2(Vector2 origin, Vector2 direction) {
      Origin = origin;
      Direction = direction;

      // Make sure the direction is normalized
      Direction = Vector2.Normalize(Direction);
    }

    /// <summary>Determines the closest point on the ray to the specified location</summary>
    /// <param name="location">Random loation to which the closest point is determined</param>
    /// <returns>The closest point within the ray</returns>
    public Vector2 ClosestPointTo(Vector2 location) {

      // Calculate the position of an orthogonal vector on the ray pointing
      // towards the location the caller specified 
      float position = Vector2.Dot(location - Origin, Direction);

      // Clip the position in the negative direction so it can't go before the ray's origin
      return Origin + Direction * Math.Max(position, 0.0f);

    }

    /// <summary>Checks two ray instances for inequality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if the instances differ or exactly one reference is set to null</returns>
    public static bool operator !=(Ray2 first, Ray2 second) {
      return !(first == second);
    }

    /// <summary>Checks two ray instances for equality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if both instances are equal or both references are null</returns>
    public static bool operator ==(Ray2 first, Ray2 second) {
      if(ReferenceEquals(first, null))
        return ReferenceEquals(second, null);

      return first.Equals(second);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public override bool Equals(object other) {
      return Equals(other as Ray2);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public virtual bool Equals(Ray2 other) {
      if(other == null)
        return false;
      else
        return (this.Origin == other.Origin) && (this.Direction == other.Direction);
    }

    /// <summary>Obtains a hash code of this instance</summary>
    /// <returns>The hash code of the instance</returns>
    public override int GetHashCode() {
      unchecked { return Origin.GetHashCode() + Direction.GetHashCode(); }
    }

    /// <summary>Origin of the ray</summary>
    public Vector2 Origin;
    /// <summary>Normalized direction into which the ray goes</summary>
    public Vector2 Direction;

  }

} // namespace Nuclex.Geometry.Ranges
