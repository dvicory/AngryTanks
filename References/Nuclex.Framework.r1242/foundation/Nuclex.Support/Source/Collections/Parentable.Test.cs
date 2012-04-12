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

using System;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the Parentable class</summary>
  [TestFixture]
  public class ParentableTest {

    #region class TestParentable

    /// <summary>Parentable object that can be the child of an int</summary>
    private class TestParentable : Parentable<int> {

      /// <summary>Initializes a new instance of the parentable test class</summary>
      public TestParentable() { }

      /// <summary>The parent object that owns this instance</summary>
      public int GetParent() {
        return base.Parent;
      }

      /// <summary>Invoked whenever the instance's owner changes</summary>
      /// <remarks>
      ///   When items are parented for the first time, the oldParent argument will
      ///   be null. Also, if the element is removed from the collection, the
      ///   current parent will be null.
      /// </remarks>
      /// <param name="oldParent">Previous owner of the instance</param>
      protected override void OnParentChanged(int oldParent) {
        this.parentChangedCalled = true;

        base.OnParentChanged(oldParent); // to satisfy NCover :-/
      }

      /// <summary>Whether the OnParentChanged method has been called</summary>
      public bool ParentChangedCalled {
        get { return this.parentChangedCalled; }
      }

      /// <summary>Whether the OnParentChanged method has been called</summary>
      private bool parentChangedCalled;

    }

    #endregion // class TestParentable

    /// <summary>
    ///   Tests whether a parent can be assigned and then retrieved from
    ///   the parentable object
    /// </summary>
    [Test]
    public void TestParentAssignment() {
      TestParentable testParentable = new TestParentable();

      testParentable.SetParent(12345);
      Assert.AreEqual(12345, testParentable.GetParent());
    }

    /// <summary>
    ///   Tests whether a parent can be assigned and then retrieved from
    ///   the parentable object
    /// </summary>
    [Test]
    public void TestParentChangedNotification() {
      TestParentable testParentable = new TestParentable();

      testParentable.SetParent(12345);

      Assert.IsTrue(testParentable.ParentChangedCalled);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
