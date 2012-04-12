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

  /// <summary>Tests the implementation of the Ray3 class</summary>
  [TestFixture]
  public class Ray3Test {

    /// <summary>Validates the equality operator</summary>
    [Test]
    public void TestEqualityOperator() {
      Ray3 ray1 = new Ray3(
        new Vector3(100.0f, 200.0f, 300.0f), new Vector3(400.0f, 500.0f, 600.0f)
      );
      Ray3 ray2 = new Ray3(ray1);

      Assert.AreEqual(ray1, ray2, "Copied ray is equal to the original ray");

      ray1.Origin.X = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Origin.X = 100.0f;

      ray1.Origin.Y = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Origin.Y = 200.0f;

      ray1.Origin.Z = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Origin.Z = 300.0f;

      ray1.Direction.X = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Direction.X = 400.0f;

      ray1.Direction.Y = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Direction.Y = 500.0f;

      ray1.Direction.Z = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Direction.Z = 600.0f;
    }

    /// <summary>Checks whether the equality operator properly handles null</summary>
    [Test]
    public void TestEqualityOperatorAgainstNull() {
      Ray3 line = new Ray3();

      Assert.IsFalse(line.Equals(null), "Initialized Line is not equal to null");
    }

    /// <summary>Tests whether the constructors are working properly</summary>
    [Test]
    public void TestConstructor() {
      Ray3 line = new Ray3(new Vector3(2.0f, 3.0f, 4.0f), new Vector3(1.0f, 0.0f, 0.0f));

      Assert.AreEqual(2.0f, line.Origin.X, "X origin is taken over from constructor");
      Assert.AreEqual(3.0f, line.Origin.Y, "Y origin is taken over from constructor");
      Assert.AreEqual(4.0f, line.Origin.Z, "Z origin is taken over from constructor");
      Assert.AreEqual(1.0f, line.Direction.X, "X direction is taken over from constructor");
      Assert.AreEqual(0.0f, line.Direction.Y, "Y direction is taken over from constructor");
      Assert.AreEqual(0.0f, line.Direction.Z, "Z direction is taken over from constructor");

      line = new Ray3(new Vector3(2.0f, 3.0f, 4.0f), new Vector3(0.0f, 1.0f, 0.0f));

      Assert.AreEqual(2.0f, line.Origin.X, "X origin is taken over from constructor");
      Assert.AreEqual(3.0f, line.Origin.Y, "Y origin is taken over from constructor");
      Assert.AreEqual(4.0f, line.Origin.Z, "Z origin is taken over from constructor");
      Assert.AreEqual(0.0f, line.Direction.X, "X direction is taken over from constructor");
      Assert.AreEqual(1.0f, line.Direction.Y, "Y direction is taken over from constructor");
      Assert.AreEqual(0.0f, line.Direction.Z, "Z direction is taken over from constructor");

      line = new Ray3(new Vector3(2.0f, 3.0f, 4.0f), new Vector3(0.0f, 0.0f, 1.0f));

      Assert.AreEqual(2, line.Origin.X, "X origin is taken over from constructor");
      Assert.AreEqual(3, line.Origin.Y, "Y origin is taken over from constructor");
      Assert.AreEqual(4, line.Origin.Z, "Z origin is taken over from constructor");
      Assert.AreEqual(0, line.Direction.X, "X direction is taken over from constructor");
      Assert.AreEqual(0, line.Direction.Y, "Y direction is taken over from constructor");
      Assert.AreEqual(1, line.Direction.Z, "Z direction is taken over from constructor");
    }

    /// <summary>Tests whether the closest point determination works</summary>
    [Test]
    public void TestClosestPoint() {

      // Test whether the closest point determination works on the X axis
      Ray3 line = new Ray3(new Vector3(0.0f, 100.0f, 100.0f), Vector3.Right);

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
        new Vector3(3.0f, 100.0f, 100.0f), rightCap, "Closest point beyond right end found"
      );

      // Test whether the closest point determination works on the Y axis
      line = new Ray3(new Vector3(100.0f, 0.0f, 100.0f), Vector3.Up);

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
        new Vector3(100.0f, 3.0f, 100.0f), rightCap, "Closest point beyond upper end found"
      );

      // Test whether the closest point determination works on the Z axis
      line = new Ray3(new Vector3(100.0f, 100.0f, 0.0f), Vector3.Backward);

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
        new Vector3(100.0f, 100.0f, 3.0f), rightCap, "Closest point beyond farthest end found"
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
        new double[] { 10.0f, 20.0f },
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f), Vector3.Right).FindContacts(box),
        "Contact locations on AAB for X sweep found"
      );
      Assert.AreEqual(
        new double[] { 0.0f, 5.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f), Vector3.Right).FindContacts(box),
        "Contact locations on AAB for X sweep starting in AAB found"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 15.0f, 15.0f), Vector3.Right).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f - outRadius, 15.0f), Vector3.Right).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f + outRadius, 15.0f), Vector3.Right).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f - outRadius), Vector3.Right).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f + outRadius), Vector3.Right).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );

      Assert.AreEqual(
        new double[] { 10.0f, 20.0f },
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f), Vector3.Up).FindContacts(box),
        "Contact locations on AAB for Y sweep found"
      );
      Assert.AreEqual(
        new double[] { 0.0f, 5.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f), Vector3.Up).FindContacts(box),
        "Contact locations on AAB for Y sweep starting in AAB found"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f + outRadius, 15.0f), Vector3.Up).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f - outRadius, 0.0f, 15.0f), Vector3.Up).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 0.0f, 15.0f), Vector3.Up).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f - outRadius), Vector3.Up).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f + outRadius), Vector3.Up).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );

      Assert.AreEqual(
        new double[] { 10.0f, 20.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 0.0f), Vector3.Backward).FindContacts(box),
        "Contact locations on AAB for Z sweep found"
      );
      Assert.AreEqual(
        new double[] { 0.0f, 5.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f), Vector3.Backward).FindContacts(box),
        "Contact locations on AAB for Z sweep starting in AAB found"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f + outRadius), Vector3.Backward).FindContacts(box),
        "Close miss of AAB on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f - outRadius, 15.0f, 0.0f), Vector3.Backward).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 15.0f, 0.0f), Vector3.Backward).FindContacts(box),
        "Close miss of AAB on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f - outRadius, 0.0f), Vector3.Backward).FindContacts(box),
        "Close miss of AAB on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f + outRadius, 0.0f), Vector3.Backward).FindContacts(box),
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

      float[] contacts =
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f), Vector3.Right).FindContacts(box);

      Assert.AreEqual(
        15.0f - boxRadius, contacts[0], Specifications.MaximumDeviation,
        "Contact locations on box for X sweep found"
      );
      Assert.AreEqual(
        15.0f + boxRadius, contacts[1], Specifications.MaximumDeviation,
        "Contact locations on box for X sweep found"
      );

      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 15.0f, 15.0f), Vector3.Right).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f - outRadius, 15.0f), Vector3.Right).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f + outRadius, 15.0f), Vector3.Right).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f - outRadius), Vector3.Right).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f + outRadius), Vector3.Right).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );

      contacts =
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f), Vector3.Up).FindContacts(box);

      Assert.AreEqual(
        15.0f - boxRadius, contacts[0], Specifications.MaximumDeviation,
        "Contact locations on box for Y sweep found"
      );
      Assert.AreEqual(
        15.0f + boxRadius, contacts[1], Specifications.MaximumDeviation,
        "Contact locations on box for Y sweep found"
      );

      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f + outRadius, 15.0f), Vector3.Up).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f - outRadius, 0.0f, 15.0f), Vector3.Up).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 0.0f, 15.0f), Vector3.Up).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f - outRadius), Vector3.Up).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f + outRadius), Vector3.Up).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );

      contacts =
        new Ray3(new Vector3(15.0f, 15.0f, 0.0f), Vector3.Backward).FindContacts(box);

      Assert.AreEqual(
        15.0f - boxRadius, contacts[0], Specifications.MaximumDeviation,
        "Contact locations on box for Z sweep found"
      );
      Assert.AreEqual(
        15.0f + boxRadius, contacts[1], Specifications.MaximumDeviation,
        "Contact locations on box for Z sweep found"
      );

      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f + outRadius), Vector3.Backward).FindContacts(box),
        "Close miss of box on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f - outRadius, 15.0f, 0.0f), Vector3.Backward).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 15.0f, 0.0f), Vector3.Backward).FindContacts(box),
        "Close miss of box on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f - outRadius, 0.0f), Vector3.Backward).FindContacts(box),
        "Close miss of box on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f + outRadius, 0.0f), Vector3.Backward).FindContacts(box),
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
        new double[] { 10.0f, 20.0f },
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f), Vector3.Right).FindContacts(sphere),
        "Contact locations on sphere for X sweep found"
      );
      Assert.AreEqual(
        new double[] { 0.0f, 5.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f), Vector3.Right).FindContacts(sphere),
        "Contact locations on sphere for X sweep starting in sphere found"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 15.0f, 15.0f), Vector3.Right).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f - outRadius, 15.0f), Vector3.Right).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f + outRadius, 15.0f), Vector3.Right).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f - outRadius), Vector3.Right).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(0.0f, 15.0f, 15.0f + outRadius), Vector3.Right).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );

      Assert.AreEqual(
        new double[] { 10.0f, 20.0f },
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f), Vector3.Up).FindContacts(sphere),
        "Contact locations on sphere for Y sweep found"
      );
      Assert.AreEqual(
        new double[] { 0.0f, 5.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f), Vector3.Up).FindContacts(sphere),
        "Contact locations on sphere for Y sweep starting in sphere found"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f + outRadius, 15.0f), Vector3.Up).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f - outRadius, 0.0f, 15.0f), Vector3.Up).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 0.0f, 15.0f), Vector3.Up).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f - outRadius), Vector3.Up).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 0.0f, 15.0f + outRadius), Vector3.Up).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );

      Assert.AreEqual(
        new double[] { 10.0f, 20.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 0.0f), Vector3.Backward).FindContacts(sphere),
        "Contact locations on sphere for Z sweep found"
      );
      Assert.AreEqual(
        new double[] { 0.0f, 5.0f },
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f), Vector3.Backward).FindContacts(sphere),
        "Contact locations on sphere for Z sweep starting in sphere found"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f, 15.0f + outRadius), Vector3.Backward).FindContacts(sphere),
        "Close miss of sphere on Z axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f - outRadius, 15.0f, 0.0f), Vector3.Backward).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f + outRadius, 15.0f, 0.0f), Vector3.Backward).FindContacts(sphere),
        "Close miss of sphere on X axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f - outRadius, 0.0f), Vector3.Backward).FindContacts(sphere),
        "Close miss of sphere on Y axis properly handled"
      );
      Assert.IsNull(
        new Ray3(new Vector3(15.0f, 15.0f + outRadius, 0.0f), Vector3.Backward).FindContacts(sphere),
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

      LineContacts contacts = new Ray3(
        new Vector3(15.0f, 15.0f, 0.0f), Vector3.Backward
      ).FindContacts(plane);
      Assert.AreEqual(
        15.0f, contacts.EntryTime,
        "Direct orthogonal contact to plane detected"
      );

      contacts = new Ray3(
        new Vector3(15.0f, 15.0f, 30.0f), -Vector3.Backward
      ).FindContacts(plane);
      Assert.AreEqual(
        15.0f, contacts.EntryTime,
        "Direct orthogonal contact to plane from backside detected"
      );
    }

    /// <summary>
    ///   Ensures that the contact finding method can find a direct orthogonal ray
    ///   passing through the triangle
    /// </summary>
    [Test]
    public void TestFindOrthogonalContactOnTriangle() {
      Areas.Triangle3 triangle = new Areas.Triangle3(
        new Vector3(10.0f, 10.0f, 15.0f),
        new Vector3(20.0f, 10.0f, 15.0f),
        new Vector3(15.0f, 20.0f, 15.0f)
      );

      LineContacts contacts = new Ray3(
        new Vector3(15.0f, 15.0f, 0.0f), Vector3.Backward
      ).FindContacts(triangle);
      Assert.AreEqual(
        15.0f, contacts.EntryTime,
        "Direct orthogonal contact to triangle detected"
      );
      contacts = new Ray3(
        new Vector3(15.0f, 15.0f, 30.0f), -Vector3.Backward
      ).FindContacts(triangle);
      Assert.AreEqual(
        15.0f, contacts.EntryTime,
        "Direct orthogonal contact to triangle backside detected"
      );
    }

    /// <summary>
    ///   Ensures that the contact finding method can detect contacts with rays
    ///   touching the outside of the triangle
    /// </summary>
    [Test]
    public void TestFindSideContactOnTriangle() {
      Areas.Triangle3 triangle = new Areas.Triangle3(
        new Vector3(10.0f, 10.0f, 15.0f),
        new Vector3(20.0f, 10.0f, 15.0f),
        new Vector3(15.0f, 20.0f, 15.0f)
      );

      LineContacts contacts = new Ray3(
        new Vector3(15.0f, 5.0f, 0.0f), Vector3.Backward
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime, "Side A->B miss of triangle plane correctly filtered out"
      );

      contacts = new Ray3(
        new Vector3(10.0f, 15.0f, 0.0f), Vector3.Backward
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime, "Side A->C miss of triangle plane correctly filtered out"
      );

      contacts = new Ray3(
        new Vector3(20.0f, 15.0f, 0.0f), Vector3.Backward
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime, "Side B->C miss of triangle plane correctly filtered out"
      );

      contacts = new Ray3(
        new Vector3(15.0f, 15.0f, 15.0f + Specifications.MaximumDeviation), Vector3.Backward
      ).FindContacts(triangle);
      Assert.IsNaN(
        contacts.EntryTime, "Close orthogonal miss of triangle correctly filtered out"
      );
    }

    /// <summary>Tests whether the class is serialized properly</summary>
    [Test]
    public void TestSerialization() {
      XmlSerializer serializer = new XmlSerializer(typeof(Ray3));

      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      Ray3 ray = new Ray3(
        new Vector3(123.0f, 456.0f, 789.0f), new Vector3(987.0f, 654.0f, 321.0f)
      );
      serializer.Serialize(stream, ray);

      stream.Seek(0, System.IO.SeekOrigin.Begin);
      Ray3 restored = (Ray3)serializer.Deserialize(stream);

      Assert.AreEqual(ray, restored, "Deserialized ray matches serialized ray");
    }
  }

} // namespace Nuclex.Geometry.Lines

#endif // UNITTEST