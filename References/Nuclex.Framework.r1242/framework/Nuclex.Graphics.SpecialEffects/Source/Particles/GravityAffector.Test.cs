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
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Unit tests for the gravity affector</summary>
  [TestFixture]
  internal class GravityAffectorTest {

    /// <summary>Tests whether the gravity affector's constructor is working</summary>
    [Test]
    public void TestConstructor() {
      GravityAffector<SimpleParticle> affector = new GravityAffector<SimpleParticle>(
        SimpleParticleModifier.Default
      );
      Assert.IsNotNull(affector); // nonsense; avoids compiler warning
    }
    
    /// <summary>
    ///   Verifies that the gravity affector reports the right status about
    ///   its coalescability
    /// </summary>
    [Test]
    public void TestCoalescability() {
      GravityAffector<SimpleParticle> affector = new GravityAffector<SimpleParticle>(
        SimpleParticleModifier.Default
      );
      Assert.IsFalse(affector.IsCoalescable);
    }

    /// <summary>
    ///   Tests whether the gravity affector imparts a momentum to particles
    /// </summary>
    [Test]
    public void TestGravity() {
      GravityAffector<SimpleParticle> affector = new GravityAffector<SimpleParticle>(
        SimpleParticleModifier.Default
      );

      SimpleParticle[/*100*/] particles = new SimpleParticle[100];
      affector.Affect(particles, 25, 50, (int)affector.UpdatesPerSecond);

      for(int index = 0; index < 25; ++index) {
        Assert.AreEqual(Vector3.Zero, particles[index].Velocity);
      }
      for(int index = 25; index < 50; ++index) {
        Assert.AreEqual(affector.Gravity, particles[index].Velocity);
      }
      for(int index = 75; index < 100; ++index) {
        Assert.AreEqual(Vector3.Zero, particles[index].Velocity);
      }
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles

#endif // UNITTEST
