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
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32;

using NUnit.Framework;

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the scheduler</summary>
  [TestFixture]
  public class SchedulerTest {

    #region class MockTimeSource

    /// <summary>Mocked time source</summary>
    private class MockTimeSource : ITimeSource {

      /// <summary>Called when the system date/time are adjusted</summary>
      public event EventHandler DateTimeAdjusted;

      /// <summary>Initializes a new mocked time source</summary>
      /// <param name="utcStartTime">Start time in UTC format</param>
      public MockTimeSource(DateTime utcStartTime) {
        this.currentTime = utcStartTime;
        this.currentTicks = 1000000000;
      }

      /// <summary>Waits for an AutoResetEvent to become signalled</summary>
      /// <param name="waitHandle">WaitHandle the method will wait for</param>
      /// <param name="ticks">Number of ticks to wait</param>
      /// <returns>
      ///   True if the WaitHandle was signalled, false if the timeout was reached
      ///   or the time source thinks its time to recheck the system date/time.
      /// </returns>
      public bool WaitOne(AutoResetEvent waitHandle, long ticks) {
        long currentTicks;
        long eventDueTicks;
        lock(this) {
          this.autoResetEvent = waitHandle;
          this.eventDueTicks += ticks;

          currentTicks = this.currentTicks;
          eventDueTicks = this.eventDueTicks;
        }

        // If we need to wait, use the wait handle. We do not use the wait handle's
        // return value (or even its timeout) because we might trigger it ourselves
        // to simulate the passing of time.
        if(eventDueTicks > 0) {
          this.autoResetEvent = waitHandle;
          waitHandle.WaitOne();
          this.autoResetEvent = null;
        }

        // Do not use the cached values here -- we might have used the WaitHandle and
        // the simulation time could have been advanced while we were waiting.
        lock(this) {
          return (this.eventDueTicks > 0); // True = signalled, false = timed out
        }
      }

      /// <summary>Current system time in UTC format</summary>
      public DateTime CurrentUtcTime {
        get { lock(this) { return this.currentTime; } }
      }

      /// <summary>How long the time source has been running</summary>
      public long Ticks {
        get {
          lock(this) {
            this.eventDueTicks = 0;
            return this.currentTicks;
          }
        }
      }

      /// <summary>Advances the time of the time source</summary>
      /// <param name="timeSpan">
      ///   Time span by which to advance the time source's time
      /// </param>
      public void AdvanceTime(TimeSpan timeSpan) {
        lock(this) {
          this.currentTicks += timeSpan.Ticks;
          this.currentTime += timeSpan;

          // Problem: The Scheduler has just calculated the remaining ticks until
          //          notification occurs. Next, another thread advances simulation time
          //          and then the scheduler calls this. It will wait, even though
          //          the simultion time has progressed.
          // To compensate this, we track the remaining time until the event is due
          // and allow for a negative time budget if AdvanceTime() is called after
          // the scheduler has just queried the current tick count.
          this.eventDueTicks -= timeSpan.Ticks;

          if(this.eventDueTicks <= 0) {
            AutoResetEvent copy = this.autoResetEvent;
            if(copy != null) {
              copy.Set();
            }
          }
        }
      }

      /// <summary>Manually triggers the date time adjusted event</summary>
      /// <param name="newUtcTime">New simulation time to jump to</param>
      public void AdjustTime(DateTime newUtcTime) {
        lock(this) {
          this.currentTime = newUtcTime;
        }
        EventHandler copy = DateTimeAdjusted;
        if(copy != null) {
          copy(this, EventArgs.Empty);
        }
      }

      /// <summary>Auto reset event the time source is currently waiting on</summary>
      private volatile AutoResetEvent autoResetEvent;
      /// <summary>Ticks at which the auto reset event will be due</summary>
      private long eventDueTicks;
      /// <summary>Current time source tick counter</summary>
      private long currentTicks;
      /// <summary>Current system time and date</summary>
      private DateTime currentTime;

    }

    #endregion // class MockTimeSource

    #region class TestSubscriber

    /// <summary>Subscriber used to test the scheduler notifications</summary>
    private class TestSubscriber : IDisposable {

      /// <summary>Initializes a new test subscriber</summary>
      public TestSubscriber() {
        this.waitHandle = new AutoResetEvent(false);
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.waitHandle != null) {
          this.waitHandle.Close();
          this.waitHandle = null;
        }
      }

      /// <summary>Callback method that can be subscribed to the scheduler</summary>
      /// <param name="state">Not used</param>
      public void Callback(object state) {
        Interlocked.Increment(ref this.callbackCount);
        this.waitHandle.Set();
      }

      /// <summary>Blocks ther caller until the callback is invoked</summary>
      /// <param name="milliseconds">
      ///   Maximum number of milliseconds to wait for the callback
      /// </param>
      /// <returns>True if the callback was invoked, false if the call timed out</returns>
      public bool WaitForCallback(int milliseconds) {
        return this.waitHandle.WaitOne(milliseconds);
      }

      /// <summary>Number of times the callback has been invoked</summary>
      public int CallbackCount {
        get { return Thread.VolatileRead(ref this.callbackCount); }
      }

      /// <summary>Callback invocation count</summary>
      private int callbackCount;
      /// <summary>WaitHandle used to wait for the callback</summary>
      private AutoResetEvent waitHandle;

    }

    #endregion // class TestSubscriber

    /// <summary>
    ///   Test whether the Scheduler can explicitely create a windows time source
    /// </summary>
    [Test]
    public void TestCreateWindowsTimeSource() {
      ITimeSource timeSource = Scheduler.CreateTimeSource(true);
      try {
        Assert.That(timeSource is WindowsTimeSource);
      } finally {
        IDisposable disposableTimeSource = timeSource as IDisposable;
        if(disposableTimeSource != null) {
          disposableTimeSource.Dispose();
        }
      }
    }

    /// <summary>
    ///   Test whether the Scheduler can explicitely create a generic time source
    /// </summary>
    [Test]
    public void TestCreateGenericTimeSource() {
      ITimeSource timeSource = Scheduler.CreateTimeSource(false);
      try {
        Assert.That(timeSource is GenericTimeSource);
      } finally {
        IDisposable disposableTimeSource = timeSource as IDisposable;
        if(disposableTimeSource != null) {
          disposableTimeSource.Dispose();
        }
      }
    }

    /// <summary>
    ///   Test whether the Scheduler can automatically choose the right time source
    /// </summary>
    [Test]
    public void TestCreateDefaultTimeSource() {
      ITimeSource timeSource = Scheduler.CreateDefaultTimeSource();
      try {
        Assert.IsNotNull(timeSource);
      } finally {
        IDisposable disposableTimeSource = timeSource as IDisposable;
        if(disposableTimeSource != null) {
          disposableTimeSource.Dispose();
        }
      }
    }

    /// <summary>
    ///   Verifies that the default constructor of the scheduler is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      using(Scheduler scheduler = new Scheduler()) { }
    }

    /// <summary>
    ///   Verifies that the default constructor of the scheduler is working
    /// </summary>
    [Test]
    public void TestThrowOnNotifyAtWithUnspecifiedDateTimeKind() {
      using(TestSubscriber subscriber = new TestSubscriber()) {
        using(Scheduler scheduler = new Scheduler()) {
          Assert.Throws<ArgumentException>(
            delegate() {
              scheduler.NotifyAt(new DateTime(2000, 1, 1), subscriber.Callback);
            }
          );
        }
      }
    }

    /// <summary>
    ///   Tests whether the NotifyAt() method invokes the callback at the right time
    /// </summary>
    [Test]
    public void TestNotifyAt() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber = new TestSubscriber()) {
        using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
          scheduler.NotifyAt(makeUtc(new DateTime(2010, 1, 2)), subscriber.Callback);

          mockTimeSource.AdvanceTime(TimeSpan.FromDays(1));

          Assert.IsTrue(subscriber.WaitForCallback(1000));
        }
      }
    }

    /// <summary>
    ///   Verifies that a notification at an absolute time is processed correctly
    ///   if a time synchronization occurs during the wait.
    /// </summary>
    [Test]
    public void TestNotifyAtWithDateTimeAdjustment() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber = new TestSubscriber()) {
        using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
          scheduler.NotifyAt(makeUtc(new DateTime(2010, 1, 2)), subscriber.Callback);

          // Let 12 hours pass, after that, we simulate a time synchronization
          // that puts the system 12 hours ahead of the original time.
          mockTimeSource.AdvanceTime(TimeSpan.FromHours(12));
          mockTimeSource.AdjustTime(new DateTime(2010, 1, 2));

          Assert.IsTrue(subscriber.WaitForCallback(1000));
        }
      }
    }

    /// <summary>Tests whether the scheduler's Cancel() method is working</summary>
    [Test]
    public void TestCancelNotification() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber1 = new TestSubscriber()) {
        using(TestSubscriber subscriber2 = new TestSubscriber()) {
          using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
            object handle = scheduler.NotifyIn(
              TimeSpan.FromHours(24), subscriber1.Callback
            );
            scheduler.NotifyIn(TimeSpan.FromHours(36), subscriber2.Callback);

            mockTimeSource.AdvanceTime(TimeSpan.FromHours(12));
            scheduler.Cancel(handle);
            mockTimeSource.AdvanceTime(TimeSpan.FromHours(24));

            // Wait for the second subscriber to be notified. This is still a race
            // condition (there's no guarantee the thread pool will run tasks in
            // the order they were added), but it's the best we can do without
            // relying on an ugly Thread.Sleep() in this test.
            Assert.IsTrue(subscriber2.WaitForCallback(1000));
            Assert.AreEqual(0, subscriber1.CallbackCount);
          }
        }
      }
    }

    /// <summary>
    ///   Tests the scheduler with two notifications that are scheduled in inverse
    ///   order of their due time.
    /// </summary>
    [Test]
    public void TestInverseOrderNotification() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber1 = new TestSubscriber()) {
        using(TestSubscriber subscriber2 = new TestSubscriber()) {
          using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
            scheduler.NotifyIn(TimeSpan.FromHours(24), subscriber1.Callback);
            scheduler.NotifyIn(TimeSpan.FromHours(12), subscriber2.Callback);

            mockTimeSource.AdvanceTime(TimeSpan.FromHours(18));

            Assert.IsTrue(subscriber2.WaitForCallback(1000));
            Assert.AreEqual(0, subscriber1.CallbackCount);

            mockTimeSource.AdvanceTime(TimeSpan.FromHours(18));

            Assert.IsTrue(subscriber1.WaitForCallback(1000));
          }
        }
      }
    }

    /// <summary>
    ///   Tests the scheduler with two notifications that are scheduled to
    ///   occur at the exact same time
    /// </summary>
    [Test]
    public void TestTwoNotificationsAtSameTime() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber1 = new TestSubscriber()) {
        using(TestSubscriber subscriber2 = new TestSubscriber()) {
          using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
            scheduler.NotifyIn(60000, subscriber1.Callback);
            scheduler.NotifyIn(60000, subscriber2.Callback);

            mockTimeSource.AdvanceTime(TimeSpan.FromMilliseconds(60000));

            Assert.IsTrue(subscriber1.WaitForCallback(1000));
            Assert.IsTrue(subscriber2.WaitForCallback(1000));
          }
        }
      }
    }

    /// <summary>
    ///   Verifies that the scheduler's NotifyEach() method is working correctly
    /// </summary>
    [Test]
    public void TestNotifyEachWithMilliseconds() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber = new TestSubscriber()) {
        using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
          scheduler.NotifyEach(1000, 1000, subscriber.Callback);

          mockTimeSource.AdvanceTime(TimeSpan.FromMilliseconds(4000));

          // Wait for 4 invocations of the callback. We might not catch each trigger
          // of the AutoResetEvent, but we know that it will be 4 at most. 
          for(int invocation = 0; invocation < 4; ++invocation) {
            Assert.IsTrue(subscriber.WaitForCallback(1000));

            if(subscriber.CallbackCount == 4) {
              break;
            }
          }
        }
      }
    }

    /// <summary>
    ///   Verifies that the scheduler's NotifyEach() method is working correctly
    /// </summary>
    [Test]
    public void TestNotifyEachWithTimespan() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber = new TestSubscriber()) {
        using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
          scheduler.NotifyEach(
            TimeSpan.FromHours(12), TimeSpan.FromHours(1), subscriber.Callback
          );

          mockTimeSource.AdvanceTime(TimeSpan.FromHours(14));

          // Wait for 3 invocations of the callback. We might not catch each trigger
          // of the AutoResetEvent, but we know that it will be 3 at most. 
          for(int invocation = 0; invocation < 3; ++invocation) {
            Assert.IsTrue(subscriber.WaitForCallback(1000));

            if(subscriber.CallbackCount == 3) {
              break;
            }
          }
        }
      }
    }

    /// <summary>
    ///   Reproduction case for a bug that occurred when the final notification in
    ///   the scheduler was cancelled (call to PriorityQueue.Peek() on empty queue)
    /// </summary>
    [Test]
    public void TestCancelFinalNotification() {
      MockTimeSource mockTimeSource = new MockTimeSource(new DateTime(2010, 1, 1));
      using(TestSubscriber subscriber = new TestSubscriber()) {
        using(Scheduler scheduler = new Scheduler(mockTimeSource)) {
          scheduler.Cancel(
            scheduler.NotifyIn(TimeSpan.FromHours(12), subscriber.Callback)
          );

          mockTimeSource.AdvanceTime(TimeSpan.FromHours(14));
          Thread.Sleep(1);
        }
      }
    }

    // TODO: Unit testing caused this exception
    //
    // Nuclex.Support.Scheduling.SchedulerTest.TestThrowOnNotifyAtWithUnspecifiedDateTimeKind :
    //   System.NullReferenceException: Der Objektverweis wurde nicht auf
    //     eine Objektinstanz festgelegt.
    //   bei Nuclex.Support.Scheduling.SchedulerTest.TestSubscriber.Callback(Object state)
    //     in D:\Devel\framework\Nuclex.Support\Source\Scheduling\Scheduler.Test.cs:Zeile 177.
    //   bei System.Threading._ThreadPoolWaitCallback.WaitCallback_Context(Object state)
    //   bei System.Threading.ExecutionContext.Run(
    //     ExecutionContext executionContext, ContextCallback callback, Object state
    //   )
    //   bei System.Threading._ThreadPoolWaitCallback.PerformWaitCallbackInternal(
    //     _ThreadPoolWaitCallback tpWaitCallBack
    //   )
    //   bei System.Threading._ThreadPoolWaitCallback.PerformWaitCallback(Object state)

    /// <summary>Returns the provided date/time value as a utc time value</summary>
    /// <param name="dateTime">Date/time value that will be returned as UTC</param>
    /// <returns>The provided date/time value as UTC</returns>
    /// <remarks>
    ///   This doesn't convert the time, it just returns the exact same date and time
    ///   but tags it as UTC by setting the DateTimeKind to UTC.
    /// </remarks>
    private static DateTime makeUtc(DateTime dateTime) {
      return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
    }

  }

} // namespace Nuclex.Support.Scheduling

#endif // UNITTEST
