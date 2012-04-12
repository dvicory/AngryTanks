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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the operation class</summary>
  [TestFixture]
  public class OperationTest {

    #region class TestOperation

    /// <summary>Dummy operation used to run the unit tests</summary>
    private class TestOperation : Operation {

      /// <summary>Launches the background operation</summary>
      public override void Start() {
        // This could become a race condition of this code would be used in a fashion
        // different than what current unit tests do with it
        if(!base.Ended) {
          OnAsyncEnded();
        }
      }

    }

    #endregion // class TestOperation

    /// <summary>Tests whether operations can be started</summary>
    [Test]
    public void TestOperationStarting() {
      TestOperation myOperation = new TestOperation();
      
      Assert.IsFalse(myOperation.Ended);
      myOperation.Start();
      Assert.IsTrue(myOperation.Ended);
    }

  }

} // namespace Nuclex.Support.Scheduling

#endif // UNITTEST
