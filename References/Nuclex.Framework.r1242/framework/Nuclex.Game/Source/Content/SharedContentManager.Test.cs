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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Graphics;
using Nuclex.Testing.Xna;

namespace Nuclex.Game.Content {

  /// <summary>Unit test for the shared content manager class</summary>
  [TestFixture]
  internal class SharedContentManagerTest {

    #region class DummyContentManager

    /// <summary>Dummy content manager used for unit testing</summary>
    private class DummyContentManager : ContentManager {

      /// <summary>Initializes a new dummy content manager</summary>
      /// <param name="gameServices">Game services the content manager will use</param>
      public DummyContentManager(IServiceProvider gameServices) :
        base(gameServices) { }

      /// <summary>
      ///   Loads an asset that has been processed by the Content Pipeline. Reference
      ///   page contains code sample.
      /// </summary>
      /// <typeparam name="AssetType">Type of the asset that will be loaded</typeparam>
      /// <param name="assetName">
      ///   Asset name, relative to the loader root directory, and not including
      ///   the .xnb file extension.
      /// </param>
      /// <returns>
      ///   The loaded asset. Repeated calls to load the same asset will return
      ///   the same object instance.
      /// </returns>
      public override AssetType Load<AssetType>(string assetName) {
        this.LastLoadedAsset = assetName;
        return default(AssetType);
      }

      /// <summary>Called to unload all content</summary>
      public override void Unload() {
        ++this.UnloadCallCount;
      }

      /// <summary>The last asset this content manager tried to load</summary>
      public string LastLoadedAsset;
      /// <summary>Number of times the Unload() method has been called</summary>
      public int UnloadCallCount;

    }

    #endregion // class DummyContentManager

    #region class TestSharedContentManager

    /// <summary>Shared content manager for testing</summary>
    private class TestSharedContentManager : SharedContentManager {

      /// <summary>Initializes a new shared content manager</summary>
      /// <param name="gameServices">
      ///   Game services the shared content manager will use and add its own
      ///   shared content service to
      /// </param>
      public TestSharedContentManager(GameServiceContainer gameServices) :
        base(gameServices) {
        this.serviceProvider = gameServices;
      }

      /// <summary>The last asset the shared content provider tried to load</summary>
      public string LastLoadedAsset {
        get {
          if(this.dummyContentManager == null) {
            return null;
          } else {
            return this.dummyContentManager.LastLoadedAsset;
          }
        }
      }

      /// <summary>Number of times the Unload() method has been called</summary>
      public int UnloadCallCount {
        get { return this.dummyContentManager.UnloadCallCount; }
      }

      /// <summary>Creates a new content manager for the shared content provider</summary>
      /// <returns>The newly created content manager</returns>
      protected override ContentManager CreateContentManager() {
        this.dummyContentManager = new DummyContentManager(this.serviceProvider);
        return this.dummyContentManager;
      }

      /// <summary>
      ///   Dummy content manager that has been created by the shared content provider
      /// </summary>
      private DummyContentManager dummyContentManager;
      /// <summary>
      ///   Service container used by the content manager to look up that graphics device
      /// </summary>
      private IServiceProvider serviceProvider;

    }

    #endregion // class TestSharedContentManager

    /// <summary>Tests the constructor of the shared content manager</summary>
    [Test]
    public void TestConstructor() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);
      using(
        SharedContentManager contentManager = new SharedContentManager(gameServices)
      ) {
        // Nonsense, but avoids compiler warning about unused variable :)
        Assert.IsNotNull(contentManager);
      }
    }

    /// <summary>
    ///   Verifies that the shared content manager registers itself as a service
    /// </summary>
    [Test]
    public void TestServiceRegistration() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);
      using(
        SharedContentManager contentManager = new SharedContentManager(gameServices)
      ) {
        object service = gameServices.GetService(typeof(ISharedContentService));

        Assert.AreSame(contentManager, service);
      }
    }

    /// <summary>
    ///   Verifies that the shared content manager can be initialized
    /// </summary>
    [Test]
    public void TestInitialization() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);
      using(
        SharedContentManager contentManager = new SharedContentManager(gameServices)
      ) {
        contentManager.Initialize();
      }
    }

    /// <summary>
    ///   Verifies that the shared content manager can be initialized and takes over
    ///   an existing graphics device provided by the game
    /// </summary>
    [Test]
    public void TestInitializationWithExistingGraphicsDevice() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);
      using(IDisposable keeper = mockedGraphics.CreateDevice()) {
        using(
          SharedContentManager contentManager = new SharedContentManager(gameServices)
        ) {
          contentManager.Initialize();
        }
      }
    }

    /// <summary>
    ///   Test whether the shared content manager performs the neccessary cleanup
    ///   work when it is disposed
    /// </summary>
    [Test]
    public void TestDispose() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);
      using(
        SharedContentManager contentManager = new SharedContentManager(gameServices)
      ) {
        object service = gameServices.GetService(typeof(ISharedContentService));
        Assert.AreSame(contentManager, service);
      }

      // Make sure the service was unregistered again when the shared content manager
      // got disposed
      object serviceAfterDispose = gameServices.GetService(typeof(ISharedContentService));
      Assert.IsNull(serviceAfterDispose);
    }

    /// <summary>
    ///   Ensures that the content manager throws an exception if it is asked to load
    ///   an asset before it has been initialized
    /// </summary>
    [Test]
    public void TestThrowOnLoadAssetBeforeInitialization() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);
      using(
        TestSharedContentManager contentManager = new TestSharedContentManager(gameServices)
      ) {
        Assert.Throws<InvalidOperationException>(
          delegate() { contentManager.Load<BadImageFormatException>("I'm a funny asset"); }
        );
      }
    }

    /// <summary>
    ///   Tests whether the shared content provider passes on the Load() call to its
    ///   internal content manager
    /// </summary>
    [Test]
    public void TestLoadAsset() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);
      using(
        TestSharedContentManager contentManager = new TestSharedContentManager(gameServices)
      ) {
        contentManager.Initialize();

        Assert.IsNull(contentManager.LastLoadedAsset);
        contentManager.Load<BadImageFormatException>("I'm a funny asset");
        Assert.AreEqual("I'm a funny asset", contentManager.LastLoadedAsset);
      }
    }

    /// <summary>
    ///   Tests whether the shared content provider passes on the Load() call to its
    ///   internal content manager
    /// </summary>
    [Test]
    public void TestUnloadContent() {
      GameServiceContainer gameServices = new GameServiceContainer();
      MockedGraphicsDeviceService mockedGraphics = new MockedGraphicsDeviceService();
      gameServices.AddService(typeof(IGraphicsDeviceService), mockedGraphics);

      using(IDisposable keeper = mockedGraphics.CreateDevice()) {
        using(
          TestSharedContentManager contentManager = new TestSharedContentManager(gameServices)
        ) {
          contentManager.Initialize();

          Assert.AreEqual(0, contentManager.UnloadCallCount);

          contentManager.Unload();

          Assert.AreEqual(1, contentManager.UnloadCallCount);
        }
      }
    }

  }

} // namespace Nuclex.Game.Content

#endif // UNITTEST
