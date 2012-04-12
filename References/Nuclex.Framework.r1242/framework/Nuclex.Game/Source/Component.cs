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
using ComponentEventHandler = System.EventHandler<System.EventArgs>;
#else
using ComponentEventHandler = System.EventHandler;
#endif

namespace Nuclex.Game {

  /// <summary>
  ///   Variant of the XNA GameComponent that doesn't reference the Game class
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This is a lightweight version of GameComponent that can be used without
  ///     requiring a Game class to be present. Useful to get all the advantages
  ///     of the XNA GameServices architecture even when you have initialized and
  ///     manage the graphics device yourself.
  ///   </para>
  ///   <para>
  ///     The name of this class is the same as 'GameComponent' minus the 'Game' part
  ///     as the Game reference is what this class removes from its namesake.
  ///   </para>
  /// </remarks>
  public class Component : IGameComponent, IUpdateable {

    /// <summary>Triggered when the value of the enabled property is changed.</summary>
    public event ComponentEventHandler EnabledChanged;

    /// <summary>Triggered when the value of the update order property is changed.</summary>
    public event ComponentEventHandler UpdateOrderChanged;

    /// <summary>Initializes a new component</summary>
    public Component() {
      this.enabled = true;
    }

    /// <summary>Gives the game component a chance to initialize itself</summary>
    public virtual void Initialize() { }

    /// <summary>Called when the component needs to update its state.</summary>
    /// <param name="gameTime">Provides a snapshot of the Game's timing values</param>
    public virtual void Update(GameTime gameTime) { }

    /// <summary>
    ///   Indicates when the updateable component should be updated in relation to
    ///   other updateables. Has no effect by itself.
    /// </summary>
    public int UpdateOrder {
      get { return this.updateOrder; }
      set {
        if(value != this.updateOrder) {
          this.updateOrder = value;
          OnUpdateOrderChanged();
        }
      }
    }

    /// <summary>
    ///   True when the updateable component is enabled and should be udpated.
    /// </summary>
    public bool Enabled {
      get {
        return this.enabled;
      }
      set {
        if(value != this.enabled) {
          this.enabled = value;
          OnEnabledChanged();
        }
      }
    }

    /// <summary>Fires the UpdateOrderChanged event</summary>
    protected virtual void OnUpdateOrderChanged() {
      if(this.UpdateOrderChanged != null) {
        this.UpdateOrderChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>Fires the EnabledChanged event</summary>
    protected virtual void OnEnabledChanged() {
      if(this.EnabledChanged != null) {
        this.EnabledChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>
    ///   Used to determine the updating order of this object in relation to other
    ///   objects in the same list.
    /// </summary>
    private int updateOrder;
    /// <summary>Whether this object is enabled (and should thus be updated)</summary>
    private bool enabled;

  }

} // namespace Nuclex.Game
