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

using Microsoft.Xna.Framework;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Geometry.Areas {

  /// <summary>Test for the three-dimensional plane implementation</summary>
  [TestFixture]
  public class Plane3Test {

    #region class BrokenRandom

    /// <summary>A broken random number generator</summary>
    private class BrokenRandom : IRandom {

      /// <summary>
      ///   Returns a nonnegative random number less than the specified maximum
      /// </summary>
      /// <param name="maximumValue">
      ///   The exclusive upper bound of the random number to be generated. maxValue must
      ///   be greater than or equal to zero
      /// </param>
      /// <returns>
      ///   A 32-bit signed integer greater than or equal to zero, and less than maxValue;
      ///   that is, the range of return values ordinarily includes zero but not maxValue.
      ///   However, if maxValue equals zero, maxValue is returned
      /// </returns>
      /// <exception cref="System.ArgumentOutOfRangeException">
      ///   maximumValue is less than zero
      /// </exception>
      public int Next(int maximumValue) {
        return -maximumValue; // Intentional misbehavior
      }

      /// <summary>Returns a random number between 0.0 and 1.0</summary>
      /// <returns>
      ///   A double-precision floating point number greater than or equal to 0.0,
      ///   and less than 1.0
      /// </returns>
      public double NextDouble() { return 0.0; }

    }

    #endregion // class BrokenRandom

    /// <summary>Verifies that the constructor accepting an offset is working</summary>
    [Test]
    public void TestOffsetConstructor() {
      Vector3 offset = new Vector3(1.0f, 2.0f, 3.0f);
      Plane3 plane = new Plane3(offset, Vector3.Up);
      Assert.AreEqual(offset, plane.Offset);
    }

    /// <summary>Verifies that the surface area of a plane is infinite</summary>
    [Test]
    public void TestDistanceConstructor() {
      Plane3 plane = new Plane3(123.0f, Vector3.Up);
      Assert.AreEqual(123.0f, plane.Offset.Length());
    }

    /// <summary>Verifies that the surface area of a plane is infinite</summary>
    [Test]
    public void TestAreaIsInfinite() {
      Plane3 plane = new Plane3(Vector3.Zero, Vector3.Up);
      Assert.AreEqual(float.PositiveInfinity, plane.Area);
    }

    /// <summary>Verifies that the circumference of a plane is infinite</summary>
    [Test]
    public void TestCircumferenceIsInfinite() {
      Plane3 plane = new Plane3(Vector3.Zero, Vector3.Up);
      Assert.AreEqual(float.PositiveInfinity, plane.CircumferenceLength);
    }

    /// <summary>Validates that a plane's center of mass can be calculated</summary>
    [Test]
    public void TestCenterOfMass() {
      Plane3 zeroPlane = new Plane3(Vector3.Zero, Vector3.Up);
      Assert.AreEqual(Vector3.Zero, zeroPlane.CenterOfMass);

      Plane3 xPlane = new Plane3(Vector3.UnitX, Vector3.Right);
      Assert.AreEqual(Vector3.Right, xPlane.CenterOfMass);

      Plane3 yPlane = new Plane3(Vector3.UnitY, Vector3.Up);
      Assert.AreEqual(Vector3.Up, yPlane.CenterOfMass);

      Plane3 zPlane = new Plane3(Vector3.UnitZ, Vector3.Backward);
      Assert.AreEqual(Vector3.Backward, zPlane.CenterOfMass);
    }

    /// <summary>
    ///   Validates that a plane's side determination helper is working as expected
    /// </summary>
    [Test]
    public void TestSideDetermination() {
      Assert.AreEqual(
        Side.Positive, Plane3.GetSide(Vector3.Zero, Vector3.Up, Vector3.UnitY)
      );
      Assert.AreEqual(
        Side.Negative, Plane3.GetSide(Vector3.Zero, Vector3.Up, -Vector3.UnitY)
      );
      Assert.AreEqual(
        Side.Negative, Plane3.GetSide(Vector3.UnitY * 2.0f, Vector3.Up, Vector3.UnitY)
      );
    }

    /// <summary>
    ///   Verifies that the bounding box calculation of the plane class is working
    ///   for planes perfectly aligned to the Y/Z axes
    /// </summary>
    [Test]
    public void TestBoundingBoxCalculationForXPlane() {
      Plane3 xPlane = new Plane3(Vector3.UnitX * 2.0f, Vector3.Right);
      Assert.AreEqual(
        new Volumes.AxisAlignedBox3(
          new Vector3(2.0f, float.NegativeInfinity, float.NegativeInfinity),
          new Vector3(2.0f, float.PositiveInfinity, float.PositiveInfinity)
        ),
        xPlane.BoundingBox
      );
    }

    /// <summary>
    ///   Verifies that the bounding box calculation of the plane class is working
    ///   for planes perfectly aligned to the X/Z axes
    /// </summary>
    [Test]
    public void TestBoundingBoxCalculationForYPlane() {
      Plane3 yPlane = new Plane3(Vector3.UnitY * 2.0f, Vector3.Up);
      Assert.AreEqual(
        new Volumes.AxisAlignedBox3(
          new Vector3(float.NegativeInfinity, 2.0f, float.NegativeInfinity),
          new Vector3(float.PositiveInfinity, 2.0f, float.PositiveInfinity)
        ),
        yPlane.BoundingBox
      );
    }

    /// <summary>
    ///   Verifies that the bounding box calculation of the plane class is working
    ///   for planes perfectly aligned to the X/Y axes
    /// </summary>
    [Test]
    public void TestBoundingBoxCalculationForZPlane() {
      Plane3 zPlane = new Plane3(Vector3.UnitZ * 2.0f, Vector3.Backward);
      Assert.AreEqual(
        new Volumes.AxisAlignedBox3(
          new Vector3(float.NegativeInfinity, float.NegativeInfinity, 2.0f),
          new Vector3(float.PositiveInfinity, float.PositiveInfinity, 2.0f)
        ),
        zPlane.BoundingBox
      );
    }

    /// <summary>
    ///   Verifies that the bounding box calculation of the plane class is working
    ///   for planes
    /// </summary>
    [Test]
    public void TestBoundingBoxCalculation() {
      Vector3 diagonal = Vector3.Normalize(Vector3.One);
      Plane3 plane = new Plane3(Vector3.Zero, diagonal);

      Assert.AreEqual(
        new Volumes.AxisAlignedBox3(
          new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity),
          new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)
        ),
        plane.BoundingBox
      );
    }

    /// <summary>
    ///   Tests whether the plane class is able to locate the closest point on the plane
    ///   to an arbitrary point
    /// </summary>
    [Test]
    public void TestClosestPointToPlane() {
      Plane3 xPlane = new Plane3(Vector3.UnitX * 2.0f, Vector3.Right);
      Assert.AreEqual(
        new Vector3(2.0f, 20.0f, 30.0f),
        xPlane.ClosestPointTo(new Vector3(10.0f, 20.0f, 30.0f))
      );

      Plane3 yPlane = new Plane3(Vector3.UnitY * 2.0f, Vector3.Up);
      Assert.AreEqual(
        new Vector3(10.0f, 2.0f, 30.0f),
        yPlane.ClosestPointTo(new Vector3(10.0f, 20.0f, 30.0f))
      );

      Plane3 zPlane = new Plane3(Vector3.UnitZ * 2.0f, Vector3.Backward);
      Assert.AreEqual(
        new Vector3(10.0f, 20.0f, 2.0f),
        zPlane.ClosestPointTo(new Vector3(10.0f, 20.0f, 30.0f))
      );
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point on the plane's perimeter
    ///   for a plane perfectly aligned to the Y/Z axes
    /// </summary>
    [Test]
    public void TestRandomPointOnPerimeterForXPlane() {
      IRandom randomNumberGenerator = new DefaultRandom();

      Plane3 xPlane = new Plane3(Vector3.UnitX * 2.0f, Vector3.Right);
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = xPlane.RandomPointOnPerimeter(randomNumberGenerator);

        Assert.AreEqual(2.0f, randomPoint.X);
        Assert.IsTrue(float.IsInfinity(randomPoint.Y));
        Assert.IsTrue(float.IsInfinity(randomPoint.Z));
      }
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point on the plane's perimeter
    ///   for a plane perfectly aligned to the X/Z axes
    /// </summary>
    [Test]
    public void TestRandomPointOnPerimeterForYPlane() {
      IRandom randomNumberGenerator = new DefaultRandom();

      Plane3 yPlane = new Plane3(Vector3.UnitY * 2.0f, Vector3.Up);
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = yPlane.RandomPointOnPerimeter(randomNumberGenerator);

        Assert.IsTrue(float.IsInfinity(randomPoint.X));
        Assert.AreEqual(2.0f, randomPoint.Y);
        Assert.IsTrue(float.IsInfinity(randomPoint.Z));
      }
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point on the plane's perimeter
    ///   for a plane perfectly aligned to the X/Y axes
    /// </summary>
    [Test]
    public void TestRandomPointOnPerimeterForZPlane() {
      IRandom randomNumberGenerator = new DefaultRandom();

      Plane3 zPlane = new Plane3(Vector3.UnitZ * 2.0f, Vector3.Backward);
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = zPlane.RandomPointOnPerimeter(randomNumberGenerator);

        Assert.IsTrue(float.IsInfinity(randomPoint.X));
        Assert.IsTrue(float.IsInfinity(randomPoint.Y));
        Assert.AreEqual(2.0f, randomPoint.Z);
      }
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point on the plane's perimeter
    /// </summary>
    [Test]
    public void TestRandomPointOnPerimeter() {
      IRandom randomNumberGenerator = new DefaultRandom();

      // A random point has to involve infinity (since the chance that of hitting the
      // meager numeric range of a float, or any other finite number system, is about
      // zero for a infinitely large plane). But given the orientation of the plane,
      // only these combinations of positive and negative infinity can be possible.
      Vector3[] possiblePoints = new Vector3[] {
        new Vector3(float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity),
        new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity),
        new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity),
        new Vector3(float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity),
      };

      Plane3 plane = new Plane3(Vector3.Zero, Vector3.Normalize(Vector3.One));
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = plane.RandomPointOnPerimeter(randomNumberGenerator);
        CollectionAssert.Contains(possiblePoints, randomPoint);
      }
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point within the plane
    ///   for a plane perfectly aligned to the Y/Z axes
    /// </summary>
    [Test]
    public void TestRandomPointWithinForXPlane() {
      IRandom randomNumberGenerator = new DefaultRandom();

      Plane3 xPlane = new Plane3(Vector3.UnitX * 2.0f, Vector3.Right);
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = xPlane.RandomPointWithin(randomNumberGenerator);

        Assert.AreEqual(2.0f, randomPoint.X);
        Assert.IsTrue(float.IsInfinity(randomPoint.Y));
        Assert.IsTrue(float.IsInfinity(randomPoint.Z));
      }
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point within the plane
    ///   for a plane perfectly aligned to the X/Z axes
    /// </summary>
    [Test]
    public void TestRandomPointWithinForYPlane() {
      IRandom randomNumberGenerator = new DefaultRandom();

      Plane3 yPlane = new Plane3(Vector3.UnitY * 2.0f, Vector3.Up);
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = yPlane.RandomPointWithin(randomNumberGenerator);

        Assert.IsTrue(float.IsInfinity(randomPoint.X));
        Assert.AreEqual(2.0f, randomPoint.Y);
        Assert.IsTrue(float.IsInfinity(randomPoint.Z));
      }
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point within the plane
    ///   for a plane perfectly aligned to the X/Y axes
    /// </summary>
    [Test]
    public void TestRandomPointWithinForZPlane() {
      IRandom randomNumberGenerator = new DefaultRandom();

      Plane3 zPlane = new Plane3(Vector3.UnitZ * 2.0f, Vector3.Backward);
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = zPlane.RandomPointWithin(randomNumberGenerator);

        Assert.IsTrue(float.IsInfinity(randomPoint.X));
        Assert.IsTrue(float.IsInfinity(randomPoint.Y));
        Assert.AreEqual(2.0f, randomPoint.Z);
      }
    }

    /// <summary>
    ///   Tests whether the plane class can find a random point within the plane
    /// </summary>
    [Test]
    public void TestRandomPointWithin() {
      IRandom randomNumberGenerator = new DefaultRandom();

      // A random point has to involve infinity (since the chance that of hitting the
      // meager numeric range of a float, or any other finite number system, is about
      // zero for a infinitely large plane). But given the orientation of the plane,
      // only these combinations of positive and negative infinity can be possible.
      Vector3[] possiblePoints = new Vector3[] {
        new Vector3(float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity),
        new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity),
        new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity),
        new Vector3(float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity),
      };

      Plane3 plane = new Plane3(Vector3.Zero, Vector3.Normalize(Vector3.One));
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector3 randomPoint = plane.RandomPointWithin(randomNumberGenerator);
        //Assert.That(randomPoint, Is.OneOf(possiblePoints));
        CollectionAssert.Contains(possiblePoints, randomPoint);
      }
    }

    /// <summary>
    ///   Tests whether the plane class throws an exception if the random number generator
    ///   is malfunctioning (talk about edge cases...)
    /// </summary>
    [Test]
    public void TestThrowOnRandomPointOnPerimeterWithBrokenRandom() {
      IRandom randomNumberGenerator = new BrokenRandom();

      Plane3 plane = new Plane3(Vector3.Zero, Vector3.Normalize(Vector3.One));
      Assert.Throws<InvalidOperationException>(
        delegate() { plane.RandomPointOnPerimeter(randomNumberGenerator); }
      );
    }

  }

} // namespace Nuclex.Geometry.Areas

#endif // UNITTEST
