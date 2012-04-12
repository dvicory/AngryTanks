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

  /// <summary>
  ///   Unit test for the 2D infinite line to 2D triangle interference detector
  /// </summary>
  [TestFixture]
  public class Line2Triangle2ColliderTest {

    /// <summary>
    ///   Tests whether a line that closely misses a triangle is detected as such
    /// </summary>
    [Test]
    public void TestCloseMiss() {
      Assert.IsFalse(
        Line2Triangle2Collider.FindContacts(
          new Vector2(-0.1f, 0.0f), Vector2.Normalize(Vector2.One),
          Vector2.Zero, Vector2.UnitX, Vector2.One
        ).HasContact
      );
    }

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a line
    ///   that crosses a triangle through its center from left to right
    /// </summary>
    [Test]
    public void TestHitLeftToRight() {
      LineContacts contacts = Line2Triangle2Collider.FindContacts(
        new Vector2(0.0f, 0.5f), Vector2.UnitX,
        Vector2.UnitX, Vector2.One, Vector2.Zero
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(0.5f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(1.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a line
    ///   that crosses a triangle through its center from right to left
    /// </summary>
    [Test]
    public void TestHitRightToLeft() {
      LineContacts contacts = Line2Triangle2Collider.FindContacts(
        new Vector2(2.0f, 0.5f), -Vector2.UnitX,
        Vector2.UnitX, Vector2.One, Vector2.Zero
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(1.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(1.5f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a line
    ///   that crosses a triangle through its center from top to bottom
    /// </summary>
    [Test]
    public void TestHitTopToBottom() {
      LineContacts contacts = Line2Triangle2Collider.FindContacts(
        new Vector2(0.5f, 1.0f), -Vector2.UnitY,
        Vector2.UnitX, Vector2.One, Vector2.Zero
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(0.5f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(1.0f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a line
    ///   that crosses a triangle through its center from bottom to top
    /// </summary>
    [Test]
    public void TestHitBottomToTop() {
      LineContacts contacts = Line2Triangle2Collider.FindContacts(
        new Vector2(0.5f, -1.0f), Vector2.UnitY,
        Vector2.UnitX, Vector2.One, Vector2.Zero
      );

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(1.0f).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(1.5f).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a line
    ///   that crosses a triangle through its center from right to bottom
    /// </summary>
    [Test]
    public void TestHitRightToBottom() {
      LineContacts contacts = Line2Triangle2Collider.FindContacts(
        new Vector2(1.5f, 1.0f), Vector2.Normalize(-Vector2.One),
        Vector2.UnitX, Vector2.One, Vector2.Zero
      );
      float entryTime = 0.5f / Vector2.Normalize(Vector2.One).X;
      float exitTime = 1.0f / Vector2.Normalize(Vector2.One).Y;

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(entryTime).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(exitTime).Within(Specifications.MaximumDeviation).Ulps
      );
    }

    /// <summary>
    ///   Tests whether the contact finder reports the correct locations for a line
    ///   that crosses a triangle through its center from bottom to right
    /// </summary>
    [Test]
    public void TestHitBottomToRight() {
      LineContacts contacts = Line2Triangle2Collider.FindContacts(
        new Vector2(0.0f, -0.5f), Vector2.Normalize(Vector2.One),
        Vector2.UnitX, Vector2.One, Vector2.Zero
      );
      float entryTime = 0.5f / Vector2.Normalize(Vector2.One).X;
      float exitTime = 1.0f / Vector2.Normalize(Vector2.One).Y;

      Assert.That(
        contacts.EntryTime,
        Is.EqualTo(entryTime).Within(Specifications.MaximumDeviation).Ulps
      );
      Assert.That(
        contacts.ExitTime,
        Is.EqualTo(exitTime).Within(Specifications.MaximumDeviation).Ulps
      );
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions

#endif // UNITTEST
