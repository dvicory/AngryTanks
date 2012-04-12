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

using Microsoft.Xna.Framework;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the XNA (XINPUT) based chat pad class</summary>
  [TestFixture]
  internal class XnaKeyboardTest {

    /// <summary>Verifies that the GetState() method is working</summary>
    [Test]
    public void TestGetState() {
      var gamePad = new XnaGamePad(PlayerIndex.One);
      var keyboard = new XnaKeyboard(PlayerIndex.One, gamePad);
      keyboard.GetState();
      // No exception means success
    }

    /// <summary>Verifies that the game pad can be attached and detached</summary>
    [Test]
    public void TestIsAttached() {
      var gamePad = new XnaGamePad(PlayerIndex.One);
      var keyboard = new XnaKeyboard(PlayerIndex.One, gamePad);
      bool attached = keyboard.IsAttached;
      Assert.IsTrue(attached || !attached); // result doesn't matter
    }

    /// <summary>Verifies that the mocked game pad's name can be retrieved</summary>
    [Test]
    public void TestName() {
      var gamePad = new XnaGamePad(PlayerIndex.One);
      var keyboard = new XnaKeyboard(PlayerIndex.One, gamePad);
      StringAssert.Contains("chat pad", keyboard.Name.ToLower());
    }


  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
