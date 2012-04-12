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

  /// <summary>Event arguments for an idle state change notification</summary>
  public class IdleStateEventArgs : EventArgs {

    /// <summary>Initializes the idle state change notification</summary>
    /// <param name="idle">The new idle state</param>
    public IdleStateEventArgs(bool idle) {
      this.idle = idle;
    }

    /// <summary>Current idle state</summary>
    public bool Idle {
      get { return this.idle; }
    }

    /// <summary>Current idle state</summary>
    private bool idle;

  }

} // namespace Nuclex.Support.Tracking
