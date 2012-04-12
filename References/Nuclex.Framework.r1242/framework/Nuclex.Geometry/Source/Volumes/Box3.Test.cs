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

namespace Nuclex.Geometry.Volumes {

  /// <summary>Test for the three-dimensional box implementation</summary>
  [TestFixture]
  public class Box3Test {

    /// <summary>Tests whether the mass properties of the volume are working</summary>
    [Test]
    public void TestMassProperties() {
      Box3 testBox = new Box3(
        Matrix.CreateTranslation(105.0f, 110.0f, 115.0f),
        new Vector3(5.0f, 10.0f, 15.0f) // these are extents, not dimensions!
      );

      Assert.AreEqual(
        new Vector3(105.0f, 110.0f, 115.0f), testBox.CenterOfMass,
        "Center of mass is correctly positioned"
      );
      Assert.AreEqual(6000.0f, testBox.Mass, "Mass of box is exactly determined");
      Assert.AreEqual(2200.0f, testBox.SurfaceArea, "Surface area of box is exactly determined");

    }

    /// <summary>Tests the bounding box generator</summary>
    [Test]
    public void TestBoundingBox() {
      Volumes.Box3 box = new Volumes.Box3(
        MatrixHelper.Create(
          new Vector3(15.0f, 15.0f, 15.0f), 
          Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f)),
          Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f)),
          Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f))
        ),
        new Vector3(5.0f, 5.0f, 5.0f)
      );

      float growth = (float)Math.Sqrt(75.0f);

      AxisAlignedBox3 expectedBoundingBox = new AxisAlignedBox3(
        new Vector3(15.0f - growth, 15.0f - growth, 15.0f - growth),
        new Vector3(15.0f + growth, 15.0f + growth, 15.0f + growth)
      );
      GeoAssertHelper.AreAlmostEqual(
        expectedBoundingBox, box.BoundingBox,
        Specifications.MaximumDeviation,
        "Bounding box for oriented box is correctly determined"
      );
    }
    /*
      /// <summary>Tests the bounding sphere generator</summary>
      [ Test ]
      public void TestBoundingSphere() {
        AxisAlignedBox3 box = new AxisAlignedBox3(
          new Vector3(10.0f, 10.0f, 10.0f), new Vector3(20.0f, 20.0f, 20.0f)
        );

        Sphere3 boundingSphere = box.BoundingSphere;
        Assert.AreEqual(
          new Vector3(15.0f, 15.0f, 15.0f), boundingSphere.Center,
          "Center of bounding sphere correctly determined"
        );
        Assert.AreEqual(
          Math.Sqrt(75.0f), boundingSphere.Radius,
          "Radius of bounding sphere exactly encloses the box"
        );
      }
    */
  }

} // namespace Nuclex.Geometry.Volumes

#endif // UNITTEST