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

  /// <summary>Queues vertices until the end of the drawing cycle</summary>
  /// <remarks>
  ///   This queuer respects the order in which drawing commands were given and
  ///   tries to merge any consecutively issued primitives of the same type into
  ///   a single DrawPrimitive() call. This is ideal if you have a large number
  ///   of small objects that are rendered with the same settings (eg. a particle
  ///   system or letters in a bitmap/vector font system).
  /// </remarks>
  internal partial class DeferredQueuer<VertexType> : Queuer<VertexType>
    where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {

    #region class RenderOperation

    /// <summary>Set of primitives that can be drawn in a single run</summary>
    private class RenderOperation {

      /// <summary>Initializes a new render operation</summary>
      /// <param name="startIndex">Starting index of the RenderOperation</param>
      /// <param name="primitiveType">What kind of primitives to draw</param>
      /// <param name="drawContext">Controls the graphics device settings</param>
      public RenderOperation(
        int startIndex, PrimitiveType primitiveType, DrawContext drawContext
      ) {
        this.StartIndex = startIndex;
        this.EndIndex = startIndex;
        this.PrimitiveType = primitiveType;
        this.DrawContext = drawContext;
      }

      /// <summary>First index to draw</summary>
      public int StartIndex;
      /// <summary>Index after the last index to draw</summary>
      public int EndIndex;
      /// <summary>Type of primitives to draw</summary>
      public PrimitiveType PrimitiveType;
      /// <summary>Draw context controlling the graphics device settings</summary>
      public DrawContext DrawContext;
      /// <summary>Base vertex index for the vertex buffer in this operation</summary>
      public int BaseVertexIndex;
      /// <summary>Number of vertices used in the operation</summary>
      public int VertexCount;

    }

    #endregion

    /// <summary>Initializes a new deferred primitive queuer</summary>
    /// <param name="batchDrawer">
    ///   Batch drawer that will be used to render completed vertex batches
    /// </param>
    public DeferredQueuer(IBatchDrawer<VertexType> batchDrawer) :
      base(batchDrawer) {

      // Create a new list of queued rendering operations
      this.operations = new List<RenderOperation>();
      this.currentOperation = new RenderOperation(0, (PrimitiveType)(-1), null);
      this.operations.Add(this.currentOperation);

      // Set up some arrays to store the vertices and indices in
      this.vertices = new VertexType[batchDrawer.MaximumBatchSize];
      this.indices = new short[batchDrawer.MaximumBatchSize];
    }

    /// <summary>Informs the queuer that a new drawing cycle is about to start</summary>
    public override void Begin() {
      reset();
    }

    /// <summary>Informs the queuer that the current drawing cycle has ended</summary>
    public override void End() {
      flush();
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
    ) {
#if !XNA_4
      if(type == PrimitiveType.PointList) {
        int remainingVertices = this.BatchDrawer.MaximumBatchSize - this.usedVertexCount;

        if(vertexCount > remainingVertices) {
          queuePointVerticesBufferSplit(
            vertices, startVertex, vertexCount, context, remainingVertices
          );
        } else {
          queuePointVerticesNoOverflow(
            vertices, startVertex, vertexCount, context
          );
        }
      } else {
#endif
      int remainingSpace = this.BatchDrawer.MaximumBatchSize - Math.Max(
        this.usedVertexCount, this.usedIndexCount
      );

      if(vertexCount > remainingSpace) {
        queueVerticesBufferSplit(
          vertices, startVertex, vertexCount, type, context, remainingSpace
        );
      } else {
        queueVerticesNoOverflow(
          vertices, startVertex, vertexCount, type, context
        );
      }
#if !XNA_4
      }
#endif
    }

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
      int remainingVertices = this.BatchDrawer.MaximumBatchSize - this.usedVertexCount;
#if !XNA_4
      if(type == PrimitiveType.PointList) {
        if(indexCount > remainingVertices) {
          queueIndexedPointVerticesBufferSplit(
            vertices, startVertex, vertexCount,
            indices, startIndex, indexCount,
            context, remainingVertices
          );
        } else {
          queueIndexedPointVerticesNoOverflow(
            vertices, startVertex, vertexCount,
            indices, startIndex, indexCount,
            context
          );
        }

      } else {
#endif
      int remainingIndices = this.BatchDrawer.MaximumBatchSize - this.usedIndexCount;

      bool exceedsBatchSpace =
        (vertexCount > remainingVertices) ||
        (indexCount > remainingIndices);

      if(exceedsBatchSpace) {
        queueIndexedVerticesBufferSplit(
          vertices, startVertex, vertexCount,
          indices, startIndex, indexCount,
          type, context, Math.Min(remainingVertices, remainingIndices)
        );
      } else {
        queueIndexedVerticesNoOverflow(
          vertices, startVertex, vertexCount,
          indices, startIndex, indexCount,
          type, context
        );
      }
#if !XNA_4
      }
#endif
    }

    /// <summary>
    ///   Queues the provided vertices for deferred rendering when there is enough
    ///   space left in the current batch to hold all vertices
    /// </summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">
    ///   Index in the vertex array of the first vertex. This vertex will become
    ///   the new index 0 for the index buffer.
    /// </param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    /// <remarks>
    ///   This is a special optimized method for adding vertices when the amount of
    ///   vertices to render does not exceed available batch space, which should be
    ///   the default usage of a vertex batcher.
    /// </remarks>
    private void queueVerticesNoOverflow(
      VertexType[] vertices, int startVertex, int vertexCount,
      PrimitiveType type, DrawContext context
    ) {
      createNewOperationIfNecessary(type, context);

      // Take over the vertices into our own array
      Array.Copy(
        vertices, startVertex,
        this.vertices, this.usedVertexCount,
        vertexCount
      );

      // Generate indices for the new vertices. This might not be faster than using
      // a special non-indexed RenderOperation, but it yields stable speeds between
      // indexed and non-indexed vertices and to lessens the impact the worst-case
      // has while allowing us to send both types in the same batch.
      int endIndex = this.currentOperation.VertexCount + vertexCount;
      for(int index = this.currentOperation.VertexCount; index < endIndex; ++index) {
        this.indices[this.usedIndexCount] = (short)index;
        ++this.usedIndexCount;
      }
      this.usedVertexCount += vertexCount;

      // Update the end index for the current operation
      this.currentOperation.VertexCount = endIndex;
      this.currentOperation.EndIndex = this.usedIndexCount;
    }

#if !XNA_4
    /// <summary>
    ///   Queues the provided vertices as point list for deferred rendering when there
    ///   is enough space left in the current batch to hold all vertices
    /// </summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">
    ///   Index in the vertex array of the first vertex. This vertex will become
    ///   the new index 0 for the index buffer.
    /// </param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    /// <remarks>
    ///   This is a special optimized method for adding vertices when the amount of
    ///   vertices to render does not exceed available batch space, which should be
    ///   the default usage of a vertex batcher.
    /// </remarks>
    private void queuePointVerticesNoOverflow(
      VertexType[] vertices, int startVertex, int vertexCount, DrawContext context
    ) {
      createNewOperationIfNecessary(PrimitiveType.PointList, context);

      // Take over the vertices into our own array
      Array.Copy(
        vertices, startVertex,
        this.vertices, this.usedVertexCount,
        vertexCount
      );
      this.usedVertexCount += vertexCount;

      // Update the end index for the current operation
      this.currentOperation.VertexCount += vertexCount;
      this.currentOperation.EndIndex = this.usedIndexCount;
    }
#endif

    /// <summary>Queues the provided indexed vertices for deferred rendering</summary>
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
    /// <remarks>
    ///   This is a special optimized method for adding vertices when the amount of
    ///   vertices to render does not exceed available batch space, which should be
    ///   the default usage of a vertex batcher.
    /// </remarks>
    private void queueIndexedVerticesNoOverflow(
      VertexType[] vertices, int startVertex, int vertexCount,
      short[] indices, int startIndex, int indexCount,
      PrimitiveType type, DrawContext context
    ) {
      createNewOperationIfNecessary(type, context);

      // Take over the vertices into our own array
      Array.Copy(
        vertices, startVertex,
        this.vertices, this.usedVertexCount,
        vertexCount
      );

      // Offset that needs to be added to the indices given the subset of vertices
      // we will we cut out and the positioning in our own vertex array
      int indexOffset = this.currentOperation.VertexCount;

      // Take over the indices and adjust them for the new vertex offset
      for(int index = startIndex; index < (startIndex + indexCount); ++index) {
        this.indices[this.usedIndexCount] = (short)(indices[index] + indexOffset);
        ++this.usedIndexCount;
      }

      // Update the end index for the current operation
      this.usedVertexCount += vertexCount;
      this.currentOperation.VertexCount += vertexCount;
      this.currentOperation.EndIndex = this.usedIndexCount;
    }

#if !XNA_4
    /// <summary>
    ///   Queues the provided indexed vertices as point list for deferred rendering
    /// </summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">
    ///   Index in the vertex array of the first vertex. This vertex will become
    ///   the new index 0 for the index buffer.
    /// </param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="indices">Indices of the vertices to draw</param>
    /// <param name="startIndex">Index of the vertex index to begin drawing with</param>
    /// <param name="indexCount">Number of vertex indices to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    /// <remarks>
    ///   This is a special optimized method for adding vertices when the amount of
    ///   vertices to render does not exceed available batch space, which should be
    ///   the default usage of a vertex batcher.
    /// </remarks>
    private void queueIndexedPointVerticesNoOverflow(
      VertexType[] vertices, int startVertex, int vertexCount,
      short[] indices, int startIndex, int indexCount,
      DrawContext context
    ) {
      createNewOperationIfNecessary(PrimitiveType.PointList, context);

      // Take over the indices and adjust them for the new vertex offset
      int endIndex = startIndex + indexCount;
      for(int index = startIndex; index < endIndex; ++index) {
        this.vertices[this.usedVertexCount] = vertices[indices[index] + startVertex];
        ++this.usedVertexCount;
      }

      // Update the end index for the current operation
      this.currentOperation.VertexCount += indexCount;
      this.currentOperation.EndIndex = this.usedIndexCount;
    }
#endif

    /// <summary>Flushes the queued vertices to the graphics card</summary>
    private void flush() {

      // The XNA classes don't like being fed zero values
      if(this.usedVertexCount == 0)
        return;

      if(this.usedIndexCount == 0) {
        this.BatchDrawer.Select(this.vertices, this.usedVertexCount);
      } else {
        this.BatchDrawer.Select(
          this.vertices, this.usedVertexCount,
          this.indices, this.usedIndexCount
        );
      }

      // Draw all queued primitives, one after another
      for(int index = 1; index < this.operations.Count; ++index) {

        // Cache the render operation to keep the following code readable
        RenderOperation operation = this.operations[index];
#if !XNA_4
        if(operation.PrimitiveType == PrimitiveType.PointList) {
          this.BatchDrawer.Draw(
            operation.BaseVertexIndex,
            operation.VertexCount,
            PrimitiveType.PointList,
            operation.DrawContext
          );
        } else {
#endif
        this.BatchDrawer.Draw(
          operation.BaseVertexIndex,
          operation.VertexCount,
          operation.StartIndex,
          operation.EndIndex - operation.StartIndex,
          operation.PrimitiveType,
          operation.DrawContext
        );
#if !XNA_4
        }
#endif

      } // for index

    }

    /// <summary>Resets the internal buffers to the empty state</summary>
    private void reset() {

      // Remove all rendering operations but the first one (which is only a placeholder
      // so we can skip some if-empty checks for the sake of performance).
      if(this.operations.Count > 1) {
        this.operations.RemoveRange(1, this.operations.Count - 1);
        this.currentOperation = this.operations[0];
      }

      // Reset the counters for used vertices and indices
      this.usedVertexCount = 0;
      this.usedIndexCount = 0;

    }

    /// <summary>
    ///   Creates a new rendering operation if the drawing context or primitive type
    ///   have changed since the last call
    /// </summary>
    /// <param name="type">Primitive type of the upcoming vertices</param>
    /// <param name="context">Drawing context used by the upcoming vertices</param>
    private void createNewOperationIfNecessary(PrimitiveType type, DrawContext context) {

      // If the currently running context is not identical to the one this
      // drawing call uses, we need to set up a new RenderOperation
      if(
        (!context.Equals(this.currentOperation.DrawContext)) ||
        (type != this.currentOperation.PrimitiveType) ||
        (this.currentOperation.PrimitiveType == PrimitiveType.LineStrip) ||
#if !XNA_4
        (this.currentOperation.PrimitiveType == PrimitiveType.TriangleFan) ||
#endif
 (this.currentOperation.PrimitiveType == PrimitiveType.TriangleStrip)
      ) {
        this.currentOperation = new RenderOperation(
          this.currentOperation.EndIndex, type, context
        );
        this.currentOperation.BaseVertexIndex = this.usedVertexCount;
        this.operations.Add(this.currentOperation);
      }

    }

    /// <summary>All vertex batches enqueued for rendering so far</summary>
    private List<RenderOperation> operations;
    /// <summary>Cached reference to the current RenderOperation</summary>
    private RenderOperation currentOperation;
    /// <summary>Queued vertices</summary>
    private VertexType[] vertices;
    /// <summary>Number of used vertices in the vertex array</summary>
    private int usedVertexCount;
    /// <summary>Queued indices</summary>
    private short[] indices;
    /// <summary>Number of used indices in the index array</summary>
    private int usedIndexCount;

  }

} // namespace Nuclex.Graphics.Batching
