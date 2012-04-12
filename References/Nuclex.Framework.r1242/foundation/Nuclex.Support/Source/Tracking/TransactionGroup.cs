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
using System.Collections.ObjectModel;
using System.Threading;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Tracking {

  /// <summary>Forms a single transaction from a group of transactions</summary>
  /// <typeparam name="TransactionType">Type of transactions to manage as a set</typeparam>
  public class TransactionGroup<TransactionType> : Transaction, IDisposable, IProgressReporter
    where TransactionType : Transaction {

    /// <summary>will be triggered to report when progress has been achieved</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new transaction group</summary>
    /// <param name="children">Transactions to track with this group</param>
    /// <remarks>
    ///   Uses a default weighting factor of 1.0 for all transactions.
    /// </remarks>
    public TransactionGroup(IEnumerable<TransactionType> children) {
      List<ObservedWeightedTransaction<TransactionType>> childrenList =
        new List<ObservedWeightedTransaction<TransactionType>>();

      // Construct a WeightedTransaction with the default weight for each
      // transaction and wrap it in an ObservedTransaction
      foreach(TransactionType transaction in children) {
        childrenList.Add(
          new ObservedWeightedTransaction<TransactionType>(
            new WeightedTransaction<TransactionType>(transaction),
            new ObservedWeightedTransaction<TransactionType>.ReportDelegate(
              asyncProgressUpdated
            ),
            new ObservedWeightedTransaction<TransactionType>.ReportDelegate(
              asyncChildEnded
            )
          )
        );
      }

      // Since all transactions have a weight of 1.0, the total weight is
      // equal to the number of transactions in our list
      this.totalWeight = (float)childrenList.Count;
      // Thread.MemoryBarrier(); // not needed because children is volatile
      this.children = childrenList;

      // Any asyncEnded events being receiving from the transactions until now
      // would have been ignored, so we need to check again here
      asyncChildEnded();
    }

    /// <summary>Initializes a new transaction group</summary>
    /// <param name="children">Transactions to track with this group</param>
    public TransactionGroup(
      IEnumerable<WeightedTransaction<TransactionType>> children
    ) {
      List<ObservedWeightedTransaction<TransactionType>> childrenList =
        new List<ObservedWeightedTransaction<TransactionType>>();

      // Construct an ObservedTransaction around each of the WeightedTransactions
      foreach(WeightedTransaction<TransactionType> transaction in children) {
        childrenList.Add(
          new ObservedWeightedTransaction<TransactionType>(
            transaction,
            new ObservedWeightedTransaction<TransactionType>.ReportDelegate(
              asyncProgressUpdated
            ),
            new ObservedWeightedTransaction<TransactionType>.ReportDelegate(
              asyncChildEnded
            )
          )
        );

        // Sum up the total weight
        this.totalWeight += transaction.Weight;
      }

      this.children = childrenList;

      // Any asyncEnded events being receiving from the transactions until now
      // would have been ignored, so we need to check again here
      asyncChildEnded();
    }

    /// <summary>Immediately releases all resources owned by the object</summary>
    public void Dispose() {

      if(this.children != null) {

        // Dispose all the observed transactions, disconnecting the events from the
        // actual transactions so the GC can more easily collect this class
        for(int index = 0; index < this.children.Count; ++index)
          this.children[index].Dispose();

        this.children = null;
        this.wrapper = null;

      }

    }

    /// <summary>Childs contained in the transaction set</summary>
    public IList<WeightedTransaction<TransactionType>> Children {
      get {

        // The wrapper is constructed only when needed. Most of the time, users will
        // just create a transaction group and monitor its progress without ever using
        // the Childs collection.
        if(this.wrapper == null) {

          // This doesn't need a lock because it's a stateless wrapper.
          // If it is constructed twice, then so be it, no problem at all.
          this.wrapper = new WeightedTransactionWrapperCollection<TransactionType>(
            this.children
          );

        }

        return this.wrapper;

      }
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

    /// <summary>
    ///   Called when the progress of one of the observed transactions changes
    /// </summary>
    private void asyncProgressUpdated() {
      if(this.children == null) {
        return;
      }

      float totalProgress = 0.0f;

      // Calculate the sum of the progress reported by our child transactions,
      // scaled to the weight each transaction has assigned to it.
      for(int index = 0; index < this.children.Count; ++index) {
        totalProgress +=
          this.children[index].Progress * this.children[index].WeightedTransaction.Weight;
      }

      // Calculate the actual combined progress
      if(this.totalWeight > 0.0f)
        totalProgress /= this.totalWeight;

      // Send out the progress update
      OnAsyncProgressChanged(totalProgress);
    }

    /// <summary>
    ///   Called when an observed transaction ends
    /// </summary>
    private void asyncChildEnded() {

      // If a transaction reports its end durign the constructor, it will end up here
      // where the collection has not been assigned yet, allowing us to skip the
      // check until all transactions are there (otherwise, we might invoke
      // OnAsyncended() early, because all transactions in the list seem to have ended
      // despite the fact that the constructor hasn't finished adding transactions yet)
      if(this.children == null) {
        return;
      }

      // If there's still at least one transaction going, don't report that
      // the transaction group has finished yet.
      for(int index = 0; index < this.children.Count; ++index)
        if(!this.children[index].WeightedTransaction.Transaction.Ended)
          return;

      // All child transactions have ended, so the set has now ended as well
      if(Interlocked.Exchange(ref this.endedCalled, 1) == 0) {
        OnAsyncEnded();
      }

    }

    /// <summary>Transactions being managed in the set</summary>
    private volatile List<ObservedWeightedTransaction<TransactionType>> children;
    /// <summary>
    ///   Wrapper collection for exposing the child transactions under the
    ///   WeightedTransaction interface
    /// </summary>
    private volatile WeightedTransactionWrapperCollection<TransactionType> wrapper;
    /// <summary>Summed weight of all transactions in the set</summary>
    private float totalWeight;
    /// <summary>Whether we already called OnAsyncEnded</summary>
    private int endedCalled;

  }

} // namespace Nuclex.Support.Tracking
