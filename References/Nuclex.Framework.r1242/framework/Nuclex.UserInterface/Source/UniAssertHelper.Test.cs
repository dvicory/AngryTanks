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

using Nuclex.Support;

namespace Nuclex.UserInterface {

  /// <summary>Unit Test for the unified value assertion helper class</summary>
  [TestFixture]
  internal class UniAssertHelperTest {

    /// <summary>
    ///   Validates that a deviation within the allowed tolerance is accepted as being equal
    ///   for a unified scalar value
    /// </summary>
    [Test]
    public void TestAlmostEqualScalars() {
      UniScalar testScalar = new UniScalar(minusOneFloat, plusOneFloat);
      UniAssertHelper.AreAlmostEqual(new UniScalar(exactFloat, exactFloat), testScalar, 1);
    }

    /// <summary>
    ///   Validates that a deviation of the fraction field outside of the tolerance results
    ///   in the assertion failing
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualScalarsWithTooHighFraction() {
      UniScalar testScalar = new UniScalar(plusTwoFloat, exactFloat);
      Assert.Throws<AssertionException>(
        delegate() {
          UniAssertHelper.AreAlmostEqual(
            new UniScalar(exactFloat, exactFloat), testScalar, 1
          );
        }
      );
    }

    /// <summary>
    ///   Validates that a deviation of the offset field outside of the tolerance results
    ///   in the assertion failing
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualScalarsWithTooLowOffset() {
      UniScalar testScalar = new UniScalar(exactFloat, minusTwoFloat);
      Assert.Throws<AssertionException>(
        delegate() {
          UniAssertHelper.AreAlmostEqual(
            new UniScalar(exactFloat, exactFloat), testScalar, 1
          );
        }
      );
    }

    /// <summary>
    ///   Validates that a deviation within the allowed tolerance is accepted as being equal
    ///   for a unified vector
    /// </summary>
    [Test]
    public void TestAlmostEqualVectors() {
      UniVector testVector = new UniVector(
        new UniScalar(minusOneFloat, plusOneFloat),
        new UniScalar(plusOneFloat, minusOneFloat)
      );
      UniVector exactVector = new UniVector(
        new UniScalar(exactFloat, exactFloat),
        new UniScalar(exactFloat, exactFloat)
      );

      UniAssertHelper.AreAlmostEqual(testVector, exactVector, 1);
    }

    /// <summary>
    ///   Validates that a deviation of the X field outside of the tolerance results
    ///   in the assertion failing
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualVectorsWithXDifference() {
      UniVector testVector = new UniVector(
        new UniScalar(minusTwoFloat, plusTwoFloat),
        new UniScalar(plusOneFloat, minusOneFloat)
      );
      UniVector exactVector = new UniVector(
        new UniScalar(exactFloat, exactFloat),
        new UniScalar(exactFloat, exactFloat)
      );

      Assert.Throws<AssertionException>(
        delegate() { UniAssertHelper.AreAlmostEqual(exactVector, testVector, 1); }
      );
    }

    /// <summary>
    ///   Validates that a deviation of the Y field outside of the tolerance results
    ///   in the assertion failing
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualVectorsWithYDifference() {
      UniVector testVector = new UniVector(
        new UniScalar(minusOneFloat, plusOneFloat),
        new UniScalar(plusTwoFloat, minusTwoFloat)
      );
      UniVector exactVector = new UniVector(
        new UniScalar(exactFloat, exactFloat),
        new UniScalar(exactFloat, exactFloat)
      );

      Assert.Throws<AssertionException>(
        delegate() { UniAssertHelper.AreAlmostEqual(exactVector, testVector, 1); }
      );
    }

    /// <summary>
    ///   Validates that a deviation within the allowed tolerance is accepted as being equal
    ///   for a unified rectangle
    /// </summary>
    [Test]
    public void TestAlmostEqualRectangles() {
      UniRectangle testRectangle = new UniRectangle(
        new UniVector(
          new UniScalar(minusOneFloat, plusOneFloat),
          new UniScalar(plusOneFloat, minusOneFloat)
        ),
        new UniVector(
          new UniScalar(plusOneFloat, minusOneFloat),
          new UniScalar(minusOneFloat, plusOneFloat)
        )
      );
      UniRectangle exactRectangle = new UniRectangle(
        new UniVector(
          new UniScalar(exactFloat, exactFloat),
          new UniScalar(exactFloat, exactFloat)
        ),
        new UniVector(
          new UniScalar(exactFloat, exactFloat),
          new UniScalar(exactFloat, exactFloat)
        )
      );

      UniAssertHelper.AreAlmostEqual(exactRectangle, testRectangle, 1);
    }

    /// <summary>
    ///   Validates that a deviation of the Location field outside of the tolerance results
    ///   in the assertion failing
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualVectorsWithLocationDifference() {
      UniRectangle testRectangle = new UniRectangle(
        new UniVector(
          new UniScalar(minusTwoFloat, plusTwoFloat),
          new UniScalar(plusTwoFloat, minusTwoFloat)
        ),
        new UniVector(
          new UniScalar(plusOneFloat, minusOneFloat),
          new UniScalar(minusOneFloat, plusOneFloat)
        )
      );
      UniRectangle exactRectangle = new UniRectangle(
        new UniVector(
          new UniScalar(exactFloat, exactFloat),
          new UniScalar(exactFloat, exactFloat)
        ),
        new UniVector(
          new UniScalar(exactFloat, exactFloat),
          new UniScalar(exactFloat, exactFloat)
        )
      );

      Assert.Throws<AssertionException>(
        delegate() { UniAssertHelper.AreAlmostEqual(exactRectangle, testRectangle, 1); }
      );
    }

    /// <summary>
    ///   Validates that a deviation of the Size field outside of the tolerance results
    ///   in the assertion failing
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualVectorsWithSizeDifference() {
      UniRectangle testRectangle = new UniRectangle(
        new UniVector(
          new UniScalar(minusOneFloat, plusOneFloat),
          new UniScalar(plusOneFloat, minusOneFloat)
        ),
        new UniVector(
          new UniScalar(plusTwoFloat, minusTwoFloat),
          new UniScalar(minusTwoFloat, plusTwoFloat)
        )
      );
      UniRectangle exactRectangle = new UniRectangle(
        new UniVector(
          new UniScalar(exactFloat, exactFloat),
          new UniScalar(exactFloat, exactFloat)
        ),
        new UniVector(
          new UniScalar(exactFloat, exactFloat),
          new UniScalar(exactFloat, exactFloat)
        )
      );

      Assert.Throws<AssertionException>(
        delegate() { UniAssertHelper.AreAlmostEqual(exactRectangle, testRectangle, 1); }
      );
    }

    /// <summary>
    ///   Adjusts a floating point value by the specified amount of neighbouring
    ///   representable values
    /// </summary>
    /// <param name="value">Floating point value to be adjusted</param>
    /// <param name="ulps">Numbers of neighbouring representable values to step</param>
    /// <returns>The adjusted floating point value</returns>
    private static float adjust(float value, int ulps) {
      return FloatHelper.ReinterpretAsFloat(FloatHelper.ReinterpretAsInt(value) + ulps);
    }

    /// <summary>The exact test value as a float</summary>
    private static readonly float exactFloat = 1234.5678f;
    /// <summary>The second next possible smaller float from the test value</summary>
    private static readonly float minusTwoFloat = adjust(exactFloat, -2);
    /// <summary>The next possible smaller float from the test value</summary>
    private static readonly float minusOneFloat = adjust(exactFloat, -1);
    /// <summary>The next possible greater float from the test value</summary>
    private static readonly float plusOneFloat = adjust(exactFloat, +1);
    /// <summary>The second next possible greater float from the test value</summary>
    private static readonly float plusTwoFloat = adjust(exactFloat, +2);

  }

} // namespace Nuclex.UserInterface

#endif // UNITTEST
