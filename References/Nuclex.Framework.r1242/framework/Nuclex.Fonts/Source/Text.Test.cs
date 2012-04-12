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
using Nuclex.Graphics.Batching;

namespace Nuclex.Fonts {

  /// <summary>Unit tests for the text class</summary>
  [TestFixture]
  public class TextTest {

    #region class TestText
    
    /// <summary>Test implemented of a text mesh for the unit test</summary>
    private class TestText : Text {
    
      /// <summary>Initializes a new test text mesh</summary>
      public TestText() {
        this.vertices = new VertexPositionNormalTexture[123];
        this.indices = new short[1234];
        this.primitiveType = PrimitiveType.TriangleList;
        this.width = 12.34f;
        this.height = 56.78f;
      }
    
    }

    #endregion // class TestText

    /// <summary>Tests whether the Vertices property works correctly</summary>
    [Test]
    public void TestVerticesProperty() {
      Text test = new TestText();
      Assert.AreEqual(123, test.Vertices.Length);
    }

    /// <summary>Tests whether the Indices property works correctly</summary>
    [Test]
    public void TestIndicesProperty() {
      Text test = new TestText();
      Assert.AreEqual(1234, test.Indices.Length);
    }

    /// <summary>Tests whether the PrimitiveType property works correctly</summary>
    [Test]
    public void TestPrimitiveTypeProperty() {
      Text test = new TestText();
      Assert.AreEqual(PrimitiveType.TriangleList, test.PrimitiveType);
    }

    /// <summary>Tests whether the Width property works correctly</summary>
    [Test]
    public void TestWidthProperty() {
      Text test = new TestText();
      Assert.AreEqual(12.34f, test.Width);
    }

    /// <summary>Tests whether the Height property works correctly</summary>
    [Test]
    public void TestHeightProperty() {
      Text test = new TestText();
      Assert.AreEqual(56.78f, test.Height);
    }

  }

} // namespace Nuclex.Fonts

#endif // UNITTEST
