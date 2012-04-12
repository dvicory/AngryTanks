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

using Nuclex.Graphics;
using Nuclex.Graphics.Batching;
using Nuclex.Testing.Xna;

namespace Nuclex.Fonts {

  /// <summary>Unit tests for the text drawing context</summary>
  [TestFixture]
  public class TextDrawContextTest {

    /// <summary>
    ///   Verifies that two text contexts which should produce the exact same result
    ///   compare as equal
    /// </summary>
    [Test]
    public void TestIdenticalEffectParameters() {
      Matrix matrix = Matrix.Identity;

      TextDrawContext context1 = new TextDrawContext(this.effect, matrix, Color.White);
      TextDrawContext context2 = new TextDrawContext(this.effect, matrix, Color.White);

      Assert.IsTrue(context1.Equals(context2));
    }

    /// <summary>
    ///   Verifies that two text contexts with different matrices compare as inequal
    /// </summary>
    [Test]
    public void TestDifferentMatrices() {
      Matrix matrix1 = Matrix.Identity;
      Matrix matrix2 = new Matrix(
        1.1f, 1.2f, 1.3f, 1.4f,
        2.1f, 2.2f, 2.3f, 2.4f,
        3.1f, 3.2f, 3.3f, 3.4f,
        4.1f, 4.2f, 4.3f, 4.4f
      );

      TextDrawContext context1 = new TextDrawContext(this.effect, matrix1, Color.White);
      TextDrawContext context2 = new TextDrawContext(this.effect, matrix2, Color.White);

      Assert.IsFalse(context1.Equals(context2));
    }

    /// <summary>
    ///   Verifies that two text contexts with different color compare as inequal
    /// </summary>
    [Test]
    public void TestDifferentColors() {
      Matrix matrix = Matrix.Identity;

      TextDrawContext context1 = new TextDrawContext(this.effect, matrix, Color.Red);
      TextDrawContext context2 = new TextDrawContext(this.effect, matrix, Color.Black);

      Assert.IsFalse(context1.Equals(context2));
    }

    /// <summary>
    ///   Verifies that the text context can be compared against another context of
    ///   a different type
    /// </summary>
    [Test]
    public void TestDifferentEffects() {
      Matrix matrix = Matrix.Identity;
      TextDrawContext context1 = new TextDrawContext(this.effect, matrix, Color.White);
      using(
        BasicEffect effect2 = new BasicEffect(
          this.mockedGraphicsDeviceService.GraphicsDevice
#if !XNA_4
          , null
#endif
        )
      ) {
        TextDrawContext context2 = new TextDrawContext(effect2, matrix, Color.White);
        Assert.IsFalse(context1.Equals(context2));
      }
    }

    /// <summary>
    ///   Verifies that the text context can be compared against another context of
    ///   a different type
    /// </summary>
    [Test]
    public void TestDifferentContexts() {
      Matrix matrix = Matrix.Identity;
      TextDrawContext context1 = new TextDrawContext(this.effect, matrix, Color.White);
      EffectDrawContext context2 = new EffectDrawContext(this.effect);

      Assert.IsFalse(context1.Equals(context2));
    }

    /// <summary>
    ///   Tests the Begin() and End() methods of the draw context without any rendering
    ///   taking place inbetween them
    /// </summary>
    [Test]
    public void TestBeginEnd() {
      Matrix matrix = Matrix.Identity;
#if XNA_4
      TextDrawContext context = new TextDrawContext(this.effect, matrix, Color.Red);
      for(int pass = 0; pass < context.Passes; ++pass) {
        context.Apply(pass);
      }
#else
      TextDrawContext context = new TextDrawContext(this.effect, matrix, Color.Red);
      context.Begin();
      try {
        for(int pass = 0; pass < context.Passes; ++pass) {
          context.BeginPass(pass);
          context.EndPass();
        }
      }
      finally {
        context.End();
      }
#endif
    }

    /// <summary>Initializes a test</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();
      this.mockedGraphicsDeviceService.CreateDevice();

      this.contentManager = new ResourceContentManager(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
          this.mockedGraphicsDeviceService
        ),
        Resources.TextBatchResources.ResourceManager
      );
      this.effect = this.contentManager.Load<Effect>("DefaultTextEffect");
    }

    /// <summary>Finalizes the resources used during the test</summary>
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

    /// <summary>Mocked graphics device service used by the test</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>ContentManager used to load text effect</summary>
    private ResourceContentManager contentManager;
    /// <summary>Effect used for testing the context</summary>
    private Effect effect;

  }

} // namespace Nuclex.Fonts

#endif // UNITTEST
