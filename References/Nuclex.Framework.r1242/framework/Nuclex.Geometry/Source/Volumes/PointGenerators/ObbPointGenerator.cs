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

namespace Nuclex.Geometry.Volumes.PointGenerators {

  /// <summary>Point generator for oriented volumes</summary>
  public static class ObbPointGenerator {

    /// <summary>Returns a random point on the surface of a box</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="orientation">Orientation of the box</param>
    /// <param name="extents">Extents of the box</param>
    /// <returns>A random point on the box's surface</returns>
    public static Vector3 GenerateRandomPointOnSurface(
      IRandom randomNumberGenerator, Matrix orientation, Vector3 extents
    ) {

      // Use the AABB point generator to generate a point in the local coordinate
      // frame of the oriented box
      Vector3 point = AabbPointGenerator.GenerateRandomPointOnSurface(
        randomNumberGenerator, extents
      );

      // Rotate the point into the global coordinate frame
      return Vector3.Transform(point, orientation);

    }

    /// <summary>Returns a random point within the box</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="orientation">Orientation of the box</param>
    /// <param name="extents">Extents of the box</param>
    /// <returns>A random point within the box</returns>
    public static Vector3 GenerateRandomPointWithin(
      IRandom randomNumberGenerator, Matrix orientation, Vector3 extents
    ) {

      // Use the AABB point generator to generate a point in the local coordinate
      // frame of the oriented box
      Vector3 point = AabbPointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, extents
      );

      // Rotate the point into the global coordinate frame
      return Vector3.Transform(point, orientation);
      
    }

  }

} // namespace Nuclex.Geometry.Volumes.Generators
