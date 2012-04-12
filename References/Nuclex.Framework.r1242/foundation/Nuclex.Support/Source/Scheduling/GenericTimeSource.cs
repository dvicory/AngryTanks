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
using System.Diagnostics;
using System.Threading;

namespace Nuclex.Support.Scheduling {

  /// <summary>
  ///   Generic time source implementation using the Stopwatch or Environment.TickCount
  /// </summary>
  public class GenericTimeSource : ITimeSource {

    /// <summary>Number of ticks (100 ns intervals) in a millisecond</summary>
    private const long TicksPerMillisecond = 10000;

    /// <summary>Tolerance for the detection of a date/time adjustment</summary>
    /// <remarks>
    ///   If the current system date/time jumps by more than this tolerance into any
    ///   direction, the default time source will trigger the DateTimeAdjusted event.
    /// </remarks>
    private const long TimeAdjustmentToleranceTicks = 75 * TicksPerMillisecond;

    /// <summary>Called when the system date/time are adjusted</summary>
    /// <remarks>
    ///   An adjustment is a change out of the ordinary, eg. when a time synchronization
    ///   alters the current system time, when daylight saving time takes effect or
    ///   when the user manually adjusts the system date/time.
    /// </remarks>
    public event EventHandler DateTimeAdjusted;

    /// <summary>Initializes the static fields of the default time source</summary>
    static GenericTimeSource() {
      tickFrequency = 10000000.0;
      tickFrequency /= (double)Stopwatch.Frequency;
    }

    /// <summary>Initializes the default time source</summary>
    public GenericTimeSource() : this(Stopwatch.IsHighResolution) { }

    /// <summary>Initializes the default time source</summary>
    /// <param name="useStopwatch">
    ///   Whether to use the Stopwatch class for measuring time
    /// </param>
    /// <remarks>
    ///   <para>
    ///     Normally it's a good idea to use the default constructor. If the Stopwatch
    ///     is unable to use the high-resolution timer, it will fall back to
    ///     DateTime.Now (as stated on MSDN). This is bad because then the tick count
    ///     will jump whenever the system time changes (eg. when the system synchronizes
    ///     its time with a time server).
    ///   </para>
    ///   <para>
    ///     Your can safely use this constructor if you always set its arugment to 'false',
    ///     but then your won't profit from the high-resolution timer if one is available.
    ///   </para>
    /// </remarks>
    public GenericTimeSource(bool useStopwatch) {
      this.useStopwatch = useStopwatch;

      // Update the lastCheckedTime and lastCheckedTicks fields
      checkForTimeAdjustment();
    }

    /// <summary>Waits for an AutoResetEvent to become signalled</summary>
    /// <param name="waitHandle">WaitHandle the method will wait for</param>
    /// <param name="ticks">Number of ticks to wait</param>
    /// <returns>
    ///   True if the WaitHandle was signalled, false if the timeout was reached
    /// </returns>
    public virtual bool WaitOne(AutoResetEvent waitHandle, long ticks) {

      // Force a timeout at least each second so the caller can check the system time
      // since we're not able to provide the DateTimeAdjusted notification
      int milliseconds = (int)(ticks / TicksPerMillisecond);
#if XNA_3
      bool signalled = waitHandle.WaitOne(Math.Min(1000, milliseconds), false);
#elif XBOX360 || WINDOWS_PHONE
      bool signalled = waitHandle.WaitOne(Math.Min(1000, milliseconds));
#else
      bool signalled = waitHandle.WaitOne(Math.Min(1000, milliseconds), false);
#endif
      // See whether the system date/time have been adjusted while we were asleep.
      checkForTimeAdjustment();

      // Now tell the caller whether his event was signalled
      return signalled;

    }

    /// <summary>Current system time in UTC format</summary>
    public DateTime CurrentUtcTime {
      get { return DateTime.UtcNow; }
    }

    /// <summary>How long the time source has been running</summary>
    /// <remarks>
    ///   There is no guarantee this value starts at zero (or anywhere near it) when
    ///   the time source is created. The only requirement for this value is that it
    ///   keeps increasing with the passing of time and that it stays unaffected
    ///   (eg. doesn't skip or jump back) when the system date/time are changed.
    /// </remarks>
    public long Ticks {
      get {

        // The docs say if Stopwatch.IsHighResolution is false, it will return
        // DateTime.Now (actually DateTime.UtcNow). This means that the Stopwatch is
        // prone to skips and jumps during DST crossings and NTP synchronizations,
        // so we cannot use it in that case.
        if(this.useStopwatch) {
          double timestamp = (double)Stopwatch.GetTimestamp();
          return (long)(timestamp * tickFrequency);
        }

        // Fallback: Use Environment.TickCount instead. Not as accurate, but at least
        // it will not jump around when the date or time are adjusted.
        return Environment.TickCount * TicksPerMillisecond;

      }
    }

    /// <summary>Called when the system time is changed</summary>
    /// <param name="sender">Not used</param>
    /// <param name="arguments">Not used</param>
    protected virtual void OnDateTimeAdjusted(object sender, EventArgs arguments) {
      EventHandler copy = DateTimeAdjusted;
      if(copy != null) {
        copy(sender, arguments);
      }
    }

    /// <summary>
    ///   Checks whether the system/date time have been adjusted since the last call
    /// </summary>
    private void checkForTimeAdjustment() {

      // Grab the current date/time and timer ticks in one go
      long currentDateTimeTicks = DateTime.UtcNow.Ticks;
      long currentStopwatchTicks = Ticks;

      // Calculate the number of timer ticks that have passed since the last check and
      // extrapolate the local date/time we should be expecting to see
      long ticksSinceLastCheck = currentStopwatchTicks - lastCheckedStopwatchTicks;
      long expectedLocalTimeTicks = this.lastCheckedDateTimeTicks + ticksSinceLastCheck;

      // Find out by what amount the actual local date/time deviates from
      // the extrapolated date/time and trigger the date/time adjustment event if
      // we can reasonably assume that the system date/time have been adjusted.
      long deviationTicks = Math.Abs(expectedLocalTimeTicks - currentDateTimeTicks);
      if(deviationTicks > TimeAdjustmentToleranceTicks) {
        OnDateTimeAdjusted(this, EventArgs.Empty);
      }

      // Remember the current local date/time and timer ticks for the next run
      this.lastCheckedDateTimeTicks = currentDateTimeTicks;
      this.lastCheckedStopwatchTicks = currentStopwatchTicks;

    }

    /// <summary>Last local time we checked for a date/time adjustment</summary>
    private long lastCheckedDateTimeTicks;
    /// <summary>Timer ticks at which we last checked the local time</summary>
    private long lastCheckedStopwatchTicks;

    /// <summary>Number of ticks per Stopwatch time unit</summary>
    private static double tickFrequency;
    /// <summary>Whether ot use the Stopwatch class for measuring time</summary>
    private bool useStopwatch;

  }

} // namespace Nuclex.Support.Scheduling
