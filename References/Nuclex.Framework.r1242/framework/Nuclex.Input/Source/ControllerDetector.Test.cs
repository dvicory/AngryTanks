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
using Microsoft.Xna.Framework;

namespace Nuclex.Input {

  /// <summary>Unit tests for the controller detector</summary>
  [TestFixture]
  public class ControllerDetectorTest {

    #region interface IControllerDetectorSubscriber

    /// <summary>Subscriber for the events of the controller detector</summary>
    public interface IControllerDetectorSubscriber {

      /// <summary>
      ///   Called when the controller detector has discovered a controller on
      ///   which a button has been pressed
      /// </summary>
      /// <param name="sender">Detector sending the event</param>
      /// <param name="arguments">
      ///   Contains the index of the player whose controller changed its state
      /// </param>
      void ControllerDetected(object sender, ControllerEventArgs arguments);

    }

    #endregion // interface IControllerDetectorSubscriber

    #region class ControllerEventArgsMatcher

    /// <summary>Matches controller event args to expected values</summary>
    private class ControllerEventArgsMatcher : Matcher {

      /// <summary>
      ///   Initializes a new matcher for controller event arguments for the default
      ///   player on a PC system
      /// </summary>
      public ControllerEventArgsMatcher() {
        this.playerIndex = null;
      }

      /// <summary>Initializes a new matcher for controller event arguments</summary>
      /// <param name="playerIndex">
      ///   Index of the player to expect in the event arguments
      /// </param>
      public ControllerEventArgsMatcher(ExtendedPlayerIndex playerIndex) {
        this.playerIndex = playerIndex;
      }

      /// <summary>Initializes a new matcher for controller event arguments</summary>
      /// <param name="playerIndex">
      ///   Index of the player to expect in the event arguments
      /// </param>
      public ControllerEventArgsMatcher(PlayerIndex playerIndex) {
        this.playerIndex = (ExtendedPlayerIndex)playerIndex;
      }

      /// <summary>Generates a human-readable description of the matcher</summary>
      /// <param name="writer">Writer the description is written into</param>
      public override void DescribeTo(System.IO.TextWriter writer) {
        if (this.playerIndex.HasValue) {
          writer.Write(this.playerIndex.Value);
        } else {
          writer.Write("null");
        }
      }

      /// <summary>Tests whether the provided object matches the expected state</summary>
      /// <param name="otherObject">Other object that will be checked</param>
      /// <returns>True if the other object matches the expected state</returns>
      public override bool Matches(object otherObject) {
        var other = otherObject as ControllerEventArgs;
        if (other == null) {
          return false;
        } else {
          return (other.PlayerIndex == this.playerIndex);
        }
      }

      /// <summary>Player index the matcher is expecting</summary>
      private ExtendedPlayerIndex? playerIndex;

    }

    #endregion // class ControllerEventArgsMatcher

    /// <summary>Called once before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
      this.inputManager = new MockInputManager();
      this.detector = new ControllerDetector(this.inputManager);
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if (this.detector != null) {
        this.detector.Dispose();
      }
      if (this.inputManager != null) {
        this.inputManager.Dispose();
        this.inputManager = null;
      }
      if (this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>Verifies that the constructor of the class is working</summary>
    [Test]
    public void TestConstructor() {
      Assert.IsNotNull(this.detector);
    }

    /// <summary>
    ///   Verifies that the controller detector detects a mouse button press
    /// </summary>
    [Test]
    public void TestMouseDetection() {
      IControllerDetectorSubscriber subscriber = mockSubscriber();

      Expect.Once.On(subscriber).Method("ControllerDetected").With(
        NMock2.Is.Anything, new ControllerEventArgsMatcher()
      );

      // Begin monitoring input devices
      this.detector.Start();

      // Press the left mouse button and read the new mouse state
      this.inputManager.GetMouse().Press(MouseButtons.Left);
      this.inputManager.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Verifies that the controller detector detects a keyboard key press
    /// </summary>
    [Test]
    public void TestKeyboard() {
      IControllerDetectorSubscriber subscriber = mockSubscriber();

      Expect.Once.On(subscriber).Method("ControllerDetected").With(
        NMock2.Is.Anything, new ControllerEventArgsMatcher()
      );

      // Begin monitoring input devices
      this.detector.Start();

      // Press the left mouse button and read the new mouse state
      this.inputManager.GetKeyboard().Press(Keys.A);
      this.inputManager.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Verifies that the controller detector detects a chat pad key press
    /// </summary>
    [
      Test,
      TestCase(PlayerIndex.One),
      TestCase(PlayerIndex.Two),
      TestCase(PlayerIndex.Three),
      TestCase(PlayerIndex.Four)
    ]
    public void TestChatPad(PlayerIndex playerIndex) {
      IControllerDetectorSubscriber subscriber = mockSubscriber();

      Expect.Once.On(subscriber).Method("ControllerDetected").With(
        NMock2.Is.Anything, new ControllerEventArgsMatcher(playerIndex)
      );

      // Begin monitoring input devices
      this.detector.Start();

      // Press the left mouse button and read the new mouse state
      this.inputManager.GetKeyboard(playerIndex).Press(Keys.A);
      this.inputManager.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Verifies that the controller detector detects a game pad button press
    /// </summary>
    [
      Test,
      TestCase(ExtendedPlayerIndex.One),
      TestCase(ExtendedPlayerIndex.Two),
      TestCase(ExtendedPlayerIndex.Three),
      TestCase(ExtendedPlayerIndex.Four),
      TestCase(ExtendedPlayerIndex.Five),
      TestCase(ExtendedPlayerIndex.Six),
      TestCase(ExtendedPlayerIndex.Seven),
      TestCase(ExtendedPlayerIndex.Eight)
    ]
    public void TestGamePad(PlayerIndex playerIndex) {
      IControllerDetectorSubscriber subscriber = mockSubscriber();

      Expect.Once.On(subscriber).Method("ControllerDetected").With(
        NMock2.Is.Anything, new ControllerEventArgsMatcher(playerIndex)
      );

      // Begin monitoring input devices
      this.detector.Start();

      // Press the left mouse button and read the new mouse state
      this.inputManager.GetGamePad(playerIndex).Press(Buttons.Y);
      this.inputManager.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Creates a mocked subscriber for the controller detector</summary>
    /// <returns>The mocked subscriber</returns>
    private IControllerDetectorSubscriber mockSubscriber() {
      IControllerDetectorSubscriber subscriber = this.mockery.NewMock<
        IControllerDetectorSubscriber
      >();

      this.detector.ControllerDetected += subscriber.ControllerDetected;

      return subscriber;
    }

    /// <summary>Controller detector being tested</summary>
    private ControllerDetector detector;
    /// <summary>Mocked input manager used for unit testing</summary>
    private MockInputManager inputManager;
    /// <summary>Creates dynamic mock objects for interfaces</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Input

#endif // UNITTEST
