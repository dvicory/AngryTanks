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
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the Priority/Item pair class</summary>
  [TestFixture]
  public class PriorityItemPairTest {

    #region class ToStringNullReturner

    /// <summary>Test class in which ToString() can return null</summary>
    private class ToStringNullReturner {

      /// <summary>
      ///   Returns a System.String that represents the current System.Object
      /// </summary>
      /// <returns>A System.String that represents the current System.Object</returns>
      public override string ToString() { return null; }

    }

    #endregion // class ToStringNullReturner

    /// <summary>Tests whether the pair's default constructor works</summary>
    [Test]
    public void TestDefaultConstructor() {
      new PriorityItemPair<int, string>();
    }

    /// <summary>Tests whether the priority can be retrieved from the pair</summary>
    [Test]
    public void TestPriorityRetrieval() {
      PriorityItemPair<int, string> testPair = new PriorityItemPair<int, string>(
        12345, "hello world"
      );

      Assert.AreEqual(12345, testPair.Priority);
    }

    /// <summary>Tests whether the item can be retrieved from the pair</summary>
    [Test]
    public void TestItemRetrieval() {
      PriorityItemPair<int, string> testPair = new PriorityItemPair<int, string>(
        12345, "hello world"
      );

      Assert.AreEqual("hello world", testPair.Item);
    }

    /// <summary>Tests whether the ToString() methods works with valid strings</summary>
    [Test]
    public void TestToStringWithValidStrings() {
      PriorityItemPair<string, string> testPair = new PriorityItemPair<string, string>(
        "hello", "world"
      );

      Assert.AreEqual("[hello, world]", testPair.ToString());
    }

    /// <summary>Tests whether the ToString() methods works with null strings</summary>
    [Test]
    public void TestToStringWithNullStrings() {
      PriorityItemPair<ToStringNullReturner, ToStringNullReturner> testPair =
        new PriorityItemPair<ToStringNullReturner, ToStringNullReturner>(
          new ToStringNullReturner(), new ToStringNullReturner()
        );

      Assert.AreEqual("[, ]", testPair.ToString());
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
