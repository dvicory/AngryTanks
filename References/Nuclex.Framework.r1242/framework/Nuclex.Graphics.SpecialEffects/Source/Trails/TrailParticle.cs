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

  /// <summary>Trail particle for reference and testing</summary>
  public class TrailParticle {

    /// <summary>Initializes a new trail particle</summary>
    public TrailParticle(int maximumTrailLength) {
      this.Trail = new Queue<Vector3>(maximumTrailLength);
    }

    /// <summary>Supporting points the trail passes through</summary>
    public Queue<Vector3> Trail;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Trails
