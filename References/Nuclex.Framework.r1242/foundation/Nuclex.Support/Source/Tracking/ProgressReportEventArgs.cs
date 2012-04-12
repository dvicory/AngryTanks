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

  /// <summary>Event arguments for a progress update notification</summary>
  public class ProgressReportEventArgs : EventArgs {

    /// <summary>Initializes the progress update informations</summary>
    /// <param name="progress">Achieved progress ranging from 0.0 to 1.0</param>
    public ProgressReportEventArgs(float progress) {
      this.progress = progress;
    }

    /// <summary>Currently achieved progress</summary>
    public float Progress {
      get { return this.progress; }
    }

    /// <summary>Achieved progress</summary>
    private float progress;

  }

} // namespace Nuclex.Support.Tracking
