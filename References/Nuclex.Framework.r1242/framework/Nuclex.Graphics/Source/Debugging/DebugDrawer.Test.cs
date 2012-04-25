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

using TestVertex = Microsoft.Xna.Framework.Graphics.VertexPositionColor;

namespace Nuclex.Graphics.Debugging {

  /// <summary>Unit tests for the debug drawer</summary>
  [TestFixture]
  internal class DebugDrawerTest {

    /// <summary>
    ///   Tests whether an instance of the debug drawer can be constructed
    /// </summary>
    [Test]
    public void TestConstructor() {
      Assert.IsNotNull(this.debugDrawer); // nonsense ;-)
    }

    /// <summary>
    ///   Verifies that the view/projection matrix is saved by the debug drawer
    /// </summary>
    [Test]
    public void TestViewProjectionMatrix() {
      Matrix testMatrix = new Matrix(
        1.1f, 1.2f, 1.3f, 1.4f,
        2.1f, 2.2f, 2.3f, 2.4f,
        3.1f, 3.2f, 3.3f, 3.4f,
        4.1f, 4.2f, 4.3f, 4.4f
      );

      this.debugDrawer.ViewProjection = testMatrix;
      Assert.AreEqual(testMatrix, this.debugDrawer.ViewProjection);
    }

    /// <summary>Tests the DrawLine() method of the debug drawer</summary>
    [Test]
    public void TestDrawLinesToOverflow() {
      int linesRequired = DebugDrawer.MaximumDebugVertexCount / 2;
      
      for(int line = 0; line <= linesRequired; ++line) {
        this.debugDrawer.DrawLine(Vector3.Zero, Vector3.One, Color.White);
      }
      
      // No exception means success
    }

    /// <summary>Tests the DrawTriangle() method of the debug drawer</summary>
    [Test]
    public void TestDrawTrianglesToOverflow() {
      int trianglesRequired = DebugDrawer.MaximumDebugVertexCount / 3;

      for(int triangle = 0; triangle <= trianglesRequired; ++triangle) {
        this.debugDrawer.DrawTriangle(
          Vector3.Zero, Vector3.UnitX, Vector3.UnitY, Color.White
        );
      }

      // No exception means success
    }

    /// <summary>Tests the DrawSolidTriangle() method of the debug drawer</summary>
    [Test]
    public void TestDrawSolidTrianglesToOverflow() {
      int trianglesRequired = DebugDrawer.MaximumDebugVertexCount / 3;

      for(int triangle = 0; triangle <= trianglesRequired; ++triangle) {
        this.debugDrawer.DrawSolidTriangle(
          Vector3.Zero, Vector3.UnitX, Vector3.UnitY, Color.White
        );
      }

      // No exception means success
    }

    /// <summary>Tests the DrawBox() method of the debug drawer</summary>
    [Test]
    public void TestDrawBoxesToOverflow() {
      int boxesRequired = DebugDrawer.MaximumDebugVertexCount / 24;

      for(int box = 0; box <= boxesRequired; ++box) {
        this.debugDrawer.DrawBox(-Vector3.One, Vector3.One, Color.White);
      }

      // No exception means success
    }

    /// <summary>Tests the DrawSolidBox() method of the debug drawer</summary>
    [Test]
    public void TestDrawSolidBoxesToOverflow() {
      int boxesRequired = DebugDrawer.MaximumDebugVertexCount / 36;

      for(int box = 0; box <= boxesRequired; ++box) {
        this.debugDrawer.DrawSolidBox(-Vector3.One, Vector3.One, Color.White);
      }

      // No exception means success
    }

    /// <summary>Tests the DrawArrow() method of the debug drawer</summary>
    [Test]
    public void TestDrawArrowsToOverflow() {
      int arrowsRequired = DebugDrawer.MaximumDebugVertexCount / 10;

      for(int arrow = 0; arrow <= arrowsRequired; ++arrow) {
        this.debugDrawer.DrawArrow(Vector3.Zero, Vector3.Forward, Color.White);
      }

      // No exception means success
    }

    /// <summary>Tests the DrawSolidArrow() method of the debug drawer</summary>
    [Test]
    public void TestDrawSolidArrowsToOverflow() {
      int arrowsRequired = DebugDrawer.MaximumDebugVertexCount / 144;

      for(int arrow = 0; arrow <= arrowsRequired; ++arrow) {
        this.debugDrawer.DrawSolidArrow(Vector3.Zero, Vector3.Forward, Color.White);
      }

      // No exception means success
    }

    /// <summary>Tests the DrawString() method of the debug drawer</summary>
    [Test]
    public void TestDrawStrings() {
      for(int index = 0; index <= 10; ++index) {
        this.debugDrawer.DrawString(Vector2.Zero, "Hello World", Color.White);
      }

      // No exception means success
    }

    /// <summary>Tests the Draw() method of the debug drawer</summary>
    [Test]
    public void TestRenderGeometry() {
      this.debugDrawer.DrawLine(Vector3.Zero, Vector3.One, Color.White);
      this.debugDrawer.DrawTriangle(
        Vector3.Zero, Vector3.UnitX, Vector3.UnitY, Color.White
      );
      this.debugDrawer.DrawSolidTriangle(
        Vector3.Zero, Vector3.UnitX, Vector3.UnitY, Color.White
      );
      this.debugDrawer.DrawBox(-Vector3.One, Vector3.One, Color.White);
      this.debugDrawer.DrawSolidBox(-Vector3.One, Vector3.One, Color.White);
      this.debugDrawer.DrawArrow(Vector3.Zero, Vector3.Forward, Color.White);
      this.debugDrawer.DrawSolidArrow(Vector3.Zero, Vector3.Forward, Color.White);
      this.debugDrawer.DrawString(Vector2.Zero, "Hello World", Color.White);
      TestDrawSolidArrowsToOverflow(); // fill the buffer

      this.debugDrawer.Draw(new GameTime());
    }

    /// <summary>Initializes a test</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(DeviceType.Reference);
      this.mockedGraphicsDeviceService.CreateDevice();
      this.debugDrawer = new DebugDrawer(this.mockedGraphicsDeviceService);
    }
    
    /// <summary>Finalizes the resources used during the test</summary>
    [TearDown]
    public void Teardown() {
      if(this.debugDrawer != null) {
        this.debugDrawer.Dispose();
        this.debugDrawer = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>Mocked graphics device service used by the test</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Debug drawer being tested</summary>
    private DebugDrawer debugDrawer;

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
