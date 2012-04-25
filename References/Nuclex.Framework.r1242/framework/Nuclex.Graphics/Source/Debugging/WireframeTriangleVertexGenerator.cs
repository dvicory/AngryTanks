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

  /// <summary>Generates vertices for a wireframe triangle</summary>
  internal static class WireframeTriangleVertexGenerator {

    /// <summary>Number of vertices this generator produces</summary>
    public const int VertexCount = 6;

    /// <summary>
    ///   Outputs the vertices for a wireframe triangle into the specified array
    /// </summary>
    /// <param name="vertices">Array to write the box vertices into</param>
    /// <param name="startIndex">Index in the array to begin writing at</param>
    /// <param name="a">First corner point of the triangle</param>
    /// <param name="b">Second corner point of the triangle</param>
    /// <param name="c">Third corner point of the triangle</param>
    /// <param name="color">Color for the faces of the box</param>
    internal static void Generate(
      VertexPositionColor[] vertices, int startIndex,
      Vector3 a, Vector3 b, Vector3 c, Color color
    ) {

      // Line a to b
      vertices[startIndex].Position = a;
      vertices[startIndex].Color = color;
      vertices[startIndex + 1].Position = b;
      vertices[startIndex + 1].Color = color;

      // Line b to c
      vertices[startIndex + 2].Position = b;
      vertices[startIndex + 2].Color = color;
      vertices[startIndex + 3].Position = c;
      vertices[startIndex + 3].Color = color;

      // Line c to a
      vertices[startIndex + 4].Position = c;
      vertices[startIndex + 4].Color = color;
      vertices[startIndex + 5].Position = a;
      vertices[startIndex + 5].Color = color;

    }

  }

} // namespace Nuclex.Graphics.Debugging
