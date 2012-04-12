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

using Nuclex.Graphics;
using Nuclex.Testing.Xna;

namespace Nuclex.Fonts {

  /// <summary>Unit tests for the text batcher</summary>
  [TestFixture]
  public class TextBatchTest {

    #region class TestText

    /// <summary>Test implemented of a text mesh for the unit test</summary>
    private class TestText : Text {

      /// <summary>Initializes a new test text mesh</summary>
      public TestText() {
        this.vertices = new VertexPositionNormalTexture[12];
        this.indices = new short[12];
        this.primitiveType = PrimitiveType.TriangleList;
        this.width = 12.34f;
        this.height = 56.78f;
      }

    }

    #endregion // class TestText

    /// <summary>Initializes a test</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(DeviceType.Reference);
      this.mockedGraphicsDeviceService.CreateDevice();

      this.textBatch = new TextBatch(this.mockedGraphicsDeviceService.GraphicsDevice);
    }

    /// <summary>Finalizes the resources used during the test</summary>
    [TearDown]
    public void Teardown() {
      if(this.textBatch != null) {
        this.textBatch.Dispose();
        this.textBatch = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>Verifies that instances of the effect can be created</summary>
    [Test]
    public void TestConstructor() {
      Assert.IsNotNull(this.textBatch);
    }


    /// <summary>
    ///   Verifies that the view/projection matrix is saved by the text batcher
    /// </summary>
    [Test]
    public void TestViewProjectionMatrix() {
      Matrix testMatrix = new Matrix(
        1.1f, 1.2f, 1.3f, 1.4f,
        2.1f, 2.2f, 2.3f, 2.4f,
        3.1f, 3.2f, 3.3f, 3.4f,
        4.1f, 4.2f, 4.3f, 4.4f
      );

      this.textBatch.ViewProjection = testMatrix;
      Assert.AreEqual(testMatrix, this.textBatch.ViewProjection);
    }

    /// <summary>
    ///   Tests whether the Begin() and End() methods can be called without any
    ///   drawing commands inbetween
    /// </summary>
    [Test]
    public void TestBeginEnd() {
      this.textBatch.Begin();
      this.textBatch.End();
    }

    /// <summary>
    ///   Tests the text drawing method using the default transformation matrix
    /// </summary>
    [Test]
    public void TestDrawTextWithDefaultTransform() {
      TestText test = new TestText();

      this.textBatch.Begin();
      try {
        this.textBatch.DrawText(test, Color.White);
      }
      finally {
        this.textBatch.End();
      }
    }

    /// <summary>
    ///   Tests the text drawing method using a custom transformation matrix
    /// </summary>
    [Test]
    public void TestDrawTextWithCustomTransform() {
      TestText test = new TestText();
      Matrix testMatrix = new Matrix(
        1.1f, 1.2f, 1.3f, 1.4f,
        2.1f, 2.2f, 2.3f, 2.4f,
        3.1f, 3.2f, 3.3f, 3.4f,
        4.1f, 4.2f, 4.3f, 4.4f
      );

      this.textBatch.Begin();
      try {
        this.textBatch.DrawText(test, testMatrix, Color.White);
      }
      finally {
        this.textBatch.End();
      }
    }

    /// <summary>
    ///   Tests the text drawing method using a custom effect
    /// </summary>
    [Test]
    public void TestDrawTextWithCustomEffect() {
      TestText test = new TestText();
      BasicEffect effect = new BasicEffect(
        this.mockedGraphicsDeviceService.GraphicsDevice
#if !XNA_4
        , null
#endif
      );
      try {

        this.textBatch.Begin();
        try {
          this.textBatch.DrawText(test, effect);
        }
        finally {
          this.textBatch.End();
        }
      }
      finally {
        effect.Dispose();
      }
    }

    /// <summary>Graphics device service used for the test</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;

    /// <summary>Textbatch being tested</summary>
    private TextBatch textBatch;

  }

} // namespace Nuclex.Fonts

#endif // UNITTEST
