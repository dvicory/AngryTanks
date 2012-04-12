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

  /// <summary>Unit tests for the keyboard dummy</summary>
  [TestFixture]
  internal class NoKeyboardTest {

    /// <summary>Verifies that the constructor is working</summary>
    [Test]
    public void TestConstructor() {
      var keyboard = new NoKeyboard();
      Assert.IsNotNull(keyboard);
    }

    /// <summary>Verifies that the GetState() method is working</summary>
    [Test]
    public void TestGetState() {
      var keyboard = new NoKeyboard();
      keyboard.GetState();
      // No exception means success
    }

    /// <summary>Verifies that the keyboard dummy is not attached</summary>
    [Test]
    public void TestIsAttached() {
      var keyboard = new NoKeyboard();
      Assert.IsFalse(keyboard.IsAttached);
    }

    /// <summary>Verifies that the keyboard dummy's name can be retrieved</summary>
    [Test]
    public void TestName() {
      var keyboard = new NoKeyboard();
      StringAssert.Contains("no", keyboard.Name.ToLower());
    }

    /// <summary>Verifies that the TakeSnapshot() method works</summary>
    [Test]
    public void TestTakeSnapshot() {
      var keyboard = new NoKeyboard();
      keyboard.TakeSnapshot();
      // No exception means success
    }

    /// <summary>Verifies that the Update() method works</summary>
    [Test]
    public void TestUpdate() {
      var keyboard = new NoKeyboard();
      keyboard.Update();
      // No exception means success
    }

    /// <summary>Tests whether the no keyboard class' events can be subscribed</summary>
    [Test]
    public void TestEvents() {
      var keyboard = new NoKeyboard();
      
      keyboard.KeyPressed += key;
      keyboard.KeyPressed -= key;
      
      keyboard.KeyReleased += key;
      keyboard.KeyReleased -= key;
      
      keyboard.CharacterEntered += characterEntered;
      keyboard.CharacterEntered -= characterEntered;      
    }
    
    /// <summary>Dummy subscriber for KeyPressed/Released events</summary>
    /// <param name="key">Key that has been pressed/released</param>
    private static void key(Keys key) {}
    
    /// <summary>Dummy subscriber for the CharacterEntered event</summary>
    /// <param name="character">Character that has been entered</param>
    private static void characterEntered(char character) {}

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
