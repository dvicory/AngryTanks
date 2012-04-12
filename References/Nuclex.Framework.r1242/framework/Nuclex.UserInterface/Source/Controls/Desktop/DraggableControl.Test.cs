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

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.Support;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the draggable control base class</summary>
  [TestFixture]
  internal class DraggableControlTest {

    #region class DummyDraggableControl

    /// <summary>Dummy implementation of a draggable control</summary>
    private class DummyDraggableControl : DraggableControl {

      /// <summary>Initializes a new draggable control</summary>
      public DummyDraggableControl() : base() { }

      /// <summary>Initializes a new draggable control</summary>
      /// <param name="canGetFocus">Whether the control can obtain the input focus</param>
      public DummyDraggableControl(bool canGetFocus) : base(canGetFocus) { }

      /// <summary>Whether the control can be dragged by the mouse</summary>
      public new bool EnableDragging {
        get { return base.EnableDragging; }
        set { base.EnableDragging = value; }
      }

    }
    
    #endregion // class DummyDraggableControl

    /// <summary>
    ///   Tests whether the default constructor of the draggable control is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      DummyDraggableControl dummy = new DummyDraggableControl();
      Assert.IsFalse(dummy.AffectsOrdering);
    }

    /// <summary>
    ///   Tests whether the full constructor of the draggable control is working
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      DummyDraggableControl dummy = new DummyDraggableControl(true);
      Assert.IsTrue(dummy.AffectsOrdering);
    }

    /// <summary>Tests whether the control can be dragged using the mouse</summary>
    [Test]
    public void TestDragging() {
      DummyDraggableControl dummy = new DummyDraggableControl();
      dummy.Bounds = new UniRectangle(10, 10, 100, 100);

      dummy.ProcessMouseMove(100, 100, 50, 50);
      dummy.ProcessMousePress(MouseButtons.Left);
      dummy.ProcessMouseMove(100, 100, 60, 50);

      Assert.AreEqual(20, dummy.Bounds.Location.X.Offset);

      dummy.ProcessMouseMove(100, 100, 40, 40);

      Assert.AreEqual(0, dummy.Bounds.Location.X.Offset);
      Assert.AreEqual(0, dummy.Bounds.Location.Y.Offset);

      dummy.ProcessMouseRelease(MouseButtons.Left);
      dummy.ProcessMouseMove(100, 100, 70, 70);

      Assert.AreEqual(0, dummy.Bounds.Location.X.Offset);
      Assert.AreEqual(0, dummy.Bounds.Location.Y.Offset);
    }

    /// <summary>
    ///   Tests whether the dragging capability can be disabled for a window
    /// </summary>
    [Test]
    public void DisableDragging() {
      DummyDraggableControl dummy = new DummyDraggableControl();
      dummy.Bounds = new UniRectangle(10, 10, 100, 100);

      // By default, dragging should be enabled
      Assert.IsTrue(dummy.EnableDragging);

      // Turn it off
      dummy.EnableDragging = false;

      // Now it should be off ;-)
      Assert.IsFalse(dummy.EnableDragging);

      // Try to drag the window
      dummy.ProcessMouseMove(100, 100, 50, 50);
      dummy.ProcessMousePress(MouseButtons.Left);
      dummy.ProcessMouseMove(100, 100, 60, 50);

      // Make sure the window has not moved
      Assert.AreEqual(10, dummy.Bounds.Location.X.Offset);
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
