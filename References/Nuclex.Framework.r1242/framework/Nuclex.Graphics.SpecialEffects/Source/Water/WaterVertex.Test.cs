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

namespace Nuclex.Graphics.SpecialEffects.Water {

  /// <summary>Unit tests for the water vertex class</summary>
  [TestFixture]
  internal class SkyboxVertexTest {

    /// <summary>
    ///   Verifies that the constructor of the water vertex class is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      WaterVertex vertex = new WaterVertex(
        new Vector3(1.0f, 2.0f, 3.0f), new Vector2(4.0f, 5.0f)
      );

      Assert.AreEqual(1.0f, vertex.Position.X);
      Assert.AreEqual(2.0f, vertex.Position.Y);
      Assert.AreEqual(3.0f, vertex.Position.Z);
      Assert.AreEqual(4.0f, vertex.TextureCoordinate.X);
      Assert.AreEqual(5.0f, vertex.TextureCoordinate.Y);
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Water

#endif // UNITTEST
