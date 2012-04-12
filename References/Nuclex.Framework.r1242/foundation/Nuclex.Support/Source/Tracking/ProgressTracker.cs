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
using System.Threading;

namespace Nuclex.Support.Tracking {

  /// <summary>
  ///   Helps tracking the progress of one or more background transactions
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This is useful if you want to display a progress bar for multiple
  ///     transactions but can not guarantee that no additional transactions
  ///     will appear inmidst of execution.
  ///   </para>
  ///   <para>
  ///     This class does not implement the <see cref="Transaction" /> interface itself
  ///     in order to not violate the design principles of transactions which
  ///     guarantee that a <see cref="Transaction" /> will only finish once (whereas the
  ///     progress tracker might 'finish' any number of times).
  ///   </para>
  /// </remarks>
  public class ProgressTracker : IDisposable, IProgressReporter {

    #region class TransactionMatcher

    /// <summary>Matches a direct transaction to a fully wrapped one</summary>
    private class TransactionMatcher {

      /// <summary>
      ///   Initializes a new transaction matcher that matches against
      ///   the specified transaction
      /// </summary>
      /// <param name="toMatch">Transaction to match against</param>
      public TransactionMatcher(Transaction toMatch) {
        this.toMatch = toMatch;
      }

      /// <summary>
      ///   Checks whether the provided transaction matches the comparison
      ///   transaction of the instance
      /// </summary>
      /// <param name="other">Transaction to match to the comparison transaction</param>
      public bool Matches(ObservedWeightedTransaction<Transaction> other) {
        return ReferenceEquals(other.WeightedTransaction.Transaction, this.toMatch);
      }

      /// <summary>Transaction this instance compares against</summary>
      private Transaction toMatch;

    }

    #endregion // class TransactionMatcher

    /// <summary>Triggered when the idle state of the tracker changes</summary>
    /// <remarks>
    ///   The tracker is idle when no transactions are being tracked in it. If you're
    ///   using this class to feed a progress bar, this would be the event to use for
    ///   showing or hiding the progress bar. The tracker starts off as idle because,
    ///   upon construction, its list of transactions will be empty.
    /// </remarks>
    public event EventHandler<IdleStateEventArgs> AsyncIdleStateChanged;

    /// <summary>Triggered when the total progress has changed</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new transaction tracker</summary>
    public ProgressTracker() {

      this.trackedTransactions = new List<ObservedWeightedTransaction<Transaction>>();
      this.idle = true;

      this.asyncEndedDelegate =
        new ObservedWeightedTransaction<Transaction>.ReportDelegate(asyncEnded);
      this.asyncProgressUpdatedDelegate =
        new ObservedWeightedTransaction<Transaction>.ReportDelegate(asyncProgressChanged);

    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      lock(this.trackedTransactions) {

        // Get rid of all transactions we're tracking. This unsubscribes the
        // observers from the events of the transactions and stops us from
        // being kept alive and receiving any further events if some of the
        // tracked transactions are still executing.
        for(int index = 0; index < this.trackedTransactions.Count; ++index)
          this.trackedTransactions[index].Dispose();

        // Help the GC a bit by untangling the references :)
        this.trackedTransactions.Clear();
        this.trackedTransactions = null;

      } // lock
    }

    /// <summary>Begins tracking the specified background transactions</summary>
    /// <param name="transaction">Background transaction to be tracked</param>
    public void Track(Transaction transaction) {
      Track(transaction, 1.0f);
    }

    /// <summary>Begins tracking the specified background transaction</summary>
    /// <param name="transaction">Background transaction to be tracked</param>
    /// <param name="weight">Weight to assign to this background transaction</param>
    public void Track(Transaction transaction, float weight) {

      // Add the new transaction into the tracking list. This has to be done
      // inside a lock to prevent issues with the progressUpdate callback, which could
      // access the totalWeight field before it has been updated to reflect the
      // new transaction added to the collection.
      lock(this.trackedTransactions) {

        bool wasEmpty = (this.trackedTransactions.Count == 0);

        if(transaction.Ended) {

          // If the ended transaction would become the only transaction in the list,
          // there's no sense in doing anything at all because it would have to be
          // thrown right out again. Only add the transaction when there are other
          // running transactions to properly sum total progress for consistency.
          if(!wasEmpty) {

            // Construct a new observation wrapper. This is done inside the lock
            // because as soon as we are subscribed to the events, we can potentially
            // receive them. The lock eliminates the risk of processing a progress update
            // before the transaction has been added to the tracked transactions list.
            this.trackedTransactions.Add(
              new ObservedWeightedTransaction<Transaction>(
                new WeightedTransaction<Transaction>(transaction, weight),
                this.asyncProgressUpdatedDelegate,
                this.asyncEndedDelegate
              )
            );

          }

        } else { // Not ended -- Transaction is still running

          // Construct a new transation observer and add the transaction to our
          // list of tracked transactions.
          ObservedWeightedTransaction<Transaction> observedTransaction =
            new ObservedWeightedTransaction<Transaction>(
              new WeightedTransaction<Transaction>(transaction, weight),
              this.asyncProgressUpdatedDelegate,
              this.asyncEndedDelegate
            );

          this.trackedTransactions.Add(observedTransaction);

          // If this is the first transaction to be added to the list, tell our
          // owner that we're idle no longer!
          if(wasEmpty) {
            setIdle(false);
          }

        } // if transaction ended

        // This can be done after we registered the wrapper to our delegates because
        // any incoming progress updates will be stopped from the danger of a
        // division-by-zero from the potentially still zeroed totalWeight by the lock.
        this.totalWeight += weight;

        // All done, the total progress is different now, so force a recalculation and
        // send out the AsyncProgressUpdated event.
        recalculateProgress();

      } // lock

    }

    /// <summary>Stops tracking the specified background transaction</summary>
    /// <param name="transaction">Background transaction to stop tracking of</param>
    public void Untrack(Transaction transaction) {
      lock(this.trackedTransactions) {

        // Locate the object to be untracked in our collection
        int index;
        for(index = 0; index < this.trackedTransactions.Count; ++index) {
          bool same = ReferenceEquals(
            transaction,
            this.trackedTransactions[index].WeightedTransaction.Transaction
          );
          if(same) {
            break;
          }
        }
        if(index == this.trackedTransactions.Count) {
          throw new ArgumentException("Specified transaction is not being tracked");
        }

        // Remove and dispose the transaction the user wants to untrack
        {
          ObservedWeightedTransaction<Transaction> wrappedTransaction =
            this.trackedTransactions[index];

          this.trackedTransactions.RemoveAt(index);
          wrappedTransaction.Dispose();
        }

        // If the list is empty, then we're back in the idle state
        if(this.trackedTransactions.Count == 0) {

          this.totalWeight = 0.0f;

          // If we entered the idle state with this call, report the state change!
          setIdle(true);

        } else {

          // Rebuild the total weight from scratch. Subtracting the removed transaction's
          // weight would work, too, but we might accumulate rounding errors making the sum
          // drift slowly away from the actual value.
          float newTotalWeight = 0.0f;
          for(index = 0; index < this.trackedTransactions.Count; ++index)
            newTotalWeight += this.trackedTransactions[index].WeightedTransaction.Weight;

          this.totalWeight = newTotalWeight;

          recalculateProgress();

        }

      } // lock
    }

    /// <summary>Whether the tracker is currently idle</summary>
    public bool Idle {
      get { return this.idle; }
    }

    /// <summary>Current summed progress of the tracked transactions</summary>
    public float Progress {
      get { return this.progress; }
    }

    /// <summary>Fires the AsyncIdleStateChanged event</summary>
    /// <param name="idle">New idle state to report</param>
    protected virtual void OnAsyncIdleStateChanged(bool idle) {
      EventHandler<IdleStateEventArgs> copy = AsyncIdleStateChanged;
      if(copy != null)
        copy(this, new IdleStateEventArgs(idle));
    }

    /// <summary>Fires the AsyncProgressUpdated event</summary>
    /// <param name="progress">New progress to report</param>
    protected virtual void OnAsyncProgressUpdated(float progress) {
      EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
      if(copy != null)
        copy(this, new ProgressReportEventArgs(progress));
    }

    /// <summary>Recalculates the total progress of the tracker</summary>
    private void recalculateProgress() {
      bool progressChanged = false;

      // Lock the collection to avoid trouble when someone tries to remove one
      // of our tracked transactions while we're just doing a progress update
      lock(this.trackedTransactions) {

        // This is a safety measure. In theory, even after all transactions have
        // ended and the collection of tracked transactions is cleared, a waiting
        // thread might deliver another progress update causing this method to
        // be entered. In this case, the right thing is to do nothing at all.
        if(this.totalWeight != 0.0f) {
          float totalProgress = 0.0f;

          // Sum up the total progress
          for(int index = 0; index < this.trackedTransactions.Count; ++index) {
            float weight = this.trackedTransactions[index].WeightedTransaction.Weight;
            totalProgress += this.trackedTransactions[index].Progress * weight;
          }

          // This also needs to be in the lock to guarantee that the total weight
          // corresponds to the number of transactions we just summed -- by design,
          // the total weight always has to be updated at the same time as the collection.
          totalProgress /= this.totalWeight;

          if(totalProgress != this.progress) {
            this.progress = totalProgress;
            progressChanged = true;
          }
        }

      } // lock

      // Finally, trigger the event if the progress has changed
      if(progressChanged) {
        OnAsyncProgressUpdated(this.progress);
      }
    }

    /// <summary>Called when one of the tracked transactions has ended</summary>
    private void asyncEnded() {
      lock(this.trackedTransactions) {

        // If any transactions in the list are still going, keep the entire list.
        // This behavior is intentional in order to prevent the tracker's progress from
        // jumping back repeatedly when multiple tracked transactions come to an end.
        for(int index = 0; index < this.trackedTransactions.Count; ++index)
          if(!this.trackedTransactions[index].WeightedTransaction.Transaction.Ended)
            return;

        // All transactions have finished, get rid of the wrappers and make a
        // fresh start for future transactions to be tracked. No need to call
        // Dispose() since, as a matter of fact, when the transaction
        this.trackedTransactions.Clear();
        this.totalWeight = 0.0f;

        // Notify our owner that we're idle now. This line is only reached when all
        // transactions were finished, so it's safe to trigger this here.
        setIdle(true);

      } // lock
    }

    /// <summary>Called when one of the tracked transactions has achieved progress</summary>
    private void asyncProgressChanged() {
      recalculateProgress();
    }

    /// <summary>Changes the idle state</summary>
    /// <param name="idle">Whether or not the tracker is currently idle</param>
    /// <remarks>
    ///   This method expects to be called during a lock() on trackedTransactions!
    /// </remarks>
    private void setIdle(bool idle) {
      this.idle = idle;

      OnAsyncIdleStateChanged(idle);
    }

    /// <summary>Whether the tracker is currently idle</summary>
    private volatile bool idle;
    /// <summary>Current summed progress of the tracked transactions</summary>
    private volatile float progress;
    /// <summary>Total weight of all transactions being tracked</summary>
    private volatile float totalWeight;
    /// <summary>Transactions being tracked by this tracker</summary>
    private List<ObservedWeightedTransaction<Transaction>> trackedTransactions;
    /// <summary>Delegate for the asyncEnded() method</summary>
    private ObservedWeightedTransaction<Transaction>.ReportDelegate asyncEndedDelegate;
    /// <summary>Delegate for the asyncProgressUpdated() method</summary>
    private ObservedWeightedTransaction<Transaction>.ReportDelegate asyncProgressUpdatedDelegate;

  }

} // namespace Nuclex.Support.Tracking
