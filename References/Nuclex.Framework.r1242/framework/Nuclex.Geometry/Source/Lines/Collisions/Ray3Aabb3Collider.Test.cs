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

  /// <summary>Test for the Ray3 to Aabb3 interference detection routines</summary>
  [TestFixture]
  public class Ray3Aabb3ColliderTest {

    /// <summary>Validates the proper behavior if the ray starts inside the box</summary>
    [Test]
    public void TestRayStartingInside() {
      Vector3 boxExtents = new Vector3(10.0f, 10.0f, 10.0f);
      LineContacts contacts = Ray3Aabb3Collider.FindContacts(
        new Vector3(-2.5f, -2.5f, -2.5f), Vector3.Up, boxExtents
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(0.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(12.5f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>Validates the proper behavior if the ray starts behind the box</summary>
    [Test]
    public void TestRayStartingBehind() {
      Vector3 boxExtents = new Vector3(10.0f, 10.0f, 10.0f);
      Assert.IsFalse(
        Ray3Aabb3Collider.FindContacts(
          new Vector3(12.5f, -2.5f, -2.5f), Vector3.Right, boxExtents
        ).HasContact
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
