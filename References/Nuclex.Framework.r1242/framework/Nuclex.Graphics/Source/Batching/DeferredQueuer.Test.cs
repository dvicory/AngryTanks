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

  /// <summary>Unit tests for the deferred queuer</summary>
  [TestFixture]
  internal class DeferredQueuerTest : QueuerTest {

    #region class Creator

    /// <summary>Sets up a test environment for the deferred queuer</summary>
    private class Creator : IDisposable {

      /// <summary>Initializes a new test environment</summary>
      public Creator() {
        this.drawContext = new DummyDrawContext();
        this.batchDrawer = new DummyBatchDrawer();
        this.queuer = new DeferredQueuer<VertexPositionColor>(this.batchDrawer);
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.queuer != null) {
          this.queuer.Dispose();
          this.queuer = null;
        }
        this.batchDrawer = null;
        this.drawContext = null;
      }

      /// <summary>Queues a series of primitives</summary>
      /// <param name="vertices">Primitive vertices</param>
      /// <param name="startVertex">Index of vertex to begin drawing with</param>
      /// <param name="vertexCount">Number of vertices to draw</param>
      /// <param name="type">Type of primitives to draw</param>
      public void Queue(
        TestVertex[] vertices, int startVertex, int vertexCount,
        PrimitiveType type
      ) {
        this.queuer.Queue(vertices, startVertex, vertexCount, type, this.drawContext);
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
      public void Queue(
        TestVertex[] vertices, int startVertex, int vertexCount,
        short[] indices, int startIndex, int indexCount,
        PrimitiveType type
      ) {
        this.queuer.Queue(
          vertices, startVertex, vertexCount, indices, startIndex, indexCount, type,
          this.drawContext
        );
      }

      /// <summary>Informs the queuer that a new drawing cycle is about to start</summary>
      public void Begin() {
        this.queuer.Begin();
      }

      /// <summary>Informs the queuer that the current drawing cycle has ended</summary>
      public void End() {
        this.queuer.End();
      }

      /// <summary>Batches that have been drawn so far</summary>
      public List<List<TestVertex>> DrawnBatches {
        get { return this.batchDrawer.DrawnBatches; }
      }

      /// <summary>Deferred queuer being tested</summary>
      private DeferredQueuer<TestVertex> queuer;
      /// <summary>Dummy batch drawer that records drawn vertices</summary>
      private DummyBatchDrawer batchDrawer;
      /// <summary>Dummy draw context that doesn't do a thing</summary>
      private DummyDrawContext drawContext;

    }

    #endregion // class Creator

    /// <summary>
    ///   Verifies that new instances of the deferred queuer can be created
    /// </summary>
    [Test]
    public void TestConstructor() {
      using(Creator creator = new Creator()) {
        Assert.IsNotNull(creator);
      }
    }

#if !XNA_4
    /// <summary>
    ///   Tests a simple drawing call which doesn't overflow the vertex batch
    /// </summary>
    [Test]
    public void TestSimpleDrawing() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(TestVertices, 0, 9, PrimitiveType.TriangleList);
          creator.Queue(TestVertices, 9, 3, PrimitiveType.PointList);
        }
        finally {
          creator.End();
        }

        Assert.AreEqual(subArray(TestVertices, 0, 9), creator.DrawnBatches[0]);
        Assert.AreEqual(subArray(TestVertices, 9, 3), creator.DrawnBatches[1]);
      }
    }
#endif

#if !XNA_4
    /// <summary>
    ///   Tests a point list drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestPointListBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(TestVertices, 0, 9, PrimitiveType.PointList);
          creator.Queue(TestVertices, 9, 30, PrimitiveType.PointList);
        }
        finally {
          creator.End();
        }

        Assert.AreEqual(subArray(TestVertices, 0, 16), creator.DrawnBatches[0]);
        Assert.AreEqual(subArray(TestVertices, 16, 16), creator.DrawnBatches[1]);
        Assert.AreEqual(subArray(TestVertices, 32, 7), creator.DrawnBatches[2]);
      }
    }
#endif

    /// <summary>
    ///   Tests a line strip drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestLineStripBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          TestVertex[] test = subArray(TestVertices, 0, 39);
          creator.Queue(TestVertices, 0, 9, PrimitiveType.LineStrip);
          creator.Queue(test, 9, 30, PrimitiveType.LineStrip);
        }
        finally {
          creator.End();
        }

        // First line strip with 9 supporting points
        Assert.AreEqual(subArray(TestVertices, 0, 9), creator.DrawnBatches[0]);
        // Second line strip, first batch
        Assert.AreEqual(subArray(TestVertices, 9, 7), creator.DrawnBatches[1]);
        // Second line strip, second batch, repeating cutoff vertex
        Assert.AreEqual(subArray(TestVertices, 15, 16), creator.DrawnBatches[2]);
        // Second line strip, third batch, repeating cutoff vertex
        Assert.AreEqual(subArray(TestVertices, 30, 9), creator.DrawnBatches[3]);
      }
    }

    /// <summary>
    ///   Tests a line strip drawing call which overflows the vertex batch size
    ///   and where no vertices can be placed in the first batch.
    /// </summary>
    [Test]
    public void TestLineStripBatchOverflowSkip() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          TestVertex[] test = subArray(TestVertices, 0, 31);
          creator.Queue(TestVertices, 0, 15, PrimitiveType.LineStrip);
          creator.Queue(test, 15, 16, PrimitiveType.LineStrip);
        }
        finally {
          creator.End();
        }

        // First line strip with 15 supporting points
        Assert.AreEqual(subArray(TestVertices, 0, 15), creator.DrawnBatches[0]);
        // Second line strip, should be unsplit in the second batch
        Assert.AreEqual(subArray(TestVertices, 15, 16), creator.DrawnBatches[1]);
      }
    }

    /// <summary>
    ///   Tests a line list drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestLineListBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          TestVertex[] test = subArray(TestVertices, 0, 40);
          creator.Queue(TestVertices, 0, 10, PrimitiveType.LineList);
          creator.Queue(test, 10, 30, PrimitiveType.LineList);
        }
        finally {
          creator.End();
        }

        Assert.AreEqual(subArray(TestVertices, 0, 16), creator.DrawnBatches[0]);
        Assert.AreEqual(subArray(TestVertices, 16, 16), creator.DrawnBatches[1]);
        Assert.AreEqual(subArray(TestVertices, 32, 8), creator.DrawnBatches[2]);
      }
    }

#if !XNA_4
    /// <summary>
    ///   Tests a triangle fan drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestTriangleFanBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          TestVertex[] test = subArray(TestVertices, 0, 39);
          creator.Queue(TestVertices, 0, 14, PrimitiveType.TriangleFan);
          creator.Queue(test, 14, 25, PrimitiveType.TriangleFan);
        }
        finally {
          creator.End();
        }

        // First triangle fan with 14 supporting points
        Assert.AreEqual(subArray(TestVertices, 0, 14), creator.DrawnBatches[0]);
        // Second triangle fan, first batch
        Assert.AreEqual(subArray(TestVertices, 14, 16), creator.DrawnBatches[1]);
        // Second triangle fan, second batch, repeating first and cutoff vertex
        Assert.AreEqual(TestVertices[14], creator.DrawnBatches[2][0]);
        Assert.AreEqual(
          subArray(TestVertices, 29, 10), subArray(creator.DrawnBatches[2].ToArray(), 1)
        );
      }
    }
#endif

    /// <summary>
    ///   Tests a triangle strip drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestTriangleStripBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          TestVertex[] test = subArray(TestVertices, 0, 39);
          creator.Queue(TestVertices, 0, 9, PrimitiveType.TriangleStrip);
          creator.Queue(test, 9, 30, PrimitiveType.TriangleStrip);
        }
        finally {
          creator.End();
        }

        // First triangle strip with 9 supporting points
        Assert.AreEqual(subArray(TestVertices, 0, 9), creator.DrawnBatches[0]);
        // Second triangle strip, first batch
        Assert.AreEqual(subArray(TestVertices, 9, 7), creator.DrawnBatches[1]);
        // Second triangle strip, second batch, repeating two vertices before cutoff
        Assert.AreEqual(subArray(TestVertices, 14, 15), creator.DrawnBatches[2]);
        // Second triangle strip, third batch, repeating two vertices before cutoff
        Assert.AreEqual(subArray(TestVertices, 27, 12), creator.DrawnBatches[3]);
      }
    }

    /// <summary>
    ///   Tests a triangle strip drawing call which overflows the vertex batch size
    ///   with the first batch too full to hold the beginning of the strip
    /// </summary>
    [Test]
    public void TestTriangleStripBatchOverflowSkip() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          TestVertex[] test = subArray(TestVertices, 0, 39);
          creator.Queue(TestVertices, 0, 13, PrimitiveType.TriangleStrip);
          creator.Queue(test, 13, 26, PrimitiveType.TriangleStrip);
        }
        finally {
          creator.End();
        }

        // First triangle strip with 13 supporting points
        Assert.AreEqual(subArray(TestVertices, 0, 13), creator.DrawnBatches[0]);
        // Second triangle strip, first batch
        Assert.AreEqual(subArray(TestVertices, 13, 15), creator.DrawnBatches[1]);
        // Second triangle strip, second batch, repeating two vertices before cutoff
        Assert.AreEqual(subArray(TestVertices, 26, 13), creator.DrawnBatches[2]);
      }
    }

    /// <summary>
    ///   Tests a triangle list drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestTriangleListBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          TestVertex[] test = subArray(TestVertices, 0, 39);
          creator.Queue(TestVertices, 0, 15, PrimitiveType.TriangleList);
          creator.Queue(test, 15, 24, PrimitiveType.TriangleList);
        }
        finally {
          creator.End();
        }

        Assert.AreEqual(subArray(TestVertices, 0, 15), creator.DrawnBatches[0]);
        Assert.AreEqual(subArray(TestVertices, 15, 15), creator.DrawnBatches[1]);
        Assert.AreEqual(subArray(TestVertices, 30, 9), creator.DrawnBatches[2]);
      }
    }

#if !XNA_4
    /// <summary>
    ///   Tests an indexed drawing call which doesn't overflow the vertex batch
    /// </summary>
    [Test]
    public void TestIndexedDrawing() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 10, 2 + 0 + 9, TestIndices, 0, 9, PrimitiveType.TriangleList
          );
          // - Indices start at 2 ("index[0] == 2")
          // - First index used is index 9 (so vertices 11 - 14 will be drawn)
          // - Base vertex (offset that becomes vertex index 0) is 10
          // -- Resulting in vertices 21 - 24 to be drawn
          creator.Queue(
            TestVertices, 10, 2 + 9 + 3, TestIndices, 9, 3, PrimitiveType.PointList
          );
        }
        finally {
          creator.End();
        }

        Assert.AreEqual(subArray(TestVertices, 12, 9), creator.DrawnBatches[0]);
        Assert.AreEqual(subArray(TestVertices, 21, 3), creator.DrawnBatches[1]);
      }
    }
#endif

#if !XNA_4
    /// <summary>
    ///   Tests an indexed point list drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestIndexedPointListBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 10, 2 + 0 + 9, TestIndices, 0, 9, PrimitiveType.PointList
          );
          // - Indices start at 2 ("index[0] == 2")
          // - First index used is index 9 (so vertices 11 - 41 will be drawn)
          // - Base vertex (offset that becomes vertex index 0) is 10
          // -- Resulting in vertices 21 - 51 to be drawn
          creator.Queue(
            TestVertices, 10, 2 + 9 + 30, TestIndices, 9, 30, PrimitiveType.PointList
          );
        }
        finally {
          creator.End();
        }

        TestVertex[] firstBatch = creator.DrawnBatches[0].ToArray();
        Assert.AreEqual(subArray(TestVertices, 12, 9), subArray(firstBatch, 0, 9));
        Assert.AreEqual(subArray(TestVertices, 21, 7), subArray(firstBatch, 9, 7));
        Assert.AreEqual(subArray(TestVertices, 28, 16), creator.DrawnBatches[1]);
        Assert.AreEqual(subArray(TestVertices, 44, 7), creator.DrawnBatches[2]);
      }
    }
#endif

    /// <summary>
    ///   Tests an indexed line strip drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestIndexedLineStripBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 10, 2 + 0 + 9, TestIndices, 0, 9, PrimitiveType.LineStrip
          );
          // - Indices start at 2 ("index[0] == 2")
          // - First index used is index 9 (so vertices 11 - 41 will be drawn)
          // - Base vertex (offset that becomes vertex index 0) is 10
          // -- Resulting in vertices 21 - 51 to be drawn
          creator.Queue(
            TestVertices, 10, 2 + 9 + 30, TestIndices, 9, 30, PrimitiveType.LineStrip
          );
        }
        finally {
          creator.End();
        }

        // First line strip with 9 supporting points
        Assert.AreEqual(subArray(TestVertices, 12, 9), creator.DrawnBatches[0]);
        // Second line strip, first batch
        Assert.AreEqual(subArray(TestVertices, 21, 5), creator.DrawnBatches[1]);
        // Second line strip, second batch, repeating cutoff vertex
        Assert.AreEqual(subArray(TestVertices, 25, 16), creator.DrawnBatches[2]);
        // Second line strip, third batch, repeating cutoff vertex
        Assert.AreEqual(subArray(TestVertices, 40, 11), creator.DrawnBatches[3]);
      }
    }

    /// <summary>
    ///   Tests an indexed line strip drawing call which overflows the vertex batch size
    ///   and where no vertices can be placed in the first batch.
    /// </summary>
    [Test]
    public void TestIndexedLineStripBatchOverflowSkip() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 0, 2 + 0 + 13, TestIndices, 0, 13, PrimitiveType.LineStrip
          );
          // - Indices start at 2 ("index[0] == 2")
          // - First index used is index 13 (so vertices 15 - 31 will be drawn)
          // - Base vertex (offset that becomes vertex index 0) is 10
          // -- Resulting in vertices 25 - 41 to be drawn
          creator.Queue(
            TestVertices, 10, 2 + 13 + 16, TestIndices, 13, 16, PrimitiveType.LineStrip
          );
        }
        finally {
          creator.End();
        }

        // First line strip with 15 supporting points
        Assert.AreEqual(subArray(TestVertices, 2, 13), creator.DrawnBatches[0]);
        // Second line strip, should be unsplit in the second batch
        Assert.AreEqual(subArray(TestVertices, 25, 16), creator.DrawnBatches[1]);
      }
    }

    /// <summary>
    ///   Tests a indexed line list drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestIndexedLineListBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 0, 2 + 0 +10, TestIndices, 0, 10, PrimitiveType.LineList
          );
          creator.Queue(
            TestVertices, 10, 2 + 10 + 30, TestIndices, 10, 30, PrimitiveType.LineList
          );
        }
        finally {
          creator.End();
        }

        TestVertex[] firstBatch = creator.DrawnBatches[0].ToArray();

        Assert.AreEqual(subArray(TestVertices, 2, 10), subArray(firstBatch, 0, 10));
        Assert.AreEqual(subArray(TestVertices, 22, 4), subArray(firstBatch, 10));
        Assert.AreEqual(subArray(TestVertices, 26, 16), creator.DrawnBatches[1]);
        Assert.AreEqual(subArray(TestVertices, 42, 10), creator.DrawnBatches[2]);
      }
    }

#if !XNA_4
    /// <summary>
    ///   Tests an indexed triangle fan drawing call which overflows the vertex batch size
    /// </summary>
    [Test]
    public void TestIndexedTriangleFanBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 0, 2 + 0 + 12, TestIndices, 0, 12, PrimitiveType.TriangleFan
          );
          creator.Queue(
            TestVertices, 10, 2 + 14 + 25, TestIndices, 14, 25, PrimitiveType.TriangleFan
          );
        }
        finally {
          creator.End();
        }

        TestVertex[] lastBatch = creator.DrawnBatches[2].ToArray();

        // First triangle fan with 12 supporting points
        Assert.AreEqual(subArray(TestVertices, 0 + 2, 12), creator.DrawnBatches[0]);
        // Second triangle fan, first batch
        Assert.AreEqual(subArray(TestVertices, 24 + 2, 16), creator.DrawnBatches[1]);
        // Second triangle fan, second batch, repeating first and cutoff vertex
        Assert.AreEqual(TestVertices[24 + 2], lastBatch[0]);
        Assert.AreEqual(subArray(TestVertices, 39 + 2, 10), subArray(lastBatch, 1));
      }
    }
#endif

    /// <summary>
    ///   Tests an indexed triangle strip drawing call which overflows
    ///   the vertex batch size
    /// </summary>
    [Test]
    public void TestIndexedTriangleStripBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 0, 7 + 2, TestIndices, 0, 7, PrimitiveType.TriangleStrip
          );
          creator.Queue(
            TestVertices, 10, 30 + 2, TestIndices, 7, 30, PrimitiveType.TriangleStrip
          );
        }
        finally {
          creator.End();
        }

        // First triangle strip with 7 supporting points
        Assert.AreEqual(subArray(TestVertices, 0 + 2, 7), creator.DrawnBatches[0]);
        // Second triangle strip, first batch
        Assert.AreEqual(subArray(TestVertices, 17 + 2, 7), creator.DrawnBatches[1]);
        // Second triangle strip, second batch, repeating two vertices before cutoff
        Assert.AreEqual(subArray(TestVertices, 22 + 2, 15), creator.DrawnBatches[2]);
        // Second triangle strip, third batch, repeating two vertices before cutoff
        Assert.AreEqual(subArray(TestVertices, 35 + 2, 12), creator.DrawnBatches[3]);
      }
    }

    /// <summary>
    ///   Tests an indexed triangle strip drawing call which overflows the vertex batch
    ///   size with the first batch too full to hold the beginning of the strip
    /// </summary>
    [Test]
    public void TestIndexedTriangleStripBatchOverflowSkip() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 0, 11 + 2, TestIndices, 0, 11, PrimitiveType.TriangleStrip
          );
          creator.Queue(
            TestVertices, 10, 26 + 2, TestIndices, 13, 26, PrimitiveType.TriangleStrip
          );
        }
        finally {
          creator.End();
        }

        // First triangle strip with 11 supporting points
        Assert.AreEqual(subArray(TestVertices, 0 + 2, 11), creator.DrawnBatches[0]);
        // Second triangle strip, first batch
        Assert.AreEqual(subArray(TestVertices, 23 + 2, 15), creator.DrawnBatches[1]);
        // Second triangle strip, second batch, repeating two vertices before cutoff
        Assert.AreEqual(subArray(TestVertices, 36 + 2, 13), creator.DrawnBatches[2]);
      }
    }

    /// <summary>
    ///   Tests an indexed triangle list drawing call which overflows
    ///   the vertex batch size
    /// </summary>
    [Test]
    public void TestIndexedTriangleListBatchOverflow() {
      using(Creator creator = new Creator()) {
        creator.Begin();
        try {
          creator.Queue(
            TestVertices, 0, 13 + 2, TestIndices, 0, 13, PrimitiveType.TriangleList
          );
          creator.Queue(
            TestVertices, 10, 24 + 2, TestIndices, 15, 24, PrimitiveType.TriangleList
          );
        }
        finally {
          creator.End();
        }

        Assert.AreEqual(subArray(TestVertices, 0 + 2, 13), creator.DrawnBatches[0]);
        Assert.AreEqual(subArray(TestVertices, 25 + 2, 15), creator.DrawnBatches[1]);
        Assert.AreEqual(subArray(TestVertices, 40 + 2, 9), creator.DrawnBatches[2]);
      }
    }

    /// <summary>Returns a subsection of an array as a new array</summary>
    /// <typeparam name="ElementType">
    ///   Type of the elements being stored in the array
    /// </typeparam>
    /// <param name="array">Array from which a subsection will be extracted</param>
    /// <param name="start">Start index in the array at which extration will begin</param>
    /// <returns>
    ///   An array containing only the elements within the specified range of
    ///   the original array
    /// </returns>
    private static ElementType[] subArray<ElementType>(ElementType[] array, int start) {
      return subArray(array, start, array.Length - start);
    }

    /// <summary>Returns a subsection of an array as a new array</summary>
    /// <typeparam name="ElementType">
    ///   Type of the elements being stored in the array
    /// </typeparam>
    /// <param name="array">Array from which a subsection will be extracted</param>
    /// <param name="start">Start index in the array at which extration will begin</param>
    /// <param name="count">Number of elements that will be extracted</param>
    /// <returns>
    ///   An array containing only the elements within the specified range of
    ///   the original array
    /// </returns>
    private static ElementType[] subArray<ElementType>(
      ElementType[] array, int start, int count
    ) {
      ElementType[] subArray = new ElementType[count];
      Array.Copy(array, start, subArray, 0, count);
      return subArray;
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
