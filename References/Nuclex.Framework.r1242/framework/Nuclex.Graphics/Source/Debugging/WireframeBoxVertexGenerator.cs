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

  /// <summary>Generates vertices for a wireframe box</summary>
  internal static class WireframeBoxVertexGenerator {

    /// <summary>Number of vertices this generator produces</summary>
    public const int VertexCount = 24;

    /// <summary>Outputs the vertices for a wireframe box into the specified array</summary>
    /// <param name="vertices">Array to write the box vertices into</param>
    /// <param name="startIndex">Index in the array to begin writing at</param>
    /// <param name="min">Contains the coordinates of the box lesser corner</param>
    /// <param name="max">Contains the coordinates of the box greater corner</param>
    /// <param name="color">Color for the faces of the box</param>
    internal static void Generate(
      VertexPositionColor[] vertices, int startIndex,
      Vector3 min, Vector3 max, Color color
    ) {

      Vector3 rightBottomBack = new Vector3(max.X, min.Y, min.Z);
      Vector3 leftTopBack = new Vector3(min.X, max.Y, min.Z);
      Vector3 leftBottomFront = new Vector3(min.X, min.Y, max.Z);
      Vector3 leftTopFront = new Vector3(min.X, max.Y, max.Z);
      Vector3 rightBottomFront = new Vector3(max.X, min.Y, max.Z);
      Vector3 rightTopBack = new Vector3(max.X, max.Y, min.Z);

      // Line left-top-back to right-top-back
      vertices[startIndex].Position = min;
      vertices[startIndex].Color = color;
      vertices[startIndex + 1].Position = rightBottomBack;
      vertices[startIndex + 1].Color = color;

      // Line left-top-back to left-bottom-back
      vertices[startIndex + 2].Position = min;
      vertices[startIndex + 2].Color = color;
      vertices[startIndex + 3].Position = leftTopBack;
      vertices[startIndex + 3].Color = color;

      // Line left-top-back to left-top-front
      vertices[startIndex + 4].Position = min;
      vertices[startIndex + 4].Color = color;
      vertices[startIndex + 5].Position = leftBottomFront;
      vertices[startIndex + 5].Color = color;

      // Line left-bottom-front to right-bottom-front
      vertices[startIndex + 6].Position = leftTopFront;
      vertices[startIndex + 6].Color = color;
      vertices[startIndex + 7].Position = max;
      vertices[startIndex + 7].Color = color;

      // Line right-top-front to right-bottom-front
      vertices[startIndex + 8].Position = rightBottomFront;
      vertices[startIndex + 8].Color = color;
      vertices[startIndex + 9].Position = max;
      vertices[startIndex + 9].Color = color;

      // Line right-bottom-back to right-bottom-front
      vertices[startIndex + 10].Position = rightTopBack;
      vertices[startIndex + 10].Color = color;
      vertices[startIndex + 11].Position = max;
      vertices[startIndex + 11].Color = color;

      // Line left-top-front to right-top-front
      vertices[startIndex + 12].Position = leftBottomFront;
      vertices[startIndex + 12].Color = color;
      vertices[startIndex + 13].Position = rightBottomFront;
      vertices[startIndex + 13].Color = color;

      // Line right-top-back to right-top-front
      vertices[startIndex + 14].Position = rightBottomBack;
      vertices[startIndex + 14].Color = color;
      vertices[startIndex + 15].Position = rightBottomFront;
      vertices[startIndex + 15].Color = color;

      // Line left-bottom-back to left-bottom-front
      vertices[startIndex + 16].Position = leftTopFront;
      vertices[startIndex + 16].Color = color;
      vertices[startIndex + 17].Position = leftTopBack;
      vertices[startIndex + 17].Color = color;

      // Line left-bottom-back to right-bottom-back
      vertices[startIndex + 18].Position = rightTopBack;
      vertices[startIndex + 18].Color = color;
      vertices[startIndex + 19].Position = leftTopBack;
      vertices[startIndex + 19].Color = color;

      // Line left-top-front to left-bottom-front
      vertices[startIndex + 20].Position = leftTopFront;
      vertices[startIndex + 20].Color = color;
      vertices[startIndex + 21].Position = leftBottomFront;
      vertices[startIndex + 21].Color = color;

      // Line right-top-back to right-bottom-back
      vertices[startIndex + 22].Position = rightTopBack;
      vertices[startIndex + 22].Color = color;
      vertices[startIndex + 23].Position = rightBottomBack;
      vertices[startIndex + 23].Color = color;

    }

  }

} // namespace Nuclex.Graphics.Debugging
