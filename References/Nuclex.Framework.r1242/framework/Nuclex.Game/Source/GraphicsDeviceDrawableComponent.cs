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
  ///     This variant of the DrawableComponent class stores a graphics device and
  ///     calls the LoadContent() method at the appropriate time. It is useful
  ///     if the deriving class actually makes direct use of the graphics device.
  ///   </para>
  ///   <para>
  ///     To work, this class requires to things: A GameServices collection and
  ///     an entry for the IGraphicsDeviceService. You can easily implement this
  ///     interface yourself for any custom graphics device manager.
  ///   </para>
  /// </remarks>
  public class GraphicsDeviceDrawableComponent : DrawableComponent, IDisposable {

    /// <summary>Initializes a new drawable component.</summary>
    /// <param name="serviceProvider">
    ///   Service provider from which the graphics device service will be taken
    /// </param>
    public GraphicsDeviceDrawableComponent(IServiceProvider serviceProvider) {
      this.serviceProvider = serviceProvider;

      // We do not look up the graphics device service right here because it might
      // not exist yet. XNA uses a two-stage initialization to avoid initialization
      // order dependencies. When constructed, all components add their own services
      // and only when Initialize() is called do they look up other services they need.
    }

    /// <summary>Initializes a new drawable component</summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service the component will use
    /// </param>
    /// <remarks>
    ///   This constructor is mainly relevant for users of IoC containers which
    ///   can wire up components to each other automatically. For the XNA
    ///   game services model, the service provider-based constructor should
    ///   be used instead because it uses a two-stage initialization process
    ///   where components wire up to each other in <see cref=" Initialize" />.
    /// </remarks>
    public GraphicsDeviceDrawableComponent(IGraphicsDeviceService graphicsDeviceService) {
      this.graphicsDeviceService = graphicsDeviceService;
    }

    /// <summary>Immediately releases all resources owned by this instance</summary>
    /// <remarks>
    ///   This method is not suitable for being called during a GC run, it is intended
    ///   for manual usage when you actually want to get rid of the drawable component.
    /// </remarks>
    public virtual void Dispose() {

      // Unsubscribe from the events of the graphics device service only once
      if (this.graphicsDeviceService != null) {
        unsubscribeFromGraphicsDeviceService();
        this.graphicsDeviceService = null;
      }

      this.serviceProvider = null;

    }

    /// <summary>Gives the game component a chance to initialize itself</summary>
    public override void Initialize() {

      // Only do something here if we were initialized with a service provider,
      // meaning that the developer is using XNA's game services system instead
      // of a full-blown IoC container.
      if (this.graphicsDeviceService == null) {

        // Look for the graphics device service in the game's service container
        this.graphicsDeviceService = this.serviceProvider.GetService(
          typeof(IGraphicsDeviceService)
        ) as IGraphicsDeviceService;

        // Like our XNA pendant, we absolutely require the graphics device service
        if (graphicsDeviceService == null)
          throw new InvalidOperationException("Graphics device service not found");

      }

      // Done, now we can register to the graphics device service's events
      subscribeToGraphicsDeviceService();

    }

    /// <summary>GraphicsDevice this component is bound to. Can be null.</summary>
    public GraphicsDevice GraphicsDevice {
      get { return this.graphicsDeviceService.GraphicsDevice; }
    }

    /// <summary>
    ///   Called when the object needs to set up graphics resources. Override to
    ///   set up any object specific graphics resources.
    /// </summary>
    /// <param name="createAllContent">
    ///   True if all graphics resources need to be set up; false if only
    ///   manual resources need to be set up.
    /// </param>
    [
      Obsolete(
        "The LoadGraphicsContent method is obsolete and will be removed in the future. " +
        "Use the LoadContent method instead."
      )
    ]
    protected virtual void LoadGraphicsContent(bool createAllContent) { }

    /// <summary>
    ///   Called when graphics resources need to be loaded. Override this method to load
    ///   any game-specific graphics resources.
    /// </summary>
    protected virtual void LoadContent() { }

    /// <summary>
    ///   Called when graphics resources should be released. Override to
    ///   handle component specific graphics resources.
    /// </summary>
    /// <param name="destroyAllContent">
    ///   True if all graphics resources should be released; false if only
    ///   manual resources should be released.
    /// </param>
    [
      Obsolete(
        "The UnloadGraphicsContent method is obsolete and will be removed in the future. " +
        "Use the UnloadContent method instead."
      )
    ]
    protected virtual void UnloadGraphicsContent(bool destroyAllContent) { }

    /// <summary>
    ///   Called when graphics resources need to be unloaded. Override this method to unload
    ///   any game-specific graphics resources.
    /// </summary>
    protected virtual void UnloadContent() { }

    /// <summary>
    ///   Subscribes this component to the events of the graphics device service.
    /// </summary>
    private void subscribeToGraphicsDeviceService() {

      // Register to the events of the graphics device service so we know when
      // the graphics device is set up, shut down or reset.
      this.graphicsDeviceService.DeviceCreated += new DeviceEventHandler(deviceCreated);
      this.graphicsDeviceService.DeviceResetting += new DeviceEventHandler(deviceResetting);
      this.graphicsDeviceService.DeviceReset += new DeviceEventHandler(deviceReset);
      this.graphicsDeviceService.DeviceDisposing += new DeviceEventHandler(deviceDisposing);

      // If a graphics device has already been created, we need to simulate the
      // DeviceCreated event that we did miss because we weren't born yet :)
      if (this.graphicsDeviceService.GraphicsDevice != null) {
#pragma warning disable 618 // Call to obsolete method
        LoadGraphicsContent(true);
#pragma warning restore 618 // Call to obsolete method
        LoadContent();
      }

    }

    /// <summary>
    ///   Unsubscribes this component from the events of the graphics device service.
    /// </summary>
    private void unsubscribeFromGraphicsDeviceService() {

      // Unsubscribe from the events again
      this.graphicsDeviceService.DeviceCreated -= new DeviceEventHandler(deviceCreated);
      this.graphicsDeviceService.DeviceResetting -= new DeviceEventHandler(deviceResetting);
      this.graphicsDeviceService.DeviceReset -= new DeviceEventHandler(deviceReset);
      this.graphicsDeviceService.DeviceDisposing -= new DeviceEventHandler(deviceDisposing);

      // If the graphics device is still active, we give the component a chance
      // to clean up its data
      if (this.graphicsDeviceService.GraphicsDevice != null) {
#pragma warning disable 618 // Call to obsolete method
        UnloadGraphicsContent(true);
#pragma warning restore 618 // Call to obsolete method
        UnloadContent();
      }

    }

    /// <summary>Called when the graphics device is created</summary>
    /// <param name="sender">Graphics device service that created a new device</param>
    /// <param name="arguments">Not used</param>
    private void deviceCreated(object sender, EventArgs arguments) {
#pragma warning disable 618 // Call to obsolete method
      LoadGraphicsContent(true);
#pragma warning restore 618 // Call to obsolete method
      LoadContent();
    }

    /// <summary>Called before the graphics device is being reset</summary>
    /// <param name="sender">Graphics device service that is resetting its device</param>
    /// <param name="arguments">Not used</param>
    private void deviceResetting(object sender, EventArgs arguments) {
#pragma warning disable 618 // Call to obsolete method
      UnloadGraphicsContent(false);
#pragma warning restore 618 // Call to obsolete method
    }

    /// <summary>Called after the graphics device has been reset</summary>
    /// <param name="sender">Graphics device service that has just reset its device</param>
    /// <param name="arguments">Not used</param>
    private void deviceReset(object sender, EventArgs arguments) {
#pragma warning disable 618 // Call to obsolete method
      LoadGraphicsContent(false);
#pragma warning restore 618 // Call to obsolete method
    }

    /// <summary>Called before the graphics device is being disposed</summary>
    /// <param name="sender">Graphics device service that's disposing the device</param>
    /// <param name="arguments">Not used</param>
    private void deviceDisposing(object sender, EventArgs arguments) {
#pragma warning disable 618 // Call to obsolete method
      UnloadGraphicsContent(true);
#pragma warning restore 618 // Call to obsolete method
      UnloadContent();
    }

    /// <summary>XNA game service provider (can be null)</summary>
    /// <remarks>
    ///   This is only set when the component is initialized using the IServiceProvider
    ///   constructor, where it needs to remember the service provider until the
    ///   Initialize() method has been called.
    /// </remarks>
    private IServiceProvider serviceProvider;
    /// <summary>Graphics device service this component is bound to.</summary>
    private IGraphicsDeviceService graphicsDeviceService;

  }

} // namespace Nuclex.Graphics
