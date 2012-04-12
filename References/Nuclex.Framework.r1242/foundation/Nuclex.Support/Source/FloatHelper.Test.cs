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
using System.Collections.Generic;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the FloatHelper class</summary>
  [TestFixture]
  public class FloatHelperTest {

    /// <summary>Tests the floating point value comparison helper</summary>
    [Test]
    public void TestFloatComparison() {
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(0.00000001f, 0.0000000100000008f, 1),
        "Minimal difference between very small floating point numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(0.00000001f, 0.0000000100000017f, 1),
        "Larger difference between very small floating point numbers is not considered equal"
      );

      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(1000000.00f, 1000000.06f, 1),
        "Minimal difference between very large floating point numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(1000000.00f, 1000000.13f, 1),
        "Larger difference between very large floating point numbers is not considered equal"
      );
    }

    /// <summary>Tests the double precision floating point value comparison helper</summary>
    [Test]
    public void TestDoubleComparison() {
      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(0.00000001, 0.000000010000000000000002, 1),
        "Minimal difference between very small double precision floating point " +
        "numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(0.00000001, 0.000000010000000000000004, 1),
        "Larger difference between very small double precision floating point " +
        "numbers is not considered equal"
      );

      Assert.IsTrue(
        FloatHelper.AreAlmostEqual(1000000.00, 1000000.0000000001, 1),
        "Minimal difference between very large double precision floating point " +
        "numbers is considered equal"
      );
      Assert.IsFalse(
        FloatHelper.AreAlmostEqual(1000000.00, 1000000.0000000002, 1),
        "Larger difference between very large double precision floating point " +
        "numbers is not considered equal"
      );
    }

    /// <summary>Tests the integer reinterpretation functions</summary>
    [Test]
    public void TestIntegerReinterpretation() {
      Assert.AreEqual(
        12345.0f,
        FloatHelper.ReinterpretAsFloat(FloatHelper.ReinterpretAsInt(12345.0f)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }

    /// <summary>Tests the long reinterpretation functions</summary>
    [Test]
    public void TestLongReinterpretation() {
      Assert.AreEqual(
        12345.67890,
        FloatHelper.ReinterpretAsDouble(FloatHelper.ReinterpretAsLong(12345.67890)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }

    /// <summary>Tests the floating point reinterpretation functions</summary>
    [Test]
    public void TestFloatReinterpretation() {
      Assert.AreEqual(
        12345,
        FloatHelper.ReinterpretAsInt(FloatHelper.ReinterpretAsFloat(12345)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }


    /// <summary>
    ///   Tests the double prevision floating point reinterpretation functions
    /// </summary>
    [Test]
    public void TestDoubleReinterpretation() {
      Assert.AreEqual(
        1234567890,
        FloatHelper.ReinterpretAsLong(FloatHelper.ReinterpretAsDouble(1234567890)),
        "Number hasn't changed after mirrored reinterpretation"
      );
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
