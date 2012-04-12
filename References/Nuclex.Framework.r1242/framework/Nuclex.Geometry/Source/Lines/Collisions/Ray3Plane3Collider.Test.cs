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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Geometry.Lines.Collisions {

  /// <summary>Test for the Ray3 to Plane3 interference detection routines</summary>
  [TestFixture]
  public class Ray3Plane3ColliderTest {

    /// <summary>Tests the collider with a ray pointing away from the plane</summary>
    [Test]
    public void TestPointingAway() {
      Assert.IsFalse(
        Ray3Plane3Collider.FindContacts(
          new Vector3(0.0f, 100.0f, 0.0f), Vector3.Up, Vector3.Zero, Vector3.Up
        ).HasContact
      );
    }

    /// <summary>Tests the collider with a ray pointing towards the plane</summary>
    [Test]
    public void TestPointingTowards() {
      LineContacts contacts = Ray3Plane3Collider.FindContacts(
        new Vector3(0.0f, 100.0f, 0.0f), Vector3.Down, Vector3.Zero, Vector3.Up
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(100.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
