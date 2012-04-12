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

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Mass-modifies properties of particles</summary>
  /// <typeparam name="ParticleType">Data type of the particles</typeparam>
  /// <remarks>
  ///   <para>
  ///     This interface provides some common operations typically applied to large
  ///     numbers of particles at once. Therefore, most methods in this interface
  ///     operate on batches to increase efficiency and avoid thousands of needless
  ///     virtual method calls.
  ///   </para>
  ///   <para>
  ///     If the exact operation you need isn't in here, either do it using
  ///     the lower granularity IParticleAccessor for the price of losing some
  ///     performance or write a specialized affector for you particle type.
  ///   </para>
  /// </remarks>
  public interface IParticleModifier<ParticleType> : IParticleAccessor<ParticleType> {

    /// <summary>Adds each particle's velocity to its current position</summary>
    /// <param name="particles">Particles that will be modified</param>
    /// <param name="start">Index of the first particle that will be modified</param>
    /// <param name="count">Number of particles that will be modified</param>
    void AddVelocityToPosition(
      ParticleType[] particles, int start, int count
    );

    /// <summary>Adds each particle's velocity to its current position scaled</summary>
    /// <param name="particles">Particles that will be modified</param>
    /// <param name="start">Index of the first particle that will be modified</param>
    /// <param name="count">Number of particles that will be modified</param>
    /// <param name="scale">Scale by which to multiply the particle velocity</param>
    void AddScaledVelocityToPosition(
      ParticleType[] particles, int start, int count, float scale
    );

    /// <summary>Adds a fixed amount to each particle's velocity</summary>
    /// <param name="particles">Particles that will be modified</param>
    /// <param name="start">Index of the first particle that will be modified</param>
    /// <param name="count">Number of particles that will be modified</param>
    /// <param name="velocityAdjustment">
    ///   Velocity adjustment that will be added to each particle's velocity
    /// </param>
    void AddToVelocity(
      ParticleType[] particles, int start, int count, ref Vector3 velocityAdjustment
    );

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
