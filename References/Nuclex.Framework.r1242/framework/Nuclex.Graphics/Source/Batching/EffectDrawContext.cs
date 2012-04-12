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
  ///   Uses a custom XNA effect for the primitives drawn by the PrimitiveBatch class
  /// </summary>
  public class EffectDrawContext : DrawContext {

    /// <summary>Initializes a new effect draw context</summary>
    /// <param name="effect">
    ///   Effect that will be used for the primitives rendered with this context
    /// </param>
    public EffectDrawContext(Effect effect) {
      this.effect = effect;
    }

    /// <summary>Number of passes this draw context requires for rendering</summary>
    public override int Passes {
      get { return this.effect.CurrentTechnique.Passes.Count; }
    }

#if !XNA_4

    /// <summary>Begins the drawing cycle</summary>
    public override void Begin() {
      this.effect.Begin(SaveStateMode.SaveState);
    }

    /// <summary>Ends the drawing cycle</summary>
    public override void End() {
      this.effect.End();
    }

    /// <summary>Prepares the graphics device for drawing</summary>
    /// <param name="pass">Index of the pass to begin rendering</param>
    public override void BeginPass(int pass) {
      this.effect.CurrentTechnique.Passes[pass].Begin();
      this.currentPass = pass;
    }

    /// <summary>Restore the graphics device after drawing has finished</summary>
    public override void EndPass() {
      this.effect.CurrentTechnique.Passes[this.currentPass].End();
    }

#else

    /// <summary>Prepares the graphics device for drawing</summary>
    /// <param name="pass">Index of the pass to begin rendering</param>
    public override void Apply(int pass) {
      this.effect.CurrentTechnique.Passes[pass].Apply();
    }

#endif

    /// <summary>The basic effect being managed by the draw context</summary>
    /// <remarks>
    ///   Warning: If you change the settings of this effect after you've already
    ///   queued other primitives to be drawn, those primitives might be affected
    ///   nontheless if they haven't been rendered yet. The recommended usage is to
    ///   initialize an effect once for each set of settings you need and then keep
    ///   using those instances without modifying them.
    /// </remarks>
    public Effect Effect {
      get { return this.effect; }
    }

    /// <summary>Tests whether another draw context is identical to this one</summary>
    /// <param name="otherContext">Other context to check for equality</param>
    /// <returns>True if the other context is identical to this one</returns>
    /// <remarks>
    ///   Classes deriving from the EffectDrawContext should override this method
    ///   and do their own comparison - for example, two drawing contexts might
    ///   use the same effect instance, but apply different effect parameters before
    ///   rendering - in that case, an additional comparison of the draw context's
    ///   own settings needs to be performed here.
    /// </remarks>
    public override bool Equals(DrawContext otherContext) {
      EffectDrawContext other = otherContext as EffectDrawContext;
      if(other == null)
        return false;

      Effect thisEffect = this.effect;
      Effect otherEffect = other.effect;

      // If the same effect instance is behind the other class, we can be sure that
      // the effects are identical. Derived clases should override this method,
      // otherwise, instance using the same effect but with different effect parameters
      // would be compared as equal in this line. If on the other hand, the effect
      // draw context is used directly, this comparison is what we want!
      if(ReferenceEquals(thisEffect, otherEffect))
        return true;

      // Short cut didn't work, compare the effects member by member
      return CompareEffectParameters(otherEffect);
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
    protected virtual bool CompareEffectParameters(Effect otherEffect) {
      return false;
    }

    /// <summary>The draw context's effect used for rendering</summary>
    protected Effect effect;

#if !XNA_4
    /// <summary>Pass being currently rendered</summary>
    private int currentPass;
#endif

  }

} // namespace Nuclex.Graphics.Batching
