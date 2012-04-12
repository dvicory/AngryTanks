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

using NUnit.Framework;

using Vector2i = Nuclex.Math.Vector2<int, Nuclex.Math.Generic.CheckedScalarMath>;
using Vector2f = Nuclex.Math.Vector2<float, Nuclex.Math.Generic.CheckedScalarMath>;

namespace Nuclex.Math {

  /// <summary>Test for the vector2 class</summary>
  [TestFixture]
  public class Vector2Test {

    /// <summary>Checks the vector constants for correctness</summary>
    [Test]
    public void TestVectorConstants() {
      Vector2i up = Vector2i.Up;

      Assert.AreEqual(0, up.X, "Vector2.Up pointing upwards");
      Assert.AreEqual(1, up.Y, "Vector2.Up pointing upwards");

      Vector2i right = Vector2i.Right;

      Assert.AreEqual(1, right.X, "Vector2.Right pointing right");
      Assert.AreEqual(0, right.Y, "Vector2.Right pointing right");

      Vector2i zero = Vector2i.Zero;

      Assert.AreEqual(0, zero.X, "Vector2.Zero is zero");
      Assert.AreEqual(0, zero.Y, "Vector2.Zero is zero");

      Vector2i one = Vector2i.One;

      Assert.AreEqual(1, one.X, "Vector2.One is one");
      Assert.AreEqual(1, one.Y, "Vector2.One is one");

    }

    /// <summary>Tests the orientation of the perpendicular vector</summary>
    [Test]
    public void TestPerpendicularity() {
      Vector2i right = Vector2i.Up.Perpendicular;

      Assert.AreEqual(1, right.X, "Perpendicular vector pointing right");
      Assert.AreEqual(0, right.Y, "Perpendicular vector pointing right");

      Vector2i down = Vector2i.Right.Perpendicular;

      Assert.AreEqual(0, down.X, "Perpendicular vector pointing down");
      Assert.AreEqual(-1, down.Y, "Perpendicular vector pointing down");
    }

  }

} // namespace Nuclex.Math

#endif // UNITTEST
