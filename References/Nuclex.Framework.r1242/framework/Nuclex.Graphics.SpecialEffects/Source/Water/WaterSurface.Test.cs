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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects.Water {

  /// <summary>Unit tests for the grid class</summary>
  [TestFixture]
  internal class WaterSurfaceTest {

    /// <summary>Executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(DeviceType.Reference);
      this.mockedGraphicsDeviceService.CreateDevice();

      MockedGraphicsDeviceService mockedService = this.mockedGraphicsDeviceService;
      this.contentManager = new ResourceContentManager(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(mockedService),
        Resources.UnitTestResources.ResourceManager
      );
    }

    /// <summary>Executed after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.contentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that the simple constructor of the Grid class is working
    /// </summary>
    [Test]
    public void TestSimpleConstructor() {
      using(
        WaterSurface surface = new WaterSurface(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f)
        )
      ) {
        Assert.IsNotNull(surface); // Nonsense; avoids compiler warning
      }
    }

    /// <summary>
    ///   Verifies that the complete constructor of the Grid class is working
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      using(
        WaterSurface surface = new WaterSurface(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
          4, 4
        )
      ) {
        Assert.IsNotNull(surface); // Nonsense; avoids compiler warning
      }
    }

    /// <summary>
    ///   Tests whether the water surface can select its index and vertex buffers
    /// </summary>
    [Test]
    public void TestSelectIndexAndVertexBuffer() {
      using(
        WaterSurface surface = new WaterSurface(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
          4, 4
        )
      ) {
        GraphicsDevice graphicsDevice = this.mockedGraphicsDeviceService.GraphicsDevice;
        graphicsDevice.Indices = null;
#if XNA_4
        graphicsDevice.SetVertexBuffer(null);
#else
        graphicsDevice.Vertices[0].SetSource(
          null, 0, 0
        );
#endif
        Assert.IsNull(graphicsDevice.Indices);
#if XNA_4
        Assert.AreEqual(0, graphicsDevice.GetVertexBuffers().Length);
#else
        Assert.IsNull(graphicsDevice.Vertices[0].VertexBuffer);
#endif
        surface.SelectVertexAndIndexBuffer();

        Assert.IsNotNull(graphicsDevice.Indices);
#if XNA_4
        Assert.IsNotNull(graphicsDevice.GetVertexBuffers()[0].VertexBuffer);
#else
        Assert.IsNotNull(graphicsDevice.Vertices[0].VertexBuffer);
#endif
      }
    }

    /// <summary>
    ///   Tests whether the water surface can draw its water plane
    /// </summary>
    [Test]
    public void TestDrawWaterPlane() {
      Effect waterSurfaceEffect = this.contentManager.Load<Effect>(
        "WaterSurfaceEffect"
      );

      using(
        WaterSurface surface = new WaterSurface(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
          4, 4
        )
      ) {
        surface.SelectVertexAndIndexBuffer();
#if XNA_4
        EffectTechnique technique = waterSurfaceEffect.CurrentTechnique;
        for(int pass = 0; pass < technique.Passes.Count; ++pass) {
          technique.Passes[pass].Apply();

          surface.DrawWaterPlane(new GameTime(), Camera.DefaultOrthographic);
        }
#else
        waterSurfaceEffect.Begin();
        try {
          EffectTechnique technique = waterSurfaceEffect.CurrentTechnique;
          for(int pass = 0; pass < technique.Passes.Count; ++pass) {
            technique.Passes[pass].Begin();
            try {
              surface.DrawWaterPlane(new GameTime(), Camera.DefaultOrthographic);
            }
            finally {
              technique.Passes[pass].End();
            }
          }
        }
        finally {
          waterSurfaceEffect.End();
        }
#endif
      }
    }

    /// <summary>
    ///   Tests whether the water surface can update its reflection texture
    /// </summary>
    [Test]
    public void TestUpdateReflection() {
      using(
        WaterSurface surface = new WaterSurface(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
          4, 4
        )
      ) {
        surface.UpdateReflection(
          new GameTime(), Camera.DefaultOrthographic,
          new WaterSurface.SceneDrawDelegate(drawReflection)
        );
        
        Assert.IsNotNull(surface.ReflectionCamera);
        Assert.IsNotNull(surface.ReflectionTexture);
      }
    }

    /// <summary>
    ///   Verifies that the water surface can survive a graphics device reset
    /// </summary>
    [Test]
    public void TestGraphicsDeviceReset() {
      using(
        WaterSurface surface = new WaterSurface(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
          4, 4
        )
      ) {
        this.mockedGraphicsDeviceService.ResetDevice();

        surface.UpdateReflection(
          new GameTime(), Camera.DefaultOrthographic,
          new WaterSurface.SceneDrawDelegate(drawReflection)
        );

        Assert.IsNotNull(surface.ReflectionCamera);
        Assert.IsNotNull(surface.ReflectionTexture);
      }
    }

    /// <summary>Dummy that's supposed to draw the water's reflection</summary>
    /// <param name="gameTime">Snapshot of the game's timing values</param>
    /// <param name="camera">Camera containing the viewer's position</param>
    private void drawReflection(GameTime gameTime, Camera camera) { }

    /// <summary>
    ///   Mocked graphics device service used for rendering in the unit test
    /// </summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;

    /// <summary>
    ///   Content manager used to load the content for the unit test
    /// </summary>
    private ResourceContentManager contentManager;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Water

#endif // UNITTEST
