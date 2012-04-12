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

  /// <summary>Segment of a line (Typical line with starting and ending location)</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Segment3 : ILine3 {

    /// <summary>Initializes a new line segment</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public Segment3() {
      Start = Vector3.Zero;
      End = Vector3.Zero;
    }

    /// <summary>Constructs a new line as copy of an existing instance</summary>
    /// <param name="other">Existing instance to copy</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Segment3(Segment3 other) {
      Start = other.Start;
      End = other.End;
    }

    /// <summary>Initializes a new line segment</summary>
    /// <param name="start">Starting location of the line segment</param>
    /// <param name="end">Ending location of the line segment</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Segment3(Vector3 start, Vector3 end) {
      this.Start = start;
      this.End = end;
    }

    /// <summary>Locates the nearest point on the line to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point on the line to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {

      // If the line's start and end location are identical we would be dividing by
      // zero further down, so we'll handle this case seperately
      if(this.Start == this.End)
        return Start;

      // Calculate the position of an orthogonal vector on the line pointing
      // towards the location that the caller specified 
      Vector3 direction = Vector3.Normalize(End - Start);
      float position = Vector3.Dot(location - Start, direction);

      // Clip the position onto the length of the line segment
      return this.Start + direction * Math.Min(Math.Max(position, 0.0f), 1.0f);

    }

    /// <summary>The length of the line</summary>
    public float Length {
      get { return (this.End - this.Start).Length(); }
    }

    /// <summary>Checks two segment instances for inequality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if the instances differ or exactly one reference is set to null</returns>
    public static bool operator !=(Segment3 first, Segment3 second) {
      return !(first == second);
    }

    /// <summary>Checks two segment instances for equality</summary>
    /// <param name="first">First instance to be compared</param>
    /// <param name="second">Second instance fo tbe compared</param>
    /// <returns>True if both instances are equal or both references are null</returns>
    public static bool operator ==(Segment3 first, Segment3 second) {
      if(ReferenceEquals(first, null))
        return ReferenceEquals(second, null);

      return first.Equals(second);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public override bool Equals(object other) {
      return Equals(other as Segment3);
    }

    /// <summary>Checks whether another instance is equal to this instance</summary>
    /// <param name="other">Other instance to compare to this instance</param>
    /// <returns>True if the other instance is equal to this instance</returns>
    public virtual bool Equals(Segment3 other) {
      if(ReferenceEquals(other, null))
        return false;
      else
        return (this.Start == other.Start) && (this.End == other.End);
    }

    /// <summary>Obtains a hash code of this instance</summary>
    /// <returns>The hash code of the instance</returns>
    public override int GetHashCode() {
      unchecked { return Start.GetHashCode() + End.GetHashCode(); }
    }

    /// <summary>Determines where the range clips a sphere</summary>
    /// <param name="sphere">Sphere that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(Sphere3 sphere) {
      throw new NotImplementedException();
      /*
      return filterContacts(
        Collisions.Line3Sphere3Collider.FindContacts(
          this.Start, this.End - this.Start, sphere.Center, sphere.Radius
        )
      );
      */
    }

    /// <summary>Determines where the range clips an axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(AxisAlignedBox3 box) {
      throw new NotImplementedException();
      /*
      return filterContacts(
        Collisions.Line3Aabb3Collider.FindContacts(
          this.Start - box.Center, this.End - this.Start, box.Dimensions / 2.0f
        )
      );
      */
    }

    /// <summary>Determines where the range clips a box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    public LineContacts FindContacts(Box3 box) {
      throw new NotImplementedException();
      /*
            // Convert line to box coordinates
            Vector3 offset = Start - box.Center;
            Vector3 relativePosition = new Vector3(
              Vector3.Dot(offset, box.Transform.Right),
              Vector3.Dot(offset, box.Transform.Up),
              Vector3.Dot(offset, box.Transform.Forward)
            );

            Vector3 direction = End - Start;
            Vector3 relativeDirection = new Vector3(
              Vector3.Dot(direction, box.Transform.Right),
              Vector3.Dot(direction, box.Transform.Up),
              Vector3.Dot(direction, box.Transform.Forward)
            );

            return filterContacts(
              Collisions.Line3Aabb3Collider.FindContacts(
                relativePosition, relativeDirection, box.Dimensions / 2.0f
              )
            );
      */
    }

    /// <summary>Determines where the range clips a plane</summary>
    /// <param name="plane">Plane that will be checked for intersection</param>
    /// <returns>The times at which the range touches the plane, if at all</returns>
    public LineContacts FindContacts(Plane3 plane) {
      LineContacts contacts = Collisions.Line3Plane3Collider.FindContacts(
        Start, End - Start, plane.Offset, plane.Normal
      );
      limitContactToLineSegment(ref contacts);

      return contacts;
    }


    /// <summary>Determines where the range clips a triangle</summary>
    /// <param name="triangle">Triangle that will be checked for intersection</param>
    /// <returns>The times at which the range touches the triangle, if at all</returns>
    public LineContacts FindContacts(Triangle3 triangle) {
      LineContacts contacts = Collisions.Line3Triangle3Collider.FindContacts(
        Start, End - Start, triangle.A, triangle.B, triangle.C
      );
      limitContactToLineSegment(ref contacts);

      return contacts;
    }

    /// <summary>Determines where the range clips an axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    /// <remarks>
    ///   Taken from the article "Simple Intersection Tests for Games" on
    ///   Gamasutra by Gomez et al.
    /// </remarks>
    public bool Intersects(AxisAlignedBox3 box) {
      Vector3 lineDirection = (End - Start);
      float lineHalfLength = lineDirection.Length();
      lineDirection /= lineHalfLength;
      lineHalfLength /= 2.0f;

      // ALGORITHM: Use the separating axis theorem to see if the line segment and
      //            the box overlap. A line segment is a degenerate OBB.

      Vector3 distance = box.Center - (Start + End) / 2.0f;
      float r;

      // Do any of the principal axes form a separating axis?
      Vector3 scaledDimensions = new Vector3(
        box.Dimensions.X * lineHalfLength * Math.Abs(lineDirection.X),
        box.Dimensions.Y * lineHalfLength * Math.Abs(lineDirection.Y),
        box.Dimensions.Z * lineHalfLength * Math.Abs(lineDirection.Z)
      );

      if(Math.Abs(distance.X) > scaledDimensions.X)
        return false;

      if(Math.Abs(distance.Y) > scaledDimensions.Y)
        return false;

      if(Math.Abs(distance.Z) > scaledDimensions.Z)
        return false;

      // NOTE: Since the separating axis is perpendicular to the line in these
      //       last four cases, the line does not contribute to the projection.

      //l.cross(x-axis)?

      r =
        box.Dimensions.Y * Math.Abs(lineDirection.Z) +
        box.Dimensions.Z * Math.Abs(lineDirection.Y);

      if(Math.Abs(distance.Y * lineDirection.Z - distance.Z * lineDirection.Y) > r)
        return false;

      //l.cross(y-axis)?

      r =
        box.Dimensions.X * Math.Abs(lineDirection.Z) +
        box.Dimensions.Z * Math.Abs(lineDirection.X);

      if(Math.Abs(distance.Z * lineDirection.X - distance.X * lineDirection.Z) > r)
        return false;

      //l.cross(z-axis)?

      r =
        box.Dimensions.X * Math.Abs(lineDirection.Y) +
        box.Dimensions.Y * Math.Abs(lineDirection.X);

      if(Math.Abs(distance.X * lineDirection.Y - distance.Y * lineDirection.X) > r)
        return false;

      return true;
    }

    /// <summary>
    ///   Limits the contact positions found in a line to the subsection of
    ///   the line covered by the line segment
    /// </summary>
    /// <param name="contacts">Contacts that will be limited to the line segment</param>
    private static void limitContactToLineSegment(ref LineContacts contacts) {
      if(!float.IsNaN(contacts.EntryTime)) {
        bool contactLiesOutsideOfSegment =
          (contacts.EntryTime > 1.0f) ||
          (contacts.ExitTime < 0.0f);

        if(contactLiesOutsideOfSegment) {
          contacts.EntryTime = float.NaN;
          contacts.ExitTime = float.NaN;
        } else {
          if(contacts.EntryTime < 0.0f) {
            contacts.EntryTime = 0.0f;
          }
          if(contacts.ExitTime > 1.0f) {
            contacts.ExitTime = 1.0f;
          }
        }
      }
    }

    /// <summary>Filters the contacts of a line contact query for the segment</summary>
    /// <param name="lineContacts">Contacts that will be filtered</param>
    /// <returns>The filtered contact list</returns>
    private float[] filterContacts(float[] lineContacts) {
      // TODO: This is just another variant of limitContactToLineSegment()
      //       Remove when switching everything to the ContactPoints structure
      if(lineContacts != null) {

        // If the leaving contact lies before the beginning of the segment,
        // there is no intersection
        if(lineContacts[1] < 0.0f)
          return null;

        // If the entering contact lies after the end of the segment,
        // there is no intersection
        if(lineContacts[0] > 1.0f)
          return null;

        // Otherwise, if the volume was entered before the beginning of the segment,
        // we need to adjust the first contact time since the segment begins within the volume
        if(lineContacts[0] < 0.0f)
          lineContacts[0] = 0.0f;

        // If the volume was left after the end of the segment, the segment ends
        // inside the volume and the exit time needs to be adjusted
        if(lineContacts[1] > 1.0f)
          lineContacts[1] = 1.0f;

      }

      return lineContacts;
    }

    /// <summary>The starting point of the line segment</summary>
    public Vector3 Start;
    /// <summary>The ending point of the line segment</summary>
    public Vector3 End;
  }

} // namespace Nuclex.Geometry.Ranges
