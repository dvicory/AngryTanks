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

  /// <summary>Detects intersections between infinite 2D lines and 2D triangles</summary>
  public static class Line2Triangle2Collider {

    /// <summary>Determines the contact location between a line and a triangle</summary>
    /// <param name="lineOffset">
    ///   Offset of the line from the coordinate system's center
    /// </param>
    /// <param name="lineDirection">Direction and length of the line</param>
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
    ///   Everyone seems to know how to do 3D line / triangle intersections, but there
    ///   are no resources whatsoever on 2D line / triangle intersections. The code in here
    ///   is hand-written by myself. Instead of fancy math tricks, it simply tries to be
    ///   efficient using the existing code. It requires 4 line checks to find the accurate
    ///   intersection point with the triangle.
    /// </remarks>
    internal static LineContacts FindContacts(
      Vector2 lineOffset, Vector2 lineDirection,
      Vector2 triangleA, Vector2 triangleB, Vector2 triangleC
    ) {
      Vector2 ab = triangleB - triangleA;
      Vector2 bc = triangleC - triangleB;
      Vector2 ca = triangleA - triangleC;

      float abLength = ab.Length();
      float bcLength = bc.Length();
      float caLength = ca.Length();

      Vector2 abNormalized = ab / abLength;
      Vector2 bcNormalized = bc / bcLength;
      Vector2 caNormalized = ca / caLength;

      LineContacts abContacts = Line2Line2Collider.FindContacts(
        triangleA, abNormalized, lineOffset, lineDirection
      );
      LineContacts bcContacts = Line2Line2Collider.FindContacts(
        triangleB, bcNormalized, lineOffset, lineDirection
      );

      // Does the line cross the A-B line of the triangle?
      if(isWithin(abContacts, abLength)) {

        abContacts = Line2Line2Collider.FindContacts(
          lineOffset, lineDirection, triangleA, ab / abLength
        );

        // Find out which other line it crosses: B-C or C-A?
        if(isWithin(bcContacts, bcLength)) {
          bcContacts = Line2Line2Collider.FindContacts(
            lineOffset, lineDirection, triangleB, bc / bcLength
          );
        } else {
          bcContacts = Line2Line2Collider.FindContacts(
            lineOffset, lineDirection, triangleC, ca / caLength
          );
        }

        // Report the contacts in the right order
        if(abContacts.EntryTime < bcContacts.EntryTime) {
          return new LineContacts(abContacts.EntryTime, bcContacts.EntryTime);
        } else {
          return new LineContacts(bcContacts.EntryTime, abContacts.EntryTime);
        }

      } else if(isWithin(bcContacts, bcLength)) { // Does is cross the B-C line?

        bcContacts = Line2Line2Collider.FindContacts(
          lineOffset, lineDirection, triangleB, bc / abLength
        );

        // We already checked A-B, so the other line it crosses must be C-A!
        abContacts = Line2Line2Collider.FindContacts(
          lineOffset, lineDirection, triangleC, ca / caLength
        );

        // Report the contacts in the right order
        if(bcContacts.EntryTime < abContacts.EntryTime) {
          return new LineContacts(bcContacts.EntryTime, abContacts.EntryTime);
        } else {
          return new LineContacts(abContacts.EntryTime, bcContacts.EntryTime);
        }

      } else { // No contact on A-B or B-C, contact is impossible
        return LineContacts.None;
      }
    }

    /// <summary>
    ///   Finds out whether a reported contact point lies within a line segment
    /// </summary>
    /// <param name="contacts">Reported contact point that will be checked</param>
    /// <param name="length">
    ///   Length of the line segment against which the test that created the contact
    ///   point was made
    /// </param>
    /// <returns>True if the contact point is within the line segment</returns>
    private static bool isWithin(LineContacts contacts, float length) {
      return
        (!float.IsNaN(contacts.EntryTime)) &&
        (contacts.EntryTime >= 0.0f) &&
        (contacts.EntryTime < length);
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
