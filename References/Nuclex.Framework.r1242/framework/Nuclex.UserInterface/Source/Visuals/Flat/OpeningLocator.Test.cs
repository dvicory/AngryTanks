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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using NUnit.Framework;
using NMock2;

using Nuclex.Testing.Xna;

namespace Nuclex.UserInterface.Visuals.Flat {

  /// <summary>Unit tests for character opening locator</summary>
  [TestFixture]
  internal class OpeningLocatorTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();
      this.mockedGraphicsDeviceService.CreateDevice();

      this.contentManager = new ResourceContentManager(
        this.mockedGraphicsDeviceService.ServiceProvider,
        Resources.UnitTestResources.ResourceManager
      );
      this.unitTestSpriteFont = this.contentManager.Load<SpriteFont>("UnitTestFont");
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.contentManager != null) {
        this.unitTestSpriteFont = null;

        this.contentManager.Dispose();
        this.contentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that the character opening locator can locate an opening between
    ///   two letter within the text
    /// </summary>
    [Test]
    public void TestLocateOpeningWithin() {
      OpeningLocator locator = new OpeningLocator();

      Vector2 helloSize = this.unitTestSpriteFont.MeasureString("he");
      int openingIndex = locator.FindClosestOpening(
        this.unitTestSpriteFont, "hello world", helloSize.X
      );

      Assert.AreEqual(2, openingIndex);
    }

    /// <summary>
    ///   Verifies that the character opening locator works if the searched location
    ///   lies before the text start
    /// </summary>
    [Test]
    public void TestLocateOpeningBefore() {
      OpeningLocator locator = new OpeningLocator();

      int openingIndex = locator.FindClosestOpening(
        this.unitTestSpriteFont, "hello world", -10.0f
      );

      Assert.AreEqual(0, openingIndex);
    }

    /// <summary>
    ///   Verifies that the character opening locator works if the searched location
    ///   lies after the text end
    /// </summary>
    [Test]
    public void TestLocateOpeningAfter() {
      OpeningLocator locator = new OpeningLocator();

      Vector2 helloSize = this.unitTestSpriteFont.MeasureString("hello world");
      int openingIndex = locator.FindClosestOpening(
        this.unitTestSpriteFont, "hello world", helloSize.X + 10.0f
      );

      Assert.AreEqual(11, openingIndex);
    }

    /// <summary>
    ///   Verifies that the character opening locator works if only a single character
    ///   should be scanned for an opening
    /// </summary>
    [Test]
    public void TestLocateOpeningOnSingleCharacter() {
      OpeningLocator locator = new OpeningLocator();

      Vector2 xSize = this.unitTestSpriteFont.MeasureString("X");

      int openingIndex = locator.FindClosestOpening(
        this.unitTestSpriteFont, "X", xSize.X / 3.0f
      );
      Assert.AreEqual(0, openingIndex);

      openingIndex = locator.FindClosestOpening(
        this.unitTestSpriteFont, "X", xSize.X / 3.0f * 2.0f
      );
      Assert.AreEqual(1, openingIndex);
    }

    /// <summary>Mocked graphics device used for the unit tests</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Content manager for loading the unit test assets</summary>
    private ResourceContentManager contentManager;
    /// <summary>Sprite font used in the unit test</summary>
    private SpriteFont unitTestSpriteFont;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat

#endif // UNITTEST
