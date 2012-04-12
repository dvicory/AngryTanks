#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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

using NUnit.Framework;

using Microsoft.Xna.Framework;

using Nuclex.Support;

namespace Nuclex.UserInterface {

  /// <summary>Unit Test for the floating point rectangle class</summary>
  [TestFixture]
  internal class RectangleFTest {

    /// <summary>
    ///   Validates the empty rectangle provided by the floating point rectangle
    /// </summary>
    [Test]
    public void TestEmptyRectangle() {
      Assert.AreEqual(0.0f, RectangleF.Empty.X);
      Assert.AreEqual(0.0f, RectangleF.Empty.Y);
      Assert.AreEqual(0.0f, RectangleF.Empty.Width);
      Assert.AreEqual(0.0f, RectangleF.Empty.Height);
    }

    /// <summary>
    ///   Verifies that the constructor of the floating point rectangle is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      Assert.AreEqual(1.2f, testRectangle.X);
      Assert.AreEqual(3.4f, testRectangle.Y);
      Assert.AreEqual(5.6f, testRectangle.Width);
      Assert.AreEqual(7.8f, testRectangle.Height);
    }

    /// <summary>
    ///   Verifies that the Offset() method is working as expected
    /// </summary>
    [Test]
    public void TestOffset() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      testRectangle.Offset(new Vector2(100.0f, 100.0f));

      Assert.AreEqual(101.2f, testRectangle.X);
      Assert.AreEqual(103.4f, testRectangle.Y);
      Assert.AreEqual(5.6f, testRectangle.Width);
      Assert.AreEqual(7.8f, testRectangle.Height);
    }

    /// <summary>
    ///   Verifies that the Inflate() method is working as expected
    /// </summary>
    [Test]
    public void TestInflate() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      testRectangle.Inflate(10.0f, 100.0f);

      Assert.That(testRectangle.X, Is.EqualTo(-8.8f).Within(4).Ulps);
      Assert.That(testRectangle.Y, Is.EqualTo(-96.6f).Within(4).Ulps);
      Assert.That(testRectangle.Width, Is.EqualTo(25.6f).Within(4).Ulps);
      Assert.That(testRectangle.Height, Is.EqualTo(207.8f).Within(4).Ulps);
    }

    /// <summary>
    ///   Verifies that the Contains() method is working as expected for points
    /// </summary>
    [Test]
    public void TestContainsWithPoint() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      Vector2 insidePoint = new Vector2(3.4f, 5.6f);
      Assert.IsTrue(testRectangle.Contains(insidePoint));

      Vector2 outsidePoint = Vector2.Zero;
      Assert.IsFalse(testRectangle.Contains(outsidePoint));

      Vector2 upperLeftBorderPoint = new Vector2(testRectangle.X, testRectangle.Y);
      Assert.IsTrue(testRectangle.Contains(upperLeftBorderPoint));

      Vector2 lowerRightBorderPoint = new Vector2(testRectangle.Right, testRectangle.Bottom);
      Assert.IsFalse(testRectangle.Contains(lowerRightBorderPoint));
    }

    /// <summary>
    ///   Verifies that the Contains() method is working as expected for point references
    /// </summary>
    [Test]
    public void TestContainsWithPointReference() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      bool result;

      Vector2 insidePoint = new Vector2(3.4f, 5.6f);
      testRectangle.Contains(ref insidePoint, out result);
      Assert.IsTrue(result);

      Vector2 outsidePoint = Vector2.Zero;
      testRectangle.Contains(ref outsidePoint, out result);
      Assert.IsFalse(result);

      Vector2 upperLeftBorderPoint = new Vector2(testRectangle.X, testRectangle.Y);
      testRectangle.Contains(ref upperLeftBorderPoint, out result);
      Assert.IsTrue(result);

      Vector2 lowerRightBorderPoint = new Vector2(testRectangle.Right, testRectangle.Bottom);
      testRectangle.Contains(ref lowerRightBorderPoint, out result);
      Assert.IsFalse(result);
    }

    /// <summary>
    ///   Verifies that the Contains() method is working as expected for rectangles
    /// </summary>
    [Test]
    public void TestContainsWithRectangle() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      RectangleF insideRectangle = new RectangleF(3.0f, 6.0f, 3.7f, 5.2f);
      Assert.IsTrue(testRectangle.Contains(insideRectangle));

      RectangleF outsideRectangle = new RectangleF(1.1f, 3.3f, 5.8f, 8.0f);
      Assert.IsFalse(testRectangle.Contains(outsideRectangle));

      RectangleF upperLeftQuadrant = new RectangleF(1.2f, 3.4f, 2.8f, 3.9f);
      Assert.IsTrue(testRectangle.Contains(upperLeftQuadrant));

      RectangleF lowerRightQuadrant = new RectangleF(4.0f, 7.3f, 2.8f, 3.9f);
      Assert.IsTrue(testRectangle.Contains(lowerRightQuadrant));
    }

    /// <summary>
    ///   Verifies that the Contains() method is working as expected for rectangle references
    /// </summary>
    [Test]
    public void TestContainsWithRectangleReference() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      bool result;

      RectangleF insideRectangle = new RectangleF(3.0f, 6.0f, 3.7f, 5.2f);
      testRectangle.Contains(ref insideRectangle, out result);
      Assert.IsTrue(result);

      RectangleF outsideRectangle = new RectangleF(1.1f, 3.3f, 5.8f, 8.0f);
      testRectangle.Contains(ref outsideRectangle, out result);
      Assert.IsFalse(result);

      RectangleF upperLeftQuadrant = new RectangleF(1.2f, 3.4f, 2.8f, 3.9f);
      testRectangle.Contains(ref upperLeftQuadrant, out result);
      Assert.IsTrue(result);

      RectangleF lowerRightQuadrant = new RectangleF(4.0f, 7.3f, 2.8f, 3.9f);
      testRectangle.Contains(ref lowerRightQuadrant, out result);
      Assert.IsTrue(result);
    }

    /// <summary>
    ///   Verifies that the Intersects() method is working as expected for rectangles
    /// </summary>
    [Test]
    public void TestIntersectsWithRectangle() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      RectangleF insideRectangle = new RectangleF(3.0f, 6.0f, 3.7f, 5.2f);
      Assert.IsTrue(testRectangle.Intersects(insideRectangle));

      RectangleF outsideRectangle = new RectangleF(1.1f, 3.3f, 5.8f, 8.0f);
      Assert.IsTrue(testRectangle.Intersects(outsideRectangle));

      RectangleF horizontalTouchingRectangle = new RectangleF(3.0f, 0.0f, 3.7f, 16.0f);
      Assert.IsTrue(testRectangle.Intersects(horizontalTouchingRectangle));

      RectangleF verticalTouchingRectangle = new RectangleF(0.0f, 6.0f, 12.0f, 5.2f);
      Assert.IsTrue(testRectangle.Intersects(verticalTouchingRectangle));

      RectangleF upperLeftTouchingRectangle = new RectangleF(0.0f, 0.0f, 1.2f, 3.4f);
      Assert.IsFalse(testRectangle.Intersects(upperLeftTouchingRectangle));

      RectangleF lowerRightTouchingRectangle = new RectangleF(6.8f, 10.2f, 1.0f, 1.0f);
      Assert.IsFalse(testRectangle.Intersects(lowerRightTouchingRectangle));
    }

    /// <summary>
    ///   Verifies that the Intersects() method is working as expected for rectangles
    /// </summary>
    [Test]
    public void TestIntersectsWithRectangleReference() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      bool result;

      RectangleF insideRectangle = new RectangleF(3.0f, 6.0f, 3.7f, 5.2f);
      testRectangle.Intersects(ref insideRectangle, out result);
      Assert.IsTrue(result);

      RectangleF outsideRectangle = new RectangleF(1.1f, 3.3f, 5.8f, 8.0f);
      testRectangle.Intersects(ref outsideRectangle, out result);
      Assert.IsTrue(result);

      RectangleF horizontalTouchingRectangle = new RectangleF(3.0f, 0.0f, 3.7f, 16.0f);
      testRectangle.Intersects(ref horizontalTouchingRectangle, out result);
      Assert.IsTrue(result);

      RectangleF verticalTouchingRectangle = new RectangleF(0.0f, 6.0f, 12.0f, 5.2f);
      testRectangle.Intersects(ref verticalTouchingRectangle, out result);
      Assert.IsTrue(result);

      RectangleF upperLeftTouchingRectangle = new RectangleF(0.0f, 0.0f, 1.2f, 3.4f);
      testRectangle.Intersects(ref upperLeftTouchingRectangle, out result);
      Assert.IsFalse(result);

      RectangleF lowerRightTouchingRectangle = new RectangleF(6.8f, 10.2f, 1.0f, 1.0f);
      testRectangle.Intersects(ref lowerRightTouchingRectangle, out result);
      Assert.IsFalse(result);
    }

    /// <summary>
    ///   Verifies that the equality operator of the unified rectangle is working
    /// </summary>
    [Test]
    public void TestEqualityOperator() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      RectangleF equivalentRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      RectangleF differingRectangle = new RectangleF(3.0f, 6.0f, 3.7f, 5.2f);

      Assert.IsTrue(testRectangle == equivalentRectangle);
      Assert.IsFalse(testRectangle == differingRectangle);
    }

    /// <summary>
    ///   Verifies that the inequality operator of the unified rectangle is working
    /// </summary>
    [Test]
    public void TestInequalityOperator() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      RectangleF equivalentRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      RectangleF differingRectangle = new RectangleF(3.0f, 6.0f, 3.7f, 5.2f);

      Assert.IsFalse(testRectangle != equivalentRectangle);
      Assert.IsTrue(testRectangle != differingRectangle);
    }

    /// <summary>
    ///   Tests the Equals() method of the unified rectangle class when it has to perform
    ///   a downcast to obtain the comparison rectangle
    /// </summary>
    [Test]
    public void TestEqualsWithDowncast() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      RectangleF equivalentRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      RectangleF differingRectangle = new RectangleF(3.0f, 6.0f, 3.7f, 5.2f);

      Assert.IsTrue(testRectangle.Equals((object)equivalentRectangle));
      Assert.IsFalse(testRectangle.Equals((object)differingRectangle));
    }

    /// <summary>
    ///   Tests the Equals() method of the unified rectangle class against a different type
    /// </summary>
    [Test]
    public void TestEqualsWithDifferentType() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      Assert.IsFalse(testRectangle.Equals(DateTime.MinValue));
    }

    /// <summary>
    ///   Tests the Equals() method of the unified rectangle class against a null pointer
    /// </summary>
    [Test]
    public void TestEqualsWithNullReference() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      Assert.IsFalse(testRectangle.Equals(null));
    }

    /// <summary>
    ///   Tests the GetHashCode() method of the unified rectangle class
    /// </summary>
    [Test]
    public void TestGetHashCode() {
      RectangleF testRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);
      RectangleF equivalentRectangle = new RectangleF(1.2f, 3.4f, 5.6f, 7.8f);

      Assert.AreEqual(testRectangle.GetHashCode(), equivalentRectangle.GetHashCode());
    }

    /// <summary>
    ///   Tests the ToString() method of the unified rectangle class
    /// </summary>
    [Test]
    public void TestToString() {
      Assert.IsNotNull(RectangleF.Empty.ToString());
    }

  }

} // namespace Nuclex.UserInterface

#endif // UNITTEST
