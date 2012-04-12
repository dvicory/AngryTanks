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

  /// <summary>Test for the Ray2 to Aabb2 interference detection routines</summary>
  [TestFixture]
  public class Ray2Aabb2ColliderTest {

    /// <summary>Validates the proper behavior if the ray starts inside the box</summary>
    [Test]
    public void TestRayStartingInside() {
      LineContacts contacts = Ray2Aabb2Collider.FindContacts(
        Vector2.Zero, Vector2.UnitX, new Vector2(2.0f, 2.0f)
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(0.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(2.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>Validates the proper behavior if the ray starts behind the box</summary>
    [Test]
    public void TestRayStartingBehind() {
      Assert.IsFalse(
        Ray2Aabb2Collider.FindContacts(
          Vector2.UnitX * 3.0f, Vector2.UnitX, new Vector2(2.0f, 2.0f)
        ).HasContact
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
