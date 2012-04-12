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

namespace Nuclex.Geometry.Volumes.Collisions {

  /// <summary>Test for the AABB interference detection routines</summary>
  [TestFixture]
  public class CylinderSphereColliderTest {

    /// <summary>Tests the cylinder/sphere contact checking </summary>
    [Test]
    public void TestCheckContact() {
      Cylinder3 testCylinder = new Cylinder3(
        Matrix.Identity, 10.0f, 5.0f
      );

      Assert.IsTrue(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          Vector3.Zero, 5.0f
        ),
        "Fully contained sphere is detected to overlap with the cylinder"
      );

      Assert.IsTrue(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(-20.0f + Specifications.HullAccuracy, 0.0f, 0.0f), 10.0f
        ),
        "Close hit of cylinder on X axis (left side) detected"
      );
      Assert.IsTrue(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(20.0f - Specifications.HullAccuracy, 0.0f, 0.0f), 10.0f
        ),
        "Close hit of cylinder on X axis (right side) detected"
      );

      Assert.IsFalse(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(-20.0f - Specifications.HullAccuracy, 0.0f, 0.0f), 10.0f
        ),
        "Close miss of cylinder on X axis (left side) properly handled"
      );
      Assert.IsFalse(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(20.0f + Specifications.HullAccuracy, 0.0f, 0.0f), 10.0f
        ),
        "Close miss of cylinder on X axis (right side) properly handled"
      );

      Assert.IsTrue(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, -20.0f + Specifications.HullAccuracy, 0.0f), 10.0f
        ),
        "Close hit of cylinder on Y axis (lower side) detected"
      );
      Assert.IsTrue(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, 20.0f - Specifications.HullAccuracy, 0.0f), 10.0f
        ),
        "Close hit of cylinder on Y axis (upper side) detected"
      );

      Assert.IsFalse(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, -20.0f - Specifications.HullAccuracy, 0.0f), 10.0f
        ),
        "Close miss of cylinder on Y axis (lower side) properly handled"
      );
      Assert.IsFalse(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, 20.0f + Specifications.HullAccuracy, 0.0f), 10.0f
        ),
        "Close miss of cylinder on Y axis (upper side) properly handled"
      );

      Assert.IsTrue(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, 0.0f, -12.5f + Specifications.HullAccuracy), 10.0f
        ),
        "Close hit of cylinder on Z axis (front side) detected"
      );
      Assert.IsTrue(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, 0.0f, 12.5f - Specifications.HullAccuracy), 10.0f
        ),
        "Close hit of cylinder on Z axis (back side) detected"
      );

      Assert.IsFalse(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, 0.0f, -12.5f - Specifications.HullAccuracy), 10.0f
        ),
        "Close miss of cylinder on Z axis (front side) properly handled"
      );
      Assert.IsFalse(
        CylinderSphereCollider.CheckContact(
          testCylinder.Transform, testCylinder.Radius, testCylinder.Height,
          new Vector3(0.0f, 0.0f, 12.5f + Specifications.HullAccuracy), 10.0f
        ),
        "Close miss of cylinder on Z axis (back side) properly handled"
      );

    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions

#endif // UNITTEST