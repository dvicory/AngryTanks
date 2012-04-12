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

using Nuclex.Support.Collections;

namespace Nuclex.Graphics.Batching {

  /// <summary>
  ///   Queues vertices until the end of the drawing cycle and sorts them
  ///   by their drawing context.
  /// </summary>
  internal class DrawContextQueuer<VertexType> : Queuer<VertexType>
    where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {

    /// <summary>Initializes a new draw context primitive queuer</summary>
    /// <param name="batchDrawer">
    ///   Batch drawer that will be used to render completed vertex batches
    /// </param>
    public DrawContextQueuer(IBatchDrawer<VertexType> batchDrawer) :
      base(batchDrawer) {

      this.contexts = new Dictionary<DrawContext, Deque<VertexType>>();
    }

    /// <summary>Informs the queuer that a new drawing cycle is about to start</summary>
    public override void Begin() { }
    /// <summary>Informs the queuer that the current drawing cycle has ended</summary>
    public override void End() { }

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
      Deque<VertexType> vertexDeque;
      if(!this.contexts.TryGetValue(context, out vertexDeque)) {
        vertexDeque = new Deque<VertexType>();
        this.contexts.Add(context, vertexDeque);
      }

      // TODO: Implement multi-context deferred queueing
    }

    /// <summary>Queues a series of primitives</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">Index of vertex to begin drawing with</param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public override void Queue(
      VertexType[] vertices, int startVertex, int vertexCount,
      PrimitiveType type, DrawContext context
    ) { }

    /// <summary>Encountered drawing contexts and their associated vertices</summary>
    private Dictionary<DrawContext, Deque<VertexType>> contexts;

  }

} // namespace Nuclex.Graphics.Batching
