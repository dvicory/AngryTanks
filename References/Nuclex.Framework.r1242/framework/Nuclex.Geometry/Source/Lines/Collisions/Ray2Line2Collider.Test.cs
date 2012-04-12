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

  /// <summary>Test for the Ray2 to Line2 interference detection routines</summary>
  [TestFixture]
  public class Ray2Line2ColliderTest {

    /// <summary>Ensures no collisions are reported between two parallel rays</summary>
    /// <remarks>
    ///   Even if two rays start at exactly the same place, no collision will be reported
    ///   for parallel lines. Think of lines as infinitely thin - they do not take up any
    ///   space and it's infinitely unlikely to touch another line if you're not crossing
    ///   it. In addition to that, the contacts would be rays, not points and couldn't
    ///   be returned in the LineContacts structure.
    /// </remarks>
    [Test]
    public void TestRayParallelToLine() {
      Assert.IsFalse(
        Ray2Line2Collider.FindContacts(
          Vector2.Zero, Vector2.UnitY,
          Vector2.Zero, Vector2.UnitY
        ).HasContact
      );

      Assert.IsFalse(
        Ray2Line2Collider.FindContacts(
          Vector2.Zero, Vector2.UnitX,
          Vector2.Zero, Vector2.UnitX
        ).HasContact
      );
    }

    /// <summary>Validates that the intersection of two crossing lines is detected</summary>
    [Test]
    public void TestRayCrossingLine() {
      LineContacts contacts = Ray2Line2Collider.FindContacts(
        new Vector2(-1.0f, 0.0f), Vector2.UnitX,
        Vector2.Zero, Vector2.UnitY
      );
      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(1.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>Validates that a ray behind a crossing line results in no collision</summary>
    [Test]
    public void TestRayBehindCrossingLine() {
      Assert.IsFalse(
        Ray2Line2Collider.FindContacts(
          new Vector2(1.0f, 0.0f), Vector2.Normalize(new Vector2(0.5f, 0.5f)),
          Vector2.Zero, Vector2.UnitY
        ).HasContact
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
