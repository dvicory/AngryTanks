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
using NMock2;

using WinForm = System.Windows.Forms.Form;

namespace Nuclex.Input {

  /// <summary>Unit tests for the window message interceptor</summary>
  [TestFixture]
  internal class WindowMessageInterceptorTest {

    #region interface IKeyboardMessageSubscriber

    /// <summary>Subscriber to keyboard messages arriving at the window</summary>
    public interface IKeyboardMessageSubscriber {

      /// <summary>Called when a key is pressed on the keyboard</summary>
      /// <param name="key">Key that has been pressed</param>
      void KeyPressed(Keys key);

      /// <summary>Called when a key on the keyboard has been released</summary>
      /// <param name="key">Key that has been released</param>
      void KeyReleased(Keys key);

      /// <summary>Called when the user has entered a character</summary>
      /// <param name="character">Character the user has entered</param>
      void CharacterEntered(char character);

    }

    #endregion // interface IKeyboardMessageSubscriber

    #region interface IMouseMessageSubscriber

    /// <summary>Subscriber to mouse messages arriving at the window</summary>
    public interface IMouseMessageSubscriber {

      /// <summary>Called when a mouse button has been pressed</summary>
      /// <param name="buttons">Buttons that have been pressed</param>
      void MouseButtonPressed(MouseButtons buttons);

      /// <summary>Called when a button on the mouse has been released</summary>
      /// <param name="buttons">Button that has been released</param>
      void MouseButtonReleased(MouseButtons buttons);

      /// <summary>Called when the mouse cursor has been moved</summary>
      /// <param name="x">New X coordinate of the mouse cursor</param>
      /// <param name="y">New Y coordinate of the mouse cursor</param>
      void MouseMoved(float x, float y);

      /// <summary>Called when the mouse wheel has been rotated</summary>
      /// <param name="ticks">Number of ticks the mouse wheel was rotated</param>
      void MouseWheelRotated(float ticks);

    }

    #endregion // interface IMouseMessageSubscriber

    /// <summary>Called once before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
      this.form = new WinForm();
      this.interceptor = new WindowMessageInterceptor(this.form.Handle);
    }

    /// <summary>Called once after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if (this.interceptor != null) {
        this.interceptor.Dispose();
        this.interceptor = null;
      }
      if (this.form != null) {
        this.form.Dispose();
        this.form = null;
      }
      if (this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>Verifies that the constructor of the interceptor is working</summary>
    [Test]
    public void TestConstructor() {
      Assert.IsNotNull(this.interceptor);
    }

    /// <summary>Checks whether the dialog code is adjusted by the interceptor</summary>
    [Test]
    public void TestGetDialogCode() {
      int dlgCode = sendMessage(UnsafeNativeMethods.WindowMessages.WM_GETDLGCODE, 0, 0);

      int expected = UnsafeNativeMethods.DLGC_WANTALLKEYS;
      Assert.AreEqual(expected, dlgCode & expected);

      expected = UnsafeNativeMethods.DLGC_WANTCHARS;
      Assert.AreEqual(expected, dlgCode & expected);
    }

    /// <summary>Verifies that the WM_KEYDOWN message is intercepted</summary>
    [Test]
    public void TestKeyPressMessage() {
      IKeyboardMessageSubscriber subscriber = mockKeyboardSubscriber();
      Expect.Once.On(subscriber).Method("KeyPressed").With(
        NMock2.Is.EqualTo(Keys.X)
      );
      sendMessage(UnsafeNativeMethods.WindowMessages.WM_KEYDOWN, (int)Keys.X, 0);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that the WM_KEYDOWN message is intercepted</summary>
    [Test]
    public void TestKeyReleaseMessage() {
      IKeyboardMessageSubscriber subscriber = mockKeyboardSubscriber();
      Expect.Once.On(subscriber).Method("KeyReleased").With(
        NMock2.Is.EqualTo(Keys.V)
      );
      sendMessage(UnsafeNativeMethods.WindowMessages.WM_KEYUP, (int)Keys.V, 0);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that the WM_CHAR message is intercepted</summary>
    [Test]
    public void TestCharacterEnteredMessage() {
      IKeyboardMessageSubscriber subscriber = mockKeyboardSubscriber();
      Expect.Once.On(subscriber).Method("CharacterEntered").With(
        NMock2.Is.EqualTo('C')
      );
      sendMessage(UnsafeNativeMethods.WindowMessages.WM_CHAR, (int)'C', 0);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that the WM_MOUSEMOVE message is intercepted</summary>
    [Test]
    public void TestMouseMoveMessage() {
      IMouseMessageSubscriber subscriber = mockMouseSubscriber();
      Expect.Once.On(subscriber).Method("MouseMoved").With(
        NMock2.Is.EqualTo(123.0f), NMock2.Is.EqualTo(456.0f)
      );
      sendMessage(UnsafeNativeMethods.WindowMessages.WM_MOUSEMOVE, 0, 123 | (456 << 16));

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that the WM_*BUTTONDOWN messages are intercepted</summary>
    [
      Test,
      TestCase(UnsafeNativeMethods.WindowMessages.WM_LBUTTONDOWN, 0, MouseButtons.Left),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_MBUTTONDOWN, 0, MouseButtons.Middle),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_RBUTTONDOWN, 0, MouseButtons.Right),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_XBUTTONDOWN, 1, MouseButtons.X1),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_XBUTTONDOWN, 2, MouseButtons.X2)
    ]
    public void TestMouseButtonPressedMessage(
      UnsafeNativeMethods.WindowMessages message, int wParam, MouseButtons button
    ) {
      IMouseMessageSubscriber subscriber = mockMouseSubscriber();

      Expect.Once.On(subscriber).Method("MouseButtonPressed").With(
        NMock2.Is.EqualTo(button)
      );
      sendMessage(message, wParam << 16, 0);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that the WM_*BUTTONUP messages are intercepted</summary>
    [
      Test,
      TestCase(UnsafeNativeMethods.WindowMessages.WM_LBUTTONUP, 0, MouseButtons.Left),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_MBUTTONUP, 0, MouseButtons.Middle),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_RBUTTONUP, 0, MouseButtons.Right),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_XBUTTONUP, 1, MouseButtons.X1),
      TestCase(UnsafeNativeMethods.WindowMessages.WM_XBUTTONUP, 2, MouseButtons.X2)
    ]
    public void TestMouseButtonReleasedMessage(
      UnsafeNativeMethods.WindowMessages message, int wParam, MouseButtons button
    ) {
      IMouseMessageSubscriber subscriber = mockMouseSubscriber();

      Expect.Once.On(subscriber).Method("MouseButtonReleased").With(
        NMock2.Is.EqualTo(button)
      );
      sendMessage(message, wParam << 16, 0);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that WM_MOUSEWHEEL messages are intercepted</summary>
    [Test]
    public void TestMouseWheelMessage() {
      IMouseMessageSubscriber subscriber = mockMouseSubscriber();

      Expect.Once.On(subscriber).Method("MouseWheelRotated").With(
        NMock2.Is.EqualTo(1.0f)
      );
      sendMessage(UnsafeNativeMethods.WindowMessages.WM_MOUSEHWHEEL, 120 << 16, 0);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that WM_MOUSELEAVE messages are intercepted</summary>
    [Test]
    public void TestMouseLeaveMessage() {
      IMouseMessageSubscriber subscriber = mockMouseSubscriber();

      Expect.Once.On(subscriber).Method("MouseMoved").With(
        NMock2.Is.EqualTo(-1.0f), NMock2.Is.EqualTo(-1.0f)
      );
      sendMessage(UnsafeNativeMethods.WindowMessages.WM_MOUSELEAVE, 0, 0);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Sends a message to the game's window</summary>
    /// <param name="message">Message that will be sent</param>
    /// <param name="wParam">Value for the message's wParam</param>
    /// <param name="lParam">Value for the message's lParam</param>
    /// <returns>The value returned by the window procedure for the message</returns>
    private int sendMessage(
      UnsafeNativeMethods.WindowMessages message, int wParam, int lParam
    ) {
      return UnsafeNativeMethods.SendMessage(
        this.form.Handle, (uint)message, new IntPtr(wParam), new IntPtr(lParam)
      ).ToInt32();
    }

    /// <summary>
    ///   Mocks a mouse message subscriber and registers it to the interceptor's events
    /// </summary>
    /// <returns>The new mouse message subscriber</returns>
    private IMouseMessageSubscriber mockMouseSubscriber() {
      var subscriber = this.mockery.NewMock<IMouseMessageSubscriber>();
      this.interceptor.MouseButtonPressed += subscriber.MouseButtonPressed;
      this.interceptor.MouseButtonReleased += subscriber.MouseButtonReleased;
      this.interceptor.MouseMoved += subscriber.MouseMoved;
      this.interceptor.MouseWheelRotated += subscriber.MouseWheelRotated;
      return subscriber;
    }

    /// <summary>
    ///   Mocks a keyboard message subscriber and registers it to the interceptor's events
    /// </summary>
    /// <returns>The new keyboard message subscriber</returns>
    private IKeyboardMessageSubscriber mockKeyboardSubscriber() {
      var subscriber = this.mockery.NewMock<IKeyboardMessageSubscriber>();
      this.interceptor.KeyPressed += subscriber.KeyPressed;
      this.interceptor.KeyReleased += subscriber.KeyReleased;
      this.interceptor.CharacterEntered += subscriber.CharacterEntered;
      return subscriber;
    }

    /// <summary>Called when a mouse button has been pressed</summary>
    private static readonly int[] MouseButtonDownMessage = new int[] {
      (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONDOWN,
      (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONDOWN,
      (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONDOWN,
      (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONDOWN,
      (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONDOWN
    };

    /// <summary>Called when a mouse button has been released</summary>
    private static readonly int[] MouseButtonUpMessage = new int[] {
      (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONUP,
      (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONUP,
      (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONUP,
      (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONUP,
      (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONUP
    };

    /// <summary>Form used to test interception of window messages</summary>
    private WinForm form;
    /// <summary>Message interceptor for the form</summary>
    private WindowMessageInterceptor interceptor;
    /// <summary>Creates dynamic mock objects based on interfaces</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Input

#endif // UNITTEST
