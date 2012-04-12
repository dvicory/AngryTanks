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

  /// <summary>Collects vertices into batches to improve rendering performance</summary>
  /// <typeparam name="VertexType">Type of vertices to be batched</typeparam>
  /// <remarks>
  ///   This class is very similar to the SpriteBatch class, but instead of being
  ///   specialized for sprite rendering, it handles all kinds of primitives.
  ///   It is ideal for dynamic, CPU-calculated geometry such as particle systems,
  ///   fluid visualization or marching cube/tetrahedron-based geometry.
  /// </remarks>
  public class PrimitiveBatch<VertexType> : IDisposable
    where VertexType : struct
#if XNA_4
    , IVertexType
#endif
 {

    /// <summary>Maximum number of vertices or indices in a single batch</summary>
    public const int BatchSize = 8192;

#if XNA_4

    /// <summary>Initializes a new primitive batcher</summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the primitive batcher will use
    /// </param>
    public PrimitiveBatch(GraphicsDevice graphicsDevice) :
      this(GetDefaultBatchDrawer(graphicsDevice)) { }

#else

    /// <summary>Initializes a new primitive batcher</summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the primitive batcher will use
    /// </param>
    /// <param name="vertexDeclaration">Vertex declaration for the input vertices</param>
    /// <param name="stride">Offset, in bytes, from one vertex to the next</param>
    public PrimitiveBatch(
      GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int stride
    ) :
      this(GetDefaultBatchDrawer(graphicsDevice, vertexDeclaration, stride)) { }

#endif

    /// <summary>Initializes a new primitive batcher</summary>
    /// <param name="batchDrawer">
    ///   Vertex batch drawer that will be used by the primitive batcher
    /// </param>
    internal PrimitiveBatch(IBatchDrawer<VertexType> batchDrawer) {
      this.batchDrawer = batchDrawer;
      this.queueingStrategy = QueueingStrategy.Immediate;
      this.primitiveQueuer = new ImmediateQueuer<VertexType>(this.batchDrawer);
    }

    /// <summary>
    ///   Immediately releases all resources owned by the primitive batch
    /// </summary>
    public void Dispose() {
      if(this.primitiveQueuer != null) {
        this.primitiveQueuer.Dispose();
        this.primitiveQueuer = null;
      }
      if(this.batchDrawer != null) {
        IDisposable disposable = this.batchDrawer as IDisposable;
        if(disposable != null) {
          disposable.Dispose();
        }
        this.batchDrawer = null;
      }
    }

    /// <summary>Begins the drawing process</summary>
    /// <param name="queueingStrategy">
    ///   By what criteria to queue primitives and when to draw them
    /// </param>
    public void Begin(QueueingStrategy queueingStrategy) {

      // If the queueing strategy hast changed from the last frame, we need to
      // build a new queuer for the selected strategy
      if(queueingStrategy != this.queueingStrategy) {
        this.primitiveQueuer.Dispose();

        switch(queueingStrategy) {
          case QueueingStrategy.Immediate: {
            this.primitiveQueuer = new ImmediateQueuer<VertexType>(this.batchDrawer);
            break;
          }
          case QueueingStrategy.Deferred: {
            this.primitiveQueuer = new DeferredQueuer<VertexType>(this.batchDrawer);
            break;
          }
          case QueueingStrategy.Context: {
            this.primitiveQueuer = new DrawContextQueuer<VertexType>(this.batchDrawer);
            break;
          }
          default: {
            throw new ArgumentException(
              "Invalid queueing strategy specified", "queueingStrategy"
            );
          }
        }

        this.queueingStrategy = queueingStrategy;
      }

      // Tell the queuer that a new drawing cycle has started
      this.primitiveQueuer.Begin();

    }

    /// <summary>Ends the drawing process</summary>
    public void End() {

      // Tell the queuer that the current drawing cycle has ended
      this.primitiveQueuer.End();

    }

    /// <summary>Draws a series of triangles</summary>
    /// <param name="vertices">Triangle vertices</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public void Draw(VertexType[] vertices, DrawContext context) {
      this.primitiveQueuer.Queue(
        vertices, 0, vertices.Length, PrimitiveType.TriangleList, context
      );
    }

    /// <summary>Draws a series of primitives</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public void Draw(VertexType[] vertices, PrimitiveType type, DrawContext context) {
      this.primitiveQueuer.Queue(vertices, 0, vertices.Length, type, context);
    }

    /// <summary>Draws a series of primitives</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">Index of vertex to begin drawing with</param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public void Draw(
      VertexType[] vertices, int startVertex, int vertexCount,
      PrimitiveType type, DrawContext context
    ) {
      this.primitiveQueuer.Queue(vertices, 0, vertices.Length, type, context);
    }

    /// <summary>Draws a series of indexed triangles</summary>
    /// <param name="vertices">Triangle vertices</param>
    /// <param name="indices">Indices of the vertices to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public void Draw(VertexType[] vertices, short[] indices, DrawContext context) {
      this.primitiveQueuer.Queue(
        vertices, 0, vertices.Length,
        indices, 0, indices.Length,
        PrimitiveType.TriangleList, context
      );
    }

    /// <summary>Draws a series of indexed primitives</summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="indices">Indices of the vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    public void Draw(
      VertexType[] vertices, short[] indices, PrimitiveType type, DrawContext context
    ) {
      this.primitiveQueuer.Queue(
        vertices, 0, vertices.Length,
        indices, 0, indices.Length,
        type, context
      );
    }

    /// <summary>Draws a series of indexed primitives</summary>
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
    public void Draw(
      VertexType[] vertices, int startVertex, int vertexCount,
      short[] indices, int startIndex, int indexCount,
      PrimitiveType type, DrawContext context
    ) {
      this.primitiveQueuer.Queue(
        vertices, startVertex, vertexCount,
        indices, startIndex, indexCount,
        type, context
      );
    }

#if XNA_4

    /// <summary>Returns the default batch drawer for the target platform</summary>
    /// <param name="graphicsDevice">Graphics device that batch drawer will use</param>
    /// <returns>
    ///   A new instance of the default batch drawer for the target platform
    /// </returns>
    internal static IBatchDrawer<VertexType> GetDefaultBatchDrawer(
      GraphicsDevice graphicsDevice
    ) {
      return new DynamicBufferBatchDrawer<VertexType>(graphicsDevice);
    }

#else

    /// <summary>Returns the default batch drawer for the target platform</summary>
    /// <param name="graphicsDevice">Graphics device that batch drawer will use</param>
    /// <param name="vertexDeclaration">
    ///   Vertex declaration for the vertices being drawn
    /// </param>
    /// <param name="stride">Offset, in bytes, from one vertex to the next</param>
    /// <returns>
    ///   A new instance of the default batch drawer for the target platform
    /// </returns>
    internal static IBatchDrawer<VertexType> GetDefaultBatchDrawer(
      GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int stride
    ) {
#if XBOX360
      return new UserPrimitiveBatchDrawer<VertexType>(
        graphicsDevice, vertexDeclaration
      );
#else
      return new DynamicBufferBatchDrawer<VertexType>(
        graphicsDevice, vertexDeclaration, stride
      );
#endif
    }

#endif

    /// <summary>Sends batches of vertices to the GPU for drawing</summary>
    private IBatchDrawer<VertexType> batchDrawer;
    /// <summary>Controls how primitives are queued (selects queuer)</summary>
    private QueueingStrategy queueingStrategy;
    /// <summary>Queues vertices and initiates rendering when needed</summary>
    private Queuer<VertexType> primitiveQueuer;

  }

} // namespace Nuclex.Graphics.Batching
