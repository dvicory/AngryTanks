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
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.Support;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the text input control</summary>
  [TestFixture]
  internal class InputControlTest {

    #region class DummyLocator

    /// <summary>Dummy implementation of an opening locator</summary>
    private class DummyLocator : IOpeningLocator {

      /// <summary>Initializes a new dummy text opening locator</summary>
      /// <param name="index">Index of the opening the dummy locator will report</param>
      public DummyLocator(int index) {
        this.index = index;
      }

      /// <summary>
      ///   Calculates which opening between two letters is closest to a position
      /// </summary>
      /// <param name="bounds">
      ///   Boundaries of the control, should be in absolute coordinates
      /// </param>
      /// <param name="text">Text in which the nearest opening will be located</param>
      /// <param name="position">
      ///   Position to which the closest opening will be found,
      ///   should be in absolute coordinates
      /// </param>
      /// <returns>The index of the opening closest to the provided position</returns>
      public int GetClosestOpening(RectangleF bounds, string text, Vector2 position) {
        return this.index;
      }

      /// <summary>Index the dummy locator will report</summary>
      private int index;

    }

    #endregion // class DummyLocator

    /// <summary>Verifies that simple text input is possible</summary>
    [Test]
    public void TestSimpleInput() {
      InputControl input = new InputControl();
      
      IWritable writable = input as IWritable;
      Assert.IsNotNull(writable);

      writable.OnCharacterEntered('H');
      writable.OnCharacterEntered('e');
      writable.OnCharacterEntered('l');
      writable.OnCharacterEntered('l');
      writable.OnCharacterEntered('o');

      Assert.AreEqual("Hello", input.Text);
    }
    
    /// <summary>
    ///   Verifies that the title and description strings for the Guide can be assigned
    ///   to the input control and queried via the IWritable interface
    /// </summary>
    [Test]
    public void TestGuideTitleAndDescription() {
      InputControl input = new InputControl();
      input.GuideTitle = "Test123";
      input.GuideDescription = "123Test";

      IWritable writable = input as IWritable;
      Assert.IsNotNull(writable);
      
      Assert.AreEqual("Test123", writable.GuideTitle);
      Assert.AreEqual("123Test", writable.GuideDescription);
    }

    /// <summary>
    ///   Tests whether the caret can be moved by the home and end keys
    /// </summary>
    [Test]
    public void TestHomeAndEnd() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);
      screen.FocusedControl = input;

      input.ProcessCharacter('W');
      input.ProcessCharacter('o');
      input.ProcessCharacter('r');
      input.ProcessCharacter('l');
      input.ProcessCharacter('d');

      input.ProcessKeyPress(Keys.Home, false);

      input.ProcessCharacter('H');
      input.ProcessCharacter('e');
      input.ProcessCharacter('l');
      input.ProcessCharacter('l');
      input.ProcessCharacter('o');
      input.ProcessCharacter(' ');

      input.ProcessKeyPress(Keys.End, false);

      input.ProcessCharacter('!');

      Assert.AreEqual("Hello World!", input.Text);
    }

    /// <summary>
    ///   Tests whether the caret can be moved using the cursor keys
    /// </summary>
    [Test]
    public void TestCaretMovementByCursor() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);
      screen.FocusedControl = input;

      input.ProcessCharacter('H');
      input.ProcessCharacter('e');
      input.ProcessCharacter('l');
      input.ProcessCharacter('o');

      input.ProcessKeyPress(Keys.Left, false);

      input.ProcessCharacter('l');

      input.ProcessKeyPress(Keys.Right, false);

      input.ProcessCharacter('!');

      Assert.AreEqual("Hello!", input.Text);
    }

    /// <summary>
    ///   Tests whether the input control rejects presses of keys it doesn't handle
    /// </summary>
    [Test]
    public void TestUnhandledKeyPress() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);
      screen.FocusedControl = input;

      Assert.IsTrue(input.ProcessKeyPress(Keys.Left, false));
      input.ProcessKeyRelease(Keys.Left);
      Assert.IsTrue(input.ProcessKeyPress(Keys.Right, false));
      input.ProcessKeyRelease(Keys.Right);
      
      Assert.IsFalse(input.ProcessKeyPress(Keys.Up, false));
      Assert.IsFalse(input.ProcessKeyPress(Keys.Down, false));
    }

    /// <summary>
    ///   Tests whether the input control rejects presses of normally handled keys
    ///   when it isn't focused
    /// </summary>
    [Test]
    public void TestUnfocusedKeyPress() {
      InputControl input = new InputControl();

      Assert.IsFalse(input.ProcessKeyPress(Keys.Left, false));
      Assert.IsFalse(input.ProcessKeyPress(Keys.Right, false));
    }

    /// <summary>
    ///   Verifies that the backspace key can be used to delete a character
    /// </summary>
    [Test]
    public void TestBackspace() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);
      screen.FocusedControl = input;

      input.ProcessCharacter('H');
      input.ProcessCharacter('e');
      input.ProcessCharacter('y');
      input.ProcessCharacter('o');

      input.ProcessKeyPress(Keys.Left, false);
      input.ProcessKeyPress(Keys.Back, false);

      input.ProcessCharacter('l');
      input.ProcessCharacter('l');

      Assert.AreEqual("Hello", input.Text);
    }

    /// <summary>
    ///   Tests whether the delete key deletes the character right of the caret
    /// </summary>
    [Test]
    public void TestDelete() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);
      screen.FocusedControl = input;

      input.ProcessCharacter('T');
      input.ProcessCharacter('e');
      input.ProcessCharacter('l');
      input.ProcessCharacter('l');
      input.ProcessCharacter('o');

      input.ProcessKeyPress(Keys.Home, false);
      input.ProcessKeyPress(Keys.Delete, false);

      input.ProcessCharacter('H');

      Assert.AreEqual("Hello", input.Text);
    }

    /// <summary>
    ///   Tests whether assigning a text that is too short for the current caret
    ///   position will adjust the caret's position
    /// </summary>
    [Test]
    public void TestTextShorteningAdjustsCaret() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);
      screen.FocusedControl = input;

      input.Text = "Hello World";
      input.CaretPosition = 11;

      Assert.AreEqual(11, input.CaretPosition);

      input.Text = "Hello";

      Assert.AreEqual(5, input.CaretPosition);
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an invalid caret position is assigned
    /// </summary>
    [Test]
    public void TestThrowOnInvalidCaretPosition() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);
      screen.FocusedControl = input;

      Assert.Throws<ArgumentException>(
        delegate() {
          input.CaretPosition = -1;
        }
      );
      Assert.Throws<ArgumentException>(
        delegate() {
          input.CaretPosition = 1;
        }
      );
    }

    /// <summary>
    ///   Tests whether the control can tell whether it has the input focus
    /// </summary>
    [Test]
    public void TestInputFocus() {
      Screen screen = new Screen();
      InputControl input = new InputControl();
      screen.Desktop.Children.Add(input);

      Assert.IsFalse(input.HasFocus);

      screen.FocusedControl = input;

      Assert.IsTrue(input.HasFocus);
    }

    /// <summary>
    ///   Verifies that the caret can be moved by clicking the mouse and that
    ///   the input box performs its default action with no opening locator assigned.
    /// </summary>
    [Test]
    public void TestMoveCaretByMouseWithoutLocator() {
      Screen screen = new Screen(100, 100);
      InputControl input = new InputControl();
      input.Bounds = new UniRectangle(10, 10, 100, 100);
      screen.Desktop.Children.Add(input);

      input.Text = "Hello World";
      input.CaretPosition = 0;

      // This should move the caret. Because the control has no locator and it
      // doesn't know which font is being used (controls have no connection to
      // their rendering code), it can only move the caret to the end.      
      input.ProcessMouseMove(100, 100, 50, 50);
      input.ProcessMousePress(MouseButtons.Left);

      Assert.AreEqual(input.Text.Length, input.CaretPosition);
    }

    /// <summary>
    ///   Verifies that the caret can be moved by clicking the mouse and that
    ///   the input box uses its opening locator if it is assigned.
    /// </summary>
    [Test]
    public void TestMoveCaretByMouseWitLocator() {
      Screen screen = new Screen(100, 100);
      InputControl input = new InputControl();
      input.Bounds = new UniRectangle(10, 10, 100, 100);
      screen.Desktop.Children.Add(input);

      input.Text = "Hello World";
      input.CaretPosition = 0;

      // Assign a dummy locator for text openings which will always report that
      // the mouse is between the 4th and 5th letters.
      input.OpeningLocator = new DummyLocator(4);

      // This should move the caret. Because the dummy locator is assigned, it
      // should be asked for the position the user has clicked on.
      input.ProcessMouseMove(100, 100, 50, 50);
      input.ProcessMousePress(MouseButtons.Left);

      Assert.AreEqual(4, input.CaretPosition);
    }

    /// <summary>
    ///   Verifies that the input box tracks the milliseconds that have passed
    ///   since the caret was last moved.
    /// </summary>
    [Test]
    public void TestMillisecondsSinceLastCaretMovement() {
      InputControl input = new InputControl();
      input.ProcessCharacter('H');
      
      int start = Environment.TickCount;
      int end;
      do {
        Thread.Sleep(1);
        end = Environment.TickCount;
      } while(start == end);

      int elapsedMilliseconds = end - start;
      
      Assert.GreaterOrEqual(
        input.MillisecondsSinceLastCaretMovement, elapsedMilliseconds
      );
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
