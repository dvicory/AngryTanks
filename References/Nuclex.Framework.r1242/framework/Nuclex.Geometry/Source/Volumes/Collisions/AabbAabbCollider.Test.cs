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
  public class AabbAabbColliderTest {

    private AxisAlignedBox3 testBox = new AxisAlignedBox3(
      new Vector3(10.0f, 10.0f, 10.0f), new Vector3(20.0f, 20.0f, 20.0f)
    );

    /// <summary>Tests intersection checking with two static AABBs</summary>
    [Test]
    public void TestCheckContact() {
      Assert.IsTrue(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(14.0f, 14.0f, 14.0f), new Vector3(16.0f, 16.0f, 16.0f)
        ),
        "Fully enclosed box is detected to intersect with test box"
      );

      Assert.IsTrue(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(5.0f, 5.0f, 5.0f), new Vector3(25.0f, 25.0f, 25.0f)
        ),
        "Fully containing box is detected to intersect with test box"
      );

      Assert.IsFalse(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(5.0f, 5.0f, 5.0f),
          new Vector3(10.0f - Specifications.MaximumDeviation, 25.0f, 25.0f)
        ),
        "Close miss of axis aligned box on X axis to left side is properly handled"
      );

      Assert.IsFalse(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(20.0f + Specifications.MaximumDeviation, 5.0f, 5.0f),
          new Vector3(25.0f, 25.0f, 25.0f)
        ),
        "Close miss of axis aligned box on X axis to right side is properly handled"
      );

      Assert.IsFalse(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(5.0f, 5.0f, 5.0f),
          new Vector3(25.0f, 10.0f - Specifications.MaximumDeviation, 25.0f)
        ),
        "Close miss of axis aligned box on Y axis to left side is properly handled"
      );

      Assert.IsFalse(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(5.0f, 20.0f + Specifications.MaximumDeviation, 5.0f),
          new Vector3(25.0f, 25.0f, 25.0f)
        ),
        "Close miss of axis aligned box on Y axis to right side is properly handled"
      );

      Assert.IsFalse(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(5.0f, 5.0f, 5.0f),
          new Vector3(25.0f, 25.0f, 10.0f - Specifications.MaximumDeviation)
        ),
        "Close miss of axis aligned box on Z axis to left side is properly handled"
      );

      Assert.IsFalse(
        AabbAabbCollider.CheckContact(
          testBox.Min, testBox.Max,
          new Vector3(5.0f, 5.0f, 20.0f + Specifications.MaximumDeviation),
          new Vector3(25.0f, 25.0f, 25.0f)
        ),
        "Close miss of axis aligned box on Z axis to right side is properly handled"
      );

    }

    /// <summary>Tests intersection checking with moving AABBs</summary>
    [Test]
    public void TestFindDynamicContact() {
      float? impactTime = AabbAabbCollider.FindContact(
        testBox.Min,
        testBox.Max,
        new Vector3(0.0f, 10.0f, 10.0f),
        new Vector3(5.0f, 15.0f, 15.0f),
        new Vector3(10.0f, 0.0f, 0.0f)
      );
      Assert.IsTrue(impactTime.HasValue);
      Assert.AreEqual(0.5f, impactTime.Value);
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions

#endif // UNITTEST