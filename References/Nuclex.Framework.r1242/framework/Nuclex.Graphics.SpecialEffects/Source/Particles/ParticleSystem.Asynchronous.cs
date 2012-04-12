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

using Nuclex.Support;

namespace Nuclex.Graphics.SpecialEffects.Particles {

  partial class ParticleSystem<ParticleType> {

    #region class AsyncPrune

    /// <summary>Prunes dead particles in the particle system asynchronously</summary>
    private class AsyncPrune : IAsyncResult, IDisposable {

      /// <summary>Initializes a new asynchronous prune process</summary>
      /// <param name="particleSystem">Particle system that will be pruned</param>
      public AsyncPrune(ParticleSystem<ParticleType> particleSystem) {
        this.particleSystem = particleSystem;

        this.runDelegate = new WaitCallback(run);
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.doneEvent != null) {
          this.doneEvent.Close();
          this.doneEvent = null;
        }
      }

      /// <summary>
      ///   Resets the asynchronous prune process for another use
      /// </summary>
      /// <param name="pruneDelegate">Method deciding which particles to prune</param>
      /// <param name="callback">
      ///   Callback that will be invoked when pruning has finished
      /// </param>
      /// <param name="state">User-defined state from the BeginPrune() method</param>
      public void Start(
        PrunePredicate pruneDelegate, AsyncCallback callback, object state
      ) {
        this.running = true;
        lock(this) {
          this.completed = false;

          if(this.doneEvent != null) {
            this.doneEvent.Reset();
          }
        }
        this.exception = null;

        this.pruneDelegate = pruneDelegate;
        this.callback = callback;
        this.state = state;

        ThreadPool.QueueUserWorkItem(this.runDelegate);
      }

      /// <summary>User defined state from the BeginPrune() method</summary>
      public object AsyncState { get { return this.state; } }

      /// <summary>Exception that occured during asynchronous processing</summary>
      public Exception AsyncException { get { return this.exception; } }

      /// <summary>
      ///   Wait handle that can be used to wait until pruning is finished
      /// </summary>
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

      /// <summary>Whether the pruning process has finished synchronously</summary>
      public bool CompletedSynchronously { get { return false; } }

      /// <summary>True if pruning has finished</summary>
      public bool IsCompleted { get { return this.completed; } }

      /// <summary>Whether the pruning process is currently running</summary>
      public bool IsRunning { get { return this.running; } }

      /// <summary>Executes the asynchronous pruning</summary>
      /// <param name="state">Not used</param>
      private void run(object state) {
        try {
          this.particleSystem.Prune(this.pruneDelegate);
        }
        catch(Exception exception) {
          this.exception = exception;
        }

        this.running = false;
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

      /// <summary>Particle system being pruned</summary>
      private ParticleSystem<ParticleType> particleSystem;
      /// <summary>Delegate for a method that decides which particles are pruned</summary>
      private PrunePredicate pruneDelegate;
      /// <summary>Callback that will be invoked when pruning has finished</summary>
      private AsyncCallback callback;
      /// <summary>Used-defined state from the BeginPrune() method</summary>
      private object state;
      /// <summary>Delegate to the run() method which performs the pruning</summary>
      private WaitCallback runDelegate;
      /// <summary>Wait handle that can be used to wait for pruning to finish</summary>
      private volatile ManualResetEvent doneEvent;
      /// <summary>Whether the pruning process is finished</summary>
      private volatile bool completed;
      /// <summary>Whether the pruning process is running</summary>
      private volatile bool running;
      /// <summary>Exception that occured during asynchronous processing</summary>
      private volatile Exception exception;

    }

    #endregion // class AsyncPrune

    #region class AsyncUpdate

    /// <summary>Updates the particle system asynchronously</summary>
    private class AsyncUpdate : IAsyncResult, IDisposable {

      #region class ThreadStartInfo

      /// <summary>Start informations for an update thread</summary>
      private class ThreadStartInfo {
        /// <summary>Index of the first particle that will be updated</summary>
        public int Start;
        /// <summary>Number of particles that will be updated</summary>
        public int Count;
      }

      #endregion // class ThreadStartInfo

      /// <summary>Initializes a new asynchronous update process</summary>
      /// <param name="particleSystem">Particle system that will be updated</param>
      public AsyncUpdate(ParticleSystem<ParticleType> particleSystem) {
        this.particleSystem = particleSystem;

        this.threadStartInfos = new List<ThreadStartInfo>();
        this.runDelegate = new WaitCallback(run);
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.doneEvent != null) {
          this.doneEvent.Close();
          this.doneEvent = null;
        }
      }

      /// <summary>
      ///   Resets the asynchronous update process for another use
      /// </summary>
      /// <param name="updates">Number of updates that will be performed</param>
      /// <param name="threads">Number of threads to use for the updates</param>
      /// <param name="callback">
      ///   Callback that will be invoked when pruning has finished
      /// </param>
      /// <param name="state">User-defined state from the BeginUpdate() method</param>
      public void Start(int updates, int threads, AsyncCallback callback, object state) {
        this.running = true;
        Interlocked.Exchange(ref this.completedUpdates, 0);
        this.completed = false;
        if(this.doneEvent != null) {
          this.doneEvent.Reset();
        }
        this.exception = null;

        this.updates = updates;
        this.threads = threads;
        this.callback = callback;
        this.state = state;

        if(threads == 0) {
          run(null);
        } else {
          // Make sure enough thread start infos are available
          while(this.threadStartInfos.Count < threads) {
            this.threadStartInfos.Add(new ThreadStartInfo());
          }
          int lastThread = threads - 1;

          // Create all threads except for the last one (because the particle count
          // might not be evenly dividable by the number of threads)
          int particlesPerThread = this.particleSystem.particleCount / threads;
          for(int thread = 0; thread < lastThread; ++thread) {
            this.threadStartInfos[thread].Start = particlesPerThread * thread;
            this.threadStartInfos[thread].Count = particlesPerThread;
            runThreaded(this.runDelegate, this.threadStartInfos[thread]);
          }

          // Let the final thread process all remaining particles. For low numbers of
          // particles, the workload might be distributed unevenly (eg. 19 particles
          // on 10 threads = 10th thread does 9 particles), but with the huge numbers
          // of particles typically simulated, it's not relevant whether the 10th thread
          // does 5009 particles instead of 5000 like the others)
          int finalStart = particlesPerThread * lastThread;
          int finalCount = this.particleSystem.particleCount - finalStart;
          this.threadStartInfos[lastThread].Start = finalStart;
          this.threadStartInfos[lastThread].Count = finalCount;
          runThreaded(this.runDelegate, this.threadStartInfos[lastThread]);
        }
      }

      /// <summary>User defined state from the BeginUpdate() method</summary>
      public object AsyncState { get { return this.state; } }

      /// <summary>Exception that occured during asynchronous processing</summary>
      public Exception AsyncException { get { return this.exception; } }

      /// <summary>
      ///   Wait handle that can be used to wait until updating is finished
      /// </summary>
      public System.Threading.WaitHandle AsyncWaitHandle {
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

      /// <summary>Whether the update process has finished synchronously</summary>
      public bool CompletedSynchronously { get { return false; } }

      /// <summary>True if updating has finished</summary>
      public bool IsCompleted { get { return this.completed; } }

      /// <summary>Whether the update process is currently running</summary>
      public bool IsRunning { get { return this.running; } }

      /// <summary>Executes the asynchronous pruning</summary>
      /// <param name="state">Not used</param>
      private void run(object state) {
        if(state != null) {
          ThreadStartInfo startInfo = state as ThreadStartInfo;
          try {
            this.particleSystem.update(this.updates, startInfo.Start, startInfo.Count);
          }
          catch(Exception exception) {
            this.exception = exception;
          }
        }

        // If we're the final thread coming around, set the async result to finished
        int completedUpdates = Interlocked.Increment(ref this.completedUpdates);
        if(completedUpdates >= this.threads) {
          this.running = false;
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
      }

      /// <summary>Runs the provided wait callback in a separate thread</summary>
      /// <param name="waitCallback">Wait callback that will be run in a thread</param>
      /// <param name="state">
      ///   User defined state that will be passed on to the wait callback
      /// </param>
      private static void runThreaded(WaitCallback waitCallback, object state) {
        AffineThreadPool.QueueUserWorkItem(waitCallback, state);
      }

      /// <summary>Particle system being pruned</summary>
      private ParticleSystem<ParticleType> particleSystem;
      /// <summary>Number of updates that have been completed</summary>
      private int completedUpdates;
      /// <summary>Number of updates that will be performed</summary>
      private int updates;
      /// <summary>Number of threads being used to perform the update</summary>
      private int threads;
      /// <summary>Callback that will be invoked when pruning has finished</summary>
      private AsyncCallback callback;
      /// <summary>Used-defined state from the BeginPrune() method</summary>
      private object state;
      /// <summary>Delegate to the run() method which performs the pruning</summary>
      private WaitCallback runDelegate;
      /// <summary>Wait handle that can be used to wait for pruning to finish</summary>
      private volatile ManualResetEvent doneEvent;
      /// <summary>Whether the pruning process is finished</summary>
      private volatile bool completed;
      /// <summary>Whether the pruning process is running</summary>
      private volatile bool running;
      /// <summary>Start information holders for the threads</summary>
      private List<ThreadStartInfo> threadStartInfos;
      /// <summary>Exception that occured during asynchronous processing</summary>
      private volatile Exception exception;

    }

    #endregion // class AsyncUpdate

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
