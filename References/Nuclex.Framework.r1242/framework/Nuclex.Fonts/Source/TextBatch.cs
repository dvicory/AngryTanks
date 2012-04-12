#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2008 Nuclex Development Labs

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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Nuclex.Graphics;
using Nuclex.Graphics.Batching;

namespace Nuclex.Fonts {

  /// <summary>Batches text drawing commands</summary>
  public class TextBatch : IDisposable {

    /// <summary>Initializes a new text batch for rendering</summary>
    /// <param name="graphicsDevice">Graphics device to render to</param>
    public TextBatch(GraphicsDevice graphicsDevice) {
      this.dummyService = GraphicsDeviceServiceHelper.MakeDummyGraphicsDeviceService(
        graphicsDevice
      );
#if WINDOWS_PHONE
      // Windows Phone doesn't expose programmable shaders to XNA
      this.solidColorEffect = new BasicEffect(graphicsDevice);
#else
      this.contentManager = new ResourceContentManager(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(this.dummyService),
        Resources.TextBatchResources.ResourceManager
      );

      // Create the default effect we're going to use for rendering
      this.solidColorEffect = this.contentManager.Load<Effect>("DefaultTextEffect");
#endif
#if !XNA_4

      // Create a new vertex declaration for the internally used primitive batch
      this.vertexDeclaration = new VertexDeclaration(
        graphicsDevice, VertexPositionNormalTexture.VertexElements
      );

#endif

      // Set up our internal primitive batch. We delegate the vertex batching
      // methods to this class and just make it our responsibility to present
      // a clean interface to the user.
      this.primitiveBatch = new PrimitiveBatch<VertexPositionNormalTexture>(
        graphicsDevice
#if !XNA_4
        , this.vertexDeclaration, VertexPositionNormalTexture.SizeInBytes
#endif
      );

      // Set up a view matrix that provides a 1:1 transformation of pixels to
      // world units. Unless the user sets his own ViewProjection matrix, this will
      // allow us to expose similar behavior to the XNA SpriteBatch class.
      this.viewProjection = new Matrix(
        2.0f / (float)graphicsDevice.Viewport.Width, 0.0f, 0.0f, 0.0f,
        0.0f, 2.0f / (float)graphicsDevice.Viewport.Height, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        -1.0f, -1.0f, 0.0f, 1.0f
      );

    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.primitiveBatch != null) {
        this.primitiveBatch.Dispose();
        this.primitiveBatch = null;
      }
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.solidColorEffect = null;
        this.contentManager = null;
      }
      if(this.dummyService != null) {
        IDisposable disposable = this.dummyService as IDisposable;
        if(disposable != null) {
          disposable.Dispose();
        }
        this.dummyService = null;
      }
    }

    /// <summary>Current concatenated view * projection matrix of the scene</summary>
    public Matrix ViewProjection {
      get { return this.viewProjection; }
      set { this.viewProjection = value; }
    }

    /// <summary>Draws a line of text to the screen</summary>
    /// <param name="text">Pregenerated text mesh to draw to the screen</param>
    /// <param name="color">Color and opacity with which to draw the text</param>
    public void DrawText(Text text, Color color) {
      this.primitiveBatch.Draw(
        text.Vertices, text.Indices, text.PrimitiveType,
        new TextDrawContext(this.solidColorEffect, this.viewProjection, color)
      );
    }

    /// <summary>Draws a line of text to the screen</summary>
    /// <param name="text">Pregenerated text mesh to draw to the screen</param>
    /// <param name="color">Color and opacity with which to draw the text</param>
    /// <param name="transform">Transformation matrix to apply to the text</param>
    public void DrawText(Text text, Matrix transform, Color color) {
      this.primitiveBatch.Draw(
        text.Vertices, text.Indices, text.PrimitiveType,
        new TextDrawContext(
          this.solidColorEffect, transform * this.viewProjection, color
        )
      );
    }

    /// <summary>Draws a line of text to the screen</summary>
    /// <param name="text">Pregenerated text mesh to draw to the screen</param>
    /// <param name="effect">Custom effect with which to draw the text</param>
    public void DrawText(Text text, Effect effect) {
      this.primitiveBatch.Draw(
        text.Vertices, text.Indices, text.PrimitiveType,
        new EffectDrawContext(effect)
      );
    }

    /// <summary>Begins a new text rendering batch</summary>
    /// <remarks>
    ///   Call this before drawing text with the DrawText() method. For optimal
    ///   performance, try to put all your text drawing commands inside as few
    ///   Begin()..End() pairs as you can manage.
    /// </remarks>
    public void Begin() {
      this.primitiveBatch.Begin(QueueingStrategy.Deferred);
    }

    /// <summary>Ends the current text rendering batch</summary>
    /// <remarks>
    ///   This method needs to be called each time you call the Begin() method
    ///   after all text drawing commands have taken place.
    /// </remarks>
    public void End() {
      this.primitiveBatch.End();
    }

    /// <summary>Dummy graphics device service used for the content manager</summary>
    private IGraphicsDeviceService dummyService;
    /// <summary>Content manager used to load the text batch's effect file</summary>
    private ResourceContentManager contentManager;
#if !XNA_4
    /// <summary>Vertex declaration for the vertices used by the font characters</summary>
    private VertexDeclaration vertexDeclaration;
#endif
    /// <summary>Primitive batch used to batch text vertices together</summary>
    private PrimitiveBatch<VertexPositionNormalTexture> primitiveBatch;
    /// <summary>Effect used for rendering text in solid color</summary>
    private Effect solidColorEffect;
    /// <summary>Current view * projection matrix to apply to the text</summary>
    private Matrix viewProjection;

  }

} // namespace Nuclex.Fonts
