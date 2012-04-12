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

using Nuclex.Support.Scheduling;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the request class</summary>
  [TestFixture]
  public class RequestTest {

    #region class CustomWaitRequest

    /// <summary>
    ///   Request with a custom wait implementation that completes the request instead
    ///   of waiting for it complete by outside means
    /// </summary>
    private class CustomWaitRequest : Request {

      /// <summary>Waits until the background process finishes</summary>
      public override void Wait() {
        // This could be a race condition if this was used for anything but this simple
        // unit test. Might be neccessary to refactor this when writing advanced tests.
        if(!base.Ended) {
          OnAsyncEnded();
        }
      }

    }

    #endregion // class CustomWaitRequest

    /// <summary>
    ///   Verifies that the SucceededDummy request is in the ended state
    /// </summary>
    [Test]
    public void TestSucceededDummy() {
      Request dummy = Request.SucceededDummy;

      Assert.IsTrue(dummy.Ended);
      dummy.Join(); // should not throw
    }

    /// <summary>
    ///   Verifies that the FailedDummy request is in the ended state and throws
    ///   an exception when Join()ing
    /// </summary>
    [Test]
    public void TestFailedDummy() {
      Request failedDummy = Request.CreateFailedDummy(
        new AbortedException("Hello World")
      );

      Assert.IsTrue(failedDummy.Ended);
      Assert.Throws<AbortedException>(
        delegate() { failedDummy.Join(); }
      );
    }

    /// <summary>
    ///   Verifies that the Request's Wait() method is invoked if the request is joined
    ///   before the request has finished.
    /// </summary>
    [Test]
    public void TestJoinWithWaiting() {
      CustomWaitRequest waitRequest = new CustomWaitRequest();
      waitRequest.Join();
      Assert.IsTrue(waitRequest.Ended);
    }


  }

  /// <summary>Unit Test for the generic request class</summary>
  [TestFixture]
  public class GenericRequestTest {

    /// <summary>
    ///   Verifies that the SucceededDummy request is in the ended state
    /// </summary>
    [Test]
    public void TestSucceededDummy() {
      Request<int> dummy = Request<int>.CreateSucceededDummy(12345);

      Assert.IsTrue(dummy.Ended);
      Assert.AreEqual(12345, dummy.Join()); // should not throw
    }

    /// <summary>
    ///   Verifies that the FailedDummy request is in the ended state and throws
    ///   an exception when Join()ing
    /// </summary>
    [Test]
    public void TestFailedDummy() {
      Request<int> failedDummy = Request<int>.CreateFailedDummy(
        new AbortedException("Hello World")
      );

      Assert.IsTrue(failedDummy.Ended);
      Assert.Throws<AbortedException>(
        delegate() { failedDummy.Join(); }
      );
    }

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
