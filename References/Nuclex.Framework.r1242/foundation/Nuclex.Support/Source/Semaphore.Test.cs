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
using System.Threading;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the Semaphore class</summary>
  [TestFixture]
  public class SemaphoreTest {

    /// <summary>
    ///   Test whether a semaphore can be initialized with reverse counting
    /// </summary>
    [Test]
    public void TestReverseCountingConstructor() {
      using(Semaphore semaphore = new Semaphore()) {
        Assert.IsNotNull(semaphore); // nonsense, avoids compiler warning
      }
    }

    /// <summary>
    ///   Test whether a semaphore can be initialized with a maximum user count
    /// </summary>
    [Test]
    public void TestLimitConstructor() {
      using(Semaphore semaphore = new Semaphore(16)) {
        Assert.IsNotNull(semaphore); // nonsense, avoids compiler warning
      }
    }

    /// <summary>
    ///   Test whether a semaphore can be initialized with an initial user
    ///   count and a maximum user count
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      using(Semaphore semaphore = new Semaphore(8, 16)) {
        Assert.IsNotNull(semaphore); // nonsense, avoids compiler warning
      }
    }

    /// <summary>
    ///   Verifies that the right exception is thrown if a semaphore is initialized
    ///   with a larger number of initial users than the maximum number of users.
    /// </summary>
    [Test]
    public void TestThrowOnMoreInitialUsersThanMaximumUsers() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() {
          Semaphore semaphore = new Semaphore(2, 1);
          semaphore.Close();
        }
      );
    }

    /// <summary>
    ///   Verifies that the semaphore can time out if the resource does not become
    ///   available within the time limit specified by the user
    /// </summary>
    [Test]
    public void TestWaitTimeout() {
      using(Semaphore semaphore = new Semaphore(1)) {
        Assert.IsTrue(semaphore.WaitOne(1000));
        Assert.IsFalse(semaphore.WaitOne(0));
      }
    }

    /// <summary>
    ///   Verifies that the semaphore can time out if the resource does not become
    ///   available within the time limit specified by the user, if the time limit
    ///   is specified using the TimeSpan class
    /// </summary>
    [Test]
    public void TestWaitTimeoutWithTimeSpan() {
      using(Semaphore semaphore = new Semaphore(1)) {
        Assert.IsTrue(semaphore.WaitOne(TimeSpan.FromSeconds(1)));
        Assert.IsFalse(semaphore.WaitOne(TimeSpan.FromSeconds(0)));
      }
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the WaitOne() method is called
    ///   with a time span that is too large for the underlying synchronization API
    /// </summary>
    [Test]
    public void TestThrowOnWaitWithTooLargeTimeSpan() {
      using(Semaphore semaphore = new Semaphore(1)) {
        Assert.Throws<ArgumentOutOfRangeException>(
          delegate() {
            semaphore.WaitOne(TimeSpan.FromMilliseconds(1L << 32));
          }
        );
      }
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
