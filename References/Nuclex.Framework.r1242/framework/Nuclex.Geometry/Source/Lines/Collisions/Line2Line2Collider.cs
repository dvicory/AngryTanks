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

  /// <summary>Detects intersections between two infinite 2D lines</summary>
  public static class Line2Line2Collider {

    /// <summary>Determines the point of contact between two infinite lines</summary>
    /// <param name="firstOffset">Offset of the first line</param>
    /// <param name="firstDirection">Direction into which the first line extends</param>
    /// <param name="secondOffset">Offset of the second line</param>
    /// <param name="secondDirection">Direction into which the second line extends</param>
    /// <returns>The relative position of the contact on the first line</returns>
    public static LineContacts FindContacts(
      Vector2 firstOffset, Vector2 firstDirection,
      Vector2 secondOffset, Vector2 secondDirection
    ) {
      Vector2 perpendicular = new Vector2(-secondDirection.Y, secondDirection.X);

      float dot = Vector2.Dot(perpendicular, firstDirection);

      // If the dot product is zero, the line is parallel to the plane and no contact occurs
      if(dot == 0.0)
        return LineContacts.None;

      return new LineContacts(
        Vector2.Dot(perpendicular, secondOffset - firstOffset) / dot
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
