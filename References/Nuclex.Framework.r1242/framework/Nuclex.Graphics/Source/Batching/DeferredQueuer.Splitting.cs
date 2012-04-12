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

  partial class DeferredQueuer<VertexType> {

    // ----------------------------------------------------------------------------------------- //
    // WARNING: Reading this code may be hazardous to your health
    //
    // The two methods below are the code path used to split the vertices of drawing commands
    // that run over the end of the vertex batch. They are optimized, very well tested and
    // completely unreadable.
    // 
    // Proceed at your own risk!
    // ----------------------------------------------------------------------------------------- //

    /// <summary>
    ///   Queues the provided vertices for deferred rendering, doing one or more
    ///   intermediate flushes when the vertex batch is full
    /// </summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">
    ///   Index in the vertex array of the first vertex. This vertex will become
    ///   the new index 0 for the index buffer.
    /// </param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    /// <param name="spaceLeft">Amount of space left in the current batch</param>
    /// <remarks>
    ///   This is a special optimized method for adding vertices when the amount of
    ///   vertices to render does not exceed available batch space, which should be
    ///   the default usage of a vertex batcher.
    /// </remarks>
    private void queueVerticesBufferSplit(
      VertexType[] vertices, int startVertex, int vertexCount,
      PrimitiveType type, DrawContext context,
      int spaceLeft
    ) {
      int fanStartVertex = startVertex; // Start vertex if we're doing a triangle fan

      // Special processing for the first run. Here we can be sure that the vertex
      // batch will become full and we can fiddle with some offsets so the second loop
      // doesn't have to check whether it's still in the first loop all the time.
      {
        switch(type) {
          case PrimitiveType.LineStrip: {
            if(spaceLeft < 2) {
              spaceLeft = 0;
              ++startVertex; // Because the second loop will add this
              --vertexCount;
            }
            break;
          }
          case PrimitiveType.LineList: {
            spaceLeft -= spaceLeft % 2; // Can be split each 2 vertices
            break;
          }
#if !XNA_4
          case PrimitiveType.TriangleFan: {
            if(spaceLeft < 3) {
              spaceLeft = 0;
              startVertex += 2; // Because the second loop will add these
              vertexCount -= 2;
            }
            break;
          }
#endif
          case PrimitiveType.TriangleStrip: {
            if(spaceLeft < 4) {
              spaceLeft = 0;
              startVertex += 2; // Because the second loop will add these
              vertexCount -= 2;
            } else {
              // Triangle strips could be split anywere, but after each triangle,
              // theculling mode is flipped. So we need to make sure we split them
              // at an even index, otherwise the next batch has flipped culling.
              spaceLeft -= (spaceLeft - 1) % 2;
            }
            break;
          }
          case PrimitiveType.TriangleList: {
            spaceLeft -= spaceLeft % 3; // Can be split each 3 vertices
            break;
          }
        } // Switch

        // Was there enough space in the batch to add some of our vertices?
        if(spaceLeft > 0) {
          createNewOperationIfNecessary(type, context);

          Array.Copy(
            vertices, startVertex,
            this.vertices, this.usedVertexCount,
            spaceLeft
          );

          // Generate indices for the new vertices. This might not be faster than using
          // a special non-indexed RenderOperation, but it yields stable speeds between
          // indexed and non-indexed vertices and allows us to lessen the impact
          // the worst-case scenario will have.
          int endIndex = this.currentOperation.VertexCount + spaceLeft;
          for(int index = this.currentOperation.VertexCount; index < endIndex; ++index) {
            this.indices[this.usedIndexCount] = (short)index;
            ++this.usedIndexCount;
          }
          this.usedVertexCount += spaceLeft;

          // Update the end index for the current operation
          this.currentOperation.VertexCount += spaceLeft;
          this.currentOperation.EndIndex = this.usedIndexCount;

          vertexCount -= spaceLeft;
          startVertex += spaceLeft;
        }

        flush();
        reset();
      } // Beauty scope

      while(vertexCount >= 0) {
        spaceLeft = this.BatchDrawer.MaximumBatchSize;

        createNewOperationIfNecessary(type, context);

        switch(type) {
          case PrimitiveType.LineStrip: {
            this.vertices[0] = vertices[startVertex - 1];
            this.indices[0] = 0;

            --spaceLeft;
            ++this.usedVertexCount;
            ++this.usedIndexCount;
            ++this.currentOperation.VertexCount;

            break;
          }
          case PrimitiveType.LineList: {
            spaceLeft -= spaceLeft % 2;

            break;
          }
#if !XNA_4
          case PrimitiveType.TriangleFan: {
            this.vertices[0] = vertices[fanStartVertex];
            this.indices[0] = 0;
            this.vertices[1] = vertices[startVertex - 1];
            this.indices[1] = 1;

            spaceLeft -= 2;
            this.usedIndexCount += 2;
            this.usedVertexCount += 2;
            this.currentOperation.VertexCount += 2;

            break;
          }
#endif
          case PrimitiveType.TriangleStrip: {
            this.vertices[0] = vertices[startVertex - 2];
            this.indices[0] = 0;
            this.vertices[1] = vertices[startVertex - 1];
            this.indices[1] = 1;

            spaceLeft -= (spaceLeft - 1) % 2 + 2;
            this.usedIndexCount += 2;
            this.usedVertexCount += 2;
            this.currentOperation.VertexCount += 2;

            break;
          }
          case PrimitiveType.TriangleList: {
            spaceLeft -= spaceLeft % 3;
            break;
          }
        } // switch

        int verticesToProcess = Math.Min(spaceLeft, vertexCount);

        Array.Copy(
          vertices, startVertex,
          this.vertices, this.usedVertexCount,
          verticesToProcess
        );

        // Generate indices for the new vertices. This might not be faster than using
        // a special non-indexed RenderOperation, but it yields stable speeds between
        // indexed and non-indexed vertices and allows us to lessen the impact
        // the worst-case scenario will have.
        int endIndex = this.currentOperation.VertexCount + verticesToProcess;
        for(int index = this.currentOperation.VertexCount; index < endIndex; ++index) {
          this.indices[this.usedIndexCount] = (short)index;
          ++this.usedIndexCount;
        }
        this.usedVertexCount += verticesToProcess;

        // Update the end index for the current operation
        this.currentOperation.VertexCount = this.usedVertexCount;
        this.currentOperation.EndIndex = this.usedIndexCount;

        vertexCount -= verticesToProcess;
        if(vertexCount == 0) {
          break; // This batch is not full yet, keep adding to it
        }

        flush();
        reset();

        startVertex += verticesToProcess;
      }
    }

#if !XNA_4
    /// <summary>
    ///   Queues the provided vertices as point list for deferred rendering
    /// </summary>
    /// <param name="vertices">Primitive vertices</param>
    /// <param name="startVertex">
    ///   Index in the vertex array of the first vertex. This vertex will become
    ///   the new index 0 for the index buffer.
    /// </param>
    /// <param name="vertexCount">Number of vertices to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    /// <param name="spaceLeft">Amount of space left in the current batch</param>
    private void queuePointVerticesBufferSplit(
      VertexType[] vertices, int startVertex, int vertexCount,
      DrawContext context,
      int spaceLeft
    ) {
      for(;;) {
        createNewOperationIfNecessary(PrimitiveType.PointList, context);

        int endIndex = startVertex + spaceLeft;
        for(int index = startVertex; index < endIndex; ++index) {
          this.vertices[this.usedVertexCount] = vertices[index];
          ++this.usedVertexCount;
        }

        // Update the end index for the current operation
        this.currentOperation.VertexCount += spaceLeft;
        startVertex += spaceLeft;
        vertexCount -= spaceLeft;

        if(vertexCount == 0) {
          break;
        }

        spaceLeft = Math.Min(this.BatchDrawer.MaximumBatchSize, vertexCount);

        flush();
        reset();
      }
    }
#endif

    /// <summary>
    ///   Queues the provided vertices for deferred rendering, doing one or more
    ///   intermediate flushes when the vertex batch is full
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
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="context">Desired graphics device settings for the primitives</param>
    /// <param name="spaceLeft">Amount of space left in the current batch</param>
    /// <remarks>
    ///   This is a special optimized method for adding vertices when the amount of
    ///   vertices to render does not exceed available batch space, which should be
    ///   the default usage of a vertex batcher.
    /// </remarks>
    private void queueIndexedVerticesBufferSplit(
      VertexType[] vertices, int startVertex, int vertexCount,
      short[] indices, int startIndex, int indexCount,
      PrimitiveType type, DrawContext context,
      int spaceLeft
    ) {
      // Start vertex if we're doing triangle fans
      int fanStartIndex = indices[startIndex] + startVertex;

      // Special processing for the first run. Here we can be sure that the vertex
      // batch will become full and we can fiddle with some offsets so the second loop
      // doesn't have to check whether it's still in the first loop all the time.
      {
        switch(type) {
          case PrimitiveType.LineStrip: {
            if(spaceLeft < 2) {
              spaceLeft = 0;
              ++startIndex; // Because the second loop will add this
              --indexCount;
            }
            break;
          }
          case PrimitiveType.LineList: {
            spaceLeft -= spaceLeft % 2; // Can be split each 2 vertices
            break;
          }
#if !XNA_4
          case PrimitiveType.TriangleFan: {
            if(spaceLeft < 3) {
              spaceLeft = 0;
              startIndex += 2; // Because the second loop will add these
              indexCount -= 2;
            }
            break;
          }
#endif
          case PrimitiveType.TriangleStrip: {
            if(spaceLeft < 4) {
              spaceLeft = 0;
              startIndex += 2; // Because the second loop will add these
              indexCount -= 2;
            } else {
              // Triangle strips could be split anywere, but after each triangle,
              // theculling mode is flipped. So we need to make sure we split them
              // at an even index, otherwise the next batch has flipped culling.
              spaceLeft -= (spaceLeft - 1) % 2;
            }
            break;
          }
          case PrimitiveType.TriangleList: {
            spaceLeft -= spaceLeft % 3; // Can be split each 3 vertices
            break;
          }
        } // Switch

        if(spaceLeft > 0) {
          createNewOperationIfNecessary(type, context);

          // Generate indices for the new vertices. This might not be faster than using
          // a special non-indexed RenderOperation, but it yields stable speeds between
          // indexed and non-indexed vertices and allows us to lessen the impact
          // the worst-case scenario will have.
          int endIndex = this.currentOperation.VertexCount + spaceLeft;
          for(int index = this.currentOperation.VertexCount; index < endIndex; ++index) {
            this.indices[this.usedIndexCount] = (short)index;
            ++this.usedIndexCount;

            int vertexIndex = indices[startIndex] + startVertex;
            this.vertices[this.usedVertexCount] = vertices[vertexIndex];
            ++this.usedVertexCount;

            ++startIndex;
          }

          // Update the end index for the current operation
          this.currentOperation.VertexCount += spaceLeft;
          this.currentOperation.EndIndex = this.usedIndexCount;

          indexCount -= spaceLeft;
        }

        flush();
        reset();
      } // Beauty scope

      // Was there enough space in the batch to add some of our vertices?
      while(indexCount >= 0) {
        spaceLeft = this.BatchDrawer.MaximumBatchSize;

        createNewOperationIfNecessary(type, context);

        switch(type) {
          case PrimitiveType.LineStrip: {
            this.vertices[0] = vertices[indices[startIndex - 1] + startVertex];
            this.indices[0] = 0;

            --spaceLeft;
            ++this.usedVertexCount;
            ++this.usedIndexCount;
            ++this.currentOperation.VertexCount;

            break;
          }
          case PrimitiveType.LineList: {
            spaceLeft -= spaceLeft % 2;

            break;
          }
#if !XNA_4
          case PrimitiveType.TriangleFan: {
            this.vertices[0] = vertices[fanStartIndex]; // index is already looked up
            this.indices[0] = 0;
            this.vertices[1] = vertices[indices[startIndex - 1] + startVertex];
            this.indices[1] = 1;

            spaceLeft -= 2;
            this.usedIndexCount += 2;
            this.usedVertexCount += 2;
            this.currentOperation.VertexCount += 2;

            break;
          }
#endif
          case PrimitiveType.TriangleStrip: {
            this.vertices[0] = vertices[indices[startIndex - 2] + startVertex];
            this.indices[0] = 0;
            this.vertices[1] = vertices[indices[startIndex - 1] + startVertex];
            this.indices[1] = 1;

            spaceLeft -= (spaceLeft - 1) % 2 + 2;
            this.usedIndexCount += 2;
            this.usedVertexCount += 2;
            this.currentOperation.VertexCount += 2;

            break;
          }
          case PrimitiveType.TriangleList: {
            spaceLeft -= spaceLeft % 3;
            break;
          }
        } // switch

        int indicesToProcess = Math.Min(spaceLeft, indexCount);

        // Generate indices for the new vertices. This might not be faster than using
        // a special non-indexed RenderOperation, but it yields stable speeds between
        // indexed and non-indexed vertices and allows us to lessen the impact
        // the worst-case scenario will have.
        int endIndex = this.currentOperation.VertexCount + indicesToProcess;
        for(int index = this.currentOperation.VertexCount; index < endIndex; ++index) {
          this.indices[this.usedIndexCount] = (short)index;
          ++this.usedIndexCount;

          int vertexIndex = indices[startIndex] + startVertex;
          this.vertices[this.usedVertexCount] = vertices[vertexIndex];
          ++this.usedVertexCount;

          ++startIndex;
        }

        // Update the end index for the current operation
        this.currentOperation.VertexCount = this.usedVertexCount;
        this.currentOperation.EndIndex = this.usedIndexCount;

        indexCount -= indicesToProcess;
        if(indexCount == 0) {
          break; // This batch is not full yet, keep adding to it
        }

        flush();
        reset();
      }
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
    /// <param name="spaceLeft">Amount of space left in the current batch</param>
    private void queueIndexedPointVerticesBufferSplit(
      VertexType[] vertices, int startVertex, int vertexCount,
      short[] indices, int startIndex, int indexCount,
      DrawContext context,
      int spaceLeft
    ) {
      for(;;) {
        createNewOperationIfNecessary(PrimitiveType.PointList, context);

        int endIndex = startIndex + spaceLeft;
        for(int index = startIndex; index < endIndex; ++index) {
          this.vertices[this.usedVertexCount] = vertices[indices[index] + startVertex];
          ++this.usedVertexCount;
        }

        // Update the end index for the current operation
        this.currentOperation.VertexCount += spaceLeft;
        startIndex += spaceLeft;
        indexCount -= spaceLeft;
        
        if(indexCount == 0) {
          break;
        }

        spaceLeft = Math.Min(this.BatchDrawer.MaximumBatchSize, indexCount);

        flush();
        reset();
      }
    }
#endif

  }

} // namespace Nuclex.Graphics.Batching
