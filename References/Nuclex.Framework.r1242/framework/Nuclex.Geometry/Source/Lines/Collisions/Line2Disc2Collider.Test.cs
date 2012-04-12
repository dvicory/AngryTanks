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

  /// <summary>Unit test for the 2D line vs. 2D disc interference detector</summary>
  [TestFixture]
  public class Line2Disc2ColliderTest {

    /// <summary>
    ///   Tests whether a close miss of a circle results in no contact being reported
    /// </summary>
    [Test]
    public void TestCloseMiss() {
      Assert.IsFalse(
        Line2Disc2Collider.FindContacts(
          new Vector2(-1.0f, 2.1f), Vector2.UnitX, 2.0f
        ).HasContact
      );
    }

    /// <summary>
    ///   Tests whether a line crossing the center of a disc generates the appropriate
    ///   contact intervals.
    /// </summary>
    [Test]
    public void TestLineThroughCenter() {
      LineContacts contacts = Line2Disc2Collider.FindContacts(
        new Vector2(-3.0f, 0.0f), Vector2.UnitX, 2.0f
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(1.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(5.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether a line crossing the center of a disc generates the appropriate
    ///   contact intervals.
    /// </summary>
    [Test]
    public void TestCircleWithAbsolutePosition() {
      Vector2 unitDiagonal = Vector2.Normalize(Vector2.One);
      LineContacts contacts = Line2Disc2Collider.FindContacts(
        Vector2.Zero, unitDiagonal, unitDiagonal * 5.0f, 2.0f
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(3.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(7.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
