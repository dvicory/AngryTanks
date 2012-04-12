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

  /// <summary>
  ///   Detects intersections of infinite 2D lines with 2D discs / circles
  /// </summary>
  public static class Line2Disc2Collider {

    /// <summary>Determines the contact location between a line and a disc</summary>
    /// <param name="lineOffset">
    ///   Offset of the line relative to the disc's center
    /// </param>
    /// <param name="lineDirection">Direction and length of the line</param>
    /// <param name="discRadius">Radius of the disc</param>
    /// <returns>The point of intersection of the line with the disc, if any</returns>
    /// <remarks>
    ///   <para>
    ///     Shamelessly lifted from the FreeMagic library at http://www.magic-software.com
    ///     and used as a supporting function for the other line/sphere contact finders.
    ///   </para>
    /// </remarks>
    internal static LineContacts FindContacts(
      Vector2 lineOffset, Vector2 lineDirection, float discRadius
    ) {
      float a0 = lineOffset.LengthSquared() - discRadius * discRadius;
      float a1 = Vector2.Dot(lineDirection, lineOffset);
      float discrete = a1 * a1 - a0;
      if(discrete > 0.0f) {
        discrete = (float)Math.Sqrt(discrete);

        return new LineContacts(-a1 - discrete, -a1 + discrete);
      } else {
        return LineContacts.None;
      }
    }

    /// <summary>Determines the contact location between a line and a disc</summary>
    /// <param name="lineOffset">
    ///   Offset of the line from the coordinate system's center
    /// </param>
    /// <param name="lineDirection">Direction and length of the line</param>
    /// <param name="discCenter">Position of the disc </param>
    /// <param name="discRadius">Radius of the disc</param>
    /// <returns>The point of intersection of the line with the disc, if any</returns>
    internal static LineContacts FindContacts(
      Vector2 lineOffset, Vector2 lineDirection, Vector2 discCenter, float discRadius
    ) {
      return FindContacts(lineOffset - discCenter, lineDirection, discRadius);
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
