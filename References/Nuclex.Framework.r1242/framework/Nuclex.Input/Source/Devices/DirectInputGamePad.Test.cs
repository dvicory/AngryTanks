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

using SlimDX.DirectInput;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Input.Devices {

  /// <summary>Unit tests for the DirectInput-based game pad</summary>
  [TestFixture]
  internal class DirectInputGamePadTest {

    /// <summary>Called before each test is run</summary>
    [Test]
    public void Setup() {
      this.directInput = new DirectInput();

      IList<DeviceInstance> devices = this.directInput.GetDevices(
        DeviceClass.GameController, DeviceEnumerationFlags.AllDevices
      );
      if (devices.Count > 0) {
        this.joystick = new Joystick(this.directInput, devices[0].InstanceGuid);
      }
    }

    /// <summary>Called after each test has run</summary>
    [Test]
    public void Teardown() {
      if (this.joystick != null) {
        this.joystick.Dispose();
        this.joystick = null;
      }
      if (this.directInput != null) {
        this.directInput.Dispose();
        this.directInput = null;
      }
    }

    /// <summary>Verifies that the constructor is working</summary>
    [Test]
    public void TestConstructor() {
      requireAttachedJoystick();

      using (var gamePad = new DirectInputGamePad(this.joystick, isAttachedMock)) {
        this.joystick = null; // ownership transfer
      }
    }

    /// <summary>Verifies that the Name method returns the correct result</summary>
    [Test]
    public void TestName() {
      requireAttachedJoystick();
      
      string expected = this.joystick.Information.InstanceName;

      using (var gamePad = new DirectInputGamePad(this.joystick, isAttachedMock)) {
        this.joystick = null; // ownership transfer

        StringAssert.Contains(expected, gamePad.Name);
      }
    }

    /// <summary>Verifies that the IsAttached method is working</summary>
    [
      Test,
      TestCase(true),
      TestCase(false)
    ]
    public void TestAttached(bool expected) {
      requireAttachedJoystick();
      
      CheckAttachedDelegate attachedCheck;
      if(expected) {
        attachedCheck = isAttachedMock;
      } else {
        attachedCheck = isNotAttachedMock;
      }

      using (var gamePad = new DirectInputGamePad(this.joystick, attachedCheck)) {
        this.joystick = null; // ownership transfer

        Assert.AreEqual(expected, gamePad.IsAttached);
      }
    }

    /// <summary>Reports that a device is attached</summary>
    /// <param name="device">Not used</param>
    /// <returns>Always true</returns>
    private static bool isAttachedMock(Device device) { return true; }

    /// <summary>Reports that a device is not attached</summary>
    /// <param name="device">Not used</param>
    /// <returns>Always false</returns>
    private static bool isNotAttachedMock(Device device) { return false; }

    /// <summary>Requires a joystick to be attached to the system</summary>
    /// <remarks>
    ///   Requiring hardware for a unit test is a big no-no, but SlimDX' wrappers arent
    ///   all based on funky interfaces, so there's no way to mock the darn thing.
    ///   Adding another layer of abstraction would only mean having no test coverage
    ///   for that layer of abstraction.
    /// </remarks>
    private void requireAttachedJoystick() {
      if (this.joystick == null) {
        Assert.Inconclusive("No DirectInput compatible josticks/game pads attached");
      }
    }

    /// <summary>DirectInput interface the game pad will be based on</summary>
    private DirectInput directInput;
    /// <summary>DirectInput joystick that will be used for testing</summary>
    private Joystick joystick;

  }

} // namespace Nuclex.Input.Devices

#endif // UNITTEST
