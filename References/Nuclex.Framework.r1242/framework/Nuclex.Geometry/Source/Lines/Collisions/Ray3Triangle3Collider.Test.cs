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

  /// <summary>Test for the Ray3 to Triangle3 interference detection routines</summary>
  [TestFixture]
  public class Ray3Triangle3ColliderTest {

    /// <summary>
    ///   Verifies that no contact is detected if the ray begins behind the triangle
    /// </summary>
    [Test]
    public void TestRayStartingBehind() {
      Assert.IsFalse(
        Ray3Triangle3Collider.FindContacts(
          new Vector3(1.75f, 1.5f, 1.0f), Vector3.Normalize(Vector3.One),
          Vector3.Zero, Vector3.UnitX, Vector3.UnitX + Vector3.UnitY
        ).HasContact
      );
    }

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a line
    ///   that crosses a triangle through its center 
    /// </summary>
    [Test]
    public void TestHitThroughCenter() {
      LineContacts contacts = Ray3Triangle3Collider.FindContacts(
        new Vector3(-0.25f, -0.5f, -1.0f), Vector3.Normalize(Vector3.One),
        Vector3.Zero, Vector3.UnitX, Vector3.UnitX + Vector3.UnitY
      );
      float contactTime = 1.0f / Vector3.Normalize(Vector3.One).Z;

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(contactTime).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(contactTime).Within(Specifications.MaximumDeviation).Ulps
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
