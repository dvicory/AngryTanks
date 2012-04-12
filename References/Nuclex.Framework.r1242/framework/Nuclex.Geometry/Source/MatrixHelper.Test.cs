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

using Nuclex.Support;

namespace Nuclex.Geometry {

  /// <summary>Unit Test for the matrix helper class</summary>
  [TestFixture]
  public class MatrixHelperTest {

    /// <summary>
    ///   Verifies that the matrix helper can construct a matrix from individual vectors
    /// </summary>
    [Test]
    public void TestCreateMatrixFromVectors() {
      Matrix testMatrix = MatrixHelper.Create(
        Vector3.Right, Vector3.Up, Vector3.Backward
      );

      Assert.AreEqual(Vector3.Right, testMatrix.Right);
      Assert.AreEqual(Vector3.Up, testMatrix.Up);
      Assert.AreEqual(Vector3.Backward, testMatrix.Backward);
      Assert.AreEqual(Vector3.Zero, testMatrix.Translation);
    }

    /// <summary>
    ///   Verifies that the matrix helper can construct a matrix from individual vectors
    ///   together with a translation point
    /// </summary>
    [Test]
    public void TestCreateMatrixFromVectorsAndTranslation() {
      Vector3 translation = new Vector3(1.0f, 2.0f, 3.0f);
      Matrix testMatrix = MatrixHelper.Create(
        translation, Vector3.Left, Vector3.Down, Vector3.Forward
      );

      Assert.AreEqual(Vector3.Left, testMatrix.Right);
      Assert.AreEqual(Vector3.Down, testMatrix.Up);
      Assert.AreEqual(Vector3.Forward, testMatrix.Backward);
      Assert.AreEqual(translation, testMatrix.Translation);
    }

    /// <summary>
    ///   Verifies that the matrix helper can be used to obtain and assign individual
    ///   elements of a matrix by their indices
    /// </summary>
    [Test]
    public void TestGetAndSetMatrixElementsByIndex() {
      float[/*4*/, /*4*/] matrixContents = new float[4, 4] {
        { 1.1f, 1.2f, 1.3f, 1.4f },
        { 2.1f, 2.2f, 2.3f, 2.4f },
        { 3.1f, 3.2f, 3.3f, 3.4f },
        { 4.1f, 4.2f, 4.3f, 4.4f }
      };

      Matrix testMatrix = new Matrix();

      // Assign the matrix 
      for(int row = 0; row < 4; ++row) {
        for(int column = 0; column < 4; ++column) {
          Assert.AreEqual(0.0f, MatrixHelper.Get(ref testMatrix, row, column));
          MatrixHelper.Set(ref testMatrix, row, column, matrixContents[row, column]);
        }
      }

      for(int row = 0; row < 4; ++row) {
        for(int column = 0; column < 4; ++column) {
          Assert.AreEqual(
            matrixContents[row, column],
            MatrixHelper.Get(ref testMatrix, row, column)
          );
        }
      }
    }

    /// <summary>
    ///   Verifies that the matrix helper throws an exception when an invalid index to
    ///   read from is specified
    /// </summary>
    [Test]
    public void TestThrowOnGetMatrixElementWithInvalidIndex() {
      Matrix test = Matrix.Identity;
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { Console.WriteLine(MatrixHelper.Get(ref test, -1, -1)); }
      );
    }

    /// <summary>
    ///   Verifies that the matrix helper throws an exception when an invalid index to
    ///   write to is specified
    /// </summary>
    [Test]
    public void TestThrowOnSetMatrixElementWithInvalidIndex() {
      Matrix test = Matrix.Identity;
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { MatrixHelper.Set(ref test, -1, -1, 1234.5678f); }
      );
    }

    /// <summary>
    ///   Verifies that the matrix helper can be used to obtain and assign individual
    ///   rows of a matrix by their indices
    /// </summary>
    [Test]
    public void TestGetAndSetMatrixRowsByIndex() {
      Vector3[/*4*/] matrixContents = new Vector3[4] {
        new Vector3(1.1f, 1.2f, 1.3f),
        new Vector3(2.1f, 2.2f, 2.3f),
        new Vector3(3.1f, 3.2f, 3.3f),
        new Vector3(4.1f, 4.2f, 4.3f)
      };

      Matrix testMatrix = new Matrix();

      // Assign the matrix 
      for(int row = 0; row < 4; ++row) {
        Assert.AreEqual(Vector3.Zero, MatrixHelper.Get(ref testMatrix, row));
        MatrixHelper.Set(ref testMatrix, row, matrixContents[row]);
      }

      for(int row = 0; row < 4; ++row) {
        Assert.AreEqual(matrixContents[row], MatrixHelper.Get(ref testMatrix, row));
      }
    }

    /// <summary>
    ///   Verifies that the matrix helper throws an exception when an invalid row index
    ///   to read from is specified
    /// </summary>
    [Test]
    public void TestThrowOnGetMatrixRowWithInvalidIndex() {
      Matrix test = Matrix.Identity;
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { Console.WriteLine(MatrixHelper.Get(ref test, -1)); }
      );
    }


    /// <summary>
    ///   Verifies that the matrix helper throws an exception when an invalid row index
    ///   to write to is specified
    /// </summary>
    [Test]
    public void TestThrowOnSetMatrixRowWithInvalidIndex() {
      Matrix test = Matrix.Identity;
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { MatrixHelper.Set(ref test, -1, Vector3.Zero); }
      );
    }

  }

} // namespace Nuclex.Geometry

#endif // UNITTEST
