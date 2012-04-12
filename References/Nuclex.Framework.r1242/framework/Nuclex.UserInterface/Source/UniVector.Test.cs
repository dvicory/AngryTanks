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

  /// <summary>Unit Test for the unified vector class</summary>
  [TestFixture]
  internal class UniVectorTest {

    /// <summary>Tests the constructor of the unified vector class</summary>
    [Test]
    public void TestConstructor() {
      UniVector testVector = new UniVector(12.34f, 45.67f);

      Assert.AreEqual(0.0f, testVector.X.Fraction);
      Assert.AreEqual(12.34f, testVector.X.Offset);
      Assert.AreEqual(0.0f, testVector.Y.Fraction);
      Assert.AreEqual(45.67f, testVector.Y.Offset);
    }

    /// <summary>Verifies that the ToOffset() method works as expected</summary>
    [Test]
    public void TestToOffset() {
      UniVector testVector = new UniVector(
        new UniScalar(1.2f, 3.4f),
        new UniScalar(5.6f, 7.8f)
      );

      Assert.AreEqual(
        new Vector2(123.4f, 567.8f),
        testVector.ToOffset(new Vector2(100.0f, 100.0f))
      );
    }

    /// <summary>
    ///   Verifies that the addition operator of the unified vector is working
    /// </summary>
    [Test]
    public void TestAdditionOperator() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector addedVector = new UniVector(thirdTestScalar, fourthTestScalar);

      UniVector result = testVector + addedVector;

      Assert.AreEqual(1.1f, result.X.Fraction);
      Assert.AreEqual(2.2f, result.X.Offset);
      Assert.AreEqual(3.3f, result.Y.Fraction);
      Assert.AreEqual(4.4f, result.Y.Offset);
    }

    /// <summary>
    ///   Verifies that the subtraction operator of the unified vector is working
    /// </summary>
    [Test]
    public void TestSubtractionOperator() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector subtractedVector = new UniVector(thirdTestScalar, fourthTestScalar);

      UniVector result = testVector - subtractedVector;

      Assert.AreEqual(0.9f, result.X.Fraction);
      Assert.AreEqual(1.8f, result.X.Offset);
      Assert.AreEqual(2.7f, result.Y.Fraction);
      Assert.AreEqual(3.6f, result.Y.Offset);
    }

    /// <summary>
    ///   Verifies that the multiplication operator of the unified vector is working
    /// </summary>
    [Test]
    public void TestMultiplicationOperator() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector factorVector = new UniVector(thirdTestScalar, fourthTestScalar);

      UniVector result = testVector * factorVector;

      Assert.That(result.X.Fraction, Is.EqualTo(0.1f).Within(4).Ulps);
      Assert.That(result.X.Offset, Is.EqualTo(0.4f).Within(4).Ulps);
      Assert.That(result.Y.Fraction, Is.EqualTo(0.9f).Within(4).Ulps);
      Assert.That(result.Y.Offset, Is.EqualTo(1.6f).Within(4).Ulps);
    }

    /// <summary>
    ///   Verifies that the multiplication operator of the unified vector can be used
    ///   to multiply a scalar by the vector
    /// </summary>
    [Test]
    public void TestMultiplicationOperatorWithScalarOnLeftSide() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);

      UniVector result = new UniScalar(2.0f, 4.0f) * testVector;

      Assert.That(result.X.Fraction, Is.EqualTo(2.0f).Within(4).Ulps);
      Assert.That(result.X.Offset, Is.EqualTo(8.0f).Within(4).Ulps);
      Assert.That(result.Y.Fraction, Is.EqualTo(6.0f).Within(4).Ulps);
      Assert.That(result.Y.Offset, Is.EqualTo(16.0f).Within(4).Ulps);
    }

    /// <summary>
    ///   Verifies that the multiplication operator of the unified vector can be used
    ///   to multiply the vector by a scalar
    /// </summary>
    [Test]
    public void TestMultiplicationOperatorWithScalarInRightSide() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);

      UniVector result = testVector * new UniScalar(4.0f, 2.0f);

      Assert.That(result.X.Fraction, Is.EqualTo(4.0f).Within(4).Ulps);
      Assert.That(result.X.Offset, Is.EqualTo(4.0f).Within(4).Ulps);
      Assert.That(result.Y.Fraction, Is.EqualTo(12.0f).Within(4).Ulps);
      Assert.That(result.Y.Offset, Is.EqualTo(8.0f).Within(4).Ulps);
    }

    /// <summary>
    ///   Verifies that the division operator of the unified vector is working
    /// </summary>
    [Test]
    public void TestDivisionOperator() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector divisorVector = new UniVector(thirdTestScalar, fourthTestScalar);

      UniVector result = testVector / divisorVector;

      Assert.That(result.X.Fraction, Is.EqualTo(10.0f).Within(4).Ulps);
      Assert.That(result.X.Offset, Is.EqualTo(10.0f).Within(4).Ulps);
      Assert.That(result.Y.Fraction, Is.EqualTo(10.0f).Within(4).Ulps);
      Assert.That(result.Y.Offset, Is.EqualTo(10.0f).Within(4).Ulps);
    }

    /// <summary>
    ///   Verifies that the equality operator of the unified vector is working
    /// </summary>
    [Test]
    public void TestEqualityOperator() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector equivalentVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector differingVector = new UniVector(thirdTestScalar, fourthTestScalar);

      Assert.IsTrue(testVector == equivalentVector);
      Assert.IsFalse(testVector == differingVector);
    }

    /// <summary>
    ///   Verifies that the inequality operator of the unified vector is working
    /// </summary>
    [Test]
    public void TestInequalityOperator() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector equivalentVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector differingVector = new UniVector(thirdTestScalar, fourthTestScalar);

      Assert.IsFalse(testVector != equivalentVector);
      Assert.IsTrue(testVector != differingVector);
    }

    /// <summary>
    ///   Tests the Equals() method of the unified vector class when it has to perform
    ///   a downcast to obtain the comparison vector
    /// </summary>
    [Test]
    public void TestEqualsWithDowncast() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector equivalentVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector differingVector = new UniVector(thirdTestScalar, fourthTestScalar);

      Assert.IsTrue(testVector.Equals((object)equivalentVector));
      Assert.IsFalse(testVector.Equals((object)differingVector));
    }

    /// <summary>
    ///   Tests the Equals() method of the unified vector class against a different type
    /// </summary>
    [Test]
    public void TestEqualsWithDifferentType() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);

      Assert.IsFalse(testVector.Equals(DateTime.MinValue));
    }

    /// <summary>
    ///   Tests the Equals() method of the unified vector class against a null pointer
    /// </summary>
    [Test]
    public void TestEqualsWithNullReference() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);

      Assert.IsFalse(testVector.Equals(null));
    }

    /// <summary>
    ///   Tests the GetHashCode() method of the unified vector class
    /// </summary>
    [Test]
    public void TestGetHashCode() {
      UniVector testVector = new UniVector(firstTestScalar, secondTestScalar);
      UniVector equivalentVector = new UniVector(firstTestScalar, secondTestScalar);

      Assert.AreEqual(testVector.GetHashCode(), equivalentVector.GetHashCode());
    }

    /// <summary>
    ///   Tests the ToString() method of the unified vector class
    /// </summary>
    [Test]
    public void TestToString() {
      Assert.IsNotNull(UniVector.Zero.ToString());
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
