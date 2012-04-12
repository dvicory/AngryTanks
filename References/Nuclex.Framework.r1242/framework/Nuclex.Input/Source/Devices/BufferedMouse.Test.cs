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

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the buffered mouse class</summary>
  [TestFixture]
  internal class BufferedMouseTest {

    #region class TestBufferedMouse

    /// <summary>Test implementation of a buffered mouse</summary>
    private class TestBufferedMouse : BufferedMouse {

      /// <summary>Moves the mouse cursor to the specified location</summary>
      /// <param name="x">New X coordinate of the mouse cursor</param>
      /// <param name="y">New Y coordinate of the mouse cursor</param>
      public override void MoveTo(float x, float y) {
        base.BufferCursorMovement(x, y);
      }

      /// <summary>Whether the input device is connected to the system</summary>
      public override bool IsAttached {
        get { return true; }
      }

      /// <summary>Human-readable name of the input device</summary>
      public override string Name {
        get { return "Test mouse"; }
      }

      /// <summary>Records a mouse button press in the event queue</summary>
      /// <param name="buttons">Buttons that have been pressed</param>
      public new void BufferButtonPress(MouseButtons buttons) {
        base.BufferButtonPress(buttons);
      }

      /// <summary>Records a mouse button release in the event queue</summary>
      /// <param name="buttons">Buttons that have been released</param>
      public new void BufferButtonRelease(MouseButtons buttons) {
        base.BufferButtonRelease(buttons);
      }

      /// <summary>Records a mouse wheel rotation in the event queue</summary>
      /// <param name="ticks">Ticks the mouse wheel has been rotated</param>
      public new void BufferWheelRotation(float ticks) {
        base.BufferWheelRotation(ticks);
      }

      /// <summary>Records a mouse cursor movement in the event queue</summary>
      /// <param name="x">X coordinate the mouse cursor has been moved to</param>
      /// <param name="y">Y coordinate the mouse cursor has been moved to</param>
      public new void BufferCursorMovement(float x, float y) {
        base.BufferCursorMovement(x, y);
      }

    }

    #endregion // TestBufferedMouse

    #region interface IMouseSubscriber

    /// <summary>Subscriber to the events of a mouse</summary>
    public interface IMouseSubscriber {

      /// <summary>Called when a mouse button has been pressed</summary>
      /// <param name="buttons">Button which has been pressed</param>
      void ButtonPressed(MouseButtons buttons);

      /// <summary>Called when a mouse button has been released</summary>
      /// <param name="buttons">Button which has been released</param>
      void ButtonReleased(MouseButtons buttons);

      /// <summary>Called when the mouse cursor has been moved</summary>
      /// <param name="x">X coordinate of the mouse cursor</param>
      /// <param name="y">Y coordinate of the mouse cursor</param>
      void Moved(float x, float y);

      /// <summary>Called when the mouse wheel has been rotated</summary>
      /// <param name="ticks">Number of ticks the mouse wheel was rotated</param>
      void WheelRotated(float ticks);

    }

    #endregion // interface IMouseSubscriber

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
      this.mouse = new TestBufferedMouse();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if (this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>Verifies that button presses can be buffered</summary>
    [Test]
    public void TestBufferButtonPress() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.BufferButtonPress(MouseButtons.Middle);

      Expect.Once.On(subscriber).Method("ButtonPressed").With(
        NMock2.Is.EqualTo(MouseButtons.Middle)
      );

      this.mouse.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that button releases can be buffered</summary>
    [Test]
    public void TestBufferButtonRelease() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.BufferButtonRelease(MouseButtons.X1);

      Expect.Once.On(subscriber).Method("ButtonReleased").With(
        NMock2.Is.EqualTo(MouseButtons.X1)
      );

      this.mouse.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that mouse movements can be buffered</summary>
    [Test]
    public void TestBufferMouseMovement() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.BufferCursorMovement(12.34f, 56.78f);

      Expect.Once.On(subscriber).Method("Moved").With(
        NMock2.Is.EqualTo(12.34f), NMock2.Is.EqualTo(56.78f)
      );

      this.mouse.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that mouse wheel rotations can be buffered</summary>
    [Test]
    public void TestBufferWheelRotation() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.BufferWheelRotation(19.28f);

      Expect.Once.On(subscriber).Method("WheelRotated").With(
        NMock2.Is.EqualTo(19.28f)
      );

      this.mouse.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the buffered keyboard</summary>
    /// <returns>A subscriber registered to the events of the keyboard</returns>
    private IMouseSubscriber mockSubscriber() {
      IMouseSubscriber subscriber = this.mockery.NewMock<IMouseSubscriber>();

      this.mouse.MouseButtonPressed += subscriber.ButtonPressed;
      this.mouse.MouseButtonReleased += subscriber.ButtonReleased;
      this.mouse.MouseMoved += subscriber.Moved;
      this.mouse.MouseWheelRotated += subscriber.WheelRotated;

      return subscriber;
    }

    /// <summary>Creates dynamic mock objects for interfaces</summary>
    private Mockery mockery;
    /// <summary>Buffered mouse being tested</summary>
    private TestBufferedMouse mouse;

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
