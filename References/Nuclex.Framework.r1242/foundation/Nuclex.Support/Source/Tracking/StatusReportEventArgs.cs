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

  /// <summary>Event arguments for reporting a status to the subscriber</summary>
  public class StatusReportEventArgs : EventArgs {

    /// <summary>Initializes a new status report event arguments container</summary>
    /// <param name="status">Status to report to the event's subscribers</param>
    public StatusReportEventArgs(string status) {
      this.status = status;
    }

    /// <summary>The currently reported status</summary>
    /// <remarks>
    ///   The contents of this string are up to the publisher of the event to
    ///   define. Though it is recommended to report the status as a human-readable
    ///   string, these strings might not in all cases be properly localized or
    ///   suitable for display in a GUI.
    /// </remarks>
    public string Status {
      get { return this.status; }
    }

    /// <summary>Reported status</summary>
    private string status;

  }

} // namespace Nuclex.Support.Tracking
