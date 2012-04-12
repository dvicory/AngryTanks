#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.Graphics.Batching;
using Nuclex.Support;

namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel {

  partial class ParticleSystemManager {

    #region class PruneAsyncResult

    /// <summary>Asynchronous result handle for the pruning process</summary>
    private class PruneAsyncResult : IAsyncResult, IDisposable {

      /// <summary>
      ///   Initializes a new asynchronous result for particle system prunes
      /// </summary>
      /// <param name="manager">Particle system manager being pruned</param>
      public PruneAsyncResult(ParticleSystemManager manager) {
        this.manager = manager;
        this.completed = true;

        this.stepCompletedDelegate = new AsyncCallback(stepCompleted);
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.doneEvent != null) {
          this.doneEvent.Close();
          this.doneEvent = null;
        }
      }

      /// <summary>Initializes a new result for asynchronous pruning</summary>
      /// <param name="callback">
      ///   Callback that will be invoked when pruning has finished
      /// </param>
      /// <param name="state">
      ///   User-defined state that will be passed to the callback
      /// </param>
      public void Start(AsyncCallback callback, object state) {
        lock(this) {
          this.completed = false;

          if(this.doneEvent != null) {
            this.doneEvent.Reset();
          }
        }
        this.exception = null;

        this.particleSystemCount = this.manager.particleSystems.Count;
        Interlocked.Exchange(ref this.currentParticleSystem, -1);
        this.callback = callback;
        this.state = state;

        stepCompleted(null);
      }

      /// <summary>User-defined state provided in the BeginPrune() method</summary>
      public object AsyncState { get { return this.state; } }

      /// <summary>Exception that occured during asynchronous processing</summary>
      public Exception AsyncException { get { return this.exception; } }

      /// <summary>WaitHandle that can be used to wait for pruning to finish</summary>
      public WaitHandle AsyncWaitHandle {
        get {
          if(this.doneEvent == null) {
            lock(this) {
              if(this.doneEvent == null) {
                this.doneEvent = new ManualResetEvent(this.completed);
              }
            }
          }

          return this.doneEvent;
        }
      }

      /// <summary>Whether the pruning process has completed synchronously</summary>
      public bool CompletedSynchronously { get { return false; } }

      /// <summary>Whether the pruning process has already finished</summary>
      public bool IsCompleted { get { return this.completed; } }

      /// <summary>Called when a single particle system has finished updating</summary>
      /// <param name="asyncResult">
      ///   Asynchronous result handle of the pruning process
      /// </param>
      private void stepCompleted(IAsyncResult asyncResult) {
        int current = Interlocked.Increment(ref this.currentParticleSystem);

        if(asyncResult != null) {
          try {
            this.manager.particleSystemHolders[current - 1].EndPrune(
              asyncResult
            );
          }
          catch(Exception exception) {
            this.exception = exception;
            reportCompletion();
            return;
          }
        }

        if(current == this.particleSystemCount) {
          reportCompletion();
        } else {
          this.stepAsyncResult = this.manager.particleSystemHolders[
            current
          ].BeginPrune(this.stepCompletedDelegate, null);
        }
      }

      /// <summary>Reports the completion of the pruning process to the caller</summary>
      private void reportCompletion() {
        AsyncCallback callbackCopy;
        lock(this) {
          callbackCopy = this.callback;
          this.completed = true;
          if(this.doneEvent != null) {
            this.doneEvent.Set();
          }
        }
        if(callbackCopy != null) {
          callbackCopy(this);
        }
      }

      /// <summary>Index of the particle system being currently pruned</summary>
      private int currentParticleSystem;
      /// <summary>Number of particles systems that will be pruned</summary>
      private int particleSystemCount;

      /// <summary>Asynchronous result of the currently pruning particle sytem</summary>
      private volatile IAsyncResult stepAsyncResult;
      /// <summary>Delegate for the stepCompleted() method</summary>
      private AsyncCallback stepCompletedDelegate;

      /// <summary>Particle system manager the async result belongs to</summary>
      private ParticleSystemManager manager;
      /// <summary>Callback that will be invoked after pruning has finished</summary>
      private AsyncCallback callback;
      /// <summary>User-defined state that will be passed to the callback</summary>
      private object state;
      /// <summary>Wait handle that can be used to wait for pruning to finish</summary>
      private volatile ManualResetEvent doneEvent;
      /// <summary>Whether pruning has already finished</summary>
      private volatile bool completed;
      /// <summary>Exception that occured during asynchronous processing</summary>
      private volatile Exception exception;

    }

    #endregion // class PruneAsyncResult

    #region class UpdateAsyncResult

    /// <summary>Asynchronous result handle for the update process</summary>
    private class UpdateAsyncResult : IAsyncResult, IDisposable {

      /// <summary>
      ///   Initializes a new asynchronous result for particle system updates
      /// </summary>
      /// <param name="manager">Particle system manager being updated</param>
      public UpdateAsyncResult(ParticleSystemManager manager) {
        this.manager = manager;
        this.completed = true;

        this.stepCompletedDelegate = new AsyncCallback(stepCompleted);
      }

      /// <summary>Immediateyl releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.doneEvent != null) {
          this.doneEvent.Close();
          this.doneEvent = null;
        }
      }

      /// <summary>Initializes a new result for an asynchronous update</summary>
      /// <param name="updates">
      ///   Number of updates that will be performed on the particle systems
      /// </param>
      /// <param name="threads">Number of threads that will be used</param>
      /// <param name="callback">
      ///   Callback that will be invoked when updating has finished
      /// </param>
      /// <param name="state">
      ///   User-defined state that will be passed to the callback
      /// </param>
      public void Start(int updates, int threads, AsyncCallback callback, object state) {
        lock(this) {
          this.completed = false;

          if(this.doneEvent != null) {
            this.doneEvent.Reset();
          }
        }
        this.exception = null;

        this.particleSystemCount = this.manager.particleSystems.Count;
        this.currentParticleSystem = -1;
        this.updates = updates;
        this.threads = threads;
        this.callback = callback;
        this.state = state;

        stepCompleted(null);
      }

      /// <summary>User-defined state provided in the BeginUpdate() method</summary>
      public object AsyncState { get { return this.state; } }

      /// <summary>Exception that occured during asynchronous processing</summary>
      public Exception AsyncException { get { return this.exception; } }

      /// <summary>WaitHandle that can be used to wait for updating to finish</summary>
      public WaitHandle AsyncWaitHandle {
        get {
          if(this.doneEvent == null) {
            lock(this) {
              if(this.doneEvent == null) {
                this.doneEvent = new ManualResetEvent(this.completed);
              }
            }
          }

          return this.doneEvent;
        }
      }

      /// <summary>Whether the updating process has completed synchronously</summary>
      public bool CompletedSynchronously { get { return false; } }

      /// <summary>Whether the updating process has already finished</summary>
      public bool IsCompleted { get { return this.completed; } }

      /// <summary>Called when a single particle system has finished updating</summary>
      /// <param name="asyncResult">
      ///   Asynchronous result handle of the update process
      /// </param>
      private void stepCompleted(IAsyncResult asyncResult) {
        int current = Interlocked.Increment(ref this.currentParticleSystem);
        if(asyncResult != null) {
          try {
            this.manager.particleSystemHolders[current - 1].EndUpdate(
              asyncResult
            );
          }
          catch(Exception exception) {
            this.exception = exception;
            reportCompletion();
            return;
          }
        }

        if(current == this.particleSystemCount) {
          reportCompletion();
        } else {
          this.stepAsyncResult = this.manager.particleSystemHolders[
            current
          ].BeginUpdate(this.updates, this.threads, this.stepCompletedDelegate, null);
        }
      }

      /// <summary>Reports the completion of the update to the caller</summary>
      private void reportCompletion() {
        AsyncCallback callbackCopy;
        lock(this) {
          callbackCopy = this.callback;
          this.completed = true;
          if(this.doneEvent != null) {
            this.doneEvent.Set();
          }
        }
        if(callbackCopy != null) {
          callbackCopy(this);
        }
      }

      /// <summary>Index of the particle system being currently updated</summary>
      private int currentParticleSystem;
      /// <summary>Number of particles systems that will be updated</summary>
      private int particleSystemCount;
      /// <summary>Number of updates to perform on the particle systems</summary>
      private int updates;
      /// <summary>Number of threads to use for the updates</summary>
      private int threads;

      /// <summary>Asynchronous result of the currently updating particle sytem</summary>
      private volatile IAsyncResult stepAsyncResult;
      /// <summary>Delegate for the stepCompleted() method</summary>
      private AsyncCallback stepCompletedDelegate;

      /// <summary>Particle system manager the async result belongs to</summary>
      private ParticleSystemManager manager;
      /// <summary>Callback that will be invoked after updating has finished</summary>
      private AsyncCallback callback;
      /// <summary>User-defined state that will be passed to the callback</summary>
      private object state;
      /// <summary>Wait handle that can be used to wait for pruning to finish</summary>
      private volatile ManualResetEvent doneEvent;
      /// <summary>Whether the update has already completed</summary>
      private volatile bool completed;
      /// <summary>Exception that occured during asynchronous processing</summary>
      private volatile Exception exception;

    }

    #endregion // class UpdateAsyncResult

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel
