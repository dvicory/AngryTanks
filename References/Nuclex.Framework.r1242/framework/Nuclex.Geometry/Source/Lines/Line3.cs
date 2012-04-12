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

using Nuclex.Geometry.Volumes;
using Nuclex.Geometry.Areas;

namespace Nuclex.Geometry.Lines {

  /// <summary>Line (extending to infinity on both directions)</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Line3 : ILine3 {

    /// <summary>Initializes a new line</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public Line3() {
      Offset = Vector3.Zero;
      Direction = Vector3.Right;
    }

    /// <summary>Constructs a new line as copy of an existing instance</summary>
    /// <param name="other">Existing instance to copy</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Line3(Line3 other) {
      Offset = other.Offset;
      Direction = other.Direction;
    }

    /// <summary>Initializes a new line</summary>
    /// <param name="offset">Offset of the line from the coordinate system's center</param>
    /// <param name="direction">Vector the defines the direction of the line</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Line3(Vector3 offset, Vector3 direction) {
      Offset = offset;
      Direction = direction;
    }

    /// <summary>Locates the nearest point on the line to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point on the line to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {
      Vector3 distance = location - Offset;

      float ratio = Vector3.Dot(Direction, distance) / Direction.LengthSquared();

      return Offset + Direction * ratio;
    }

    /// <summary>Checks two line instances for inequality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if the instances differ or exactly one reference is set to null</returns>
    public static bool operator !=(Line3 first, Line3 second) {
      return !(first == second);
    }

    /// <summary>Checks two line instances for equality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if both instances are equal or both references are null</returns>
    public static bool operator ==(Line3 first, Line3 second) {
      if(ReferenceEquals(first, null))
        return ReferenceEquals(second, null);

      return first.Equals(second);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public override bool Equals(object other) {
      return Equals(other as Line3);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public virtual bool Equals(Line3 other) {
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

    /// <summary>Determines where the range clips a sphere</summary>
    /// <param name="sphere">Sphere that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(Sphere3 sphere) {
      return Collisions.Line3Sphere3Collider.FindContacts(
        Offset, Direction, sphere.Center, sphere.Radius
      );
    }

    /// <summary>Determines where the range clips an axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(AxisAlignedBox3 box) {
      return Collisions.Line3Aabb3Collider.FindContacts(
        Offset - box.Center, Direction, box.Dimensions / 2.0f
      );
    }

    /// <summary>Determines where the range clips a box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(Box3 box) {

      // Convert line to box coordinates
      Vector3 difference = Offset - box.Center;

      Vector3 relativeCenter = new Vector3(
        Vector3.Dot(difference, box.Transform.Right),
        Vector3.Dot(difference, box.Transform.Up),
        Vector3.Dot(difference, box.Transform.Forward)
      );
      Vector3 relativeDirection = new Vector3(
        Vector3.Dot(Direction, box.Transform.Right),
        Vector3.Dot(Direction, box.Transform.Up),
        Vector3.Dot(Direction, box.Transform.Forward)
      );

      return Collisions.Line3Aabb3Collider.FindContacts(
        relativeCenter, relativeDirection, box.Dimensions / 2.0f
      );

    }

    /// <summary>Determines where the range clips a triangle</summary>
    /// <param name="triangle">Triangle that will be checked for intersection</param>
    /// <returns>The times at which the range touches the triangle, if at all</returns>
    public LineContacts FindContacts(Triangle3 triangle) {
      return Collisions.Line3Triangle3Collider.FindContacts(
        Offset, Direction, triangle.A, triangle.B, triangle.C
      );
    }

    /// <summary>Determines where the range clips a plane</summary>
    /// <param name="plane">Plane that will be checked for intersection</param>
    /// <returns>The times at which the range touches the plane, if at all</returns>
    public LineContacts FindContacts(Plane3 plane) {
      return Collisions.Line3Plane3Collider.FindContacts(
        Offset, Direction, plane.Offset, plane.Normal
      );
    }

    /// <summary>Offset of the line from the coordinate system's center</summary>
    public Vector3 Offset;
    /// <summary>Direction into which the line extends</summary>
    public Vector3 Direction;
  }

} // namespace Nuclex.Geometry.Ranges
