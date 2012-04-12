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

using System;

using Microsoft.Xna.Framework;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Geometry.Areas {

  /// <summary>Test for the three-dimensional triangle implementation</summary>
  [TestFixture]
  public class Triangle3Test {

    /// <summary>Tests whether the mass properties of the volume are working</summary>
    [Test]
    public void TestMassProperties() {
      Triangle3 testTriangle = new Triangle3(
        new Vector3(100.0f, 100.0f, 100.0f),
        new Vector3(110.0f, 100.0f, 110.0f),
        new Vector3(105.0f, 110.0f, 100.0f)
      );

      Assert.AreEqual(
        new Vector3(105.0f, 103.33333333333333f, 103.33333333333333f), testTriangle.CenterOfMass,
        "Center of mass is correctly positioned"
      );

      Assert.AreEqual(
        74.999999999999972f, testTriangle.Area,
        "Mass of triangle is exactly determined"
      );
      Assert.AreEqual(
        23.45207879911715f, testTriangle.CircumferenceLength,
        "Surface area of triangle is exactly determined"
      );

    }

  }

} // namespace Nuclex.Geometry.Areas

#endif // UNITTEST