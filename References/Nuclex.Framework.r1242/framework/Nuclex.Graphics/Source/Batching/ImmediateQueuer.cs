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

  /// <summary>Draws any vertices sent to it immediately</summary>
  /// <remarks>
  ///   This is the simplest possible queuer. It will draw any vertices handed to
  ///   it immediately. This causes a large number of DrawPrimitive() calls, which
  ///   will have a drastic impact on performance (DirectX 9.0 level games are expected
  ///   to have only a few hundred DrawPrimitive calls per frame). It is advisable to
  ///   use this queuer only if you have a very few primitive batches to draw.
  /// </remarks>
  internal class ImmediateQueuer<VertexType> : Queuer<VertexType>
    where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {

    /// <summary>Initializes a new immediate primitive queuer</summary>
    /// <param name="batchDrawer">
    ///   Batch drawer that will be used to render completed vertex batches
    /// </param>
    public ImmediateQueuer(IBatchDrawer<VertexType> batchDrawer) :
      base(batchDrawer) { }

    /// <summary>Queues a series of indexed primitives</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">
    ///   Index in the vertex array of the first vertex. This vertex will become
    ///   the new index 0 for the index buffer.
    /// </param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="indices">Indices of the vertices to draw</param>
    /// <param name="startIndex">Index of the vertex index to begin drawing with</param>
    /// <param name="indexCount">Number of vertex indices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public override void Queue(
      VertexType[] vertices, int startVertex, int vertexCount,
      short[] indices, int startIndex, int indexCount,
      PrimitiveType type, DrawContext context
    ) {
      this.BatchDrawer.Select(vertices, vertexCount, indices, indexCount);
      this.BatchDrawer.Draw(
        startVertex,
        startVertex + vertexCount,
        startIndex,
        indexCount,
        type,
        context
      );
    }

    // TODO: Make the queuer split large vertex lists
    //   Otherwise, it would have different behavior from the deferred queuer when
    //   dealing with vertex lists that are too large to be rendered in a single call

    /// <summary>Queues a series of primitives</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">Index of vertex to begin drawing with</param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public override void Queue(
      VertexType[] vertices, int startVertex, int vertexCount,
      PrimitiveType type, DrawContext context
    ) {
      this.BatchDrawer.Select(vertices, vertexCount);
      this.BatchDrawer.Draw(startVertex, vertexCount, type, context);
    }

  }

} // namespace Nuclex.Graphics.Batching
