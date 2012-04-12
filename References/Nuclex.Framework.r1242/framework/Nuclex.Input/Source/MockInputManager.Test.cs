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

namespace Nuclex.Input {

  /// <summary>Unit tests for the fake input device provider</summary>
  [TestFixture]
  internal class MockInputManagerTest {

    /// <summary>Ensures that the default constructor is working</summary>
    [Test]
    public void TestDefaultConstructor() {
      using (var manager = new InputManager()) {
        Assert.IsNotNull(manager); // nonsense, avoids compiler warning
      }
    }

    /// <summary>Ensures that the service container constructor is working</summary>
    [Test]
    public void TestServiceConstructor() {
      var services = new GameServiceContainer();

      using (var manager = new MockInputManager(services)) {
        Assert.IsNotNull(services.GetService(typeof(IInputService)));
      }

      Assert.IsNull(services.GetService(typeof(IInputService)));
    }

    /// <summary>
    ///   Verifies that the expected number of keyboards are in the keyboards collection
    /// </summary>
    [Test]
    public void TestKeyboardCollection() {
      using (var manager = new MockInputManager()) {
        Assert.AreEqual(5, manager.Keyboards.Count);
        Assert.AreEqual(5, ((IInputService)manager).Keyboards.Count);
      }
    }

    /// <summary>
    ///   Verifies that the expected number of mice are in the mice collection
    /// </summary>
    [Test]
    public void TestMouseCollection() {
      using (var manager = new MockInputManager()) {
        Assert.AreEqual(1, manager.Mice.Count);
        Assert.AreEqual(1, ((IInputService)manager).Mice.Count);
      }
    }

    /// <summary>
    ///   Verifies that the expected number of game pads are in the game pads collection
    /// </summary>
    [Test]
    public void TestGamePadCollection() {
      using (var manager = new MockInputManager()) {
        Assert.AreEqual(8, manager.GamePads.Count);
        Assert.AreEqual(8, ((IInputService)manager).GamePads.Count);
      }
    }

    /// <summary>
    ///   Verifies that the mouse can be retrieved from the input manager
    /// </summary>
    [Test]
    public void TestGetMouse() {
      using (var manager = new MockInputManager()) {
        Assert.IsNotNull(manager.GetMouse());
        Assert.IsNotNull(((IInputService)manager).GetMouse());
      }
    }

    /// <summary>
    ///   Verifies that the PC keyboard can be retrieved from the input manager
    /// </summary>
    [Test]
    public void TestGetKeyboard() {
      using (var manager = new MockInputManager()) {
        Assert.IsNotNull(manager.GetKeyboard());
        Assert.IsNotNull(((IInputService)manager).GetKeyboard());
      }
    }

    /// <summary>
    ///   Verifies that XBox 360 chat pads can be retrieved from the input manager
    /// </summary>
    [
      Test,
      TestCase(PlayerIndex.One),
      TestCase(PlayerIndex.Two),
      TestCase(PlayerIndex.Three),
      TestCase(PlayerIndex.Four)
    ]
    public void TestGetChatPad(PlayerIndex playerIndex) {
      using (var manager = new MockInputManager()) {
        Assert.IsNotNull(manager.GetKeyboard(playerIndex));
        Assert.IsNotNull(((IInputService)manager).GetKeyboard(playerIndex));
      }
    }

    /// <summary>
    ///   Verifies that game pads can be retrieved from the input manager
    /// </summary>
    [
      Test,
      TestCase(PlayerIndex.One),
      TestCase(PlayerIndex.Two),
      TestCase(PlayerIndex.Three),
      TestCase(PlayerIndex.Four)
    ]
    public void TestGetXinputGamePad(PlayerIndex playerIndex) {
      using (var manager = new MockInputManager()) {
        Assert.IsNotNull(manager.GetGamePad(playerIndex));
        Assert.IsNotNull(((IInputService)manager).GetGamePad(playerIndex));
      }
    }

    /// <summary>
    ///   Verifies that game pads can be retrieved from the input manager
    /// </summary>
    [
      Test,
      TestCase(ExtendedPlayerIndex.Five),
      TestCase(ExtendedPlayerIndex.Six),
      TestCase(ExtendedPlayerIndex.Seven),
      TestCase(ExtendedPlayerIndex.Eight)
    ]
    public void TestGetDirectInputGamePad(ExtendedPlayerIndex playerIndex) {
      using (var manager = new MockInputManager()) {
        Assert.IsNotNull(manager.GetGamePad(playerIndex));
        Assert.IsNotNull(((IInputService)manager).GetGamePad(playerIndex));
      }
    }
    
    /// <summary>
    ///   Verifies that the snap shot system is working in the mocked input manager
    /// </summary>
    [Test]
    public void TestSnapshots() {
      using (var manager = new MockInputManager()) {
        Assert.AreEqual(0, manager.SnapshotCount);
        manager.TakeSnapshot();
        Assert.AreEqual(1, manager.SnapshotCount);
        manager.Update();
        Assert.AreEqual(0, manager.SnapshotCount);
      }
    }

  }

} // namespace Nuclex.Input

#endif // UNITTEST
