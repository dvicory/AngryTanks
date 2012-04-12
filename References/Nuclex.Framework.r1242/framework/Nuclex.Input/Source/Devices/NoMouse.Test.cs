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

  /// <summary>Unit tests for the dummy mouse</summary>
  [TestFixture]
  internal class NoMouseTest {

    /// <summary>Verifies that the constructor is working</summary>
    [Test]
    public void TestConstructor() {
      var mouse = new NoMouse();
      Assert.IsNotNull(mouse);
    }

    /// <summary>Verifies that the GetState() method is working</summary>
    [Test]
    public void TestGetState() {
      var mouse = new NoMouse();
      mouse.GetState();
      // No exception means success
    }

    /// <summary>Verifies that the keyboard dummy is not attached</summary>
    [Test]
    public void TestIsAttached() {
      var mouse = new NoMouse();
      Assert.IsFalse(mouse.IsAttached);
    }

    /// <summary>Verifies that the keyboard dummy's name can be retrieved</summary>
    [Test]
    public void TestName() {
      var mouse = new NoMouse();
      StringAssert.Contains("no", mouse.Name.ToLower());
    }

    /// <summary>Verifies that the TakeSnapshot() method works</summary>
    [Test]
    public void TestTakeSnapshot() {
      var mouse = new NoMouse();
      mouse.TakeSnapshot();
      // No exception means success
    }

    /// <summary>Verifies that the Update() method works</summary>
    [Test]
    public void TestUpdate() {
      var mouse = new NoMouse();
      mouse.Update();
      // No exception means success
    }
    
    /// <summary>Tests whether the no mouse class' events can be subscribed</summary>
    [Test]
    public void TestEvents() {
      var mouse = new NoMouse();
      
      mouse.MouseMoved += mouseMoved;
      mouse.MouseMoved -= mouseMoved;
      
      mouse.MouseButtonPressed += mouseButton;
      mouse.MouseButtonPressed -= mouseButton;
      
      mouse.MouseButtonReleased += mouseButton;
      mouse.MouseButtonReleased -= mouseButton;
      
      mouse.MouseWheelRotated += mouseWheel;
      mouse.MouseWheelRotated -= mouseWheel;
    }

    /// <summary>Dummy subscriber for the MouseMoved event</summary>
    /// <param name="x">X coordinate of the mouse cursor</param>
    /// <param name="y">Y coordinate of the mouse cursor</param>
    private static void mouseMoved(float x, float y) { }
    
    /// <summary>Dummy subscriber for the MouseButtonPressed/Released event</summary>
    /// <param name="buttons">Buttons that have been pressed/released</param>
    private static void mouseButton(MouseButtons buttons) { }

    /// <summary>Dummy subscriber for the MouseWheelRotated event</summary>
    /// <param name="ticks">Ticks the wheel has been rotated</param>
    private static void mouseWheel(float ticks) { }

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
