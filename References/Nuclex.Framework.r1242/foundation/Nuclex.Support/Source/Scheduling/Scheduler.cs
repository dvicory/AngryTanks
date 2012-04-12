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
  public partial class Scheduler : ISchedulerService, IDisposable {

    /// <summary>One tick is 100 ns, meaning 10000 ticks equal 1 ms</summary>
    private const long TicksPerMillisecond = 10000;

    #region class Notification

    /// <summary>Scheduled notification</summary>
    private class Notification {

      /// <summary>Initializes a new notification</summary>
      /// <param name="intervalTicks">
      ///   Interval in which the notification will re-executed
      /// </param>
      /// <param name="nextDueTicks">
      ///   Time source ticks the notification is next due at
      /// </param>
      /// <param name="absoluteUtcTime">
      ///   Absolute time in UTC at which the notification is due
      /// </param>
      /// <param name="callback">
      ///   Callback to be invoked when the notification is due
      /// </param>
      public Notification(
        long intervalTicks,
        long nextDueTicks,
        DateTime absoluteUtcTime,
        WaitCallback callback
      ) {
        this.IntervalTicks = intervalTicks;
        this.NextDueTicks = nextDueTicks;
        this.AbsoluteUtcTime = absoluteUtcTime;
        this.Callback = callback;
        this.Cancelled = false;
      }

      /// <summary>
      ///   Ticks specifying the interval in which the notification will be re-executed
      /// </summary>
      public long IntervalTicks;

      /// <summary>Next due time for this notification</summary>
      public long NextDueTicks;
      /// <summary>Absolute time in UTC at which the notification is due</summary>
      /// <remarks>
      ///   Only stored for notifications scheduled in absolute time, meaning they
      ///   have to be adjusted if the system date/time changes
      /// </remarks>
      public DateTime AbsoluteUtcTime;
      /// <summary>Callback that will be invoked when the notification is due</summary>
      public WaitCallback Callback;
      /// <summary>Whether the notification has been cancelled</summary>
      public bool Cancelled;

    }

    #endregion // class Notification

    #region class NotificationComparer

    /// <summary>Compares two notifications to each other</summary>
    private class NotificationComparer : IComparer<Notification> {

      /// <summary>The default instance of the notification comparer</summary>
      public static readonly NotificationComparer Default = new NotificationComparer();

      /// <summary>Compares two notifications to each other based on their time</summary>
      /// <param name="left">Notification that will be compared on the left side</param>
      /// <param name="right">Notification that will be comapred on the right side</param>
      /// <returns>The relation of the two notification's times to each other</returns>
      public int Compare(Notification left, Notification right) {
        if(left.NextDueTicks > right.NextDueTicks) {
          return -1;
        } else if(left.NextDueTicks < right.NextDueTicks) {
          return 1;
        } else {
          return 0;
        }
      }

    }

    #endregion // class NotificationComparer

    /// <summary>Initializes a new scheduler using the default time source</summary>
    public Scheduler() : this(DefaultTimeSource) { }

    /// <summary>Initializes a new scheduler using the specified time source</summary>
    /// <param name="timeSource">Time source the scheduler will use</param>
    public Scheduler(ITimeSource timeSource) {
      this.dateTimeAdjustedDelegate = new EventHandler(dateTimeAdjusted);

      this.timeSource = timeSource;
      this.timeSource.DateTimeAdjusted += this.dateTimeAdjustedDelegate;

      this.notifications = new PriorityQueue<Notification>(NotificationComparer.Default);
      this.notificationWaitEvent = new AutoResetEvent(false);

      this.timerThread = new Thread(new ThreadStart(runTimerThread));
      this.timerThread.Name = "Nuclex.Support.Scheduling.Scheduler";
#if XNA_3
      this.timerThread.Priority = ThreadPriority.Highest;
#elif !XBOX360 && !WINDOWS_PHONE
      this.timerThread.Priority = ThreadPriority.Highest;
#endif
      this.timerThread.IsBackground = true;
      this.timerThread.Start();
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.timerThread != null) {
        this.endRequested = true;
        this.notificationWaitEvent.Set();

        // Wait for the timer thread to exit. If it doesn't exit in 10 seconds (which is
        // a lot of time given that it doesn't do any real work), forcefully abort
        // the thread. This may risk some leaks, but it's the only thing we can do.
        bool success = this.timerThread.Join(2500);
#if !XBOX360 && !WINDOWS_PHONE
        Trace.Assert(success, "Scheduler timer thread did not exit in time");
#endif
        // Unsubscribe from the time source to avoid surprise events during or
        // after shutdown
        if(this.timeSource != null) {
          this.timeSource.DateTimeAdjusted -= this.dateTimeAdjustedDelegate;
          this.timeSource = null;
        }

        // Get rid of the notification wait event now that we've made sure that
        // the timer thread is down.
        this.notificationWaitEvent.Close();

        // Help the GC a bit
        this.notificationWaitEvent = null;
        this.notifications = null;

        // Set to null so we don't attempt to end the thread again if Dispose() is
        // called multiple times.
        this.timerThread = null;
      }
    }

    /// <summary>Time source being used by the scheduler</summary>
    public ITimeSource TimeSource {
      get { return this.timeSource; }
    }

    /// <summary>Schedules a notification at the specified absolute time</summary>
    /// <param name="notificationTime">
    ///   Absolute time at which the notification will occur
    /// </param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    /// <remarks>
    ///   The notification is scheduled for the indicated absolute time. If the system
    ///   enters/leaves daylight saving time or the date/time is changed (for example
    ///   when the system synchronizes with an NTP server), this will affect
    ///   the notification. So if you need to be notified after a fixed time, use
    ///   the NotifyIn() method instead.
    /// </remarks>
    public object NotifyAt(DateTime notificationTime, WaitCallback callback) {
      if(notificationTime.Kind == DateTimeKind.Unspecified) {
        throw new ArgumentException(
          "Notification time is neither UTC or local", "notificationTime"
        );
      }

      DateTime notificationTimeUtc = notificationTime.ToUniversalTime();
      DateTime now = this.timeSource.CurrentUtcTime;

      long remainingTicks = notificationTimeUtc.Ticks - now.Ticks;
      long nextDueTicks = this.timeSource.Ticks + remainingTicks;

      return scheduleNotification(
        new Notification(
          0,
          nextDueTicks,
          notificationTimeUtc,
          callback
        )
      );
    }

    /// <summary>Schedules a notification after the specified time span</summary>
    /// <param name="delay">Delay after which the notification will occur</param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyIn(TimeSpan delay, WaitCallback callback) {
      return scheduleNotification(
        new Notification(
          0,
          this.timeSource.Ticks + delay.Ticks,
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>
    ///   Schedules a notification after the specified amount of milliseconds
    /// </summary>
    /// <param name="delayMilliseconds">
    ///   Number of milliseconds after which the notification will occur
    /// </param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyIn(int delayMilliseconds, WaitCallback callback) {
      return scheduleNotification(
        new Notification(
          0,
          this.timeSource.Ticks + ((long)delayMilliseconds * TicksPerMillisecond),
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>
    ///   Schedules a recurring notification after the specified time span
    /// </summary>
    /// <param name="delay">Delay after which the first notification will occur</param>
    /// <param name="interval">Interval at which the notification will be repeated</param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyEach(TimeSpan delay, TimeSpan interval, WaitCallback callback) {
      return scheduleNotification(
        new Notification(
          interval.Ticks,
          this.timeSource.Ticks + delay.Ticks,
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>
    ///   Schedules a recurring notification after the specified amount of milliseconds
    /// </summary>
    /// <param name="delayMilliseconds">
    ///   Milliseconds after which the first notification will occur
    /// </param>
    /// <param name="intervalMilliseconds">
    ///   Interval in milliseconds at which the notification will be repeated
    /// </param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyEach(
      int delayMilliseconds, int intervalMilliseconds, WaitCallback callback
    ) {
      return scheduleNotification(
        new Notification(
          (long)intervalMilliseconds * TicksPerMillisecond,
          this.timeSource.Ticks + ((long)delayMilliseconds * TicksPerMillisecond),
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>Cancels a scheduled notification</summary>
    /// <param name="notificationHandle">
    ///   Handle of the notification that will be cancelled
    /// </param>
    public void Cancel(object notificationHandle) {
      Notification notification = notificationHandle as Notification;
      if(notification != null) {
        notification.Cancelled = true;
      }
    }

    /// <summary>Called when the system date/time have been adjusted</summary>
    /// <param name="sender">Time source which detected the adjustment</param>
    /// <param name="arguments">Not used</param>
    private void dateTimeAdjusted(object sender, EventArgs arguments) {
      lock(this.timerThread) {
        long currentTicks = this.timeSource.Ticks;
        DateTime currentTime = this.timeSource.CurrentUtcTime;

        PriorityQueue<Notification> updatedQueue = new PriorityQueue<Notification>(
          NotificationComparer.Default
        );

        // Copy all notifications from the original queue to a new one, adjusting
        // those with an absolute notification time along the way to a new due tick
        while(this.notifications.Count > 0) {
          Notification notification = this.notifications.Dequeue();
          if(!notification.Cancelled) {

            // If this notification has an absolute due time, adjust its due tick
            if(notification.AbsoluteUtcTime != DateTime.MinValue) {

              // Combining recurrent notifications with absolute time isn't allowed
              Debug.Assert(notification.IntervalTicks == 0);

              // Make the adjustment
              long remainingTicks = (notification.AbsoluteUtcTime - currentTime).Ticks;
              notification.NextDueTicks = currentTicks + remainingTicks;

            }

            // Notification processed, move it over to the new priority queue
            updatedQueue.Enqueue(notification);

          }
        }

        // Replace the working queue with the updated queue
        this.notifications = updatedQueue;
      }

      this.notificationWaitEvent.Set();
    }

    /// <summary>Schedules a notification for processing by the timer thread</summary>
    /// <param name="notification">Notification that will be scheduled</param>
    /// <returns>The scheduled notification</returns>
    private object scheduleNotification(Notification notification) {
      lock(this.timerThread) {
        this.notifications.Enqueue(notification);

        // If this notification has become the next due notification, wake up
        // the timer thread so it can adjust its sleep period.
        if(ReferenceEquals(this.notifications.Peek(), notification)) {
          this.notificationWaitEvent.Set();
        }
      }

      return notification;
    }

    /// <summary>Executes the timer thread</summary>
    private void runTimerThread() {
      Notification nextDueNotification;
      lock(this.timerThread) {
        nextDueNotification = getNextDueNotification();
      }

      // Keep processing notifications until we're told to quit
      for(; ; ) {

        // Wait until the nextmost notification is due or something else wakes us up
        if(nextDueNotification == null) {
          this.notificationWaitEvent.WaitOne();
        } else {
          long remainingTicks = nextDueNotification.NextDueTicks - this.timeSource.Ticks;
          if(remainingTicks > 0) {
            this.timeSource.WaitOne(this.notificationWaitEvent, remainingTicks);
          }
        }

        // Have we been woken up because the Scheduler is being disposed?
        if(this.endRequested) {
          break;
        }

        // Process all notifications that are due by handing them over to the thread pool.
        // The notification queue might have been updated while we were sleeping, so
        // look for the notification that is due next again
        long ticks = this.timeSource.Ticks;
        lock(this.timerThread) {
          for(; ; ) {
            nextDueNotification = getNextDueNotification();
            if(nextDueNotification == null) {
              break;
            }

            // If the next notification is more than a millisecond away, we've reached
            // the end of the notifications we need to process.
            long remainingTicks = (nextDueNotification.NextDueTicks - ticks);
            if(remainingTicks >= TicksPerMillisecond) {
              break;
            }

            if(!nextDueNotification.Cancelled) {
              ThreadPool.QueueUserWorkItem(nextDueNotification.Callback);
            }

            this.notifications.Dequeue();
            if(nextDueNotification.IntervalTicks != 0) {
              nextDueNotification.NextDueTicks += nextDueNotification.IntervalTicks;
              this.notifications.Enqueue(nextDueNotification);
            }
          } // for
        } // lock

      } // for
    }

    /// <summary>Retrieves the notification that is due next</summary>
    /// <returns>The notification that is due next</returns>
    private Notification getNextDueNotification() {
      while(this.notifications.Count > 0) {
        Notification nextDueNotification = this.notifications.Peek();
        if(nextDueNotification.Cancelled) {
          this.notifications.Dequeue();
        } else {
          return nextDueNotification;
        }
      }

      return null;
    }

    /// <summary>Time source used by the scheduler</summary>
    private ITimeSource timeSource;
    /// <summary>Thread that will wait for the next scheduled event</summary>
    private Thread timerThread;
    /// <summary>Notifications in the scheduler's queue</summary>
    private PriorityQueue<Notification> notifications;

    /// <summary>Event used by the timer thread to wait for the next notification</summary>
    private AutoResetEvent notificationWaitEvent;
    /// <summary>Whether the timer thread should end</summary>
    private volatile bool endRequested;

    /// <summary>Delegate for the dateTimeAdjusted() method</summary>
    private EventHandler dateTimeAdjustedDelegate;

  }

} // namespace Nuclex.Support.Scheduling
