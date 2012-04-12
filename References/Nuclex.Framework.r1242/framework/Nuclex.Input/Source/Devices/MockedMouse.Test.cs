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
using System.Windows.Forms;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the mocked mouse</summary>
  [TestFixture]
  internal class MockedMouseTest {

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
      this.mouse = new MockedMouse();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if (this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>Verifies that the GetState() method is working</summary>
    [Test]
    public void TestGetState() {
      this.mouse.GetState();
      // No exception means success
    }

    /// <summary>Verifies that the mouse can be attached and detached</summary>
    [Test]
    public void TestAttachAndDetach() {
      Assert.IsFalse(this.mouse.IsAttached);
      this.mouse.Attach();
      Assert.IsTrue(this.mouse.IsAttached);
      this.mouse.Detach();
      Assert.IsFalse(this.mouse.IsAttached);
    }

    /// <summary>Verifies that the mocked mouse's name can be retrieved</summary>
    [Test]
    public void TestName() {
      StringAssert.Contains("mock", this.mouse.Name.ToLower());
    }

    /// <summary>Verifies that button presses can be simulated</summary>
    [Test]
    public void TestPressButton() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.Press(MouseButtons.Right);

      Expect.Once.On(subscriber).Method("ButtonPressed").With(
        NMock2.Is.EqualTo(MouseButtons.Right)
      );

      this.mouse.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that button releases can be simulated</summary>
    [Test]
    public void TestReleaseButton() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.Release(MouseButtons.X2);

      Expect.Once.On(subscriber).Method("ButtonReleased").With(
        NMock2.Is.EqualTo(MouseButtons.X2)
      );

      this.mouse.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that mouse movement can be simulated</summary>
    [Test]
    public void TestMoveTo() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.MoveTo(43.21f, 87.65f);

      Expect.Once.On(subscriber).Method("Moved").With(
        NMock2.Is.EqualTo(43.21f), NMock2.Is.EqualTo(87.65f)
      );

      this.mouse.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that mouse wheel rotations can be simulated</summary>
    [Test]
    public void TestRotateWheel() {
      IMouseSubscriber subscriber = mockSubscriber();

      this.mouse.RotateWheel(1.2f);

      Expect.Once.On(subscriber).Method("WheelRotated").With(
        NMock2.Is.EqualTo(1.2f)
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
    private MockedMouse mouse;

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
