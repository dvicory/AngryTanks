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

#if UNITTEST

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using TestVertex = Microsoft.Xna.Framework.Graphics.VertexPositionColor;

namespace Nuclex.Graphics.Batching {

  /// <summary>Unit tests for the queuer</summary>
  internal class QueuerTest {

    #region class DummyDrawContext

    /// <summary>Drawing context used for the unit test</summary>
    protected class DummyDrawContext : DrawContext {

      /// <summary>Number of passes this draw context requires for rendering</summary>
      public override int Passes {
        get { return 123; }
      }

#if !XNA_4

      /// <summary>Begins the drawing cycle</summary>
      public override void Begin() { }

      /// <summary>Ends the drawing cycle</summary>
      public override void End() { }

      /// <summary>Prepares the graphics device for drawing</summary>
      /// <param name="pass">Index of the pass to begin rendering</param>
      /// <remarks>
      ///   Should only be called between the normal Begin() and End() methods.
      /// </remarks>
      public override void BeginPass(int pass) { }

      /// <summary>Restores the graphics device after drawing has finished</summary>
      /// <remarks>
      ///   Should only be called between the normal Begin() and End() methods.
      /// </remarks>
      public override void EndPass() { }

#else

      /// <summary>Prepares the graphics device for drawing</summary>
      /// <param name="pass">Index of the pass to begin rendering</param>
      /// <remarks>
      ///   Should only be called between the normal Begin() and End() methods.
      /// </remarks>
      public override void Apply(int pass) { }

#endif

      /// <summary>Tests whether another draw context is identical to this one</summary>
      /// <param name="otherContext">Other context to check for equality</param>
      /// <returns>True if the other context is identical to this one</returns>
      public override bool Equals(DrawContext otherContext) {
        return ReferenceEquals(this, otherContext);
      }

    }

    #endregion // class DummyDrawContext

    #region class DummyBatchDrawer

    /// <summary>Dummy drawer for vertex batches</summary>
    protected class DummyBatchDrawer : IBatchDrawer<TestVertex> {

      /// <summary>Initializes as new instance of the vertex drawer</summary>
      public DummyBatchDrawer() {
        this.drawnBatches = new List<List<TestVertex>>();
      }

      /// <summary>
      ///   Maximum number of vertices or indices a single batch is allowed to have
      /// </summary>
      public int MaximumBatchSize {
        get { return 16; }
      }

      /// <summary>Selects the vertices that will be used for drawing</summary>
      /// <param name="vertices">Primitive vertices</param>
      /// <param name="vertexCount">Number of vertices to draw</param>
      /// <param name="indices">Indices of the vertices to draw</param>
      /// <param name="indexCount">Number of vertex indices to draw</param>
      public void Select(
        TestVertex[] vertices, int vertexCount,
        short[] indices, int indexCount
      ) {
        this.vertices = vertices;
        this.indices = indices;
      }

      /// <summary>Selects the vertices that will be used for drawing</summary>
      /// <param name="vertices">Primitive vertices</param>
      /// <param name="vertexCount">Number of vertices to draw</param>
      public void Select(TestVertex[] vertices, int vertexCount) {
        this.vertices = vertices;
      }

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
      public void Draw(
        int startVertex, int vertexCount,
        int startIndex, int indexCount,
        PrimitiveType type, DrawContext context
      ) {
        List<TestVertex> drawnVertices = new List<VertexPositionColor>(indexCount);
        for(int index = startIndex; index < startIndex + indexCount; ++index) {
          int vertexIndex = this.indices[index] + startVertex;
          drawnVertices.Add(this.vertices[vertexIndex]);
        }
        this.drawnBatches.Add(drawnVertices);
      }

      /// <summary>Draws a batch of primitives</summary>
      /// <param name="startVertex">Index of vertex to begin drawing with</param>
      /// <param name="vertexCount">Number of vertices used</param>
      /// <param name="type">Type of primitives that will be drawn</param>
      /// <param name="context">Desired graphics device settings for the primitives</param>
      public void Draw(
        int startVertex, int vertexCount,
        PrimitiveType type, DrawContext context
      ) {
        List<TestVertex> drawnVertices = new List<VertexPositionColor>(vertexCount);
        for(int index = startVertex; index < startVertex + vertexCount; ++index) {
          drawnVertices.Add(this.vertices[index]);
        }
        this.drawnBatches.Add(drawnVertices);
      }

      /// <summary>Batchs that would have been drawn so far</summary>
      public List<List<TestVertex>> DrawnBatches {
        get { return this.drawnBatches; }
      }

      /// <summary>Vertices currently selected for drawing</summary>
      private TestVertex[] vertices;
      /// <summary>Indices currently selected for drawing</summary>
      private short[] indices;
      /// <summary>Records the batches that would have been drawn so far</summary>
      private List<List<TestVertex>> drawnBatches;

    }

    #endregion // class DummyBatchDrawer

    #region class DummyQueuer

    /// <summary>Dummy implementation for testing the Queuer class</summary>
    private class DummyQueuer : Queuer<TestVertex> {

      /// <summary>Initializes a new dummy queuer</summary>
      public DummyQueuer() : base(null) { }

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
        TestVertex[] vertices, int startVertex, int vertexCount,
        short[] indices, int startIndex, int indexCount,
        PrimitiveType type, DrawContext context
      ) { }

      /// <summary>Queues a series of primitives</summary>
      /// <param name="vertices">Primitive vertices</param>
      /// <param name="startVertex">Index of vertex to begin drawing with</param>
      /// <param name="vertexCount">Number of vertices to draw</param>
      /// <param name="type">Type of primitives to draw</param>
      /// <param name="context">Desired graphics device settings for the primitives</param>
      public override void Queue(
        TestVertex[] vertices, int startVertex, int vertexCount,
        PrimitiveType type, DrawContext context
      ) { }

    }

    #endregion // class DummyQueuer

    /// <summary>Static constructor that initializes the test vertex array</summary>
    static QueuerTest() {
      const int TestVertexCount = 64;

      TestVertices = new TestVertex[TestVertexCount];
      TestIndices = new short[TestVertexCount];

      for(int index = 0; index < TestVertexCount; ++index) {
        TestVertices[index] = new TestVertex(
          new Vector3((float)index, (float)index, (float)index), Color.White
        );
        TestIndices[index] = (short)(index + 2);
      }
    }

    /// <summary>Array of vertices being used for testing</summary>
    protected static readonly TestVertex[] TestVertices;
    /// <summary>Array of indices being used for testing, ordered in reverse</summary>
    protected static readonly short[] TestIndices;

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
