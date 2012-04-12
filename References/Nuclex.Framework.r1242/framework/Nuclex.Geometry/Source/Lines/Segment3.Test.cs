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
using System.Xml.Serialization;

using Microsoft.Xna.Framework;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Geometry.Lines {

  /// <summary>Tests the implementation of the Ray2 class</summary>
  [TestFixture]
  public class Segment3Test {

    /// <summary>Validates the equality operator</summary>
    [Test]
    public void TestEqualityOperator() {
      Segment3 segment1 = new Segment3(
        new Vector3(100.0f, 200.0f, 300.0f), new Vector3(400.0f, 500.0f, 600.0f)
      );
      Segment3 segment2 = new Segment3(segment1);

      Assert.AreEqual(segment1, segment2, "Copied segment is equal to the original segment");

      segment1.Start.X = 0.0f;
      Assert.AreNotEqual(
        segment1, segment2, "Modified copy is no longer equal to the original segment"
      );
      segment1.Start.X = 100.0f;

      segment1.Start.Y = 0.0f;
      Assert.AreNotEqual(
        segment1, segment2, "Modified copy is no longer equal to the original segment"
      );
      segment1.Start.Y = 200.0f;

      segment1.Start.Z = 0.0f;
      Assert.AreNotEqual(
        segment1, segment2, "Modified copy is no longer equal to the original segment"
      );
      segment1.Start.Z = 300.0f;

      segment1.End.X = 0.0f;
      Assert.AreNotEqual(
        segment1, segment2, "Modified copy is no longer equal to the original segment"
      );
      segment1.End.X = 400.0f;

      segment1.End.Y = 0.0f;
      Assert.AreNotEqual(
        segment1, segment2, "Modified copy is no longer equal to the original segment"
      );
      segment1.End.Y = 500.0f;

      segment1.End.Z = 0.0f;
      Assert.AreNotEqual(
        segment1, segment2, "Modified copy is no longer equal to the original segment"
      );
      segment1.End.Z = 600.0f;
    }

    /// <summary>Checks whether the equality operator properly handles null</summary>
    [Test]
    public void TestEqualityOperatorAgainstNull() {
      Segment3 line = new Segment3();

      Assert.IsFalse(line.Equals(null), "Initialized Line is not equal to null");
    }

    /// <summary>Tests whether the constructors are working properly</summary>
    [Test]
    public void TestConstructor() {
      Segment3 line = new Segment3(
        new Vector3(1.0f, 2.0f, 3.0f), new Vector3(4.0f, 5.0f, 6.0f)
      );

      Assert.AreEqual(1.0f, line.Start.X, "X start is taken over from constructor");
      Assert.AreEqual(2.0f, line.Start.Y, "Y start is taken over from constructor");
      Assert.AreEqual(3.0f, line.Start.Z, "Z start is taken over from constructor");
      Assert.AreEqual(4.0f, line.End.X, "X end is taken over from constructor");
      Assert.AreEqual(5.0f, line.End.Y, "Y end is taken over from constructor");
      Assert.AreEqual(6.0f, line.End.Z, "Z end is taken over from constructor");
    }

    /// <summary>Tests whether the closest point determination works</summary>
    [Test]
    public void TestClosestPoint() {

      // Test whether the closest point determination works on the X axis
      Segment3 line = new Segment3(
        new Vector3(0.0f, 100.0f, 100.0f), new Vector3(1.0f, 100.0f, 100.0f)
      );

      Vector3 leftCap = line.ClosestPointTo(new Vector3(-2.0f, 200.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(0.0f, 100.0f, 100.0f), leftCap, "Closest point beyond left end found"
      );

      Vector3 leftPoint = line.ClosestPointTo(new Vector3(0.0f, 200.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(0.0f, 100.0f, 100.0f), leftPoint, "Closest point on left end found"
      );

      Vector3 midLeftRight = line.ClosestPointTo(new Vector3(0.5f, 200.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(0.5f, 100.0f, 100.0f), midLeftRight, "Closest point inmidst of line found"
      );

      Vector3 rightPoint = line.ClosestPointTo(new Vector3(1.0f, 200.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(1.0f, 100.0f, 100.0f), rightPoint, "Closest point on right end found"
      );

      Vector3 rightCap = line.ClosestPointTo(new Vector3(3.0f, 200.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(1.0f, 100.0f, 100.0f), rightCap, "Closest point beyond right end found"
      );

      // Test whether the closest point determination works on the Y axis
      line = new Segment3(new Vector3(100.0f, 0.0f, 100.0f), new Vector3(100.0f, 1.0f, 100.0f));

      leftCap = line.ClosestPointTo(new Vector3(200.0f, -2.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 0.0f, 100.0f), leftCap, "Closest point beyond lower end found"
      );

      leftPoint = line.ClosestPointTo(new Vector3(200.0f, 0.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 0.0f, 100.0f), leftPoint, "Closest point on lower end found"
      );

      midLeftRight = line.ClosestPointTo(new Vector3(200.0f, 0.5f, 200.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 0.5f, 100.0f), midLeftRight, "Closest point inmidst of line found"
      );

      rightPoint = line.ClosestPointTo(new Vector3(200.0f, 1.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 1.0f, 100.0f), rightPoint, "Closest point on upper end found"
      );

      rightCap = line.ClosestPointTo(new Vector3(200.0f, 3.0f, 200.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 1.0f, 100.0f), rightCap, "Closest point beyond upper end found"
      );

      // Test whether the closest point determination works on the Z axis
      line = new Segment3(new Vector3(100.0f, 100.0f, 0.0f), new Vector3(100.0f, 100.0f, 1.0f));

      leftCap = line.ClosestPointTo(new Vector3(200.0f, 200.0f, -2.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 100.0f, 0.0f), leftCap, "Closest point beyond nearest end found"
      );

      leftPoint = line.ClosestPointTo(new Vector3(200.0f, 200.0f, 0.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 100.0f, 0.0f), leftPoint, "Closest point on nearest end found"
      );

      midLeftRight = line.ClosestPointTo(new Vector3(200.0f, 200.0f, 0.5f));
      Assert.AreEqual(
        new Vector3(100.0f, 100.0f, 0.5f), midLeftRight, "Closest point inmidst of line found"
      );

      rightPoint = line.ClosestPointTo(new Vector3(200.0f, 200.0f, 1.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 100.0f, 1.0f), rightPoint, "Closest point on farthest end found"
      );

      rightCap = line.ClosestPointTo(new Vector3(200.0f, 200.0f, 3.0f));
      Assert.AreEqual(
        new Vector3(100.0f, 100.0f, 1.0f), rightCap, "Closest point beyond farthest end found"
      );
    }

    /// <summary>Ensures that the contact finding method works for axis aligned boxes</summary>
    [Test, Ignore]
    public void TestFindContactsOnAxisAlignedBox() {
      Volumes.AxisAlignedBox3 box = new Volumes.AxisAlignedBox3(
        new Vector3(10.0f, 10.0f, 10.0f), new Vector3(20.0f, 20.0f, 20.0f)
      );

      float outRadius = 5.0f + Specifications.MaximumDeviation;

      Assert.AreEqual(
        new float[] { 10.0f / 30.0f, 20.0f / 30.0f }, // 30.0f to take line's length into account
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f),
          new Vector3(30.0f, 15.0f, 15.0f)
        ).FindContacts(box),
        "Contact locations on AAB for X sweep found"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f),
          new Vector3(15.0f - outRadius, 15.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 15.0f, 15.0f),
          new Vector3(30.0f, 15.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f - outRadius, 15.0f),
          new Vector3(30.0f, 15.0f - outRadius, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f + outRadius, 15.0f),
          new Vector3(30.0f, 15.0f + outRadius, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f - outRadius),
          new Vector3(30.0f, 15.0f, 15.0f - outRadius)
        ).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f + outRadius),
          new Vector3(30.0f, 15.0f, 15.0f + outRadius)
        ).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );

      Assert.AreEqual(
        new float[] { 10.0f / 30.0f, 20.0f / 30.0f }, // 30.0 to take line's length into account
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f),
          new Vector3(15.0f, 30.0f, 15.0f)
        ).FindContacts(box),
        "Contact locations on AAB for Y sweep found"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f),
          new Vector3(15.0f, 15.0f - outRadius, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f + outRadius, 15.0f),
          new Vector3(15.0f, 30.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f - outRadius, 0.0f, 15.0f),
          new Vector3(15.0f - outRadius, 30.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 0.0f, 15.0f),
          new Vector3(15.0f + outRadius, 30.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f - outRadius),
          new Vector3(15.0f, 30.0f, 15.0f - outRadius)
        ).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f + outRadius),
          new Vector3(15.0f, 30.0f, 15.0f + outRadius)
        ).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );

      Assert.AreEqual(
        new float[] { 10.0f / 30.0f, 20.0f / 30.0f }, // 30.0 to take line's length into account
        new Segment3(
          new Vector3(15.0f, 15.0f, 0.0f),
          new Vector3(15.0f, 15.0f, 30.0f)
        ).FindContacts(box),
        "Contact locations on AAB for Z sweep found"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f, 0.0f),
          new Vector3(15.0f, 15.0f, 15.0f - outRadius)
        ).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f, 15.0f + outRadius),
          new Vector3(15.0f, 15.0f, 30.0f)
        ).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f - outRadius, 15.0f, 0.0f),
          new Vector3(15.0f - outRadius, 15.0f, 30.0f)
        ).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 15.0f, 0.0f),
          new Vector3(15.0f + outRadius, 15.0f, 30.0f)
        ).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f - outRadius, 0.0f),
          new Vector3(15.0f, 15.0f - outRadius, 30.0f)
        ).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f + outRadius, 0.0f),
          new Vector3(15.0f, 15.0f + outRadius, 30.0f)
        ).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );

    }

    /// <summary>Ensures that the contact finding method works for oriented boxes</summary>
    [Test, Ignore]
    public void TestFindContactsOnOrientedBox() {
      Volumes.Box3 box = new Volumes.Box3(
        MatrixHelper.Create(
          new Vector3(15.0f, 15.0f, 15.0f),
          Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f)),
          Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f)),
          Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f))
        ),
        new Vector3(5.0f, 5.0f, 5.0f)
      );

      float boxRadius = (float)Math.Sqrt(5 * 5 + 5 * 5 + 5 * 5);
      float outRadius = boxRadius + Specifications.MaximumDeviation;

      float[] contacts = new Segment3(
        new Vector3(0.0f, 15.0f, 15.0f),
        new Vector3(30.0f, 15.0f, 15.0f)
      ).FindContacts(box);

      Assert.AreEqual(
        (15.0f - boxRadius) / 30.0f, contacts[0], Specifications.MaximumDeviation,
        "Contact locations on box for X sweep found"
      );
      Assert.AreEqual(
        (15.0f + boxRadius) / 30.0f, contacts[1], Specifications.MaximumDeviation,
        "Contact locations on box for X sweep found"
      );

      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f),
          new Vector3(15.0f - outRadius, 15.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 15.0f, 15.0f),
          new Vector3(30.0f, 15.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f - outRadius, 15.0f),
          new Vector3(30.0f, 15.0f - outRadius, 15.0f)
        ).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f + outRadius, 15.0f),
          new Vector3(30.0f, 15.0f + outRadius, 15.0f)
        ).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f - outRadius),
          new Vector3(30.0f, 15.0f, 15.0f - outRadius)
        ).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f + outRadius),
          new Vector3(30.0f, 15.0f, 15.0f + outRadius)
        ).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );

      contacts = new Segment3(
        new Vector3(15.0f, 0.0f, 15.0f),
        new Vector3(15.0f, 30.0f, 15.0f)
      ).FindContacts(box);

      Assert.AreEqual(
        (15.0f - boxRadius) / 30.0f, contacts[0], Specifications.MaximumDeviation,
        "Contact locations on box for Y sweep found"
      );
      Assert.AreEqual(
        (15.0f + boxRadius) / 30.0f, contacts[1], Specifications.MaximumDeviation,
        "Contact locations on box for Y sweep found"
      );

      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f),
          new Vector3(15.0f, 15.0f - outRadius, 15.0f)
        ).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f + outRadius, 15.0f),
          new Vector3(15.0f, 30.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f - outRadius, 0.0f, 15.0f),
          new Vector3(15.0f - outRadius, 30.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 0.0f, 15.0f),
          new Vector3(15.0f + outRadius, 30.0f, 15.0f)
        ).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f - outRadius),
          new Vector3(15.0f, 30.0f, 15.0f - outRadius)
        ).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f + outRadius),
          new Vector3(15.0f, 30.0f, 15.0f + outRadius)
        ).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );

      contacts = new Segment3(
        new Vector3(15.0f, 15.0f, 0.0f),
        new Vector3(15.0f, 15.0f, 30.0f)
      ).FindContacts(box);

      Assert.AreEqual(
        (15.0f - boxRadius) / 30.0, contacts[0], Specifications.MaximumDeviation,
        "Contact locations on boxf for Z sweep found"
      );
      Assert.AreEqual(
        (15.0f + boxRadius) / 30.0f, contacts[1], Specifications.MaximumDeviation,
        "Contact locations on box for Z sweep found"
      );

      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f, 0.0f),
          new Vector3(15.0f, 15.0f, 15.0f - outRadius)
        ).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f, 15.0f + outRadius),
          new Vector3(15.0f, 15.0f, 30.0f)
        ).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f - outRadius, 15.0f, 0.0f),
          new Vector3(15.0f - outRadius, 15.0f, 30.0f)
        ).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 15.0f, 0.0f),
          new Vector3(15.0f + outRadius, 15.0f, 30.0f)
        ).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f - outRadius, 0.0f),
          new Vector3(15.0f, 15.0f - outRadius, 30.0f)
        ).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f + outRadius, 0.0f),
          new Vector3(15.0f, 15.0f + outRadius, 30.0f)
        ).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
    }

    /// <summary>Ensures that the contact finding method works for spheres</summary>
    [Test, Ignore]
    public void TestFindContactsOnSphere() {
      Volumes.Sphere3 sphere = new Volumes.Sphere3(
        new Vector3(15.0f, 15.0f, 15.0f), 5.0f
      );

      float outRadius = 5.0f + Specifications.MaximumDeviation;

      Assert.AreEqual(
        new float[] { 10.0f / 30.0f, 20.0f / 30.0f },
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f),
          new Vector3(30.0f, 15.0f, 15.0f)
        ).FindContacts(sphere),
        "Contact locations on sphere for X sweep found"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f),
          new Vector3(15.0f - outRadius, 15.0f, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 15.0f, 15.0f),
          new Vector3(30.0f, 15.0f, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f - outRadius, 15.0f),
          new Vector3(30.0f, 15.0f - outRadius, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f + outRadius, 15.0f),
          new Vector3(30.0f, 15.0f + outRadius, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f - outRadius),
          new Vector3(30.0f, 15.0f, 15.0f - outRadius)
        ).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(0.0f, 15.0f, 15.0f + outRadius),
          new Vector3(30.0f, 15.0f, 15.0f + outRadius)
        ).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );

      Assert.AreEqual(
        new float[] { 10.0f / 30.0f, 20.0f / 30.0f },
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f),
          new Vector3(15.0f, 30.0f, 15.0f)
        ).FindContacts(sphere),
        "Contact locations on sphere for Y sweep found"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f),
          new Vector3(15.0f, 15.0f - outRadius, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f + outRadius, 15.0f),
          new Vector3(15.0f, 30.0f, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f - outRadius, 0.0f, 15.0f),
          new Vector3(15.0f - outRadius, 30.0f, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 0.0f, 15.0f),
          new Vector3(15.0f + outRadius, 30.0f, 15.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f - outRadius),
          new Vector3(15.0f, 30.0f, 15.0f - outRadius)
        ).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 0.0f, 15.0f + outRadius),
          new Vector3(15.0f, 30.0f, 15.0f + outRadius)
        ).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );

      Assert.AreEqual(
        new float[] { 10.0f / 30.0f, 20.0f / 30.0f },
        new Segment3(
          new Vector3(15.0f, 15.0f, 0.0f),
          new Vector3(15.0f, 15.0f, 30.0f)
        ).FindContacts(sphere),
        "Contact locations on sphere for Z sweep found"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f, 0.0f),
          new Vector3(15.0f, 15.0f, 15.0f - outRadius)
        ).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f, 15.0f + outRadius),
          new Vector3(15.0f, 15.0f, 30.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f - outRadius, 15.0f, 0.0f),
          new Vector3(15.0f - outRadius, 15.0f, 30.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f + outRadius, 15.0f, 0.0f),
          new Vector3(15.0f + outRadius, 15.0f, 30.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f - outRadius, 0.0f),
          new Vector3(15.0f, 15.0f - outRadius, 30.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Segment3(
          new Vector3(15.0f, 15.0f + outRadius, 0.0f),
          new Vector3(15.0f, 15.0f + outRadius, 30.0f)
        ).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );

    }

    /// <summary>Ensures that the contact finding method works for planes</summary>
    [Test]
    public void TestFindContactsOnPlane() {
      Areas.Plane3 plane = new Areas.Plane3(
        new Vector3(15.0f, 15.0f, 15.0f),
        -Vector3.Backward
      );

      LineContacts contacts = new Segment3(
        new Vector3(15.0f, 15.0f, 0.0f), new Vector3(15.0f, 15.0f, 30.0f)
      ).FindContacts(plane);
      Assert.AreEqual(
        0.5f, contacts.EntryTime,
        "Direct orthogonal contact to plane detected"
      );

      contacts = new Segment3(
        new Vector3(15.0f, 15.0f, 30.0f), new Vector3(15.0f, 15.0f, 0.0f)
      ).FindContacts(plane);
      Assert.AreEqual(
        0.5f, contacts.EntryTime,
        "Direct orthogonal contact to plane from backside detected"
      );

    }

    /// <summary>
    ///   Ensures that the contact finding method detects a line segment that
    ///   directly pierces the triangle orthogonally
    /// </summary>
    [Test]
    public void TestFindOrthogonalContactsOnTriangle() {
      Areas.Triangle3 triangle = new Areas.Triangle3(
        new Vector3(10.0f, 10.0f, 15.0f),
        new Vector3(20.0f, 10.0f, 15.0f),
        new Vector3(15.0f, 20.0f, 15.0f)
      );

      LineContacts contacts = new Segment3(
        new Vector3(15.0f, 15.0f, 0.0f), new Vector3(15.0f, 15.0f, 30.0f)
      ).FindContacts(triangle);
      Assert.AreEqual(
        0.5f, contacts.EntryTime,
        "Direct orthogonal contact to triangle detected"
      );

      contacts = new Segment3(
        new Vector3(15.0f, 15.0f, 30.0f), new Vector3(15.0f, 15.0f, 0.0f)
      ).FindContacts(triangle);
      Assert.AreEqual(
        0.5f, contacts.EntryTime,
        "Direct orthogonal contact to triangle backside detected"
      );
    }

    /// <summary>
    ///   Ensures that the contact finding method detects a line segment that
    ///   touches the sides of the triangle
    /// </summary>
    [Test]
    public void TestFindSideContactsOnTriangle() {
      Areas.Triangle3 triangle = new Areas.Triangle3(
        new Vector3(10.0f, 10.0f, 15.0f),
        new Vector3(20.0f, 10.0f, 15.0f),
        new Vector3(15.0f, 20.0f, 15.0f)
      );

      LineContacts contacts = new Segment3(
        new Vector3(15.0f, 5.0f, 0.0f), new Vector3(15.0f, 5.0f, 30.0f)
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime, "Side A->B miss of triangle plane correctly filtered out"
      );

      contacts = new Segment3(
        new Vector3(10.0f, 15.0f, 0.0f), new Vector3(10.0f, 15.0f, 30.0f)
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime, "Side A->C miss of triangle plane correctly filtered out"
      );

      contacts = new Segment3(
        new Vector3(20.0f, 15.0f, 0.0f), new Vector3(20.0f, 15.0f, 30.0f)
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime, "Side B->C miss of triangle plane correctly filtered out"
      );

      contacts = new Ray3(
        new Vector3(15.0f, 15.0f, 15.0f + Specifications.MaximumDeviation),
        new Vector3(15.0f, 15.0f, 30.0f)
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime,
        "Close orthogonal miss of triangle at segment beginning correctly filtered out"
      );

      contacts = new Ray3(
        new Vector3(15.0f, 15.0f, 0.0f),
        new Vector3(15.0f, 15.0f, 15.0f - Specifications.MaximumDeviation)
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime,
        "Close orthogonal miss of triangle at segment end correctly filtered out"
      );

    }

    /// <summary>Tests whether the class is serialized properly</summary>
    [Test]
    public void TestSerialization() {
      XmlSerializer serializer = new XmlSerializer(typeof(Segment3));

      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      Segment3 segment = new Segment3(
        new Vector3(123.0f, 456.0f, 789.0f), new Vector3(987.0f, 654.0f, 321.0f)
      );
      serializer.Serialize(stream, segment);

      stream.Seek(0, System.IO.SeekOrigin.Begin);
      Segment3 restored = (Segment3)serializer.Deserialize(stream);

      Assert.AreEqual(segment, restored, "Deserialized segment matches serialized segment");
    }
  }

} // namespace Nuclex.Geometry.Ranges

#endif // UNITTEST