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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.Graphics;

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Modifies fields in the simple particle structure</summary>
  public class SimpleParticleModifier : IParticleModifier<SimpleParticle> {

    /// <summary>The default instance of this modifier</summary>
    public static readonly SimpleParticleModifier Default = new SimpleParticleModifier();

    /// <summary>Adds each particle's velocity to its current position</summary>
    /// <param name="particles">Particles that will be modified</param>
    /// <param name="start">Index of the first particle that will be modified</param>
    /// <param name="count">Number of particles that will be modified</param>
    public void AddVelocityToPosition(SimpleParticle[] particles, int start, int count) {
      for(int end = start + count; start < end; ++start) {
        particles[start].Position.X += particles[start].Velocity.X;
        particles[start].Position.Y += particles[start].Velocity.Y;
        particles[start].Position.Z += particles[start].Velocity.Z;
      }
    }

    /// <summary>Adds each particle's velocity to its current position scaled</summary>
    /// <param name="particles">Particles that will be modified</param>
    /// <param name="start">Index of the first particle that will be modified</param>
    /// <param name="count">Number of particles that will be modified</param>
    /// <param name="scale">Scale by which to multiply the particle velocity</param>
    public void AddScaledVelocityToPosition(
      SimpleParticle[] particles, int start, int count, float scale
    ) {
      for(int end = start + count; start < end; ++start) {
        particles[start].Position.X += particles[start].Velocity.X * scale;
        particles[start].Position.Y += particles[start].Velocity.Y * scale;
        particles[start].Position.Z += particles[start].Velocity.Z * scale;
      }
    }

    /// <summary>Adds a fixed amount to each particle's velocity</summary>
    /// <param name="particles">Particles that will be modified</param>
    /// <param name="start">Index of the first particle that will be modified</param>
    /// <param name="count">Number of particles that will be modified</param>
    /// <param name="velocityAdjustment">
    ///   Velocity adjustment that will be added to each particle's velocity
    /// </param>
    public void AddToVelocity(
      SimpleParticle[] particles, int start, int count, ref Vector3 velocityAdjustment
    ) {
      for(int end = start + count; start < end; ++start) {
        particles[start].Velocity.X += velocityAdjustment.X;
        particles[start].Velocity.Y += velocityAdjustment.Y;
        particles[start].Velocity.Z += velocityAdjustment.Z;
      }
    }

    /// <summary>Obtains the position of an individual particle</summary>
    /// <param name="particle">Particle whose position will be returned</param>
    /// <param name="position">
    ///   Output parameter that will receive the particle's position
    /// </param>
    public void GetPosition(ref SimpleParticle particle, out Vector3 position) {
      position = particle.Position;
    }

    /// <summary>Changes the position of an individual particle</summary>
    /// <param name="particle">Particle whose position will be changed</param>
    /// <param name="position">Position the particle will be moved to</param>
    public void SetPosition(ref SimpleParticle particle, ref Vector3 position) {
      particle.Position = position;
    }

    /// <summary>Whether the particle type has a velocity</summary>
    public bool HasVelocity {
      get { return true; }
    }

    /// <summary>Obtains the velocity of an individual particle</summary>
    /// <param name="particle">Particle whose velocity will be returned</param>
    /// <param name="velocity">
    ///   Output parameter that will receive the particle's velocity
    /// </param>
    /// <remarks>
    ///   If the particle doesn't have a velocity, it's safe to set the velocity
    ///   to Vector3.Zero.
    /// </remarks>
    public void GetVelocity(ref SimpleParticle particle, out Vector3 velocity) {
      velocity = particle.Velocity;
    }

    /// <summary>Changes the velocity of an individual particle</summary>
    /// <param name="particle">Particle whose velocity will be changed</param>
    /// <param name="velocity">Velocity that will be assigned to the particle</param>
    /// <remarks>
    ///   If particles don't have a velocity, it's safe to do nothing in this method.
    /// </remarks>
    public void SetVelocity(ref SimpleParticle particle, ref Vector3 velocity) {
      particle.Velocity = velocity;
    }

    /// <summary>Whether the particle type has a weight</summary>
    public bool HasWeight {
      get { return false; }
    }

    /// <summary>Obtains the weight of a particle</summary>
    /// <param name="particle">Particle whose weight will be returned</param>
    /// <returns>The weight of the provided particle</returns>
    public float GetWeight(ref SimpleParticle particle) { return 0.0f; }

    /// <summary>Changes the weight of a particle</summary>
    /// <param name="particle">Particle whose weight will be set</param>
    /// <param name="weight">Weight that will be assigned to the particle</param>
    public void SetWeight(ref SimpleParticle particle, float weight) { }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
