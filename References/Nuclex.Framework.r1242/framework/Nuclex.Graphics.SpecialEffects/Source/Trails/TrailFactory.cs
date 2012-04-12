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

namespace Nuclex.Graphics.SpecialEffects.Trails {

  /// <summary>Creates trail particles, reusing their trail queues</summary>
  public class TrailFactory {

    /// <summary>Initializes a new trail particle</summary>
    public TrailFactory(int maximumTrailLength) {
      this.maximumTrailLength = maximumTrailLength;
    }

#if false
    /// <summary>Queues of trail particles the factory will resuse</summary>
    private List<Queue<TrailParticle>> trailQueues;
#endif

    /// <summary>Maximum number of particles </summary>
    private int maximumTrailLength;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Trails
