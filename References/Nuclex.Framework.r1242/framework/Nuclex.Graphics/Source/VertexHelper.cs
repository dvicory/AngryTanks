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
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics {

  /// <summary>Provides some supporting functions for working with vertices</summary>
  public static class VertexHelper {

    /// <summary>
    ///   Calculates the number of primitives given the number of vertices (or indices)
    ///   and the type of primitives to be drawn.
    /// </summary>
    /// <param name="verticesOrIndices">Number of vertices or indices</param>
    /// <param name="type">Type of primitives the vertices will be processed as</param>
    /// <returns>The number of primitives that can be built from the vertices</returns>
    public static int GetPrimitiveCount(int verticesOrIndices, PrimitiveType type) {
      switch(type) {
#if !XNA_4
        case PrimitiveType.PointList: { return verticesOrIndices; }
#endif
        case PrimitiveType.LineStrip: { return verticesOrIndices - 1; }
        case PrimitiveType.LineList: { return verticesOrIndices / 2; }
#if !XNA_4
        case PrimitiveType.TriangleFan:
#endif
        case PrimitiveType.TriangleStrip: { return verticesOrIndices - 2; }
        case PrimitiveType.TriangleList: { return verticesOrIndices / 3; }
        default: { throw new ArgumentException("Invalid primitive type"); }
      }
    }

    /// <summary>
    ///   Checks whether a vertex count is valid for the specified type of primitives
    /// </summary>
    /// <param name="verticesOrIndices">Number of vertices or indices</param>
    /// <param name="type">Type of primitives the vertices will be processed as</param>
    /// <returns>
    ///   True if the specified number is a valid vertex count for the specified
    ///   type of primitives
    /// </returns>
    /// <remarks>
    ///   A zero check is expected to be done in addition to this method. Negative
    ///   vertex or index counts will result in undefined behavior.
    /// </remarks>
    public static bool IsValidVertexCount(int verticesOrIndices, PrimitiveType type) {
      switch(type) {
#if !XNA_4
        case PrimitiveType.PointList: {
          return true;
        }
#endif
        case PrimitiveType.LineStrip: {
          return (verticesOrIndices >= 2);
        }
        case PrimitiveType.LineList: {
          return ((verticesOrIndices % 2) == 0);
        }
#if !XNA_4
        case PrimitiveType.TriangleFan:
#endif
        case PrimitiveType.TriangleStrip: {
          return (verticesOrIndices >= 3);
        }
        case PrimitiveType.TriangleList: {
          return (verticesOrIndices % 3) == 0;
        }
        default: { throw new ArgumentException("Invalid primitive type"); }
      }
    }

  }

} // namespace Nuclex.Graphics
