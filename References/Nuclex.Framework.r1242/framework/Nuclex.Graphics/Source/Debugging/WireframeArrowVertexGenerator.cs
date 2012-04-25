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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Nuclex.Graphics.Debugging {

  /// <summary>Generates vertices for a wireframe arrow</summary>
  internal static class WireframeArrowVertexGenerator {

    /// <summary>Number of vertices this generator produces</summary>
    public const int VertexCount = 10;

    /// <summary>Outputs the vertices for an arrow into the specified array</summary>
    /// <param name="vertices">Array to write the arrow vertices into</param>
    /// <param name="startIndex">Index in the array to begin writing at</param>
    /// <param name="origin">
    ///   Location at which to draw the arrow (this will form the exact center of
    ///   the drawn arrow's base)
    /// </param>
    /// <param name="direction">
    ///   Direction the arrow is pointing into. The arrow's size is relative to
    ///   the length of this vector.
    /// </param>
    /// <param name="color">Color of the lines making up the arrow</param>
    internal static void Generate(
      VertexPositionColor[] vertices, int startIndex,
      Vector3 origin, Vector3 direction, Color color
    ) {

      // Build a vector pointing in an arbitrary direction that is perpendicular to
      // the direction the arrow is pointing at. This can be done by simply juggling
      // the vector elements around by one place.
      Vector3 normalizedDirection = Vector3.Normalize(direction);
      Vector3 up = VectorHelper.GetPerpendicularVector(normalizedDirection);
      Vector3 right = Vector3.Cross(normalizedDirection, up);

      float length = direction.Length();
      up *= length;
      right *= length;

      Vector3 twoThird = origin + (direction * 0.667f);
      Vector3 end = origin + direction;

      // Line origin to arrowhead
      vertices[startIndex].Position = origin;
      vertices[startIndex].Color = color;
      vertices[startIndex + 1].Position = end;
      vertices[startIndex + 1].Color = color;

      // Upper blade on arrowhead
      vertices[startIndex + 2].Position = end;
      vertices[startIndex + 2].Color = color;
      vertices[startIndex + 3].Position = twoThird + up * 0.3f;
      vertices[startIndex + 3].Color = color;

      // Right blade on arrowhead
      vertices[startIndex + 4].Position = end;
      vertices[startIndex + 4].Color = color;
      vertices[startIndex + 5].Position = twoThird + right * 0.3f;
      vertices[startIndex + 5].Color = color;

      // Lower blade on arrowhead
      vertices[startIndex + 6].Position = end;
      vertices[startIndex + 6].Color = color;
      vertices[startIndex + 7].Position = twoThird + up * -0.3f;
      vertices[startIndex + 7].Color = color;

      // Left blade on arrowhead
      vertices[startIndex + 8].Position = end;
      vertices[startIndex + 8].Color = color;
      vertices[startIndex + 9].Position = twoThird + right * -0.3f;
      vertices[startIndex + 9].Color = color;

    }

  }

} // namespace Nuclex.Graphics.Debugging
