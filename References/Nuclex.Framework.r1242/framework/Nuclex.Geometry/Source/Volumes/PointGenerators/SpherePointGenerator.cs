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

  /// <summary>Point generator for sphere volumes</summary>
  public static class SpherePointGenerator {

    /// <summary>Returns a random point on the surface of a sphere</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="radius">Radius of the sphere</param>
    /// <returns>A random point on the sphere's surface</returns>
    public static Vector3 GenerateRandomPointOnSurface(
      IRandom randomNumberGenerator, float radius
    ) {

      // Choose a random longitude for the point
      float phi = (float)randomNumberGenerator.NextDouble() * MathHelper.TwoPi;

      // The z axis goes straight through the sphere and is chosen in random.
      float z = (float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f;

      // From that, we'll calculate the latitude this point will have on the
      // sphere's surface.
      float theta = (float)Math.Sqrt(1.0f - z * z);

      // Calculate the final position of the point in world space
      return new Vector3(
        radius * theta * (float)Math.Cos(phi),
        radius * theta * (float)Math.Sin(phi),
        radius * z
      );

    }

    /// <summary>Returns a random point within a sphere</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="radius">Radius of the sphere</param>
    /// <returns>A random point with the sphere</returns>
    public static Vector3 GenerateRandomPointWithin(
      IRandom randomNumberGenerator, float radius
    ) {

      // TODO: This is just an approximation. Find the real algorithm.
      float r = (float)randomNumberGenerator.NextDouble();
      r = (float)Math.Sqrt(r * r);
      r = (float)Math.Sqrt(r * r);

      return GenerateRandomPointOnSurface(randomNumberGenerator, r);

    }


  }

} // namespace Nuclex.Geometry.Volumes.Generators
