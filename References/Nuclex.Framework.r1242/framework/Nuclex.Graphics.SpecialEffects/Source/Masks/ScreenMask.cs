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

namespace Nuclex.Graphics.SpecialEffects.Masks {

  /// <summary>Mask that draws over the entire screen</summary>
  /// <typeparam name="VertexType">Type of vertices used in the mask</typeparam>
  /// <remarks>
  ///   A 'screen mask' draws over the entire screen with a polygon. This can be used
  ///   to provide damage feedback to the player (screen flashes red or, like in some
  ///   FPS games, the screen borders become red when the player's health is low) or
  ///   to generate post-processing effects.
  /// </remarks>
  public class ScreenMask<VertexType> : StaticMesh<VertexType>
    where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {

    /// <summary>Initializes as new skybox cube</summary>
    /// <param name="graphicsDevice">Graphics device the skybox cube lives on</param>
    /// <param name="effect">Effect by which the screen mask will be rendered</param>
    /// <param name="vertices">Vertices that make up the screen mask</param>
    public ScreenMask(
      GraphicsDevice graphicsDevice, Effect effect, VertexType[/*4*/] vertices
    ) :
      base(graphicsDevice, 4) {

      this.Effect = effect;
      this.Vertices = vertices;

      base.VertexBuffer.SetData<VertexType>(vertices);
    }

    /// <summary>Draws the screen mask</summary>
    public void Draw() {
#if !XNA_4
      this.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
#endif
      Select();

#if XNA_4
      EffectTechnique technique = this.Effect.CurrentTechnique;
      for(int pass = 0; pass < technique.Passes.Count; ++pass) {
        technique.Passes[pass].Apply();

        base.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
      }
#else
      this.Effect.Begin(SaveStateMode.SaveState);
      try {
        EffectTechnique technique = this.Effect.CurrentTechnique;
        for(int pass = 0; pass < technique.Passes.Count; ++pass) {
          technique.Passes[pass].Begin();
          try {
            base.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
          }
          finally {
            technique.Passes[pass].End();
          }
        }
      }
      finally {
        this.Effect.End();
      }
#endif
    }

    /// <summary>Effect being used to render the screen mask</summary>
    protected Effect Effect;
    /// <summary>Vertices used to render the screen mask</summary>
    protected VertexType[/*4*/] Vertices;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Masks
