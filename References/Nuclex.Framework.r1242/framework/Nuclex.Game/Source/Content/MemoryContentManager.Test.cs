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

  /// <summary>Unit test for the embedded content manager class</summary>
  [TestFixture]
  internal class MemoryContentManagerTest {

    /// <summary>Tests the constructor of the embedded content manager</summary>
    [Test]
    public void TestConstructor() {
      Assert.IsNotNull(this.memoryContentManager);
    }
    
    /// <summary>Verifies that assets can be loaded</summary>
    [Test]
    public void TestLoadAsset() {
      Effect effect = this.memoryContentManager.Load<Effect>(
        Resources.UnitTestResources.UnitTestEffect
      );
      Assert.IsNotNull(effect);
    }

    /// <summary>Verifies that uniquely named assets can be loaded</summary>
    [Test]
    public void TestLoadNamedAsset() {
      Effect effect = this.memoryContentManager.Load<Effect>(
        Resources.UnitTestResources.UnitTestEffect, "UnitTestEffect"
      );
      Assert.IsNotNull(effect);
    }

    /// <summary>Verifies that the ReadAsset() method is working</summary>
    [Test]
    public void TestReadAsset() {
      Effect effect = this.memoryContentManager.ReadAsset<Effect>(
        Resources.UnitTestResources.UnitTestEffect
      );
      Assert.IsNotNull(effect);
    }
    
    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();
      this.mockedGraphicsDeviceService.CreateDevice();
      
      this.memoryContentManager = new MemoryContentManager(
        this.mockedGraphicsDeviceService
      );
    }
    
    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.memoryContentManager != null) {
        this.memoryContentManager.Dispose();
        this.memoryContentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }
    
    /// <summary>Mock of the graphics device service used for unit testing</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Content manager which loads resources from in-memory arrays</summary>
    private MemoryContentManager memoryContentManager;

  }

} // namespace Nuclex.Game.Content

#endif // UNITTEST
