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
using System.IO;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the enumeration helper class</summary>
  [TestFixture]
  public class EnumHelperTest {

    #region enum TestEnumeration

    /// <summary>An enumeration used for unit testing</summary>
    internal enum TestEnumeration {
      /// <summary>First arbitrary enumeration value</summary>
      One = -2,
      /// <summary>Third arbitrary enumeration value</summary>
      Three = 33,
      /// <summary>Second arbitrary enumeration value</summary>
      Two = 23
    }

    #endregion // enum TestEnumeration

    #region enum EmptyEnumeration

    internal enum EmptyEnumeration { }

    #endregion // enum EmptyEnumeration

    /// <summary>
    ///   Verifies that the enum helper can list the members of an enumeration
    ///   manually (as it needs to be done on the XBox 360)
    /// </summary>
    [Test]
    public void TestGetValuesXbox360() {
      CollectionAssert.AreEquivalent(
        new TestEnumeration[] {
          TestEnumeration.One, TestEnumeration.Two, TestEnumeration.Three
        },
        EnumHelper.GetValuesXbox360<TestEnumeration>()
      );
    }

    /// <summary>
    ///   Verifies that the enum helper can list the members of an enumeration
    /// </summary>
    [Test]
    public void TestGetValues() {
      CollectionAssert.AreEquivalent(
        new TestEnumeration[] {
          TestEnumeration.One, TestEnumeration.Two, TestEnumeration.Three
        },
        EnumHelper.GetValues<TestEnumeration>()
      );
    }

    /// <summary>
    ///   Verifies that the enum helper can locate the highest value in an enumeration
    /// </summary>
    [Test]
    public void TestGetHighestValue() {
      Assert.AreEqual(
        TestEnumeration.Three, EnumHelper.GetHighestValue<TestEnumeration>()
      );
    }

    /// <summary>
    ///   Verifies that the enum helper can locate the lowest value in an enumeration
    /// </summary>
    [Test]
    public void TestGetLowestValue() {
      Assert.AreEqual(
        TestEnumeration.One, EnumHelper.GetLowestValue<TestEnumeration>()
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the GetValuesXbox360() method is
    ///   used on a non-enumeration type
    /// </summary>
    [Test]
    public void TestThrowOnNonEnumTypeXbox360() {
      Assert.Throws<ArgumentException>(
        delegate() { EnumHelper.GetValuesXbox360<int>(); }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the GetValues() method is used on
    ///   a non-enumeration type
    /// </summary>
    [Test]
    public void TestThrowOnNonEnumType() {
      Assert.Throws<ArgumentException>(
        delegate() { EnumHelper.GetValues<int>(); }
      );
    }

    /// <summary>
    ///   Verifies that the default value for an enumeration is returned if
    ///   the GetLowestValue() method is used on an empty enumeration
    /// </summary>
    [Test]
    public void TestLowestValueInEmptyEnumeration() {
      Assert.AreEqual(
        default(EmptyEnumeration), EnumHelper.GetLowestValue<EmptyEnumeration>()
      );
    }

    /// <summary>
    ///   Verifies that the default value for an enumeration is returned if
    ///   the GetHighestValue() method is used on an empty enumeration
    /// </summary>
    [Test]
    public void TestHighestValueInEmptyEnumeration() {
      Assert.AreEqual(
        default(EmptyEnumeration), EnumHelper.GetHighestValue<EmptyEnumeration>()
      );
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
