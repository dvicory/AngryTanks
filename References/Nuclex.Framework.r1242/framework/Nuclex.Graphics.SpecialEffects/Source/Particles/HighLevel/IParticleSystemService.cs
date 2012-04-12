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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.Graphics.Batching;

namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel {

  /// <summary>Provides services for rendering and updating particle systems</summary>
  public interface IParticleSystemService {

#if !XNA_4 // TODO: Write a triangle billboard particle renderer

    /// <summary>Adds a particle system to be processed by the manager</summary>
    /// <typeparam name="ParticleType">
    ///   Type of particles being stored in the particle system
    /// </typeparam>
    /// <param name="particleSystem">
    ///   Particle system that will be added to the manager
    /// </param>
    /// <param name="pruneDelegate">Method used to detect dead particles</param>
    /// <param name="effect">Effect that will be used to render the particles</param>
    /// <remarks>
    ///   The particles will be rendered as simple point sprites using the effect.
    ///   Because 
    /// </remarks>
    void AddParticleSystem<ParticleType>(
      ParticleSystem<ParticleType> particleSystem,
      ParticleSystem<ParticleType>.PrunePredicate pruneDelegate,
      Effect effect
    ) where ParticleType : struct
#if XNA_4
, IVertexType
#endif
;

    /// <summary>Adds a particle system to be processed by the manager</summary>
    /// <typeparam name="ParticleType">
    ///   Type of particles being stored in the particle system
    /// </typeparam>
    /// <param name="particleSystem">
    ///   Particle system that will be added to the manager
    /// </param>
    /// <param name="pruneDelegate">Method used to detect dead particles</param>
    /// <param name="drawContext">Drawing context using to render the particles</param>
    /// <remarks>
    ///   The particles will be rendered as simple point sprites using the effect
    ///   provided by the drawing context.
    /// </remarks>
    void AddParticleSystem<ParticleType>(
      ParticleSystem<ParticleType> particleSystem,
      ParticleSystem<ParticleType>.PrunePredicate pruneDelegate,
      DrawContext drawContext
    ) where ParticleType : struct
#if XNA_4
, IVertexType
#endif
;

#endif // !XNA_4

    /// <summary>Adds a particle system to be processed by the manager</summary>
    /// <typeparam name="ParticleType">
    ///   Type of particles being stored in the particle system
    /// </typeparam>
    /// <typeparam name="VertexType">
    ///   Type of vertices that will be generated from the particles
    /// </typeparam>
    /// <param name="particleSystem">
    ///   Particle system that will be added to the manager
    /// </param>
    /// <param name="pruneDelegate">Method used to detect dead particles</param>
    /// <param name="renderer">
    ///   Particle renderer that will turn the particles into vertices and send
    ///   them to a primitive batch for rendering
    /// </param>
    void AddParticleSystem<ParticleType, VertexType>(
      ParticleSystem<ParticleType> particleSystem,
      ParticleSystem<ParticleType>.PrunePredicate pruneDelegate,
      IParticleRenderer<ParticleType, VertexType> renderer
    )
      where ParticleType : struct
      where VertexType : struct 
#if XNA_4
, IVertexType
#endif
;

    /// <summary>Removes a particle system from the manager</summary>
    /// <typeparam name="ParticleType">
    ///   Type of particles being stored in the particle system
    /// </typeparam>
    /// <param name="particleSystem">
    ///   Particle system that will be removed from the manager
    /// </param>
    void RemoveParticleSystem<ParticleType>(
      ParticleSystem<ParticleType> particleSystem
    ) where ParticleType : struct
#if XNA_4
, IVertexType
#endif
;

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
    void RegisterVertex<VertexType>(
      VertexDeclaration vertexDeclaration, int stride
    ) where VertexType : struct;

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
    void RegisterVertex<VertexType>(
      VertexDeclaration vertexDeclaration, int stride, bool takeOwnership
    ) where VertexType : struct;

    /// <summary>Unregisters the specified vertex type for particle systems</summary>
    /// <typeparam name="VertexType">Vertex type that will be unregistered</typeparam>
    void UnregisterVertex<VertexType>() where VertexType : struct;
    
#endif // !XNA_4

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel
