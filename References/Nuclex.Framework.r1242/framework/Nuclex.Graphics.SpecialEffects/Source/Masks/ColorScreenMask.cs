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

#if !WINDOWS_PHONE

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Nuclex.Graphics.SpecialEffects.Masks {

  /// <summary>Screen mask that fills the screen with a solid color</summary>
  public class ColorScreenMask : ScreenMask<PositionVertex> {

    /// <summary>Delegate for a factory method that creates this screen mask</summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the screen mask is rendered with
    /// </param>
    /// <param name="contentManager">
    ///   Content manager the effect was loaded from
    /// </param>
    /// <param name="effect">Effect that will be used to render the screen mask </param>
    /// <returns>A new instance of the solid color screen mask</returns>
    internal delegate ColorScreenMask CreateDelegate(
      GraphicsDevice graphicsDevice, ContentManager contentManager, Effect effect
    );

    /// <summary>Initializes as new solid color screen mask</summary>
    /// <param name="graphicsDevice">Graphics device the skybox cube lives on</param>
    /// <param name="contentManager">Content manager the effect belongs to</param>
    /// <param name="effect">Effect that will be used to draw the screen mask</param>
    protected ColorScreenMask(
      GraphicsDevice graphicsDevice, ContentManager contentManager, Effect effect
    ) :
      base(graphicsDevice, effect, vertices) {

      this.contentManager = contentManager;
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public override void Dispose() {
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.contentManager = null;
      }

      base.Dispose();
    }

    /// <summary>Color the mask fills the screen with</summary>
    public Color Color {
      get { return new Color(this.Effect.Parameters[0].GetValueVector4()); }
      set { this.Effect.Parameters[0].SetValue(value.ToVector4()); }
    }

    /// <summary>Creates a new solid color screen mask</summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the screen mask will be draw with
    /// </param>
    /// <returns>The newly created solid color screen mask</returns>
    public static ColorScreenMask Create(GraphicsDevice graphicsDevice) {
      return Create(graphicsDevice, CreateDefault);
    }

    /// <summary>Creates a new solid color screen mask</summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the screen mask will be draw with
    /// </param>
    /// <param name="createDelegate">
    ///   Factory method that will be used to instantiate the mask
    /// </param>
    /// <returns>The newly created solid color screen mask</returns>
    internal static ColorScreenMask Create(
      GraphicsDevice graphicsDevice, CreateDelegate createDelegate
    ) {

      // Fake up a service provider with a graphics device service so we can
      // create a content manager without the huge rat-tail of references
      IServiceProvider serviceProvider;
      serviceProvider = GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
        GraphicsDeviceServiceHelper.MakeDummyGraphicsDeviceService(graphicsDevice)
      );

      // Create a resource content manager to load the default effect and hand
      // everything to the new screen mask instance, which will then be responsible
      // for freeing those resources again.
      ResourceContentManager contentManager = new ResourceContentManager(
        serviceProvider, Resources.ScreenMaskResources.ResourceManager
      );
      try {
        Effect effect = contentManager.Load<Effect>("ScreenMaskEffect");
        try {
          return createDelegate(graphicsDevice, contentManager, effect);
        }
        catch(Exception) {
          effect.Dispose();
          throw;
        }
      }
      catch(Exception) {
        contentManager.Dispose();
        throw;
      }
    }

    /// <summary>Creates a new instance of the solid color screen mask class</summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the screen mask is rendered with
    /// </param>
    /// <param name="contentManager">
    ///   Content manager the effect was loaded from
    /// </param>
    /// <param name="effect">Effect that will be used to render the screen mask </param>
    /// <returns>A new instance of the solid color screen mask</returns>
    private static ColorScreenMask CreateDefault(
      GraphicsDevice graphicsDevice, ContentManager contentManager, Effect effect
    ) {
      return new ColorScreenMask(graphicsDevice, contentManager, effect);
    }

    /// <summary>Vertices used to draw the screen mask</summary>
    private static readonly PositionVertex[] vertices = {
      new PositionVertex(new Vector2(-1.0f, +1.0f)),
      new PositionVertex(new Vector2(-1.0f, -1.0f)),
      new PositionVertex(new Vector2(+1.0f, +1.0f)),
      new PositionVertex(new Vector2(+1.0f, -1.0f))
    };

    /// <summary>Content manager the solid color fill effect was loaded from</summary>
    private ContentManager contentManager;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Masks

#endif // !WINDOWS_PHONE