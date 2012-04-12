#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
using System.IO;

using NUnit.Framework;

namespace Nuclex.Support.Plugins {

  /// <summary>Unit Test for the employer class</summary>
  [TestFixture]
  public class EmployerTest {

    #region class TestEmployer

    /// <summary>Dummy implementation for testing the employer class</summary>
    private class TestEmployer : Employer {

      /// <summary>Employs the specified plugin type</summary>
      /// <param name="type">Type to be employed</param>
      public override void Employ(Type type) { }

    }

    #endregion // class TestEmployer

    #region class NoDefaultConstructor

    /// <summary>Test class that doesn't have a default constructor</summary>
    private class NoDefaultConstructor {
      /// <summary>Initializes a new instance of the test class</summary>
      /// <param name="dummy">Dummy argument so this is no default constructor</param>
      public NoDefaultConstructor(int dummy) { }
    }

    #endregion // class NoDefaultConstructor

    #region class NonPublicDefaultConstructor

    /// <summary>Test class that has a non-public default constructor</summary>
    private class NonPublicDefaultConstructor {
      /// <summary>Initializes a new instance of the test class</summary>
      protected NonPublicDefaultConstructor() { }
    }

    #endregion // class NonPublicDefaultConstructor

    #region class PublicDefaultConstructor

    /// <summary>Test class that has a public default constructor</summary>
    private class PublicDefaultConstructor {
      /// <summary>Initializes a new instance of the test class</summary>
      public PublicDefaultConstructor() { }
    }

    #endregion // class PublicDefaultConstructor

    /// <summary>Tests whether the employer can detect a default constructor</summary>
    [Test]
    public void TestEmployerDefaultConstructorDetection() {
      TestEmployer testEmployer = new TestEmployer();

      Assert.IsFalse(testEmployer.CanEmploy(typeof(NoDefaultConstructor)));
      Assert.IsFalse(testEmployer.CanEmploy(typeof(NonPublicDefaultConstructor)));
      Assert.IsTrue(testEmployer.CanEmploy(typeof(PublicDefaultConstructor)));
    }

  }

} // namespace Nuclex.Support.Plugins

#endif // UNITTEST
