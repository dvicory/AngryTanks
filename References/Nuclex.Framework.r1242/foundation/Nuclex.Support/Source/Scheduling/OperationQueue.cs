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

using Nuclex.Support.Tracking;

namespace Nuclex.Support.Scheduling {

  /// <summary>Operation that sequentially executes a series of operations</summary>
  /// <typeparam name="OperationType">
  ///   Type of the child operations the QueueOperation will contain
  /// </typeparam>
  public class OperationQueue<OperationType> : Operation, IProgressReporter
    where OperationType : Operation {

    /// <summary>will be triggered to report when progress has been achieved</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new queue operation with default weights</summary>
    /// <param name="childs">Child operations to execute in this operation</param>
    /// <remarks>
    ///   All child operations will have a default weight of 1.0
    /// </remarks>
    public OperationQueue(IEnumerable<OperationType> childs) : this() {

      // Construct a WeightedTransaction with the default weight for each
      // transaction and wrap it in an ObservedTransaction
      foreach(OperationType operation in childs)
        this.children.Add(new WeightedTransaction<OperationType>(operation));

      // Since all transactions have a weight of 1.0, the total weight is
      // equal to the number of transactions in our list
      this.totalWeight = (float)this.children.Count;

    }

    /// <summary>Initializes a new queue operation with custom weights</summary>
    /// <param name="childs">Child operations to execute in this operation</param>
    public OperationQueue(IEnumerable<WeightedTransaction<OperationType>> childs) : this() {

      // Construct an ObservedTransactionn around each of the WeightedTransactions
      foreach(WeightedTransaction<OperationType> operation in childs) {
        this.children.Add(operation);

        // Sum up the total weight
        this.totalWeight += operation.Weight;
      }

    }

    /// <summary>Initializes a new queue operation</summary>
    private OperationQueue() {
      this.asyncOperationEndedDelegate = new EventHandler(asyncOperationEnded);
      this.asyncOperationProgressChangedDelegate = new EventHandler<ProgressReportEventArgs>(
        asyncOperationProgressChanged
      );

      this.children = new List<WeightedTransaction<OperationType>>();
    }

    /// <summary>Provides access to the child operations of this queue</summary>
    public IList<WeightedTransaction<OperationType>> Children {
      get { return this.children; }
    }

    /// <summary>Launches the background operation</summary>
    public override void Start() {
      startCurrentOperation();
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

    /// <summary>Prepares the current operation and calls its Start() method</summary>
    /// <remarks>
    ///   This subscribes the queue to the events of to the current operation
    ///   and launches the operation by calling its Start() method.
    /// </remarks>
    private void startCurrentOperation() {
      do {
        Thread.MemoryBarrier();
        OperationType operation = this.children[this.currentOperationIndex].Transaction;

        operation.AsyncEnded += this.asyncOperationEndedDelegate;

        IProgressReporter progressReporter = operation as IProgressReporter;
        if(progressReporter != null)
          progressReporter.AsyncProgressChanged += this.asyncOperationProgressChangedDelegate;

        Interlocked.Exchange(ref this.completionStatus, 1);
        operation.Start();
      } while(Interlocked.Decrement(ref this.completionStatus) > 0);
    }

    /// <summary>Disconnects from the current operation and calls its End() method</summary>
    /// <remarks>
    ///   This unsubscribes the queue from the current operation's events, calls End()
    ///   on the operation and, if the operation didn't have an exception to report,
    ///   counts up the accumulated progress of th  e queue.
    /// </remarks>
    private void endCurrentOperation() {
      Thread.MemoryBarrier();
      OperationType operation = this.children[this.currentOperationIndex].Transaction;

      // Disconnect from the operation's events
      operation.AsyncEnded -= this.asyncOperationEndedDelegate;

      IProgressReporter progressReporter = operation as IProgressReporter;
      if(progressReporter != null)
        progressReporter.AsyncProgressChanged -= this.asyncOperationProgressChangedDelegate;

      try {
        operation.Join();

        // Add the operations weight to the total amount of completed weight in the queue
        this.completedWeight += this.children[this.currentOperationIndex].Weight;

        // Trigger another progress update
        OnAsyncProgressChanged(this.completedWeight / this.totalWeight);
      }
      catch(Exception exception) {
        this.exception = exception;
      }
    }

    /// <summary>Called when the current executing operation ends</summary>
    /// <param name="sender">Operation that ended</param>
    /// <param name="arguments">Not used</param>
    private void asyncOperationEnded(object sender, EventArgs arguments) {

      // Unsubscribe from the current operation's events and update the
      // accumulating progress counter
      endCurrentOperation();

      // Only jump to the next operation if no exception occured
      if(this.exception == null) {
        int newIndex = Interlocked.Increment(ref this.currentOperationIndex);
        Thread.MemoryBarrier();

        // Execute the next operation unless we reached the end of the queue
        if(newIndex < this.children.Count) {
          if(Interlocked.Increment(ref this.completionStatus) == 1) {
            startCurrentOperation();
          }
          return;
        }
      }

      // Either an exception has occured or we reached the end of the operation
      // queue. In any case, we need to report that the operation is over.
      OnAsyncEnded();

    }

    /// <summary>Called when currently executing operation makes progress</summary>
    /// <param name="sender">Operation that has achieved progress</param>
    /// <param name="arguments">Not used</param>
    private void asyncOperationProgressChanged(
      object sender, ProgressReportEventArgs arguments
    ) {

      // Determine the completed weight of the currently executing operation
      float operationWeight = this.children[this.currentOperationIndex].Weight;
      float operationCompletedWeight = arguments.Progress * operationWeight;

      // Build the total normalized amount of progress for the queue
      float progress = (this.completedWeight + operationCompletedWeight) / this.totalWeight;

      // Done, we can send the actual progress to any event subscribers
      OnAsyncProgressChanged(progress);

    }

    /// <summary>Delegate to the asyncOperationEnded() method</summary>
    private EventHandler asyncOperationEndedDelegate;
    /// <summary>Delegate to the asyncOperationProgressUpdated() method</summary>
    private EventHandler<ProgressReportEventArgs> asyncOperationProgressChangedDelegate;
    /// <summary>Operations being managed in the queue</summary>
    private List<WeightedTransaction<OperationType>> children;
    /// <summary>Summed weight of all operations in the queue</summary>
    private float totalWeight;
    /// <summary>Accumulated weight of the operations already completed</summary>
    private float completedWeight;
    /// <summary>Index of the operation currently executing</summary>
    private int currentOperationIndex;
    /// <summary>Used to detect when an operation completes synchronously</summary>
    private int completionStatus;
    /// <summary>Exception that has occured in the background process</summary>
    private volatile Exception exception;

  }

} // namespace Nuclex.Support.Scheduling
