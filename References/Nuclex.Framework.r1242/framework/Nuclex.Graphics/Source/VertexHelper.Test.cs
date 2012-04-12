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
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

namespace Nuclex.Graphics {

  /// <summary>Unit tests for the vertex helper functions</summary>
  [TestFixture]
  internal class VertexHelperTest {

#if !XNA_4
    /// <summary>
    ///   Verifies that the primitive count for a point list is correctly calculated
    /// </summary>
    [Test]
    public void TestPointListPrimitiveCount() {
      Assert.AreEqual(
        123,
        VertexHelper.GetPrimitiveCount(123, PrimitiveType.PointList)
      );
    }
#endif

    /// <summary>
    ///   Verifies that the primitive count for a line strip is correctly calculated
    /// </summary>
    [Test]
    public void TestLineStripPrimitiveCount() {
      Assert.AreEqual(
        122,
        VertexHelper.GetPrimitiveCount(123, PrimitiveType.LineStrip)
      );
    }

    /// <summary>
    ///   Verifies that the primitive count for a line list is correctly calculated
    /// </summary>
    [Test]
    public void TestLineListPrimitiveCount() {
      Assert.AreEqual(
        62,
        VertexHelper.GetPrimitiveCount(124, PrimitiveType.LineList)
      );
    }

#if !XNA_4
    /// <summary>
    ///   Verifies that the primitive count for a triangle fan is correctly calculated
    /// </summary>
    [Test]
    public void TestTriangleFanPrimitiveCount() {
      Assert.AreEqual(
        121,
        VertexHelper.GetPrimitiveCount(123, PrimitiveType.TriangleFan)
      );
    }
#endif

    /// <summary>
    ///   Verifies that the primitive count for a triangle strip is correctly calculated
    /// </summary>
    [Test]
    public void TestTriangleStripPrimitiveCount() {
      Assert.AreEqual(
        121,
        VertexHelper.GetPrimitiveCount(123, PrimitiveType.TriangleStrip)
      );
    }

    /// <summary>
    ///   Verifies that the primitive count for a triangle strip is correctly calculated
    /// </summary>
    [Test]
    public void TestTriangleListPrimitiveCount() {
      Assert.AreEqual(
        41,
        VertexHelper.GetPrimitiveCount(123, PrimitiveType.TriangleList)
      );
    }

    /// <summary>
    ///   Tests whether passing an invalid primitive type to the GetPrimitiveCount()
    ///   method causes it to throw the right exception
    /// </summary>
    [Test]
    public void TestInvalidTypePrimitiveCount() {
      Assert.Throws<ArgumentException>(
        delegate() {
          VertexHelper.GetPrimitiveCount(123, (PrimitiveType)(-1));
        }
      );
    }

#if !XNA_4
    /// <summary>
    ///   Verifies that the helper can determine whether a vertex count is valid
    ///   for a point list
    /// </summary>
    [Test]
    public void TestIsValidVertexCountWithPointList() {
      Assert.IsTrue(VertexHelper.IsValidVertexCount(123, PrimitiveType.PointList));
    }
#endif

    /// <summary>
    ///   Verifies that the helper can determine whether a vertex count is valid
    ///   for a line strip
    /// </summary>
    [Test]
    public void TestIsValidVertexCountWithLineStrip() {
      Assert.IsFalse(VertexHelper.IsValidVertexCount(1, PrimitiveType.LineStrip));
      Assert.IsTrue(VertexHelper.IsValidVertexCount(123, PrimitiveType.LineStrip));
    }

    /// <summary>
    ///   Verifies that the helper can determine whether a vertex count is valid
    ///   for a line list
    /// </summary>
    [Test]
    public void TestIsValidVertexCountWithLineList() {
      Assert.IsFalse(VertexHelper.IsValidVertexCount(123, PrimitiveType.LineList));
      Assert.IsTrue(VertexHelper.IsValidVertexCount(124, PrimitiveType.LineList));
    }

#if !XNA_4
    /// <summary>
    ///   Verifies that the helper can determine whether a vertex count is valid
    ///   for a triangle fan
    /// </summary>
    [Test]
    public void TestIsValidVertexCountWithTriangleFan() {
      Assert.IsFalse(VertexHelper.IsValidVertexCount(2, PrimitiveType.TriangleFan));
      Assert.IsTrue(VertexHelper.IsValidVertexCount(123, PrimitiveType.TriangleFan));
    }
#endif

    /// <summary>
    ///   Verifies that the helper can determine whether a vertex count is valid
    ///   for a triangle strip
    /// </summary>
    [Test]
    public void TestIsValidVertexCountWithTriangleStrip() {
      Assert.IsFalse(VertexHelper.IsValidVertexCount(2, PrimitiveType.TriangleStrip));
      Assert.IsTrue(VertexHelper.IsValidVertexCount(123, PrimitiveType.TriangleStrip));
    }

    /// <summary>
    ///   Verifies that the helper can determine whether a vertex count is valid
    ///   for a triangle list
    /// </summary>
    [Test]
    public void TestIsValidVertexCountWithTriangleList() {
      Assert.IsFalse(VertexHelper.IsValidVertexCount(122, PrimitiveType.TriangleList));
      Assert.IsTrue(VertexHelper.IsValidVertexCount(123, PrimitiveType.TriangleList));
    }

    /// <summary>
    ///   Tests whether passing an invalid primitive type to the IsValidVertexCount()
    ///   method causes it to throw the right exception
    /// </summary>
    [Test]
    public void TestIsValidVertexCountWithInvalidPrimitiveType() {
      Assert.Throws<ArgumentException>(
        delegate() {
          VertexHelper.IsValidVertexCount(123, (PrimitiveType)(-1));
        }
      );
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST