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
using Microsoft.Xna.Framework.Content;

using NUnit.Framework;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects.Masks {

  /// <summary>Unit tests for the solid color screen mask class</summary>
  [TestFixture]
  internal class ColorScreenMaskTest {

    /// <summary>Executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(DeviceType.Reference);
      this.mockedGraphicsDeviceService.CreateDevice();

      serviceProvider = GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
        this.mockedGraphicsDeviceService
      );
    }

    /// <summary>Executed after each test has completed</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockedGraphicsDeviceService != null) {
        this.serviceProvider = null;
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that the constructor of the solid color screen mask class is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      using(
        ColorScreenMask testMask = ColorScreenMask.Create(
          this.mockedGraphicsDeviceService.GraphicsDevice
        )
      ) { }
    }

    /// <summary>
    ///   Tests whether the color property can be assigned and read from
    /// </summary>
    [Test]
    public void TestColorProperty() {
      using(
        ColorScreenMask testMask = ColorScreenMask.Create(
          this.mockedGraphicsDeviceService.GraphicsDevice
        )
      ) {
        Color testColor = new Color(12, 34, 56, 78);

        testMask.Color = testColor;
        Assert.AreEqual(testColor, testMask.Color);
      }
    }

    /// <summary>
    ///   Tests whether a rollback is performed if an exception occurs in
    ///   the solid color screen mask's constructor
    /// </summary>
    [Test]
    public void TestThrowInConstructor() {
      Assert.Throws<AssertionException>(
        delegate() {
          using(
            ColorScreenMask testMask = ColorScreenMask.Create(
              this.mockedGraphicsDeviceService.GraphicsDevice, createFail
            )
          ) { }
        }
      );
    }

    /// <summary>Creates a new instance of the solid color screen mask class</summary>
    /// <param name="graphicsDevice">
    ///   Graphics device the screen mask is rendered with
    /// </param>
    /// <param name="contentManager">
    ///   Content manager the effect was loaded from
    /// </param>
    /// <param name="effect">Effect that will be used to render the screen mask </param>
    /// <returns>A new instance of the solid color screen mask</returns>
    private static ColorScreenMask createFail(
      GraphicsDevice graphicsDevice, ContentManager contentManager, Effect effect
    ) {
      throw new AssertionException("Simulated exception for the unit test");
    }

    /// <summary>
    ///   Tests whether the solid color screen mask is able to draw itself
    /// </summary>
    [Test]
    public void TestDraw() {
      using(
        ColorScreenMask testMask = ColorScreenMask.Create(
          this.mockedGraphicsDeviceService.GraphicsDevice
        )
      ) {
        testMask.Draw();
      }
    }

    /// <summary>Mocked graphics device service used to run the test</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Service provider containing the mocked graphics device service</summary>
    private IServiceProvider serviceProvider;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Masks

#endif // UNITTEST
