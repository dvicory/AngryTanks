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

using PolarVector2i = Nuclex.Math.PolarVector2<int, Nuclex.Math.Generic.CheckedScalarMath>;
using PolarVector2f = Nuclex.Math.PolarVector2<float, Nuclex.Math.Generic.CheckedScalarMath>;

namespace Nuclex.Math {

  /// <summary>Test for the 2D polar vector class</summary>
  [TestFixture]
  public class PolarVector2Test {

    /// <summary>Checks the vector constants for correctness</summary>
    [Test]
    public void TestVectorConstants() {

      PolarVector2i up = PolarVector2i.Up;

      Assert.AreEqual(
        System.Math.PI / 2.0, up.Phi, double.Epsilon,
        "PolarVector2.Up pointing upwards"
      );
      Assert.AreEqual(1, up.Radius, "PolarVector2.Up pointing upwards");

      PolarVector2i right = PolarVector2i.Right;

      Assert.AreEqual(0.0, right.Phi, "PolarVector2.Right pointing right");
      Assert.AreEqual(1, right.Radius, "PolarVector2.Right pointing right");

      PolarVector2i zero = PolarVector2i.Zero;

      Assert.AreEqual(0.0, zero.Phi, "PolarVector2.Zero is zero");
      Assert.AreEqual(0, zero.Radius, "PolarVector2.Zero is zero");

    }

  }

} // namespace Nuclex.Math

#endif // UNITTEST