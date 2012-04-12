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

using Microsoft.Xna.Framework.Input;

using NUnit.Framework;

using Nuclex.Input;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Unit Test for the pressable control</summary>
  [TestFixture]
  internal class PressableControlTest {

    #region class TestPressableControl

    /// <summary>
    ///   Test implementation of a pressable control used in the unit tests
    /// </summary>
    private class TestPressableControl : PressableControl {

      /// <summary>Called when the control is pressed</summary>
      protected override void OnPressed() {
        ++this.PressCount;

        base.OnPressed();
      }

      /// <summary>Number of times the button was pressed</summary>
      public int PressCount;

    }

    #endregion // class TestPressableControl

    /// <summary>Verifies whether the control can be pressed using the mouse</summary>
    [Test]
    public void TestPressByMouse() {
      TestPressableControl pressable = new TestPressableControl();
      pressable.Bounds = new UniRectangle(10, 10, 100, 100);

      // Move the mouse over the control and do a left-click
      pressable.ProcessMouseMove(0, 0, 50, 50);
      pressable.ProcessMousePress(MouseButtons.Left);
      pressable.ProcessMouseRelease(MouseButtons.Left);

      Assert.AreEqual(1, pressable.PressCount);
    }

    /// <summary>
    ///   Tests whether a press can be aborted at the last second by moving
    ///   the mouse away from the control before the mouse button is released
    /// </summary>
    [Test]
    public void TestLastSecondAbortByMouse() {
      TestPressableControl pressable = new TestPressableControl();
      pressable.Bounds = new UniRectangle(10, 10, 100, 100);

      // Move the mouse over the control and do a left-click
      pressable.ProcessMouseMove(0, 0, 50, 50);
      pressable.ProcessMousePress(MouseButtons.Left);
      pressable.ProcessMouseMove(0, 0, 5, 5); // outside of the button
      pressable.ProcessMouseRelease(MouseButtons.Left);

      Assert.AreEqual(0, pressable.PressCount);
    }

    /// <summary>Ensures that a disabled control cannot be pressed</summary>
    [Test]
    public void TestPressDisabledControl() {
      TestPressableControl pressable = new TestPressableControl();
      pressable.Enabled = false;
      pressable.Bounds = new UniRectangle(10, 10, 100, 100);

      // Move the mouse over the control and do a left-click
      pressable.ProcessMouseMove(0, 0, 50, 50);
      pressable.ProcessMousePress(MouseButtons.Left);
      pressable.ProcessMouseRelease(MouseButtons.Left);

      Assert.AreEqual(0, pressable.PressCount);
    }

    /// <summary>
    ///   Verifies that the pressable control can be pressed using the space bar
    ///   if it is in focus
    /// </summary>
    [Test]
    public void TestPressWithSpaceBar() {
      TestPressableControl pressable = new TestPressableControl();
      pressable.Enabled = false;
      
      Screen screen = new Screen();
      screen.Desktop.Children.Add(pressable);
      screen.FocusedControl = pressable;
      
      // Press the space bar on the control
      pressable.ProcessKeyPress(Keys.Space, false);
      pressable.ProcessKeyRelease(Keys.Space);

      Assert.AreEqual(1, pressable.PressCount);
    }
    
    /// <summary>
    ///   Verifies that pressed keys are replayed to a control if those keys have been
    ///   ignore before but now account to the control because it has been activated.
    /// </summary>
    [Test]
    public void TestIgnoredKeys() {
      TestPressableControl pressable = new TestPressableControl();
      pressable.Enabled = false;

      Screen screen = new Screen();
      screen.Desktop.Children.Add(pressable);
      screen.FocusedControl = pressable;

      // Press the space bar on the control after pressing 'G'
      screen.InjectKeyPress(Keys.G);
      screen.InjectKeyPress(Keys.G);
      screen.InjectKeyPress(Keys.Space);
      screen.InjectKeyRelease(Keys.G);
      screen.InjectKeyRelease(Keys.Space);

      Assert.IsFalse(pressable.Depressed);
    }
    
    /// <summary>
    ///   Verifies that sending an invalid command to a control causes an exception
    /// </summary>
    [Test]
    public void TestThrowOnInvalidCommand() {
      TestPressableControl pressable = new TestPressableControl();
      Assert.Throws<ArgumentException>(
        delegate() { pressable.ProcessCommand((Command)(-1)); }
      );
    }

    /// <summary>
    ///   Verifies that the pressable control can be operated by the game pad
    /// </summary>
    [Test]
    public void TestShortcutByGamepad() {
      TestPressableControl pressable = new TestPressableControl();
      pressable.ShortcutButton = Buttons.A;

      pressable.ProcessButtonPress(Buttons.B);
      pressable.ProcessButtonRelease(Buttons.B);

      Assert.AreEqual(0, pressable.PressCount);

      pressable.ProcessButtonPress(Buttons.A);
      pressable.ProcessButtonRelease(Buttons.A);
      
      Assert.AreEqual(1, pressable.PressCount);
    }

    /// <summary>
    ///   Tests whether the pressable control can be operated by the keyboard
    /// </summary>
    /// <param name="button">Shortcut button that will be assigned to the control</param>
    /// <param name="key">Equivalent key on the keyboard for the shortcut button</param>
    [
      Test,
      TestCase(Buttons.A, Keys.A),
      TestCase(Buttons.B, Keys.B),
      TestCase(Buttons.Back, Keys.Back),
      TestCase(Buttons.LeftShoulder, Keys.L),
      TestCase(Buttons.LeftStick, Keys.LeftControl),
      TestCase(Buttons.RightShoulder, Keys.R),
      TestCase(Buttons.RightStick, Keys.RightControl),
      TestCase(Buttons.Start, Keys.Enter),
      TestCase(Buttons.X, Keys.X),
      TestCase(Buttons.Y, Keys.Y)
    ]
    public void TestShortcutByKeyboard(Buttons button, Keys key) {
      TestPressableControl pressable = new TestPressableControl();
      pressable.ShortcutButton = button;

      Assert.IsFalse(pressable.ProcessKeyPress(Keys.D, false));

      Assert.AreEqual(0, pressable.PressCount);

      Assert.IsTrue(pressable.ProcessKeyPress(key, false));
      pressable.ProcessKeyRelease(key);

      Assert.AreEqual(1, pressable.PressCount);
    }

    /// <summary>
    ///   Verifies that the control ignores an invalid shortcut button
    /// </summary>
    [Test]
    public void TestIgnoreInvalidShortcut() {
      TestPressableControl pressable = new TestPressableControl();
      pressable.ShortcutButton = (Buttons)(-1);

      Assert.IsFalse(pressable.ProcessKeyPress(Keys.D, false));
    }

    /// <summary>
    ///   Verifies that a shortcut button can be assigned to the control
    /// </summary>
    [Test]
    public void TestShortcutAssignment() {
      TestPressableControl pressable = new TestPressableControl();

      Assert.IsFalse(pressable.ShortcutButton.HasValue);

      pressable.ShortcutButton = Buttons.A;

      Assert.IsTrue(pressable.ShortcutButton.HasValue);
      Assert.AreEqual(Buttons.A, pressable.ShortcutButton.Value);
    }

  }

} // namespace Nuclex.UserInterface.Controls

#endif // UNITTEST
