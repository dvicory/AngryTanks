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

  /// <summary>
  ///   Manages a the state and resources of a distinct state the game can be in
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This class follows the usual game state concept: Instead of using hard-to-maintain
  ///     if-else-trees for deciding whether to render the main menu, game scene or
  ///     credits scroller, each of the game's phases is put in its own game state class. This
  ///     improves modularity and prevents the mixing of code that normally has nothing to
  ///     with each other. The game state manager allows multiple states to be active at
  ///     the same time and manages these active states in a stack, which is useful for
  ///     realizing ingame menus and nested scenes.
  ///   </para>
  ///   <para>
  ///     Game states can be either active or inactive and will be notified when this state
  ///     changes by the OnEntered() and OnLeaving() methods. Any game state starts out as
  ///     being inactive. Game states should only load and keep their resources during their
  ///     active period and free them again when they become inactive.
  ///   </para>
  ///   <para>
  ///     In addition to being active and inactive, game states also have a pause mode. This
  ///     mode can only be entered by active game states and is used to put the game state
  ///     in the back when another game state serves as a means to halt the
  ///     time in the game state. If multiple game states are active, only the topmost state
  ///     will be in unpaused mode.
  ///   </para>
  /// </remarks>
  public abstract class GameState : GraphicsDeviceDrawableComponent {

    /// <summary>Initializes a new game state</summary>
    /// <param name="gameStateManager">Game state manager the game state belongs to</param>
    public GameState(GameStateManager gameStateManager) :
      base(gameStateManager.InternalGameServices) {

      this.gameStateManager = gameStateManager;
    }

    /// <summary>
    ///   Allows the game state to run logic such as updating the world,
    ///   checking for collisions, gathering input and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values</param>
    public override void Update(GameTime gameTime) { }

    /// <summary>This is called when the game state should draw itself</summary>
    /// <param name="gameTime">Provides a snapshot of timing values</param>
    public override void Draw(GameTime gameTime) { }

    /// <summary>Called when the game state has been entered</summary>
    protected virtual void OnEntered() { }

    /// <summary>Called when the game state is being left again</summary>
    protected virtual void OnLeaving() { }

    /// <summary>Called when the game state should enter pause mode</summary>
    protected virtual void OnPause() { }

    /// <summary>Called when the game state should resume from pause mode</summary>
    protected virtual void OnResume() { }

    /// <summary>Game state manager this game state belongs to</summary>
    protected GameStateManager GameStateManager {
      get { return this.gameStateManager; }
    }

    /// <summary>
    ///   Calls the state's OnPause() method;
    ///   for internal use by the game state manager
    /// </summary>
    internal void InternalPause() {
      if(!this.paused) {
        this.paused = true;
        OnPause();
      }
    }

    /// <summary>
    ///   Calls the state's OnResume() method;
    ///   for internal use by the game state manager
    /// </summary>
    internal void InternalResume() {
      if(this.paused) {
        this.paused = false;
        OnResume();
      }
    }

    /// <summary>
    ///   Calls the state's OnEntered() method;
    ///   for internal use by the game state manager
    /// </summary>
    internal void InternalEntered() {
      OnEntered();
    }

    /// <summary>
    ///   Calls the state's OnLeaving() method;
    ///   for internal use by the game state manager
    /// </summary>
    internal void InternalLeaving() {
      OnLeaving();
    }

    /// <summary>Game state manager this state belongs to</summary>
    private GameStateManager gameStateManager;

    /// <summary>Used to avoid pausing the game state multiple times</summary>
    private bool paused;

  }

} // namespace Nuclex.Game.States
