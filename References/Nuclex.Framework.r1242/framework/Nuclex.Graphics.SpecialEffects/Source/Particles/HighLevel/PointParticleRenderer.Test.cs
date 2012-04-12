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
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Graphics.Batching;
using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel {

#if !XNA_4 // XNA 4.0 doesn't support point sprites anymore

  /// <summary>Unit tests for the point particle renderer</summary>
  [TestFixture]
  internal class PointParticleRendererTest {

    /// <summary>Executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(DeviceType.Reference);
      this.mockedGraphicsDeviceService.CreateDevice();

      this.contentManager = new ResourceContentManager(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
          this.mockedGraphicsDeviceService
        ),
        Resources.ScreenMaskResources.ResourceManager
      );
      this.effect = this.contentManager.Load<Effect>("ScreenMaskEffect");

      this.vertexDeclaration = new VertexDeclaration(
        this.mockedGraphicsDeviceService.GraphicsDevice, SimpleParticle.VertexElements
      );
    }

    /// <summary>Executed after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.vertexDeclaration != null) {
        this.vertexDeclaration.Dispose();
        this.vertexDeclaration = null;
      }
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.effect = null;
        this.contentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Tests whether the point particle renderer's constructor is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      PointParticleRenderer<SimpleParticle> renderer;
      renderer = new PointParticleRenderer<SimpleParticle>(this.effect);
      Assert.IsNotNull(renderer); // Nonsense; avoids compiler warning
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an instance of the point particle
    ///   renderer is created with null specified for the drawing context.
    /// </summary>
    [Test]
    public void TestThrowOnNullEffectInConstructor() {
      DrawContext nullContext = null;
      PointParticleRenderer<SimpleParticle> renderer;
      Assert.Throws<ArgumentException>(
        delegate() {
          renderer = new PointParticleRenderer<SimpleParticle>(nullContext);
        }
      );
    }

    /// <summary>
    ///   Verifies that the point particle renderer can be used to render particles
    /// </summary>
    [Test]
    public void TestRenderParticles() {
      SimpleParticle[] particles = new SimpleParticle[] {
        new SimpleParticle(Vector3.Zero, Vector3.Zero),
        new SimpleParticle(Vector3.Zero, Vector3.Zero),
        new SimpleParticle(Vector3.Zero, Vector3.Zero),
        new SimpleParticle(Vector3.Zero, Vector3.Zero)
      };

      PrimitiveBatch<SimpleParticle> primitiveBatch;
      using(
        primitiveBatch = new PrimitiveBatch<SimpleParticle>(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          this.vertexDeclaration, SimpleParticle.SizeInBytes
        )
      ) {
        PointParticleRenderer<SimpleParticle> renderer = createRenderer();
        renderer.Render(
          new ArraySegment<SimpleParticle>(particles, 1, 2), primitiveBatch
        );
      }
    }

    /// <summary>
    ///   Creates a new point particle renderer with a private primitive batch
    /// </summary>
    /// <returns>The newly created point particle renderer</returns>
    private PointParticleRenderer<SimpleParticle> createRenderer() {
      return new PointParticleRenderer<SimpleParticle>(this.effect);
    }

    /// <summary>Mocked graphics device service used for the unit test</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>
    ///   Content manager used to load the effect by which the particles are drawn
    /// </summary>
    private ResourceContentManager contentManager;
    /// <summary>Effect used to draw the particles</summary>
    private Effect effect;
    /// <summary>Vertex declaration for the particle vertices</summary>
    private VertexDeclaration vertexDeclaration;

  }

#endif // !XNA_4

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel

#endif // UNITTEST
