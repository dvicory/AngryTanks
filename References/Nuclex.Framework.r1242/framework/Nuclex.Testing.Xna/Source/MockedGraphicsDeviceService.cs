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
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Testing.Xna {

  /// <summary>Helper for unit tests requiring a mocked graphics device</summary>
  /// <remarks>
  ///   This doesn't actually mock the graphics device, but creates a real graphics
  ///   device on an invisible window. Tests have shown this method to be fast
  ///   enough for usage in a unit test.
  /// </remarks>
  public class MockedGraphicsDeviceService : IGraphicsDeviceService {

    /// <summary>Will be triggered when the graphics device has been created</summary>
    public event DeviceEventHandler DeviceCreated;
    /// <summary>
    ///   Will be triggered when the graphics device is about to be destroyed
    /// </summary>
    public event DeviceEventHandler DeviceDisposing;
    /// <summary>
    ///   Will be triggered when the graphics device has completed a reset
    /// </summary>
    public event DeviceEventHandler DeviceReset;
    /// <summary>
    ///   Will be triggered when the graphics device is about to reset itself
    /// </summary>
    public event DeviceEventHandler DeviceResetting;

    #region class GraphicsDeviceKeeper

    /// <summary>Keeps a graphics device alive for RAII-like usage</summary>
    /// <remarks>
    ///   RAII means "Resource Acquisition Is Initialization" and is a very widespread
    ///   pattern in languages with deterministic finalization (read: not .NET).
    /// </remarks>
    private class GraphicsDeviceKeeper : IDisposable {

      /// <summary>Initializes a new graphics device keeper</summary>
      /// <param name="dummyService">
      ///   Dummy graphics device service for whose graphics device the keeper
      ///   will be responsible
      /// </param>
      public GraphicsDeviceKeeper(MockedGraphicsDeviceService dummyService) {
        this.dummyService = dummyService;
      }

      /// <summary>Immediately releases all resources owned by the instancer</summary>
      public void Dispose() {
        if(this.dummyService != null) {
          this.dummyService.DestroyDevice();
          this.dummyService = null;
        }
      }

      /// <summary>
      ///   Dummy graphics device service in which the graphics device needs to be
      ///   destroyed when the keeper is disposed
      /// </summary>
      private MockedGraphicsDeviceService dummyService;

    }

    #endregion // classs GraphicsDeviceKeeper

    /// <summary>Initializs a new mocked graphics device service</summary>
    public MockedGraphicsDeviceService() : this(DeviceType.NullReference) { }

#if XNA_4

    /// <summary>Initializs a new mocked graphics device service</summary>
    /// <param name="deviceType">Type of graphics device that will be created</param>
    public MockedGraphicsDeviceService(DeviceType deviceType) :
      this(deviceType, GraphicsProfile.Reach) { }

    /// <summary>Initializs a new mocked graphics device service</summary>
    /// <param name="deviceType">Type of graphics device that will be created</param>
    /// <param name="graphicsProfile">Profile the graphics device will be initialized for</param>
    public MockedGraphicsDeviceService(
      DeviceType deviceType, GraphicsProfile graphicsProfile
    ) {
      this.deviceType = deviceType;
      this.graphicsProfile = graphicsProfile;
      this.serviceContainer = new GameServiceContainer();
      this.serviceContainer.AddService(typeof(IGraphicsDeviceService), this);
    }

#else

    /// <summary>Initializs a new mocked graphics device service</summary>
    /// <param name="deviceType">Type of graphics device that will be created</param>
    public MockedGraphicsDeviceService(DeviceType deviceType) {
      this.deviceType = deviceType;
      this.serviceContainer = new GameServiceContainer();
      this.serviceContainer.AddService(typeof(IGraphicsDeviceService), this);
    }

#endif

    /// <summary>Graphics device provided by the graphics device service</summary>
    public GraphicsDevice GraphicsDevice {
      get { return this.dummyGraphicsDevice; }
    }

    /// <summary>
    ///   A service provider containing the mocked graphics device service
    /// </summary>
    public IServiceProvider ServiceProvider {
      get { return this.serviceContainer; }
    }

    /// <summary>Creates a new graphics device</summary>
    /// <returns>
    ///   An object implementing IDisposable that will destroy the graphics device
    ///   again as soon as its Dispose() method is called.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     Make sure to call DestroyGraphicsDevice() either manually,
    ///     or by disposing the returned object. A typical usage of this method is
    ///     shown in the following code.
    ///   </para>
    ///   <example>
    ///     <code>
    ///       using(IDisposable keeper = CreateDevice()) {
    ///         GraphicsDevice.DoSomethingThatCouldFail();
    ///       }
    ///     </code>
    ///   </example>
    /// </remarks>
    public IDisposable CreateDevice() {
      this.emptyPresentationParameters = new PresentationParameters();

      this.invisibleRenderWindow = new Form();
      try {
        this.invisibleRenderWindow.Visible = false;
        this.invisibleRenderWindow.ShowInTaskbar = false;
        // Do not minimize, the GraphicsDevice doesn't like that!

        IntPtr renderWindowHandle = this.invisibleRenderWindow.Handle;
#if XNA_4
        this.emptyPresentationParameters.DeviceWindowHandle = renderWindowHandle;
        this.emptyPresentationParameters.IsFullScreen = false;

        //GraphicsAdapter.UseNullDevice = (this.deviceType == DeviceType.NullReference);
        GraphicsAdapter.UseReferenceDevice = (this.deviceType != DeviceType.Hardware);
#endif
        this.dummyGraphicsDevice = new GraphicsDevice(
          GraphicsAdapter.DefaultAdapter,
#if XNA_4
 this.graphicsProfile,
#else
          this.deviceType,
          renderWindowHandle,
#endif
 this.emptyPresentationParameters
        );

        OnDeviceCreated();

        return new GraphicsDeviceKeeper(this);
      }
      catch(InvalidOperationException exception) {
        if(this.deviceType == DeviceType.Reference) {
          string message =
            "GraphicsDevice creation failed with InvalidOperationException when asking " +
            "for the reference rasterizer. DirectX Debug Runtime not installed?";

          // Also write the message to stderr so it will be seen if the unit tests are
          // run and fail due to the missing DirectX debug runtime
          Console.Error.WriteLine(message);
          System.Diagnostics.Trace.WriteLine(message);

          disposeEverything();
          throw new InvalidOperationException(message, exception);
        }

        disposeEverything();
        throw;
      }
      catch(Exception) {
        disposeEverything();
        throw;
      }
    }

    /// <summary>Destroys the created graphics device again</summary>
    public void DestroyDevice() {
      OnDeviceDisposing();
      disposeEverything();
    }

    /// <summary>Performs a graphics device reset</summary>
    public void ResetDevice() {
      OnDeviceResetting();
#if XNA_4
      Viewport dummyViewport = new Viewport(
        0, 0,
        this.invisibleRenderWindow.ClientSize.Width,
        this.invisibleRenderWindow.ClientSize.Height
      );
      dummyViewport.MinDepth = 0.1f;
      dummyViewport.MaxDepth = 0.9f;
      this.dummyGraphicsDevice.Viewport = dummyViewport;
#endif
      this.dummyGraphicsDevice.Reset();
      OnDeviceReset();
    }

    /// <summary>
    ///   Shuts down and disposes all resources used by the mocked graphics device service
    /// </summary>
    private void disposeEverything() {
      if(this.dummyGraphicsDevice != null) {
        this.dummyGraphicsDevice.Dispose();
        this.dummyGraphicsDevice = null;
      }
      if(this.invisibleRenderWindow != null) {
        this.invisibleRenderWindow.Dispose();
        this.invisibleRenderWindow = null;
      }
#if !XNA_4
      if(this.emptyPresentationParameters != null) {
        this.emptyPresentationParameters.Dispose();
        this.emptyPresentationParameters = null;
      }
#endif
    }

    /// <summary>Fires the DeviceCreated event</summary>
    protected virtual void OnDeviceCreated() {
      if(this.DeviceCreated != null) {
        DeviceCreated(this, EventArgs.Empty);
      }
    }

    /// <summary>Fires the DeviceDisposing event</summary>
    protected virtual void OnDeviceDisposing() {
      if(this.DeviceDisposing != null) {
        DeviceDisposing(this, EventArgs.Empty);
      }
    }

    /// <summary>Fires the DeviceResetting event</summary>
    protected virtual void OnDeviceResetting() {
      if(this.DeviceResetting != null) {
        DeviceResetting(this, EventArgs.Empty);
      }
    }

    /// <summary>Fires the DeviceReset event</summary>
    protected virtual void OnDeviceReset() {
      if(this.DeviceReset != null) {
        DeviceReset(this, EventArgs.Empty);
      }
    }

    /// <summary>A dummy graphics device used to run the unit tests</summary>
    private GraphicsDevice dummyGraphicsDevice;
    /// <summary>
    ///   Empty presentation parameters used to initialize the dummy graphics device
    /// </summary>
    private PresentationParameters emptyPresentationParameters;
    /// <summary>Invisible render window the dummy graphics device renders into</summary>
    private Form invisibleRenderWindow;
    /// <summary>A service container providing this service</summary>
    private GameServiceContainer serviceContainer;
    /// <summary>Type of device that will be created</summary>
    private DeviceType deviceType;
#if XNA_4
    /// <summary>Graphics profile the device will be created for</summary>
    private GraphicsProfile graphicsProfile;
#endif

  }

} // namespace Nuclex.Testing.Xna
