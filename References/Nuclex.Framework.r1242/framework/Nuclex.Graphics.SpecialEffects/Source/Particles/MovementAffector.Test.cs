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

  /// <summary>Unit tests for the movement affector</summary>
  [TestFixture]
  internal class MovementAffectorTest {

    /// <summary>Tests whether the movement affector's constructor is working</summary>
    [Test]
    public void TestConstructor() {
      MovementAffector<SimpleParticle> affector = new MovementAffector<SimpleParticle>(
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
      MovementAffector<SimpleParticle> affector = new MovementAffector<SimpleParticle>(
        SimpleParticleModifier.Default
      );
      Assert.IsFalse(affector.IsCoalescable);
    }

    /// <summary>
    ///   Tests whether the gravity affector imparts a momentum to particles
    /// </summary>
    [Test]
    public void TestMovement() {
      MovementAffector<SimpleParticle> affector = new MovementAffector<SimpleParticle>(
        SimpleParticleModifier.Default
      );

      SimpleParticle[/*100*/] particles = new SimpleParticle[100];
      particles[0].Velocity = Vector3.One;
      particles[25].Velocity.X = 1.2f;
      particles[50].Velocity.Y = 3.4f;
      particles[75].Velocity.Z = 5.6f;
      particles[99].Velocity = -Vector3.One;

      affector.Affect(particles, 20, 60, 1);

      for(int index = 0; index < 100; ++index) {
        switch(index) {
          case 25: {
            Assert.AreEqual(new Vector3(1.2f, 0.0f, 0.0f), particles[index].Position);
            break;
          }
          case 50: {
            Assert.AreEqual(new Vector3(0.0f, 3.4f, 0.0f), particles[index].Position);
            break;
          }
          case 75: {
            Assert.AreEqual(new Vector3(0.0f, 0.0f, 5.6f), particles[index].Position);
            break;
          }
          default: {
            Assert.AreEqual(Vector3.Zero, particles[index].Position);
            break;
          }
        }
      }
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles

#endif // UNITTEST
