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

  /// <summary>Unit Test for the item event argument container</summary>
  [TestFixture]
  public class ItemEventArgsTest {

    /// <summary>
    ///   Tests whether an integer argument can be stored in the argument container
    /// </summary>
    [Test]
    public void TestIntegerArgument() {
      ItemEventArgs<int> test = new ItemEventArgs<int>(12345);
      Assert.AreEqual(12345, test.Item);
    }

    /// <summary>
    ///   Tests whether a string argument can be stored in the argument container
    /// </summary>
    [Test]
    public void TestStringArgument() {
      ItemEventArgs<string> test = new ItemEventArgs<string>("hello world");
      Assert.AreEqual("hello world", test.Item);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
