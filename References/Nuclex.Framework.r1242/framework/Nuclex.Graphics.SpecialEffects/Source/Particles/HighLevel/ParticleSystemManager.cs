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

  /// <summary>Manages multiple particles systems and renders them</summary>
  /// <remarks>
  ///   <para>
  ///     The particle system manager allows you to offload pruning, updating and
  ///     rendering of your particle systems to a central place. You still have to
  ///     keep a reference to your particle systems to add particles or modify
  ///     affectors, but you no longer have to worry about manually updating,
  ///     pruning and drawing them yourself.
  ///   </para>
  ///   <para>
  ///     As an added benefit, multiple particle systems using the same vertex type
  ///     will be rendered in combined batches, allowing for efficient rendering
  ///     even when your particle systems are only sparsely populated.
  ///   </para>
  ///   <para>
  ///     The recommended usage is to create a particle system management service
  ///     for your game where particle systems can be registered to this component
  ///     (if you want to construct and destroy particle systems on the fly).
  ///     Another option would be to let the service provide a fixed set of
  ///     particle systems via properties so that particles can easily be added to
  ///     them from your game entities, depending on your design approach.
  ///   </para>
  /// </remarks>
  public partial class ParticleSystemManager : Drawable, IParticleSystemService {

    /// <summary>Initializes a new particle system manager</summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service being used to render the particle systems
    /// </param>
    public ParticleSystemManager(IGraphicsDeviceService graphicsDeviceService) :
      base(graphicsDeviceService) {

      this.particleSystems = new Dictionary<object, IParticleSystemHolder>();
      this.primitiveBatches = new Dictionary<Type, PrimitiveBatchHolder>();

      this.pruneAsyncResult = new PruneAsyncResult(this);
      this.updateAsyncResult = new UpdateAsyncResult(this);

      this.particleSystemHolders = new IParticleSystemHolder[16];
      this.primitiveBatchHolders = new PrimitiveBatchHolder[4];

      this.holderArraysDirty = true;
    }

    /// <summary>Immediately releases all resources used by the instance</summary>
    public override void Dispose() {
      updateHolderArraysIfNeeded();

      if(this.particleSystems.Count > 0) {
        for(int index = this.particleSystems.Count - 1; index >= 0; --index) {
          this.particleSystemHolders[index].Dispose();
          this.particleSystemHolders[index].ReleasePrimitiveBatch();
        }
        this.particleSystems.Clear();
      }

      if(this.primitiveBatches.Count > 0) {
        for(int index = this.primitiveBatches.Count - 1; index >= 0; --index) {
          this.primitiveBatchHolders[index].Release();
        }
        this.primitiveBatches.Clear();
      }

      if(this.pruneAsyncResult != null) {
        this.pruneAsyncResult.Dispose();
        this.pruneAsyncResult = null;
      }

      if(this.updateAsyncResult != null) {
        this.updateAsyncResult.Dispose();
        this.updateAsyncResult = null;
      }

      base.Dispose();
    }

    /// <summary>Draws the particle systems</summary>
    /// <param name="gameTime">Snapshot of the game's timing values</param>
    public override void Draw(GameTime gameTime) {
      updateHolderArraysIfNeeded();

      // If an exception occurs somewhere in Begin(), we want to try our best
      // to return into the previous state.
      int initIndex = 0;
      try {

        // Call Begin() on all primitive batches we have
        for(; initIndex < this.primitiveBatches.Count; ++initIndex) {
          this.primitiveBatchHolders[initIndex].Begin();
        }

        // Render the particle systems
        for(int index = 0; index < this.particleSystems.Count; ++index) {
          this.particleSystemHolders[index].Render();
        }

      }
      finally {
        // Call End() on all primitive batches that sucessfully had their Begin()
        // method called so far (normally all, unless an exception occured inside
        // the Begin() loop)
        for(--initIndex; initIndex >= 0; --initIndex) {
          this.primitiveBatchHolders[initIndex].End();
        }
      }
    }

    /// <summary>Prunes dead particles from all particle systems</summary>
    public void Prune() {
      updateHolderArraysIfNeeded();

      for(int index = 0; index < this.particleSystems.Count; ++index) {
        this.particleSystemHolders[index].Prune();
      }
    }

    /// <summary>Updates the particle systems</summary>
    /// <param name="updates">Number of updates that will be performed</param>
    public void Update(int updates) {
      updateHolderArraysIfNeeded();

      for(int index = 0; index < this.particleSystems.Count; ++index) {
        this.particleSystemHolders[index].Update(updates);
      }
    }

    /// <summary>Begins an asynchronous update on the particle system</summary>
    /// <param name="updates">Number of updates that will be performed</param>
    /// <param name="threads">Number of threads that will be used</param>
    /// <param name="callback">
    ///   Callback that will be invoked after the update has finished
    /// </param>
    /// <param name="state">User-defined state</param>
    /// <returns>
    ///   An asynchronous result handle that can be used to wait for the operation
    /// </returns>
    public IAsyncResult BeginUpdate(
      int updates, int threads, AsyncCallback callback, object state
    ) {
      updateHolderArraysIfNeeded();

      this.updateAsyncResult.Start(updates, threads, callback, state);
      return this.updateAsyncResult;
    }

    /// <summary>Ends the asynchronous update</summary>
    /// <param name="asyncResult">
    ///   Asynchronous result handle obtained from the BeginUpdate() method
    /// </param>
    public void EndUpdate(IAsyncResult asyncResult) {
      if(!ReferenceEquals(asyncResult, this.updateAsyncResult)) {
        throw new ArgumentException("Wrong async result specified", "asyncResult");
      }

      if(!this.updateAsyncResult.IsCompleted) {
        this.updateAsyncResult.AsyncWaitHandle.WaitOne();
      }

      if(this.updateAsyncResult.AsyncException != null) {
        throw this.updateAsyncResult.AsyncException;
      }
    }

    /// <summary>Begins asynchronously pruning dead particles from the system</summary>
    /// <param name="callback">
    ///   Callback that will be invoked after pruning has finished
    /// </param>
    /// <param name="state">User-defined state</param>
    /// <returns>
    ///   An asynchronous result handle that can be used to wait for the operation
    /// </returns>
    public IAsyncResult BeginPrune(AsyncCallback callback, object state) {
      updateHolderArraysIfNeeded();

      this.pruneAsyncResult.Start(callback, state);
      return this.pruneAsyncResult;
    }

    /// <summary>Ends asynchronous pruning</summary>
    /// <param name="asyncResult">
    ///   Asynchronous result handle obtained from the BeginPrune() method
    /// </param>
    public void EndPrune(IAsyncResult asyncResult) {
      if(!ReferenceEquals(asyncResult, this.pruneAsyncResult)) {
        throw new ArgumentException("Wrong async result specified", "asyncResult");
      }

      if(!this.pruneAsyncResult.IsCompleted) {
        this.pruneAsyncResult.AsyncWaitHandle.WaitOne();
      }

      if(this.pruneAsyncResult.AsyncException != null) {
        throw this.pruneAsyncResult.AsyncException;
      }
    }

    /// <summary>
    ///   Updates the particle system and primitive batch holder arrays if needed
    /// </summary>
    private void updateHolderArraysIfNeeded() {
      if(this.holderArraysDirty) {
        if(this.particleSystemHolders.Length < this.particleSystems.Count) {
          this.particleSystemHolders = new IParticleSystemHolder[
            IntegerHelper.NextPowerOf2(this.particleSystems.Count)
          ];
        }
        if(this.primitiveBatchHolders.Length < this.primitiveBatches.Count) {
          this.primitiveBatchHolders = new PrimitiveBatchHolder[
            IntegerHelper.NextPowerOf2(this.primitiveBatches.Count)
          ];
        }

        this.primitiveBatches.Values.CopyTo(this.primitiveBatchHolders, 0);
        this.particleSystems.Values.CopyTo(this.particleSystemHolders, 0);

        this.holderArraysDirty = false;
      }
    }

    /// <summary>
    ///   Whether the arrays for the primitive batch and particle system holders
    ///   need to be updated
    /// </summary>
    private bool holderArraysDirty;

    /// <summary>Stores the primitive batch holders from the dictionary</summary>
    private PrimitiveBatchHolder[] primitiveBatchHolders;
    /// <summary>Stores the particle system holders from the dictionary</summary>
    private IParticleSystemHolder[] particleSystemHolders;

    /// <summary>Used to asynchronously prune the particle systems</summary>
    private PruneAsyncResult pruneAsyncResult;
    /// <summary>Used to asynchronously update the particle systems</summary>
    private UpdateAsyncResult updateAsyncResult;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel
