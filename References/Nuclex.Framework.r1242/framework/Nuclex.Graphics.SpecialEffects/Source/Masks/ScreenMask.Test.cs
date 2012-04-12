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
using Microsoft.Xna.Framework.Content;

using NUnit.Framework;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects.Masks {

  /// <summary>Unit tests for the screen mask class</summary>
  [TestFixture]
  internal class ScreenMaskTest {

    /// <summary>Executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(DeviceType.Reference);
      this.mockedGraphicsDeviceService.CreateDevice();

      serviceProvider = GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
        this.mockedGraphicsDeviceService
      );

      this.contentManager = new ResourceContentManager(
        serviceProvider, Resources.ScreenMaskResources.ResourceManager
      );
    }

    /// <summary>Executed after each test has completed</summary>
    [TearDown]
    public void Teardown() {
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.contentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.serviceProvider = null;
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that the constructor of the screen mask class is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      Effect effect = this.contentManager.Load<Effect>("ScreenMaskEffect");
      using(
        ScreenMask<PositionVertex> testMask = new ScreenMask<PositionVertex>(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          effect,
          new PositionVertex[4]
        )
      ) { }
    }

    /// <summary>Tests whether the screen mask is able to draw itself</summary>
    [Test]
    public void TestDraw() {
      Effect effect = this.contentManager.Load<Effect>("ScreenMaskEffect");
      using(
        ScreenMask<PositionVertex> testMask = new ScreenMask<PositionVertex>(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          effect,
          new PositionVertex[4]
        )
      ) {
        testMask.Draw();
      }
    }

    /// <summary>Mocked graphics device service used to run the test</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Service provider containing the mocked graphics device service</summary>
    private IServiceProvider serviceProvider;
    /// <summary>Content manager used to load the assets used during testing</summary>
    private ResourceContentManager contentManager;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Masks

#endif // UNITTEST
