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
using System.Diagnostics;

using Microsoft.Xna.Framework;

using Nuclex.Geometry.Volumes;
using Nuclex.Geometry.Areas;

namespace Nuclex.Geometry.Lines {

  /// <summary>A Ray from some origin to infinity</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Ray3 : ILine3 {

    /// <summary>Initializes a new ray</summary>
    [DebuggerStepThrough]
    public Ray3() {
      Origin = Vector3.Zero;
      Direction = Vector3.Right;
    }

    /// <summary>Constructs a new line as copy of an existing instance</summary>
    /// <param name="other">Existing instance to copy</param>
    [DebuggerStepThrough]
    public Ray3(Ray3 other) {
      Origin = other.Origin;
      Direction = other.Direction;
    }

    /// <summary>Initializes a new ray</summary>
    /// <param name="origin">Location from which the ray originates</param>
    /// <param name="direction">Direction into which the ray goes</param>
    [DebuggerStepThrough]
    public Ray3(Vector3 origin, Vector3 direction) {
      Origin = origin;
      Direction = direction;

      // Make sure the direction is normalized
      Direction = Vector3.Normalize(Direction);
    }

    /// <summary>Determines the closest point on the ray to the specified location</summary>
    /// <param name="location">Random location to which the closest point is determined</param>
    /// <returns>The closest point within the ray</returns>
    public Vector3 ClosestPointTo(Vector3 location) {

      // Calculate the position of an orthogonal vector on the ray pointing
      // towards the location the caller specified 
      float position = Vector3.Dot(location - Origin, Direction);

      // Clip the position in the negative direction so it can't go before the ray's origin
      return Origin + Direction * Math.Max(position, 0.0f);

    }

    /// <summary>Checks two ray instances for inequality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if the instances differ or exactly one reference is set to null</returns>
    public static bool operator !=(Ray3 first, Ray3 second) {
      return !(first == second);
    }

    /// <summary>Checks two ray instances for equality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if both instances are equal or both references are null</returns>
    public static bool operator ==(Ray3 first, Ray3 second) {
      if(ReferenceEquals(first, null))
        return ReferenceEquals(second, null);

      return first.Equals(second);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public override bool Equals(object other) {
      return Equals(other as Ray3);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public virtual bool Equals(Ray3 other) {
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

    /// <summary>Determines where the range clips a sphere</summary>
    /// <param name="sphere">Sphere that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(Sphere3 sphere) {
      return Collisions.Ray3Sphere3Collider.FindContacts(
        Origin, Direction, sphere.Center, sphere.Radius
      );
    }

    /// <summary>Determines where the range clips an axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(AxisAlignedBox3 box) {
      return Collisions.Ray3Aabb3Collider.FindContacts(
        Origin - box.Center, Direction, box.Extents
      );
    }

    /// <summary>Determines where the range clips a box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(Box3 box) {

      // Convert line to box coordinates
      Vector3 offset = Origin - box.Center;

      Vector3 relativePosition = new Vector3(
        Vector3.Dot(offset, box.Transform.Right),
        Vector3.Dot(offset, box.Transform.Up),
        Vector3.Dot(offset, box.Transform.Forward)
      );
      Vector3 relativeDirection = new Vector3(
        Vector3.Dot(Direction, box.Transform.Right),
        Vector3.Dot(Direction, box.Transform.Up),
        Vector3.Dot(Direction, box.Transform.Forward)
      );

      return Collisions.Ray3Aabb3Collider.FindContacts(
        relativePosition, relativeDirection, box.Dimensions / 2.0f
      );

    }

    /// <summary>Determines where the range clips a plane</summary>
    /// <param name="plane">Plane that will be checked for intersection</param>
    /// <returns>The times at which the range touches the plane, if at all</returns>
    public LineContacts FindContacts(Plane3 plane) {
      LineContacts contacts = Collisions.Line3Plane3Collider.FindContacts(
        Origin, Direction, plane.Offset, plane.Normal
      );
      limitContactToRay(ref contacts);
      return contacts;
    }

    /// <summary>Determines where the range clips a triangle</summary>
    /// <param name="triangle">Triangle that will be checked for intersection</param>
    /// <returns>The times at which the range touches the triangle, if at all</returns>
    public LineContacts FindContacts(Triangle3 triangle) {
      LineContacts contacts = Collisions.Line3Triangle3Collider.FindContacts(
        Origin, Direction, triangle.A, triangle.B, triangle.C
      );
      limitContactToRay(ref contacts);
      return contacts;
    }

    /// <summary>
    ///   Limits the contact positions found in a line to the subsection of
    ///   the line covered by the line segment
    /// </summary>
    /// <param name="contacts">Contacts that will be limited to the line segment</param>
    private static void limitContactToRay(ref LineContacts contacts) {
      if(!float.IsNaN(contacts.EntryTime)) {
        bool contactLiesOutsideOfRay =
          (contacts.ExitTime < 0.0f);

        if(contactLiesOutsideOfRay) {
          contacts.EntryTime = float.NaN;
          contacts.ExitTime = float.NaN;
        } else {
          if(contacts.EntryTime < 0.0f) {
            contacts.EntryTime = 0.0f;
          }
        }
      }
    }

    /// <summary>Filters the contacts of a line contact query for the ray</summary>
    /// <param name="lineContacts">Contacts that will be filtered</param>
    /// <returns>The filtered contact list</returns>
    private static float[] filterContacts(float[] lineContacts) {
      if(lineContacts != null) {

        // If the leaving contact lies before the beginning of the ray,
        // there is no intersection
        if(lineContacts[1] < 0.0f)
          return null;

        // Otherwise, if the volume was entered before the beginning of the ray,
        // we need to adjust the first contact time since the ray begins within the volume
        if(lineContacts[0] < 0.0f)
          lineContacts[0] = 0.0f;

      }

      return lineContacts;
    }

    /// <summary>Origin of the ray</summary>
    public Vector3 Origin;
    /// <summary>Normalized direction into which the ray goes</summary>
    public Vector3 Direction;

  }

} // namespace Nuclex.Geometry.Ranges
