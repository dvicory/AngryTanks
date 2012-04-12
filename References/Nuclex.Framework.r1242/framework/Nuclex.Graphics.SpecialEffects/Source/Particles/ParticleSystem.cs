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

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Manages a series of simulated particles</summary>
  /// <typeparam name="ParticleType">Type of the particles being simulated</typeparam>
  /// <remarks>
  ///   <para>
  ///     A particle system comprises of the particles being simulated and their
  ///     affectors, which influence the behavior of the particles. Affectors can
  ///     simulate gravity, wind, decay and just about any physical behavior your
  ///     particles my display.
  ///   </para>
  ///   <para>
  ///     The design is optimized for a multi-threading scenario: affactors only
  ///     update particles and never delete or add particles, so multiple affectors
  ///     can be run in parallel or one affector can be run from multiple threads
  ///     on different segments of the particle array
  ///   </para>
  ///   <para>
  ///     Pruning (the process of removing dead particles) can be performed
  ///     post-update in a single, controlled step which could run in the background
  ///     while the game engine is doing other things.
  ///   </para>
  /// </remarks>
  public partial class ParticleSystem<ParticleType> : IDisposable
    where ParticleType : struct {

    /// <summary>Delegate used for detecting dead particles to prune</summary>
    /// <param name="particle">Particle that should be checked</param>
    /// <returns>True if the particle should be kept in the list</returns>
    public delegate bool PrunePredicate(ref ParticleType particle);

    /// <summary>Initializes a new particle system</summary>
    /// <param name="maximumParticles">
    ///   Maximum number of particles the system can support
    /// </param>
    public ParticleSystem(int maximumParticles) {
      this.particles = new ParticleType[maximumParticles];

      this.affectors = new AffectorCollection<ParticleType>();
      this.coalescableAffectors = this.affectors.CoalescableAffectors;
      this.noncoalescableAffectors = this.affectors.NoncoalescableAffectors;

      this.asyncPrune = new AsyncPrune(this);
      this.asyncUpdate = new AsyncUpdate(this);
    }

    /// <summary>
    ///   Immediately releases all resources owned by the particle system
    /// </summary>
    public void Dispose() {
      if(this.asyncUpdate != null) {
        this.asyncUpdate.Dispose();
        this.asyncUpdate = null;
      }
      if(this.asyncPrune != null) {
        this.asyncPrune.Dispose();
        this.asyncPrune = null;
      }
      
      this.particles = null;
      this.coalescableAffectors = null;
      this.noncoalescableAffectors = null;
    }

    /// <summary>Adds a new particle to the particle system</summary>
    /// <param name="particle">Particle that will be added to the system</param>
    /// <remarks>
    ///   If the particle system is full, the added particle will be silently discarded.
    /// </remarks>
    public void AddParticle(ParticleType particle) {
      if(this.particleCount < this.particles.Length) {
        this.particles[this.particleCount] = particle;
        ++this.particleCount;
      }
    }

    /// <summary>Removes a particle from the particle system</summary>
    /// <param name="index">Index of the particle that will be removed</param>
    public void RemoveParticle(int index) {
      this.particles[index] = this.particles[this.particleCount - 1];
      --this.particleCount;
    }

    /// <summary>Particles being simulated by the particle system</summary>
    public ArraySegment<ParticleType> Particles {
      get {
        return new ArraySegment<ParticleType>(this.particles, 0, this.particleCount);
      }
    }

    /// <summary>Runs the specified number of updates on the particle system</summary>
    /// <param name="updates">Number of updates that will be run</param>
    public void Update(int updates) {
      update(updates, 0, this.particleCount);
    }

    /// <summary>Number of particles the particle system can manage</summary>
    public int Capacity { get { return this.particles.Length; } }

    /// <summary>Begins a multi-threaded update of the particles</summary>
    /// <param name="updates">
    ///   Number of updates to perform. A single update will take full advantage
    ///   of multiple threads as well.
    /// </param>
    /// <param name="threads">Number of threads to perform the updates in</param>
    /// <param name="callback">
    ///   Callback that will be invoked when the update has finished
    /// </param>
    /// <param name="state">
    ///   User defined parameter that will be passed to the callback
    /// </param>
    /// <returns>An asynchronous result handle for the background operation</returns>
    public IAsyncResult BeginUpdate(
      int updates, int threads, AsyncCallback callback, object state
    ) {
      Debug.Assert(
        !this.asyncUpdate.IsRunning, "An asynchronous update is already running"
      );
      Debug.Assert(
        !this.asyncPrune.IsRunning, "Can't update while an asynchronous prune is running"
      );

      this.asyncUpdate.Start(
        updates, Math.Min(threads, this.particleCount), callback, state
      );
      return this.asyncUpdate;
    }

    /// <summary>Ends a multi-threaded particle system update</summary>
    /// <param name="asyncResult">
    ///   Asynchronous result handle returned by the BeginUpdate() method
    /// </param>
    public void EndUpdate(IAsyncResult asyncResult) {
      if(!ReferenceEquals(asyncResult, this.asyncUpdate)) {
        throw new ArgumentException(
          "Async result does not belong to the particle system", "asyncResult"
        );
      }

      if(!this.asyncUpdate.IsCompleted) {
        this.asyncUpdate.AsyncWaitHandle.WaitOne();
      }

      if(this.asyncUpdate.AsyncException != null) {
        throw this.asyncUpdate.AsyncException;
      }
    }

    /// <summary>Prunes dead particles from the system</summary>
    /// <param name="pruneDelegate">
    ///   Delegate deciding which particles will be pruned
    /// </param>
    public void Prune(PrunePredicate pruneDelegate) {
      for (int index = 0; index < this.particleCount; ) {
        bool keep = pruneDelegate(ref this.particles[index]);
        if (keep) {
          ++index;
        } else {
          this.particles[index] = this.particles[this.particleCount - 1];
          --this.particleCount;
        }
      }
    }

    /// <summary>Begins a threaded prune of the particles</summary>
    /// <param name="pruneDelegate">
    ///   Method that evaluates whether a particle should be pruned from the system
    /// </param>
    /// <param name="callback">
    ///   Callback that will be invoked when the update has finished
    /// </param>
    /// <param name="state">
    ///   User defined parameter that will be passed to the callback
    /// </param>
    /// <returns>An asynchronous result handle for the background operation</returns>
    public IAsyncResult BeginPrune(
      PrunePredicate pruneDelegate, AsyncCallback callback, object state
    ) {
      Debug.Assert(
        !this.asyncPrune.IsRunning, "An asynchronous prune is already running"
      );
      Debug.Assert(
       !this.asyncUpdate.IsRunning, "Can't prune while an asynchronous update is running"
      );

      this.asyncPrune.Start(pruneDelegate, callback, state);
      return this.asyncPrune;
    }

    /// <summary>Ends a multi-threaded prune of the particle system</summary>
    /// <param name="asyncResult">
    ///   Asynchronous result handle returned by the BeginPrune() method
    /// </param>
    public void EndPrune(IAsyncResult asyncResult) {
      if(!ReferenceEquals(asyncResult, this.asyncPrune)) {
        throw new ArgumentException(
          "Async result does not belong to the particle system", "asyncResult"
        );
      }

      if(!this.asyncPrune.IsCompleted) {
        this.asyncPrune.AsyncWaitHandle.WaitOne();
      }

      if(this.asyncPrune.AsyncException != null) {
        throw this.asyncPrune.AsyncException;
      }
    }

    /// <summary>Affectors that are influencing the particles in this system</summary>
    public AffectorCollection<ParticleType> Affectors {
      get { return this.affectors; }
    }

    /// <summary>Runs the specified number of updates on the particle system</summary>
    /// <param name="updates">Number of updates that will be run</param>
    /// <param name="start">Particle index at which updating will begin</param>
    /// <param name="count">Number of particles that will be updated</param>
    private void update(int updates, int start, int count) {

      // Coalescable affectors can be executed in one go per affector because they
      // will not produce side effects on each other or the non-coalescable ones.
      for(int index = 0; index < this.coalescableAffectors.Count; ++index) {
        this.coalescableAffectors[index].Affect(
          this.particles, start, count, updates
        );
      }

      // Process any non-coalescable affectors one after another
      if(this.noncoalescableAffectors.Count > 1) { // multiple non-coalescables?

        // Run all non-coalescable affectors in succession for the number of updates
        // we were instructed to perform. This stepped approach is neccessary to
        // ensure that the outcome of any cross-affector effects does not change.
        for(int update = 0; update < updates; ++update) {
          for(int index = 0; index < this.noncoalescableAffectors.Count; ++index) {
            this.noncoalescableAffectors[index].Affect(
              this.particles, start, count, 1
            );
          }
        }

      } else if(this.noncoalescableAffectors.Count == 1) { // only one non-coalescable

        // Since there's only one non-coalescable affector, we can let it run
        // all the updates in a single step
        this.noncoalescableAffectors[0].Affect(
          this.particles, start, count, updates
        );

      }

    }

    /// <summary>Stores the particles simulated by the system</summary>
    private ParticleType[] particles;
    /// <summary>Number of particles currently stored in the particle array</summary>
    private int particleCount;

    /// <summary>Affectors registered to the particle system</summary>
    private AffectorCollection<ParticleType> affectors;
    /// <summary>Affectors that are coalescable into a single update</summary>
    private List<IParticleAffector<ParticleType>> coalescableAffectors;
    /// <summary>Affectors that are not coalescable into a single update</summary>
    private List<IParticleAffector<ParticleType>> noncoalescableAffectors;

    /// <summary>Manages the asynchronous pruning process</summary>
    private AsyncPrune asyncPrune;
    /// <summary>Manages the asynchronous updating process</summary>
    private AsyncUpdate asyncUpdate;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
