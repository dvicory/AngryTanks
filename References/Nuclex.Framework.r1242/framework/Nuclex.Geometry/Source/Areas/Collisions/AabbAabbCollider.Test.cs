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

namespace Nuclex.Geometry.Areas.Collisions {

  /// <summary>Test for the AABB interference detection routines</summary>
  [TestFixture]
  public class AabbAabbColliderTest {

    private AxisAlignedRectangle2 testRectangle = new AxisAlignedRectangle2(
      new Vector2(10.0f, 10.0f), new Vector2(20.0f, 20.0f)
    );

    struct TestPoint {
      public TestPoint(Vector2 min, Vector2 max, string name) {
        this.Min = min;
        this.Max = max;
        this.Name = name;
      }
      public Vector2 Min, Max;
      public string Name;
    }

    /// <summary>Tests intersection checking with two static AABBs</summary>
    [Test]
    public void TestCheckContact() {

      Assert.IsTrue(
        AabbAabbCollider.CheckContact(
          testRectangle.Min, testRectangle.Max,
          new Vector2(14.0f, 14.0f), new Vector2(16.0f, 16.0f)
        ),
        "Fully enclosed box"
      );

      Assert.IsTrue(
        AabbAabbCollider.CheckContact(
          testRectangle.Min, testRectangle.Max,
          new Vector2(5.0f, 5.0f), new Vector2(25.0f, 25.0f)
        ),
        "Fully containing box"
      );
      
      TestPoint[] testPoints = {
        new TestPoint(
          new Vector2(5.0f, 5.0f), new Vector2(10.0f - Specifications.MaximumDeviation, 25.0f),
          "Close miss on X axis, left side"
        ),
        new TestPoint(
          new Vector2(20.0f + Specifications.MaximumDeviation, 5.0f), new Vector2(25.0f, 25.0f),
          "Close miss on X axis, right side"
        ),
        new TestPoint(
          new Vector2(5.0f, 5.0f), new Vector2(25.0f, 10.0f - Specifications.MaximumDeviation),
          "Close miss on Y axis, upper side"
        ),
        new TestPoint(
          new Vector2(5.0f, 20.0f + Specifications.MaximumDeviation), new Vector2(25.0f, 25.0f),
          "Close miss on Y axis, lower side"
        )
      };

      foreach(TestPoint testPoint in testPoints) {
        Assert.IsFalse(
          AabbAabbCollider.CheckContact(
            testRectangle.Min, testRectangle.Max, testPoint.Min, testPoint.Max
          ),
          testPoint.Name
        );
        Assert.IsFalse(
          AabbAabbCollider.CheckContact(
            testRectangle.Min, testRectangle.Max, testPoint.Min, testPoint.Max
          ),
          testPoint.Name
        );
      }

    }

    /// <summary>Tests intersection checking with moving AABBs</summary>
    [Test]
    public void TestFindDynamicContact() {
      float? impactTime = AabbAabbCollider.FindContact(
        testRectangle.Min,
        testRectangle.Max,
        new Vector2(0.0f, 10.0f),
        new Vector2(5.0f, 5.0f),
        new Vector2(10.0f, 0.0f)
      );
      Assert.IsTrue(impactTime.HasValue);
      Assert.AreEqual(0.5f, impactTime.Value);
    }

  }

} // namespace Nuclex.Geometry.Areas.Collisions

#endif // UNITTEST
