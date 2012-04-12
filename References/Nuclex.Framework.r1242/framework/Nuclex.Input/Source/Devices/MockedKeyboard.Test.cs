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
using Microsoft.Xna.Framework.Input;

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the mocked keyboard</summary>
  [TestFixture]
  public class MockedKeyboardTest {

    #region interface IKeyboardSubscriber

    /// <summary>Subscriber to the </summary>
    public interface IKeyboardSubscriber {

      /// <summary>Called when a key has been pressed</summary>
      /// <param name="key">Key that has been pressed</param>
      void KeyPressed(Keys key);

      /// <summary>Called when a key has been released</summary>
      /// <param name="key">Key that has been released</param>
      void KeyReleased(Keys key);

      /// <summary>Called when a character has been entered</summary>
      /// <param name="character">Character that has been entered</param>
      void CharacterEntered(char character);

    }

    #endregion // interface IKeyboardSubscriber

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
      this.keyboard = new MockedKeyboard();
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
      this.keyboard.GetState();
      // No exception means success
    }

    /// <summary>Verifies that the game pad can be attached and detached</summary>
    [Test]
    public void TestAttachAndDetach() {
      Assert.IsFalse(this.keyboard.IsAttached);
      this.keyboard.Attach();
      Assert.IsTrue(this.keyboard.IsAttached);
      this.keyboard.Detach();
      Assert.IsFalse(this.keyboard.IsAttached);
    }

    /// <summary>Verifies that the mocked keyboard's name can be retrieved</summary>
    [Test]
    public void TestName() {
      StringAssert.Contains("mock", this.keyboard.Name.ToLower());
    }

    /// <summary>Verifies that key presses can be simulation</summary>
    [Test]
    public void TestPressKey() {
      IKeyboardSubscriber subscriber = mockSubscriber();

      this.keyboard.Press(Keys.H);

      Expect.Once.On(subscriber).Method("KeyPressed").With(
        NMock2.Is.EqualTo(Keys.H)
      );

      this.keyboard.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that key releases can be simulated</summary>
    [Test]
    public void TestReleaseKey() {
      IKeyboardSubscriber subscriber = mockSubscriber();

      this.keyboard.Release(Keys.W);

      Expect.Once.On(subscriber).Method("KeyReleased").With(
        NMock2.Is.EqualTo(Keys.W)
      );

      this.keyboard.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that character entries can be simulated</summary>
    [Test]
    public void TestEnterCharacter() {
      IKeyboardSubscriber subscriber = mockSubscriber();

      this.keyboard.Enter('!');

      Expect.Once.On(subscriber).Method("CharacterEntered").With(
        NMock2.Is.EqualTo('!')
      );

      this.keyboard.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that text entries can be simulated</summary>
    [Test]
    public void TestTypeText() {
      IKeyboardSubscriber subscriber = mockSubscriber();

      this.keyboard.Type("Xyz™");

      Expect.Once.On(subscriber).Method("KeyPressed").With(
        NMock2.Is.EqualTo(Keys.LeftShift)
      );
      Expect.Once.On(subscriber).Method("KeyPressed").With(
        NMock2.Is.EqualTo(Keys.X)
      );
      Expect.Once.On(subscriber).Method("CharacterEntered").With(
        NMock2.Is.EqualTo('X')
      );
      Expect.Once.On(subscriber).Method("KeyReleased").With(
        NMock2.Is.EqualTo(Keys.X)
      );
      Expect.Once.On(subscriber).Method("KeyReleased").With(
        NMock2.Is.EqualTo(Keys.LeftShift)
      );

      Expect.Once.On(subscriber).Method("KeyPressed").With(
        NMock2.Is.EqualTo(Keys.Y)
      );
      Expect.Once.On(subscriber).Method("CharacterEntered").With(
        NMock2.Is.EqualTo('y')
      );
      Expect.Once.On(subscriber).Method("KeyReleased").With(
        NMock2.Is.EqualTo(Keys.Y)
      );

      Expect.Once.On(subscriber).Method("KeyPressed").With(
        NMock2.Is.EqualTo(Keys.Z)
      );
      Expect.Once.On(subscriber).Method("CharacterEntered").With(
        NMock2.Is.EqualTo('z')
      );
      Expect.Once.On(subscriber).Method("KeyReleased").With(
        NMock2.Is.EqualTo(Keys.Z)
      );

      Expect.Once.On(subscriber).Method("KeyPressed").With(
        NMock2.Is.EqualTo(Keys.None)
      );
      Expect.Once.On(subscriber).Method("CharacterEntered").With(
        NMock2.Is.EqualTo('™')
      );
      Expect.Once.On(subscriber).Method("KeyReleased").With(
        NMock2.Is.EqualTo(Keys.None)
      );

      this.keyboard.Update();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the buffered keyboard</summary>
    /// <returns>A subscriber registered to the events of the keyboard</returns>
    private IKeyboardSubscriber mockSubscriber() {
      IKeyboardSubscriber subscriber = this.mockery.NewMock<IKeyboardSubscriber>();

      this.keyboard.KeyPressed += subscriber.KeyPressed;
      this.keyboard.KeyReleased += subscriber.KeyReleased;
      this.keyboard.CharacterEntered += subscriber.CharacterEntered;

      return subscriber;
    }

    /// <summary>Creates dynamic mock objects for interfaces</summary>
    private Mockery mockery;
    /// <summary>Buffered keyboard being tested</summary>
    private MockedKeyboard keyboard;

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
