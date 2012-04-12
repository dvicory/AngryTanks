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

  /// <summary>Contains all Line3 to Sphere3 interference detection code</summary>
  public static class Line3Sphere3Collider {

    /// <summary>Determines the points of contact between a line and a sphere</summary>
    /// <param name="lineOffset">
    ///   Offset of the line from the coordinate system's center
    /// </param>
    /// <param name="lineDirection">Vector indicating the line's direction</param>
    /// <param name="sphereRadius">Radius of the sphere</param>
    /// <returns>
    ///   The locations at which the line enters and leaves the sphere, if any
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This variant of the algorithm either returns two contacts or none. There is no
    ///     special case for when the line exactly touches the circumference of the sphere,
    ///     thus producing a single contact. This case will be handled as if no contact
    ///     occured at all.
    ///   </para>
    ///   <para>
    ///     Shamelessly lifted from the FreeMagic library at http://www.magic-software.com
    ///     and used as a supporting function for the other line/sphere contact finders.
    ///   </para>
    /// </remarks>
    public static LineContacts FindContacts(
      Vector3 lineOffset, Vector3 lineDirection, float sphereRadius
    ) {

      // Set up quadratic Q(t) = a*t^2 + 2*b*t + c
      float a = lineDirection.LengthSquared();
      float b = Vector3.Dot(lineOffset, lineDirection);
      float c = lineOffset.LengthSquared() - sphereRadius * sphereRadius;

      float discr = b * b - a * c;
      if(discr <= 0.0f) {
        return LineContacts.None;
      }

      float root = (float)Math.Sqrt(discr);
      return new LineContacts(
        (-b - root) / a,
        (-b + root) / a
      );

    }

    /// <summary>Determines the points of contact between a line and a sphere</summary>
    /// <param name="lineOffset">Offset of the line from the sphere's center</param>
    /// <param name="lineDirection">Vector indicating the line's direction</param>
    /// <param name="sphereCenter">
    ///   Absolute coordinates of the sphere's center point
    /// </param>
    /// <param name="sphereRadius">Radius of the sphere</param>
    /// <returns>
    ///   The locations at which the line enters and leaves the sphere, if any
    /// </returns>
    public static LineContacts FindContacts(
      Vector3 lineOffset, Vector3 lineDirection, Vector3 sphereCenter, float sphereRadius
    ) {
      return FindContacts(
        lineOffset - sphereCenter, lineDirection, sphereRadius
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
