#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using Nuclex.Support.Tracking;

namespace Nuclex.Windows.Forms {

  /// <summary>Progress bar for tracking the progress of background operations</summary>
  public partial class TrackingBar : AsyncProgressBar {

    /// <summary>Initializes a new tracking bar</summary>
    public TrackingBar() {
      InitializeComponent();

      // We start off being in the idle state (and thus, being invisible)
      this.isIdle = true;
      this.Visible = false;

      // Initialize the delegates we use to update the control's state and those
      // we use to register ourselfes to the tracker's events
      this.updateIdleStateDelegate = new MethodInvoker(updateIdleState);
      this.asyncIdleStateChangedDelegate = new EventHandler<IdleStateEventArgs>(
        asyncIdleStateChanged
      );
      this.asyncProgressUpdateDelegate = new EventHandler<ProgressReportEventArgs>(
        asyncProgressUpdated
      );

      // Create the tracker and attach ourselfes to its events
      this.tracker = new ProgressTracker();
      this.tracker.AsyncIdleStateChanged += this.asyncIdleStateChangedDelegate;
      this.tracker.AsyncProgressChanged += this.asyncProgressUpdateDelegate;
    }

    /// <summary>Tracks the specified transaction in the tracking bar</summary>
    /// <param name="transaction">Transaction to be tracked</param>
    public void Track(Transaction transaction) {
      this.tracker.Track(transaction);
    }

    /// <summary>Tracks the specified transaction in the tracking bar</summary>
    /// <param name="transaction">Transaction to be tracked</param>
    /// <param name="weight">Weight of this transaction in the total progress</param>
    public void Track(Transaction transaction, float weight) {
      this.tracker.Track(transaction, weight);
    }

    /// <summary>Stops tracking the specified transaction</summary>
    /// <param name="transaction">Transaction to stop tracking</param>
    public void Untrack(Transaction transaction) {
      this.tracker.Untrack(transaction);
    }

    /// <summary>
    ///   Called when the summed progressed of the tracked transaction has changed
    /// </summary>
    /// <param name="sender">Transaction whose progress has changed</param>
    /// <param name="arguments">Contains the progress achieved by the transaction</param>
    private void asyncProgressUpdated(
      object sender, ProgressReportEventArgs arguments
    ) {
      AsyncSetValue(arguments.Progress);
    }

    /// <summary>Called when the tracker becomes enters of leaves the idle state</summary>
    /// <param name="sender">Tracker that has entered or left the idle state</param>
    /// <param name="arguments">Contains the new idle state</param>
    private void asyncIdleStateChanged(object sender, IdleStateEventArgs arguments) {

      // Do a fully synchronous update of the idle state. This update must not be
      // lost because otherwise, the progress bar might stay on-screen when in fact,
      // the background operation has already finished and nothing is happening anymore.
      this.isIdle = arguments.Idle;

      // Update the bar's idle state
      if(InvokeRequired) {
        Invoke(this.updateIdleStateDelegate);
      } else {
        updateIdleState();
      }

    }

    /// <summary>
    ///   Updates the idle state of the progress bar
    ///   (controls whether the progress bar is shown or invisible)
    /// </summary>
    private void updateIdleState() {

      // Only show the progress bar when something is happening
      base.Visible = !this.isIdle;

    }

    /// <summary>Whether the progress bar is in the idle state</summary>
    private volatile bool isIdle;
    /// <summary>Tracker used to sum and update the total progress</summary>
    private ProgressTracker tracker;
    /// <summary>Delegate for the idle state update method</summary>
    private MethodInvoker updateIdleStateDelegate;
    /// <summary>Delegate for the asyncIdleStateChanged() method</summary>
    private EventHandler<IdleStateEventArgs> asyncIdleStateChangedDelegate;
    /// <summary>Delegate for the asyncProgressUpdate() method</summary>
    private EventHandler<ProgressReportEventArgs> asyncProgressUpdateDelegate;

  }

} // namespace Nuclex.Windows.Forms
