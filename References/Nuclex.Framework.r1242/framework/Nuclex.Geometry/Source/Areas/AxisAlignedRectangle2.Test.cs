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

  /// <summary>Test for the two-dimensional axis aligned box implementation</summary>
  [TestFixture]
  public class AxisAlignedRectangle2Test {

    /// <summary>Tests whether the full constructor is working</summary>
    [Test]
    public void TestFullConstructor() {
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(1.2f, 3.4f), new Vector2(5.6f, 7.8f)
      );
      Assert.AreEqual(1.2f, testRectangle.Min.X);
      Assert.AreEqual(3.4f, testRectangle.Min.Y);
      Assert.AreEqual(5.6f, testRectangle.Max.X);
      Assert.AreEqual(7.8f, testRectangle.Max.Y);
    }

    /// <summary>
    ///   Validates that the surface area of an axis aligned box can be calculated
    /// </summary>
    [Test]
    public void TestArea() {
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(10.0f, 20.0f), new Vector2(30.0f, 40.0f)
      );

      float surfaceArea = 20.0f * 20.0f;
      Assert.AreEqual(surfaceArea, testRectangle.Area);
    }

    /// <summary>
    ///   Validates that the circumference length of an axis aligned box can be calculated
    /// </summary>
    [Test]
    public void TestCircumferenceLength() {
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(10.0f, 20.0f), new Vector2(30.0f, 40.0f)
      );

      float circumferenceLength = (20.0f * 2.0f) + (20.0f * 2.0f);
      Assert.AreEqual(circumferenceLength, testRectangle.CircumferenceLength);
    }

    /// <summary>
    ///   Tests whether the center of mass for the rectangle is correctly calculated
    /// </summary>
    [Test]
    public void TestCenterOfMass() {
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(100, 100), new Vector2(110, 120)
      );

      Assert.AreEqual(new Vector2(105, 110), testRectangle.CenterOfMass);
    }

    /// <summary>
    ///   Validates that the bounding box determination works for the rectangle
    /// </summary>
    [Test]
    public void TestBoundingBox() {
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(100, 100), new Vector2(110, 120)
      );

      Assert.AreEqual(testRectangle, testRectangle.BoundingBox);
    }

    /// <summary>
    ///   Verifies that the closest point finder is working
    /// </summary>
    [Test]
    public void TestClosestPointTo() {
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(100, 100), new Vector2(110, 120)
      );

      Assert.AreEqual(
        testRectangle.Min,
        testRectangle.ClosestPointTo(new Vector2(60, 70))
      );
      Assert.AreEqual(
        testRectangle.Max,
        testRectangle.ClosestPointTo(new Vector2(115, 200))
      );
      Assert.AreEqual(
        new Vector2(104, 117),
        testRectangle.ClosestPointTo(new Vector2(104, 117))
      );
    }

    /// <summary>
    ///   Verifies that random points on the rectangle's perimeter can be calculated
    /// </summary>
    [Test]
    public void TestRandomPointOnPerimeter() {
      DefaultRandom random = new DefaultRandom();
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(12.3f, 45.6f), new Vector2(98.7f, 65.4f)
      );

      float[] possibleXs = new float[] { 12.3f, 98.7f };
      float[] possibleYs = new float[] { 45.6f, 65.4f };

      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector2 randomPoint = testRectangle.RandomPointOnPerimeter(random);
        Assert.AreEqual(testRectangle.ClosestPointTo(randomPoint), randomPoint);

        Assert.IsTrue(
          arrayContains(possibleXs, randomPoint.X) || arrayContains(possibleYs, randomPoint.Y)
        );
      }
    }

    /// <summary>
    ///   Verifies that the closest point finder is working
    /// </summary>
    [Test]
    public void TestRandomPointWithin() {
      DefaultRandom random = new DefaultRandom();
      AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
        new Vector2(12.3f, 45.6f), new Vector2(98.7f, 65.4f)
      );

      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Vector2 randomPoint = testRectangle.RandomPointWithin(random);
        Assert.AreEqual(testRectangle.ClosestPointTo(randomPoint), randomPoint);
      }
    }
    
    private bool arrayContains(float[] array, float toCheck) {
      for(int index = 0; index < array.Length; ++index) {
        bool almostEqual = FloatHelper.AreAlmostEqual(
          array[index], toCheck, Specifications.MaximumDeviation
        );
        if(almostEqual) {
          return true;
        }
      }
      return false;
    }
      
    /*
    /// <summary>Tests the intersection query on moving boxes</summary>
    [Test]
    public void TestMovingBoxImpactPoint() {
      AxisAlignedRectangle2 leftBox =
        new AxisAlignedRectangle2(new Vector2(0, 0), new Vector2(10, 10));
      AxisAlignedRectangle2 rightBox =
        new AxisAlignedRectangle2(new Vector2(100, 0), new Vector2(110, 10));

      ImpactPoint impact;

      impact = leftBox.Impact(new Vector2(0, 0), leftBox);
      Assert.IsTrue(impact.Found, "Contact with clone box is detected");
      Assert.AreEqual(
        0.0, impact.Time, Specifications.MaximumDeviation,
        "Contact with clone occured immediately"
      );

      impact = leftBox.Impact(new Vector2(200, 0), rightBox);
      Assert.IsTrue(impact.Found, "Fast moving contact with right box is detected");
      Assert.AreEqual(
        0.45, impact.Time, Specifications.MaximumDeviation,
        "Contact location is determined exactly"
      );

      impact = rightBox.Impact(new Vector2(-200, 0), leftBox);
      Assert.IsTrue(impact.Found, "Fast moving contact with left box is detected");
      Assert.AreEqual(
        0.45, impact.Time, Specifications.MaximumDeviation,
        "Contact location is determined exactly"
      );

    }
    */
  }

} // namespace Nuclex.Geometry.Areas

#endif // UNITTEST