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
using System.IO;
using System.Resources;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using NUnit.Framework;

using Nuclex.Graphics;
using Nuclex.Testing.Xna;

namespace Nuclex.Fonts.Content {

  /// <summary>Unit tests for the vector font reader</summary>
  [TestFixture]
  public class VectorFontReaderTest {

    #region class MemoryContentManager

    /// <summary>Content manager for loading content from arrays</summary>
    private class MemoryContentManager : ContentManager {

      /// <summary>
      ///   Initializes a new embedded content manager using a directly specified
      ///   graphics device service for the resources.
      /// </summary>
      /// <param name="graphicsDeviceService">
      ///   Graphics device service to load the content asset in
      /// </param>
      public MemoryContentManager(IGraphicsDeviceService graphicsDeviceService) :
        this(makePrivateServiceContainer(graphicsDeviceService)) { }

      /// <summary>
      ///   Initializes a new embedded content manager using the provided game services
      ///   container for providing services for the loaded asset.
      /// </summary>
      /// <param name="services">
      ///   Service container containing the services the asset may access
      /// </param>
      public MemoryContentManager(IServiceProvider services) :
        base(services) { }

      /// <summary>Loads the asset the embedded content manager was created for</summary>
      /// <typeparam name="AssetType">Type of the asset to load</typeparam>
      /// <param name="content">Content that will be loaded as an asset</param>
      /// <returns>The loaded asset</returns>
      public AssetType Load<AssetType>(byte[] content) {
        lock(this) {
          using(this.memoryStream = new MemoryStream(content, false)) {
            return base.ReadAsset<AssetType>("null", null);
          }
        } // lock(this)
      }

      /// <summary>Opens a stream for reading the specified asset</summary>
      /// <param name="assetName">The name of the asset to be read</param>
      /// <returns>The opened stream for the asset</returns>
      protected override Stream OpenStream(string assetName) {
        return this.memoryStream;
      }

      /// <summary>
      ///   Creates a new game service container containing the specified graphics device
      ///   service only.
      /// </summary>
      /// <param name="graphicsDeviceService">Service to add to the service container</param>
      /// <returns>A service container with the specified graphics device service</returns>
      private static IServiceProvider makePrivateServiceContainer(
        IGraphicsDeviceService graphicsDeviceService
      ) {
        GameServiceContainer gameServices = new GameServiceContainer();
        gameServices.AddService(typeof(IGraphicsDeviceService), graphicsDeviceService);
        return gameServices;
      }

      /// <summary>Content that will be loaded by the embedded content manager</summary>
      private MemoryStream memoryStream;

    }

    #endregion // class MemoryContentManager

    /// <summary>
    ///   Tests whether the constructor if the vector font reader is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      VectorFontReader reader = new VectorFontReader();
      Assert.IsNotNull(reader); // nonsense; avoids compiler warning
    }

    /// <summary>Verifies that the vector font reader can load a vector font</summary>
    [Test]
    public void TestVectorFontReading() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
        using(
          ResourceContentManager contentManager = new ResourceContentManager(
            GraphicsDeviceServiceHelper.MakePrivateServiceProvider(service),
            Resources.UnitTestResources.ResourceManager
          )
        ) {
          VectorFont font = contentManager.Load<VectorFont>("UnitTestVectorFont");
          Assert.IsNotNull(font);
        }
      }
    }

  }

} // namespace Nuclex.Fonts.Content

#endif // UNITTEST
