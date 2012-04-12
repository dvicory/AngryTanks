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

  /// <summary>Unit test for the 2D ray vs. 2D disc interference detector</summary>
  [TestFixture]
  public class Ray2Disc2ColliderTest {

    /// <summary>
    ///   Verifies that the right contacts are returned for a ray that starts
    ///   inside of the disc
    /// </summary>
    [Test]
    public void TestRayStartingInside() {
      LineContacts contacts = Ray2Disc2Collider.FindContacts(
        new Vector2(0.0f, 0.0f), Vector2.UnitX, 2.0f
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

    /// <summary>
    ///   Verifies that no contacts are returned for a ray whose starting point
    ///   lies behind the disc
    /// </summary>
    [Test]
    public void TestRayStartingBehind() {
      Assert.IsFalse(
        Ray2Disc2Collider.FindContacts(
          Vector2.UnitX * 3.0f, Vector2.UnitX, 2.0f
        ).HasContact
      );
    }

    /// <summary>
    ///   Verifies that a ray starting inside an absolutely positioned disc is
    ///   correctly handled by the intersection detector
    /// </summary>
    [Test]
    public void TestRayStartingInsideWithAbsoluteDiscPosition() {
      Vector2 unitDiagonal = Vector2.Normalize(Vector2.One);
      LineContacts contacts = Ray2Disc2Collider.FindContacts(
        unitDiagonal * 4.0f, unitDiagonal, unitDiagonal * 5.0f, 2.0f
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(0.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(3.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Verifies that a ray starting behind an absolutely positioned disc causes
    ///   no contacts to be returned by the intersection detector
    /// </summary>
    [Test]
    public void TestRayStartingBehindWithAbsoluteDiscPosition() {
      Vector2 unitDiagonal = Vector2.Normalize(Vector2.One);
      Assert.IsFalse(
        Ray2Disc2Collider.FindContacts(
          unitDiagonal * 6.0f, unitDiagonal, unitDiagonal * 3.0f, 2.0f
        ).HasContact
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
