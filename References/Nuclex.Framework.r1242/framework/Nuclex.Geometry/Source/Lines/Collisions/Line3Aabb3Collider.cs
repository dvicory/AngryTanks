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

  /// <summary>Contains all Line3 to Aabb3 interference detection code</summary>
  public static class Line3Aabb3Collider {

    /// <summary>Determines the contact times of a line with an axis aligned box</summary>
    /// <param name="lineOffset">Offset of the line from the box</param>
    /// <param name="lineDirection">Direction into which the line goes</param>
    /// <param name="boxExtents">Extents of the box (half of the box' dimensions)</param>
    /// <returns>The contact points, if any, between the line and the box</returns>
    /// <remarks>
    ///   Shamelessly lifted from the FreeMagic library at http://www.magic-software.com
    ///   and used as a supporting function for the other line/box contact finders.
    /// </remarks>
    public static LineContacts FindContacts(
      Vector3 lineOffset, Vector3 lineDirection, Vector3 boxExtents
    ) {
      float entrytime = float.MinValue;
      float exitTime = float.MaxValue;

      // Determine the instants of entry and exit into the box, if any
      bool notEntirelyClipped =
        clip(+lineDirection.X, -lineOffset.X - boxExtents.X, ref entrytime, ref exitTime) &&
        clip(-lineDirection.X, +lineOffset.X - boxExtents.X, ref entrytime, ref exitTime) &&
        clip(+lineDirection.Y, -lineOffset.Y - boxExtents.Y, ref entrytime, ref exitTime) &&
        clip(-lineDirection.Y, +lineOffset.Y - boxExtents.Y, ref entrytime, ref exitTime) &&
        clip(+lineDirection.Z, -lineOffset.Z - boxExtents.Z, ref entrytime, ref exitTime) &&
        clip(-lineDirection.Z, +lineOffset.Z - boxExtents.Z, ref entrytime, ref exitTime);

      // Find out if an intersection with the box has actually occured
      bool intersects =
        notEntirelyClipped &&
        (entrytime != float.MinValue || exitTime != float.MaxValue);

      if(intersects) {
        return new LineContacts(entrytime, exitTime);
      } else {
        return LineContacts.None;
      }
    }

    /// <summary>Determines the contact times of a line with an axis aligned box</summary>
    /// <param name="lineOffset">Offset of the line from the box</param>
    /// <param name="lineDirection">Direction into which the line goes</param>
    /// <param name="minBoxCorner">Corner of the box with the lesser coordinates</param>
    /// <param name="maxBoxCorner">Corner of the box with the greater coordinates</param>
    /// <returns>The contact points, if any, between the line and the box</returns>
    public static LineContacts FindContacts(
      Vector3 lineOffset, Vector3 lineDirection,
      Vector3 minBoxCorner, Vector3 maxBoxCorner
    ) {
      Vector3 boxExtents = (maxBoxCorner - minBoxCorner) / 2.0f;
      Vector3 boxOffset = minBoxCorner + boxExtents;

      return FindContacts(
        lineOffset - boxOffset, lineDirection, boxExtents
      );
    }

    /// <summary>Determines where a line will intersect with a normalized plane</summary>
    /// <param name="denominator">Denominator of the line's direction towards the plane</param>
    /// <param name="numerator">Numerator of the line's direction towards the plane</param>
    /// <param name="entryTime">Time of entry into the plane</param>
    /// <param name="exitTime">Time of exit from the plane</param>
    /// <returns>True if the line segment actually intersects with the plane</returns>
    /// <remarks>
    ///   Shamelessly lifted from the FreeMagic library at http://www.magic-software.com
    ///   and used as a supporting function for the other line/box contact finders.
    /// </remarks>
    private static bool clip(
      float denominator, float numerator,
      ref float entryTime, ref float exitTime
    ) {
      if(denominator > 0.0f) {
        if(numerator > denominator * exitTime)
          return false;

        if(numerator > denominator * entryTime)
          entryTime = numerator / denominator;

        return true;

      } else if(denominator < 0.0f) {
        if(numerator > denominator * entryTime)
          return false;

        if(numerator > denominator * exitTime)
          exitTime = numerator / denominator;

        return true;

      } else {
        return numerator <= 0.0f;
      }
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
