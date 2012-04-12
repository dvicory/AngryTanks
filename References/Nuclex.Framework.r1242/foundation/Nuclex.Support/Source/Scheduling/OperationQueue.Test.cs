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

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Scheduling {

  /// <summary>Unit Test for the operation queue class</summary>
  [TestFixture]
  public class OperationQueueTest {

    #region interface IOperationQueueSubscriber

    /// <summary>Interface used to test the operation queue</summary>
    public interface IOperationQueueSubscriber {

      /// <summary>Called when the operations queue's progress changes</summary>
      /// <param name="sender">Operation queue whose progress has changed</param>
      /// <param name="arguments">Contains the new progress achieved</param>
      void ProgressChanged(object sender, ProgressReportEventArgs arguments);

      /// <summary>Called when the operation queue has ended</summary>
      /// <param name="sender">Operation queue that as ended</param>
      /// <param name="arguments">Not used</param>
      void Ended(object sender, EventArgs arguments);

    }

    #endregion // interface IOperationQueueSubscriber

    #region class ProgressUpdateEventArgsMatcher

    /// <summary>Compares two ProgressUpdateEventArgsInstances for NMock validation</summary>
    private class ProgressUpdateEventArgsMatcher : Matcher {

      /// <summary>Initializes a new ProgressUpdateEventArgsMatcher </summary>
      /// <param name="expected">Expected progress update event arguments</param>
      public ProgressUpdateEventArgsMatcher(ProgressReportEventArgs expected) {
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

    #region class TestOperation

    /// <summary>Operation used for testing in this unit test</summary>
    private class TestOperation : Operation, IProgressReporter {

      /// <summary>will be triggered to report when progress has been achieved</summary>
      public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

      /// <summary>Begins executing the operation. Yeah, sure :)</summary>
      public override void Start() { }

      /// <summary>Moves the operation into the ended state</summary>
      public void SetEnded() {
        SetEnded(null);
      }

      /// <summary>Moves the operation into the ended state with an exception</summary>
      /// <param name="exception">Exception</param>
      public void SetEnded(Exception exception) {
        this.exception = exception;
        OnAsyncEnded();
      }

      /// <summary>Changes the testing operation's indicated progress</summary>
      /// <param name="progress">
      ///   New progress to be reported by the testing operation
      /// </param>
      public void ChangeProgress(float progress) {
        OnAsyncProgressChanged(progress);
      }

      /// <summary>
      ///   Allows the specific request implementation to re-throw an exception if
      ///   the background process finished unsuccessfully
      /// </summary>
      protected override void ReraiseExceptions() {
        if(this.exception != null)
          throw this.exception;
      }

      /// <summary>Fires the progress update event</summary>
      /// <param name="progress">Progress to report (ranging from 0.0 to 1.0)</param>
      /// <remarks>
      ///   Informs the observers of this operation about the achieved progress.
      /// </remarks>
      protected virtual void OnAsyncProgressChanged(float progress) {
        OnAsyncProgressChanged(new ProgressReportEventArgs(progress));
      }

      /// <summary>Fires the progress update event</summary>
      /// <param name="eventArguments">Progress to report (ranging from 0.0 to 1.0)</param>
      /// <remarks>
      ///   Informs the observers of this operation about the achieved progress.
      ///   Allows for classes derived from the Operation class to easily provide
      ///   a custom event arguments class that has been derived from the
      ///   operation's ProgressUpdateEventArgs class.
      /// </remarks>
      protected virtual void OnAsyncProgressChanged(ProgressReportEventArgs eventArguments) {
        EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
        if(copy != null)
          copy(this, eventArguments);
      }

      /// <summary>Exception that has occured in the background process</summary>
      private volatile Exception exception;

    }

    #endregion // class TestOperation

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Validates that the queue executes operations sequentially</summary>
    [Test]
    public void TestSequentialExecution() {
      TestOperation operation1 = new TestOperation();
      TestOperation operation2 = new TestOperation();

      OperationQueue<TestOperation> testQueueOperation =
        new OperationQueue<TestOperation>(
          new TestOperation[] { operation1, operation2 }
        );

      IOperationQueueSubscriber mockedSubscriber = mockSubscriber(testQueueOperation);

      testQueueOperation.Start();

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(OperationQueue<TestOperation>)),
            new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.25f))
          }
        );

      operation1.ChangeProgress(0.5f);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(OperationQueue<TestOperation>)),
            new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.5f))
          }
        );

      operation1.SetEnded();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the queue executes operations sequentially and honors the weights
    ///   assigned to the individual operations
    /// </summary>
    [Test]
    public void TestWeightedSequentialExecution() {
      TestOperation operation1 = new TestOperation();
      TestOperation operation2 = new TestOperation();

      OperationQueue<TestOperation> testQueueOperation =
        new OperationQueue<TestOperation>(
          new WeightedTransaction<TestOperation>[] {
            new WeightedTransaction<TestOperation>(operation1, 0.5f),
            new WeightedTransaction<TestOperation>(operation2, 2.0f)
          }
        );

      IOperationQueueSubscriber mockedSubscriber = mockSubscriber(testQueueOperation);

      testQueueOperation.Start();

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(OperationQueue<TestOperation>)),
            new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.1f))
          }
        );

      operation1.ChangeProgress(0.5f);

      Expect.Once.On(mockedSubscriber).
        Method("ProgressChanged").
        With(
          new Matcher[] {
            new NMock2.Matchers.TypeMatcher(typeof(OperationQueue<TestOperation>)),
            new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.2f))
          }
        );

      operation1.SetEnded();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Validates that the operation queue propagates the ended event once all contained
    ///   operations have completed
    /// </summary>
    [Test]
    public void TestEndPropagation() {
      TestOperation operation1 = new TestOperation();
      TestOperation operation2 = new TestOperation();

      OperationQueue<TestOperation> testQueueOperation =
        new OperationQueue<TestOperation>(
          new TestOperation[] {
            operation1,
            operation2
          }
        );

      testQueueOperation.Start();

      Assert.IsFalse(testQueueOperation.Ended);
      operation1.SetEnded();
      Assert.IsFalse(testQueueOperation.Ended);
      operation2.SetEnded();
      Assert.IsTrue(testQueueOperation.Ended);

      testQueueOperation.Join();
    }

    /// <summary>
    ///   Validates that the operation queue delivers an exception occuring in one of the
    ///   contained operations to the operation queue joiner
    /// </summary>
    [Test]
    public void TestExceptionPropagation() {
      TestOperation operation1 = new TestOperation();
      TestOperation operation2 = new TestOperation();

      OperationQueue<TestOperation> testQueueOperation =
        new OperationQueue<TestOperation>(
          new TestOperation[] {
            operation1,
            operation2
          }
        );

      testQueueOperation.Start();

      Assert.IsFalse(testQueueOperation.Ended);
      operation1.SetEnded();
      Assert.IsFalse(testQueueOperation.Ended);
      operation2.SetEnded(new AbortedException("Hello World"));

      Assert.Throws<AbortedException>(
        delegate() { testQueueOperation.Join(); }
      );
    }

    /// <summary>
    ///   Ensures that the operation queue transparently wraps the child operations in
    ///   an observation wrapper that is not visible to an outside user
    /// </summary>
    [Test]
    public void TestTransparentWrapping() {
      WeightedTransaction<TestOperation> operation1 = new WeightedTransaction<TestOperation>(
        new TestOperation()
      );
      WeightedTransaction<TestOperation> operation2 = new WeightedTransaction<TestOperation>(
        new TestOperation()
      );

      OperationQueue<TestOperation> testQueueOperation =
        new OperationQueue<TestOperation>(
          new WeightedTransaction<TestOperation>[] {
            operation1,
            operation2
          }
        );

      // Order is important due to sequential execution!
      Assert.AreSame(operation1, testQueueOperation.Children[0]);
      Assert.AreSame(operation2, testQueueOperation.Children[1]);
    }

    /// <summary>Mocks a subscriber for the events of an operation</summary>
    /// <param name="operation">Operation to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IOperationQueueSubscriber mockSubscriber(Operation operation) {
      IOperationQueueSubscriber mockedSubscriber =
        this.mockery.NewMock<IOperationQueueSubscriber>();

      operation.AsyncEnded += new EventHandler(mockedSubscriber.Ended);
      (operation as IProgressReporter).AsyncProgressChanged +=
        new EventHandler<ProgressReportEventArgs>(mockedSubscriber.ProgressChanged);

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
