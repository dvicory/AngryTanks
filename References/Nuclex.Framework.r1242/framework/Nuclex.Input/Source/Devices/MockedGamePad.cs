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
using Microsoft.Xna.Framework;

#if !XBOX360

namespace Nuclex.Input.Devices {

  /// <summary>Code-controllable game pad for unit testing</summary>
  public class MockedGamePad : GamePad {

    /// <summary>Initializes a new mocked game pad</summary>
    public MockedGamePad() {
      this.states = new Queue<GamePadState>();
      this.current = new GamePadState();
    }

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    public override GamePadState GetState() {
      return this.current;
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return this.isAttached; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "Mocked game pad"; }
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
        this.current = buildState();
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
      this.states.Enqueue(buildState());
    }

    /// <summary>Presses the specified buttons on the game pad</summary>
    /// <param name="buttons">Buttons that will be pressed</param>
    public void Press(Buttons buttons) {
      this.buttons |= buttons;
    }

    /// <summary>Releases the specified buttons on the game pad</summary>
    /// <param name="buttons">Buttons that will be released</param>
    public void Release(Buttons buttons) {
      this.buttons &= ~buttons;
    }

    /// <summary>Moves the left thumb stick to the specified position</summary>
    /// <param name="x">X coordinate of the thumb stick's position</param>
    /// <param name="y">Y coordinate of the thumb stick's position</param>
    public void MoveLeftThumbStick(float x, float y) {
      this.leftThumbStick.X = x;
      this.leftThumbStick.Y = y;
    }

    /// <summary>Moves the right thumb stick to the specified position</summary>
    /// <param name="x">X coordinate of the thumb stick's position</param>
    /// <param name="y">Y coordinate of the thumb stick's position</param>
    public void MoveRightThumbStick(float x, float y) {
      this.rightThumbStick.X = x;
      this.rightThumbStick.Y = y;
    }

    /// <summary>Pushes the left analog trigger to the specified depth</summary>
    /// <param name="depth">Depth the left analog trigger will be pushed to</param>
    public void PushLeftTrigger(float depth) {
      this.leftTrigger = depth;
    }

    /// <summary>Pushes the right analog trigger to the specified depth</summary>
    /// <param name="depth">Depth the right analog trigger will be pushed to</param>
    public void PushRightTrigger(float depth) {
      this.rightTrigger = depth;
    }

    /// <summary>Attaches (connects) the game pad</summary>
    public void Attach() {
      this.isAttached = true;
    }

    /// <summary>Detaches (disconnects) the game pad</summary>
    public void Detach() {
      this.isAttached = false;
    }

    /// <summary>Builds a game pad state from the current settings</summary>
    /// <returns>The new game pad state</returns>
    private GamePadState buildState() {
      bool dpadUp = (this.buttons & Buttons.DPadUp) != 0;
      bool dpadDown = (this.buttons & Buttons.DPadDown) != 0;
      bool dpadLeft = (this.buttons & Buttons.DPadLeft) != 0;
      bool dpadRight = (this.buttons & Buttons.DPadRight) != 0;

      return new GamePadState(
        new GamePadThumbSticks(this.leftThumbStick, this.rightThumbStick),
        new GamePadTriggers(this.leftTrigger, this.rightTrigger),
        new GamePadButtons(this.buttons),
        new GamePadDPad(
          dpadUp ? ButtonState.Pressed : ButtonState.Released,
          dpadDown ? ButtonState.Pressed : ButtonState.Released,
          dpadLeft ? ButtonState.Pressed : ButtonState.Released,
          dpadRight ? ButtonState.Pressed : ButtonState.Released
        )
      );
    }

    /// <summary>Snapshots of the game pad state waiting to be processed</summary>
    private Queue<GamePadState> states;

    /// <summary>Currently published game pad state</summary>
    private GamePadState current;

    /// <summary>Whether the game pad is attached</summary>
    private bool isAttached;

    /// <summary>Current position of the left thumb stick</summary>
    private Vector2 leftThumbStick;
    /// <summary>Current position of the right thumb stick</summary>
    private Vector2 rightThumbStick;
    /// <summary>Current depth the left trigger is pushed</summary>
    private float leftTrigger;
    /// <summary>Current depth the right trigger is pushed</summary>
    private float rightTrigger;
    /// <summary>Buttons that are currently pressed on the game pad</summary>
    private Buttons buttons;

  }

} // namespace Nuclex.Input.Devices

#endif // !XBOX360
