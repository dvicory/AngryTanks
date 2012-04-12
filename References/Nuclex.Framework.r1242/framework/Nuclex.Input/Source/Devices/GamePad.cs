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

namespace Nuclex.Input.Devices {

  /// <summary>Interfaces with an XBox 360 controller via XNA (XINPUT)</summary>
  public abstract class GamePad : IGamePad {

    /// <summary>Called when one or more buttons on the game pad have been pressed</summary>
    public event GamePadButtonDelegate ButtonPressed;
    /// <summary>Called when one or more buttons on the game pad have been released</summary>
    public event GamePadButtonDelegate ButtonReleased;

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    public abstract GamePadState GetState();

    /// <summary>Whether the input device is connected to the system</summary>
    public abstract bool IsAttached { get; }

    /// <summary>Human-readable name of the input device</summary>
    public abstract string Name { get; }

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
    public abstract void Update();

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public abstract void TakeSnapshot();

    /// <summary>Fires the ButtonPressed event</summary>
    /// <param name="buttons">Buttons that have been pressed</param>
    protected void OnButtonPressed(Buttons buttons) {
      if (ButtonPressed != null) {
        ButtonPressed(buttons);
      }
    }

    /// <summary>Fires the ButtonReleased event</summary>
    /// <param name="buttons">Buttons that have been released</param>
    protected void OnButtonReleased(Buttons buttons) {
      if (ButtonReleased != null) {
        ButtonReleased(buttons);
      }
    }

    /// <summary>Checks for state changes and triggers the corresponding events</summary>
    /// <param name="previous">Previous state of the game pad</param>
    /// <param name="current">Current state of the game pad</param>
    protected void GenerateEvents(ref GamePadState previous, ref GamePadState current) {
      if ((ButtonReleased == null) && (ButtonPressed == null)) {
        return;
      }

      Buttons pressedButtons = 0;
      Buttons releasedButtons = 0;

      // See if the state of the 'A' button has changed between two polls
      if (current.Buttons.A != previous.Buttons.A) {
        if (current.Buttons.A == ButtonState.Pressed) {
          pressedButtons |= Buttons.A;
        } else {
          releasedButtons |= Buttons.A;
        }
      }

      // See if the state of the 'B' button has changed between two polls
      if (current.Buttons.B != previous.Buttons.B) {
        if (current.Buttons.B == ButtonState.Pressed) {
          pressedButtons |= Buttons.B;
        } else {
          releasedButtons |= Buttons.B;
        }
      }

      // See if the state of the 'X' button has changed between two polls
      if (current.Buttons.X != previous.Buttons.X) {
        if (current.Buttons.X == ButtonState.Pressed) {
          pressedButtons |= Buttons.X;
        } else {
          releasedButtons |= Buttons.X;
        }
      }

      // See if the state of the 'A' button has changed between two polls
      if (current.Buttons.Y != previous.Buttons.Y) {
        if (current.Buttons.Y == ButtonState.Pressed) {
          pressedButtons |= Buttons.Y;
        } else {
          releasedButtons |= Buttons.Y;
        }
      }

      // See if the state of the left shoulder button has changed between two polls
      if (current.Buttons.LeftShoulder != previous.Buttons.LeftShoulder) {
        if (current.Buttons.LeftShoulder == ButtonState.Pressed) {
          pressedButtons |= Buttons.LeftShoulder;
        } else {
          releasedButtons |= Buttons.LeftShoulder;
        }
      }

      // See if the state of the right shoulder button has changed between two polls
      if (current.Buttons.RightShoulder != previous.Buttons.RightShoulder) {
        if (current.Buttons.RightShoulder == ButtonState.Pressed) {
          pressedButtons |= Buttons.RightShoulder;
        } else {
          releasedButtons |= Buttons.RightShoulder;
        }
      }

      // See if the state of the left stick button has changed between two polls
      if (current.Buttons.LeftStick != previous.Buttons.LeftStick) {
        if (current.Buttons.LeftStick == ButtonState.Pressed) {
          pressedButtons |= Buttons.LeftStick;
        } else {
          releasedButtons |= Buttons.LeftStick;
        }
      }


      // See if the state of the right stick button has changed between two polls
      if (current.Buttons.RightStick != previous.Buttons.RightStick) {
        if (current.Buttons.RightStick == ButtonState.Pressed) {
          pressedButtons |= Buttons.RightStick;
        } else {
          releasedButtons |= Buttons.RightStick;
        }
      }

      // See if the state of the start button has changed between two polls
      if (current.Buttons.Start != previous.Buttons.Start) {
        if (current.Buttons.Start == ButtonState.Pressed) {
          pressedButtons |= Buttons.Start;
        } else {
          releasedButtons |= Buttons.Start;
        }
      }

      // See if the state of the back button has changed between two polls
      if (current.Buttons.Back != previous.Buttons.Back) {
        if (current.Buttons.Back == ButtonState.Pressed) {
          pressedButtons |= Buttons.Back;
        } else {
          releasedButtons |= Buttons.Back;
        }
      }

      // See if the left side of the directional pad has changed
      if (current.DPad.Left != previous.DPad.Left) {
        if (current.DPad.Left == ButtonState.Pressed) {
          pressedButtons |= Buttons.DPadLeft;
        } else {
          releasedButtons |= Buttons.DPadLeft;
        }
      }

      // See if the right side of the directional pad has changed
      if (current.DPad.Right != previous.DPad.Right) {
        if (current.DPad.Right == ButtonState.Pressed) {
          pressedButtons |= Buttons.DPadRight;
        } else {
          releasedButtons |= Buttons.DPadRight;
        }
      }

      // See if the upper side of the directional pad has changed
      if (current.DPad.Up != previous.DPad.Up) {
        if (current.DPad.Up == ButtonState.Pressed) {
          pressedButtons |= Buttons.DPadUp;
        } else {
          releasedButtons |= Buttons.DPadUp;
        }
      }

      // See if the lower side of the directional pad has changed
      if (current.DPad.Down != previous.DPad.Down) {
        if (current.DPad.Down == ButtonState.Pressed) {
          pressedButtons |= Buttons.DPadDown;
        } else {
          releasedButtons |= Buttons.DPadDown;
        }
      }

      if (releasedButtons != 0) {
        OnButtonReleased(releasedButtons);
      }
      if (pressedButtons != 0) {
        OnButtonPressed(pressedButtons);
      }

    }

  }

} // namespace Nuclex.Input.Devices
