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

namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel {

  /// <summary>Controls the number of particles generated per update cycle</summary>
  public interface IEmissionController {

    /// <summary>
    ///   Number of particles that should be emitted in the update cycle
    /// </summary>
    int ParticlesToEmit { get; }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel
