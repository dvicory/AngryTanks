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

  /// <summary>Test for the Ray2 to Triangle2 interference detection routines</summary>
  [TestFixture]
  public class Ray2Triangle2ColliderTest {

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a ray
    ///   that starts inside a triangle.
    /// </summary>
    [Test]
    public void TestRayStartingInside() {
      LineContacts contacts = Ray2Triangle2Collider.FindContacts(
        new Vector2(0.75f, 0.5f), Vector2.UnitX,
        Vector2.UnitX, Vector2.One, Vector2.Zero
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(0.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(0.25f).Within(Specifications.MaximumDeviation).Ulps
      );
    }


    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a ray
    ///   that starts behind a triangle.
    /// </summary>
    [Test]
    public void TestRayStartingBehind() {
      Assert.IsFalse(
        Ray2Triangle2Collider.FindContacts(
          new Vector2(1.5f, 0.5f), Vector2.UnitX,
          Vector2.UnitX, Vector2.One, Vector2.Zero
        ).HasContact
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
