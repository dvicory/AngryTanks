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

  /// <summary>
  ///   Unit test for the 2D infinite line vs. 2d infinite line interference detector
  /// </summary>
  [TestFixture]
  public class Line2Line2ColliderTest {

    /// <summary>
    ///   Ensures no collisions are reported between two horizontal parallel lines
    /// </summary>
    [Test]
    public void TestHorizontalParallelLines() {
      Assert.IsFalse(
        Line2Line2Collider.FindContacts(
          Vector2.Zero, Vector2.UnitX,
          Vector2.Zero, Vector2.UnitX
        ).HasContact
      );
    }

    /// <summary>
    ///   Ensures no collisions are reported between two vertical parallel lines
    /// </summary>
    [Test]
    public void TestVerticalParallelLines() {
      Assert.IsFalse(
        Line2Line2Collider.FindContacts(
          Vector2.Zero, Vector2.UnitY,
          Vector2.Zero, Vector2.UnitY
        ).HasContact
      );
    }

    /// <summary>
    ///   Verifies that the intersection of two lines crossing each other
    ///   orthogonally is detected
    /// </summary>
    [Test]
    public void TestOrthogonallyCrossingLines() {
      LineContacts contacts = Line2Line2Collider.FindContacts(
        new Vector2(-1.0f, 0.0f), Vector2.UnitX,
        Vector2.Zero, Vector2.UnitY
      );
      float touchTime = 1.0f;

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
    ///   Verifies that the intersection of two lines crossing each other
    ///   diagonally is detected
    /// </summary>
    [Test]
    public void TestDiagonallyCrossingLines() {
      LineContacts contacts = Line2Line2Collider.FindContacts(
        new Vector2(1.0f, 0.0f), Vector2.Normalize(Vector2.One),
        Vector2.Zero, Vector2.UnitY
      );
      float touchTime = -1.4142135623730950488016887242097f;

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
