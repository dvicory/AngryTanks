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

namespace Nuclex.Support.Tracking {

  /// <summary>Base class for background processes the user can wait on</summary>
  /// <remarks>
  ///   <para>
  ///     By encapsulating long-running operations which will ideally be running in
  ///     a background thread in a class that's derived from <see cref="Transaction" />
  ///     you can wait for the completion of the operation and optionally even receive
  ///     feedback on the achieved progress. This is useful for displaying a progress
  ///     bar, loading screen or some other means of entertaining the user while he
  ///     waits for the task to complete.
  ///   </para>
  ///   <para>
  ///     You can register callbacks which will be fired once the <see cref="Transaction" />
  ///     task has completed. This class deliberately does not provide an Execute()
  ///     method or anything similar to clearly seperate the initiation of an operation
  ///     from just monitoring it. By omitting an Execute() method, it also becomes
  ///     possible to construct a transaction just-in-time when it is explicitely being
  ///     asked for.
  ///   </para>
  /// </remarks>
  public abstract class Transaction {

    #region class EndedDummyTransaction

    /// <summary>Dummy transaction which always is in the 'ended' state</summary>
    private class EndedDummyTransaction : Transaction {

      /// <summary>Initializes a new ended dummy transaction</summary>
      public EndedDummyTransaction() {
        OnAsyncEnded();
      }

    }

    #endregion // class EndedDummyTransaction

    /// <summary>A dummy transaction that's always in the 'ended' state</summary>
    /// <remarks>
    ///   Useful if an operation is already complete when it's being asked for or
    ///   when a transaction that's lazily created is accessed after the original
    ///   operation has ended already.
    /// </remarks>
    public static readonly Transaction EndedDummy = new EndedDummyTransaction();

    /// <summary>Will be triggered when the transaction has ended</summary>
    /// <remarks>
    ///   If the process is already finished when a client registers to this event,
    ///   the registered callback will be invoked synchronously right when the
    ///   registration takes place.
    /// </remarks>
    public virtual event EventHandler AsyncEnded {
      add {

        // If the background process has not yet ended, add the delegate to the
        // list of subscribers. This uses the double-checked locking idiom to
        // avoid taking the lock when the background process has already ended.
        if(!this.ended) {
          lock(this) {
            if(!this.ended) {

              // The subscriber list is also created lazily ;-)
              if(ReferenceEquals(this.endedEventSubscribers, null)) {
                this.endedEventSubscribers = new List<EventHandler>();
              }

              // Subscribe the event handler to the list
              this.endedEventSubscribers.Add(value);
              return;

            }
          }
        }

        // If this point is reached, the background process was already finished
        // and we have to invoke the subscriber manually as promised.
        value(this, EventArgs.Empty);

      }
      remove {

        if(!this.ended) {
          lock(this) {
            if(!this.ended) {

              // Only try to remove the event handler if the subscriber list was created,
              // otherwise, we can be sure that no actual subscribers exist.
              if(!ReferenceEquals(this.endedEventSubscribers, null)) {
                int eventHandlerIndex = this.endedEventSubscribers.IndexOf(value);

                // Unsubscribing a non-subscribed delegate from an event is allowed and
                // should not throw an exception.
                if(eventHandlerIndex != -1) {
                  this.endedEventSubscribers.RemoveAt(eventHandlerIndex);
                }
              }

            }
          }
        }

      }
    }

    /// <summary>Waits until the background process finishes</summary>
    public virtual void Wait() {
      if(!this.ended) {
        WaitHandle.WaitOne();
      }
    }

#if !XBOX360 && !WINDOWS_PHONE

    /// <summary>Waits until the background process finishes or a timeout occurs</summary>
    /// <param name="timeout">
    ///   Time span after which to stop waiting and return immediately
    /// </param>
    /// <returns>
    ///   True if the background process completed, false if the timeout was reached
    /// </returns>
    public virtual bool Wait(TimeSpan timeout) {
      if(this.ended) {
        return true;
      }

      return WaitHandle.WaitOne(timeout, false);
    }

#endif // !XBOX360

    /// <summary>Waits until the background process finishes or a timeout occurs</summary>
    /// <param name="timeoutMilliseconds">
    ///   Number of milliseconds after which to stop waiting and return immediately
    /// </param>
    /// <returns>
    ///   True if the background process completed, false if the timeout was reached
    /// </returns>
    public virtual bool Wait(int timeoutMilliseconds) {
      if(this.ended) {
        return true;
      }

#if XNA_3
      return WaitHandle.WaitOne(timeoutMilliseconds, false);
#elif XBOX360 || WINDOWS_PHONE
      return WaitHandle.WaitOne(timeoutMilliseconds);
#else
      return WaitHandle.WaitOne(timeoutMilliseconds, false);
#endif
    }

    /// <summary>Whether the transaction has ended already</summary>
    public virtual bool Ended {
      get { return this.ended; }
    }

    /// <summary>WaitHandle that can be used to wait for the transaction to end</summary>
    public virtual WaitHandle WaitHandle {
      get {

        // The WaitHandle will only be created when someone asks for it!
        // We can *not* optimize this lock away since we absolutely must not create
        // two doneEvents -- someone might call .WaitOne() on the first one when only
        // the second one is referenced by this.doneEvent and thus gets set in the end.
        if(this.doneEvent == null) {
          lock(this) {
            if(this.doneEvent == null) {
              this.doneEvent = new ManualResetEvent(this.ended);
            }
          }
        }

        // We can be sure the doneEvent has been created now!
        return this.doneEvent;

      }
    }

    /// <summary>Fires the AsyncEnded event</summary>
    /// <remarks>
    ///   <para>
    ///     This event should be fired by the implementing class when its work is completed.
    ///     It's of no interest to this class whether the outcome of the process was
    ///     successfull or not, the outcome and results of the process taking place both
    ///     need to be communicated seperately.
    ///   </para>
    ///   <para>
    ///     Calling this method is mandatory. Implementers need to take care that
    ///     the OnAsyncEnded() method is called on any instance of transaction that's
    ///     being created. This method also must not be called more than once.
    ///   </para>
    /// </remarks>
    protected virtual void OnAsyncEnded() {

      // Make sure the transaction is not ended more than once. By guaranteeing that
      // a transaction can only be ended once, we allow users of this class to
      // skip some safeguards against notifications arriving twice.
      lock(this) {

        // No double lock here, this is an exception that indicates an implementation
        // error that will not be triggered under normal circumstances. We don't need
        // to waste any effort optimizing the speed at which an implementation fault
        // will be reported ;-)
        if(this.ended)
          throw new InvalidOperationException("The transaction has already been ended");

        this.ended = true;

        // Doesn't really need a lock: if another thread wins the race and creates
        // the event after we just saw it being null, it would be created in an already
        // set state due to the ended flag (see above) being set to true beforehand!
        // But since we've got a lock ready, we can even avoid that 1 in a million
        // performance loss and prevent the doneEvent from being signalled needlessly.
        if(this.doneEvent != null)
          this.doneEvent.Set();

      }

      // Fire the ended events to all event subscribers. We can freely use the list
      // without synchronization at this point on since once this.ended is set to true,
      // the subscribers list will not be accessed any longer
      if(!ReferenceEquals(this.endedEventSubscribers, null)) {
        for(int index = 0; index < this.endedEventSubscribers.Count; ++index) {
          this.endedEventSubscribers[index](this, EventArgs.Empty);
        }
        this.endedEventSubscribers = null;
      }

    }

    /// <summary>Event handlers which have subscribed to the ended event</summary>
    /// <remarks>
    ///   Does not need to be volatile since it's only accessed inside 
    /// </remarks>
    protected volatile List<EventHandler> endedEventSubscribers;
    /// <summary>Whether the operation has completed yet</summary>
    protected volatile bool ended;
    /// <summary>Event that will be set when the transaction is completed</summary>
    /// <remarks>
    ///   This event is will only be created when it is specifically asked for using
    ///   the WaitHandle property.
    /// </remarks>
    protected volatile ManualResetEvent doneEvent;

  }

} // namespace Nuclex.Support.Tracking
