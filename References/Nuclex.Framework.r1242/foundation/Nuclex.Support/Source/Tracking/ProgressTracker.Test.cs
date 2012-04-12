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

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the progress tracker class</summary>
  [TestFixture]
  public class ProgressTrackerTest {

    #region interface IProgressTrackerSubscriber

    /// <summary>Interface used to test the progress tracker</summary>
    public interface IProgressTrackerSubscriber {

      /// <summary>Called when the tracked progress changes</summary>
      /// <param name="sender">Progress tracker whose progress has changed</param>
      /// <param name="arguments">Contains the new progress achieved</param>
      void ProgressChanged(object sender, ProgressReportEventArgs arguments);

      /// <summary>Called when the progress tracker's idle state changes</summary>
      /// <param name="sender">Progress tracker whose idle state has changed</param>
      /// <param name="arguments">Contains the new idle state of the tracker</param>
      void IdleStateChanged(object sender, IdleStateEventArgs arguments);

    }

    #endregion // interface IProgressTrackerSubscriber

    #region class ProgressUpdateEventArgsMatcher

    /// <summary>Compares two ProgressUpdateEventArgs instances</summary>
    private class ProgressReportEventArgsMatcher : Matcher {

      /// <summary>Initializes a new ProgressUpdateEventArgsMatcher</summary>
      /// <param name="expected">Expected progress update event arguments</param>
      public ProgressReportEventArgsMatcher(ProgressReportEventArgs expected) {
        this.expected = expected;
      }

      /// <summary>
      ///   Called by NMock to verfiy the ProgressUpdateEventArgs match the expected value
      /// </summary>
      /// <param name="actualAsObject">Actual value to compare to the expected value</param>
      /// <returns>
      ///   True if the actual value matches the expected value; otherwise false
      /// </returns>
      public override bool Matches(object actualAsObject) {
        ProgressReportEventArgs actual = (actualAsObject as ProgressReportEventArgs);
        if(actual == null)
          return false;

        return (actual.Progress == this.expected.Progress);
      }

      /// <summary>Creates a string representation of the expected value</summary>
      /// <param name="writer">Writer to write the string representation into</param>
      public override void DescribeTo(TextWriter writer) {
        writer.Write(this.expected.Progress.ToString());
      }

      /// <summary>Expected progress update event args value</summary>
      private ProgressReportEventArgs expected;

    }

    #endregion // class ProgressUpdateEventArgsMatcher

    #region class TestTransaction

    /// <summary>Transaction used for testing in this unit test</summary>
    private class TestTransaction : Transaction, IProgressReporter {

      /// <summary>will be triggered to report when progress has been achieved</summary>
      public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

      /// <summary>Changes the testing transaction's indicated progress</summary>
      /// <param name="progress">New progress to be reported by the testing transaction</param>
      public void ChangeProgress(float progress) {
        OnAsyncProgressChanged(progress);
      }

      /// <summary>Transitions the transaction into the ended state</summary>
      public void End() {
        OnAsyncEnded();
      }

      /// <summary>Fires the progress update event</summary>
      /// <param name="progress">Progress to report (ranging from 0.0 to 1.0)</param>
      /// <remarks>
      ///   Informs the observers of this transaction about the achieved progress.
      /// </remarks>
      protected virtual void OnAsyncProgressChanged(float progress) {
        OnAsyncProgressChanged(new ProgressReportEventArgs(progress));
      }

      /// <summary>Fires the progress update event</summary>
      /// <param name="eventArguments">Progress to report (ranging from 0.0 to 1.0)</param>
      /// <remarks>
      ///   Informs the observers of this transaction about the achieved progress.
      ///   Allows for classes derived from the transaction class to easily provide
      ///   a custom event arguments class that has been derived from the
      ///   transaction's ProgressUpdateEventArgs class.
      /// </remarks>
      protected virtual void OnAsyncProgressChanged(ProgressReportEventArgs eventArguments) {
        EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
        if(copy != null)
          copy(this, eventArguments);
      }

    }

    #endregion // class TestTransaction

    #region class EvilTransaction

    /// <summary>
    ///   Transaction that tries to emulate a thread giving a progress report at
    ///   a very inconvenient time ;)
    /// </summary>
    private class EvilTransaction : Transaction, IProgressReporter {

      /// <summary>will be triggered to report when progress has been achieved</summary>
      public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged {
        add { }
        remove {
          // Send a progress update right when the subscriber is trying to unsubscribe
          value(this, new ProgressReportEventArgs(0.5f));
        }
      }

    }

    #endregion // class EvilTransaction

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Validates that the tracker properly sums the progress</summary>
    [Test]
    public void TestSummedProgress() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        IProgressTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

        TestTransaction test1 = new TestTransaction();
        TestTransaction test2 = new TestTransaction();

        // Step 1
        {
          Expect.Once.On(mockedSubscriber).Method("IdleStateChanged").WithAnyArguments();

          // Since the progress is already at 0, these redundant reports are optional
          Expect.Between(0, 2).On(mockedSubscriber).Method("ProgressChanged").With(
            new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
              new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.0f))
            }
          );

          tracker.Track(test1);
          tracker.Track(test2);
        }

        // Step 2
        {
          Expect.Once.On(mockedSubscriber).Method("ProgressChanged").With(
            new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
              new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.25f))
            }
          );

          test1.ChangeProgress(0.5f);
        }
      }

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the tracker only removes transactions when the whole
    ///   tracking list has reached the 'ended' state.
    /// </summary>
    /// <remarks>
    ///   If the tracker would remove ended transactions right when they finished,
    ///   the total progress would jump back each time. This is unwanted, of course.
    /// </remarks>
    [Test]
    public void TestDelayedRemoval() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        IProgressTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

        TestTransaction test1 = new TestTransaction();
        TestTransaction test2 = new TestTransaction();

        // Step 1
        {
          Expect.Once.On(mockedSubscriber).
            Method("IdleStateChanged").
            WithAnyArguments();

          // This is optional. The tracker's progress is currently 0, so there's no need
          // to send out superfluous progress reports.
          Expect.Between(0, 2).On(mockedSubscriber).
            Method("ProgressChanged").
            With(
              new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
              new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.0f))
            }
            );

          tracker.Track(test1);
          tracker.Track(test2);
        }

        // Step 2
        {
          Expect.Once.On(mockedSubscriber).
            Method("ProgressChanged").
            With(
              new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
              new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.25f))
            }
            );

          // Total progress should be 0.25 after this call (two transactions, one with
          // 0% progress and one with 50% progress)
          test1.ChangeProgress(0.5f);
        }

        // Step 3
        {
          Expect.Once.On(mockedSubscriber).
            Method("ProgressChanged").
            With(
              new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
              new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.75f))
            }
            );

          // Total progress should be 0.75 after this call (one transaction at 100%,
          // the other one at 50%). If the second transaction would be removed by the tracker,
          // (which would be inappropriate!) the progress would falsely jump to 0.5 instead.
          test2.End();
        }

        // Step 4
        {
          Expect.Once.On(mockedSubscriber).
            Method("ProgressChanged").
            With(
              new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
              new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(1.0f))
            }
            );

          Expect.Once.On(mockedSubscriber).
            Method("IdleStateChanged").
            WithAnyArguments();

          test1.End();
        }
      }

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the tracker behaves correctly if it is fed with transactions
    ///   that have already ended.
    /// </summary>
    [Test]
    public void TestSoleEndedTransaction() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        IProgressTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

        Expect.Never.On(mockedSubscriber).Method("IdleStateChanged").WithAnyArguments();
        Expect.Never.On(mockedSubscriber).Method("ProgressChanged").WithAnyArguments();

        tracker.Track(Transaction.EndedDummy);
      }

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the tracker behaves correctly if it is fed with transactions
    ///   that have already ended in addition to transactions that are actively executing.
    /// </summary>
    [Test]
    public void TestEndedTransaction() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        IProgressTrackerSubscriber mockedSubscriber = mockSubscriber(tracker);

        TestTransaction test1 = new TestTransaction();

        // Step 1
        {
          Expect.Once.On(mockedSubscriber).
            Method("IdleStateChanged").
            WithAnyArguments();

          Expect.Between(0, 1).On(mockedSubscriber).
            Method("ProgressChanged").
            With(
              new Matcher[] {
                new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
                new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.0f))
              }
            );

          tracker.Track(test1);
        }

        // Step 2
        {
          Expect.Once.On(mockedSubscriber).
            Method("ProgressChanged").
            With(
              new Matcher[] {
                new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
                new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(0.5f))
              }
            );

          tracker.Track(Transaction.EndedDummy);
        }

        // Step 3
        {
          Expect.Once.On(mockedSubscriber).
            Method("ProgressChanged").
            With(
              new Matcher[] {
                new NMock2.Matchers.TypeMatcher(typeof(ProgressTracker)),
                new ProgressReportEventArgsMatcher(new ProgressReportEventArgs(1.0f))
              }
            );

          Expect.Once.On(mockedSubscriber).
            Method("IdleStateChanged").
            WithAnyArguments();

          test1.End();
        }
      }

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Tries to provoke a deadlock by re-entering the tracker from one of its own events
    /// </summary>
    [Test]
    public void TestProvokedDeadlock() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        TestTransaction test1 = new TestTransaction();
        tracker.Track(test1);

        tracker.AsyncIdleStateChanged +=
          (EventHandler<IdleStateEventArgs>)delegate(object sender, IdleStateEventArgs arguments) {
          tracker.Track(Transaction.EndedDummy);
        };

        test1.End();
      }
    }

    /// <summary>
    ///   Tests whether the progress tracker enters and leaves the idle state correctly
    ///   when a transaction is removed via Untrack()
    /// </summary>
    [Test]
    public void TestIdleWithUntrack() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        TestTransaction test1 = new TestTransaction();

        Assert.IsTrue(tracker.Idle);

        tracker.Track(test1);

        Assert.IsFalse(tracker.Idle);

        tracker.Untrack(test1);

        Assert.IsTrue(tracker.Idle);
      }
    }

    /// <summary>
    ///   Tests whether the progress tracker enters and leaves the idle state correctly
    ///   when a transaction is removed the transaction finishing
    /// </summary>
    [Test]
    public void TestIdleWithAutoRemoval() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        TestTransaction test1 = new TestTransaction();

        Assert.IsTrue(tracker.Idle);

        tracker.Track(test1);

        Assert.IsFalse(tracker.Idle);

        test1.End();

        Assert.IsTrue(tracker.Idle);
      }
    }

    /// <summary>
    ///   Tests whether the progress tracker enters and leaves the idle state correctly
    ///   when a transaction is removed via Untrack()
    /// </summary>
    [Test]
    public void TestProgressWithUntrack() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        TestTransaction test1 = new TestTransaction();
        TestTransaction test2 = new TestTransaction();
        tracker.Track(test1);
        tracker.Track(test2);

        Assert.AreEqual(0.0f, tracker.Progress);

        test1.ChangeProgress(0.5f);

        Assert.AreEqual(0.25f, tracker.Progress);

        tracker.Untrack(test2);

        Assert.AreEqual(0.5f, tracker.Progress);
      }
    }

    /// <summary>
    ///   Verifies that the progress tracker throws an exception if it is instructed
    ///   to untrack a transaction it doesn't know about
    /// </summary>
    [Test]
    public void TestThrowOnUntrackNonTrackedTransaction() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        TestTransaction test1 = new TestTransaction();

        Assert.Throws<ArgumentException>(
          delegate() { tracker.Untrack(test1); }
        );
      }
    }

    /// <summary>
    ///   Verifies that the progress tracker throws an exception if it is instructed
    ///   to untrack a transaction it doesn't know about
    /// </summary>
    [Test]
    public void TestProgressReportDuringUnsubscribe() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        EvilTransaction evil = new EvilTransaction();
        tracker.Track(evil);
        tracker.Untrack(evil);
      }
    }

    /// <summary>
    ///   Verifies that the progress tracker doesn't choke on a transaction being tracked
    ///   multiple times.
    /// </summary>
    [Test]
    public void TestMultiTrackedTransaction() {
      using(ProgressTracker tracker = new ProgressTracker()) {
        TestTransaction test = new TestTransaction();
        tracker.Track(test);
        tracker.Track(test);
        tracker.Track(test);
        tracker.Untrack(test);
        tracker.Untrack(test);
        tracker.Untrack(test);
      }
    }

    /// <summary>Mocks a subscriber for the events of a tracker</summary>
    /// <param name="tracker">Tracker to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IProgressTrackerSubscriber mockSubscriber(ProgressTracker tracker) {
      IProgressTrackerSubscriber mockedSubscriber =
        this.mockery.NewMock<IProgressTrackerSubscriber>();

      tracker.AsyncIdleStateChanged +=
        new EventHandler<IdleStateEventArgs>(mockedSubscriber.IdleStateChanged);

      tracker.AsyncProgressChanged +=
        new EventHandler<ProgressReportEventArgs>(mockedSubscriber.ProgressChanged);

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
