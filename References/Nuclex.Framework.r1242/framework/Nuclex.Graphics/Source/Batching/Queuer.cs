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

  /// <summary>Controls the queuing of primitives for the primitive batcher</summary>
  internal abstract class Queuer<VertexType> : IDisposable
    where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {

    /// <summary>Initializes a new primitive queuer</summary>
    /// <param name="batchDrawer">
    ///   Batch drawer that will be used to render completed vertex batches
    /// </param>
    public Queuer(IBatchDrawer<VertexType> batchDrawer) {
      this.BatchDrawer = batchDrawer;
    }

    /// <summary>Immediately releases all resources owned by the queuer</summary>
    public virtual void Dispose() { }

    /// <summary>Informs the queuer that a new drawing cycle is about to start</summary>
    public virtual void Begin() { }
    /// <summary>Informs the queuer that the current drawing cycle has ended</summary>
    public virtual void End() { }

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
    public abstract void Queue(
      VertexType[] vertices, int startVertex, int vertexCount,
      short[] indices, int startIndex, int indexCount,
      PrimitiveType type, DrawContext context
    );

    /// <summary>Queues a series of primitives</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">Index of vertex to begin drawing with</param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public abstract void Queue(
      VertexType[] vertices, int startVertex, int vertexCount,
      PrimitiveType type, DrawContext context
    );

    /// <summary>Vertex batch drawer used to send vertex batches to the GPU</summary>
    protected IBatchDrawer<VertexType> BatchDrawer;

  }

} // namespace Nuclex.Graphics.Batching
