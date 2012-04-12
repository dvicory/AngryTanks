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
  public class SphereSphereColliderTest {

    /// <summary>Tests intersection between two spheres</summary>
    [Test]
    public void TestCheckContact() {
      Assert.IsTrue(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(15.0f, 15.0f, 15.0f), 1.0f
        ),
        "Fully enclosed sphere is detected to intersect with test sphere"
      );

      Assert.IsTrue(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(15.0f, 15.0f, 15.0f), 10.0f
        ),
        "Fully containing sphere is detected to intersect with test sphere"
      );

      Assert.IsFalse(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(5.0f - Specifications.MaximumDeviation, 5.0f, 5.0f), 5.0f
        ),
        "Close miss of sphere on X axis to left side is correctly handled"
      );

      Assert.IsFalse(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(25.0f + Specifications.MaximumDeviation, 5.0f, 5.0f), 5.0f
        ),
        "Close miss of sphere on X axis to right side is correctly handled"
      );

      Assert.IsFalse(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(5.0f, 5.0f - Specifications.MaximumDeviation, 5.0f), 5.0f
        ),
        "Close miss of sphere on Y axis to lower side is correctly handled"
      );
      Assert.IsFalse(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(5.0f, 25.0f + Specifications.MaximumDeviation, 5.0f), 5.0f
        ),
        "Close miss of sphere on Y axis to upper side is correctly handled"
      );

      Assert.IsFalse(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(5.0f, 5.0f, 5.0f - Specifications.MaximumDeviation), 5.0f
        ),
        "Close miss of sphere on Z axis to front side is correctly handled"
      );

      Assert.IsFalse(
        SphereSphereCollider.CheckContact(
          new Vector3(15.0f, 15.0f, 15.0f), 5.0f,
          new Vector3(5.0f, 5.0f, 25.0f + Specifications.MaximumDeviation), 5.0f
        ),
        "Close miss of sphere on Z axis to back side is correctly handled"
      );

    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions

#endif // UNITTEST