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

using NUnit.Framework;

namespace Nuclex.Graphics.SpecialEffects {

  /// <summary>Unit tests for the triangle stip index builder class</summary>
  [TestFixture]
  internal class TriangleStripIndexBuilderTest {

    /// <summary>
    ///   Verifies that the number of indices required to draw a triangle strip of
    ///   a fixed size is calculated correctly
    /// </summary>
    [Test]
    public void TestAlternatingStripVertexCount() {
      Assert.AreEqual(
        56, TriangleStripIndexBuilder.CountAlternatingStripIndices(5, 5)
      );
      Assert.AreEqual(
        851, TriangleStripIndexBuilder.CountAlternatingStripIndices(12, 34)
      );
    }

    /// <summary>
    ///   Tests whether an alternating strip of triangle indices can be generated
    ///   for a grid of non-trivial size
    /// </summary>
    [Test]
    public void TestBigAlternatingStripGeneration() {
      short[] expected = {
        0,
        5,   1,  6,  2,  7,  3,  8,  4,  9,
        14,  8, 13,  7, 12,  6, 11,  5, 10,
        15, 11, 16, 12, 17, 13, 18, 14, 19,
        24, 18, 23, 17, 22, 16, 21, 15, 20
      };

      short[] actual = TriangleStripIndexBuilder.BuildAlternatingStrip(4, 4);

      CollectionAssert.AreEqual(actual, expected);
    }

    /// <summary>
    ///   Tests whether an alternating strip of triangle indices can be generated
    ///   for a small grid (9 vertices)
    /// </summary>
    [Test]
    public void TestSmallAlternatingStripGeneration() {
      short[] expected = {
        0,
        3, 1, 4, 2, 5,
        8, 4, 7, 3, 6
      };

      short[] actual = TriangleStripIndexBuilder.BuildAlternatingStrip(2, 2);

      CollectionAssert.AreEqual(actual, expected);
    }

    /// <summary>
    ///   Tests whether an alternating strip of triangle indices can be generated
    ///   for a single quad (4 vertices)
    /// </summary>
    [Test]
    public void TestSingleQuadAlternatingStripGeneration() {
      short[] expected = {
        0,
        2, 1, 3
      };

      short[] actual = TriangleStripIndexBuilder.BuildAlternatingStrip(1, 1);

      CollectionAssert.AreEqual(actual, expected);
    }

    /// <summary>
    ///   Verifies that an exception is thrown when a invalid segment count is
    ///   passed for the X axis
    /// </summary>
    [Test]
    public void TestThrowOnInvalidSegmentsX() {
      Assert.Throws<ArgumentException>(
        delegate() { TriangleStripIndexBuilder.BuildAlternatingStrip(0, 4); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown when a invalid segment count is
    ///   passed for the Z axis
    /// </summary>
    [Test]
    public void TestThrowOnInvalidSegmentsZ() {
      Assert.Throws<ArgumentException>(
        delegate() { TriangleStripIndexBuilder.BuildAlternatingStrip(4, 0); }
      );
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects

#endif // UNITTEST
