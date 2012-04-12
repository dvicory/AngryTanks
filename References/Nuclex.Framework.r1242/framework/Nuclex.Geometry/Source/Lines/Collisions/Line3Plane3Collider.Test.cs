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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using NUnit.Framework;

namespace Nuclex.Geometry.Lines.Collisions {

  /// <summary>Test for the Line3 to Plane3 interference detection routines</summary>
  [TestFixture]
  public class Line3Plane3ColliderTest {

    /// <summary>
    ///   Verifies that a line that is above and parallel to a plane does not
    ///   register as a contact
    /// </summary>
    [Test]
    public void TestParallelLineAbovePlane() {
      LineContacts contacts = Line3Plane3Collider.FindContacts(
        new Vector3(0.0f, 10.0f, 0.0f), Vector3.Right,
        new Vector3(0.0f, 0.0f, 0.0f), Vector3.Up
      );

      Assert.IsFalse(contacts.HasContact);
    }

    /// <summary>
    ///   Verifies that a line that is above and parallel to a plane does not
    ///   register as a contact
    /// </summary>
    [Test]
    public void TestParallelLineBelowPlane() {
      LineContacts contacts = Line3Plane3Collider.FindContacts(
        new Vector3(-10.0f, 0.0f, 0.0f), Vector3.Up,
        new Vector3(0.0f, 0.0f, 0.0f), Vector3.Left
      );

      Assert.IsFalse(contacts.HasContact);
    }

    /// <summary>
    ///   Tests whether a contact is detected for a line crossing a plane orthogonally
    /// </summary>
    [Test]
    public void TestLineCrossingPlaneOrthogonally() {
      LineContacts contacts = Line3Plane3Collider.FindContacts(
        new Vector3(-10.0f, 0.0f, 0.0f), Vector3.Right,
        new Vector3(0.0f, 0.0f, 0.0f), Vector3.Left
      );
      float touchTime = 10.0f;

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(touchTime).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(touchTime).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether a contact is detected for a line crossing a plane diagonally
    /// </summary>
    [Test]
    public void TestLineCrossingPlaneDiagonally() {
      LineContacts contacts = Line3Plane3Collider.FindContacts(
        new Vector3(-10.0f, 0.0f, 0.0f), Vector3.Normalize(Vector3.One),
        new Vector3(0.0f, 0.0f, 0.0f), Vector3.Left
      );
      float touchTime = 10.0f / Vector3.Normalize(Vector3.One).X;

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(touchTime).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(touchTime).Within(Specifications.MaximumDeviation).Ulps
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
