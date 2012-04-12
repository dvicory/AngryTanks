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

  /// <summary>Unit Test for the vector helper class</summary>
  [TestFixture]
  public class VectorHelperTest {

    /// <summary>
    ///   Verifies that the vector helper can calculate the absolute value of the
    ///   individual elements in a vector
    /// </summary>
    [Test]
    public void TestVectorAbs() {
      Vector3 testVector = new Vector3(-1.1f, -2.2f, -3.3f);
      Assert.AreEqual(
        new Vector3(1.1f, 2.2f, 3.3f),
        VectorHelper.Abs(testVector)
      );
    }

    /// <summary>
    ///   Verifies that the vector helper can be used to obtain and assign individual
    ///   rows of a matrix by their indices
    /// </summary>
    [Test]
    public void TestGetAndSetVectorElementsByIndex() {
      float[/*3*/] vectorContents = new float[3] { 1.2f, 3.4f, 5.6f };

      Vector3 testVector = new Vector3();

      // Assign the vector
      for(int index = 0; index < 3; ++index) {
        Assert.AreEqual(0.0f, VectorHelper.Get(ref testVector, index));
        VectorHelper.Set(ref testVector, index, vectorContents[index]);
      }

      for(int index = 0; index < 3; ++index) {
        Assert.AreEqual(vectorContents[index], VectorHelper.Get(ref testVector, index));
      }
    }

    /// <summary>
    ///   Verifies that the vector helper throws an exception when an invalid row index
    ///   to read from is specified
    /// </summary>
    [Test]
    public void TestThrowOnGetVectorRowWithInvalidIndex() {
      Vector3 test = Vector3.Zero;
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { Console.WriteLine(VectorHelper.Get(ref test, -1)); }
      );
    }

    /// <summary>
    ///   Verifies that the vector helper throws an exception when an invalid row index
    ///   to write to is specified
    /// </summary>
    [Test]
    public void TestThrowOnSetVectorRowWithInvalidIndex() {
      Vector3 test = Vector3.Zero;
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { VectorHelper.Set(ref test, -1, 0.0f); }
      );
    }

    /// <summary>
    ///   Verifies that the vector helper can determine an arbitrary perpendicular vector
    ///   to another vector
    /// </summary>
    [Test]
    public void TestGetPerpendicularVector() {
      Vector3[] testVectors = new Vector3[] {
        Vector3.Right,
        Vector3.Up,
        Vector3.Backward,
        Vector3.Normalize(Vector3.One),
        Vector3.Normalize(new Vector3(1.0f, 0.0f, 0.0f)),
        Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f)),
        Vector3.Normalize(new Vector3(0.0f, 0.0f, 1.0f)),
        Vector3.Normalize(new Vector3(1.0f, 1.0f, 0.0f)),
        Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)),
        Vector3.Normalize(new Vector3(0.0f, 1.0f, 1.0f)),
        Vector3.Normalize(new Vector3(1.0f, 2.0f, 3.0f)),
        Vector3.Normalize(new Vector3(3.0f, 1.0f, 2.0f)),
        Vector3.Normalize(new Vector3(2.0f, 3.0f, 1.0f))
      };

      const double ninetyDegrees = Math.PI / 2.0;
      for(int index = 0; index < testVectors.Length; ++index) {
        Vector3 perpendicular = VectorHelper.GetPerpendicularVector(testVectors[index]);

        double angle = Math.Acos(Vector3.Dot(testVectors[index], perpendicular));
        Assert.That(
          angle, Is.EqualTo(ninetyDegrees).Within(Specifications.MaximumDeviation).Ulps
        );
      }
    }

    /// <summary>
    ///   Tests whether the AbsMax() method is working correctly for 2D vectors
    /// </summary>
    [Test]
    public void TestAbsMaxWithVector2() {
      Vector2 absMaxVector = VectorHelper.AbsMax(
        new Vector2(-10, -1), new Vector2(1, 10)
      );
      Assert.AreEqual(new Vector2(-10, 10), absMaxVector);
    }

    /// <summary>
    ///   Tests whether the AbsMax() method is working correctly for 3D vectors
    /// </summary>
    [Test]
    public void TestAbsMaxWithVector3() {
      Vector3 absMaxVector = VectorHelper.AbsMax(
        new Vector3(-10, -1, 10), new Vector3(1, 10, -10)
      );
      Assert.AreEqual(new Vector3(-10, 10, 10), absMaxVector);
    }

  }

} // namespace Nuclex.Geometry

#endif // UNITTEST
