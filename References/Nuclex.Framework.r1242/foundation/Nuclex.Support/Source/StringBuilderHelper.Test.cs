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
using System.Text;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>
  ///   Unit test for the helper class to .NET's string builder
  /// </summary>
  [TestFixture]
  public class StringBuilderHelperTest {

    /// <summary>
    ///   Verifies that bytes are correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendByte() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, (byte)255);

      Assert.AreEqual("255", builder.ToString());
    }

    /// <summary>
    ///   Verifies that a byte with value 0 is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNullByte() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, (byte)0);

      Assert.AreEqual("0", builder.ToString());
    }

    /// <summary>
    ///   Verifies that a positive integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendPositiveInteger() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 12345);

      Assert.AreEqual("12345", builder.ToString());
    }

    /// <summary>
    ///   Verifies that an integer with value 0 is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNullInteger() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0);

      Assert.AreEqual("0", builder.ToString());
    }

    /// <summary>
    ///   Verifies that a negative integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNegativeInteger() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, -12345);

      Assert.AreEqual("-12345", builder.ToString());
    }

    /// <summary>
    ///   Verifies that a positive long integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendPositiveLong() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 12345L);

      Assert.AreEqual("12345", builder.ToString());
    }

    /// <summary>
    ///   Verifies that a long integer with value 0 is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNullLong() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0L);

      Assert.AreEqual("0", builder.ToString());
    }

    /// <summary>
    ///   Verifies that a negative long integer is correctly appended to a string builder
    /// </summary>
    [Test]
    public void TestAppendNegativeLong() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, -12345L);

      Assert.AreEqual("-12345", builder.ToString());
    }

    /// <summary>
    ///   Verifies that negative floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendNegativeFloat() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, -32.015625f);

      Assert.AreEqual("-32.015625", builder.ToString());
    }

    /// <summary>
    ///   Verifies that positive floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendPositiveFloat() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 10.0625f);

      Assert.AreEqual("10.0625", builder.ToString());
    }

    /// <summary>
    ///   Verifies that very small floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendSmallFloat() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625f);

      Assert.AreEqual("0.00390625", builder.ToString());
    }

    /// <summary>
    ///   Verifies that very large floating point values are correctly converted
    /// </summary>
    [Test]
    public void TestAppendHugeFloat() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 1000000000.0f);

      Assert.AreEqual("1000000000.0", builder.ToString());
    }

    /// <summary>Tests whether the number of decimal places can be restricted</summary>
    [Test]
    public void TestAppendFloatLimitDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625f, 3);

      Assert.AreEqual("0.003", builder.ToString());
    }

    /// <summary>
    ///   Verifies that a float with no decimal places is correctly appended
    /// </summary>
    [Test]
    public void TestAppendFloatWithoutDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625f, 0);

      Assert.AreEqual("0", builder.ToString()); // Note: no rounding!
    }

    /// <summary>
    ///   Verifies the behavior of the helper with unsupported floating point values
    /// </summary>
    [Test]
    public void TestAppendOutOfRangeFloat() {
      StringBuilder builder = new StringBuilder();
      Assert.IsFalse(StringBuilderHelper.Append(builder, float.PositiveInfinity));
      Assert.IsFalse(StringBuilderHelper.Append(builder, float.NegativeInfinity));
      Assert.IsFalse(StringBuilderHelper.Append(builder, float.NaN));
      Assert.IsFalse(StringBuilderHelper.Append(builder, 0.000000059604644775390625f));
    }

    /// <summary>
    ///   Verifies that negative double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendNegativeDouble() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, -32.015625);

      Assert.AreEqual("-32.015625", builder.ToString());
    }

    /// <summary>
    ///   Verifies that positive double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendPositiveDouble() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 10.0625);

      Assert.AreEqual("10.0625", builder.ToString());
    }

    /// <summary>
    ///   Verifies that very small double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendSmallDouble() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625);

      Assert.AreEqual("0.00390625", builder.ToString());
    }

    /// <summary>
    ///   Verifies that very large double precision floating point values are
    ///   correctly converted
    /// </summary>
    [Test]
    public void TestAppendHugeDouble() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 1000000000000000000.0);

      Assert.AreEqual("1000000000000000000.0", builder.ToString());
    }

    /// <summary>Tests whether the number of decimal places can be restricted</summary>
    [Test]
    public void TestAppendDoubleLimitDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625, 3);

      Assert.AreEqual("0.003", builder.ToString()); // Note: no rounding!
    }

    /// <summary>
    ///   Verifies that a double with no decimal places is correctly appended
    /// </summary>
    [Test]
    public void TestAppendDoubleWithoutDecimalPlaces() {
      StringBuilder builder = new StringBuilder();
      StringBuilderHelper.Append(builder, 0.00390625, 0);

      Assert.AreEqual("0", builder.ToString());
    }

    /// <summary>
    ///   Verifies the behavior of the helper with unsupported double precision
    ///   floating point values
    /// </summary>
    [Test]
    public void TestAppendOutOfRangeDouble() {
      StringBuilder builder = new StringBuilder();
      Assert.IsFalse(StringBuilderHelper.Append(builder, double.PositiveInfinity));
      Assert.IsFalse(StringBuilderHelper.Append(builder, double.NegativeInfinity));
      Assert.IsFalse(StringBuilderHelper.Append(builder, double.NaN));
      Assert.IsFalse(
        StringBuilderHelper.Append(builder, 1.1102230246251565404236316680908e-16)
      );
    }

    /// <summary>
    ///   Verifies that the contents of a string builder can be cleared
    /// </summary>
    [Test]
    public void TestClear() {
      StringBuilder builder = new StringBuilder("Hello World");
      StringBuilderHelper.Clear(builder);

      Assert.AreEqual(string.Empty, builder.ToString());
    }
    
  }

} // namespace Nuclex.Support

#endif // UNITTEST
