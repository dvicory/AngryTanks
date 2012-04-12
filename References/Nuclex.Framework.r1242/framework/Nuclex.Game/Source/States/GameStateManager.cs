#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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

using Microsoft.Xna.Framework;

namespace Nuclex.Game.States {

  /// <summary>Manages the game states and updates the active game state</summary>
  public class GameStateManager : GraphicsDeviceDrawableComponent, IGameStateService {

    /// <summary>Initializes a new game state manager</summary>
    /// <param name="gameServices">
    ///   Services container the game state manager will add itself to
    /// </param>
    public GameStateManager(GameServiceContainer gameServices) :
      base(gameServices) {

      this.gameServices = gameServices;
      this.activeGameStates = new Stack<GameState>();

      // Register ourselves as a service
      gameServices.AddService(typeof(IGameStateService), this);
    }

    /// <summary>Immediately releases all resources used by the component</summary>
    public override void Dispose() {
      leaveAllActiveStates();

      base.Dispose();
    }

    /// <summary>Updates the active game state</summary>
    /// <param name="gameTime">Snapshot of the game's timing values</param>
    public override void Update(GameTime gameTime) {

      if(this.activeGameStates.Count > 0) {
        this.activeGameStates.Peek().Update(gameTime);
      }

    }

    /// <summary>Draws the active game state</summary>
    /// <param name="gameTime">Snapshot of the game's timing values</param>
    public override void Draw(GameTime gameTime) {

      if(this.activeGameStates.Count > 0) {
        this.activeGameStates.Peek().Draw(gameTime);
      }

    }

    /// <summary>Pushes the specified state onto the state stack</summary>
    /// <param name="state">State that will be pushed onto the stack</param>
    public void Push(GameState state) {
      if(this.activeGameStates.Count > 0) {
        // Pause the state the was previously active
        this.activeGameStates.Peek().InternalPause();
      }

      try {
        this.activeGameStates.Push(state);

        // Push the new state onto the stack and notify it that it has become active
        try {
          state.InternalEntered();
        }
        catch(Exception) {
          this.activeGameStates.Pop();
          throw;
        }
      }
      catch(Exception) {
        if(this.activeGameStates.Count > 0) {
          this.activeGameStates.Peek().InternalResume();
        }
        throw;
      }
    }

    /// <summary>Takes the currently active game state from the stack</summary>
    public void Pop() {
      GameState topMostGameState = this.activeGameStates.Peek();
      topMostGameState.InternalLeaving();
      this.activeGameStates.Pop();

      if(this.activeGameStates.Count > 0) {
        try {
          this.activeGameStates.Peek().InternalResume();
        }
        catch(Exception) {
          topMostGameState.InternalEntered();
          this.activeGameStates.Push(topMostGameState);
          throw;
        }
      }
    }

    /// <summary>Switches the game to the specified state</summary>
    /// <param name="state">State the game will be switched to</param>
    /// <remarks>
    ///   This replaces the running game state in the stack with the specified state.
    /// </remarks>
    public void Switch(GameState state) {
      GameState topMostGameState;

      if(this.activeGameStates.Count > 0) {
        topMostGameState = this.activeGameStates.Peek();
        // If something goes wrong here, the stack is still in order
        topMostGameState.InternalLeaving();
        this.activeGameStates.Pop();
      } else {
        topMostGameState = null;
      }

      try {
        Push(state);
      }
      catch(Exception) {
        if(topMostGameState != null) {
          topMostGameState.InternalEntered();
          this.activeGameStates.Push(topMostGameState);
        }
        throw;
      }
    }

    /// <summary>The currently active game state. Can be null.</summary>
    public GameState ActiveState {
      get {
        if(this.activeGameStates.Count > 0) {
          return this.activeGameStates.Peek();
        } else {
          return null;
        }
      }
    }

    /// <summary>
    ///   Service container used to access services provided by external components
    /// </summary>
    internal GameServiceContainer InternalGameServices {
      get { return this.gameServices; }
    }

    /// <summary>Leaves all currently active game states</summary>
    private void leaveAllActiveStates() {
      while(this.activeGameStates.Count > 0) {
        GameState topMostGameState = this.activeGameStates.Pop();
        topMostGameState.InternalLeaving();
      }
    }

    /// <summary>Service container used for accessing external services</summary>
    private GameServiceContainer gameServices;
    /// <summary>Currently active game states</summary>
    /// <remarks>
    ///   The game state manager supports multiple active game states. For example,
    ///   a menu might appear on top of the running game. Only the topmost active
    ///   state receives input through the game 
    /// </remarks>
    private Stack<GameState> activeGameStates;

  }

} // namespace Nuclex.Game.States
