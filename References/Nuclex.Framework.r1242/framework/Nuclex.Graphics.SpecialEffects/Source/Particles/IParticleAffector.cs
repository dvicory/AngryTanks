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

  /// <summary>Influences particles to simulate environmental effects</summary>
  /// <typeparam name="ParticleType">Data type of the particles</typeparam>
  public interface IParticleAffector<ParticleType> {

    /// <summary>
    ///   Whether the affector can do multiple updates in a single step without
    ///   changing the outcome of the simulation
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Generally, this should be true when the affector only modifies properties
    ///     that no other affector accesses or that do not change the simulation's
    ///     outcome.
    ///   </para>
    ///   <para>
    ///     For example, a color affector that changes a particle's color solely
    ///     based on its age is coalescable. If the particle system needs to do
    ///     10 update steps at once, it can instruct the color affector to do 10 updates
    ///     in a row without calling any other registered affectors inbetween.
    ///   </para>
    ///   <para>
    ///     However, a weight affector and a gravity affector are not coalescable
    ///     because running the gravity affector 10 times and then running the
    ///     weight affector 10 times will not yield an equivalent result to running
    ///     both affectors in succession 10 times.
    ///   </para>
    /// </remarks>
    bool IsCoalescable { get; }
    
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
    void Affect(ParticleType[] particles, int start, int count, int updates);

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
