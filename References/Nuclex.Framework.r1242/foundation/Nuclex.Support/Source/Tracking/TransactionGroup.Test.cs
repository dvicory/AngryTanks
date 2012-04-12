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
using System.Collections.Generic;
using System.IO;
using System.Threading;

#if UNITTEST

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the transaction group class</summary>
  [TestFixture]
  public class TransactionGroupTest {

    #region interface ITransactionGroupSubscriber

    /// <summary>Interface used to test the transaction group</summary>
    public interface ITransactionGroupSubscriber {

      /// <summary>Called when the transaction group's progress changes</summary>
      /// <param name="sender">Transaction group whose progress has changed</param>
      /// <param name="arguments">Contains the new progress achieved</param>
      void ProgressChanged(object sender, ProgressReportEventArgs arguments);

      /// <summary>Called when the transaction group has ended</summary>
      /// <param name="sender">Transaction group that as ended</param>
      /// <param name="arguments">Not used</param>
      void Ended(object sender, EventArgs arguments);

    }

    #endregion // interface ITransactionGroupSubscriber

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

    #region class TestTransaction

    /// <summary>Transaction used for testing in this unit test</summary>
    private class TestTransaction : Transaction, IProgressReporter {

      /// <summary>will be triggered to report when progress has been achieved</summary>
      public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

      /// <summary>Changes the testing transaction's indicated progress</summary>
      /// <param name="progress">
      ///   New progress to be reported by the testing transaction
      /// </param>
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

    #region class ChainEndingTransaction

    /// <summary>
    ///   Transaction that ends another transaction when its Ended property is called
    /// </summary>
    private class ChainEndingTransaction : Transaction {

      /// <summary>Initializes a new chain ending transaction</summary>
      public ChainEndingTransaction() {
        this.chainedTransaction = new TestTransaction();
      }

      /// <summary>Transitions the transaction into the ended state</summary>
      public void End() {
        OnAsyncEnded();
      }

      /// <summary>
      ///   Transaction that will end when this transaction's ended property is accessed
      /// </summary>
      public TestTransaction ChainedTransaction {
        get { return this.chainedTransaction; }
      }

      /// <summary>Whether the transaction has ended already</summary>
      public override bool Ended {
        get {
          if(Interlocked.Exchange(ref this.endedCalled, 1) == 0) {
            this.chainedTransaction.End();
          }

          return base.Ended;
        }
      }

      /// <summary>
      ///   Transaction that will end when this transaction's ended property is accessed
      /// </summary>
      private TestTransaction chainedTransaction;

      /// <summary>Whether we already ended the chained transaction and ourselves</summary>
      private int endedCalled;

    }

    #endregion // class ChainEndingTransaction

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Validates that the transaction group correctly sums the progress</summary>
    [Test]
    public void TestSummedProgress() {
      using(
        TransactionGroup<TestTransaction> testTransactionGroup =
          new TransactionGroup<TestTransaction>(
            new TestTransaction[] { new TestTransaction(), new TestTransaction() }
          )
      ) {
        ITransactionGroupSubscriber mockedSubscriber = mockSubscriber(testTransactionGroup);

        Expect.Once.On(mockedSubscriber).
          Method("ProgressChanged").
          With(
            new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(TransactionGroup<TestTransaction>)),
              new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.25f))
            }
          );

        testTransactionGroup.Children[0].Transaction.ChangeProgress(0.5f);

        this.mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>Validates that the transaction group respects the weights</summary>
    [Test]
    public void TestWeightedSummedProgress() {
      using(
        TransactionGroup<TestTransaction> testTransactionGroup =
          new TransactionGroup<TestTransaction>(
            new WeightedTransaction<TestTransaction>[] {
              new WeightedTransaction<TestTransaction>(new TestTransaction(), 1.0f),
              new WeightedTransaction<TestTransaction>(new TestTransaction(), 2.0f)
            }
          )
      ) {
        ITransactionGroupSubscriber mockedSubscriber = mockSubscriber(testTransactionGroup);

        Expect.Once.On(mockedSubscriber).
          Method("ProgressChanged").
          With(
            new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(TransactionGroup<TestTransaction>)),
              new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.5f / 3.0f))
            }
          );

        testTransactionGroup.Children[0].Transaction.ChangeProgress(0.5f);

        Expect.Once.On(mockedSubscriber).
          Method("ProgressChanged").
          With(
            new Matcher[] {
              new NMock2.Matchers.TypeMatcher(typeof(TransactionGroup<TestTransaction>)),
              new ProgressUpdateEventArgsMatcher(new ProgressReportEventArgs(0.5f))
            }
          );

        testTransactionGroup.Children[1].Transaction.ChangeProgress(0.5f);

        this.mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>
    ///   Validates that the ended event is triggered when the last transaction out of
    ///   multiple transactions in the group ends.
    /// </summary>
    [Test]
    public void TestEndedEventWithTwoTransactions() {
      using(
        TransactionGroup<TestTransaction> testTransactionGroup =
          new TransactionGroup<TestTransaction>(
            new TestTransaction[] { new TestTransaction(), new TestTransaction() }
          )
      ) {
        ITransactionGroupSubscriber mockedSubscriber = mockSubscriber(testTransactionGroup);

        Expect.Exactly(2).On(mockedSubscriber).
          Method("ProgressChanged").
          WithAnyArguments();

        Expect.Once.On(mockedSubscriber).
          Method("Ended").
          WithAnyArguments();

        testTransactionGroup.Children[0].Transaction.End();
        testTransactionGroup.Children[1].Transaction.End();

        this.mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>
    ///   Validates that the ended event is triggered when a single transaction contained
    ///   in the group ends.
    /// </summary>
    [Test]
    public void TestEndedEventWithSingleTransaction() {
      using(
        TransactionGroup<TestTransaction> testTransactionGroup =
          new TransactionGroup<TestTransaction>(
            new TestTransaction[] { new TestTransaction() }
          )
      ) {
        ITransactionGroupSubscriber mockedSubscriber = mockSubscriber(testTransactionGroup);

        Expect.Once.On(mockedSubscriber).
          Method("ProgressChanged").
          WithAnyArguments();

        Expect.Once.On(mockedSubscriber).
          Method("Ended").
          WithAnyArguments();

        testTransactionGroup.Children[0].Transaction.End();

        this.mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>
    ///   Verifies that the transaction group immediately enters the ended state when
    ///   the contained transactions have already ended before the constructor
    /// </summary>
    /// <remarks>
    ///   This was a bug at one time and should prevent a regression
    /// </remarks>
    [Test]
    public void TestAlreadyEndedTransactions() {
      using(
        TransactionGroup<Transaction> testTransactionGroup =
          new TransactionGroup<Transaction>(
            new Transaction[] { Transaction.EndedDummy, Transaction.EndedDummy }
          )
      ) {
        Assert.IsTrue(testTransactionGroup.Wait(1000));
      }
    }

    /// <summary>
    ///   Verifies that the transaction group doesn't think it's already ended when
    ///   the first transaction being added is in the ended state
    /// </summary>
    /// <remarks>
    ///   This was a bug at one time and should prevent a regression
    /// </remarks>
    [Test]
    public void TestAlreadyEndedTransactionAsFirstTransaction() {
      using(
        TransactionGroup<Transaction> testTransactionGroup =
          new TransactionGroup<Transaction>(
            new Transaction[] { Transaction.EndedDummy, new TestTransaction() }
          )
      ) {
        Assert.IsFalse(testTransactionGroup.Ended);
      }
    }

    /// <summary>
    ///   Verifies that a transaction ending while the constructor is running doesn't
    ///   wreak havoc on the transaction group
    /// </summary>
    [Test]
    public void TestTransactionEndingDuringConstructor() {
      ChainEndingTransaction chainTransaction = new ChainEndingTransaction();
      using(
        TransactionGroup<Transaction> testTransactionGroup =
          new TransactionGroup<Transaction>(
            new Transaction[] { chainTransaction.ChainedTransaction, chainTransaction }
          )
      ) {
        Assert.IsFalse(testTransactionGroup.Ended);
        chainTransaction.End();
        Assert.IsTrue(testTransactionGroup.Ended);
      }
    }

    /// <summary>Mocks a subscriber for the events of a transaction</summary>
    /// <param name="transaction">Transaction to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private ITransactionGroupSubscriber mockSubscriber(Transaction transaction) {
      ITransactionGroupSubscriber mockedSubscriber =
        this.mockery.NewMock<ITransactionGroupSubscriber>();

      transaction.AsyncEnded += new EventHandler(mockedSubscriber.Ended);
      (transaction as IProgressReporter).AsyncProgressChanged +=
        new EventHandler<ProgressReportEventArgs>(mockedSubscriber.ProgressChanged);

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
