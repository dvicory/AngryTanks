#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
using System.Globalization;
using System.IO;
using System.Resources;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

using Nuclex.Testing.Xna;

namespace Nuclex.UserInterface.Visuals.Flat {

  /// <summary>Unit tests for flat GUI graphics interface</summary>
  [TestFixture]
  internal class FlatGuiGraphicsTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(
        DeviceType.Reference
      );
      this.mockedGraphicsDeviceService.CreateDevice();
    }

    private void loadSkin(ResourceManager resourceManager, byte[] skinDescription) {
      this.contentManager = new ResourceContentManager(
        this.mockedGraphicsDeviceService.ServiceProvider, resourceManager
      );

      using(
        MemoryStream skinStream = new MemoryStream(skinDescription, false)
      ) {
        this.graphics = new FlatGuiGraphics(this.contentManager, skinStream);
      }
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.graphics != null) {
        this.graphics.Dispose();
        this.graphics = null;
      }
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.contentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>Verifies that the painter's constructor is working</summary>
    [Test]
    public void TestConstructor() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );
      Assert.IsNotNull(this.graphics); // for readability, graphics cannot be null here
    }

    /// <summary>Verifies that the painter can assign a clipping region</summary>
    [Test]
    public void TestClipRegion() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );

      RectangleF clipRegion = new RectangleF(4.0f, 4.0f, 12.0f, 12.0f);
      this.graphics.BeginDrawing();
      try {
        using(this.graphics.SetClipRegion(clipRegion)) { }

        // No exception means the test passes.
        // Further validation would require visual inspection.
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>
    ///   Verifies that the painter can handle a clipping region that leaves
    ///   the viewport
    /// </summary>
    [Test]
    public void TestClipRegionOffScreen() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );

      RectangleF clipRegion = new RectangleF(-20.0f, -4.0f, 4.0f, 20.0f);
      this.graphics.BeginDrawing();
      try {
        using(this.graphics.SetClipRegion(clipRegion)) { }

        // No exception means the test passes.
        // Further validation would require visual inspection.
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>Verifies that the painter can draw skin elements</summary>
    [Test]
    public void TestDrawElement() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );

      this.graphics.BeginDrawing();
      try {
        this.graphics.DrawElement("window", new RectangleF(2.0f, 2.0f, 14.0f, 14.0f));

        // No exception means the test passes.
        // Further validation would require visual inspection.
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>Verifies that the painter can draw strings</summary>
    [Test]
    public void TestDrawString() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );

      this.graphics.BeginDrawing();
      try {
        this.graphics.DrawString(
          "window", new RectangleF(2.0f, 2.0f, 14.0f, 14.0f), "Hello World"
        );

        // No exception means the test passes.
        // Further validation would require visual inspection.
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>Verifies that the painter can draw a text cursor (caret)</summary>
    [Test]
    public void TestDrawCaret() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );

      this.graphics.BeginDrawing();
      try {
        this.graphics.DrawCaret(
          "window", new RectangleF(2.0f, 2.0f, 14.0f, 14.0f), "Hello World", 5
        );

        // No exception means the test passes.
        // Further validation would require visual inspection.
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>Verifies that the painter can measure the size of a string</summary>
    [Test]
    public void TestMeasureString() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );

      this.graphics.BeginDrawing();
      try {
        RectangleF size = this.graphics.MeasureString(
          "window", new RectangleF(2.0f, 2.0f, 14.0f, 14.0f), "Hello World"
        );

        Assert.Greater(size.Width, 0.0f);
        Assert.Greater(size.Height, 0.0f);
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>
    ///   Verifies that the painter can measure the size of a string drawn to a frame
    ///   which contains no text
    /// </summary>
    [Test]
    public void TestMeasureStringOnTextlessFrame() {
      loadSkin(
        Resources.UnitTestResources.ResourceManager,
        Resources.UnitTestResources.UnitTestSkin
      );

      this.graphics.BeginDrawing();
      try {
        RectangleF size = this.graphics.MeasureString(
          "textless", new RectangleF(2.0f, 2.0f, 14.0f, 14.0f), "Hello World"
        );
        Assert.AreEqual(RectangleF.Empty, size);
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>
    ///   Verifies that the painter can locate the opening between two characters
    ///   in a string that's closest to a specific position
    /// </summary>
    [Test]
    public void TestGetClosestOpening() {
      loadSkin(
        Resources.SuaveSkinResources.ResourceManager,
        Resources.SuaveSkinResources.SuaveSkin
      );

      this.graphics.BeginDrawing();
      try {
        int index = this.graphics.GetClosestOpening(
          "window",
          new RectangleF(2.0f, 2.0f, 14.0f, 14.0f),
          "Hello World",
          new Vector2(1.0f, 8.0f)
        );

        Assert.AreEqual(0, index);
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>Tests whether strings can be positioned at all anchoring spots</summary>
    [Test]
    public void TestStringPositioning() {
      loadSkin(
        Resources.UnitTestResources.ResourceManager,
        Resources.UnitTestResources.UnitTestSkin
      );

      this.graphics.BeginDrawing();
      try {
        this.graphics.DrawString(
          "test", new RectangleF(2.0f, 2.0f, 12.0f, 12.0f), "Hello World"
        );
      }
      finally {
        this.graphics.EndDrawing();
      }
    }

    /// <summary>
    ///   Tests whether an exception is thrown if a skin element doesn't exist
    /// </summary>
    [Test]
    public void TestThrowOnMissingFrame() {
      loadSkin(
        Resources.UnitTestResources.ResourceManager,
        Resources.UnitTestResources.UnitTestSkin
      );
      Assert.Throws<ArgumentException>(
        delegate() {
          this.graphics.DrawElement("this.does.not.exist", RectangleF.Empty);
        }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if a skin contains an bad color constant
    /// </summary>
    [Test]
    public void TestThrowOnBadColor() {
      Assert.Throws<ArgumentException>(
        delegate() {
          loadSkin(
            Resources.UnitTestResources.ResourceManager,
            Resources.UnitTestResources.BadColorSkin
          );
        }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if a skin contains an bad horizontal
    ///   text position
    /// </summary>
    [Test]
    public void TestThrowOnBadHorizontalPosition() {
      Assert.Throws<System.Xml.Schema.XmlSchemaValidationException>(
        delegate() {
          loadSkin(
            Resources.UnitTestResources.ResourceManager,
            Resources.UnitTestResources.BadHorizontalPositionSkin
          );
        }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if a skin contains an bad horizontal
    ///   text position
    /// </summary>
    [Test]
    public void TestThrowOnBadVerticalPosition() {
      Assert.Throws<System.Xml.Schema.XmlSchemaValidationException>(
        delegate() {
          loadSkin(
            Resources.UnitTestResources.ResourceManager,
            Resources.UnitTestResources.BadVerticalPositionSkin
          );
        }
      );
    }

    /// <summary>Mocked graphics device used to run the unit tests</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Content manager holding the resources of the painter's skin</summary>
    private ResourceContentManager contentManager;
    /// <summary>GUI graphics interface being tested</summary>
    private FlatGuiGraphics graphics;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat

#endif // UNITTEST
