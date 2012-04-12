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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.Graphics.Batching;

namespace Nuclex.Fonts {

  /// <summary>Drawing context for drawing text with vector fonts</summary>
  internal class TextDrawContext : DrawContext {

    /// <summary>Initializes a new text draw context</summary>
    /// <param name="effect">Effect that will be used to render the text</param>
    /// <param name="transform">Transformation matrix for the text</param>
    /// <param name="textColor">Drawing color of the text</param>
    public TextDrawContext(Effect effect, Matrix transform, Color textColor) {
      this.effect = effect;
      this.transform = transform;
      this.textColor = textColor;
    }

    /// <summary>Number of passes this draw context requires for rendering</summary>
    public override int Passes {
      get { return this.effect.CurrentTechnique.Passes.Count; }
    }

#if XNA_4

    /// <summary>Prepares the graphics device for drawing</summary>
    /// <param name="pass">Index of the pass to begin rendering</param>
    public override void Apply(int pass) {

#if WINDOWS_PHONE
      BasicEffect basicEffect = this.effect as BasicEffect;
      //basicEffect.World = Matrix.Identity;
      //basicEffect.View = Matrix.Identity;
      basicEffect.Projection = this.transform;
      basicEffect.Alpha = (float)this.textColor.A / 255.0f;
      basicEffect.DiffuseColor = this.textColor.ToVector3();
#else
      this.effect.Parameters["ViewProjection"].SetValue(this.transform);
      this.effect.Parameters["TextColor"].SetValue(this.textColor.ToVector4());
#endif
      this.effect.CurrentTechnique.Passes[pass].Apply();
    }

#else

    /// <summary>Begins the drawing cycle</summary>
    public override void Begin() {
      this.effect.Parameters["ViewProjection"].SetValue(this.transform);
      this.effect.Parameters["TextColor"].SetValue(this.textColor.ToVector4());
      this.effect.Begin(SaveStateMode.SaveState);
    }

    /// <summary>Ends the drawing cycle</summary>
    public override void End() {
      this.effect.End();
    }

    /// <summary>Prepares the graphics device for drawing</summary>
    /// <param name="pass">Index of the pass to begin rendering</param>
    /// <remarks>
    ///   Should only be called between the normal Begin() and End() methods.
    /// </remarks>
    public override void BeginPass(int pass) {
      this.effect.CurrentTechnique.Passes[pass].Begin();
      this.currentPass = pass;
    }

    /// <summary>Restores the graphics device after drawing has finished</summary>
    /// <remarks>
    ///   Should only be called between the normal Begin() and End() methods.
    /// </remarks>
    public override void EndPass() {
      this.effect.CurrentTechnique.Passes[this.currentPass].End();
    }

#endif

    /// <summary>Tests whether another draw context is identical to this one</summary>
    /// <param name="otherContext">Other context to check for equality</param>
    /// <returns>True if the other context is identical to this one</returns>
    public override bool Equals(DrawContext otherContext) {
      TextDrawContext other = otherContext as TextDrawContext;
      if(other == null)
        return false;

      Effect thisEffect = this.effect;
      Effect otherEffect = other.effect;

      // If the same effect instances are different, we stop comparing right here.
      // This context is specialized to run the same effect in multiple configurations,
      // different effects with identical settings will not happen due to its usage.
      if(!ReferenceEquals(thisEffect, otherEffect))
        return false;

      // It's the same effect instance, so compare the configuration we're assigning
      // to the effect before each drawing cycle
      return
        (this.textColor == other.textColor) &&
        (this.transform == other.transform);
    }

    /// <summary>The draw context's effect used for rendering</summary>
    private Effect effect;
#if !XNA_4
    /// <summary>Pass being currently rendered</summary>
    private int currentPass;
#endif
    /// <summary>Transformation matrix controlling the text's placement</summary>
    private Matrix transform;
    /// <summary>Drawing color of the text</summary>
    private Color textColor;

  }

} // namespace Nuclex.Fonts
