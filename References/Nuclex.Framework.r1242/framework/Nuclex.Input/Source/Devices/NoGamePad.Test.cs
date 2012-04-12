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
using Microsoft.Xna.Framework.Input;

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the game pad dummy</summary>
  [TestFixture]
  internal class NoGamePadTest {

    /// <summary>Verifies that the constructor is working</summary>
    [Test]
    public void TestConstructor() {
      var gamePad = new NoGamePad();
      Assert.IsNotNull(gamePad);
    }

    /// <summary>Verifies that the GetState() method is working</summary>
    [Test]
    public void TestGetState() {
      var gamePad = new NoGamePad();
      gamePad.GetState();
      // No exception means success
    }

    /// <summary>Verifies that the game pad dummy is not attached</summary>
    [Test]
    public void TestIsAttached() {
      var gamePad = new NoGamePad();
      Assert.IsFalse(gamePad.IsAttached);
    }

    /// <summary>Verifies that the game pad dummy's name can be retrieved</summary>
    [Test]
    public void TestName() {
      var gamePad = new NoGamePad();
      StringAssert.Contains("no", gamePad.Name.ToLower());
    }

    /// <summary>Verifies that the TakeSnapshot() method works</summary>
    [Test]
    public void TestTakeSnapshot() {
      var gamePad = new NoGamePad();
      gamePad.TakeSnapshot();
      // No exception means success
    }

    /// <summary>Verifies that the Update() method works</summary>
    [Test]
    public void TestUpdate() {
      var gamePad = new NoGamePad();
      gamePad.Update();
      // No exception means success
    }

    /// <summary>Tests whether the no game pad class' events can be subscribed</summary>
    [Test]
    public void TestEvents() {
      var gamePad = new NoGamePad();

      gamePad.ButtonPressed += button;
      gamePad.ButtonPressed -= button;

      gamePad.ButtonReleased += button;
      gamePad.ButtonReleased -= button;
    }

    /// <summary>Dummy subscriber for the ButtonPressed/Released event</summary>
    /// <param name="buttons">Buttons that have been pressed/released</param>
    private static void button(Buttons buttons) { }

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
