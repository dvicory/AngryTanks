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

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Game {

  /// <summary>
  ///   Lightweight variant DrawableGameComponent that doesn't reference the Game class
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This is a lightweight version of DrawableGameComponent that can be used
  ///     without requiring a Game class to be present. Useful to get all the
  ///     advantages of the XNA GameServices architecture even when you have
  ///     initialized and manage the graphics device yourself.
  ///   </para>
  ///   <para>
  ///     The name of this class is the same as 'DrawableGameComponent' minus the
  ///     'Game' part as the Game reference is what this class removes from its namesake.
  ///   </para>
  /// </remarks>
  public class DrawableComponent : Component, IDrawable {

    /// <summary>Triggered when the value of the draw order property is changed.</summary>
    public event DeviceEventHandler DrawOrderChanged;

    /// <summary>Triggered when the value of the visible property is changed.</summary>
    public event DeviceEventHandler VisibleChanged;

    /// <summary>Initializes a new drawable component.</summary>
    public DrawableComponent() {
      this.visible = true;
    }

    /// <summary>Called when the drawable component needs to draw itself</summary>
    /// <param name="gameTime">Provides a snapshot of the game's timing values</param>
    public virtual void Draw(GameTime gameTime) { }

    /// <summary>
    ///   Indicates when the drawable component should be drawn in relation to other
    ///   drawables. Has no effect by itself.
    /// </summary>
    public int DrawOrder {
      get { return this.drawOrder; }
      set {
        if (value != this.drawOrder) {
          this.drawOrder = value;
          OnDrawOrderChanged();
        }
      }
    }

    /// <summary>True when the drawable component is visible and should be drawn.</summary>
    public bool Visible {
      get { return this.visible; }
      set {
        if (value != this.visible) {
          this.visible = value;
          OnVisibleChanged();
        }
      }
    }

    /// <summary>Fires the DrawOrderChanged event</summary>
    protected virtual void OnDrawOrderChanged() {
      if (this.DrawOrderChanged != null) {
        this.DrawOrderChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>Fires the VisibleChanged event</summary>
    protected virtual void OnVisibleChanged() {
      if (this.VisibleChanged != null) {
        this.VisibleChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>
    ///   Used to determine the drawing order of this object in relation to other
    ///   objects in the same list.
    /// </summary>
    private int drawOrder;
    /// <summary>Whether this object is visible (and should thus be drawn)</summary>
    private bool visible;

  }

} // namespace Nuclex.Graphics
