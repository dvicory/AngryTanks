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

  /// <summary>Generates vertices for a solid box</summary>
  internal static class SolidBoxVertexGenerator {

    /// <summary>Number of vertices this generator produces</summary>
    internal const int VertexCount = 36;

    /// <summary>Outputs the vertices for a solid box into the specified array</summary>
    /// <param name="vertices">Array to write the box vertices into</param>
    /// <param name="startIndex">Index in the array to begin writing at</param>
    /// <param name="min">Contains the coordinates of the box lesser corner</param>
    /// <param name="max">Contains the coordinates of the box greater corner</param>
    /// <param name="color">Color for the faces of the box</param>
    internal static void Generate(
      VertexPositionColor[] vertices, int startIndex,
      Vector3 min, Vector3 max, Color color
    ) {

      // Precalculate the box corners for later use
      Vector3 rightTopBack = new Vector3(max.X, min.Y, min.Z);
      Vector3 leftBottomBack = new Vector3(min.X, max.Y, min.Z);
      Vector3 leftTopFront = new Vector3(min.X, min.Y, max.Z);
      Vector3 leftBottomFront = new Vector3(min.X, max.Y, max.Z);
      Vector3 rightTopFront = new Vector3(max.X, min.Y, max.Z);
      Vector3 rightBottomBack = new Vector3(max.X, max.Y, min.Z);

      // Backside
      vertices[startIndex].Position = rightTopBack;
      vertices[startIndex].Color = color;
      vertices[startIndex + 1].Position = rightBottomBack;
      vertices[startIndex + 1].Color = color;
      vertices[startIndex + 2].Position = leftBottomBack;
      vertices[startIndex + 2].Color = color;
      vertices[startIndex + 3].Position = rightTopBack;
      vertices[startIndex + 3].Color = color;
      vertices[startIndex + 4].Position = leftBottomBack;
      vertices[startIndex + 4].Color = color;
      vertices[startIndex + 5].Position = min;
      vertices[startIndex + 5].Color = color;

      // Left side
      vertices[startIndex + 6].Position = min;
      vertices[startIndex + 6].Color = color;
      vertices[startIndex + 7].Position = leftBottomBack;
      vertices[startIndex + 7].Color = color;
      vertices[startIndex + 8].Position = leftBottomFront;
      vertices[startIndex + 8].Color = color;
      vertices[startIndex + 9].Position = min;
      vertices[startIndex + 9].Color = color;
      vertices[startIndex + 10].Position = leftBottomFront;
      vertices[startIndex + 10].Color = color;
      vertices[startIndex + 11].Position = leftTopFront;
      vertices[startIndex + 11].Color = color;

      // Frontside
      vertices[startIndex + 12].Position = leftTopFront;
      vertices[startIndex + 12].Color = color;
      vertices[startIndex + 13].Position = leftBottomFront;
      vertices[startIndex + 13].Color = color;
      vertices[startIndex + 14].Position = max;
      vertices[startIndex + 14].Color = color;
      vertices[startIndex + 15].Position = leftTopFront;
      vertices[startIndex + 15].Color = color;
      vertices[startIndex + 16].Position = max;
      vertices[startIndex + 16].Color = color;
      vertices[startIndex + 17].Position = rightTopFront;
      vertices[startIndex + 17].Color = color;

      // Right side
      vertices[startIndex + 18].Position = rightTopFront;
      vertices[startIndex + 18].Color = color;
      vertices[startIndex + 19].Position = max;
      vertices[startIndex + 19].Color = color;
      vertices[startIndex + 20].Position = rightBottomBack;
      vertices[startIndex + 20].Color = color;
      vertices[startIndex + 21].Position = rightTopFront;
      vertices[startIndex + 21].Color = color;
      vertices[startIndex + 22].Position = rightBottomBack;
      vertices[startIndex + 22].Color = color;
      vertices[startIndex + 23].Position = rightTopBack;
      vertices[startIndex + 23].Color = color;

      // Upper side
      vertices[startIndex + 24].Position = min;
      vertices[startIndex + 24].Color = color;
      vertices[startIndex + 25].Position = leftTopFront;
      vertices[startIndex + 25].Color = color;
      vertices[startIndex + 26].Position = rightTopFront;
      vertices[startIndex + 26].Color = color;
      vertices[startIndex + 27].Position = min;
      vertices[startIndex + 27].Color = color;
      vertices[startIndex + 28].Position = rightTopFront;
      vertices[startIndex + 28].Color = color;
      vertices[startIndex + 29].Position = rightTopBack;
      vertices[startIndex + 29].Color = color;

      // Lower side
      vertices[startIndex + 30].Position = leftBottomFront;
      vertices[startIndex + 30].Color = color;
      vertices[startIndex + 31].Position = leftBottomBack;
      vertices[startIndex + 31].Color = color;
      vertices[startIndex + 32].Position = rightBottomBack;
      vertices[startIndex + 32].Color = color;
      vertices[startIndex + 33].Position = leftBottomFront;
      vertices[startIndex + 33].Color = color;
      vertices[startIndex + 34].Position = rightBottomBack;
      vertices[startIndex + 34].Color = color;
      vertices[startIndex + 35].Position = max;
      vertices[startIndex + 35].Color = color;

    }

  }

} // namespace Nuclex.Graphics.Debugging
