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

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.UserInterface {

  /// <summary>Unit Test for the unified rectangle class</summary>
  [TestFixture]
  internal class UniRectangleTest {

    /// <summary>
    ///   Verifies that the vector constructor of the unified rectangle class is working
    /// </summary>
    [Test]
    public void TestVectorConstructor() {
      UniVector location = new UniVector(firstTestScalar, secondTestScalar);
      UniVector size = new UniVector(thirdTestScalar, fourthTestScalar);

      UniRectangle testRectangle = new UniRectangle(location, size);

      Assert.AreEqual(1.0f, testRectangle.Location.X.Fraction);
      Assert.AreEqual(2.0f, testRectangle.Location.X.Offset);
      Assert.AreEqual(3.0f, testRectangle.Location.Y.Fraction);
      Assert.AreEqual(4.0f, testRectangle.Location.Y.Offset);
      Assert.AreEqual(0.1f, testRectangle.Size.X.Fraction);
      Assert.AreEqual(0.2f, testRectangle.Size.X.Offset);
      Assert.AreEqual(0.3f, testRectangle.Size.Y.Fraction);
      Assert.AreEqual(0.4f, testRectangle.Size.Y.Offset);
    }

    /// <summary>
    ///   Verifies that the scalar constructor of the unified rectangle class is working
    /// </summary>
    [Test]
    public void TestScalarConstructor() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );

      Assert.AreEqual(1.0f, testRectangle.Location.X.Fraction);
      Assert.AreEqual(2.0f, testRectangle.Location.X.Offset);
      Assert.AreEqual(3.0f, testRectangle.Location.Y.Fraction);
      Assert.AreEqual(4.0f, testRectangle.Location.Y.Offset);
      Assert.AreEqual(0.1f, testRectangle.Size.X.Fraction);
      Assert.AreEqual(0.2f, testRectangle.Size.X.Offset);
      Assert.AreEqual(0.3f, testRectangle.Size.Y.Fraction);
      Assert.AreEqual(0.4f, testRectangle.Size.Y.Offset);
    }

    /// <summary>Verifies that the ToOffset() method works as expected</summary>
    [Test]
    public void TestToOffset() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );

      RectangleF offsetRectangle = testRectangle.ToOffset(new Vector2(100.0f, 100.0f));

      Assert.That(offsetRectangle.X, Is.EqualTo(102.0f).Within(4).Ulps);
      Assert.That(offsetRectangle.Y, Is.EqualTo(304.0f).Within(4).Ulps);
      Assert.That(offsetRectangle.Width, Is.EqualTo(10.2f).Within(4).Ulps);
      Assert.That(offsetRectangle.Height, Is.EqualTo(30.4f).Within(4).Ulps);
    }

    /// <summary>
    ///   Verifies that the equality operator of the unified rectangle is working
    /// </summary>
    [Test]
    public void TestEqualityOperator() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );
      UniRectangle equivalentRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );
      UniRectangle differingRectangle = new UniRectangle(
        fourthTestScalar, thirdTestScalar, secondTestScalar, firstTestScalar
      );

      Assert.IsTrue(testRectangle == equivalentRectangle);
      Assert.IsFalse(testRectangle == differingRectangle);
    }

    /// <summary>
    ///   Verifies that the inequality operator of the unified rectangle is working
    /// </summary>
    [Test]
    public void TestInequalityOperator() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );
      UniRectangle equivalentRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );
      UniRectangle differingRectangle = new UniRectangle(
        fourthTestScalar, thirdTestScalar, secondTestScalar, firstTestScalar
      );

      Assert.IsFalse(testRectangle != equivalentRectangle);
      Assert.IsTrue(testRectangle != differingRectangle);
    }

    /// <summary>
    ///   Tests the Equals() method of the unified rectangle class when it has to perform
    ///   a downcast to obtain the comparison rectangle
    /// </summary>
    [Test]
    public void TestEqualsWithDowncast() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );
      UniRectangle equivalentRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );
      UniRectangle differingRectangle = new UniRectangle(
        fourthTestScalar, thirdTestScalar, secondTestScalar, firstTestScalar
      );

      Assert.IsTrue(testRectangle.Equals((object)equivalentRectangle));
      Assert.IsFalse(testRectangle.Equals((object)differingRectangle));
    }

    /// <summary>
    ///   Tests the Equals() method of the unified rectangle class against a different type
    /// </summary>
    [Test]
    public void TestEqualsWithDifferentType() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );

      Assert.IsFalse(testRectangle.Equals(DateTime.MinValue));
    }

    /// <summary>
    ///   Tests the Equals() method of the unified rectangle class against a null pointer
    /// </summary>
    [Test]
    public void TestEqualsWithNullReference() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );

      Assert.IsFalse(testRectangle.Equals(null));
    }

    /// <summary>
    ///   Tests the GetHashCode() method of the unified rectangle class
    /// </summary>
    [Test]
    public void TestGetHashCode() {
      UniRectangle testRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );
      UniRectangle equivalentRectangle = new UniRectangle(
        firstTestScalar, secondTestScalar, thirdTestScalar, fourthTestScalar
      );

      Assert.AreEqual(testRectangle.GetHashCode(), equivalentRectangle.GetHashCode());
    }

    /// <summary>
    ///   Tests the ToString() method of the unified rectangle class
    /// </summary>
    [Test]
    public void TestToString() {
      Assert.IsNotNull(UniRectangle.Empty.ToString());
    }

    /// <summary>Verifies that the left property works as expected</summary>
    [Test]
    public void TestLeftProperty() {
      UniRectangle testRectangle = new UniRectangle();
      
      testRectangle.Left = firstTestScalar;

      Assert.AreEqual(firstTestScalar, testRectangle.Left);
      Assert.AreEqual(firstTestScalar, testRectangle.Location.X);
    }

    /// <summary>Verifies that the right property works as expected</summary>
    [Test]
    public void TestRightProperty() {
      UniRectangle testRectangle = new UniRectangle();

      testRectangle.Right = firstTestScalar;

      Assert.AreEqual(firstTestScalar, testRectangle.Right);
      Assert.AreEqual(firstTestScalar, testRectangle.Size.X);
    }

    /// <summary>Verifies that the top property works as expected</summary>
    [Test]
    public void TestTopProperty() {
      UniRectangle testRectangle = new UniRectangle();

      testRectangle.Top = firstTestScalar;

      Assert.AreEqual(firstTestScalar, testRectangle.Top);
      Assert.AreEqual(firstTestScalar, testRectangle.Location.Y);
    }

    /// <summary>Verifies that the bottom property works as expected</summary>
    [Test]
    public void TestBottomProperty() {
      UniRectangle testRectangle = new UniRectangle();

      testRectangle.Bottom = firstTestScalar;

      Assert.AreEqual(firstTestScalar, testRectangle.Bottom);
      Assert.AreEqual(firstTestScalar, testRectangle.Size.Y);
    }

    /// <summary>Verifies that the min property works as expected</summary>
    [Test]
    public void TestMinProperty() {
      UniVector minVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector maxVector = new UniVector(thirdTestScalar, fourthTestScalar);
      UniRectangle testRectangle = new UniRectangle(UniVector.Zero, maxVector);

      // This doesn't move the rectangle, it resizes it!
      testRectangle.Min = minVector;

      Assert.AreEqual(minVector, testRectangle.Min);
      Assert.AreEqual(minVector, testRectangle.Location);

      UniAssertHelper.AreAlmostEqual(maxVector, testRectangle.Max, 4);
      UniAssertHelper.AreAlmostEqual(maxVector - minVector, testRectangle.Size, 4);
    }

    /// <summary>Verifies that the max property works as expected</summary>
    [Test]
    public void TestMaxProperty() {
      UniVector minVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector maxVector = new UniVector(thirdTestScalar, fourthTestScalar);
      UniRectangle testRectangle = new UniRectangle(UniVector.Zero, minVector);

      testRectangle.Max = maxVector;

      Assert.AreEqual(UniVector.Zero, testRectangle.Min);
      Assert.AreEqual(UniVector.Zero, testRectangle.Location);

      UniAssertHelper.AreAlmostEqual(maxVector, testRectangle.Max, 4);
      UniAssertHelper.AreAlmostEqual(maxVector, testRectangle.Size, 4);
    }

    /// <summary>First value used for testing in the individual unit test methods</summary>
    private static UniScalar firstTestScalar = new UniScalar(1.0f, 2.0f);
    /// <summary>Second value used for testing in the individual unit test methods</summary>
    private static UniScalar secondTestScalar = new UniScalar(3.0f, 4.0f);
    /// <summary>Third value used for testing in the individual unit test methods</summary>
    private static UniScalar thirdTestScalar = new UniScalar(0.1f, 0.2f);
    /// <summary>Fourth value used for testing in the individual unit test methods</summary>
    private static UniScalar fourthTestScalar = new UniScalar(0.3f, 0.4f);

  }

} // namespace Nuclex.UserInterface

#endif // UNITTEST
