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

namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel {

  /// <summary>Interface for an source that can emit particles</summary>
  /// <typeparam name="ParticleType">Type of particles being emitted</typeparam>
  public interface IEmitter<ParticleType> {
    
    /// <summary>Emits a new particle</summary>
    /// <returns>The new particle</returns>
    ParticleType Emit();
    
  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel
