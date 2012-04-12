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

namespace Nuclex.Graphics.SpecialEffects.Trails {

  /// <summary>Used to access the queue of a trail</summary>
  /// <typeparam name="TrailType">Data type storing the trail sections</typeparam>
  /// <typeparam name="ParticleType">Data type of the particles</typeparam>
  public interface ITrailAccessor<TrailType, ParticleType> {

    /// <summary>Retrieves the queue storing a trail's sections</summary>
    /// <param name="trail">Trail whose sections will be returned</param>
    /// <param name="queue">
    ///   Will recieve the queue containing the sections that make up the trail
    /// </param>
    void GetQueue(ref TrailType trail, out Queue<ParticleType> queue);

    /// <summary>Assigns the queue storing a trail's sections</summary>
    /// <param name="trail">Trail whose sections will be assigned</param>
    /// <param name="queue">
    ///   Queue containing the sections that will be assigned to the trail
    /// </param>
    void SetQueue(ref TrailType trail, ref Queue<ParticleType> queue);

  }

} // namespace Nuclex.Graphics.SpecialEffects.Trails
