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

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Graphics.SpecialEffects.Water {

  /// <summary>Simple water surface</summary>
  public class WaterSurface : IDisposable {

    /// <summary>Delegate for a method used to draw the scene</summary>
    /// <param name="gameTime">Snapshot of the game's timing values</param>
    /// <param name="camera">Camera through which the scene is being viewed</param>
    public delegate void SceneDrawDelegate(GameTime gameTime, Camera camera);

    /// <summary>Initializes a new water surface</summary>
    /// <param name="graphicsDevice">Graphics device to use for rendering</param>
    /// <param name="min">Lesser coordinates of the region covered by water</param>
    /// <param name="max">Greater coordinates of the region covered by water</param>
    public WaterSurface(
      GraphicsDevice graphicsDevice, Vector2 min, Vector2 max
    )
      : this(graphicsDevice, min, max, 1, 1) { }

    /// <summary>Initializes a new water surface</summary>
    /// <param name="graphicsDevice">Graphics device to use for rendering</param>
    /// <param name="min">Lesser coordinates of the region covered by water</param>
    /// <param name="max">Greater coordinates of the region covered by water</param>
    /// <param name="segmentsX">Number segments (and texture repeats) on the X axis</param>
    /// <param name="segmentsZ">Number segments (and texture repeats) on the Y axis</param>
    public WaterSurface(
      GraphicsDevice graphicsDevice,
      Vector2 min, Vector2 max,
      int segmentsX, int segmentsZ
    ) {
      this.graphicsDevice = graphicsDevice;

      this.grid = new WaterGrid(graphicsDevice, min, max, segmentsX, segmentsZ);

      this.reflectionCamera = new Camera(Matrix.Identity, Matrix.Identity);

      this.graphicsDevice.DeviceResetting += new DeviceEventHandler(graphicsDeviceResetting);
      this.graphicsDevice.DeviceReset += new DeviceEventHandler(graphicsDeviceReset);

      createRenderTarget();
    }

    /// <summary>
    ///   Called when graphics resources should be released. Override to
    ///   handle component specific graphics resources
    /// </summary>
    public void Dispose() {
      if(this.reflectionRenderTarget != null) {
#if !XNA_4
        this.reflectionTexture = null;
#endif
        this.reflectionRenderTarget.Dispose();
        this.reflectionRenderTarget = null;
      }
      if(this.grid != null) {
        this.grid.Dispose();
        this.grid = null;
      }
    }

    /// <summary>Selects the vertex and index buffer for the water surface</summary>
    public void SelectVertexAndIndexBuffer() {
#if XNA_4
      this.graphicsDevice.SetVertexBuffer(this.grid.VertexBuffer);
#else
      this.graphicsDevice.VertexDeclaration = this.grid.VertexDeclaration;
      this.graphicsDevice.Vertices[0].SetSource(
        this.grid.VertexBuffer, 0, WaterVertex.Stride
      );
#endif
      this.graphicsDevice.Indices = this.grid.IndexBuffer;
    }

    /// <summary>Draws the plane making up the water surface</summary>
    /// <param name="gameTime">Snapshot of the game's timing values</param>
    /// <param name="camera">Camera through which the scene is being viewed</param>
    public void DrawWaterPlane(GameTime gameTime, Camera camera) {
      this.graphicsDevice.DrawIndexedPrimitives(
        this.grid.PrimitiveType, // primitive type to render
        0, // will be added to all vertex indices in the index buffer
        0, // minimum index of the vertices in the vertex buffer used in this call
        this.grid.VertexCount, // number of vertices used in this call
        0, // where in the index buffer to start drawing
        this.grid.PrimitiveCount // number of primitives to draw
      );
    }

    /// <summary>Updates the reflected image on the water surface</summary>
    /// <param name="gameTime">Snapshot of the game's timing values</param>
    /// <param name="camera">Camera through which the scene is being viewed</param>
    /// <param name="reflectedSceneDrawer">
    ///   Delegate that will be called to draw the scene in its reflected state
    /// </param>
    /// <remarks>
    ///   <para>
    ///     When the delegate is called, the scene should be drawn normally using
    ///     the provided game time and camera. The view matrix of the provided camera
    ///     will have been adjusted to draw the scene upside-down and the graphics device
    ///     will be configured to clip off anything that's below the water surface.
    ///   </para>
    ///   <para>
    ///     Some adjustments can be made, like rendering the reflection with reduced
    ///     detail, cheaper effects or even leaving our parts of the scene to improve
    ///     performance since the reflection will not be clearly displayed anyway.
    ///   </para>
    /// </remarks>
    public void UpdateReflection(
      GameTime gameTime, Camera camera, SceneDrawDelegate reflectedSceneDrawer
    ) {

      // Create a matrix that undoes the view and projection transforms. We don't
      // involve any world matrix here because the water plane exists in
      // the world coordinate frame, its world matrix is always the identity matrix.
      Matrix viewProjection = camera.View * camera.Projection;
      Matrix inverseViewProjection = Matrix.Invert(viewProjection);

      // The water plane in world coordinates. We want to clip away everything that's
      // below the water surface (as only things above it should be reflected)
      Vector4 worldWaterPlane = new Vector4(Vector3.Down, 0.0f);

      // The water plane as sent through the inverse view and projection transforms.
      // This is neccessary because the plane will be transformed by those two
      // matrices when it is applied to the scene. Don't ask me why.
      Vector4 projectedWaterPlane = Vector4.Transform(
        worldWaterPlane, Matrix.Transpose(inverseViewProjection)
      );

#if XNA_4
      this.graphicsDevice.SetRenderTarget(this.reflectionRenderTarget);
#else
      this.graphicsDevice.SetRenderTarget(0, this.reflectionRenderTarget);
#endif
      try {

        // Set up a clipping plane that only draws those parts of the scene that
        // are above the water because that's the only thing we want to appear in
        // the reflection on the water surface.
#if !XNA_4 // TODO: Move this to the shader for both XNA 3.1 and XNA 4.0
        this.graphicsDevice.ClipPlanes[0].Plane = new Plane(projectedWaterPlane);
        this.graphicsDevice.ClipPlanes[0].IsEnabled = true;
        try {
#endif
        Matrix reflectionMatrix = Matrix.CreateReflection(new Plane(worldWaterPlane));

        this.reflectionCamera.View = reflectionMatrix * camera.View;
        this.reflectionCamera.Projection = camera.Projection;

        reflectedSceneDrawer(gameTime, this.reflectionCamera);
#if !XNA_4
        }
        finally {
          this.graphicsDevice.ClipPlanes[0].IsEnabled = false;
        }
#endif
      }
      finally {
#if XNA_4
        this.graphicsDevice.SetRenderTarget(null);
#else
        this.graphicsDevice.SetRenderTarget(0, null);
        this.reflectionTexture = reflectionRenderTarget.GetTexture();
#endif
      }

    }

    /// <summary>Texture containing the water reflection</summary>
    public Texture2D ReflectionTexture {
      get {
#if XNA_4
        return this.reflectionRenderTarget;
#else
        return this.reflectionTexture;
#endif
      }
    }

    /// <summary>Camera which views the scene turned upside-down</summary>
    public Camera ReflectionCamera {
      get { return this.reflectionCamera; }
    }

    /// <summary>Sets up the render target for the water surface reflection</summary>
    private void createRenderTarget() {
      int width = Math.Max(graphicsDevice.PresentationParameters.BackBufferWidth, 16);
      int height = Math.Max(graphicsDevice.PresentationParameters.BackBufferHeight, 16);

      this.reflectionRenderTarget = new RenderTarget2D(
        graphicsDevice,
        width, height,
#if XNA_4
        false, // mipMap
#else
        1,
#endif
        graphicsDevice.PresentationParameters.BackBufferFormat,
#if XNA_4
        graphicsDevice.PresentationParameters.DepthStencilFormat,
        1, // MultisampleCount
#endif
        RenderTargetUsage.PlatformContents
      );
    }

    /// <summary>Called when the graphics device has completed a reset</summary>
    /// <param name="sender">Graphics device that has completed a reset</param>
    /// <param name="arguments">Not used</param>
    private void graphicsDeviceReset(object sender, EventArgs arguments) {
      createRenderTarget();
    }

    /// <summary>Called when the graphics device is about to perform a reset</summary>
    /// <param name="sender">Graphics device that is about to perform a reset</param>
    /// <param name="arguments">Not used</param>
    private void graphicsDeviceResetting(object sender, EventArgs arguments) {
      if(this.reflectionRenderTarget != null) {
        this.reflectionRenderTarget.Dispose();
        this.reflectionRenderTarget = null;
      }
    }

    /// <summary>Camera used to draw the water reflection</summary>
    private Camera reflectionCamera;

    /// <summary>GraphicsDevice the water surface is rendered with</summary>
    private GraphicsDevice graphicsDevice;

    /// <summary>Grid containing the vertices of the water surface</summary>
    private WaterGrid grid;

    /// <summary>Render target used for the refraction and reflection textures</summary>
    private RenderTarget2D reflectionRenderTarget;
#if !XNA_4
    /// <summary>Texture containing what's reflected by the water surface</summary>
    private Texture2D reflectionTexture;
#endif
  }

} // namespace Nuclex.SpecialEffects.Water

