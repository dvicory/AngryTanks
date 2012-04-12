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

  /// <summary>Unit test for the game state manager</summary>
  [TestFixture]
  internal class GameStateManagerTest {

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

    #region class UnenterableGameState

    /// <summary>Game state the canont be entered</summary>
    private class UnenterableGameState : GameState {

      /// <summary>Initializes a new unenterable game state</summary>
      /// <param name="manager">
      ///   Game state manager the unenterable game state belongs to
      /// </param>
      public UnenterableGameState(GameStateManager manager) : base(manager) { }

      /// <summary>Called when the game state has been entered</summary>
      protected override void OnEntered() {
        throw new InvalidOperationException("Simulated error for unit testing");
      }

    }

    #endregion // class UnenterableGameState

    #region class UnresumableGameState

    /// <summary>Game state that cannot be resumed</summary>
    private class UnresumableGameState : GameState {

      /// <summary>Initializes a new unresumable game state</summary>
      /// <param name="manager">
      ///   Game state manager the unresumable game state belongs to
      /// </param>
      public UnresumableGameState(GameStateManager manager) : base(manager) { }

      /// <summary>Called when the game state should resume from pause mode</summary>
      protected override void OnResume() {
        throw new InvalidOperationException("Simulated error for unit testing");
      }

    }

    #endregion // class UnresumableGameState

    #region class ReentrantGameState

    /// <summary>Game state that nests another game state upon being entered</summary>
    private class ReentrantGameState : GameState {

      /// <summary>Initializes a new unresumable game state</summary>
      /// <param name="manager">
      ///   Game state manager the unresumable game state belongs to
      /// </param>
      public ReentrantGameState(GameStateManager manager) : base(manager) { }

      /// <summary>Called when the game state has been entered</summary>
      protected override void OnEntered() {
        GameStateManager.Push(new TestGameState(GameStateManager));
      }

    }

    #endregion // class ReentrantGameState

    /// <summary>
    ///   Verifies that the constructor of the game state manager is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
    }

    /// <summary>
    ///   Tests whether disposing the game state manager causes it to leave
    ///   the active game state
    /// </summary>
    [Test]
    public void TestLeaveOnDisposal() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState test = new TestGameState(manager);

      manager.Switch(test);

      Assert.AreEqual(0, test.OnLeavingCallCount);
      manager.Dispose();
      Assert.AreEqual(1, test.OnLeavingCallCount);
    }

    /// <summary>
    ///   Verifies that pushing another game state onto the game state manager's
    ///   stack pauses the original game state
    /// </summary>
    [Test]
    public void TestPauseOnPush() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState test = new TestGameState(manager);

      manager.Switch(test);

      Assert.AreEqual(0, test.OnPauseCallCount);
      manager.Push(new TestGameState(manager));
      Assert.AreEqual(1, test.OnPauseCallCount);
    }

    /// <summary>
    ///   Verifies that switch only replaces the active game state,
    ///   not the whole stack
    /// </summary>
    [Test]
    public void TestSwitchOnlyChangesActiveState() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState test1 = new TestGameState(manager);
      TestGameState test2 = new TestGameState(manager);

      manager.Push(test1);
      manager.Push(test2);

      Assert.AreEqual(0, test1.OnLeavingCallCount);
      Assert.AreEqual(0, test2.OnLeavingCallCount);

      manager.Switch(new TestGameState(manager));

      Assert.AreEqual(0, test1.OnLeavingCallCount);
      Assert.AreEqual(1, test2.OnLeavingCallCount);
    }

    /// <summary>
    ///   Verifies that an exception whilst pushing a state on the stack leaves the
    ///   game state manager unchanged
    /// </summary>
    [Test]
    public void TestPushUnenterableState() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState test = new TestGameState(manager);

      manager.Push(test);

      Assert.Throws<InvalidOperationException>(
        delegate() { manager.Push(new UnenterableGameState(manager)); }
      );

      // Make sure the test state is still running. Whether pause was
      // called zero times or more, we only care that it's running after
      // the push has failed
      Assert.AreEqual(test.OnResumeCallCount, test.OnPauseCallCount);
    }

    /// <summary>
    ///   Verifies that an exception whilst entering another state leaves the
    ///   game state manager unchanged
    /// </summary>
    [Test]
    public void TestSwitchToUnenterableState() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState test = new TestGameState(manager);

      manager.Push(new TestGameState(manager));
      manager.Push(test);

      Assert.Throws<InvalidOperationException>(
        delegate() { manager.Switch(new UnenterableGameState(manager)); }
      );

      // Make sure the test state is still running. Whether pause was
      // called zero times or more, we only care that it's running after
      // the push has failed
      Assert.AreEqual(test.OnResumeCallCount, test.OnPauseCallCount);
      // Make sure the state is entered (meaning entered has been called
      // one more time than leave)
      Assert.AreEqual(test.OnLeavingCallCount + 1, test.OnEnteredCallCount);
    }

    /// <summary>
    ///   Verifies that an exception whilst popping the stack to an unresumeable
    ///   state leaves the game state manager unchanged
    /// </summary>
    [Test]
    public void TestPopToUnresumableState() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      TestGameState test = new TestGameState(manager);

      manager.Push(new UnresumableGameState(manager));
      manager.Push(test);

      Assert.Throws<InvalidOperationException>(
        delegate() { manager.Pop(); }
      );

      Assert.AreEqual(test.OnPauseCallCount, test.OnResumeCallCount);
      Assert.AreEqual(test.OnLeavingCallCount + 1, test.OnEnteredCallCount);
    }

    /// <summary>
    ///   Verifies that the active game state can be queried
    /// </summary>
    [Test]
    public void TestActiveState() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());

      Assert.IsNull(manager.ActiveState);
      
      TestGameState test = new TestGameState(manager);
      manager.Push(test);

      Assert.AreSame(test, manager.ActiveState);
    }
    
    /// <summary>
    ///   Tests whether the game state manager correctly handles a reentrant call
    ///   to Push() from a pushed game state
    /// </summary>
    [Test]
    public void TestReeantrantPush() {
      GameStateManager manager = new GameStateManager(new GameServiceContainer());
      ReentrantGameState test = new ReentrantGameState(manager);
      
      manager.Push(test);

      // The reentrant game state pushes another game state onto the stack in its
      // OnEntered() notification. If this causes the stack to be built in the wrong
      // order, the ReentrantGameState would become the new active game state instead
      // of the sub-game-state it pushed onto the stack.
      Assert.AreNotSame(test, manager.ActiveState);
    }

  }

} // namespace Nuclex.Game.States

#endif // UNITTEST
