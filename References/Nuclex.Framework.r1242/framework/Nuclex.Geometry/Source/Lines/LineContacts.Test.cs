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

using NUnit.Framework;

namespace Nuclex.Geometry.Lines {

  /// <summary>Test for the LineContacts class</summary>
  [TestFixture]
  public class LineContactsTest {

    /// <summary>
    ///   Verifies that the constructor for a single contact point is working
    /// </summary>
    [Test]
    public void TestSingleContactConstructor() {
      LineContacts contacts = new LineContacts(123.456f);

      Assert.IsTrue(contacts.HasContact);
      Assert.AreEqual(123.456f, contacts.EntryTime);
      Assert.AreEqual(123.456f, contacts.ExitTime);
    }

    /// <summary>
    ///   Tests the constructor with normal entry and exit contact times
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      LineContacts contacts = new LineContacts(12.34f, 56.78f);

      Assert.IsTrue(contacts.HasContact);
      Assert.AreEqual(12.34f, contacts.EntryTime);
      Assert.AreEqual(56.78f, contacts.ExitTime);
    }

    /// <summary>Verifies that the LineContacts.None constant has no contacts</summary>
    [Test]
    public void TestNoneConstant() {
      Assert.IsFalse(LineContacts.None.HasContact);
    }

    /// <summary>
    ///   Verifies that the GetHashCode() method returns the same hash code for
    ///   two identical instances
    /// </summary>
    [Test]
    public void TestGetHashCode() {
      LineContacts contacts1 = new LineContacts(12.34f, 56.78f);
      LineContacts contacts2 = new LineContacts(12.34f, 56.78f);

      Assert.AreEqual(contacts1.GetHashCode(), contacts2.GetHashCode());
    }

    /// <summary>
    ///   Verifies that the Equals() method is working correctly
    /// </summary>
    [Test]
    public void TestEqualityComparison() {
      LineContacts contacts = new LineContacts(12.34f, 56.78f);
      LineContacts identical = new LineContacts(12.34f, 56.78f);
      LineContacts different = new LineContacts(56.78f, 12.34f);

      Assert.IsTrue(contacts.Equals(identical));
      Assert.IsFalse(contacts.Equals(different));
    }

  }

} // namespace Nuclex.Geometry.Lines

#endif // UNITTEST
