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

using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Lines.Collisions {

  /// <summary>Contains all Line3 to Triangle3 interference detection code</summary>
  public static class Line3Triangle3Collider {

    /// <summary>Determines the contact location between a line and a triangle</summary>
    /// <param name="lineOffset">
    ///   Offset of the line from the coordinate system's center
    /// </param>
    /// <param name="lineDirection">Direction of the line</param>
    /// <param name="triangleA">
    ///   First corner point of triangle in counter-clockwise order
    /// </param>
    /// <param name="triangleB">
    ///   Second corner point of triangle in counter-clockwise order
    /// </param>
    /// <param name="triangleC">
    ///   Third corner point of triangle in counter-clockwise order
    /// </param>
    /// <returns>The point of intersection of the line with the triangle, if any</returns>
    /// <remarks>
    ///   <para>
    ///     I saw this algorithm in an article to line/triangle intersections tests
    ///     by Christopher Bartlett. The material was stated to be free for learning
    ///     purposes, so I felt free to apply what I've learned here =)
    ///   </para>
    ///   <para>
    ///     There is no special case for when the line precisely touches one of
    ///     the triangle's corners. It will either enter and exit the triangle or
    ///     no contacts will be detected at all.
    ///   </para>
    /// </remarks>
    internal static LineContacts FindContacts(
      Vector3 lineOffset, Vector3 lineDirection,
      Vector3 triangleA, Vector3 triangleB, Vector3 triangleC
    ) {

      // Calculate the normal vector of the triangle for the plane intersection check
      Vector3 ab = triangleB - triangleA;
      Vector3 bc = triangleC - triangleB;
      Vector3 normal = Vector3.Cross(ab, bc);

      // Find out when the line will touch the triangle's plane
      LineContacts contactPoints = Collisions.Line3Plane3Collider.FindContacts(
        lineOffset, lineDirection, triangleA, normal
      );

      if(contactPoints.HasContact) {

        // Calculate the actual point of intersection on the plane
        Vector3 intersectionLocation =
          lineOffset + lineDirection * contactPoints.EntryTime;

        // Now all that's left to do is to find out whether this point is inside the triangle
        bool isInsideTriangle =
          isOnPositiveSide(triangleA, triangleB, intersectionLocation, normal) ==
          isOnPositiveSide(triangleB, triangleC, intersectionLocation, normal) ==
          isOnPositiveSide(triangleC, triangleA, intersectionLocation, normal);

        if(!isInsideTriangle) {
          contactPoints = LineContacts.None;
        }

      }

      return contactPoints;
    }

    /// <summary>Tests whether a point is on the positive side of a line</summary>
    /// <param name="start">Starting point of the line</param>
    /// <param name="end">Ending point of the line</param>
    /// <param name="position">Position to check for the side it is on</param>
    /// <param name="normal">Normal vector of the plane the query takes place on</param>
    /// <returns>True if the point is on the positive side of the line</returns>
    private static bool isOnPositiveSide(
      Vector3 start, Vector3 end, Vector3 position, Vector3 normal
    ) {
      float x =
        ((end.Y - start.Y) * (position.Z - start.Z)) -
        ((position.Y - start.Y) * (end.Z - start.Z));

      float y =
        ((end.Z - start.Z) * (position.X - start.X)) -
        ((position.Z - start.Z) * (end.X - start.X));

      float z =
        ((end.X - start.X) * (position.Y - start.Y)) -
        ((position.X - start.X) * (end.Y - start.Y));

      return ((x * normal.X) + (y * normal.Y) + (z * normal.Z)) >= 0.0f;
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
