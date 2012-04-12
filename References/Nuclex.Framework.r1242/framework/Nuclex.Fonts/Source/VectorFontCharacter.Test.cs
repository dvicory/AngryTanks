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

using KerningEntry = System.Collections.Generic.KeyValuePair<
  Nuclex.Fonts.VectorFont.KerningPair, Microsoft.Xna.Framework.Vector2
>;

namespace Nuclex.Fonts {

  /// <summary>Unit tests for the vector font character class</summary>
  [TestFixture]
  public class VectorFontCharacterTest {

    /// <summary>Verifies that the tested character has a valid advancement</summary>
    [Test]
    public void TestAdvancement() {
      Assert.Greater(this.vectorCharacter.Advancement.X, 0);
    }

    /// <summary>Verifies that the tested character has a face list</summary>
    [Test]
    public void TestFaces() {
      Assert.Greater(this.vectorCharacter.Faces.Count, 0);
    }

    /// <summary>Verifies that the tested character has an outline list</summary>
    [Test]
    public void TestOutlines() {
      Assert.Greater(this.vectorCharacter.Outlines.Count, 0);
    }

    /// <summary>Verifies that the tested character has vertices</summary>
    [Test]
    public void TestVertices() {
      Assert.Greater(this.vectorCharacter.Vertices.Count, 0);
    }

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();
      this.mockedGraphicsDeviceService.CreateDevice();

      this.contentManager = new ResourceContentManager(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
          this.mockedGraphicsDeviceService
        ),
        Resources.UnitTestResources.ResourceManager
      );
      this.vectorFont = this.contentManager.Load<VectorFont>("UnitTestVectorFont");

      char character = getFirstVisibleCharacter();
      int characterIndex = this.vectorFont.CharacterMap[character];
      this.vectorCharacter = this.vectorFont.Characters[characterIndex];
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.contentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>Retrieves the first visible character in the font</summary>
    /// <returns>The first visible character in the font</returns>
    private char getFirstVisibleCharacter() {
      foreach(KeyValuePair<char, int> character in this.vectorFont.CharacterMap) {
        int index = character.Value;

        if(this.vectorFont.Characters[index].Outlines.Count > 0) {
          return character.Key;
        }
      }

      throw new InvalidOperationException("No visible characters found");
    }

    /// <summary>Mocked graphics device service used for unit testing</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Content manager used to load the vector font</summary>
    private ResourceContentManager contentManager;
    /// <summary>Vector font the tested character is taken from</summary>
    private VectorFont vectorFont;
    /// <summary>Vector font character being tested</summary>
    private VectorFontCharacter vectorCharacter;

  }

} // namespace Nuclex.Fonts

#endif // UNITTEST
