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

using NUnit.Framework;
using NMock2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the mocked game pad</summary>
  [TestFixture]
  internal class MockedGamePadTest {
  
    /// <summary>Verifies that the constructor is working</summary>
    [Test]
    public void TestConstructor() {
      var gamePad = new MockedGamePad();
      Assert.IsNotNull(gamePad);
    }

    /// <summary>Verifies that the GetState() method is working</summary>
    [Test]
    public void TestGetState() {
      var gamePad = new MockedGamePad();
      gamePad.GetState();
      // No exception means success
    }
    
    /// <summary>Verifies that the game pad can be attached and detached</summary>
    [Test]
    public void TestAttachAndDetach() {
      var gamePad = new MockedGamePad();
      Assert.IsFalse(gamePad.IsAttached);
      gamePad.Attach();
      Assert.IsTrue(gamePad.IsAttached);
      gamePad.Detach();
      Assert.IsFalse(gamePad.IsAttached);
    }

    /// <summary>Verifies that the mocked game pad's name can be retrieved</summary>
    [Test]
    public void TestName() {
      var gamePad = new MockedGamePad();
      StringAssert.Contains("mock", gamePad.Name.ToLower());
    }

    /// <summary>Tests whether the left thumb stick can be simulated</summary>
    [Test]
    public void TestMoveLeftThumbStick() {
      var gamePad = new MockedGamePad();
      gamePad.MoveLeftThumbStick(0.1234f, 0.5678f);
      gamePad.Update();
      
      GamePadState state = gamePad.GetState();
      Assert.AreEqual(new Vector2(0.1234f, 0.5678f), state.ThumbSticks.Left);
    }

    /// <summary>Tests whether the right thumb stick can be simulated</summary>
    [Test]
    public void TestMoveRightThumbStick() {
      var gamePad = new MockedGamePad();
      gamePad.MoveRightThumbStick(0.8765f, 0.4321f);
      gamePad.Update();

      GamePadState state = gamePad.GetState();
      Assert.AreEqual(new Vector2(0.8765f, 0.4321f), state.ThumbSticks.Right);
    }
    
    /// <summary>Verifies that the left trigger can be moved</summary>
    [Test]
    public void TestLeftTrigger() {
      var gamePad = new MockedGamePad();
      gamePad.PushLeftTrigger(0.1357f);
      gamePad.Update();

      GamePadState state = gamePad.GetState();
      Assert.AreEqual(0.1357f, state.Triggers.Left);
    }

    /// <summary>Verifies that the right trigger can be moved</summary>
    [Test]
    public void TestRightTrigger() {
      var gamePad = new MockedGamePad();
      gamePad.PushRightTrigger(0.2468f);
      gamePad.Update();

      GamePadState state = gamePad.GetState();
      Assert.AreEqual(0.2468f, state.Triggers.Right);
    }

    /// <summary>Verifies that the buttons on the game pad can be pressed</summary>
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
    public void TestButtons(Buttons button) {
      var gamePad = new MockedGamePad();

      GamePadState state = gamePad.GetState();
      Assert.IsFalse(state.IsButtonDown(button));

      gamePad.Press(button);
      gamePad.Update();

      state = gamePad.GetState();
      Assert.IsTrue(state.IsButtonDown(button));

      gamePad.Release(button);
      gamePad.Update();

      state = gamePad.GetState();
      Assert.IsFalse(state.IsButtonDown(button));
    }

    /// <summary>Verifies that the directional pad can be simulated</summary>
    [
      Test,
      TestCase(Buttons.DPadUp),
      TestCase(Buttons.DPadDown),
      TestCase(Buttons.DPadLeft),
      TestCase(Buttons.DPadRight)
    ]
    public void TestDirectionalPad(Buttons button) {
      var gamePad = new MockedGamePad();
      gamePad.Press(button);
      gamePad.Update();

      GamePadState state = gamePad.GetState();
      switch(button) {
        case Buttons.DPadUp: {
          Assert.AreEqual(ButtonState.Pressed, state.DPad.Up);
          Assert.AreEqual(ButtonState.Released, state.DPad.Down);
          break;
        }
        case Buttons.DPadDown: {
          Assert.AreEqual(ButtonState.Pressed, state.DPad.Down);
          Assert.AreEqual(ButtonState.Released, state.DPad.Up);
          break;
        }
        case Buttons.DPadLeft: {
          Assert.AreEqual(ButtonState.Pressed, state.DPad.Left);
          Assert.AreEqual(ButtonState.Released, state.DPad.Right);
          break;
        }
        case Buttons.DPadRight: {
          Assert.AreEqual(ButtonState.Pressed, state.DPad.Right);
          Assert.AreEqual(ButtonState.Released, state.DPad.Left);
          break;
        }
      }
    }

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
