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

#if UNITTEST

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Game.States {

  /// <summary>Unit test for the game state class</summary>
  [TestFixture]
  internal class GameStateTest {

    #region class TestGameState

    /// <summary>Game state used for unit testing</summary>
    private class TestGameState : GameState {

      /// <summary>Initializes a new test game state</summary>
      /// <param name="manager">
      ///   Game state manager the test game state belongs to
      /// </param>
      public TestGameState(GameStateManager manager) : base(manager) { }

      /// <summary>
      ///   Allows the game state to run logic such as updating the world,
      ///   checking for collisions, gathering input and playing audio.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values</param>
      public override void Update(GameTime gameTime) {
        ++this.UpdateCallCount;
        base.Update(gameTime);
      }

      /// <summary>This is called when the game state should draw itself</summary>
      /// <param name="gameTime">Provides a snapshot of timing values</param>
      public override void Draw(GameTime gameTime) {
        ++this.DrawCallCount;
        base.Draw(gameTime);
      }

      /// <summary>Game state manager this game state belongs to</summary>
      public new GameStateManager GameStateManager {
        get { return base.GameStateManager; }
      }

      /// <summary>Called when the game state has been entered</summary>
      protected override void OnEntered() {
        ++this.OnEnteredCallCount;
        base.OnEntered();
      }

      /// <summary>Called when the game state is being left again</summary>
      protected override void OnLeaving() {
        ++this.OnLeavingCallCount;
        base.OnLeaving();
      }

      /// <summary>Called when the game state should enter pause mode</summary>
      protected override void OnPause() {
        ++this.OnPauseCallCount;
        base.OnPause();
      }

      /// <summary>Called when the game state should resume from pause mode</summary>
      protected override void OnResume() {
        ++this.OnResumeCallCount;
        base.OnResume();
      }
      
      /// <summary>Number of calls to the Update() method</summary>
      public int UpdateCallCount;
      /// <summary>Number of calls to the DraW() method</summary>
      public int DrawCallCount;
      /// <summary>Number of calls to the OnEntered() method</summary>
      public int OnEnteredCallCount;
      /// <summary>Number of calls to the OnLeaving() method</summary>
      public int OnLeavingCallCount;
      /// <summary>Number of calls to the OnPause() method</summary>
      public int OnPauseCallCount;
      /// <summary>Number of calls to the OnResume() method</summary>
      public int OnResumeCallCount;

    }

    #endregion // class TestGameState

    /// <summary>
    ///   Verifies that the constructor of the game state class is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      GameState gameState = new TestGameState(manager);
    }

    /// <summary>
    ///   Verifies that the Update() call is forwarded to the active game state
    /// </summary>
    [Test]
    public void TestUpdateForwarding() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState gameState = new TestGameState(manager);
      manager.Switch(gameState);
      
      manager.Update(new GameTime());
      Assert.AreEqual(1, gameState.UpdateCallCount);
    }

    /// <summary>
    ///   Verifies that the Draw() call is forwarded to the active game state
    /// </summary>
    [Test]
    public void TestDrawPropagation() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState gameState = new TestGameState(manager);
      manager.Switch(gameState);

      manager.Draw(new GameTime());
      Assert.AreEqual(1, gameState.DrawCallCount);
    }

    /// <summary>
    ///   Tests whether the Enter() and Leave() notifications of a game state are
    ///   invoked at the appropriate times
    /// </summary>
    [Test]
    public void TestEnterLeave() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState gameState = new TestGameState(manager);
      
      Assert.AreEqual(0, gameState.OnEnteredCallCount);
      manager.Push(gameState);
      Assert.AreEqual(1, gameState.OnEnteredCallCount);
      
      Assert.AreEqual(0, gameState.OnLeavingCallCount);
      manager.Pop();
      Assert.AreEqual(1, gameState.OnLeavingCallCount);
    }

    /// <summary>
    ///   Tests whether the Pause() and Resume() notifications of a game state are
    ///   invoked at the appropriate times
    /// </summary>
    [Test]
    public void TestPauseResume() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState gameState = new TestGameState(manager);
      manager.Push(gameState);

      Assert.AreEqual(0, gameState.OnPauseCallCount);
      manager.Push(new TestGameState(manager));
      Assert.AreEqual(1, gameState.OnPauseCallCount);

      Assert.AreEqual(0, gameState.OnResumeCallCount);
      manager.Pop();
      Assert.AreEqual(1, gameState.OnResumeCallCount);
    }

    /// <summary>
    ///   Verifies that the game state manager reference can be looked up by
    ///   the game state class
    /// </summary>
    [Test]
    public void TestGameStateManagerReference() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState gameState = new TestGameState(manager);

      Assert.AreSame(manager, gameState.GameStateManager);
    }

  }

} // namespace Nuclex.Game.States

#endif // UNITTEST
