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

namespace Nuclex.Graphics {

  /// <summary>Drawable object that monitors the GraphicsDeviceService</summary>
  public abstract class Drawable : IDisposable {

    /// <summary>Initializes a new drawable object.</summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device this drawable object will be bound to
    /// </param>
    public Drawable(IGraphicsDeviceService graphicsDeviceService) {

      // Remember the graphics device service for later
      this.graphicsDeviceService = graphicsDeviceService;

#if XNA_4
      this.deviceCreatedDelegate = new EventHandler<EventArgs>(deviceCreated);
      this.deviceResettingDelegate = new EventHandler<EventArgs>(deviceResetting);
      this.deviceResetDelegate = new EventHandler<EventArgs>(deviceReset);
      this.deviceDisposingDelegate = new EventHandler<EventArgs>(deviceDisposing);
#else
      this.deviceCreatedDelegate = new EventHandler(deviceCreated);
      this.deviceResettingDelegate = new EventHandler(deviceResetting);
      this.deviceResetDelegate = new EventHandler(deviceReset);
      this.deviceDisposingDelegate = new EventHandler(deviceDisposing);
#endif

      // Register to the graphics device events
      subscribeToGraphicsDeviceService();

    }

    /// <summary>Immediately releases all resources owned by this instance</summary>
    /// <remarks>
    ///   This method is not suitable for being called during a GC run, it is intended
    ///   for manual usage when you actually want to get rid of the Drawable object.
    /// </remarks>
    public virtual void Dispose() {

      // Unsubscribe from the graphics device service's events
      if(this.graphicsDeviceService != null) {
        unsubscribeFromGraphicsDeviceService();
        this.graphicsDeviceService = null;
      }

    }

    /// <summary>Called when the Drawable should draw itself.</summary>
    /// <param name="gameTime">Provides a snapshot of the game's timing values</param>
    public virtual void Draw(GameTime gameTime) { }

    /// <summary>GraphicsDevice this component is bound to.</summary>
    public GraphicsDevice GraphicsDevice {
      get { return this.graphicsDeviceService.GraphicsDevice; }
    }

    /// <summary>Retrieves the graphics device service from a service provider.</summary>
    /// <param name="serviceProvider">Service provider to look in</param>
    /// <returns>The graphics device service, if it was available</returns>
    /// <remarks>
    ///   This method is included in the Drawable class to allow deriving classes
    ///   to expect an IServerProvider as their constructor argument and still
    ///   initialize Drawable, their base class, with a graphics device service.
    ///   <example>
    ///     <code>
    ///       public class MyDrawable : Drawable {
    /// 
    ///         public MyDrawable(IServiceProvider serviceProvider) :
    ///           base(GetGraphicsDeviceService(serviceProvider)) { }
    ///      
    ///       }
    ///     </code>
    ///   </example>
    /// </remarks>
    protected static IGraphicsDeviceService GetGraphicsDeviceService(
      IServiceProvider serviceProvider
    ) {
      IGraphicsDeviceService graphicsDeviceService = serviceProvider.GetService(
        typeof(IGraphicsDeviceService)
      ) as IGraphicsDeviceService;

      if(graphicsDeviceService == null)
        throw new InvalidOperationException("Graphics device service not found");

      return graphicsDeviceService;
    }

    /// <summary>Graphics device service the drawable was constucted on</summary>
    protected IGraphicsDeviceService GraphicsDeviceService {
      get { return this.graphicsDeviceService; }
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
      this.graphicsDeviceService.DeviceCreated += this.deviceCreatedDelegate;
      this.graphicsDeviceService.DeviceResetting += this.deviceResettingDelegate;
      this.graphicsDeviceService.DeviceReset += this.deviceResetDelegate;
      this.graphicsDeviceService.DeviceDisposing += this.deviceDisposingDelegate;

      // If a graphics device has already been created, we need to simulate the
      // DeviceCreated event that we did miss because we weren't born yet :)
      if(this.graphicsDeviceService.GraphicsDevice != null) {
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
#if XNA_4
      this.graphicsDeviceService.DeviceCreated -= new EventHandler<EventArgs>(deviceCreated);
      this.graphicsDeviceService.DeviceResetting -= new EventHandler<EventArgs>(deviceResetting);
      this.graphicsDeviceService.DeviceReset -= new EventHandler<EventArgs>(deviceReset);
      this.graphicsDeviceService.DeviceDisposing -= new EventHandler<EventArgs>(deviceDisposing);
#else
      this.graphicsDeviceService.DeviceCreated -= new EventHandler(deviceCreated);
      this.graphicsDeviceService.DeviceResetting -= new EventHandler(deviceResetting);
      this.graphicsDeviceService.DeviceReset -= new EventHandler(deviceReset);
      this.graphicsDeviceService.DeviceDisposing -= new EventHandler(deviceDisposing);
#endif
      // If the graphics device is still active, we give the component a chance
      // to clean up its data
      if(this.graphicsDeviceService.GraphicsDevice != null) {
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

    /// <summary>Graphics device service this component is bound to.</summary>
    private IGraphicsDeviceService graphicsDeviceService;

#if XNA_4
    /// <summary>Delegate for the deviceCreated() method</summary>
    private EventHandler<EventArgs> deviceCreatedDelegate;
    /// <summary>Delegate for the deviceResetting() method</summary>
    private EventHandler<EventArgs> deviceResettingDelegate;
    /// <summary>Delegate for the deviceReset() method</summary>
    private EventHandler<EventArgs> deviceResetDelegate;
    /// <summary>Delegate for the deviceDisposing() method</summary>
    private EventHandler<EventArgs> deviceDisposingDelegate;
#else
    /// <summary>Delegate for the deviceCreated() method</summary>
    private EventHandler deviceCreatedDelegate;
    /// <summary>Delegate for the deviceResetting() method</summary>
    private EventHandler deviceResettingDelegate;
    /// <summary>Delegate for the deviceReset() method</summary>
    private EventHandler deviceResetDelegate;
    /// <summary>Delegate for the deviceDisposing() method</summary>
    private EventHandler deviceDisposingDelegate;
#endif

  }

} // namespace Nuclex.Graphics
