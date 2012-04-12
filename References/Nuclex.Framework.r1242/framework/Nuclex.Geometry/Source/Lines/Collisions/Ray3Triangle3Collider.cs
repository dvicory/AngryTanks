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

  /// <summary>Contains all Ray3 to Triangle3 interference detection code</summary>
  public static class Ray3Triangle3Collider {

    /// <summary>Determines the contact location between a ray and a triangle</summary>
    /// <param name="rayStart">
    ///   Offset of the ray from the coordinate system's center
    /// </param>
    /// <param name="rayDirection">Direction of the line</param>
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
      Vector3 rayStart, Vector3 rayDirection,
      Vector3 triangleA, Vector3 triangleB, Vector3 triangleC
    ) {
      LineContacts contacts = Line3Triangle3Collider.FindContacts(
        rayStart, rayDirection, triangleA, triangleB, triangleC
      );

      // If the line has entered the triangle before the reference point, this means
      // that the ray starts within the triangle and its first contact occurs immediately
      if(!float.IsNaN(contacts.EntryTime)) {
        if(contacts.EntryTime < 0.0f) {
          return LineContacts.None;
        }
      }

      return contacts;
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
