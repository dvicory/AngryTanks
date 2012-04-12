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
  public class AabbObbColliderTest {

    /// <summary>Tests intersection checking between AABBs and OBBs</summary>
    [Test]
    public void TestAxisAlignedBoxBoxIntersection() {
      Vector3 forward = Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f));
      Vector3 right = Vector3.Normalize(new Vector3(1.0f, -1.0f, 1.0f));
      Vector3 up = Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f));

      Matrix obbTransform = new Matrix(
        right.X,   right.Y,   right.Z,   0.0f,
        up.X,      up.Y,      up.Z,      0.0f,
        forward.X, forward.Y, forward.Z, 0.0f,
        100.0f,    100.0f,    100.0f,    1.0f
      );
        
/*
      AabbObbCollider.CheckContact(
        new Vector3(5.0f, 5.0f, 5.0f),
        obbTransform,
        new Vector3(5.0f, 5.0f, 5.0f)
*/

      /*
      double outRadius = Math.Sqrt(75.0f) + Specifications.MaximumDeviation;

      Assert.IsTrue(
        AabbObbCollider.StaticTest(
          new Vector3(1.0f, 1.0f, 1.0f),
          tiltedBox.Center, tiltedBox.Extents, tiltedBox.Orientation
        ),
        "Fully enclosed box is detected to intersect with test box"
      );

      Assert.IsTrue(
        Intersection.Test(
          new AxisAlignedBox3(new Vector3(5.0f, 5.0f, 5.0f), new Vector3(25.0f, 25.0f, 25.0f)),
          tiltedBox.Center, tiltedBox.Extents, tiltedBox.Orientation
        ),
        "Fully containing box is detected to intersect with test box"
      );

      Assert.IsFalse(
        Intersection.Test(
          new AxisAlignedBox3(
            new Vector3(5.0f, 5.0f, 5.0f),
            new Vector3(15.0f - outRadius, 25.0f, 25.0f)
          ),
          box
        ),
        "Close miss of axis aligned box on X axis to left side is properly handled"
      );
      Assert.IsFalse(
        Intersection.Test(
          new AxisAlignedBox3(
            new Vector3(15.0f + outRadius, 5.0f, 5.0f),
            new Vector3(25.0f, 25.0f, 25.0f)
          ),
          box
        ),
        "Close miss of axis aligned box on X axis to right side is properly handled"
      );

      Assert.IsFalse(
        Intersection.Test(
          new AxisAlignedBox3(
            new Vector3(5.0f, 5.0f, 5.0f),
            new Vector3(25.0f, 15.0f - outRadius, 25.0f)
          ),
          box
        ),
        "Close miss of axis aligned box on Y axis to left side is properly handled"
      );
      Assert.IsFalse(
        Intersection.Test(
          new AxisAlignedBox3(
            new Vector3(5.0f, 15.0f + outRadius, 5.0f),
            new Vector3(25.0f, 25.0f, 25.0f)
          ),
          box
        ),
        "Close miss of axis aligned box on Y axis to right side is properly handled"
      );

      Assert.IsFalse(
        Intersection.Test(
          new AxisAlignedBox3(
            new Vector3(5.0f, 5.0f, 5.0f),
            new Vector3(25.0f, 25.0f, 15.0f - outRadius)
          ),
          box
        ),
        "Close miss of axis aligned box on Z axis to left side is properly handled"
      );
      Assert.IsFalse(
        Intersection.Test(
          new AxisAlignedBox3(
            new Vector3(5.0f, 5.0f, 15.0f + outRadius),
            new Vector3(25.0f, 25.0f, 25.0f)
          ),
          box
        ),
        "Close miss of axis aligned box on Z axis to right side is properly handled"
      );
      */
    }

    private Box3 tiltedBox = new Box3(
      MatrixHelper.Create(
        new Vector3(15.0f, 15.0f, 15.0f),
        Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f)),
        Vector3.Normalize(new Vector3(-1.0f, 1.0f, -1.0f)),
        Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f))
      ),
      new Vector3(5.0f, 5.0f, 5.0f)
    );

  }

} // namespace Nuclex.Geometry.Volumes.Collisions

#endif // UNITTEST