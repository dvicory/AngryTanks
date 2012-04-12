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
using System.Windows.Forms;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Input {

  /// <summary>Unit tests for the collection helper</summary>
  [TestFixture]
  internal class CollectionHelperTest {

    #region class Test

    /// <summary>Test implementation of the IDisposable interface</summary>
    private class Test { }

    #endregion // class Test

    #region class TestDisposable

    /// <summary>Test implementation of the IDisposable interface</summary>
    private class TestDisposable : Test, IDisposable {

      /// <summary>Called to dispose the instance</summary>
      public void Dispose() {
        ++this.DisposeCallCount;
      }

      /// <summary>Number of times the Dispose() method has been called</summary>
      public int DisposeCallCount;

    }

    #endregion // class TestDisposable

    /// <summary>Verifies that the Contains() method is working</summary>
    [Test]
    public void TestGetIfExists() {
      var ints = new List<int>(new int[] { 10, 20, 30, 40 });

      Assert.AreEqual(10, CollectionHelper.GetIfExists(ints, 0));
      Assert.AreEqual(20, CollectionHelper.GetIfExists(ints, 1));
      Assert.AreEqual(30, CollectionHelper.GetIfExists(ints, 2));
      Assert.AreEqual(40, CollectionHelper.GetIfExists(ints, 3));
      Assert.AreEqual(0, CollectionHelper.GetIfExists(ints, 4));
      Assert.AreEqual(0, CollectionHelper.GetIfExists(ints, 5));
    }

    /// <summary>Verifies that the DisposeItems() method is working correctly</summary>
    [Test]
    public void TestDisposeItems() {
      var tests = new List<Test>();

      var disposable = new TestDisposable();
      var test = new Test();

      tests.Add(disposable);
      tests.Add(test);

      Assert.AreEqual(0, disposable.DisposeCallCount);
      CollectionHelper.DisposeItems(tests);
      Assert.AreEqual(1, disposable.DisposeCallCount);
    }

  }

} // namespace Nuclex.Input

#endif // UNITTEST
