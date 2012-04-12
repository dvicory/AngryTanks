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

#if !XBOX360

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32;

using NUnit.Framework;

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the windows time source</summary>
  [TestFixture]
  public class WindowsTimeSourceTest {

    #region class TestWindowsTimeSource

    /// <summary>Windows time source used for testing</summary>
    private class TestWindowsTimeSource : WindowsTimeSource {

      /// <summary>
      ///   Forces a time change notification even if the system time was not adjusted
      /// </summary>
      public void ForceTimeChange() {
        OnDateTimeAdjusted(this, EventArgs.Empty);
      }
    }

    #endregion // class TestWindowsTimeSource

    #region class TestTimeChangedSubscriber

    /// <summary>Dummy subscriber used to test the time changed event</summary>
    private class TestTimeChangedSubscriber {

      /// <summary>Callback subscribed to the TimeChanged event</summary>
      /// <param name="sender">Not used</param>
      /// <param name="arguments">Not used</param>
      public void TimeChanged(object sender, EventArgs arguments) {
        ++this.CallCount;
      }

      /// <summary>Number of times the callback was invoked</summary>
      public int CallCount;

    }

    #endregion // class TestTimeChangedSubscriber

    /// <summary>
    ///   Verifies that the time source's default constructor is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      using(WindowsTimeSource timeSource = new WindowsTimeSource()) { }
    }

    /// <summary>
    ///   Verifies that the time source can provide the current UTC time
    /// </summary>
    [Test]
    public void TestCurrentUtcTime() {
      using(WindowsTimeSource timeSource = new WindowsTimeSource()) {

        Assert.That(
          timeSource.CurrentUtcTime, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds
        );
      }
    }

    /// <summary>
    ///   Verifies that the time source's tick property is working if
    ///   the Stopwatch class is used to measure time
    /// </summary>
    [Test]
    public void TestTicks() {
      using(WindowsTimeSource timeSource = new WindowsTimeSource()) {
        long ticks1 = timeSource.Ticks;
        long ticks2 = timeSource.Ticks;

        Assert.That(ticks2, Is.GreaterThanOrEqualTo(ticks1));
      }
    }

    /// <summary>
    ///   Verifies that the time source's WaitOne() method works correctly
    /// </summary>
    [Test]
    public void TestWaitOne() {
      using(WindowsTimeSource timeSource = new WindowsTimeSource()) {
        AutoResetEvent waitEvent = new AutoResetEvent(true);

        Assert.IsTrue(timeSource.WaitOne(waitEvent, TimeSpan.FromMilliseconds(1).Ticks));
        Assert.IsFalse(timeSource.WaitOne(waitEvent, TimeSpan.FromMilliseconds(1).Ticks));
      }
    }

    /// <summary>
    ///   Verifies that the default time source's WaitOne() method works correctly
    /// </summary>
    [Test]
    public void TestTimeChange() {
      using(TestWindowsTimeSource timeSource = new TestWindowsTimeSource()) {
        TestTimeChangedSubscriber subscriber = new TestTimeChangedSubscriber();

        EventHandler callbackDelegate = new EventHandler(subscriber.TimeChanged);
        timeSource.DateTimeAdjusted += callbackDelegate;
        try {
          timeSource.ForceTimeChange();
        }
        finally {
          timeSource.DateTimeAdjusted -= callbackDelegate;
        }
        
        // Using greater than because during the test run a real time change notification
        // might have happened, increasing the counter to 2 or more.
        Assert.That(subscriber.CallCount, Is.GreaterThanOrEqualTo(1));
      }
    }

    /// <summary>
    ///   Tests whether the Windows-specific time source can reports its availability on
    ///   the current platform
    /// </summary>
    [Test]
    public void TestAvailability() {
      bool isAvailable = WindowsTimeSource.Available;
      Assert.IsTrue(
        (isAvailable == true) ||
        (isAvailable == false)
      );
    }

  }

} // namespace Nuclex.Support.Scheduling

#endif // !XBOX360

#endif // UNITTEST
