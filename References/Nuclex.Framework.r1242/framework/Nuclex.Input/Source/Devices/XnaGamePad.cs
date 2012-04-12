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

using MsGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Nuclex.Input.Devices {

  /// <summary>Interfaces with an XBox 360 controller via XNA (XINPUT)</summary>
  internal class XnaGamePad : GamePad {

    /// <summary>Initializes a new XNA-based keyboard device</summary>
    public XnaGamePad(PlayerIndex playerIndex) {
      this.playerIndex = playerIndex;
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
      get { return this.current.IsConnected; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "XBox 360 game pad #" + ((int)this.playerIndex + 1).ToString(); }
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
    
      if(this.states.Count == 0) {
        this.current = MsGamePad.GetState(this.playerIndex);
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
      this.states.Enqueue(MsGamePad.GetState(this.playerIndex));
    }

    /// <summary>Index of the player this device represents</summary>
    private PlayerIndex playerIndex;

    /// <summary>Snapshots of the game pad state waiting to be processed</summary>
    private Queue<GamePadState> states;

    /// <summary>Currently published game pad state</summary>
    private GamePadState current;

  }

} // namespace Nuclex.Input.Devices
