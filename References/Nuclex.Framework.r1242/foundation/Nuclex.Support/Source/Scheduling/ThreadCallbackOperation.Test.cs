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
using System.Threading;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the thread callback operation class</summary>
  [TestFixture]
  public class ThreadCallbackOperationTest {

    #region class TestThreadOperation

    /// <summary>
    ///   Provides a test callback for unit testing the thread callback operation
    /// </summary>
    private class TestCallbackProvider {

      /// <summary>Method that can be invoked as a callback</summary>
      public void Callback() {
        this.called = true;
      }

      /// <summary>Whether the callback has been called</summary>
      public bool Called {
        get { return this.called; }
      }

      /// <summary>Set to true when the callback has been called</summary>
      private bool called;

    }

    #endregion // class TestOperation

    /// <summary>Verifies that the default constructor for a thread operation works</summary>
    [Test]
    public void TestDefaultConstructor() {
      ThreadCallbackOperation test = new ThreadCallbackOperation(
        new ThreadStart(errorCallback)
      );
      Assert.IsFalse(test.Ended);
    }

    /// <summary>
    ///   Verifies that the threaded operation can execute in a thread pool thread
    /// </summary>
    [Test]
    public void TestExecutionInThreadPool() {
      TestCallbackProvider callbackProvider = new TestCallbackProvider();

      ThreadCallbackOperation test = new ThreadCallbackOperation(
        new ThreadStart(callbackProvider.Callback), true
      );

      Assert.IsFalse(test.Ended);
      Assert.IsFalse(callbackProvider.Called);
      test.Start();
      test.Join();
      Assert.IsTrue(test.Ended);
      Assert.IsTrue(callbackProvider.Called);
    }

    /// <summary>
    ///   Verifies that the threaded operation can execute in an explicit thread
    /// </summary>
    [Test]
    public void TestExecutionInExplicitThread() {
      TestCallbackProvider callbackProvider = new TestCallbackProvider();

      ThreadCallbackOperation test = new ThreadCallbackOperation(
        new ThreadStart(callbackProvider.Callback), false
      );

      Assert.IsFalse(test.Ended);
      Assert.IsFalse(callbackProvider.Called);
      test.Start();
      test.Join();
      Assert.IsTrue(test.Ended);
      Assert.IsTrue(callbackProvider.Called);
    }

    /// <summary>
    ///   Verifies that the threaded operation forwards an exception that occurred in
    ///   a thread pool thread.
    /// </summary>
    [Test]
    public void TestForwardExceptionFromThreadPool() {
      ThreadCallbackOperation test = new ThreadCallbackOperation(
        new ThreadStart(errorCallback), false
      );

      Assert.IsFalse(test.Ended);
      test.Start();
      Assert.Throws<AbortedException>(
        delegate() { test.Join(); }
      );
    }

    /// <summary>
    ///   Verifies that the threaded operation forwards an exception that occurred in
    ///   an explicit thread.
    /// </summary>
    [Test]
    public void TestForwardExceptionFromExplicitThread() {
      ThreadCallbackOperation test = new ThreadCallbackOperation(
        new ThreadStart(errorCallback), false
      );

      Assert.IsFalse(test.Ended);
      test.Start();
      Assert.Throws<AbortedException>(
        delegate() { test.Join(); }
      );
    }

    /// <summary>
    ///   Callback which throws an exception to simulate an error during callback execution
    /// </summary>
    private static void errorCallback() {
      throw new AbortedException("Hello World");
    }

  }

} // namespace Nuclex.Support.Scheduling

#endif // UNITTEST
