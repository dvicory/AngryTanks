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

  /// <summary>Used to access particles in a generic manner</summary>
  /// <typeparam name="ParticleType">Data type of the particles</typeparam>
  /// <remarks>
  ///   This interface allows generic particle affectors to modify custom particles
  ///   without knowing about their specific implementation details. This way,
  ///   generic particle affectors can be written that allow gravity, wind and
  ///   other effects to be reused on different particle types.
  /// </remarks>
  public interface IParticleAccessor<ParticleType> {

    /// <summary>Obtains the position of an individual particle</summary>
    /// <param name="particle">Particle whose position will be returned</param>
    /// <param name="position">
    ///   Output parameter that will receive the particle's position
    /// </param>
    void GetPosition(ref ParticleType particle, out Vector3 position);

    /// <summary>Changes the position of an individual particle</summary>
    /// <param name="particle">Particle whose position will be changed</param>
    /// <param name="position">Position the particle will be moved to</param>
    void SetPosition(ref ParticleType particle, ref Vector3 position);

    /// <summary>Whether the particle type has a velocity</summary>
    bool HasVelocity { get; }

    /// <summary>Obtains the velocity of an individual particle</summary>
    /// <param name="particle">Particle whose velocity will be returned</param>
    /// <param name="velocity">
    ///   Output parameter that will receive the particle's velocity
    /// </param>
    /// <remarks>
    ///   If the particle doesn't have a velocity, it's safe to set the velocity
    ///   to Vector3.Zero.
    /// </remarks>
    void GetVelocity(ref ParticleType particle, out Vector3 velocity);

    /// <summary>Changes the velocity of an individual particle</summary>
    /// <param name="particle">Particle whose velocity will be changed</param>
    /// <param name="velocity">Velocity that will be assigned to the particle</param>
    /// <remarks>
    ///   If particles don't have a velocity, it's safe to do nothing in this method.
    /// </remarks>
    void SetVelocity(ref ParticleType particle, ref Vector3 velocity);

    /// <summary>Whether the particle type has a weight</summary>
    bool HasWeight { get; }

    /// <summary>Obtains the weight of a particle</summary>
    /// <param name="particle">Particle whose weight will be returned</param>
    /// <returns>The weight of the provided particle</returns>
    float GetWeight(ref ParticleType particle);

    /// <summary>Changes the weight of a particle</summary>
    /// <param name="particle">Particle whose weight will be set</param>
    /// <param name="weight">Weight that will be assigned to the particle</param>
    void SetWeight(ref ParticleType particle, float weight);

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
