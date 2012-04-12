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

  /// <summary>Test for the Line3 to Triangle3 interference detection routines</summary>
  [TestFixture]
  public class Line3Triangle3ColliderTest {

    /// <summary>
    ///   Tests whether a line that closely misses a triangle is detected as such
    /// </summary>
    [Test]
    public void TestCloseMiss() {
      Assert.IsFalse(
        Line3Triangle3Collider.FindContacts(
          new Vector3(0.0f, 0.5f, 0.0f), Vector3.Right,
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
      LineContacts contacts = Line3Triangle3Collider.FindContacts(
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
