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

#if UNITTEST

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Testing.Xna {

  /// <summary>Unit tests for the graphics device mock test helper</summary>
  [TestFixture]
  public class MockedGraphicsDeviceServiceTest {

    #region interface IGraphicsDeviceServiceSubscriber

    /// <summary>Subscriber for the event of the graphics device service</summary>
    public interface IGraphicsDeviceServiceSubscriber {
      /// <summary>Called when a graphics device has been created</summary>
      /// <param name="sender">
      ///   Graphics device service that created a graphics device
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceCreated(object sender, EventArgs arguments);
      /// <summary>Called when a graphics device is about to be destroyed</summary>
      /// <param name="sender">
      ///   Graphics device service that is about to destroy its graphics device
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceDisposing(object sender, EventArgs arguments);
      /// <summary>Called when the graphics device is about to reset itself</summary>
      /// <param name="sender">
      ///   Graphics device service whose graphics device is about to reset itself
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceResetting(object sender, EventArgs arguments);
      /// <summary>Called when the graphics device has completed a reset</summary>
      /// <param name="sender">
      ///   Graphics device service whose graphics device has completed a reset
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceReset(object sender, EventArgs arguments);
    }

    #endregion // interface IGraphicsDeviceSubscriber

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Tests whether the mock's service provider is set up correctly</summary>
    [Test]
    public void TestServiceProvider() {
      MockedGraphicsDeviceService mock = new MockedGraphicsDeviceService();

      IServiceProvider serviceProvider = mock.ServiceProvider;
      Assert.IsNotNull(serviceProvider);

      IGraphicsDeviceService service = (IGraphicsDeviceService)serviceProvider.GetService(
        typeof(IGraphicsDeviceService)
      );
      Assert.AreSame(mock, service);
    }

    /// <summary>Tests whether a graphics device can be created</summary>
    [Test]
    public void TestGraphicsDeviceCreation() {
      MockedGraphicsDeviceService mock = new MockedGraphicsDeviceService();

      using(IDisposable keeper = mock.CreateDevice()) {
        Assert.IsNotNull(mock.GraphicsDevice);
      }
    }

    /// <summary>
    ///   Verifies that the graphics device is destroyed when the keeper returned
    ///   by the CreateDevice() method gets disposed explicitely.
    /// </summary>
    [Test]
    public void TestAutomaticGraphicsDeviceDestruction() {
      MockedGraphicsDeviceService mock = new MockedGraphicsDeviceService();

      try {
        using(IDisposable keeper = mock.CreateDevice()) {
          Assert.IsNotNull(mock.GraphicsDevice);
          throw new ArithmeticException("Test exception");
        }
      }
      catch(ArithmeticException) {
        // Munch
      }

      Assert.IsNull(mock.GraphicsDevice);
    }

    /// <summary>
    ///   Verifies that the mocked graphics device service fires its events
    /// </summary>
    [Test]
    public void TestGraphicsDeviceServiceEvents() {
      MockedGraphicsDeviceService mock = new MockedGraphicsDeviceService();
      IGraphicsDeviceServiceSubscriber mockedSubscriber = mockSubscriber(mock);

      Expect.Once.On(mockedSubscriber).Method("DeviceCreated").WithAnyArguments();
      using(IDisposable keeper = mock.CreateDevice()) {
        this.mockery.VerifyAllExpectationsHaveBeenMet();

        Expect.Once.On(mockedSubscriber).Method("DeviceResetting").WithAnyArguments();
        Expect.Once.On(mockedSubscriber).Method("DeviceReset").WithAnyArguments();
        mock.ResetDevice();
        this.mockery.VerifyAllExpectationsHaveBeenMet();

        Expect.Once.On(mockedSubscriber).Method("DeviceDisposing").WithAnyArguments();
      }
      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Tests whether the graphics device can be destroyed manually even
    ///   though it the RAII helper is used without causing an exception
    /// </summary>
    [Test]
    public void TestRedundantDestroyInvocation() {
      MockedGraphicsDeviceService mock = new MockedGraphicsDeviceService();

      using(IDisposable keeper = mock.CreateDevice()) {
        mock.DestroyDevice();
      } // should not cause an exception
    }

    /// <summary>
    ///   Verifies that the mocked graphics device service cleans up the graphics
    ///   device and all of its resources again when an exception occurs during
    ///   its creation
    /// </summary>
    [Test]
    public void TestExceptionDuringDeviceCreation() {
      MockedGraphicsDeviceService mock = new MockedGraphicsDeviceService();

      IGraphicsDeviceServiceSubscriber mockedSubscriber = mockSubscriber(mock);

      Expect.Once.On(mockedSubscriber).Method("DeviceCreated").WithAnyArguments();

      mock.DeviceCreated += (DeviceEventHandler)delegate(object sender, EventArgs arguments) {
        Assert.IsNotNull(mock.GraphicsDevice);
        throw new ArithmeticException("Test exception");
      };
      try {
        mock.CreateDevice();
      }
      catch(ArithmeticException) {
        // Munch
      }

      Assert.IsNull(mock.GraphicsDevice);
    }

    /// <summary>
    ///   Verifies that the mocked graphics device service can cope with
    ///   a NotSupportedException when the reference rasterizer is selected
    /// </summary>
    [Test]
    public void TestNotSupportedExceptionForReferenceRasterizer() {
      MockedGraphicsDeviceService mock = new MockedGraphicsDeviceService(
        DeviceType.Reference
      );
      mock.DeviceCreated += delegate(object sender, EventArgs arguments) {
#if XNA_4
        throw new InvalidOperationException("Simulated error for unit testing");
#else
        throw new NotSupportedException("Simulated error for unit testing");
#endif
      };

      Console.Error.WriteLine(
        "The next line should contain an error message indicating that the reference " +
        "rasterizer could not be created"
      );
#if XNA_4
      Assert.Throws<InvalidOperationException>(
#else
      Assert.Throws<NotSupportedException>(
#endif
delegate() {
          mock.CreateDevice();
          mock.DestroyDevice();
        }
      );
    }

    /// <summary>
    ///   Mocks a subscriber for the events of the mocked graphics device service
    /// </summary>
    /// <returns>The mocked event subscriber</returns>
    private IGraphicsDeviceServiceSubscriber mockSubscriber(
      IGraphicsDeviceService graphicsDeviceService
    ) {
      IGraphicsDeviceServiceSubscriber mockedSubscriber =
        this.mockery.NewMock<IGraphicsDeviceServiceSubscriber>();

      graphicsDeviceService.DeviceCreated += new DeviceEventHandler(
        mockedSubscriber.DeviceCreated
      );
      graphicsDeviceService.DeviceResetting += new DeviceEventHandler(
        mockedSubscriber.DeviceResetting
      );
      graphicsDeviceService.DeviceReset += new DeviceEventHandler(
        mockedSubscriber.DeviceReset
      );
      graphicsDeviceService.DeviceDisposing += new DeviceEventHandler(
        mockedSubscriber.DeviceDisposing
      );

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Testing.Xna

#endif // UNITTEST
