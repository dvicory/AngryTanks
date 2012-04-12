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

using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics.Batching {

  /// <summary>
  ///   Controls the graphics device settings during the rendering process
  /// </summary>
  public abstract class DrawContext {

    /// <summary>Number of passes this draw context requires for rendering</summary>
    public abstract int Passes { get; }

#if !XNA_4

    /// <summary>Begins the drawing cycle</summary>
    public abstract void Begin();

    /// <summary>Ends the drawing cycle</summary>
    public abstract void End();


    /// <summary>Prepares the graphics device for drawing</summary>
    /// <param name="pass">Index of the pass to begin rendering</param>
    /// <remarks>
    ///   Should only be called between the normal Begin() and End() methods.
    /// </remarks>
    public abstract void BeginPass(int pass);

    /// <summary>Restores the graphics device after drawing has finished</summary>
    /// <remarks>
    ///   Should only be called between the normal Begin() and End() methods.
    /// </remarks>
    public abstract void EndPass();

#else

    /// <summary>Prepares the graphics device for drawing</summary>
    /// <param name="pass">Index of the pass to begin rendering</param>
    public abstract void Apply(int pass);

#endif

    /// <summary>Tests whether another draw context is identical to this one</summary>
    /// <param name="otherContext">Other context to check for equality</param>
    /// <returns>True if the other context is identical to this one</returns>
    public abstract bool Equals(DrawContext otherContext);

    /// <summary>
    ///   Tests whether another object is a draw context with identical settings
    /// </summary>
    /// <param name="other">Object to check for equality</param>
    /// <returns>
    ///   True if the other context is a draw context identical to this one
    /// </returns>
    public override bool Equals(object other) {
      DrawContext drawContext = other as DrawContext;

      if(drawContext != null)
        return Equals(drawContext);
      else
        return false;
    }

    /// <summary>
    ///   Returns a hashcode that is not guaranteed to be unique but will be equal for
    ///   all instances of the class that are in an identical state
    /// </summary>
    /// <returns>The hashcode of the object</returns>
    public override int GetHashCode() {
      return base.GetHashCode();
    }

  }

} // namespace Nuclex.Graphics.Batching
