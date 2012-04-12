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

using System;
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the shared instance provider class</summary>
  [TestFixture]
  public class SharedTest {

    #region class Dummy

    /// <summary>Dummy class for testing the shared instance provider</summary>
    private class Dummy {
      /// <summary>Initializes a new dummy</summary>
      public Dummy() {}
    }

    #endregion // class Dummy

    /// <summary>
    ///   Verifies that the shared instance provider returns the same instance of a class
    ///   when asked for the same class twice.
    /// </summary>
    [Test]
    public void TestSameInstance() {
      Dummy dummyInstance = Shared<Dummy>.Instance;
      Dummy otherDummyInstance = Shared<Dummy>.Instance;
      
      // Make sure they're the same instance. We could have put an instance counter in
      // the dummy class, but this might or might not work well across multiple tests
      // because the order in which tests are executed is undefined and Shared<> changes
      // its global state when the first test is run by remembering the instance.
      //
      // Maybe this really is a defect in Shared<> and the class should be equipped with
      // a method such as Discard() or Dispose() to get rid of the instance?
      Assert.AreSame(dummyInstance, otherDummyInstance);
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
