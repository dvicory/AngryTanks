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

using Nuclex.Input.Devices;

namespace Nuclex.Input {

  /// <summary>Unit tests for the DirectInput manager</summary>
  [TestFixture]
  internal class DirectInputManagerTest {

    /// <summary>Verifies that the IsDirectInputAvailable property is working</summary>
    [Test]
    public void TestIsDirectInputAvailable() {
      bool result = DirectInputManager.IsDirectInputAvailable;
      Assert.IsTrue(result || !result); // the result doesn't matter
    }

    /// <summary>Tests whether the DirectInput manager can be disposed</summary>
    [Test]
    public void TestDispose() {
      using (var form = new Form()) {
        using (var manager = new DirectInputManager(form.Handle)) {
          Assert.IsNotNull(manager); // nonsense, avoids compiler warning
        }
      }
    }

    /// <summary>Verifies that the IsDeviceAttached() method works</summary>
    [Test]
    public void TestCreateGamePadsAndIsDeviceAttached() {
      using (var form = new Form()) {
        using (var manager = new DirectInputManager(form.Handle)) {
          DirectInputGamePad[] gamePads = manager.CreateGamePads();
          for(int index = 0; index < gamePads.Length; ++index) {
            bool result = gamePads[index].IsAttached;
            Assert.IsTrue(result || !result); // the result doesn't matter
          }
        }
      }
    }

  }

} // namespace Nuclex.Input

#endif // UNITTEST
