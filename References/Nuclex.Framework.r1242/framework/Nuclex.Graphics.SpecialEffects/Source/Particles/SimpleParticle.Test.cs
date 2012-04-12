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

  /// <summary>Unit tests for the simple particle class</summary>
  [TestFixture]
  internal class SimpleParticleTest {

    /// <summary>
    ///   Verifies that the constructor of the simple particle class is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      SimpleParticle particle = new SimpleParticle(
        new Vector3(1.1f, 2.2f, 3.3f), new Vector3(4.4f, 5.5f, 6.6f)
      );

      Assert.AreEqual(1.1f, particle.Position.X);
      Assert.AreEqual(2.2f, particle.Position.Y);
      Assert.AreEqual(3.3f, particle.Position.Z);
      Assert.AreEqual(4.4f, particle.Velocity.X);
      Assert.AreEqual(5.5f, particle.Velocity.Y);
      Assert.AreEqual(6.6f, particle.Velocity.Z);
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Water

#endif // UNITTEST
