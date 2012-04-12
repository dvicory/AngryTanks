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

using Nuclex.Graphics;
using Nuclex.Testing.Xna;

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Game {

  /// <summary>Unit test for the drawable component class</summary>
  [TestFixture]
  internal class GraphicsDeviceGraphicsDeviceDrawableComponentTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();

      GameServiceContainer services = new GameServiceContainer();
      services.AddService(
        typeof(IGraphicsDeviceService), this.mockedGraphicsDeviceService
      );
      this.testComponent = new GraphicsDeviceDrawableComponent(services);
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if (this.testComponent != null) {
        this.testComponent.Dispose();
        this.testComponent = null;
      }

      if (this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that the constructor of the drawable component is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      // This should work even without a graphics device service since the services
      // should only be queried in Initialize() to allow for order-free initialization
      GraphicsDeviceDrawableComponent testComponent = new GraphicsDeviceDrawableComponent(
        new GameServiceContainer()
      );
    }

    /// <summary>
    ///   Tests whether the Initialize() method throws an exception if the drawable
    ///   component is initialized without a graphics device service present.
    /// </summary>
    [Test]
    public void TestThrowOnInitializeWithoutGraphicsDeviceService() {
      GraphicsDeviceDrawableComponent testComponent = new GraphicsDeviceDrawableComponent(
        new GameServiceContainer()
      );
      Assert.Throws<InvalidOperationException>(
        delegate() { testComponent.Initialize(); }
      );
    }

    /// <summary>
    ///   Tests whether the Initialize() method is working when it is called before
    ///   the graphics device has been created
    /// </summary>
    [Test]
    public void TestInitializeBeforeGraphicsDeviceCreation() {
      this.testComponent.Initialize();
      Assert.IsNull(testComponent.GraphicsDevice);

      this.mockedGraphicsDeviceService.CreateDevice();

      Assert.AreSame(
        this.mockedGraphicsDeviceService.GraphicsDevice,
        this.testComponent.GraphicsDevice
      );
    }

    /// <summary>
    ///   Tests whether the Initialize() method is working when it is called after
    ///   the graphics device has been created
    /// </summary>
    [Test]
    public void TestInitializeAfterGraphicsDeviceCreation() {
      this.mockedGraphicsDeviceService.CreateDevice();

      this.testComponent.Initialize();

      Assert.AreSame(
        this.mockedGraphicsDeviceService.GraphicsDevice,
        this.testComponent.GraphicsDevice
      );
    }

    /// <summary>
    ///   Tests whether the drawable component survives a graphics device reset
    /// </summary>
    [Test]
    public void TestGraphicsDeviceReset() {
      this.mockedGraphicsDeviceService.CreateDevice();
      this.testComponent.Initialize();
      this.mockedGraphicsDeviceService.ResetDevice();

      // No exception means success
    }

    /// <summary>Component being tested</summary>
    private GraphicsDeviceDrawableComponent testComponent;
    /// <summary>Mock of the graphics device service</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;

  }

} // namespace Nuclex.Game

#endif // UNITTEST
