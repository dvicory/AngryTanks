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

using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics.Batching {

  /// <summary>
  ///   Draws batches of primitives using the most efficient method available
  ///   on the platform the game is running on
  /// </summary>
  internal interface IBatchDrawer<VertexType> where VertexType : struct
#if XNA_4
  , IVertexType
#endif
 {

    /// <summary>
    ///   Maximum number of vertices or indices a single batch is allowed to have
    /// </summary>
    /// <remarks>
    ///   This value must not change once the batch drawer is passed to a queuer
    ///   since the queuers will size the vertex arrays according to this number.
    /// </remarks>
    int MaximumBatchSize { get; }

    /// <summary>Selects the vertices that will be used for drawing</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="indices">Indices of the vertices to draw</param>
    /// <param name="indexCount">Number of vertex indices to draw</param>
    void Select(
      VertexType[] vertices, int vertexCount,
      short[] indices, int indexCount
    );

    /// <summary>Selects the vertices that will be used for drawing</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    void Select(
      VertexType[] vertices, int vertexCount
    );

    /// <summary>Draws a batch of indexed primitives</summary>
    /// <param name="startVertex">
    ///   Index of the first vertex in the vertex array. This vertex will become
    ///   the new index 0 for the index buffer.
    /// </param>
    /// <param name="vertexCount">Number of vertices used in the call</param>
    /// <param name="startIndex">
    ///   Position at which to begin processing the index array
    /// </param>
    /// <param name="indexCount">Number of indices that will be processed</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    void Draw(
      int startVertex, int vertexCount,
      int startIndex, int indexCount,
      PrimitiveType type, DrawContext context
    );

    /// <summary>Draws a batch of primitives</summary>
    /// <param name="startVertex">Index of vertex to begin drawing with</param>
    /// <param name="vertexCount">Number of vertices used</param>
    /// <param name="type">Type of primitives that will be drawn</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    void Draw(
      int startVertex, int vertexCount,
      PrimitiveType type, DrawContext context
    );

  }

} // namespace Nuclex.Graphics.Batching
