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

  /// <summary>Unit Test for the thread operation class</summary>
  [TestFixture]
  public class ThreadOperationTest {

    #region class TestThreadOperation

    /// <summary>Dummy operation used to run the unit tests</summary>
    private class TestThreadOperation : ThreadOperation {

      /// <summary>Initializes a dummy operation</summary>
      public TestThreadOperation() { }

      /// <summary>Initializes a dummy operation</summary>
      /// <param name="useThreadPool">Whether to use a ThreadPool thread.</param>
      public TestThreadOperation(bool useThreadPool) : base(useThreadPool) { }

      /// <summary>Contains the payload to be executed in the background thread</summary>
      protected override void Execute() { }

    }

    #endregion // class TestOperation

    #region class FailingThreadOperation

    /// <summary>Dummy operation used to run the unit tests</summary>
    private class FailingThreadOperation : ThreadOperation {

      /// <summary>Initializes a dummy operation</summary>
      /// <param name="useThreadPool">Whether to use a ThreadPool thread.</param>
      public FailingThreadOperation(bool useThreadPool) : base(useThreadPool) { }

      /// <summary>Contains the payload to be executed in the background thread</summary>
      protected override void Execute() {
        throw new AbortedException("Hello World");
      }

    }

    #endregion // class FailingThreadOperation

    /// <summary>Verifies that the default constructor for a thread operation works</summary>
    [Test]
    public void TestDefaultConstructor() {
      TestThreadOperation test = new TestThreadOperation();
      Assert.IsFalse(test.Ended);
    }

    /// <summary>
    ///   Verifies that the threaded operation can execute in a thread pool thread
    /// </summary>
    [Test]
    public void TestExecutionInThreadPool() {
      TestThreadOperation test = new TestThreadOperation(true);
      Assert.IsFalse(test.Ended);
      test.Start();
      test.Join();
      Assert.IsTrue(test.Ended);
    }

    /// <summary>
    ///   Verifies that the threaded operation can execute in an explicit thread
    /// </summary>
    [Test]
    public void TestExecutionInExplicitThread() {
      TestThreadOperation test = new TestThreadOperation(false);
      Assert.IsFalse(test.Ended);
      test.Start();
      test.Join();
      Assert.IsTrue(test.Ended);
    }

    /// <summary>
    ///   Verifies that the threaded operation forwards an exception that occurred in
    ///   a thread pool thread.
    /// </summary>
    [Test]
    public void TestForwardExceptionFromThreadPool() {
      FailingThreadOperation test = new FailingThreadOperation(true);

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
      FailingThreadOperation test = new FailingThreadOperation(false);

      Assert.IsFalse(test.Ended);
      test.Start();
      Assert.Throws<AbortedException>(
        delegate() { test.Join(); }
      );
    }

  }

} // namespace Nuclex.Support.Scheduling

#endif // UNITTEST
