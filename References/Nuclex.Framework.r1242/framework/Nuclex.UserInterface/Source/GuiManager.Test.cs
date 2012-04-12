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

#if UNITTEST

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;
using Is = NUnit.Framework.Is;

using Nuclex.Input;
using Nuclex.Testing.Xna;
using Nuclex.UserInterface.Visuals;
using Nuclex.UserInterface.Visuals.Flat;

#if XNA_4
using ComponentEventHandler = System.EventHandler<System.EventArgs>;
#else
using ComponentEventHandler = System.EventHandler;
#endif

namespace Nuclex.UserInterface {

  /// <summary>Unit Test for the GUI manager</summary>
  [TestFixture]
  internal class GuiManagerTest {

    #region class DummyVisualizer

    /// <summary>Dummy GUI visualizer for unit testing</summary>
    private class DummyVisualizer : IGuiVisualizer, IUpdateable {

      /// <summary>Raised when the Enabled property changes</summary>
      public event ComponentEventHandler EnabledChanged { add { } remove { } }

      /// <summary>Raised when the UpdateOrder property changes</summary>
      public event ComponentEventHandler UpdateOrderChanged { add { } remove { } }

      /// <summary>Renders an entire control tree starting at the provided control</summary>
      /// <param name="screen">Screen containing the GUI that will be drawn</param>
      public void Draw(Screen screen) {
        ++this.DrawCallCount;
      }

      /// <summary>
      ///   Whether the game component's Update() method should be called in Game.Update()
      /// </summary>
      bool IUpdateable.Enabled {
        get { throw new NotImplementedException(); }
      }

      /// <summary>Called when the game component should be updated</summary>
      /// <param name="gameTime">Snapshot of the game's timing state</param>
      public void Update(GameTime gameTime) {
        ++this.UpdateCallCount;
      }

      /// <summary>
      ///   When the game component should be updated relative to other game components.
      ///   Lower values are updated first.
      /// </summary>
      int IUpdateable.UpdateOrder {
        get { return 0; }
      }

      /// <summary>Number of times the Draw() method has been called</summary>
      public int DrawCallCount;
      /// <summary>Number of times the Update() method has been called</summary>
      public int UpdateCallCount;

    }

    #endregion // class DummyVisualizer

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();
      this.mockedGraphicsDeviceService.CreateDevice();
      
      this.mockedInputService = new MockInputManager();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if (this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that an the GUI manager's standard constructor is working
    /// </summary>
    [Test]
    public void TestStandardConstructor() {
      var gameServices = new GameServiceContainer();
      using (GuiManager guiManager = new GuiManager(gameServices)) {
        Assert.IsNotNull(gameServices.GetService(typeof(IGuiService)));
      }
    }

    /// <summary>
    ///   Verifies that an the GUI manager's explicit constructor is working
    /// </summary>
    [Test]
    public void TestExplicitConstructor() {
      using (
        GuiManager guiManager = new GuiManager(
          this.mockedGraphicsDeviceService, this.mockedInputService
        )
      ) {
        Assert.IsNotNull(guiManager); // nonsense, avoids compiler warning
      }
    }

    /// <summary>
    ///   Verifies that an the GUI manager's compatibility constructor is working
    /// </summary>
    [Test]
    public void TestCompatibilityConstructor() {
      var gameServices = new GameServiceContainer();
      using (
        GuiManager guiManager = new GuiManager(
          gameServices, this.mockedGraphicsDeviceService, this.mockedInputService
        )
      ) {
        Assert.IsNotNull(gameServices.GetService(typeof(IGuiService)));
      }
    }

    /// <summary>Verifies that the GUI manager can be initialized</summary>
    [Test]
    public void TestInitialization() {
      var gameServices = new GameServiceContainer();
      
      gameServices.AddService(
        typeof(IGraphicsDeviceService), this.mockedGraphicsDeviceService
      );
      gameServices.AddService(
        typeof(IInputService), this.mockedInputService
      );
      
      using (GuiManager guiManager = new GuiManager(gameServices)) {
        (guiManager as IGameComponent).Initialize();
      }
    }

    /// <summary>
    ///   Verifies that the screen can be assigned to the GUI manager before it
    ///   has been initialized.
    /// </summary>
    [Test]
    public void TestAssignScreenBeforeInitialize() {
      var gameServices = new GameServiceContainer();
      gameServices.AddService(
        typeof(IGraphicsDeviceService), this.mockedGraphicsDeviceService
      );
      gameServices.AddService(
        typeof(IInputService), this.mockedInputService
      );

      using (GuiManager guiManager = new GuiManager(gameServices)) {
        Screen screen = new Screen();

        guiManager.Screen = screen;
        (guiManager as IGameComponent).Initialize();

        Assert.AreSame(screen, guiManager.Screen);
      }
    }

    /// <summary>
    ///   Verifies that the screen can be assigned to the GUI manager after it
    ///   has been initialized.
    /// </summary>
    [Test]
    public void TestAssignScreenAfterInitialize() {
      var gameServices = new GameServiceContainer();
      gameServices.AddService(
        typeof(IGraphicsDeviceService), this.mockedGraphicsDeviceService
      );
      gameServices.AddService(
        typeof(IInputService), this.mockedInputService
      );

      using (GuiManager guiManager = new GuiManager(gameServices)) {
        Screen screen = new Screen();

        (guiManager as IGameComponent).Initialize();
        guiManager.Screen = screen;

        Assert.AreSame(screen, guiManager.Screen);
      }
    }

    /// <summary>
    ///   Verifies that the Update() method is forwarded to the visualizer to allow
    ///   for animated GUIs
    /// </summary>
    [Test]
    public void TestVisualizerUpdate() {
      using (GuiManager guiManager = new GuiManager(new GameServiceContainer())) {
        DummyVisualizer dummy = new DummyVisualizer();

        IGuiVisualizer visualizer = guiManager.Visualizer;
        guiManager.Visualizer = dummy;
        tryDispose(visualizer);

        Assert.AreEqual(0, dummy.UpdateCallCount);
        guiManager.Update(new GameTime());
        Assert.AreEqual(1, dummy.UpdateCallCount);
      }
    }

    /// <summary>
    ///   Verifies that the Draw() method works even when no screen is assigned
    /// </summary>
    [Test]
    public void TestDrawingWithoutScreen() {
      using (GuiManager guiManager = new GuiManager(new GameServiceContainer())) {
        guiManager.Draw(new GameTime());
        // No exception means success
      }
    }

    /// <summary>
    ///   Verifies that the Draw() method is forwarded to the visualizer
    /// </summary>
    [Test]
    public void TestDrawing() {
      using (GuiManager guiManager = new GuiManager(new GameServiceContainer())) {
        DummyVisualizer dummy = new DummyVisualizer();

        IGuiVisualizer visualizer = guiManager.Visualizer;
        guiManager.Visualizer = dummy;
        tryDispose(visualizer);

        guiManager.Screen = new Screen();

        Assert.AreEqual(0, dummy.DrawCallCount);
        guiManager.Draw(new GameTime());
        Assert.AreEqual(1, dummy.DrawCallCount);
      }
    }

    /// <summary>
    ///   Verifies that the GUI manager's visualizer can be retrieved and replaced
    /// </summary>
    [Test]
    public void TestVisualizerAssignment() {
      using (GuiManager guiManager = new GuiManager(new GameServiceContainer())) {
        IGuiVisualizer visualizer = new DummyVisualizer();
        try {
          IGuiVisualizer newVisualizer = visualizer;

          Assert.AreNotSame(newVisualizer, guiManager.Visualizer);
          {
            IGuiVisualizer oldVisualizer = guiManager.Visualizer;
            guiManager.Visualizer = newVisualizer;
            visualizer = oldVisualizer;
          }
          Assert.AreSame(newVisualizer, guiManager.Visualizer);
        }
        finally {
          tryDispose(visualizer);
        }
      }
    }

    /// <summary>Tries to dispose an object that might implement IDisposable</summary>
    /// <typeparam name="PotentiallyDisposableType">
    ///   Type which might implement IDisposable
    /// </typeparam>
    /// <param name="instance">
    ///   Instance that will be disposed if it implement IDisposable
    /// </param>
    /// <returns>True if the instance implemented IDisposable and was disposed</returns>
    private static bool tryDispose<PotentiallyDisposableType>(
      PotentiallyDisposableType instance
    ) {
      IDisposable disposable = instance as IDisposable;
      if (disposable != null) {
        disposable.Dispose();
        return true;
      } else {
        return false;
      }
    }

    /// <summary>Mocked graphics device service used in the tests</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Mocked input manager used in the tests</summary>
    private MockInputManager mockedInputService;

  }

} // namespace Nuclex.UserInterface

#endif // UNITTEST
