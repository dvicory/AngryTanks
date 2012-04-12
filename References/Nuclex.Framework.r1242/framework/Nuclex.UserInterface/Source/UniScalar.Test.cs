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

namespace Nuclex.UserInterface {

  /// <summary>Unit Test for the unified scalar class</summary>
  [TestFixture]
  internal class UniScalarTest {

    /// <summary>Validates the zero value provided by the unified scalar class</summary>
    [Test]
    public void TestZeroValue() {
      Assert.AreEqual(0.0f, UniScalar.Zero.Fraction);
      Assert.AreEqual(0.0f, UniScalar.Zero.Offset);
    }

    /// <summary>Verifies that the constructor accepting an offset is working</summary>
    [Test]
    public void TestOffsetConstructor() {
      UniScalar testScalar = new UniScalar(123.456f);
      Assert.AreEqual(0.0f, testScalar.Fraction);
      Assert.AreEqual(123.456f, testScalar.Offset);
    }

    /// <summary>
    ///   Verifies that the full constructor of the unified scalar class is working
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      UniScalar testScalar = new UniScalar(12.34f, 56.78f);
      Assert.AreEqual(12.34f, testScalar.Fraction);
      Assert.AreEqual(56.78f, testScalar.Offset);
    }
    
    /// <summary>
    ///   Verifies that the implicit offset conversion constructor is working
    /// </summary>
    [Test]
    public void TestImplicitConstructor() {
      UniScalar testScalar = 654.321f;
      Assert.AreEqual(0.0f, testScalar.Fraction);
      Assert.AreEqual(654.321f, testScalar.Offset);
    }
    
    /// <summary>
    ///   Verifies that the ToOffset() method converts a scalar consisting only of an offset
    /// </summary>
    [Test]
    public void TestToOffsetWithOffsetOnly() {
      UniScalar testScalar = new UniScalar(0.0f, 987.654f);
      Assert.AreEqual(987.654f, testScalar.ToOffset(1000.0f));
    }

    /// <summary>
    ///   Verifies that the ToOffset() method converts a normal scalar
    /// </summary>
    [Test]
    public void TestToOffsetWithNormalScalar() {
      UniScalar testScalar = new UniScalar(1.23f, 4.5f);
      Assert.AreEqual(1234.5f, testScalar.ToOffset(1000.0f));
    }

    /// <summary>Tests the addition operator of the unified scalar class</summary>
    [Test]
    public void TestAdditionOperator() {
      UniScalar testScalar = new UniScalar(1.2f, 3.4f);
      UniScalar addedScalar = new UniScalar(5.6f, 7.8f);
      UniAssertHelper.AreAlmostEqual(
        new UniScalar(6.8f, 11.2f), testScalar + addedScalar, 4
      );
    }

    /// <summary>Tests the subtraction operator of the unified scalar class</summary>
    [Test]
    public void TestSubtractionOperator() {
      UniScalar testScalar = new UniScalar(1.2f, 3.4f);
      UniScalar subtractedScalar = new UniScalar(5.6f, 7.8f);
      UniAssertHelper.AreAlmostEqual(
        new UniScalar(-4.4f, -4.4f), testScalar - subtractedScalar, 4
      );
    }

    /// <summary>Tests the multiplication operator of the unified scalar class</summary>
    [Test]
    public void TestMultiplicationOperator() {
      UniScalar testScalar = new UniScalar(1.2f, 3.4f);
      UniScalar factorScalar = new UniScalar(5.6f, 7.8f);
      UniAssertHelper.AreAlmostEqual(
        new UniScalar(6.72f, 26.52f), testScalar * factorScalar, 4
      );
    }

    /// <summary>Tests the division operator of the unified scalar class</summary>
    [Test]
    public void TestDivisionOperator() {
      UniScalar testScalar = new UniScalar(1.2f, 3.4f);
      UniScalar divisorScalar = new UniScalar(2.5f, 5.0f);
      UniAssertHelper.AreAlmostEqual(
        new UniScalar(0.48f, 0.68f), testScalar / divisorScalar, 4
      );
    }

    /// <summary>Tests the equality operator of the unified scalar class</summary>
    [Test]
    public void TestEqualityOperator() {
      UniScalar testScalar = new UniScalar(12.34f, 56.78f);
      UniScalar equivalentScalar = new UniScalar(12.34f, 56.78f);
      UniScalar differingScalar = new UniScalar(87.65f, 43.21f);
      
      Assert.IsTrue(testScalar == equivalentScalar);
      Assert.IsFalse(testScalar == differingScalar);
    }

    /// <summary>Test the inequality operator of the unified scalar class</summary>
    [Test]
    public void TestInequalityOperator() {
      UniScalar testScalar = new UniScalar(12.34f, 56.78f);
      UniScalar equivalentScalar = new UniScalar(12.34f, 56.78f);
      UniScalar differingScalar = new UniScalar(87.65f, 43.21f);

      Assert.IsFalse(testScalar != equivalentScalar);
      Assert.IsTrue(testScalar != differingScalar);
    }

    /// <summary>
    ///   Tests the Equals() method of the unified scalar class when it has to perform
    ///   a downcast to obtain the comparison scalar
    /// </summary>
    [Test]
    public void TestEqualsWithDowncast() {
      UniScalar testScalar = new UniScalar(12.34f, 56.78f);
      UniScalar equivalentScalar = new UniScalar(12.34f, 56.78f);
      UniScalar differingScalar = new UniScalar(87.65f, 43.21f);

      Assert.IsTrue(testScalar.Equals((object)equivalentScalar));
      Assert.IsFalse(testScalar.Equals((object)differingScalar));
    }


    /// <summary>
    ///   Tests the Equals() method of the unified scalar class against a different type
    /// </summary>
    [Test]
    public void TestEqualsWithDifferentType() {
      UniScalar testScalar = new UniScalar(12.34f, 56.78f);

      Assert.IsFalse(testScalar.Equals(DateTime.MinValue));
    }

    /// <summary>
    ///   Tests the Equals() method of the unified scalar class against a null pointer
    /// </summary>
    [Test]
    public void TestEqualsWithNullReference() {
      UniScalar testScalar = new UniScalar(12.34f, 56.78f);

      Assert.IsFalse(testScalar.Equals(null));
    }

    /// <summary>
    ///   Tests the GetHashCode() method of the unified scalar class
    /// </summary>
    [Test]
    public void TestGetHashCode() {
      UniScalar testScalar = new UniScalar(12.34f, 56.78f);
      UniScalar equivalentScalar = new UniScalar(12.34f, 56.78f);

      Assert.AreEqual(testScalar.GetHashCode(), equivalentScalar.GetHashCode());
    }

    /// <summary>
    ///   Tests the ToString() method of the unified scalar class
    /// </summary>
    [Test]
    public void TestToString() {
      Assert.IsNotNull(UniScalar.Zero.ToString());
    }

  }

} // namespace Nuclex.UserInterface

#endif // UNITTEST
