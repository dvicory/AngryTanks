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

  /// <summary>Test for the Ray3 to Sphere3 interference detection routines</summary>
  [TestFixture]
  public class Line3Sphere3ColliderTest {

    /// <summary>
    ///   Tests whether a line that is barely outside of the sphere is correctly
    ///   identified as a non-hit
    /// </summary>
    [Test]
    public void TestCloseMiss() {
      Assert.IsFalse(
        Line3Sphere3Collider.FindContacts(
          new Vector3(0.0f, 10.1f, 0.0f), Vector3.Right,
          Vector3.Zero, 10.0f
        ).HasContact
      );
    }

    /// <summary>
    ///   Tests whether a line that crosses the sphere through its center causes
    ///   to right contact points to be reported
    /// </summary>
    [Test]
    public void TestCenterCrossing() {
      Vector3 diagonalUnit = Vector3.Normalize(Vector3.One);
      LineContacts contacts = Line3Sphere3Collider.FindContacts(
        diagonalUnit * -15.0f, diagonalUnit,
        Vector3.Zero, 10.0f
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(5.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(25.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
