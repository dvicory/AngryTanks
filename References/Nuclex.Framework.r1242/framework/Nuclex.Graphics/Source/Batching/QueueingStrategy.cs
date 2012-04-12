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

using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics.Batching {

  /// <summary>
  ///   Strategies by which the primitive batcher can consolidate vertices
  /// </summary>
  public enum QueueingStrategy {

    /// <summary>Immediately draws any primitives sent to the batcher.</summary>
    /// <remarks>
    ///   This results in rather poor performance but keeps the drawing order
    ///   unchanged even when multiple primitve batchers are used to draw at
    ///   the same time.
    /// </remarks>
    Immediate,

    /// <summary>
    ///   The vertex batcher caches any drawing commands until a batch is full
    ///   or the End() method is called.
    /// </summary>
    /// <remarks>
    ///   This will greatly improve rendering performance compared to immediate
    ///   drawing. However, using multiple primitve batchers at the same time
    ///   will result in an undefined drawing order (if there are no overlaps or
    ///   the depth buffer is enabled, this won't matter)
    /// </remarks>
    Deferred,

    /// <summary>
    ///   Caches all drawing commands and sorts the primitives queued for drawing
    ///   by the context they use.
    /// </summary>
    /// <remarks>
    ///   This is the fastest mode for many small objects. However, drawing order
    ///   will be undefined, requiring either a zero overlap guarantee for any
    ///   vertices drawn or the depth buffer to be enabled.
    /// </remarks>
    Context

  }

} // namespace Nuclex.Graphics.Batching
