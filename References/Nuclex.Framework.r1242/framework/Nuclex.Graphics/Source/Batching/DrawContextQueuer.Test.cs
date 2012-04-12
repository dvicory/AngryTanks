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

  /// <summary>Unit tests for the draw context queuer</summary>
  [TestFixture]
  internal class DrawContextQueuerTest : QueuerTest {

    /// <summary>
    ///   Verifies that new instances of the draw context queuer can be created
    /// </summary>
    [Test]
    public void TestConstructor() {
      DummyBatchDrawer batchDrawer = new DummyBatchDrawer();
      using(
        DrawContextQueuer<TestVertex> queuer = new DrawContextQueuer<TestVertex>(
          batchDrawer
        )
      ) { }
    }

#if !XNA_4
    /// <summary>
    ///   Tests a simple drawing call which doesn't overflow the vertex batch
    /// </summary>
    [Test]
    public void TestSimpleDrawing() {
      DummyBatchDrawer batchDrawer = new DummyBatchDrawer();
      using(
        DrawContextQueuer<TestVertex> queuer = new DrawContextQueuer<TestVertex>(
          batchDrawer
        )
      ) {
        DummyDrawContext context = new DummyDrawContext();

        queuer.Begin();
        try {
          queuer.Queue(TestVertices, 0, 9, PrimitiveType.TriangleList, context);
          queuer.Queue(TestVertices, 9, 3, PrimitiveType.PointList, context);
        }
        finally {
          queuer.End();
        }
      }
      
      // TODO: Check results
    }
#endif

#if !XNA_4
    /// <summary>
    ///   Tests a drawing call that uses the index buffer
    /// </summary>
    [Test]
    public void TestIndexedDrawing() {
      DummyBatchDrawer batchDrawer = new DummyBatchDrawer();
      using(
        DrawContextQueuer<TestVertex> queuer = new DrawContextQueuer<TestVertex>(
          batchDrawer
        )
      ) {
        DummyDrawContext context = new DummyDrawContext();

        queuer.Begin();
        try {
          queuer.Queue(
            TestVertices, 0, 9,
            TestIndices, 0, 9,
            PrimitiveType.TriangleList, context
          );
          queuer.Queue(
            TestVertices, 9, 3,
            TestIndices, 9, 3,
            PrimitiveType.PointList, context
          );
        }
        finally {
          queuer.End();
        }
      }

      // TODO: Check results
    }
#endif

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
