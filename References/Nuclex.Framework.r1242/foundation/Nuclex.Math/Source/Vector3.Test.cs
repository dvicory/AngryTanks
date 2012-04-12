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

using Vector3i = Nuclex.Math.Vector3<int, Nuclex.Math.Generic.CheckedScalarMath>;
using Vector3f = Nuclex.Math.Vector3<float, Nuclex.Math.Generic.CheckedScalarMath>;

namespace Nuclex.Math {

  /// <summary>Test for the vector3 class</summary>
  [TestFixture]
  public class Vector3Test {

    /// <summary>Checks the vector constants for correctness</summary>
    [Test]
    public void TestVectorConstants() {
      Vector3i up = Vector3i.Up;

      Assert.AreEqual(0, up.X, "Vector3.Up pointing upwards");
      Assert.AreEqual(1, up.Y, "Vector3.Up pointing upwards");
      Assert.AreEqual(0, up.Z, "Vector3.Up pointing upwards");

      Vector3i right = Vector3i.Right;

      Assert.AreEqual(1, right.X, "Vector3.Right pointing right");
      Assert.AreEqual(0, right.Y, "Vector3.Right pointing right");
      Assert.AreEqual(0, right.Z, "Vector3.Right pointing right");

      Vector3i into = Vector3i.In;

      Assert.AreEqual(0, into.X, "Vector3.In pointing in");
      Assert.AreEqual(0, into.Y, "Vector3.In pointing in");
      Assert.AreEqual(1, into.Z, "Vector3.In pointing in");

      Vector3i zero = Vector3i.Zero;

      Assert.AreEqual(0, zero.X, "Vector3.Zero is zero");
      Assert.AreEqual(0, zero.Y, "Vector3.Zero is zero");
      Assert.AreEqual(0, zero.Z, "Vector3.Zero is zero");

      Vector3i one = Vector3i.One;

      Assert.AreEqual(1, one.X, "Vector3.One is one");
      Assert.AreEqual(1, one.Y, "Vector3.One is one");
      Assert.AreEqual(1, one.Z, "Vector3.One is one");

    }

  }

} // namespace Nuclex.Math

#endif // UNITTEST
