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

namespace Nuclex.Support.Tracking {

  /// <summary>Transaction being observed by another object</summary>
  /// <typeparam name="TransactionType">
  ///   Type of the transaction that is being observed
  /// </typeparam>
  internal class ObservedWeightedTransaction<TransactionType> : IDisposable
    where TransactionType : Transaction {

    /// <summary>Delegate for reporting progress updates</summary>
    public delegate void ReportDelegate();

    /// <summary>Initializes a new observed transaction</summary>
    /// <param name="weightedTransaction">Weighted transaction being observed</param>
    /// <param name="progressUpdateCallback">
    ///   Callback to invoke when the transaction's progress changes
    /// </param>
    /// <param name="endedCallback">
    ///   Callback to invoke when the transaction has ended
    /// </param>
    internal ObservedWeightedTransaction(
      WeightedTransaction<TransactionType> weightedTransaction,
      ReportDelegate progressUpdateCallback,
      ReportDelegate endedCallback
    ) {
      this.weightedTransaction = weightedTransaction;

      // See if this transaction has already ended (initial check for performance)
      if(weightedTransaction.Transaction.Ended) {

        // Since we don't subscribe to the .Ended event (which would be fired immediately on
        // subscription if the transaction was already finished), we will emulate this
        // behavior here. There is no race condition here: The transition to .Ended occurs
        // only once and will never happen in reverse. This is just a minor optimization to
        // prevent object coupling where none is neccessary and to save some processing time.
        this.progress = 1.0f;
        progressUpdateCallback();
        
        // Do not call the ended callback here. This constructor is called when the
        // TransactionGroup constructs its list of transactions. If this is called and
        // the first transaction to be added to the group happens to be in the ended
        // state, the transactionGroup will immediately think it has ended!
        //!DONT!endedCallback();

        return;

      }

      this.endedCallback = endedCallback;
      this.progressUpdateCallback = progressUpdateCallback;

      // This might trigger the event handler to be invoked right here if the transaction
      // ended between our initial optimization attempt and this line. It's unlikely,
      // however, so we'll not waste time with another optimization attempt.
      this.weightedTransaction.Transaction.AsyncEnded += new EventHandler(asyncEnded);

      // See whether this transaction implements the IProgressReporter interface and if
      // so, connect to its progress report event in order to pass these reports on
      // to whomever created ourselfes.
      this.progressReporter = this.weightedTransaction.Transaction as IProgressReporter;
      if(this.progressReporter != null) {
        this.asyncProgressChangedEventHandler = new EventHandler<ProgressReportEventArgs>(
          asyncProgressChanged
        );
        this.progressReporter.AsyncProgressChanged += this.asyncProgressChangedEventHandler;
      }
    }

    /// <summary>Immediately releases all resources owned by the object</summary>
    public void Dispose() {
      asyncDisconnectEvents();
    }

    /// <summary>Weighted transaction being observed</summary>
    public WeightedTransaction<TransactionType> WeightedTransaction {
      get { return this.weightedTransaction; }
    }

    /// <summary>Amount of progress this transaction has achieved so far</summary>
    public float Progress {
      get { return this.progress; }
    }

    /// <summary>Called when the observed transaction has ended</summary>
    /// <param name="sender">Transaction that has ended</param>
    /// <param name="e">Not used</param>
    private void asyncEnded(object sender, EventArgs e) {
      ReportDelegate savedEndedCallback = this.endedCallback;
      ReportDelegate savedProgressUpdateCallback = this.progressUpdateCallback;

      asyncDisconnectEvents(); // We don't need those anymore!

      // If the progress hasn't reached 1.0 yet, make a fake report so that even
      // when a transaction doesn't report any progress at all, the set or queue
      // owning us will have a percentage of transactions completed.
      //
      // There is the possibility of a race condition here, as a final progress
      // report could have been generated by a thread running the transaction
      // that was preempted by this thread. This would cause the progress to
      // jump to 1.0 and then back to whatever the waiting thread will report.
      if(this.progress != 1.0f) {
        this.progress = 1.0f;
        savedProgressUpdateCallback();
      }

      savedEndedCallback();
    }

    /// <summary>Called when the progress of the observed transaction changes</summary>
    /// <param name="sender">Transaction whose progress has changed</param>
    /// <param name="arguments">Contains the updated progress</param>
    private void asyncProgressChanged(object sender, ProgressReportEventArgs arguments) {
      this.progress = arguments.Progress;
      
      ReportDelegate savedProgressUpdateCallback = this.progressUpdateCallback;
      if(savedProgressUpdateCallback != null) {
        savedProgressUpdateCallback();
      }
    }

    /// <summary>Unsubscribes from all events of the observed transaction</summary>
    private void asyncDisconnectEvents() {

      // Make use of the double check locking idiom to avoid the costly lock when
      // the events have already been unsubscribed
      if(this.endedCallback != null) {

        // This is an internal class with special knowledge that there
        // is no risk of deadlock involved, so we don't need a fancy syncRoot!
        lock(this) {
          if(this.endedCallback != null) {
            this.weightedTransaction.Transaction.AsyncEnded -= new EventHandler(asyncEnded);

            if(this.progressReporter != null) {
              this.progressReporter.AsyncProgressChanged -=
                this.asyncProgressChangedEventHandler;

              this.asyncProgressChangedEventHandler = null;
            }

            this.endedCallback = null;
            this.progressUpdateCallback = null;
          }
        }

      } // endedCallback != null

    }

    private EventHandler<ProgressReportEventArgs> asyncProgressChangedEventHandler;
    /// <summary>The observed transaction's progress reporting interface</summary>
    private IProgressReporter progressReporter;
    /// <summary>The weighted wable that is being observed</summary>
    private WeightedTransaction<TransactionType> weightedTransaction;
    /// <summary>Callback to invoke when the progress updates</summary>
    private volatile ReportDelegate progressUpdateCallback;
    /// <summary>Callback to invoke when the transaction ends</summary>
    private volatile ReportDelegate endedCallback;
    /// <summary>Progress achieved so far</summary>
    private volatile float progress;
  }

} // namespace Nuclex.Support.Tracking
