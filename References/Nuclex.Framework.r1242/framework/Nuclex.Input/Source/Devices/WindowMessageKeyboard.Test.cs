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

  /// <summary>Unit tests for the window message based keyboard</summary>
  [TestFixture]
  internal class WindowMessageKeyboardTest {

    #region class TestKeyboardMessageSource

    /// <summary>Dummy implementation of a keyboard message source</summary>
    private class TestKeyboardMessageSource : IKeyboardMessageSource {

      /// <summary>Triggered when a key has been pressed down</summary>
      public event KeyboardKeyEventDelegate KeyPressed { add { } remove { } }

      /// <summary>Triggered when a key has been released again</summary>
      public event KeyboardKeyEventDelegate KeyReleased { add { } remove { } }

      /// <summary>Triggered when the user has entered a character</summary>
      public event KeyboardCharacterEventDelegate CharacterEntered { add { } remove { } }

    }

    #endregion // class TestKeyboardMessageSource

    /// <summary>Verifies that the IsAttached property is working</summary>
    [Test]
    public void TestIsAttached() {
      using (var keyboard = new WindowMessageKeyboard(new TestKeyboardMessageSource())) {
        Assert.IsTrue(keyboard.IsAttached);
      }
    }

    /// <summary>Verifies that the IsAttached property is working</summary>
    [Test]
    public void TestName() {
      using (var keyboard = new WindowMessageKeyboard(new TestKeyboardMessageSource())) {
        StringAssert.Contains("keyboard", keyboard.Name.ToLower());
      }
    }

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
