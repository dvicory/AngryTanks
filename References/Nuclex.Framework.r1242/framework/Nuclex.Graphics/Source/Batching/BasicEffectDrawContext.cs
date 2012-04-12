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

namespace Nuclex.Graphics.Batching {

  /// <summary>
  ///   Sets up a BasicEffect instance for primitives drawn by the PrimitiveBatch class
  /// </summary>
  public class BasicEffectDrawContext : EffectDrawContext, IDisposable {

    /// <summary>Initializes a new BasicEffect draw context</summary>
    /// <param name="graphicsDevice">GraphicsDevice the effect will use</param>
    public BasicEffectDrawContext(GraphicsDevice graphicsDevice) :
      base(createEffect(graphicsDevice)) {

      this.basicEffect = base.effect as BasicEffect;
    }

    /// <summary>Immediately releases all resources owned by the primitive batch</summary>
    public void Dispose() {
      if(this.basicEffect != null) {
        this.basicEffect.Dispose();
        this.basicEffect = null;
      }
    }

    /// <summary>The basic effect being managed by the draw context</summary>
    /// <remarks>
    ///   Warning: If you change the settings of this effect after you've already
    ///   queued other primitives to be drawn, those primitives might be affected
    ///   nontheless if they haven't been rendered yet. The recommended usage is to
    ///   initialize an effect once for each set of settings you need and then keep
    ///   using those instances without modifying them.
    /// </remarks>
    public BasicEffect BasicEffect {
      get { return this.basicEffect; }
    }

    /// <summary>Compares the effect parameters member by member</summary>
    /// <param name="otherEffect">
    ///   Other effect that will be compared against the context's own effect
    /// </param>
    /// <returns>True of all parameters of the other effect are equal</returns>
    /// <remarks>
    ///   Override this to perform a comparison on the relevant parameters of
    ///   your custom effect. By default, this will return false, causing only
    ///   effect drawing contexts with the same effect object to be considered
    ///   for batching.
    /// </remarks>
    protected override bool CompareEffectParameters(Effect otherEffect) {
      BasicEffect otherBasicEffect = otherEffect as BasicEffect;
      if(otherBasicEffect == null)
        return false;

      BasicEffect thisBasicEffect = this.basicEffect;

      bool topLevelPropertiesEqual =
        (otherBasicEffect.Alpha == thisBasicEffect.Alpha) &&
        (otherBasicEffect.AmbientLightColor == thisBasicEffect.AmbientLightColor) &&
        (otherBasicEffect.DiffuseColor == thisBasicEffect.DiffuseColor) &&
        (otherBasicEffect.EmissiveColor == thisBasicEffect.EmissiveColor) &&
        (otherBasicEffect.FogColor == thisBasicEffect.FogColor) &&
        (otherBasicEffect.FogEnabled == thisBasicEffect.FogEnabled) &&
        (otherBasicEffect.FogEnd == thisBasicEffect.FogEnd) &&
        (otherBasicEffect.FogStart == thisBasicEffect.FogStart) &&
        (otherBasicEffect.LightingEnabled == thisBasicEffect.LightingEnabled) &&
        (otherBasicEffect.PreferPerPixelLighting == thisBasicEffect.PreferPerPixelLighting) &&
        (otherBasicEffect.Projection == thisBasicEffect.Projection) &&
        (otherBasicEffect.SpecularColor == thisBasicEffect.SpecularColor) &&
        (otherBasicEffect.SpecularPower == thisBasicEffect.SpecularPower) &&
        (otherBasicEffect.Texture == thisBasicEffect.Texture) &&
        (otherBasicEffect.TextureEnabled == thisBasicEffect.TextureEnabled) &&
        (otherBasicEffect.VertexColorEnabled == thisBasicEffect.VertexColorEnabled) &&
        (otherBasicEffect.View == thisBasicEffect.View) &&
        (otherBasicEffect.World == thisBasicEffect.World);

      return
        topLevelPropertiesEqual &&
        areEqual(otherBasicEffect.DirectionalLight0, thisBasicEffect.DirectionalLight0) &&
        areEqual(otherBasicEffect.DirectionalLight1, thisBasicEffect.DirectionalLight1) &&
        areEqual(otherBasicEffect.DirectionalLight2, thisBasicEffect.DirectionalLight2);
    }

    /// <summary>Compares two directional lights against each other</summary>
    /// <param name="left">Left directional light that will be compared</param>
    /// <param name="right">Reft directional light that will be compared</param>
    /// <returns>True if both directional lights have identical properties</returns>
    private static bool areEqual(
#if XNA_4
      DirectionalLight left, DirectionalLight right
#else
      BasicDirectionalLight left, BasicDirectionalLight right
#endif
    ) {
      return
        (left.DiffuseColor == right.DiffuseColor) &&
        (left.Direction == right.Direction) &&
        (left.Enabled == right.Enabled) &&
        (left.SpecularColor == right.SpecularColor);
    }

    /// <summary>Creates the effect used by the draw context</summary>
    /// <param name="graphicsDevice">GraphicsDevice the effect will use</param>
    private static BasicEffect createEffect(GraphicsDevice graphicsDevice) {
#if XNA_4
      return new BasicEffect(graphicsDevice);
#else
      return new BasicEffect(graphicsDevice, null);
#endif
    }

    /// <summary>The draw context's BasicEffect instance used for rendering</summary>
    private BasicEffect basicEffect;

  }

} // namespace Nuclex.Graphics.Batching
