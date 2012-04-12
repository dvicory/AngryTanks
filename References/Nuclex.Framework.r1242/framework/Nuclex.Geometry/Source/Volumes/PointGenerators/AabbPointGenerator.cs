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

  /// <summary>Point generator for axis aligned box volumes</summary>
  public static class AabbPointGenerator {

    /// <summary>Returns a random point on the surface of a box</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="extents">Extents of the box</param>
    /// <returns>A random point on the box' surface</returns>
    /// <remarks>
    ///   The performance of this algorithm varies slightly depending on the face
    ///   that is chosen for the random point because a different number of
    ///   comparisons and subtractions will be performed.
    /// </remarks>
    public static Vector3 GenerateRandomPointOnSurface(
      IRandom randomNumberGenerator, Vector3 extents
    ) {

      // For this task, we also need the dimensions of the box
      Vector3 dimensions = extents * 2.0f;

      // Determine the area covered by the sides of the box
      float leftRightArea = dimensions.Z * dimensions.Y;
      float topBottomArea = dimensions.X * dimensions.Z;
      float frontBackArea = dimensions.X * dimensions.Y;

      // Choose which face of the box the point is going be on
      float side = (float)randomNumberGenerator.NextDouble() *
        (leftRightArea * 2.0f) * (topBottomArea * 2.0f) * (frontBackArea * 2.0f);

      // Now obtain were the point will be located
      float u = (float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f;
      float v = (float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f;

      // Calculate the final world space coordinates of the point
      side -= leftRightArea;
      if(side < 0.0)
        return new Vector3(-extents.X, v * extents.Y, u * extents.Z);

      side -= leftRightArea;
      if(side < 0.0)
        return new Vector3(+extents.X, v * extents.Y, u * extents.Z);

      side -= topBottomArea;
      if(side < 0.0)
        return new Vector3(u * extents.X, +extents.Y, v * extents.Z);

      side -= topBottomArea;
      if(side < 0.0)
        return new Vector3(u * extents.X, -extents.Y, v * extents.Z);

      side -= frontBackArea;
      if(side < 0.0)
        return new Vector3(u * extents.X, v * extents.Y, -extents.Z);

      else
        return new Vector3(u * extents.X, v * extents.Y, +extents.Z);
    }

    /// <summary>Returns a random point within a box</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="extents">Extents of the box</param>
    /// <returns>A random point within the box</returns>
    public static Vector3 GenerateRandomPointWithin(
      IRandom randomNumberGenerator, Vector3 extents
    ) {

      return new Vector3(
        ((float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f) * extents.X,
        ((float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f) * extents.Y,
        ((float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f) * extents.Z
      );

    }

  }

} // namespace Nuclex.Geometry.Volumes.Generators
