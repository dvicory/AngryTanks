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

  /// <summary>Test for the Line3 to Aabb3 interference detection routines</summary>
  [TestFixture]
  public class Line3Aabb3ColliderTest {

    /// <summary>
    ///   Tests whether a close miss of the box results in no contacts being reported
    /// </summary>
    [Test]
    public void TestCloseMiss() {
      Vector3 boxExtents = new Vector3(10.0f, 10.0f, 10.0f);
      LineContacts contacts = Line3Aabb3Collider.FindContacts(
        new Vector3(0.0f, 0.0f, 10.1f), Vector3.Right, boxExtents
      );

      Assert.IsNaN(contacts.EntryTime);
      Assert.IsNaN(contacts.ExitTime);
    }

    /// <summary>
    ///   Tests whether a close miss of the box with a diagonal line results in
    ///   no contacts being reported
    /// </summary>
    [Test]
    public void TestDiagonalCloseMiss() {
      Vector3 boxExtents = new Vector3(5.0f, 5.0f, 5.0f);
      Vector3 diagonalVector = Vector3.Normalize(Vector3.One);

      LineContacts contacts = Line3Aabb3Collider.FindContacts(
        new Vector3(-10.1f, 0.0f, 0.0f), diagonalVector, boxExtents
      );

      Assert.IsNaN(contacts.EntryTime);
      Assert.IsNaN(contacts.ExitTime);

      diagonalVector.Y = -diagonalVector.Y;

      contacts = Line3Aabb3Collider.FindContacts(
        new Vector3(0.0f, 10.1f, 0.0f), diagonalVector, boxExtents
      );

      Assert.IsNaN(contacts.EntryTime);
      Assert.IsNaN(contacts.ExitTime);
    }

    /// <summary>Tests whether a contact with a box is reported correctly</summary>
    [Test]
    public void TestContact() {
      Vector3 boxExtents = new Vector3(10.0f, 10.0f, 10.0f);
      LineContacts contacts = Line3Aabb3Collider.FindContacts(
        new Vector3(-2.5f, -2.5f, -2.5f), Vector3.Up, boxExtents
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(-7.5f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(12.5f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether a contact with a box defined by two corner points can be detected
    /// </summary>
    [Test]
    public void TestContactOnCornerDefinedBox() {
      LineContacts contacts = Line3Aabb3Collider.FindContacts(
        new Vector3(97.5f, -102.5f, 0.0f), Vector3.Up,
        new Vector3(90.0f, -110.0f, -10.0f), new Vector3(110.0f, -90.0f, 10.0f)
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(-7.5f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(12.5f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
