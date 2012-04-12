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
using System.Threading;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the transaction class</summary>
  [TestFixture]
  public class TransactionTest {

    #region interface ITransactionSubscriber

    /// <summary>Interface used to test the transaction</summary>
    public interface ITransactionSubscriber {

      /// <summary>Called when the set transaction has ended</summary>
      /// <param name="sender">Transaction group that as ended</param>
      /// <param name="arguments">Not used</param>
      void Ended(object sender, EventArgs arguments);

    }

    #endregion // interface ITransactionGroupSubscriber

    #region class TestTransaction

    /// <summary>Transaction used for testing in this unit test</summary>
    private class TestTransaction : Transaction {

      /// <summary>Transitions the transaction into the ended state</summary>
      public void End() {
        OnAsyncEnded();
      }

    }

    #endregion // class TestWiatable

    #region class UnsubscribingTransaction

    /// <summary>Transaction that unsubscribes during an event callback</summary>
    private class UnsubscribingTransaction : Transaction {

      /// <summary>Initializes a new unsubscribing transaction</summary>
      /// <param name="transactionToMonitor">
      ///   Transaction whose AsyncEnded event will be monitored to trigger
      ///   the this transaction unsubscribing from the event.
      /// </param>
      public UnsubscribingTransaction(Transaction transactionToMonitor) {
        this.transactionToMonitor = transactionToMonitor;
        this.monitoredTransactionEndedDelegate = new EventHandler(
          monitoredTransactionEnded
        );

        this.transactionToMonitor.AsyncEnded += this.monitoredTransactionEndedDelegate;
      }

      /// <summary>Called when the monitored transaction has ended</summary>
      /// <param name="sender">Monitored transaction that has ended</param>
      /// <param name="arguments">Not used</param>
      private void monitoredTransactionEnded(object sender, EventArgs arguments) {
        this.transactionToMonitor.AsyncEnded -= this.monitoredTransactionEndedDelegate;
      }

      /// <summary>Transitions the transaction into the ended state</summary>
      public void End() {
        OnAsyncEnded();
      }

      /// <summary>Transaction whose ending in being monitored</summary>
      private Transaction transactionToMonitor;
      /// <summary>Delegate to the monitoredTransactionEnded() method</summary>
      private EventHandler monitoredTransactionEndedDelegate;

    }

    #endregion // class TestWiatable

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>
    ///   Verifies that the transaction throws an exception when it is ended multiple times
    /// </summary>
    [Test]
    public void TestThrowOnRepeatedlyEndedTransaction() {
      TestTransaction test = new TestTransaction();
      test.End();
      Assert.Throws<InvalidOperationException>(
        delegate() { test.End(); }
      );
    }

    /// <summary>
    ///   Tests whether the Ended event of the transaction is correctly delivered if
    ///   the transaction ends after the subscription already took place
    /// </summary>
    [Test]
    public void TestEndedEventAfterSubscription() {
      TestTransaction test = new TestTransaction();

      ITransactionSubscriber mockedSubscriber = mockSubscriber(test);
      Expect.Once.On(mockedSubscriber).
        Method("Ended").
        WithAnyArguments();

      test.End();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Tests whether the Ended event of the transaction is correctly delivered if
    ///   the transaction is already done when the subscription takes place
    /// </summary>
    [Test]
    public void TestEndedEventDuingSubscription() {
      TestTransaction test = new TestTransaction();
      test.End();

      ITransactionSubscriber mockedSubscriber =
        this.mockery.NewMock<ITransactionSubscriber>();

      Expect.Once.On(mockedSubscriber).
        Method("Ended").
        WithAnyArguments();

      test.AsyncEnded += new EventHandler(mockedSubscriber.Ended);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Verifies that the Wait() method of the transaction works as expected
    /// </summary>
    [Test]
    public void TestWaitUnlimited() {
      TestTransaction test = new TestTransaction();

      // We can only do a positive test here without slowing down the unit test
      ThreadPool.QueueUserWorkItem(
        (WaitCallback)delegate(object state) { Thread.Sleep(1); test.End(); }
      );

      test.Wait();
    }

    /// <summary>
    ///   Verifies that the Wait() method of the transaction works as expected using
    ///   a millisecond count as its argument
    /// </summary>
    [Test]
    public void TestWaitMilliseconds() {
      TestTransaction test = new TestTransaction();

      // Wait 0 milliseconds for the transaction to end. Of course, this will not happen,
      // so a timeout occurs and false is returned
      Assert.IsFalse(test.Wait(0));

      test.End();

      // Wait another 0 milliseconds for the transaction to end. Now it has already ended
      // and no timeout will occur, even with a wait time of 0 milliseconds.
      Assert.IsTrue(test.Wait(0));
    }

    /// <summary>
    ///   Verifies that the Wait() method of the transaction works as expected using
    ///   a TimeSpan as its argument
    /// </summary>
    [Test]
    public void TestWaitTimeSpan() {
      TestTransaction test = new TestTransaction();

      // Wait 0 milliseconds for the transaction to end. Of course, this will not happen,
      // so a timeout occurs and false is returned
      Assert.IsFalse(test.Wait(TimeSpan.Zero));

      test.End();

      // Wait another 0 milliseconds for the transaction to end. Now it has already ended
      // and no timeout will occur, even with a wait time of 0 milliseconds.
      Assert.IsTrue(test.Wait(TimeSpan.Zero));
    }

    /// <summary>
    ///   Verifies that no error occurs when an even subscriber to the AsyncEnded event
    ///   unsubscribes in the event callback handler
    /// </summary>
    [Test]
    public void TestUnsubscribeInEndedCallback() {
      TestTransaction monitored = new TestTransaction();
      UnsubscribingTransaction test = new UnsubscribingTransaction(monitored);

      ITransactionSubscriber mockedSubscriber = mockSubscriber(monitored);

      try {
        Expect.Once.On(mockedSubscriber).Method("Ended").WithAnyArguments();
        monitored.End();
        this.mockery.VerifyAllExpectationsHaveBeenMet();
      }
      finally {
        test.End();
      }
    }

    /// <summary>Mocks a subscriber for the events of a transaction</summary>
    /// <param name="transaction">Transaction to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private ITransactionSubscriber mockSubscriber(Transaction transaction) {
      ITransactionSubscriber mockedSubscriber =
        this.mockery.NewMock<ITransactionSubscriber>();

      transaction.AsyncEnded += new EventHandler(mockedSubscriber.Ended);

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
