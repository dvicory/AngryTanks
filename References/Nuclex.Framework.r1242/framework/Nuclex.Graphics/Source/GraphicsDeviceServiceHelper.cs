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
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Graphics {

  /// <summary>Provides supporting functions for the graphics device service</summary>
  public static class GraphicsDeviceServiceHelper {

    #region class DummyGraphicsDeviceService

    /// <summary>Dummy graphics device service using an existing graphics device</summary>
    private class DummyGraphicsDeviceService : IGraphicsDeviceService, IDisposable {

      /// <summary>Triggered when the graphics device has been created</summary>
      public event DeviceEventHandler DeviceCreated { add { } remove { } }
      /// <summary>Triggered when the graphics device is about to be disposed</summary>
      public event DeviceEventHandler DeviceDisposing;
      /// <summary>Triggered after the graphics device has completed a reset</summary>
      public event DeviceEventHandler DeviceReset;
      /// <summary>Triggered when the graphcis device is about to be reset</summary>
      public event DeviceEventHandler DeviceResetting;

      /// <summary>Initializes a new dummy graphics device service</summary>
      /// <param name="graphicsDevice">Graphics device the service will use</param>
      public DummyGraphicsDeviceService(GraphicsDevice graphicsDevice) {
        this.graphicsDevice = graphicsDevice;
        
        this.graphicsDeviceResettingDelegate = new DeviceEventHandler(
          graphicsDeviceResetting
        );
        this.graphicsDeviceResetDelegate = new DeviceEventHandler(
          graphicsDeviceReset
        );
        this.graphicsDeviceDisposingDelegate = new DeviceEventHandler(
          graphicsDeviceDisposing
        );

        graphicsDevice.DeviceResetting += this.graphicsDeviceResettingDelegate;
        graphicsDevice.DeviceReset += this.graphicsDeviceResetDelegate;
        graphicsDevice.Disposing += this.graphicsDeviceDisposingDelegate;
      }

      /// <summary>Immediately releases all resouces owned by the instance</summary>
      public void Dispose() {
        if(this.graphicsDevice != null) {
          graphicsDeviceDisposing(this.graphicsDevice, EventArgs.Empty);

          this.graphicsDevice.Disposing -= this.graphicsDeviceDisposingDelegate;
          this.graphicsDevice.DeviceReset -= this.graphicsDeviceResetDelegate;
          this.graphicsDevice.DeviceResetting -= this.graphicsDeviceResettingDelegate;

          this.graphicsDevice = null;
        }
      }

      /// <summary>Graphics device provided by the service</summary>
      public GraphicsDevice GraphicsDevice {
        get { return this.graphicsDevice; }
      }

      /// <summary>Called when the graphics device is about to reset</summary>
      /// <param name="sender">Graphics device that is started a reset</param>
      /// <param name="arguments">Not used</param>
      private void graphicsDeviceResetting(object sender, EventArgs arguments) {
        if(DeviceResetting != null) {
          DeviceResetting(this, EventArgs.Empty);
        }
      }

      /// <summary>Called when the graphics device has completed a reset</summary>
      /// <param name="sender">Graphics device that has completed its reset</param>
      /// <param name="arguments">Not used</param>
      private void graphicsDeviceReset(object sender, EventArgs arguments) {
        if(DeviceReset != null) {
          DeviceReset(this, EventArgs.Empty);
        }
      }

      /// <summary>Called when the graphics device is being disposed</summary>
      /// <param name="sender">Graphics device that is being disposed</param>
      /// <param name="arguments">Not used</param>
      private void graphicsDeviceDisposing(object sender, EventArgs arguments) {
        if(DeviceDisposing != null) {
          DeviceDisposing(this, EventArgs.Empty);
        }
      }

      /// <summary>Graphics device the dummy service is being created for</summary>
      private GraphicsDevice graphicsDevice;
      /// <summary>Delegate for the graphicsDeviceResetting() method</summary>
      private DeviceEventHandler graphicsDeviceResettingDelegate;
      /// <summary>Delegate for the graphicsDeviceReset() method</summary>
      private DeviceEventHandler graphicsDeviceResetDelegate;
      /// <summary>Delegate for the graphicsDeviceDisposing() method</summary>
      private DeviceEventHandler graphicsDeviceDisposingDelegate;

    }

    #endregion // class DummyGraphicsDeviceService

    /// <summary>
    ///   Creates a service provider containing only the graphics device service
    /// </summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service that will be provided by the service provider
    /// </param>
    /// <returns>
    ///   A new service provider that provides the graphics device service
    /// </returns>
    public static IServiceProvider MakePrivateServiceProvider(
      IGraphicsDeviceService graphicsDeviceService
    ) {
      GameServiceContainer serviceContainer = new GameServiceContainer();
      serviceContainer.AddService(
        typeof(IGraphicsDeviceService), graphicsDeviceService
      );
      return serviceContainer;
    }

    /// <summary>
    ///   Creates a dummy graphics device service for the provided graphics device
    /// </summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the dummy service is created around
    /// </param>
    /// <returns>A new dummy service for the provided graphics device</returns>
    /// <remarks>
    ///   The dummy graphics device service is in all terms equal to the real thing,
    ///   except that it will trigger the service's events *after* the graphics device
    ///   might have already notified other subscribers.
    /// </remarks>
    public static IGraphicsDeviceService MakeDummyGraphicsDeviceService(
      GraphicsDevice graphicsDevice
    ) {
      return new DummyGraphicsDeviceService(graphicsDevice);
    }

  }

} // namespace Nuclex.Graphics
