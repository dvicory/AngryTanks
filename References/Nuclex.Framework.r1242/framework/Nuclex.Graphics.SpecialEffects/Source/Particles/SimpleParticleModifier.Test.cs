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

#if UNITTEST

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Unit tests for the simple particle modifier</summary>
  [TestFixture]
  internal class SimpleParticleModifierTest {

    /// <summary>
    ///   Verifies that the simple particle modifier returns the right
    ///   result when queried whether the particles support weight
    /// </summary>
    [Test]
    public void TestHasWeight() {
      Assert.IsFalse(SimpleParticleModifier.Default.HasWeight);
    }

    /// <summary>Verifies that the GetWeight() method can be used</summary>
    [Test]
    public void TestGetWeight() {
      SimpleParticle particle = new SimpleParticle();
      Assert.AreEqual(0.0f, SimpleParticleModifier.Default.GetWeight(ref particle));
    }

    /// <summary>Verifies that the SetWeight() method can be used</summary>
    [Test]
    public void TestSetWeight() {
      SimpleParticle particle = new SimpleParticle();
      SimpleParticleModifier.Default.SetWeight(ref particle, 12.34f);
    }

    /// <summary>
    ///   Verifies that the simple particle modifier returns the right
    ///   result when queried whether the particles support velocity
    /// </summary>
    [Test]
    public void TestHasVelocity() {
      Assert.IsTrue(SimpleParticleModifier.Default.HasVelocity);
    }

    /// <summary>Verifies that the GetVelocity() method can be used</summary>
    [Test]
    public void TestGetVelocity() {
      SimpleParticle particle = new SimpleParticle();
      particle.Velocity = new Vector3(1.2f, 3.4f, 5.6f);

      Vector3 velocity;
      SimpleParticleModifier.Default.GetVelocity(ref particle, out velocity);

      Assert.AreEqual(new Vector3(1.2f, 3.4f, 5.6f), velocity);
    }

    /// <summary>Verifies that the SetVelocity() method can be used</summary>
    [Test]
    public void TestSetVelocity() {
      SimpleParticle particle = new SimpleParticle();

      Vector3 velocity = new Vector3(1.2f, 3.4f, 5.6f);
      SimpleParticleModifier.Default.SetVelocity(ref particle, ref velocity);

      Assert.AreEqual(velocity, particle.Velocity);
    }

    /// <summary>Verifies that the GetPosition() method can be used</summary>
    [Test]
    public void TestGetPosition() {
      SimpleParticle particle = new SimpleParticle();
      particle.Position = new Vector3(1.2f, 3.4f, 5.6f);

      Vector3 position;
      SimpleParticleModifier.Default.GetPosition(ref particle, out position);

      Assert.AreEqual(new Vector3(1.2f, 3.4f, 5.6f), position);
    }

    /// <summary>Verifies that the SetVelocity() method can be used</summary>
    [Test]
    public void TestSetPosition() {
      SimpleParticle particle = new SimpleParticle();

      Vector3 position = new Vector3(1.2f, 3.4f, 5.6f);
      SimpleParticleModifier.Default.SetPosition(ref particle, ref position);

      Assert.AreEqual(position, particle.Position);
    }


    /// <summary>Verifies that the AddToVelocity() method can be used</summary>
    [Test]
    public void TestAddToVelocity() {
      SimpleParticle[] particles = new SimpleParticle[] {
        new SimpleParticle(), new SimpleParticle()
      };

      Vector3 adjustment = new Vector3(1.2f, 3.4f, 5.6f);
      SimpleParticleModifier.Default.AddToVelocity(particles, 0, 2, ref adjustment);

      Assert.AreEqual(adjustment, particles[0].Velocity);
      Assert.AreEqual(adjustment, particles[1].Velocity);
    }

    /// <summary>Verifies that the AddVelocityToPosition() method can be used</summary>
    [Test]
    public void TestAddVelocityToPosition() {
      SimpleParticle[] particles = new SimpleParticle[] {
        new SimpleParticle(), new SimpleParticle()
      };
      
      particles[0].Velocity = new Vector3(1.0f, 2.0f, 3.0f);
      particles[1].Velocity = new Vector3(4.0f, 5.0f, 6.0f);

      SimpleParticleModifier.Default.AddVelocityToPosition(particles, 0, 2);

      Assert.AreEqual(particles[0].Velocity, particles[0].Position);
      Assert.AreEqual(particles[1].Velocity, particles[1].Position);
    }

    /// <summary>
    ///   Verifies that the AddScaledVelocityToPosition() method can be used
    /// </summary>
    [Test]
    public void TestAddScaledVelocityToPosition() {
      SimpleParticle[] particles = new SimpleParticle[] {
        new SimpleParticle(), new SimpleParticle()
      };

      particles[0].Velocity = new Vector3(1.0f, 2.0f, 3.0f);
      particles[1].Velocity = new Vector3(4.0f, 5.0f, 6.0f);

      SimpleParticleModifier.Default.AddScaledVelocityToPosition(particles, 0, 2, 2.5f);

      Assert.AreEqual(particles[0].Velocity * 2.5f, particles[0].Position);
      Assert.AreEqual(particles[1].Velocity * 2.5f, particles[1].Position);
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Water

#endif // UNITTEST
