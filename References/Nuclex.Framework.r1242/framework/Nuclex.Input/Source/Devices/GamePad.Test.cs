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

  /// <summary>Unit tests for the game pad base class</summary>
  [TestFixture]
  internal class GamePadTest {

    #region interface IGamePadSubscriber

    /// <summary>Subscriber for the game pad's events</summary>
    public interface IGamePadSubscriber {

      /// <summary>Called when a button on the game pad is pressed</summary>
      /// <param name="buttons">Button that has been pressed</param>
      void ButtonPressed(Buttons buttons);

      /// <summary>Called when a button on the game pad has been released</summary>
      /// <param name="buttons">Button that has been released</param>
      void ButtonReleased(Buttons buttons);

    }

    #endregion interface IGamePadSubscriber

    #region class TestGamePad

    /// <summary>Implementation of a game pad for unit testing</summary>
    private class TestGamePad : GamePad {

      /// <summary>Retrieves the current state of the game pad</summary>
      /// <returns>The current state of the game pad</returns>
      public override GamePadState GetState() {
        return new GamePadState();
      }

      /// <summary>Whether the input device is connected to the system</summary>
      public override bool IsAttached {
        get { return true; }
      }

      /// <summary>Human-readable name of the input device</summary>
      public override string Name {
        get { return "Test dummy"; }
      }

      /// <summary>Update the state of all input devices</summary>
      public override void Update() { }

      /// <summary>Takes a snapshot of the current state of the input device</summary>
      public override void TakeSnapshot() { }

      /// <summary>Triggers the ButtonPressed event</summary>
      /// <param name="buttons">Buttons that will be reported</param>
      public void FireButtonPressed(Buttons buttons) {
        OnButtonPressed(buttons);
      }

      /// <summary>Triggers the ButtonReleased event</summary>
      /// <param name="buttons">Buttons that will be reported</param>
      public void FireButtonReleased(Buttons buttons) {
        OnButtonReleased(buttons);
      }

      /// <summary>Generates events for the difference between the two states</summary>
      /// <param name="previous">Previous state of the game pad</param>
      /// <param name="current">Current state of the game pad</param>
      public new void GenerateEvents(ref GamePadState previous, ref GamePadState current) {
        base.GenerateEvents(ref previous, ref current);
      }

    }

    #endregion // class TestGamePad

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
      this.testGamePad = new TestGamePad();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if (this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>Verifies that the ButtonPressed event is working</summary>
    [Test]
    public void TestButtonPressedEvent() {
      IGamePadSubscriber subscriber = mockSubscriber();

      Expect.Once.On(subscriber).Method("ButtonPressed").With(
        NMock2.Is.EqualTo(Buttons.LeftShoulder)
      );
      this.testGamePad.FireButtonPressed(Buttons.LeftShoulder);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that the ButtonReleased event is working</summary>
    [Test]
    public void TestButtonReleasedEvent() {
      IGamePadSubscriber subscriber = mockSubscriber();

      Expect.Once.On(subscriber).Method("ButtonReleased").With(
        NMock2.Is.EqualTo(Buttons.RightStick)
      );
      this.testGamePad.FireButtonReleased(Buttons.RightStick);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether state changes are detected by the game pad class</summary>
    /// <param name="button">Button that the detection will be tested with</param>
    [
      Test,
      TestCase(Buttons.Start),
      TestCase(Buttons.Back),
      TestCase(Buttons.LeftStick),
      TestCase(Buttons.RightStick),
      TestCase(Buttons.LeftShoulder),
      TestCase(Buttons.RightShoulder),
      TestCase(Buttons.A),
      TestCase(Buttons.B),
      TestCase(Buttons.X),
      TestCase(Buttons.Y)
    ]
    public void TestStateChangeDetection(Buttons button) {
      IGamePadSubscriber subscriber = mockSubscriber();

      GamePadState pressedState = makeGamePadState(button);
      GamePadState releasedState = new GamePadState();

      Expect.Once.On(subscriber).Method("ButtonPressed").With(
        NMock2.Is.EqualTo(button)
      );
      this.testGamePad.GenerateEvents(ref releasedState, ref pressedState);

      Expect.Once.On(subscriber).Method("ButtonReleased").With(
        NMock2.Is.EqualTo(button)
      );
      this.testGamePad.GenerateEvents(ref pressedState, ref releasedState);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the game pad</summary>
    /// <returns>The mocked subscriber</returns>
    private IGamePadSubscriber mockSubscriber() {
      IGamePadSubscriber subscriber = this.mockery.NewMock<IGamePadSubscriber>();
      this.testGamePad.ButtonPressed += subscriber.ButtonPressed;
      this.testGamePad.ButtonReleased += subscriber.ButtonReleased;
      return subscriber;
    }

    /// <summary>Creates a game pad state with the specified button pressed</summary>
    /// <param name="pressedButton">Button that will be pressed down</param>
    /// <returns>The new game pad state</returns>
    private static GamePadState makeGamePadState(Buttons pressedButton) {
      return new GamePadState(
        new GamePadThumbSticks(),
        new GamePadTriggers(),
        new GamePadButtons(pressedButton),
        new GamePadDPad()
      );
    }

    /// <summary>Creates dynamic interface-based mock objects</summary>
    private Mockery mockery;
    /// <summary>Test implementation of a game pad</summary>
    private TestGamePad testGamePad;

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
