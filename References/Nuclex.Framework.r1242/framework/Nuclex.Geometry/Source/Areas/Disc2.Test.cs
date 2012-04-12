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

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.Geometry.Areas {

  /// <summary>Test for the circle (2D sphere) implementation</summary>
  [TestFixture]
  public class Disc2Test {

    /// <summary>Verifies that the constructor of the disc class is working</summary>
    [Test]
    public void TestConstructor() {
      Disc2 testDisc = new Disc2(new Vector2(12.34f, 56.78f), 9.0f);
      Assert.AreEqual(12.34f, testDisc.Center.X);
      Assert.AreEqual(56.78f, testDisc.Center.Y);
      Assert.AreEqual(9.0f, testDisc.Radius);
    }

    /// <summary>
    ///   Verifies that the total surface area of a disc can be determined
    /// </summary>
    [Test]
    public void TestSurfaceArea() {
      Disc2 testDisc = new Disc2(new Vector2(12.34f, 56.78f), 20.0f);

      Assert.AreEqual(1256.63706f, testDisc.Area);
    }

    /// <summary>
    ///   Verifies that the circumference length of the disc can be determined
    /// </summary>
    [Test]
    public void TestCircumferenceLength() {
      Disc2 testDisc = new Disc2(new Vector2(12.34f, 56.78f), 20.0f);

      Assert.AreEqual(125.66371f, testDisc.CircumferenceLength);
    }

    /// <summary>Verifies that the center of mass is correctly determined</summary>
    [Test]
    public void TestCenterOfMass() {
      Disc2 testDisc = new Disc2(new Vector2(12.34f, 56.78f), 20.0f);

      Assert.AreEqual(testDisc.Center, testDisc.CenterOfMass);
    }

    /// <summary>
    ///   Tests whether a tight-fitting bounding box for a disc can be obtained
    /// </summary>
    [Test]
    public void TestBoundingBox() {
      Disc2 testDisc = new Disc2(new Vector2(123.4f, 567.8f), 9.0f);

      AxisAlignedRectangle2 boundingRectangle = testDisc.BoundingBox;

      Assert.AreEqual(testDisc.Center.X - testDisc.Radius, boundingRectangle.Min.X);
      Assert.AreEqual(testDisc.Center.X + testDisc.Radius, boundingRectangle.Max.X);

      Assert.AreEqual(testDisc.Center.Y - testDisc.Radius, boundingRectangle.Min.Y);
      Assert.AreEqual(testDisc.Center.Y + testDisc.Radius, boundingRectangle.Max.Y);
    }

    /// <summary>
    ///   Tests whether the closest point determination works for two-dimensional discs
    /// </summary>
    [Test]
    public void TestClosestPointTo() {
      Disc2 testDisc = new Disc2(new Vector2(123.4f, 567.8f), 9.0f);

      Random randomNumberGenerator = new Random();
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector2 randomPoint = new Vector2(
          (float)randomNumberGenerator.NextDouble() * testDisc.Radius * 4.0f,
          (float)randomNumberGenerator.NextDouble() * testDisc.Radius * 4.0f
        );
        randomPoint += testDisc.Center;

        float distance = (randomPoint - testDisc.Center).Length();
        if(distance < testDisc.Radius) {
          Assert.AreEqual(randomPoint, testDisc.ClosestPointTo(randomPoint));
        } else {
          Vector2 closestPoint = testDisc.ClosestPointTo(randomPoint);
          Assert.That(
            (closestPoint - testDisc.Center).Length(),
            Is.EqualTo(testDisc.Radius).Within(36).Ulps
            // TODO: This method suffers from accuracy problems!
          );
        }
      }
    }

    /// <summary>
    ///   Tests whether random points on the perimeter of two-dimensional discs can
    ///   be obtained
    /// </summary>
    [Test]
    public void TestRandomPointOnPerimeter() {
      Disc2 testDisc = new Disc2(new Vector2(123.4f, 567.8f), 9.0f);

      DefaultRandom randomNumberGenerator = new DefaultRandom();
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector2 randomPoint = testDisc.RandomPointOnPerimeter(randomNumberGenerator);
        Assert.That(
          (testDisc.Center - randomPoint).Length(),
          Is.EqualTo(testDisc.Radius).Within(36).Ulps
          // TODO: This method suffers from accuracy problems!
        );
      }
      
    }

    /// <summary>
    ///   Tests whether random points on the perimeter of two-dimensional discs can
    ///   be obtained
    /// </summary>
    [Test]
    public void TestRandomPointWithin() {
      Disc2 testDisc = new Disc2(new Vector2(123.4f, 567.8f), 9.0f);

      DefaultRandom randomNumberGenerator = new DefaultRandom();
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector2 randomPoint = testDisc.RandomPointWithin(randomNumberGenerator);

        float randomPointDistance = (testDisc.Center - randomPoint).Length();
        if(!FloatHelper.AreAlmostEqual(randomPointDistance, testDisc.Radius, 36)) {
          Assert.LessOrEqual(randomPointDistance, testDisc.Radius);
        }
      }

    }

  }

} // namespace Nuclex.Geometry.Areas

#endif // UNITTEST