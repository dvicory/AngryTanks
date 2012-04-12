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

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects.Water {

  /// <summary>Unit tests for the grid class</summary>
  [TestFixture]
  internal class WaterGridTest {

    /// <summary>
    ///   Verifies that the simple constructor of the Grid class is working
    /// </summary>
    [Test]
    public void TestSimpleConstructor() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        WaterGrid theGrid = new WaterGrid(
          mockGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f)
        );
        theGrid.Dispose();
      }
    }

    /// <summary>
    ///   Verifies that the full constructor of the Grid class is working
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        WaterGrid theGrid = new WaterGrid(
          mockGraphicsDeviceService.GraphicsDevice,
          new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
          20, 20
        );
        theGrid.Dispose();
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an invalid segment count for
    ///   the X axis is specified
    /// </summary>
    [Test]
    public void TestThrowOnInvalidSegmentCountX() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        Assert.Throws<ArgumentException>(
          delegate() {
            new WaterGrid(
              mockGraphicsDeviceService.GraphicsDevice, Vector2.Zero, Vector2.Zero, 0, 20
            );
          }
        );
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an invalid segment count for
    ///   the Y axis is specified
    /// </summary>
    [Test]
    public void TestThrowOnInvalidSegmentCountY() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        Assert.Throws<ArgumentException>(
          delegate() {
            new WaterGrid(
              mockGraphicsDeviceService.GraphicsDevice, Vector2.Zero, Vector2.Zero, 20, 0
            );
          }
        );
      }
    }

    /// <summary>
    ///   Verifies that the properties required for rendering the grid are working
    /// </summary>
    [Test]
    public void TestRenderingProperties() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        using(
          WaterGrid theGrid = new WaterGrid(
            mockGraphicsDeviceService.GraphicsDevice,
            new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
            20, 20
          )
        ) {
#if !XNA_4
          Assert.IsNotNull(theGrid.VertexDeclaration);
#endif
          Assert.IsNotNull(theGrid.VertexBuffer);
          Assert.IsNotNull(theGrid.IndexBuffer);
        }
      }
    }

    /// <summary>
    ///   Verifies that the statistical properties of the grid work as expected
    /// </summary>
    [Test]
    public void TestStatisticalProperties() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        using(
          WaterGrid theGrid = new WaterGrid(
            mockGraphicsDeviceService.GraphicsDevice,
            new Vector2(-10.0f, -10.0f), new Vector2(10.0f, 10.0f),
            4, 4
          )
        ) {
          Assert.AreEqual(PrimitiveType.TriangleStrip, theGrid.PrimitiveType);
          Assert.AreEqual(25, theGrid.VertexCount); // 4x4 segments = 5x5 vertices
          Assert.AreEqual(37, theGrid.IndexCount); // pick a pen & paper and check it...
          Assert.AreEqual(35, theGrid.PrimitiveCount); // 8 per row, 3 degenerate polys
        }
      }
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Water

#endif // UNITTEST
