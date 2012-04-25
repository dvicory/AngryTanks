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

using TestVertex = Microsoft.Xna.Framework.Graphics.VertexPositionColor;

namespace Nuclex.Graphics.Debugging {

  /// <summary>Unit tests for the solid box vertex generator</summary>
  [TestFixture]
  internal class SolidBoxVertexGeneratorTest {

    /// <summary>Verifies that the vertex generator can create a box</summary>
    [Test]
    public void TestBoxGeneration() {
      int count = SolidBoxVertexGenerator.VertexCount;
      VertexPositionColor[] vertices = new VertexPositionColor[count + 1];

      SolidBoxVertexGenerator.Generate(
        vertices, 1,
        new Vector3(10.0f, 20.0f, 30.0f), new Vector3(20.0f, 25.0f, 32.5f),
        Color.Blue
      );

      Assert.AreEqual(Vector3.Zero, vertices[0].Position);
      Assert.AreNotEqual(Vector3.Zero, vertices[1].Position);
      Assert.AreNotEqual(Vector3.Zero, vertices[count - 1].Position);
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
