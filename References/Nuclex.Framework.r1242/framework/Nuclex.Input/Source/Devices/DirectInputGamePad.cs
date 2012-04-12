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

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

#if !NO_DIRECTINPUT

using SlimDX;
using SlimDX.DirectInput;

namespace Nuclex.Input.Devices {

  /// <summary>Interfaces with a game pad-like controller through DirectInput</summary>
  internal class DirectInputGamePad : GamePad, IDisposable {

    /// <summary>Initializes a new DirectInput-based game pad</summary>
    /// <param name="joystick">The DirectInput joystick this instance will query</param>
    /// <param name="checkAttachedDelegate">
    ///   Delegate through which the instance can check if the device is attached
    /// </param>
    public DirectInputGamePad(
      Joystick joystick, CheckAttachedDelegate checkAttachedDelegate
    ) {
      this.joystick = joystick;
      this.checkAttachedDelegate = checkAttachedDelegate;

      this.states = new Queue<GamePadState>();

      this.currentJoystickState = new JoystickState();
      this.current = new GamePadState();

      this.converter = new GamePadConverter(this.joystick);
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if (this.joystick != null) {
        this.joystick.Unacquire();
        this.joystick.Dispose();
      }
    }

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    public override GamePadState GetState() {
      return this.converter.Convert(ref this.currentJoystickState);
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return this.checkAttachedDelegate(this.joystick); }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return this.joystick.Information.InstanceName; }
    }

    /// <summary>Updates the state of the input device</summary>
    /// <remarks>
    ///   <para>
    ///     If this method is called with no snapshots in the queue, it will take
    ///     an immediate snapshot and make it the current state. This way, you
    ///     can use the input devices without caring for the snapshot system if
    ///     you wish.
    ///   </para>
    ///   <para>
    ///     If this method is called while one or more snapshots are waiting in
    ///     the queue, this method takes the next snapshot from the queue and makes
    ///     it the current state.
    ///   </para>
    /// </remarks>
    public override void Update() {
      GamePadState previous = this.current;

      if (this.states.Count == 0) {
        if (queryState()) {
          this.current = this.converter.Convert(ref this.currentJoystickState);
        }
      } else {
        this.current = this.states.Dequeue();
      }

      GenerateEvents(ref previous, ref this.current);
    }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public override void TakeSnapshot() {
      queryState();
      this.states.Enqueue(this.converter.Convert(ref this.currentJoystickState));
    }

    /// <summary>Tries to retrieve the current state of the input device</summary>
    /// <returns>True if the state was successfully retrieved</returns>
    private bool queryState() {
      Result result;

      // DirectInput devices should be acquired before data can be obtained from
      // them. However, they can unacquire themselves when the user Alt+Tabs out,
      // after which all input retrieval methods return DIERR_INPUTLOST. If we got
      // a DIERR_INPUTLOST (or never acquired the device before), we will start
      // by attempting to acquire the device. If it fails, we'll try again in
      // the next update cycle.
      if (!this.currentlyAcquired) {
        result = this.joystick.Acquire();
        if (result == ResultCode.InputLost) {
          return false;
        }

        this.currentlyAcquired = true;
      }

      // Some devices which do not generate events need to be polled. According
      // to the docs, calling this method when the device doesn't need to be polled
      // is very fast and causes no damage.
      result = this.joystick.Poll();
      if (result == ResultCode.InputLost) {
        this.currentlyAcquired = false;
        return false;
      }

      // Finally, take the current state from the device. Using the ref overload
      // of SlimDX, this can be done without producing a single byte of garbage.
      result = this.joystick.GetCurrentState(ref this.currentJoystickState);
      if (result == ResultCode.InputLost) {
        this.currentlyAcquired = false;
        return false;
      }

      return true;
    }

    /// <summary>The DirectInput joystick wrapped by this instance</summary>
    private Joystick joystick;

    /// <summary>
    ///   Delegate through which the instance can check if the device is attached
    /// </summary>
    private CheckAttachedDelegate checkAttachedDelegate;

    /// <summary>Whether the device should currently be in the acquired state</summary>
    private bool currentlyAcquired;

    /// <summary>The current state as provided by DirectInput</summary>
    private JoystickState currentJoystickState;

    /// <summary>Snapshots of the game pad state waiting to be processed</summary>
    private Queue<GamePadState> states;

    /// <summary>Converts joystick states into game pad states</summary>
    private GamePadConverter converter;

    /// <summary>Currently published game pad state</summary>
    private GamePadState current;

  }

} // namespace Nuclex.Input.Devices

#endif // !NO_DIRECTINPUT
