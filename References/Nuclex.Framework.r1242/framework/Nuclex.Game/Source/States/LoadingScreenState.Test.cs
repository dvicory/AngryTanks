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
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

using Nuclex.Graphics;
using Nuclex.Testing.Xna;

namespace Nuclex.Game.States {

  /// <summary>Unit test for the loading screen game state</summary>
  [TestFixture]
  internal class LoadingScreenStateTest {

    #region class TestGameState

    /// <summary>Game state used for unit testing</summary>
    private class TestGameState : GameState, ILoadableGameState {

      #region class SynchronousAsyncResult

      /// <summary>Dummy async result for a synchronously completed process</summary>
      private class SynchronousAsyncResult : IAsyncResult {

        /// <summary>Initializes a new dummy async result</summary>
        /// <param name="state">State that will be provided by the async result</param>
        public SynchronousAsyncResult(object state) {
          this.state = state;
        }

        /// <summary>User defined state passed to the async result</summary>
        public object AsyncState {
          get { return this.state; }
        }

        /// <summary>
        ///   Wait handle that can be used to wait for the asynchronous process
        /// </summary>
        public WaitHandle AsyncWaitHandle {
          get {
            if(this.waitHandle == null) {
              this.waitHandle = new ManualResetEvent(true);
            }
            return this.waitHandle;
          }
        }

        /// <summary>Whether the process has completed synchronously</summary>
        public bool CompletedSynchronously {
          get { return true; }
        }

        /// <summary>True of the asynchronous process has already completed</summary>
        public bool IsCompleted {
          get { return true; }
        }

        /// <summary>State being provided by the async result</summary>
        private object state;
        /// <summary>Event that can be used to wait for the process to complete</summary>
        private ManualResetEvent waitHandle;

      }

      #endregion // class SynchronousAsyncResult

      #region class AsynchronousAsyncResult

      /// <summary>Dummy async result for an asynchronously completed process</summary>
      public class AsynchronousAsyncResult : IAsyncResult {

        /// <summary>Initializes a new dummy async result</summary>
        /// <param name="callback">
        ///   Callback that will be invoked when the asynchronous process completes
        /// </param>
        /// <param name="state">State that will be provided by the async result</param>
        public AsynchronousAsyncResult(AsyncCallback callback, object state) {
          this.callback = callback;
          this.state = state;
        }

        /// <summary>User defined state passed to the async result</summary>
        public object AsyncState {
          get { return this.state; }
        }

        /// <summary>
        ///   Wait handle that can be used to wait for the asynchronous process
        /// </summary>
        public WaitHandle AsyncWaitHandle {
          get {
            if(this.waitHandle == null) {
              SetCompleted(); // To finish in Dispose()

              this.waitHandle = new ManualResetEvent(this.completed);
            }
            return this.waitHandle;
          }
        }

        /// <summary>Whether the process has completed synchronously</summary>
        public bool CompletedSynchronously {
          get { return false; }
        }

        /// <summary>True of the asynchronous process has already completed</summary>
        public bool IsCompleted {
          get { return this.completed; }
        }

        /// <summary>Moves the game state into the completed state</summary>
        public void SetCompleted() {
          if(!this.completed) {
            if(this.waitHandle != null) {
              this.waitHandle.Set();
            }
            this.completed = true;
            this.callback(this);
          }
        }

        /// <summary>State being provided by the async result</summary>
        private object state;
        /// <summary>Event that can be used to wait for the process to complete</summary>
        private ManualResetEvent waitHandle;
        /// <summary>
        ///   Callback that will be invoked when the asynchronous process completes
        /// </summary>
        private AsyncCallback callback;
        /// <summary>Whether the asynchronous process has completed</summary>
        private bool completed;

      }

      #endregion // class SynchronousAsyncResult

      /// <summary>Can be fired when the loading progress has changed</summary>
      public event EventHandler<LoadProgressEventArgs> ProgressChanged;

      /// <summary>Initializes a new test game state</summary>
      /// <param name="manager">
      ///   Game state manager the test game state belongs to
      /// </param>
      public TestGameState(GameStateManager manager) :
        base(manager) { }

      /// <summary>Begins loading the game state</summary>
      /// <param name="callback">
      ///   Callback to be called when the game state has been loaded
      /// </param>
      /// <param name="state">User defined object to pass on to the callback</param>
      /// <returns>A result handle that can be used to wait for the loading process</returns>
      public IAsyncResult BeginLoad(AsyncCallback callback, object state) {
        if(this.Asynchronous) {
          this.AsyncResult = new AsynchronousAsyncResult(callback, state);
          return this.AsyncResult;
        } else {
          this.AsyncResult = new SynchronousAsyncResult(state);
          callback(this.AsyncResult);
          return this.AsyncResult;
        }
      }

      /// <summary>Waits for the loading operation to finish</summary>
      /// <param name="asyncResult">Pending operation to wait for</param>
      public void EndLoad(IAsyncResult asyncResult) { }

      /// <summary>Gives the game component a chance to initialize itself</summary>
      public override void Initialize() {
        base.Initialize();

        ++this.InitializeCallCount;
      }
      
      /// <summary>Changes the reported loading progress</summary>
      /// <param name="progress">New progress to report in a range from 0.0 to 1.0</param>
      public void SetProgress(float progress) {
        EventHandler<LoadProgressEventArgs> copy = ProgressChanged;
        if(copy != null) {
          copy(this, new LoadProgressEventArgs(progress));
        }
      }

      /// <summary>Number of times the Initialize() method has been called</summary>
      public int InitializeCallCount;
      /// <summary>Whether the loading process should complete asynchronously</summary>
      public bool Asynchronous;
      /// <summary>Asynchronous result that is tracking the loading progress</summary>
      public IAsyncResult AsyncResult;

    }

    #endregion // class TestGameState

    #region class TestLoadingScreenState

    /// <summary>Loading screen game state used for testing</summary>
    private class TestLoadingScreenState : LoadingScreenState<TestGameState> {

      /// <summary>Initializes a new loading screen game state</summary>
      /// <param name="gameStateManager">
      ///   Game state manager the loading screen state belongs to
      /// </param>
      /// <param name="gameStateToLoad">
      ///   Game state that will be loaded by the loading screen
      /// </param>
      public TestLoadingScreenState(
        GameStateManager gameStateManager, TestGameState gameStateToLoad
      ) :
        base(gameStateManager, gameStateToLoad) { }

      /// <summary>Game state being loaded by the loading screen</summary>
      public new TestGameState GameStateToLoad {
        get { return base.GameStateToLoad; }
      }

      /// <summary>Called when the load progress has changed</summary>
      /// <param name="progress">New load progress</param>
      protected override void OnLoadProgressChanged(float progress) {
        this.LoadProgress = progress;
        base.OnLoadProgressChanged(progress);
      }
      
      /// <summary>Most recently received loading progress</summary>
      public float LoadProgress;

    }

    #endregion // class TestLoadingScreenState

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();

      GameServiceContainer services = new GameServiceContainer();
      services.AddService(
        typeof(IGraphicsDeviceService), this.mockedGraphicsDeviceService
      );

      this.stateManager = new GameStateManager(services);
      this.testState = new TestGameState(this.stateManager);
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.testState != null) {
        this.testState.Dispose();
        this.testState = null;
      }

      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>Verifies that the constructor is working</summary>
    [Test]
    public void TestConstructor() {
      LoadingScreenState<TestGameState> loadingScreen = new LoadingScreenState<TestGameState>(
        this.stateManager, this.testState
      );
    }

    /// <summary>
    ///   Ensures that the Initialize() call is passed on to the game state being loaded
    /// </summary>
    [Test]
    public void TestInitialize() {
      LoadingScreenState<TestGameState> loadingScreen =
        new LoadingScreenState<TestGameState>(this.stateManager, this.testState);

      Assert.AreEqual(0, testState.InitializeCallCount);
      loadingScreen.Initialize();
      Assert.AreEqual(1, testState.InitializeCallCount);
    }

    /// <summary>
    ///   Tests whether the loading screen can be disposed before loading has begun
    /// </summary>
    [Test]
    public void TestDisposeUnloaded() {
      using(
        LoadingScreenState<TestGameState> loadingScreen =
          new LoadingScreenState<TestGameState>(this.stateManager, this.testState)
      ) {
        loadingScreen.Initialize();
      }
    }

    /// <summary>
    ///   Tests whether the loading screen is able to begin the background loading process
    /// </summary>
    [Test]
    public void TestStartLoadingSynchronously() {
      using(
        LoadingScreenState<TestGameState> loadingScreen =
          new LoadingScreenState<TestGameState>(this.stateManager, this.testState)
      ) {
        loadingScreen.Initialize();
        loadingScreen.StartLoading();
      }
    }

    /// <summary>
    ///   Tests whether the loading screen is able to begin the background loading process
    ///   for an asynchronously loading game state.
    /// </summary>
    [Test]
    public void TestStartLoadingAsynchronously() {
      using(
        LoadingScreenState<TestGameState> loadingScreen =
          new LoadingScreenState<TestGameState>(this.stateManager, this.testState)
      ) {
        loadingScreen.Initialize();
        this.testState.Asynchronous = true;
        loadingScreen.StartLoading();
      }
    }

    /// <summary>
    ///   Tests whether the loading screen can cope with the StartLoading() method
    ///   being called multiple times.
    /// </summary>
    [Test]
    public void TestStartLoadingMultipleTimes() {
      using(
        LoadingScreenState<TestGameState> loadingScreen =
          new LoadingScreenState<TestGameState>(this.stateManager, this.testState)
      ) {
        loadingScreen.Initialize();
        loadingScreen.StartLoading();
        loadingScreen.StartLoading();
        loadingScreen.StartLoading();
      }
    }

    /// <summary>
    ///   Verifies that the GameStateToLoad property is returning the right value
    /// </summary>
    [Test]
    public void TestGameStateToLoad() {
      using(
        TestLoadingScreenState loadingScreen =
          new TestLoadingScreenState(this.stateManager, this.testState)
      ) {
        Assert.AreSame(this.testState, loadingScreen.GameStateToLoad);
      }
    }

    /// <summary>
    ///   Verifies that the GameStateToLoad property is returning the right value
    /// </summary>
    [Test]
    public void TestProgressReporting() {
      using(
        TestLoadingScreenState loadingScreen =
          new TestLoadingScreenState(this.stateManager, this.testState)
      ) {
        loadingScreen.Initialize();

        // Before StartLoading(), progress reports are ignored
        this.testState.SetProgress(0.123f);
        Assert.AreNotEqual(0.123f, loadingScreen.LoadProgress);

        // Begin the asynchronous loading process
        this.testState.Asynchronous = true;
        loadingScreen.StartLoading();

        // This progress report should be accepted
        this.testState.SetProgress(0.1234f);
        Assert.AreEqual(0.1234f, loadingScreen.LoadProgress);

        // Move the game state into the completed state
        TestGameState.AsynchronousAsyncResult asyncResult =
          (this.testState.AsyncResult as TestGameState.AsynchronousAsyncResult);
        asyncResult.SetCompleted();

        // This progress report should not be processed anymore
        this.testState.SetProgress(0.12345f);
        Assert.AreNotEqual(0.12345f, loadingScreen.LoadProgress);
      }
    }

    /// <summary>
    ///   Verifies that the loading screen switched to the actual game state when
    ///   loading has finished
    /// </summary>
    [Test]
    public void TestGameStateSwitching() {
      using(
        LoadingScreenState<TestGameState> loadingScreen =
          new LoadingScreenState<TestGameState>(this.stateManager, this.testState)
      ) {
        loadingScreen.Initialize();

        this.stateManager.Push(loadingScreen);
        Assert.AreSame(loadingScreen, this.stateManager.ActiveState);

        // Synchronous mode - loading finishes right here        
        loadingScreen.StartLoading();

        // The loading screen may decide to do the switch in Update() or Draw() to
        // avoid thread synchronization issues.
        loadingScreen.Update(new GameTime());
        loadingScreen.Draw(new GameTime());

        // By now, the loading screen should have established the game state it was
        // loading as the new active state
        Assert.AreSame(this.testState, this.stateManager.ActiveState);
      }
    }

    /// <summary>Game state used to test the loading screen state</summary>
    private TestGameState testState;
    /// <summary>Mock of the graphics device service</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;

    /// <summary>State manager used for testing the loading screen state</summary>
    private GameStateManager stateManager;

  }

} // namespace Nuclex.Game.States

#endif // UNITTEST
