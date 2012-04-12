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

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Unit Test for the parenting control collection class</summary>
  [TestFixture]
  internal class ParentingControlCollectionTest {

    /// <summary>
    ///   Tests whether the attempt to assign a node as its own parent is detected
    ///   and causes an exception to be thrown.
    /// </summary>
    [Test]
    public void TestThrowOnSelfParentage() {
      Control testControl = new Control();
      Assert.Throws<InvalidOperationException>(
        delegate() { testControl.Children.Add(testControl); }
      );
    }

    /// <summary>
    ///   Tests whether the attempt to assign a node's parent as its child
    ///   causes an exception to be thrown.
    /// </summary>
    [Test]
    public void TestThrowOnRingParentage() {
      Control parent = new Control();
      Control child = new Control();

      parent.Children.Add(child);
      Assert.Throws<InvalidOperationException>(
        delegate() { child.Children.Add(parent); }
      );
    }

    /// <summary>
    ///   Tests whether the attempt to assign a node as child to more multiple
    ///   parents causes an exception to be thrown.
    /// </summary>
    [Test]
    public void TestThrowOnMultipleParents() {
      Control parent1 = new Control();
      Control parent2 = new Control();
      Control child = new Control();

      parent1.Children.Add(child);
      Assert.Throws<InvalidOperationException>(
        delegate() { parent2.Children.Add(child); }
      );
    }

    /// <summary>
    ///   Tests whether a control is properly instated as the parent to
    ///   any controls added to its children list.
    /// </summary>
    [Test]
    public void TestParentAssignment() {
      Control parent = new Control();
      Control child = new Control();

      parent.Children.Add(child);

      Assert.AreSame(parent, child.Parent);
    }

    /// <summary>
    ///   Tests whether items are unparented when the collection is cleared
    /// </summary>
    [Test]
    public void TestClearing() {
      Control parent = new Control();
      Control child = new Control();

      parent.Children.Add(child);
      Assert.AreSame(parent, child.Parent);

      parent.Children.Clear();
      Assert.IsNull(child.Parent);
    }

    /// <summary>
    ///   Tests whether the parent references are updated correctly if a control
    ///   is replaced in the parent's children collection
    /// </summary>
    [Test]
    public void TestItemReplacement() {
      Control parent = new Control();
      Control child1 = new Control();
      Control child2 = new Control();

      parent.Children.Add(child1);

      Assert.AreSame(parent, child1.Parent);
      Assert.IsNull(child2.Parent);

      parent.Children[0] = child2;

      Assert.IsNull(child1.Parent);
      Assert.AreSame(parent, child2.Parent);
    }

    /// <summary>
    ///   Verifies that the parenting control collection can move a child control to
    ///   the end of the collection
    /// </summary>
    [Test]
    public void TestMoveToEnd() {
      Control parent = new Control();
      Control child1 = new Control();
      Control child2 = new Control();
      Control child3 = new Control();

      parent.Children.Add(child1);
      parent.Children.Add(child2);
      parent.Children.Add(child3);

      Assert.AreSame(child1, parent.Children[0]);
      Assert.AreSame(child2, parent.Children[1]);
      Assert.AreSame(child3, parent.Children[2]);

      child3.BringToFront();

      Assert.AreSame(child3, parent.Children[0]);
      Assert.AreSame(child1, parent.Children[1]);
      Assert.AreSame(child2, parent.Children[2]);
    }

  }

} // namespace Nuclex.UserInterface.Controls

#endif // UNITTEST
