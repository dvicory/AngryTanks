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

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.Geometry.Volumes {

  /// <summary>Test for the three-dimensional box implementation</summary>
  [TestFixture]
  public class Cylinder3Test {

    /// <summary>Tests whether the mass properties of the volume are working</summary>
    [Test]
    public void TestMassProperties() {
      Cylinder3 testCylinder = new Cylinder3(
        Matrix.CreateTranslation(new Vector3(100.0f, 200.0f, 300.0f)), 10.0f, 20.0f
      );

      Assert.AreEqual(
        new Vector3(100.0f, 200.0f, 300.0f), testCylinder.CenterOfMass,
        "Center of mass is correctly positioned"
      );

      // Formula for cylinder surface area: 2 * pi * (r ^ 2) + 2 * pi * r * h
      Assert.AreEqual(
        1884.9555921538759430775860299677f, testCylinder.SurfaceArea,
        Specifications.MaximumDeviation, "Surface area of cylinder is exactly determined"
      );

      // Formula for cylinder volume: pi * (r ^ 2) * h
      Assert.AreEqual(
        6283.185307179586476925286766559f, testCylinder.Mass,
        Specifications.MaximumDeviation, "Mass of cylinder is exactly determined"
      );
    }

    /// <summary>Tests the bounding box generator</summary>
    [Test]
    public void TestBoundingBox() {
      // TODO: Implement bounding box determination for cylinder volumes
      /*
      Cylinder3 testCylinder = new Cylinder3(
        Matrix.CreateTranslation(new Vector3(100.0f, 200.0f, 300.0f)), 10.0f, 20.0f
      );
      AxisAlignedBox3 boundingBox = testCylinder.BoundingBox;

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
      GeoAssert.AreEqual(
        expectedBoundingBox, box.BoundingBox,
        Specifications.MaximumDeviation,
        "Bounding box for oriented box is correctly determined"
      );
      */
    }

    /// <summary>Tests the bounding sphere generator</summary>
    [Test]
    public void TestBoundingSphere() {
      Cylinder3 testCylinder = new Cylinder3(
        Matrix.CreateTranslation(new Vector3(100.0f, 200.0f, 300.0f)), 10.0f, 20.0f
      );
      Sphere3 boundingSphere = testCylinder.BoundingSphere;

      GeoAssertHelper.AreAlmostEqual(
        new Vector3(100.0f, 200.0f, 300.0f), boundingSphere.Center,
        Specifications.MaximumDeviation, "Center of bounding sphere correctly determined"
      );

      Assert.That(
        boundingSphere.Radius,
        Is.EqualTo(14.142135623730950488016887242097f).Within(
          Specifications.MaximumDeviation
        ).Ulps,
        "Radius of bounding sphere exactly encloses the box"
      );
    }
  }

} // namespace Nuclex.Geometry.Volumes

#endif // UNITTEST