using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Nuclex.Windows.Forms {

  /// <summary>Progress bar with optimized multi-threading behavior</summary>
  /// <remarks>
  ///   <para>
  ///     If a background thread is generating lots of progress updates, using synchronized
  ///     calls can drastically reduce performance. This progress bar optimizes that case
  ///     by performing the update asynchronously and keeping only the most recent update
  ///     when multiple updates arrive while the asynchronous update call is still running.
  ///   </para>
  ///   <para>
  ///     This design eliminates useless queueing of progress updates, thereby reducing
  ///     CPU load occuring in the UI thread and at the same time avoids blocking the
  ///     worker thread, increasing its performance.
  ///   </para>
  /// </remarks>
  public partial class AsyncProgressBar : ProgressBar {

    /// <summary>Initializes a new asynchronous progress bar</summary>
    public AsyncProgressBar() {
      InitializeComponent();

      this.Disposed += new EventHandler(progressBarDisposed);
      this.updateProgressDelegate = new MethodInvoker(updateProgress);

      // Could probably use VolatileWrite() as well, but for consistency reasons
      // this is an Interlocked call, too. Mixing different synchronization measures
      // for a variable raises a red flag whenever I see it :)
      Interlocked.Exchange(ref this.newProgress, -1.0f);
    }
    
    /// <summary>Called when the progress bar is being disposed</summary>
    /// <param name="sender">Progress bar that is being disposed</param>
    /// <param name="arguments">Not used</param>
    private void progressBarDisposed(object sender, EventArgs arguments) {

      // CHECK: This method is only called on an explicit Dispose() of the control.
      //   It is legal to call Control.BeginInvoke() without calling Control.EndInvoke(),
      //   so the code is quite correct even if no Dispose() occurs, but is it also clean?
      //   http://www.interact-sw.co.uk/iangblog/2005/05/16/endinvokerequired

      // Since this has to occur in the UI thread, there's no way that updateProgress()
      // could be executing just now. But the final call to updateProgress() will not
      // have EndInvoke() called on it yet, so we do this here before the control
      // is finally disposed.
      if(this.progressUpdateAsyncResult != null) {
        EndInvoke(this.progressUpdateAsyncResult);
        this.progressUpdateAsyncResult = null;
      }

    }

    /// <summary>Asynchronously updates the value to be shown in the progress bar</summary>
    /// <param name="value">New value to set the progress bar to</param>
    /// <remarks>
    ///   This will schedule an asynchronous update of the progress bar in the UI thread.
    ///   If you change the progress value again before the progress bar has completed its
    ///   update cycle, the original progress value will be skipped and the progress bar
    ///   jumps directly to the latest progress value. Updates are not queued, there is
    ///   at most one update waiting on the UI thread. It is also strictly guaranteed that
    ///   the last most progress value set will be shown and never skipped.
    /// </remarks>
    public void AsyncSetValue(float value) {

      // Update the value to be shown on the progress bar. If this happens multiple
      // times, that's not a problem, the progress bar updates as fast as it can
      // and always tries to show the most recent value assigned.
      float oldValue = Interlocked.Exchange(ref this.newProgress, value);

      // If the previous value was -1, the UI thread has already taken out the most recent
      // value and assigned it (or is about to assign it) to the progress bar control.
      // In this case, we'll wait until the current update has completed and immediately
      // begin the next update - since we know that the value the UI thread has extracted
      // is no longer the most recent one.
      if(oldValue == -1.0f) {
        if(this.progressUpdateAsyncResult != null)
          EndInvoke(this.progressUpdateAsyncResult);

        this.progressUpdateAsyncResult = BeginInvoke(this.updateProgressDelegate);
      }

    }

    /// <summary>Synchronously updates the value visualized in the progress bar</summary>
    private void updateProgress() {

      // Cache these to shorten the code that follows :)
      int minimum = base.Minimum;
      int maximum = base.Maximum;

      // Take out the most recent value that has been given to the asynchronous progress
      // bar up until now and replace it by -1. This enables the updater to see when
      // the update has actually been performed and whether it needs to start a new
      // invocation to ensure the most recent value will remain at the end.
      float progress = Interlocked.Exchange(ref this.newProgress, -1.0f);

      // Restrain the value to the progress bar's configured range and assign it.
      // This is done to prevent exceptions in the UI thread (theoretically the user
      // could change the progress bar's min and max just before the UI thread executes
      // this method, so we cannot validate the value in AsyncSetValue())
      int value = (int)(progress * (maximum - minimum)) + minimum;
      base.Value = Math.Min(Math.Max(value, minimum), maximum);

    }

    /// <summary>New progress being assigned to the progress bar</summary>
    private float newProgress;
    /// <summary>Delegate for the progress update method</summary>
    private MethodInvoker updateProgressDelegate;
    /// <summary>Async result for the invoked control state update method</summary>
    private volatile IAsyncResult progressUpdateAsyncResult;

  }

} // namespace Nuclex.Windows.Forms
