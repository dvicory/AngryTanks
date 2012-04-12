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

  /// <summary>Simulates the effects of gravity on particles</summary>
  /// <typeparam name="ParticleType">Data type of the particles</typeparam>
  public class GravityAffector<ParticleType> : IParticleAffector<ParticleType> {

    /// <summary>Average gravity on earth in meters per squared second</summary>
    public const float StandardEarthGravity = 9.80665f;

    /// <summary>Initializes a new gravity affector</summary>
    /// <param name="modifier">
    ///   Modifier through which the particles' properties will be changed
    /// </param>
    public GravityAffector(IParticleModifier<ParticleType> modifier) :
      this(modifier, StandardEarthGravity) { }

    /// <summary>
    ///   Initializes a new gravity affector with a custom gravity constant
    /// </summary>
    /// <param name="modifier">
    ///   Modifier through which the particles' properties will be changed
    /// </param>
    /// <param name="gravity">
    ///   Gravity constant that will be applied to the particles
    /// </param>
    public GravityAffector(IParticleModifier<ParticleType> modifier, float gravity) :
      this(modifier, new Vector3(0.0f, -gravity, 0.0f)) { }

    /// <summary>
    ///   Initializes a new gravity affector with a custom gravity vector
    /// </summary>
    /// <param name="modifier">
    ///   Modifier through which the particles' properties will be changed
    /// </param>
    /// <param name="gravity">Gravity vector that will be applied to the particles</param>
    public GravityAffector(IParticleModifier<ParticleType> modifier, Vector3 gravity) {
      this.modifier = modifier;
      this.Gravity = gravity;
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
        float seconds = (float)updates / UpdatesPerSecond;
        Vector3 accumulatedVelocity = this.Gravity * seconds;

        this.modifier.AddToVelocity(particles, start, count, ref accumulatedVelocity);
      }
    }

    /// <summary>XNA's standard update frequency</summary>
    /// <remarks>
    ///   Can be adjusted if your simulation runs at a different speed to ensure that
    ///   the gravity constant is still expressed in m/s^2
    /// </remarks>
    public float UpdatesPerSecond = 60.0f;

    /// <summary>Gravity in m/s^2 that will be applied to the particles</summary>
    public Vector3 Gravity;

    /// <summary>Particle modifier used to apply gravity to the particles</summary>
    private IParticleModifier<ParticleType> modifier;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
