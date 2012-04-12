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

  /// <summary>Unit tests for the Pool class</summary>
  [TestFixture]
  public class PoolTest {

    #region class TestClass

    /// <summary>Used to test the pool</summary>
    private class TestClass : IRecyclable {

      /// <summary>Returns the object to its initial state</summary>
      public void Recycle() {
        this.Recycled = true;
      }

      /// <summary>Whether the instance has been recycled</summary>
      public bool Recycled;

    }

    #endregion // class TestClass

    /// <summary>
    ///   Verifies that the pool can return newly constructed objects
    /// </summary>
    [Test]
    public void TestGet() {
      Pool<TestClass> pool = new Pool<TestClass>();
      Assert.IsNotNull(pool.Get());
    }

    /// <summary>
    ///   Verifies that the pool will return a recycled object if one is available
    /// </summary>
    [Test]
    public void TestGetRecycled() {
      Pool<TestClass> pool = new Pool<TestClass>();
      pool.Redeem(new TestClass());

      TestClass test = pool.Get();
      Assert.IsTrue(test.Recycled);
    }

    /// <summary>
    ///   Tests whether the pool can redeem objects that are no longer used
    /// </summary>
    [Test]
    public void TestRedeem() {
      Pool<TestClass> pool = new Pool<TestClass>();
      pool.Redeem(new TestClass());
    }

    /// <summary>
    ///   Tests whether the Recycle() method is called at the appropriate time
    /// </summary>
    [Test]
    public void TestRecycle() {
      Pool<TestClass> pool = new Pool<TestClass>();
      TestClass x = new TestClass();

      Assert.IsFalse(x.Recycled);
      pool.Redeem(x);
      Assert.IsTrue(x.Recycled);
    }

    /// <summary>Verifies that the pool's Capacity is applied correctly</summary>
    [Test]
    public void TestPoolSize() {
      Pool<TestClass> pool = new Pool<TestClass>(123);
      Assert.AreEqual(123, pool.Capacity);
      pool.Capacity = 321;
      Assert.AreEqual(321, pool.Capacity);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
