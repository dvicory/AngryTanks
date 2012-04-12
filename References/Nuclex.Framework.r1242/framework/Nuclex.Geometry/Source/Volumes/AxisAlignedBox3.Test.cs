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

  /// <summary>Test for the three-dimensional axis aligned box implementation</summary>
  [TestFixture]
  public class AxisAlignedBox3Test {

    /// <summary>Tests whether the mass properties of the volume are working</summary>
    [Test]
    public void TestMassProperties() {
      AxisAlignedBox3 testBox =
        new AxisAlignedBox3(new Vector3(100, 100, 100), new Vector3(110, 120, 130));

      Assert.AreEqual(
        new Vector3(105, 110, 115), testBox.CenterOfMass,
        "Center of mass is correctly positioned"
      );
      Assert.AreEqual(6000, testBox.Mass, "Mass of box is exactly determined");
      Assert.AreEqual(2200, testBox.SurfaceArea, "Surface area of box is exactly determined");

    }

    /// <summary>Tests the intersection query on moving boxes</summary>
    [Test]
    public void TestMovingBoxImpactPoint() {
      AxisAlignedBox3 leftBox = new AxisAlignedBox3(
        new Vector3(0.0f, 0.0f, 0.0f), new Vector3(10.0f, 10.0f, 10.0f)
      );
      AxisAlignedBox3 rightBox = new AxisAlignedBox3(
        new Vector3(100.0f, 0.0f, 0.0f), new Vector3(110.0f, 10.0f, 10.0f)
      );
/*
      Assert.AreEqual(
        new float[] { 0.0f },
        leftBox.LocateImpact(new Vector3(0.0f, 0.0f, 0.0f), leftBox),
        "Contact with clone occured immediately"
      );

      Assert.AreEqual(
        new float[] { 0.45f },
        leftBox.LocateImpact(new Vector3(200.0f, 0.0f, 0.0f), rightBox),
        "Fast moving contact with right box is determined exactly"
      );

      Assert.AreEqual(
        new float[] { 0.45f },
        rightBox.LocateImpact(new Vector3(-200.0f, 0.0f, 0.0f), leftBox),
        "Fast moving contact with left box is determined exactly"
      );
 */
    }

    /// <summary>Tests the bounding box generator</summary>
    [Test]
    public void TestBoundingBox() {
      AxisAlignedBox3 box = new AxisAlignedBox3(
        new Vector3(10.0f, 10.0f, 10.0f), new Vector3(20.0f, 20.0f, 20.0f)
      );

      AxisAlignedBox3 boundingBox = box.BoundingBox;
      Assert.AreEqual(
        box, boundingBox,
        "Bounding box for axis aligned box is identical to the box itself"
      );
    }

    /// <summary>Tests the bounding sphere generator</summary>
    [Test]
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
        (float)Math.Sqrt(75.0f), boundingSphere.Radius,
        "Radius of bounding sphere exactly encloses the box"
      );
    }

  }

} // namespace Nuclex.Geometry.Volumes

#endif // UNITTEST