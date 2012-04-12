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

  /// <summary>Unit tests for the window message based mouse</summary>
  [TestFixture]
  internal class WindowMessageMouseTest {

    #region class TestMouseMessageSource

    /// <summary>Dummy implementation of a mouse message source</summary>
    private class TestMouseMessageSource : IMouseMessageSource {

      /// <summary>Triggered when a mouse button has been pressed</summary>
      public event MouseButtonEventDelegate MouseButtonPressed { add { } remove { } }

      /// <summary>Triggered when a mouse button has been released</summary>
      public event MouseButtonEventDelegate MouseButtonReleased { add { } remove { } }

      /// <summary>Triggered when the mouse has been moved</summary>
      public event MouseMoveEventDelegate MouseMoved { add { } remove { } }

      /// <summary>Triggered when the mouse wheel has been rotated</summary>
      public event MouseWheelEventDelegate MouseWheelRotated { add { } remove { } }

    }

    #endregion // class TestMouseMessageSource

    /// <summary>Verifies that the IsAttached property is working</summary>
    [Test]
    public void TestIsAttached() {
      using (var mouse = new WindowMessageMouse(new TestMouseMessageSource())) {
        Assert.IsTrue(mouse.IsAttached);
      }
    }

    /// <summary>Verifies that the IsAttached property is working</summary>
    [Test]
    public void TestName() {
      using (var mouse = new WindowMessageMouse(new TestMouseMessageSource())) {
        StringAssert.Contains("mouse", mouse.Name.ToLower());
      }
    }

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
