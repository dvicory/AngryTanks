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
using System.Diagnostics;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Scheduling {

  /// <summary>Schedules actions for execution at a future point in time</summary>
  partial class Scheduler {

    #region class TimeSourceSingleton

    /// <summary>
    ///   Manages the singleton instance of the scheduler's default time source
    /// </summary>
    private class TimeSourceSingleton {

      /// <summary>
      ///   Explicit static constructor to guarantee the singleton is initialized only
      ///   when a static member of this class is accessed.
      /// </summary>
      static TimeSourceSingleton() { } // Do not remove!

      /// <summary>The singleton instance of the default time source</summary>
      internal static readonly ITimeSource Instance = Scheduler.CreateDefaultTimeSource();

    }

    #endregion // class TimeSourceSingleton

    /// <summary>Returns the default time source for the scheduler</summary>
    public static ITimeSource DefaultTimeSource {
      get { return TimeSourceSingleton.Instance; }
    }

    /// <summary>Creates a new default time source for the scheduler</summary>
    /// <param name="useWindowsTimeSource">
    ///   Whether the specialized windows time source should be used
    /// </param>
    /// <returns>The newly created time source</returns>
    internal static ITimeSource CreateTimeSource(bool useWindowsTimeSource) {
      if(useWindowsTimeSource) {
        return new WindowsTimeSource();
      } else {
        return new GenericTimeSource();
      }
    }

    /// <summary>Creates a new default time source for the scheduler</summary>
    /// <returns>The newly created time source</returns>
    internal static ITimeSource CreateDefaultTimeSource() {
      return CreateTimeSource(WindowsTimeSource.Available);
    }

  }

} // namespace Nuclex.Support.Scheduling
