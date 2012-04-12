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

  /// <summary>Contains unit tests for the integer helper class</summary>
  [TestFixture]
  public class IntegerHelperTest {

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for long integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2ULong() {
      Assert.AreEqual(1UL, IntegerHelper.NextPowerOf2(0UL));
      Assert.AreEqual(1UL, IntegerHelper.NextPowerOf2(1UL));
      Assert.AreEqual(2UL, IntegerHelper.NextPowerOf2(2UL));
      Assert.AreEqual(4UL, IntegerHelper.NextPowerOf2(3UL));
      Assert.AreEqual(4UL, IntegerHelper.NextPowerOf2(4UL));
      Assert.AreEqual(
        9223372036854775808UL, IntegerHelper.NextPowerOf2(4611686018427387905UL)
      );
      Assert.AreEqual(
        9223372036854775808UL, IntegerHelper.NextPowerOf2(9223372036854775807UL)
      );
      Assert.AreEqual(
        9223372036854775808UL, IntegerHelper.NextPowerOf2(9223372036854775808UL)
      );
    }

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for long integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2Long() {
      Assert.AreEqual(1L, IntegerHelper.NextPowerOf2(0L));
      Assert.AreEqual(1L, IntegerHelper.NextPowerOf2(1L));
      Assert.AreEqual(2L, IntegerHelper.NextPowerOf2(2L));
      Assert.AreEqual(4L, IntegerHelper.NextPowerOf2(3L));
      Assert.AreEqual(4L, IntegerHelper.NextPowerOf2(4L));
      Assert.AreEqual(4611686018427387904L, IntegerHelper.NextPowerOf2(2305843009213693953L));
      Assert.AreEqual(4611686018427387904L, IntegerHelper.NextPowerOf2(4611686018427387903L));
      Assert.AreEqual(4611686018427387904L, IntegerHelper.NextPowerOf2(4611686018427387904L));
    }

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2UInt() {
      Assert.AreEqual(1U, IntegerHelper.NextPowerOf2(0U));
      Assert.AreEqual(1U, IntegerHelper.NextPowerOf2(1U));
      Assert.AreEqual(2U, IntegerHelper.NextPowerOf2(2U));
      Assert.AreEqual(4U, IntegerHelper.NextPowerOf2(3U));
      Assert.AreEqual(4U, IntegerHelper.NextPowerOf2(4U));
      Assert.AreEqual(2147483648U, IntegerHelper.NextPowerOf2(1073741825U));
      Assert.AreEqual(2147483648U, IntegerHelper.NextPowerOf2(2147483647U));
      Assert.AreEqual(2147483648U, IntegerHelper.NextPowerOf2(2147483648U));
    }

    /// <summary>
    ///   Verifies that the next power of 2 calculation works for integers
    /// </summary>
    [Test]
    public void TestNextPowerOf2Int() {
      Assert.AreEqual(1, IntegerHelper.NextPowerOf2(0));
      Assert.AreEqual(1, IntegerHelper.NextPowerOf2(1));
      Assert.AreEqual(2, IntegerHelper.NextPowerOf2(2));
      Assert.AreEqual(4, IntegerHelper.NextPowerOf2(3));
      Assert.AreEqual(4, IntegerHelper.NextPowerOf2(4));
      Assert.AreEqual(1073741824, IntegerHelper.NextPowerOf2(536870913));
      Assert.AreEqual(1073741824, IntegerHelper.NextPowerOf2(1073741823));
      Assert.AreEqual(1073741824, IntegerHelper.NextPowerOf2(1073741824));
    }


  }

} // namespace Nuclex.Support

#endif // UNITTEST