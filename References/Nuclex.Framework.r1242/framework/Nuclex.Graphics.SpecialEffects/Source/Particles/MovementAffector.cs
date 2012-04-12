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

  /// <summary>Moves particles by their current velocity</summary>
  /// <typeparam name="ParticleType">Data type of the particles</typeparam>
  public class MovementAffector<ParticleType> : IParticleAffector<ParticleType> {

    /// <summary>Initializes a new particle movement affector</summary>
    /// <param name="modifier">Used to modify the particles</param>
    public MovementAffector(IParticleModifier<ParticleType> modifier) {
      this.modifier = modifier;
    }

    /// <summary>
    ///   Whether the affector can do multiple updates in a single step without
    ///   changing the outcome of the simulation
    /// </summary>
    public bool IsCoalescable { get { return false; } }

    /// <summary>Applies the affector's effect to a series of particles</summary>
    /// <param name="particles">Particles the affector will be applied to</param>
    /// <param name="start">Index of the first particle that will be affected</param>
    /// <param name="count">Number of particles that will be affected</param>
    /// <param name="updates">Number of updates to perform in the affector</param>
    /// <remarks>
    ///   Contrary to general-purpose particle systems like we might find in expensive
    ///   animation packages, we don't update particles based on time but instead
    ///   use the simplified approach of updating particles in simulation steps.
    ///   This simplifies the implementation and matches a game's architecture where
    ///   the simulation is updated in steps as well to have a predictable outcome.
    /// </remarks>
    public void Affect(ParticleType[] particles, int start, int count, int updates) {
      if(this.modifier.HasVelocity) {
        if(updates == 1) {
          this.modifier.AddVelocityToPosition(particles, start, count);
        } else {
          this.modifier.AddScaledVelocityToPosition(
            particles, start, count, (float)updates
          );
        }
      }
    }

    /// <summary>Particle modifier used to apply gravity to the particles</summary>
    private IParticleModifier<ParticleType> modifier;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
