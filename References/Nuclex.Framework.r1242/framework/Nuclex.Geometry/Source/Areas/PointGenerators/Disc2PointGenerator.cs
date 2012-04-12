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
  public static class Disc2PointGenerator {

    /// <summary>Returns a random point on the perimeter of a disc</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="radius">Radius of the disc</param>
    /// <returns>A random point on the disc's perimeter</returns>
    public static Vector2 GenerateRandomPointOnPerimeter(
      IRandom randomNumberGenerator, float radius
    ) {

      // Choose a random angle for the point
      float phi = (float)randomNumberGenerator.NextDouble() * MathHelper.TwoPi;

      // Calculate the final position of the point in world space
      return new Vector2(
        radius * (float)Math.Cos(phi),
        radius * (float)Math.Sin(phi)
      );

    }

    /// <summary>Returns a random point within a disc</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="radius">Radius of the disc</param>
    /// <returns>A random point within the disc</returns>
    public static Vector2 GenerateRandomPointWithin(
      IRandom randomNumberGenerator, float radius
    ) {

      // Choose a random angle for the point
      float phi = (float)randomNumberGenerator.NextDouble() * MathHelper.Pi * 2.0f;

      // Get a random radius with compensated probability for the outer regions of the disc
      float r = (float)Math.Sqrt(randomNumberGenerator.NextDouble()) * radius;

      // Calculate the final position of the point in world space
      return new Vector2(
        r * (float)Math.Cos(phi),
        r * (float)Math.Sin(phi));

    }


  }

} // namespace Nuclex.Geometry.Volumes.Generators
