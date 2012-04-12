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

using Nuclex.Support;

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the window control class</summary>
  [TestFixture]
  internal class WindowControlTest {

    /// <summary>Tests whether the window can be dragged using the mouse</summary>
    [Test]
    public void TestWindowDragging() {
      WindowControl window = new WindowControl();
      window.Bounds = new UniRectangle(10, 10, 100, 100);

      window.ProcessMouseMove(100, 100, 50, 50);
      window.ProcessMousePress(MouseButtons.Left);
      window.ProcessMouseMove(100, 100, 60, 50);

      Assert.AreEqual(20, window.Bounds.Location.X.Offset);

      window.ProcessMouseMove(100, 100, 40, 40);

      Assert.AreEqual(0, window.Bounds.Location.X.Offset);
      Assert.AreEqual(0, window.Bounds.Location.Y.Offset);

      window.ProcessMouseRelease(MouseButtons.Left);
      window.ProcessMouseMove(100, 100, 70, 70);

      Assert.AreEqual(0, window.Bounds.Location.X.Offset);
      Assert.AreEqual(0, window.Bounds.Location.Y.Offset);
    }

    /// <summary>
    ///   Tests whether the Close() method can be called when the window isn't open
    /// </summary>
    [Test]
    public void TestCloseUnopenedWindow() {
      WindowControl window = new WindowControl();
      window.Close();
      // No exception means success
    }

    /// <summary>
    ///   Verifies that a window can be opened and closed again
    /// </summary>
    [Test]
    public void TestOpenCloseWindow() {
      Screen screen = new Screen(100, 100);
      WindowControl window = new WindowControl();

      // A window not in a screen's control hierarchy is considered closed
      Assert.IsFalse(window.IsOpen);

      // Once the window is added to a screen, meaning it will be drawn and
      // can be interacted with by the user, it is considered open
      screen.Desktop.Children.Add(window);
      Assert.IsTrue(window.IsOpen);

      // Close the window. Essentially just syntactic sugar for removing
      // the window from the control hierarchy
      window.Close();
      Assert.IsFalse(window.IsOpen);
    }

    /// <summary>
    ///   Tests whether the dragging capability can be disabled for a window
    /// </summary>
    [Test]
    public void DisableDragging() {
      WindowControl window = new WindowControl();
      window.Bounds = new UniRectangle(10, 10, 100, 100);

      // By default, dragging should be enabled
      Assert.IsTrue(window.EnableDragging);

      // Turn it off
      window.EnableDragging = false;

      // Now it should be off ;-)
      Assert.IsFalse(window.EnableDragging);

      // Try to drag the window
      window.ProcessMouseMove(100, 100, 50, 50);
      window.ProcessMousePress(MouseButtons.Left);
      window.ProcessMouseMove(100, 100, 60, 50);

      // Make sure the window has not moved
      Assert.AreEqual(10, window.Bounds.Location.X.Offset);
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
