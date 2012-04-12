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
  ///   Game state that displays a loading screen while another state loads
  /// </summary>
  public class LoadingScreenState<LoadedGameStateType> : GameState, IDisposable
    where LoadedGameStateType : GameState, ILoadableGameState {

    /// <summary>Initializes a new loading screen game state</summary>
    /// <param name="gameStateManager">
    ///   Game state manager the loading screen state belongs to
    /// </param>
    /// <param name="gameStateToLoad">
    ///   Game state that will be loaded by the loading screen
    /// </param>
    public LoadingScreenState(
      GameStateManager gameStateManager, LoadedGameStateType gameStateToLoad
    ) :
      base(gameStateManager) {
      this.gameStateToLoad = gameStateToLoad;

      this.progressChangedDelegate = new EventHandler<LoadProgressEventArgs>(
        progressChanged
      );
    }

    /// <summary>Gives the game component a chance to initialize itself</summary>
    public override void Initialize() {
      base.Initialize();

      this.gameStateToLoad.Initialize();
    }

    /// <summary>Immediately releases all resources used by the instance</summary>
    public override void Dispose() {
      IAsyncResult asyncResult = this.asyncResult;
      if(asyncResult != null) {
        if(!asyncResult.IsCompleted) {
#if NO_EXITCONTEXT
          asyncResult.AsyncWaitHandle.WaitOne(15000);
#else
          asyncResult.AsyncWaitHandle.WaitOne(15000, false);
#endif
        }
      }
      if(this.gameStateToLoad != null) {
        this.gameStateToLoad.ProgressChanged -= this.progressChangedDelegate;
        this.gameStateToLoad = null;
      }

      base.Dispose();
    }

    /// <summary>
    ///   Begins loading the loadable game state associated with the loading screen
    /// </summary>
    public void StartLoading() {
      bool redundantCall =
        (this.gameStateLoaded) ||
        (this.asyncResult != null);

      if(redundantCall) {
        System.Diagnostics.Debug.WriteLine(
          "Warning: redundant call to LoadScreenState.StartLoading() encountered"
        );
        return;
      }

      // Subscribe to the progress change event
      this.gameStateToLoad.ProgressChanged += this.progressChangedDelegate;

      // Begin loading the game state this loading screen was associated with
      this.asyncResult = this.gameStateToLoad.BeginLoad(
        new AsyncCallback(loadingCompleted), null
      );
      if(this.asyncResult.CompletedSynchronously) {
        endLoading();
      }
    }

    /// <summary>
    ///   Allows the game state to run logic such as updating the world,
    ///   checking for collisions, gathering input and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Update(GameTime gameTime) {
      if(this.gameStateLoaded) {
        // Game state is loaded, kill the loading screen and switch to the new state
        GameStateManager.Switch(this.gameStateToLoad);
      }
    }

    /// <summary>Called when the load progress has changed</summary>
    /// <param name="progress">New load progress</param>
    protected virtual void OnLoadProgressChanged(float progress) { }

    /// <summary>Game state being loaded by the loading screen</summary>
    protected LoadedGameStateType GameStateToLoad {
      get { return this.gameStateToLoad; }
    }

    /// <summary>Ends the asynchronous loading operation</summary>
    /// <remarks>
    ///   Should only be called when the game state has notified us that loading is
    ///   complete, otherwise, the caller will be blocked until loading actually completes.
    /// </remarks>
    private void endLoading() {
      this.gameStateToLoad.EndLoad(this.asyncResult);

      // Unsubscribe from the progress change event
      this.gameStateToLoad.ProgressChanged -= this.progressChangedDelegate;

      this.asyncResult = null;
    }

    /// <summary>Called when the associated game state has finished loading</summary>
    /// <param name="asyncResult">Handle of the asynchronous loading operation</param>
    private void loadingCompleted(IAsyncResult asyncResult) {
      this.gameStateLoaded = true;

      // Take into account that the game state may have completed its loading
      // synchronously, in which case the StartLoading() method will initiate
      // the endLoading() call. Not strictly neccessary here, since we're not
      // running a loop with the risk for endless recursion, but might be
      // preferrable for the implementer of ILoadableGameState.BeginLoad().
      if(!asyncResult.CompletedSynchronously) {
        endLoading();
      }
    }

    /// <summary>Called when the loading progress has changed</summary>
    /// <param name="sender">Game state whose loading progress has changed</param>
    /// <param name="arguments">Contains the new loading progress</param>
    private void progressChanged(object sender, LoadProgressEventArgs arguments) {
      OnLoadProgressChanged(arguments.Progress);
    }

    /// <summary>Delegate we suscribe to the progress change notification</summary>
    private EventHandler<LoadProgressEventArgs> progressChangedDelegate;
    /// <summary>Game state being loaded by the loading screen</summary>
    private LoadedGameStateType gameStateToLoad;
    /// <summary>
    ///   Whether the game state associated with the loading screen has been loaded already
    /// </summary>
    private volatile bool gameStateLoaded;
    /// <summary>Result handle for the asynchronous loading operation</summary>
    private volatile IAsyncResult asyncResult;

  }

} // namespace Nuclex.Game.States
