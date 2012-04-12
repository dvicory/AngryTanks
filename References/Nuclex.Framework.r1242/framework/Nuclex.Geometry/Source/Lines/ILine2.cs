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
using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Lines {

  /// <summary>Straight line situated in 2D space</summary>
  /// <remarks>
  ///   This is the generalized interface to all kinds of lines (including rays and
  ///   line segments). Be careful not to mistake this for an infinite Line like it
  ///   is represented by the Line2 class.
  /// </remarks>
  public interface ILine2 {

    /// <summary>Determines the closest point on the ray to the specified location</summary>
    /// <param name="location">Random location to which the closest point is determined</param>
    /// <returns>The closest point within the ray</returns>
    Vector2 ClosestPointTo(Vector2 location);

  }

} // namespace Nuclex.Geometry.Ranges
