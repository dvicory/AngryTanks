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

  /// <summary>Unit Test for the NUnit assertion helper</summary>
  [TestFixture]
  public class GeoAssertHelperTest {

    /// <summary>Tests whether the almost equal check works with vectors</summary>
    [Test]
    public void TestAlmostEqualWithVectors() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 minusOneVector = new Vector3(minusOneFloat, minusOneFloat, minusOneFloat);
      Vector3 plusOneVector = new Vector3(plusOneFloat, plusOneFloat, plusOneFloat);

      GeoAssertHelper.AreAlmostEqual(exactVector, minusOneVector, 1);
      GeoAssertHelper.AreAlmostEqual(exactVector, plusOneVector, 1);
    }

    /// <summary>
    ///   Tests whether the almost equal check detects an X component in a vector that
    ///   is just barely too low
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualWithTooLowXInVector() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 minusTwoVector = new Vector3(minusTwoFloat, exactFloat, exactFloat);
      Assert.Throws<AssertionException>(
        delegate() {
          GeoAssertHelper.AreAlmostEqual(exactVector, minusTwoVector, 1);
        }
      );
    }

    /// <summary>
    ///   Tests whether the almost equal check detects an X component in a vector that
    ///   is just barely too high
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualWithTooHighXInVector() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 plusTwoVector = new Vector3(plusTwoFloat, exactFloat, exactFloat);
      Assert.Throws<AssertionException>(
        delegate() { GeoAssertHelper.AreAlmostEqual(exactVector, plusTwoVector, 1); }
      );
    }

    /// <summary>
    ///   Tests whether the almost equal check detects an Y component in a vector that
    ///   is just barely too low
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualWithTooLowYInVector() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 minusTwoVector = new Vector3(exactFloat, minusTwoFloat, exactFloat);
      Assert.Throws<AssertionException>(
        delegate() { GeoAssertHelper.AreAlmostEqual(exactVector, minusTwoVector, 1); }
      );
    }

    /// <summary>
    ///   Tests whether the almost equal check detects an Y component in a vector that
    ///   is just barely too high
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualWithTooHighYInVector() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 plusTwoVector = new Vector3(exactFloat, plusTwoFloat, exactFloat);
      Assert.Throws<AssertionException>(
        delegate() { GeoAssertHelper.AreAlmostEqual(exactVector, plusTwoVector, 1); }
      );
    }

    /// <summary>
    ///   Tests whether the almost equal check detects an Z component in a vector that
    ///   is just barely too low
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualWithTooLowZInVector() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 minusTwoVector = new Vector3(exactFloat, exactFloat, minusTwoFloat);
      Assert.Throws<AssertionException>(
        delegate() { GeoAssertHelper.AreAlmostEqual(exactVector, minusTwoVector, 1); }
      );
    }

    /// <summary>
    ///   Tests whether the almost equal check detects an Z component in a vector that
    ///   is just barely too high
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualWithTooHighZInVector() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 plusTwoVector = new Vector3(exactFloat, exactFloat, plusTwoFloat);
      Assert.Throws<AssertionException>(
        delegate() { GeoAssertHelper.AreAlmostEqual(exactVector, plusTwoVector, 1); }
      );
    }

    /// <summary>
    ///   Verifies that the AreAlmostEqual() helper works correctly when comparing
    ///   two axis aligned boxes
    /// </summary>
    [Test]
    public void TestAlmostEqualWithAxisAlignedBoxes() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 minusOneVector = new Vector3(minusOneFloat, minusOneFloat, minusOneFloat);
      Vector3 plusOneVector = new Vector3(plusOneFloat, plusOneFloat, plusOneFloat);

      Volumes.AxisAlignedBox3 oneOffAabb = new Volumes.AxisAlignedBox3(
        minusOneVector, plusOneVector
      );
      Volumes.AxisAlignedBox3 exactAabb = new Volumes.AxisAlignedBox3(
        exactVector, exactVector
      );

      GeoAssertHelper.AreAlmostEqual(exactAabb, oneOffAabb, 1);
    }

    /// <summary>
    ///   Verifies that the AreAlmostEqual() helper throws an exception when the compared
    ///   boxes differ by more than the allowed amount
    /// </summary>
    [Test]
    public void TestThrowOnAlmostEqualWithTooLargeAxisAlignedBox() {
      Vector3 exactVector = new Vector3(exactFloat, exactFloat, exactFloat);
      Vector3 minusTwoVector = new Vector3(minusTwoFloat, minusTwoFloat, minusTwoFloat);
      Vector3 plusTwoVector = new Vector3(plusTwoFloat, plusTwoFloat, plusTwoFloat);

      Volumes.AxisAlignedBox3 twoOffAabb = new Volumes.AxisAlignedBox3(
        minusTwoVector, plusTwoVector
      );
      Volumes.AxisAlignedBox3 exactAabb = new Volumes.AxisAlignedBox3(
        exactVector, exactVector
      );

      Assert.Throws<AssertionException>(
        delegate() { GeoAssertHelper.AreAlmostEqual(exactAabb, twoOffAabb, 1); }
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

    /// <summary>
    ///   Adjusts a double precision floating point value by the specified amount of
    ///   neighbouring representable values
    /// </summary>
    /// <param name="value">Double precision floating point value to be adjusted</param>
    /// <param name="ulps">Numbers of neighbouring representable values to step</param>
    /// <returns>The adjusted double precision floating point value</returns>
    private static double adjust(double value, long ulps) {
      return FloatHelper.ReinterpretAsDouble(FloatHelper.ReinterpretAsLong(value) + ulps);
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

    /// <summary>The exact test value as a float</summary>
    private static readonly double exactDouble = 1234.5678f;
    /// <summary>The second next possible smaller float from the test value</summary>
    private static readonly double minusTwoDouble = adjust(exactDouble, -2);
    /// <summary>The next possible smaller float from the test value</summary>
    private static readonly double minusOneDouble = adjust(exactDouble, -1);
    /// <summary>The next possible greater float from the test value</summary>
    private static readonly double plusOneDouble = adjust(exactDouble, +1);
    /// <summary>The second next possible greater float from the test value</summary>
    private static readonly double plusTwoDouble = adjust(exactDouble, +2);

  }

} // namespace Nuclex.Geometry

#endif // UNITTEST
