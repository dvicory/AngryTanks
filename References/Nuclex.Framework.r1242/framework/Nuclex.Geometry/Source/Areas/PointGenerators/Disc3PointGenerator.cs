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

namespace Nuclex.Geometry.Areas.PointGenerators {

  /// <summary>Point generator for disc areas</summary>
  public static class Disc3PointGenerator {

    /// <summary>Returns a random point on the perimeter of a disc</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="orientation">Orientation of the disc in space</param>
    /// <param name="radius">Radius of the disc</param>
    /// <returns>A random point on the disc's perimeter</returns>
    public static Vector3 GenerateRandomPointOnPerimeter(
      IRandom randomNumberGenerator, Matrix orientation, float radius
    ) {

      // Use the 2D random point generator to get determine the random point
      Vector2 point = Disc2PointGenerator.GenerateRandomPointOnPerimeter(
        randomNumberGenerator, radius
      );

      // Transform the point by the disc's orientation matrix to get the world
      // coordinates of the point
      return Vector3.Transform(new Vector3(point.X, point.Y, 0.0f), orientation);

    }

    /// <summary>Returns a random point within a disc</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="orientation">Orientation of the disc in space</param>
    /// <param name="radius">Radius of the disc</param>
    /// <returns>A random point within the disc</returns>
    public static Vector3 GenerateRandomPointWithin(
      IRandom randomNumberGenerator, Matrix orientation, float radius
    ) {

      // Use the 2D random point generator to get determine the random point
      Vector2 point = Disc2PointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, radius
      );

      // Transform the point by the disc's orientation matrix to get the world
      // coordinates of the point
      return Vector3.Transform(new Vector3(point.X, point.Y, 0.0f), orientation);

    }

  }

} // namespace Nuclex.Geometry.Volumes.Generators
