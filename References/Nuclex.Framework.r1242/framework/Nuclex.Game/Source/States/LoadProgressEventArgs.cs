using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Game.States {

  /// <summary>Event argument container for a loading progress notification</summary>
  public class LoadProgressEventArgs : EventArgs {

    /// <summary>Initializes a new load progress event argument container</summary>
    /// <param name="progress">
    ///   Progress that will be reported to the event subscribers
    /// </param>
    public LoadProgressEventArgs(float progress) {
      this.progress = progress;
    }

    /// <summary>Loading progress achieved so far</summary>
    public float Progress {
      get { return this.progress; }
    }

    /// <summary>Loading progress achieved so far</summary>
    private float progress;

  }

} // namespace Nuclex.Game.States
