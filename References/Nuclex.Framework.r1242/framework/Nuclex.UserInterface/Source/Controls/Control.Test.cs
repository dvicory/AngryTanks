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

using Microsoft.Xna.Framework.Input;

using NUnit.Framework;

using Nuclex.Input;
using Nuclex.Support;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Unit Test for the control class</summary>
  [TestFixture]
  internal class ControlTest {

    #region class MouseOverTestControl

    /// <summary>Control used for testing the mouse over notifications</summary>
    private class MouseOverTestControl : Control {

      /// <summary>
      ///   Called when the mouse has entered the control and is now hovering over it
      /// </summary>
      protected override void OnMouseEntered() {
        this.MouseOver = true;
        base.OnMouseEntered(); // not needed, for test coverage ;-)
      }

      /// <summary>
      ///   Called when the mouse has left the control and is no longer hovering over it
      /// </summary>
      protected override void OnMouseLeft() {
        this.MouseOver = false;
        base.OnMouseLeft(); // not needed, for test coverage ;-)
      }

      /// <summary>Whether the mouse is currently hovering over the control</summary>
      public bool MouseOver;

    }

    #endregion // class MouseOverTestControl

    #region class MouseWheelTestControl

    /// <summary>Control used for testing the mouse wheel notification</summary>
    private class MouseWheelTestControl : Control {

      /// <summary>Called when the mouse wheel has been rotated</summary>
      /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
      protected override void OnMouseWheel(float ticks) {
        this.Ticks += ticks;
        base.OnMouseWheel(ticks); // not needed; only for test coverage ;-)
      }

      /// <summary>Number of ticks the mouse wheel has been moved</summary>
      public float Ticks;

    }

    #endregion // class MouseWheelTestControl

    #region class KeyboardTestControl

    /// <summary>Control used for testing the keyboard notification</summary>
    private class KeyboardTestControl : Control {

      /// <summary>Initializes a new keyboard test control</summary>
      /// <param name="responsible">
      ///   Whether the control will claim responsibility for input routed to it
      /// </param>
      public KeyboardTestControl(bool responsible) {
        this.responsible = responsible;
      }

      /// <summary>Called when a key on the keyboard has been pressed down</summary>
      /// <param name="keyCode">Code of the key that was pressed</param>
      /// <returns>
      ///   True if the key press was handled by the control, otherwise false.
      /// </returns>
      protected override bool OnKeyPressed(Keys keyCode) {
        ++this.HeldKeyCount;
        base.OnKeyPressed(keyCode); // not needed; only for test coverage ;-)
        return this.responsible;
      }

      /// <summary>Called when a key on the keyboard has been released again</summary>
      /// <param name="keyCode">Code of the key that was released</param>
      protected override void OnKeyReleased(Keys keyCode) {
        --this.HeldKeyCount;
        base.OnKeyReleased(keyCode); // not needed; only for test coverage ;-)
      }

      /// <summary>Number of keys being held down</summary>
      public int HeldKeyCount;
      /// <summary>
      ///   Whether the control claims responsibility for input routed to it
      /// </summary>
      private bool responsible;

    }

    #endregion // class KeyboardTestControl

    #region class GamePadTestControl

    /// <summary>Control used for testing the keyboard notification</summary>
    private class GamePadTestControl : Control {

      /// <summary>Initializes a new keyboard test control</summary>
      /// <param name="responsible">
      ///   Whether the control will claim responsibility for input routed to it
      /// </param>
      public GamePadTestControl(bool responsible) {
        this.responsible = responsible;
      }

      /// <summary>Called when a button on the gamepad has been pressed</summary>
      /// <param name="button">Button that has been pressed</param>
      /// <returns>
      ///   True if the button press was handled by the control, otherwise false.
      /// </returns>
      protected override bool OnButtonPressed(Buttons button) {
        ++this.HeldButtonCount;
        base.OnButtonPressed(button);
        return this.responsible;
      }

      /// <summary>Called when a button on the gamepad has been released</summary>
      /// <param name="button">Button that has been released</param>
      protected override void OnButtonReleased(Buttons button) {
        --this.HeldButtonCount;
        base.OnButtonReleased(button);
      }

      /// <summary>Number of keys being held down</summary>
      public int HeldButtonCount;
      /// <summary>
      ///   Whether the control claims responsibility for input routed to it
      /// </summary>
      private bool responsible;

    }

    #endregion // class GamePadTestControl

    /// <summary>
    ///   Tests whether the control detects an id collision with one of its siblings
    ///   when the id is assigned to an already used string.
    /// </summary>
    [Test]
    public void TestNameCollisionOnIdAssignment() {
      Control parent = new Control();
      Control child1 = new Control();
      Control child2 = new Control();

      parent.Children.Add(child1);
      parent.Children.Add(child2);

      child1.Name = "DuplicateName";
      Assert.Throws<DuplicateNameException>(
        delegate() { child2.Name = "DuplicateName"; }
      );
    }

    /// <summary>
    ///   Tests whether the control detects an id collision with one of its siblings
    ///   when a sibling is added that has the same name.
    /// </summary>
    [Test]
    public void TestNameCollisionOnInsertion() {
      Control parent = new Control();
      Control child1 = new Control();
      Control child2 = new Control();

      child1.Name = "DuplicateName";
      child2.Name = "DuplicateName";

      parent.Children.Add(child1);
      Assert.Throws<DuplicateNameException>(
        delegate() { parent.Children.Add(child2); }
      );
    }

    /// <summary>Verifies that the control rejects unsupported commands</summary>
    [Test]
    public void TestUnsupportedCommands() {
      Control control = new Control();
      
      // These will never be acknowledged
      Assert.IsFalse(control.ProcessCommand(Command.SelectPrevious));
      Assert.IsFalse(control.ProcessCommand(Command.SelectNext));
    }

    /// <summary>Verifies that the control accepts supported commands</summary>
    [Test]
    public void TestSupportedCommands() {
      Control control = new Control();

      // False because the control doesn't acknowledge them
      Assert.IsFalse(control.ProcessCommand(Command.Up));
      Assert.IsFalse(control.ProcessCommand(Command.Down));
      Assert.IsFalse(control.ProcessCommand(Command.Left));
      Assert.IsFalse(control.ProcessCommand(Command.Right));
      Assert.IsFalse(control.ProcessCommand(Command.Accept));
      Assert.IsFalse(control.ProcessCommand(Command.Cancel));
    }

    /// <summary>
    ///   Validates that the control can correctly resolve its unified coordinates
    ///   into absolute screen coordinates if parented to a screen.
    /// </summary>
    [Test]
    public void TestAbsoluteCoordinateTransformation() {

      // Create a test control that occupies 80 percent of the space in its
      // parent using unified coordinates
      Control myControl = new Control();
      myControl.Bounds.Left = new UniScalar(0.1f, 0);
      myControl.Bounds.Top = new UniScalar(0.1f, 0);
      myControl.Bounds.Right = new UniScalar(0.9f, 0);
      myControl.Bounds.Bottom = new UniScalar(0.9f, 0);

      // Place the test control on a screen sized 1000 x 1000 pixels
      Screen myScreen = new Screen(1000.0f, 1000.0f);
      myScreen.Desktop.Children.Add(myControl);

      // Verify that the test control's absolute coordinates reflect its size
      // given in unified coordinates
      RectangleF absoluteBoundaries = myControl.GetAbsoluteBounds();
      assertAlmostEqual(100.0f, absoluteBoundaries.Left);
      assertAlmostEqual(100.0f, absoluteBoundaries.Top);
      assertAlmostEqual(900.0f, absoluteBoundaries.Right);
      assertAlmostEqual(900.0f, absoluteBoundaries.Bottom);

      // Now change the size of the desktop to only one fourth of the screen
      myScreen.Desktop.Bounds.Location.X.Fraction = 0.5f;
      myScreen.Desktop.Bounds.Location.Y.Fraction = 0.5f;
      myScreen.Desktop.Bounds.Size.X.Fraction = 0.5f;
      myScreen.Desktop.Bounds.Size.Y.Fraction = 0.5f;

      // Verify that the desktop size change is reflected in the absolute
      // coordinates that control's GetAbsoluteBounds() method hands out
      absoluteBoundaries = myControl.GetAbsoluteBounds();
      assertAlmostEqual(550.0f, absoluteBoundaries.Left);
      assertAlmostEqual(550.0f, absoluteBoundaries.Top);
      assertAlmostEqual(950.0f, absoluteBoundaries.Right);
      assertAlmostEqual(950.0f, absoluteBoundaries.Bottom);

    }

    /// <summary>
    ///   Verifies that the control throws an exception if it is asked to provide its
    ///   absolute position within being connected to a screen.
    /// </summary>
    [Test]
    public void TestThrowsOnGetAbsolutePositionWithoutScreen() {
      Control control = new Control();
      Assert.Throws<InvalidOperationException>(
        delegate() { control.GetAbsoluteBounds(); }
      );
    }

    /// <summary>
    ///   Tests whether a control can be brought to the front of the drawing hierarchy
    /// </summary>
    [Test]
    public void TestBringToFront() {
      // Create this:
      //
      //   Root
      //    |- Child 1
      //    |- Child 2
      //         |- Child 2 Child 1
      //         |- Child 2 Child 2
      Control root = new Control();
      Control child1 = new Control();
      Control child2 = new Control();
      Control child2Child1 = new Control();
      Control child2Child2 = new Control();
      root.Children.Add(child1);
      root.Children.Add(child2);
      child2.Children.Add(child2Child1);
      child2.Children.Add(child2Child2);

      // The second child of each control should not be on top
      Assert.AreNotSame(child2, root.Children[0]);
      Assert.AreNotSame(child2Child2, child2.Children[0]);

      // Bring the control to the top. This should recursively move its parents
      // to the top of the siblings so the control ends up in the foreground.
      child2Child2.BringToFront();

      // Make sure the control and its parent are the first one in each list
      Assert.AreSame(child2, root.Children[0]);
      Assert.AreSame(child2Child2, child2.Children[0]);
    }

    /// <summary>
    ///   Verifies that mouse presses outside of the control's area can be handled
    /// </summary>
    [Test]
    public void TestInitialMousePressOutsideOfControl() {
      Control control = new Control();
      control.ProcessMousePress(MouseButtons.Left);
      control.ProcessMouseRelease(MouseButtons.Left);
    }


    /// <summary>
    ///   Ensures that the control can handle mouse over notifications if its notification
    ///   is not overridden and it has no children
    /// </summary>
    [Test]
    public void TestMouseOver() {
      MouseOverTestControl control = new MouseOverTestControl();
      control.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);

      Assert.IsFalse(control.MouseOver);      
      control.ProcessMouseMove(100.0f, 100.0f, 20.0f, 20.0f);
      Assert.IsTrue(control.MouseOver);
      control.ProcessMouseMove(100.0f, 100.0f, -1.0f, -1.0f);
      Assert.IsFalse(control.MouseOver);
    }

    /// <summary>
    ///   Ensures that the control passes on mouse over notifications to its children
    /// </summary>
    [Test]
    public void TestMouseOverChildren() {
      Control parent = new Control();
      parent.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      MouseOverTestControl child1 = new MouseOverTestControl();
      child1.Bounds = new UniRectangle(10.0f, 10.0f, 25.0f, 60.0f);
      MouseOverTestControl child2 = new MouseOverTestControl();
      child2.Bounds = new UniRectangle(45.0f, 10.0f, 25.0f, 60.0f);
      
      parent.Children.Add(child1);
      parent.Children.Add(child2);

      Assert.IsFalse(child1.MouseOver);
      Assert.IsFalse(child2.MouseOver);
      parent.ProcessMouseMove(100.0f, 100.0f, 20.0f, 30.0f);
      Assert.IsTrue(child1.MouseOver);
      Assert.IsFalse(child2.MouseOver);
      parent.ProcessMouseMove(100.0f, 100.0f, 60.0f, 30.0f);
      Assert.IsFalse(child1.MouseOver);
      Assert.IsTrue(child2.MouseOver);
    }

    /// <summary>
    ///   Ensures that the control passes mouse movement notifications to the activated
    ///   control first.
    /// </summary>
    [Test]
    public void TestMouseOverWithActivatedControl() {
      Control parent = new Control();
      parent.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      MouseOverTestControl child = new MouseOverTestControl();
      child.Bounds = new UniRectangle(10.0f, 10.0f, 25.0f, 60.0f);
      parent.Children.Add(child);

      parent.ProcessMouseMove(100.0f, 100.0f, 20.0f, 30.0f);
      Assert.IsTrue(child.MouseOver);

      parent.ProcessMousePress(MouseButtons.Left);
      parent.ProcessMouseMove(100.0f, 100.0f, -1.0f, -1.0f);
      
      Assert.IsFalse(child.MouseOver);
    }

    /// <summary>
    ///   Ensures that a control with no overridden methods mouse presses are ignored
    /// </summary>
    [Test]
    public void TestIgnoreMousePress() {
      Control control = new Control();
      control.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);

      control.ProcessMouseMove(100.0f, 100.0f, 20.0f, 20.0f);
      control.ProcessMousePress(MouseButtons.Left);
      control.ProcessMouseRelease(MouseButtons.Left);
    }

    /// <summary>
    ///   Verifies that order-affecting controls are reordered when the user clicks
    ///   on a control
    /// </summary>
    [Test]
    public void TestReorderControlsOnMousePress() {
      Control parent = new Control();
      parent.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      Controls.Desktop.WindowControl child1 = new Controls.Desktop.WindowControl();
      child1.Bounds = new UniRectangle(10.0f, 10.0f, 25.0f, 60.0f);
      Controls.Desktop.WindowControl child2 = new Controls.Desktop.WindowControl();
      child2.Bounds = new UniRectangle(45.0f, 10.0f, 25.0f, 60.0f);

      parent.Children.Add(child1);
      parent.Children.Add(child2);

      Assert.AreSame(child1, parent.Children[0]);
      Assert.AreSame(child2, parent.Children[1]);
      
      parent.ProcessMouseMove(100.0f, 100.0f, 60.0f, 30.0f);
      parent.ProcessMousePress(MouseButtons.Left);

      Assert.AreSame(child2, parent.Children[0]);
      Assert.AreSame(child1, parent.Children[1]);
    }

    /// <summary>
    ///   Verifies that mouse wheel turns are delivered to the activated control
    /// </summary>
    [Test]
    public void TestMouseWheelWithActivatedControl() {
      Control parent = new Control();
      MouseWheelTestControl child = new MouseWheelTestControl();
      child.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      parent.Children.Add(child);

      parent.ProcessMouseMove(100.0f, 100.0f, 50.0f, 50.0f);
      parent.ProcessMousePress(MouseButtons.Left);
      parent.ProcessMouseMove(100.0f, 100.0f, -1.0f, -1.0f);
      
      Assert.AreEqual(0.0f, child.Ticks);
      parent.ProcessMouseWheel(12.34f);
      Assert.AreEqual(12.34f, child.Ticks);
    }

    /// <summary>
    ///   Verifies that mouse wheel turns are delivered to the control the mouse
    ///   is over when no control is activated
    /// </summary>
    [Test]
    public void TestMouseWheelWithMouseOverControl() {
      Control parent = new Control();
      MouseWheelTestControl child = new MouseWheelTestControl();
      child.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      parent.Children.Add(child);

      parent.ProcessMouseMove(100.0f, 100.0f, 50.0f, 50.0f);

      Assert.AreEqual(0.0f, child.Ticks);
      parent.ProcessMouseWheel(12.34f);
      Assert.AreEqual(12.34f, child.Ticks);
    }

    /// <summary>
    ///   Verifies that keyboard messages are routed to the activated control
    /// </summary>
    [Test]
    public void TestKeyPressWithActivatedControl() {
      Control parent = new Control();
      parent.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      KeyboardTestControl child1 = new KeyboardTestControl(true);
      KeyboardTestControl child2 = new KeyboardTestControl(true);
      child2.Bounds = new UniRectangle(10.0f, 10.0f, 25.0f, 60.0f);

      parent.Children.Add(child1);
      parent.Children.Add(child2);

      parent.ProcessMouseMove(100.0f, 100.0f, 20.0f, 30.0f);
      parent.ProcessMousePress(MouseButtons.Left);
      
      Assert.AreEqual(0, child1.HeldKeyCount);
      Assert.AreEqual(0, child2.HeldKeyCount);
      parent.ProcessKeyPress(Keys.A, false);
      Assert.AreEqual(0, child1.HeldKeyCount);
      Assert.AreEqual(1, child2.HeldKeyCount); // Because child 1 was activated
      parent.ProcessKeyRelease(Keys.A);
      Assert.AreEqual(0, child1.HeldKeyCount);
      Assert.AreEqual(0, child2.HeldKeyCount);
    }

    /// <summary>
    ///   Verifies that keyboard messages are only sent to the foreground window 
    /// </summary>
    [Test]
    public void TestKeyPressOnFocusAffectingControl() {
      Control parent = new Control();
      Controls.Desktop.WindowControl child1 = new Controls.Desktop.WindowControl();
      Controls.Desktop.WindowControl child2 = new Controls.Desktop.WindowControl();
      KeyboardTestControl child3 = new KeyboardTestControl(true);
      parent.Children.Add(child1);
      parent.Children.Add(child2);
      parent.Children.Add(child3);
      KeyboardTestControl child1child1 = new KeyboardTestControl(false);
      KeyboardTestControl child1child2 = new KeyboardTestControl(false);
      KeyboardTestControl child2child1 = new KeyboardTestControl(false);
      child1.Children.Add(child1child1);
      child1.Children.Add(child1child2);
      child2.Children.Add(child2child1);

      parent.ProcessKeyPress(Keys.A, false);
      Assert.AreEqual(1, child1child1.HeldKeyCount);
      Assert.AreEqual(1, child1child2.HeldKeyCount); // because child 1 returned false
      Assert.AreEqual(0, child2child1.HeldKeyCount); // because its parent affects focus
      Assert.AreEqual(1, child3.HeldKeyCount); // because it doesn't affect focus
    }

    /// <summary>
    ///   Verifies that game pad messages are routed to the activated control
    /// </summary>
    [Test]
    public void TestButtonPressWithActivatedControl() {
      Control parent = new Control();
      parent.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      GamePadTestControl child1 = new GamePadTestControl(true);
      GamePadTestControl child2 = new GamePadTestControl(true);
      child2.Bounds = new UniRectangle(10.0f, 10.0f, 25.0f, 60.0f);

      parent.Children.Add(child1);
      parent.Children.Add(child2);

      parent.ProcessMouseMove(100.0f, 100.0f, 20.0f, 30.0f);
      parent.ProcessMousePress(MouseButtons.Left);

      Assert.AreEqual(0, child1.HeldButtonCount);
      Assert.AreEqual(0, child2.HeldButtonCount);
      parent.ProcessButtonPress(Buttons.A);
      Assert.AreEqual(0, child1.HeldButtonCount);
      Assert.AreEqual(1, child2.HeldButtonCount); // Because child 1 was activated
      parent.ProcessButtonRelease(Buttons.A);
      Assert.AreEqual(0, child1.HeldButtonCount);
      Assert.AreEqual(0, child2.HeldButtonCount);
    }

    /// <summary>
    ///   Verifies that game pad messages are only sent to the foreground window 
    /// </summary>
    [Test]
    public void TestButtonPressOnFocusAffectingControl() {
      Control parent = new Control();
      Controls.Desktop.WindowControl child1 = new Controls.Desktop.WindowControl();
      Controls.Desktop.WindowControl child2 = new Controls.Desktop.WindowControl();
      GamePadTestControl child3 = new GamePadTestControl(true);
      parent.Children.Add(child1);
      parent.Children.Add(child2);
      parent.Children.Add(child3);
      GamePadTestControl child1child1 = new GamePadTestControl(false);
      GamePadTestControl child1child2 = new GamePadTestControl(false);
      GamePadTestControl child2child1 = new GamePadTestControl(false);
      child1.Children.Add(child1child1);
      child1.Children.Add(child1child2);
      child2.Children.Add(child2child1);

      parent.ProcessButtonPress(Buttons.A);
      Assert.AreEqual(1, child1child1.HeldButtonCount);
      Assert.AreEqual(1, child1child2.HeldButtonCount); // because child 1 returned false
      Assert.AreEqual(0, child2child1.HeldButtonCount); // because its parent affects focus
      Assert.AreEqual(1, child3.HeldButtonCount); // because it doesn't affect focus
    }

    /// <summary>Asserts that two floating point values are almost equal</summary>
    /// <param name="expected">Expected value</param>
    /// <param name="actual">Actual value</param>
    private void assertAlmostEqual(float expected, float actual) {
      if(!FloatHelper.AreAlmostEqual(expected, actual, 1)) {
        Assert.AreEqual(expected, actual);
      }
    }

  }

} // namespace Nuclex.UserInterface.Controls

#endif // UNITTEST
