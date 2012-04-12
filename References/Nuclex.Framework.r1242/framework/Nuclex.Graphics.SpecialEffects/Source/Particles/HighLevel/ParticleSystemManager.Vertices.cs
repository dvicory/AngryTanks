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

using Microsoft.Xna.Framework.Graphics;

using Nuclex.Graphics.Batching;
using Nuclex.Support;

namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel {

  partial class ParticleSystemManager {

    #region class PrimitiveBatchHolder

    /// <summary>Holds a primitive batch used to render particles</summary>
    private abstract class PrimitiveBatchHolder {

#if XNA_4

      /// <summary>Initializes a new primitive batch holder</summary>
      /// <param name="manager">Particle system manager the holder belongs to</param>
      public PrimitiveBatchHolder(ParticleSystemManager manager) {
        this.Manager = manager;
        this.ReferenceCount = 1;
      }

#else

      /// <summary>Initializes a new primitive batch holder</summary>
      /// <param name="manager">Particle system manager the holder belongs to</param>
      /// <param name="ownedVertexDeclaration">
      ///   Vertex declaration if the particle system manager took ownership of it
      /// </param>
      public PrimitiveBatchHolder(
        ParticleSystemManager manager, VertexDeclaration ownedVertexDeclaration
      ) {
        this.Manager = manager;
        this.ReferenceCount = 1;
        this.OwnedVertexDeclaration = ownedVertexDeclaration;
      }

#endif

      /// <summary>Begins drawing with the contained primitive batch</summary>
      public abstract void Begin();
      /// <summary>Ends drawing with the contained primitve batch</summary>
      public abstract void End();

      /// <summary>Releases or destroys the referenced primitive batch</summary>
      public abstract void Release();

      /// <summary>Particle system manager this holder belongs to</summary>
      protected ParticleSystemManager Manager;
      /// <summary>Number of active references to the primitive batch holder</summary>
      public int ReferenceCount;
#if !XNA_4
      /// <summary>
      ///   Vertex declaration to be disposed together with the primitive batch
      /// </summary>
      /// <remarks>
      ///   Only set if we have ownership of his vertex declaration
      /// </remarks>
      public VertexDeclaration OwnedVertexDeclaration;
      /// <summary>
      ///   Whether this vertex type has been explicitely registered to the particle
      ///   system manager
      /// </summary>
      /// <remarks>
      ///   Can change during the lifespan of the instance if the user late-registers
      ///   or early-unregisters his type from the particle system manager.
      /// </remarks>
      public bool ExplicitlyRegistered;
#endif
    }

    #endregion // class PrimitiveBatchHolder

    #region class PrimitiveBatchHolder<>

    /// <summary>Holds a type-safe primitive batch used to render particles</summary>
    private class PrimitiveBatchHolder<VertexType> : PrimitiveBatchHolder
      where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {

#if XNA_4

      /// <summary>Initializes a new primitive batch holder</summary>
      /// <param name="manager">
      ///   Particle system manager the primitive batch belongs to
      /// </param>
      /// <param name="primitiveBatch">Primitive batch being held</param>
      public PrimitiveBatchHolder(
        ParticleSystemManager manager,
        PrimitiveBatch<VertexType> primitiveBatch
      ) :
        base(manager) {
        this.PrimitiveBatch = primitiveBatch;
      }

#else

      /// <summary>Initializes a new primitive batch holder</summary>
      /// <param name="manager">
      ///   Particle system manager the primitive batch belongs to
      /// </param>
      /// <param name="primitiveBatch">Primitive batch being held</param>
      /// <param name="ownedVertexDeclaration">
      ///   Vertex declaration if the particle system manager took ownership of it
      /// </param>
      public PrimitiveBatchHolder(
        ParticleSystemManager manager,
        PrimitiveBatch<VertexType> primitiveBatch,
        VertexDeclaration ownedVertexDeclaration
      ) :
        base(manager, ownedVertexDeclaration) {
        this.PrimitiveBatch = primitiveBatch;
      }

#endif

      /// <summary>Begins drawing with the contained primitive batch</summary>
      public override void Begin() {
        this.PrimitiveBatch.Begin(QueueingStrategy.Deferred);
      }

      /// <summary>Ends drawing with the contained primitive batch</summary>
      public override void End() {
        this.PrimitiveBatch.End();
      }

      /// <summary>Releases or destroys the referenced primitive batch</summary>
      public override void Release() {
        if(this.ReferenceCount > 1) {
          --this.ReferenceCount;
        } else {
          this.PrimitiveBatch.Dispose();
#if !XNA_4
          if(this.OwnedVertexDeclaration != null) {
            this.OwnedVertexDeclaration.Dispose();
            this.OwnedVertexDeclaration = null;
          }
#endif
          base.Manager.primitiveBatches.Remove(typeof(VertexType));
        }
      }

      /// <summary>Primitive batch for the holder's vertex type</summary>
      public PrimitiveBatch<VertexType> PrimitiveBatch;

    }

    #endregion // class PrimitiveBatchHolder<>

    /// <summary>Delegate for creating a new primitive batch holder</summary>
    internal delegate void InduceErrorDelegate();

#if !XNA_4

    /// <summary>Registers the specified vertex type for particle systems</summary>
    /// <typeparam name="VertexType">Type of vertex that will be registered</typeparam>
    /// <param name="vertexDeclaration">
    ///   Vertex declaration describing the vertex structure to the graphics device
    /// </param>
    /// <param name="stride">Offset, in bytes, from one vertex to the next</param>
    /// <remarks>
    ///   <para>
    ///     Registering a vertex type is only required when the vertex declaration can not
    ///     be automatically extracted from the vertex structure (eg. you didn't use
    ///     VertexElementAttribute to describe your fields).
    ///   </para>
    ///   <para>
    ///     If the vertex type is used by the particle system manager throughout its
    ///     lifespan, it's okay to not call unregister and rely on Dispose() doing
    ///     the clean-up work. Take note that in this variant, the particle system
    ///     manager doesn't take ownership of the vertex declaration; you'll have to
    ///     destroy it yourself after the particle system manager is destroyed.
    ///   </para>
    /// </remarks>
    public void RegisterVertex<VertexType>(
      VertexDeclaration vertexDeclaration, int stride
    ) where VertexType : struct {
      RegisterVertex<VertexType>(vertexDeclaration, stride, false);
    }

    /// <summary>Registers the specified vertex type for particle systems</summary>
    /// <typeparam name="VertexType">Type of vertex that will be registered</typeparam>
    /// <param name="vertexDeclaration">
    ///   Vertex declaration describing the vertex structure to the graphics device
    /// </param>
    /// <param name="stride">Offset, in bytes, from one vertex to the next</param>
    /// <param name="takeOwnership">
    ///   Whether the particle system manager takes ownership of the vertex declaration
    ///   and will destroy it upon shutdown
    /// </param>
    /// <remarks>
    ///   <para>
    ///     Registering a vertex type is only required when the vertex declaration can not
    ///     be automatically extracted from the vertex structure (eg. you didn't use
    ///     VertexElementAttribute to describe your fields).
    ///   </para>
    ///   <para>
    ///     If the vertex type is used by the particle system manager throughout its
    ///     lifespan, it's okay to not call unregister and rely on Dispose() doing
    ///     the clean-up work.
    ///   </para>
    /// </remarks>
    public void RegisterVertex<VertexType>(
      VertexDeclaration vertexDeclaration, int stride, bool takeOwnership
    ) where VertexType : struct {

      // Check whether a vertex declaration and primitive batch already exist for
      // this vertex type. If so, turn it into an explicitly registered one.
      PrimitiveBatchHolder holder;
      if(this.primitiveBatches.TryGetValue(typeof(VertexType), out holder)) {

        // If this was set by explicit registration, complain
        if(holder.ExplicitlyRegistered) {
          throw new ArgumentException("Vertex type is already registered", "VertexType");
        }
        holder.ExplicitlyRegistered = true;

        // If this point is reached, it's clear that we have ownership of
        // the old vertex declaration and need to destroy it.
        holder.OwnedVertexDeclaration.Dispose();
        holder.OwnedVertexDeclaration = takeOwnership ? vertexDeclaration : null;

        // Make sure this primitive batch will be kept alive even if no particle systems
        // use it any longer. Registration counts as one reference to prevent us from
        // falling back to an autogenerated vertex declaration when the particle system
        // using it is removed from the manager and then re-added.
        ++holder.ReferenceCount;

      } else { // No vertex declaration and primitive batch for this vertex type yet

        holder = new PrimitiveBatchHolder<VertexType>(
          this,
          new PrimitiveBatch<VertexType>(GraphicsDevice, vertexDeclaration, stride),
          takeOwnership ? vertexDeclaration : null
        );
        holder.ExplicitlyRegistered = true;

        // Create a new primitive batch and take over the vertex declaration
        this.primitiveBatches.Add(typeof(VertexType), holder);
        this.holderArraysDirty = true;

      }

    }

    /// <summary>Unregisters the specified vertex type for particle systems</summary>
    /// <typeparam name="VertexType">Vertex type that will be unregistered</typeparam>
    public void UnregisterVertex<VertexType>() where VertexType : struct {

      // Try to find the vertex declaration and primitive batch the user has registered
      PrimitiveBatchHolder holder;
      bool found = this.primitiveBatches.TryGetValue(typeof(VertexType), out holder);

      // If the vertex declaration doesn't exist or was not explicit registered, complain
      if(!found || !holder.ExplicitlyRegistered) {
        throw new ArgumentException("Vertex type is not registered", "VertexType");
      }

      PrimitiveBatchHolder<VertexType> typeSafeHolder;
      typeSafeHolder = (holder as PrimitiveBatchHolder<VertexType>);

      // Get rid of the primitive batch since its vertex declaration is no longer valid
      typeSafeHolder.PrimitiveBatch.Dispose();

      // If the user transferred ownership of the vertex declaration to us, destroy it
      if(holder.OwnedVertexDeclaration != null) {
        holder.OwnedVertexDeclaration.Dispose();
      }

      // If there are still particle systems referencing this primitive batch, turn
      // it into an implicitly-created one
      if(holder.ReferenceCount > 1) {
        --holder.ReferenceCount;

        holder.ExplicitlyRegistered = false;

        // Try to automatically determine the vertex declaration we're going to use
        // from now on. This might fail if the user didn't prepare his structure.
        // If so, the particle syste manager becomes unusable - we can't just remove
        // the primitive batch holder altogether because it is still referenced by
        // the particle system holders.
        VertexDeclaration vertexDeclaration;
        int stride;
        autoCreateVertexDeclaration<VertexType>(out vertexDeclaration, out stride);

        typeSafeHolder.PrimitiveBatch = new PrimitiveBatch<VertexType>(
          GraphicsDevice, vertexDeclaration, stride
        );

      } else { // No more particle systems referencing this - destroy it
        this.primitiveBatches.Remove(typeof(VertexType));
        this.holderArraysDirty = true;
      }

    }

#endif

    /// <summary>
    ///   Retrieves or creates the primitive batch for the specified vertex type
    /// </summary>
    /// <typeparam name="VertexType">
    ///   Vertex type a primitive batch will be returned for
    /// </typeparam>
    /// <returns>A primitive batch that renders the specified vertex type</returns>
    private PrimitiveBatchHolder<VertexType> getOrCreatePrimitiveBatch<VertexType>()
      where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {
      PrimitiveBatchHolder holder;

      // Find out if we a primitive batch and vertex declaration already exist
      if(this.primitiveBatches.TryGetValue(typeof(VertexType), out holder)) {

        // Yes, add another reference to this primitive batch and vertex declaration
        ++holder.ReferenceCount;
        return (holder as PrimitiveBatchHolder<VertexType>);

      } else { // No, let's create one

        // Create the primitive batch for rendering this vertex structure
        PrimitiveBatchHolder<VertexType> newHolder;

#if !XNA_4
        // The user has not explicitely registered this vertex type, so we'll try
        // our best to automatically generate a vertex declaration for it
        VertexDeclaration vertexDeclaration;
        int stride;
        autoCreateVertexDeclaration<VertexType>(out vertexDeclaration, out stride);
        try {
#endif
#if UNITTEST
        if(this.InducePrimitiveBatchErrorDelegate != null) {
          this.InducePrimitiveBatchErrorDelegate();
        }
#endif
        newHolder = new PrimitiveBatchHolder<VertexType>(
          this,
#if XNA_4
 new PrimitiveBatch<VertexType>(GraphicsDevice)
#else
            new PrimitiveBatch<VertexType>(GraphicsDevice, vertexDeclaration, stride),
            vertexDeclaration
#endif
);
        this.primitiveBatches.Add(typeof(VertexType), newHolder);
        this.holderArraysDirty = true;
#if !XNA_4
        }
        catch(Exception) {
          vertexDeclaration.Dispose();
          throw;
        }
#endif
        return newHolder;

      }
    }

#if !XNA_4
    /// <summary>
    ///   Automatically creates the vertex declaration for the specified type
    /// </summary>
    /// <typeparam name="VertexType">
    ///   Vertex type for which the declaration will be built
    /// </typeparam>
    /// <param name="vertexDeclaration">
    ///   Output parameter that receives the automatically generated vertex declaration
    /// </param>
    /// <param name="stride">
    ///   Output parameter that resizes the size of a particle vertex in bytes
    /// </param>
    private void autoCreateVertexDeclaration<VertexType>(
      out VertexDeclaration vertexDeclaration, out int stride
    ) where VertexType : struct {

      // In reverse order so we don't loose the pointer if an exception happens
      // inside GetStride()
      stride = VertexDeclarationHelper.GetStride<VertexType>();
      vertexDeclaration = new VertexDeclaration(
        GraphicsDevice, VertexDeclarationHelper.BuildElementList<VertexType>()
      );

    }
#endif

#if UNITTEST
    /// <summary>
    ///   Can be used to induce a consturction error in the primitive batch holder
    /// </summary>
    internal InduceErrorDelegate InducePrimitiveBatchErrorDelegate;
#endif
    /// <summary>Primitive batches for the vertex types of all particle systems</summary>
    private Dictionary<Type, PrimitiveBatchHolder> primitiveBatches;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel
