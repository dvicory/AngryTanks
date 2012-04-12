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
using System.IO;
using System.Threading;

#if UNITTEST

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the observation wrapper of weighted transactions</summary>
  [TestFixture]
  public class ObservedWeightedTransactionTest {

    #region interface IObservationSubscriber

    /// <summary>
    ///   Interface used to test the observation wrapper of weighted transactions
    /// </summary>
    public interface IObservationSubscriber {

      /// <summary>Will be invoked when an observed transaction's progress changes</summary>
      void ProgressUpdated();

      /// <summary>Will be invoked when an observed transaction completes</summary>
      void Ended();

    }

    #endregion // interface IObservationSubscriber

    #region class FunkyTransaction

    /// <summary>
    ///   Transaction that goes into the 'ended' state as soon as someone registers for
    ///   state change notifications
    /// </summary>
    private class FunkyTransaction : Transaction {

      /// <summary>Manages registrations to the AsyncEnded event</summary>
      public override event EventHandler AsyncEnded {
        add {
          base.AsyncEnded += value;

          // To deterministically provoke an 'Ended' event just after registration we
          // will switch the transaction into the 'ended' state right here
          int oldValue = Interlocked.Exchange(ref this.alreadyEnded, 1);
          if(oldValue != 1) {
            OnAsyncEnded();
          }
        }
        remove {
          base.AsyncEnded -= value;
        }
      }

      /// <summary>Whether the transaction has already been ended</summary>
      private int alreadyEnded;

    }

    #endregion // class FunkyTransaction

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Verifies that the constructor of the observation wrapper works</summary>
    [Test]
    public void TestConstructorWithAlreadyEndedTransaction() {
      WeightedTransaction<Transaction> testTransaction = new WeightedTransaction<Transaction>(
        Transaction.EndedDummy
      );

      IObservationSubscriber subscriber = this.mockery.NewMock<IObservationSubscriber>();

      Expect.AtLeast(0).On(subscriber).Method("ProgressUpdated");
      // This should no be called because otherwise, the 'Ended' event would be raised
      // to the transaction group before all transactions have been added into
      // the internal list, leading to an early ending or even multiple endings.
      Expect.Never.On(subscriber).Method("Ended");

      using(
        ObservedWeightedTransaction<Transaction> test =
          new ObservedWeightedTransaction<Transaction>(
            testTransaction,
            new ObservedWeightedTransaction<Transaction>.ReportDelegate(
              subscriber.ProgressUpdated
            ),
            new ObservedWeightedTransaction<Transaction>.ReportDelegate(
              subscriber.Ended
            )
          )
      ) {
        this.mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>
    ///   Verifies that the constructor of the observation wrapper can handle a transaction
    ///   entering the 'ended' state right on subscription
    /// </summary>
    [Test]
    public void TestConstructorWithEndingTransaction() {
      WeightedTransaction<Transaction> testTransaction = new WeightedTransaction<Transaction>(
        new FunkyTransaction()
      );

      IObservationSubscriber subscriber = this.mockery.NewMock<IObservationSubscriber>();

      Expect.AtLeast(0).On(subscriber).Method("ProgressUpdated");
      Expect.Once.On(subscriber).Method("Ended");

      using(
        ObservedWeightedTransaction<Transaction> test =
          new ObservedWeightedTransaction<Transaction>(
            testTransaction,
            new ObservedWeightedTransaction<Transaction>.ReportDelegate(
              subscriber.ProgressUpdated
            ),
            new ObservedWeightedTransaction<Transaction>.ReportDelegate(
              subscriber.Ended
            )
        )
      ) {
        this.mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
