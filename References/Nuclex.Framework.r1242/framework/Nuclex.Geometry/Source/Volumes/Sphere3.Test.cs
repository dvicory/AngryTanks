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

  /// <summary>Test for the sphere (3D) implementation</summary>
  [TestFixture]
  public class Sphere3Test {

    /// <summary>Tests whether the mass properties of the volume are working</summary>
    [Test]
    public void TestMassProperties() {
      Sphere3 testSphere =
        new Sphere3(new Vector3(100.0f, 100.0f, 100.0f), 20.0f);

      Assert.AreEqual(
        new Vector3(100.0f, 100.0f, 100.0f), testSphere.CenterOfMass,
        "Center of mass is correctly positioned"
      );
      Assert.AreEqual(
        33510.32421875f, testSphere.Mass,
        Specifications.MaximumDeviation,
        "Mass of sphere is exactly determined"
      );
      Assert.AreEqual(
        5026.5482457436692f, testSphere.SurfaceArea,
        Specifications.MaximumDeviation,
        "Surface area of sphere is exactly determined"
      );

    }

    /// <summary>Tests the intersection query on moving spheres</summary>
    [Test]
    public void TestMovingSphereIntersection() {
      Sphere3 leftSphere = new Sphere3(new Vector3(0.0f, 0.0f, 0.0f), 10.0f);
      Sphere3 rightSphere = new Sphere3(new Vector3(100.0f, 0.0f, 0.0f), 10.0f);

      Assert.AreEqual(
        new float[] { 0.0f },
        leftSphere.LocateImpact(new Vector3(0.0f, 0.0f, 0.0f), leftSphere),
        "Contact with clone occured immediately"
      );

      Assert.That(
        leftSphere.LocateImpact(new Vector3(200.0f, 0.0f, 0.0f), rightSphere),
        Is.EqualTo(new float[] { 0.4f }).Within(Specifications.MaximumDeviation).Ulps,
        "Fast moving contact with right sphere is determined exactly"
      );

      Assert.That(
        rightSphere.LocateImpact(new Vector3(-200.0f, 0.0f, 0.0f), leftSphere),
        Is.EqualTo(new float[] { 0.4f }).Within(Specifications.MaximumDeviation).Ulps,
        "Fast moving contact with left sphere is determined exactly"
      );
    }

    /// <summary>Tests the bounding box generator</summary>
    [Test]
    public void TestBoundingBox() {
      Sphere3 sphere = new Sphere3(
        new Vector3(15.0f, 15.0f, 15.0f), 5.0f
      );

      AxisAlignedBox3 boundingBox = sphere.BoundingBox;

      Assert.AreEqual(
        new Vector3(10.0f, 10.0f, 10.0f), boundingBox.Min,
        "Minimum corner of bounding box correctly determined"
      );
      Assert.AreEqual(
        new Vector3(20.0f, 20.0f, 20.0f), boundingBox.Max,
        "Maximum corner of bounding box correctly determined"
      );
    }

    /// <summary>Tests the bounding sphere generator</summary>
    [Test]
    public void TestBoundingSphere() {
      Sphere3 sphere = new Sphere3(
        new Vector3(15.0f, 15.0f, 15.0f), 5.0f
      );

      Sphere3 boundingSphere = sphere.BoundingSphere;
      Assert.AreEqual(
        sphere, boundingSphere,
        "Bounding sphere for sphere is identical to the sphere itself"
      );
    }

    /// <summary>Tests the random points on surface function</summary>
 
    [Test]
    public void TestRandomPointOnSurface() {
      Sphere3 unitSphere = new Sphere3(Vector3.Zero, 1.0f);
      Sphere3 innerSphere = new Sphere3(Vector3.Zero, 1.0f - Specifications.HullAccuracy);
      Sphere3 outerSphere = new Sphere3(Vector3.Zero, 1.0f + Specifications.HullAccuracy);

      DefaultRandom random = new DefaultRandom();

      Vector3 total = Vector3.Zero;
      for(int i = 0; i < Specifications.ProbabilisticFunctionSamples; ++i) {
        Vector3 point = unitSphere.RandomPointOnSurface(random);

        Assert.IsFalse(
          innerSphere.Contains(point), "Random point lies on the sphere's hull"
        );
        Assert.IsTrue(
          outerSphere.Contains(point), "Random point lies on the sphere's hull"
        );

        total += point;
      }

      total /= Specifications.ProbabilisticFunctionSamples;

      Assert.AreEqual(
        0.0f, total.X, Specifications.ProbabilisticFunctionDeviation,
        "Random points average on the center of the sphere on the X axis"
      );
      Assert.AreEqual(
        0.0f, total.Y, Specifications.ProbabilisticFunctionDeviation,
        "Random points average on the center of the sphere on the Y axis"
      );
      Assert.AreEqual(
        0.0f, total.Z, Specifications.ProbabilisticFunctionDeviation,
        "Random points average on the center of the sphere on the Z axis"
      );
      
    }

  }

} // namespace Nuclex.Geometry.Volumes

#endif // UNITTEST