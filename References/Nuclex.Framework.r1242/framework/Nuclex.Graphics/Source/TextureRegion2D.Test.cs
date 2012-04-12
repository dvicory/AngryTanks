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
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics {

  /// <summary>Unit tests for the texture region class</summary>
  [TestFixture]
  internal class TextureRegion2DTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();
      this.mockedGraphicsDeviceService.CreateDevice();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Tests whether the minimal constructor of the texture region works
    /// </summary>
    [Test]
    public void TestConstructor() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region = new TextureRegion2D(testTexture);

        Assert.AreSame(testTexture, region.Texture);
        Assert.AreEqual(Vector2.Zero, region.Min);
        Assert.AreEqual(Vector2.One, region.Max);
      }
    }

    /// <summary>
    ///   Test whether the explicit texture coordinate constructor is working
    /// </summary>
    [Test]
    public void TestFloatConstructor() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region = new TextureRegion2D(testTexture, 0.1f, 0.2f, 0.3f, 0.4f);

        Assert.AreSame(testTexture, region.Texture);
        Assert.AreEqual(new Vector2(0.1f, 0.2f), region.Min);
        Assert.AreEqual(new Vector2(0.3f, 0.4f), region.Max);
      }
    }

    /// <summary>
    ///   Verifies that a texture region can be built from a texel values
    /// </summary>
    [Test]
    public void TestFromTexelInts() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 256
        )
      ) {
        TextureRegion2D region = TextureRegion2D.FromTexels(
          testTexture, 16, 32, 96, 112
        );

        Vector2 min = new Vector2(16.0f / 128.0f, 32.0f / 256.0f);
        Vector2 max = new Vector2(96.0f / 128.0f, 112.0f / 256.0f);

        Assert.AreSame(testTexture, region.Texture);
        Assert.AreEqual(min, region.Min);
        Assert.AreEqual(max, region.Max);
      }
    }

    /// <summary>
    ///   Verifies that a texture region can be built from a texel point structures
    /// </summary>
    [Test]
    public void TestFromTexelPoints() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 256
        )
      ) {
        TextureRegion2D region = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(96, 112)
        );

        Vector2 min = new Vector2(16.0f / 128.0f, 32.0f / 256.0f);
        Vector2 max = new Vector2(96.0f / 128.0f, 112.0f / 256.0f);

        Assert.AreSame(testTexture, region.Texture);
        Assert.AreEqual(min, region.Min);
        Assert.AreEqual(max, region.Max);
      }
    }

    /// <summary>
    ///   Tests whether two differing instances produce different hash codes
    /// </summary>
    [Test]
    public void TestHashCodeOnDifferingInstances() {
      using(
        Texture2D testTexture1 = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 256
        )
      ) {
        using(
          Texture2D testTexture2 = new Texture2D(
            this.mockedGraphicsDeviceService.GraphicsDevice,
            256, 128
          )
        ) {
          TextureRegion2D region1 = TextureRegion2D.FromTexels(
            testTexture1, new Point(16, 32), new Point(48, 64)
          );
          TextureRegion2D region2 = TextureRegion2D.FromTexels(
            testTexture1, new Point(80, 96), new Point(112, 128)
          );

          Assert.AreNotEqual(region1.GetHashCode(), region2.GetHashCode());
        }
      }
    }

    /// <summary>
    ///   Tests whether two equivalent instances produce an identical hash code
    /// </summary>
    [Test]
    public void TestHashCodeOnEquivalentInstances() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region1 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );
        TextureRegion2D region2 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );

        Assert.AreEqual(region1.GetHashCode(), region2.GetHashCode());
      }
    }

    /// <summary>Tests the Equals() method performing a comparison against null</summary>
    [Test]
    public void TestEqualsOnNull() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );

        Assert.IsFalse(region.Equals(null));
      }
    }

    /// <summary>Tests the Equals() method comparing two equal instances</summary>
    [Test]
    public void TestEqualsWithEquivalentInstances() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region1 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );
        TextureRegion2D region2 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );

        Assert.IsTrue(region1.Equals(region2));
      }
    }

    /// <summary>Tests the Equals() method comparing two differing instances</summary>
    [Test]
    public void TestEqualsWithDifferingInstances() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region1 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );
        TextureRegion2D region2 = TextureRegion2D.FromTexels(
          testTexture, new Point(80, 96), new Point(112, 128)
        );

        Assert.IsFalse(region1.Equals(region2));
      }
    }

    /// <summary>Tests the equality operator with differing instances</summary>
    [Test]
    public void TestEqualityOnDifferingInstances() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region1 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );
        TextureRegion2D region2 = TextureRegion2D.FromTexels(
          testTexture, new Point(80, 96), new Point(112, 128)
        );

        Assert.IsFalse(region1 == region2);
      }
    }

    /// <summary>Tests the equality operator with equivalent instances</summary>
    [Test]
    public void TestEqualityOnEquivalentInstances() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region1 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );
        TextureRegion2D region2 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );

        Assert.IsTrue(region1 == region2);
      }
    }

    /// <summary>Tests the inequality operator with differing instances</summary>
    [Test]
    public void TestInequalityOnDifferingInstances() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region1 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );
        TextureRegion2D region2 = TextureRegion2D.FromTexels(
          testTexture, new Point(80, 96), new Point(112, 128)
        );

        Assert.IsTrue(region1 != region2);
      }
    }

    /// <summary>Tests the inequality operator with equivalent instances</summary>
    [Test]
    public void TestInequalityOnEquivalentInstances() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region1 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );
        TextureRegion2D region2 = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );

        Assert.IsFalse(region1 != region2);
      }
    }

    /// <summary>Tests the ToString() method of the string segment</summary>
    [Test]
    public void TestToString() {
      using(
        Texture2D testTexture = new Texture2D(
          this.mockedGraphicsDeviceService.GraphicsDevice,
          128, 128
        )
      ) {
        TextureRegion2D region = TextureRegion2D.FromTexels(
          testTexture, new Point(16, 32), new Point(48, 64)
        );

        Assert.IsNotNull(region.ToString());
      }
    }

    /// <summary>Mocked graphics service used to run the unit tests</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
