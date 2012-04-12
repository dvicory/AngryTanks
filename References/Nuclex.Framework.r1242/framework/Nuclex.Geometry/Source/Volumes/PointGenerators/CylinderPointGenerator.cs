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

  /// <summary>Point generator for cylinder volumes</summary>
  public static class CylinderPointGenerator {

    /// <summary>Returns a random point on the surface of a cylinder</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="orientation">Orientation of the cylinder</param>
    /// <param name="radius">Radius of the cylinder</param>
    /// <param name="length">Length of the cylinder</param>
    /// <returns>A random point on the volume's surface</returns>
    public static Vector3 GenerateRandomPointOnSurface(
      IRandom randomNumberGenerator,
      Matrix orientation, float radius, float length
    ) {

      // Calculate the surface areas of the three sections our cylinder has:
      // Upper cap, side and lower cap
      float capArea = MathHelper.Pi * (radius * radius);
      float sideArea = 2.0f * MathHelper.Pi * radius * length;
      float capAndSideArea = capArea + sideArea;

      // We need a phi value (angle of the random point) in any of the cases
      float phi = (float)randomNumberGenerator.NextDouble() * MathHelper.TwoPi;

      // Choose the section that the random point will be generated on in relation
      // to its surface area so the probability is constant on the entire surface
      float section = (float)randomNumberGenerator.NextDouble() * (capArea * 2.0f + sideArea);

      // Depending on the section, these two values are calculated differently
      float randomRadius;
      float randomZ;

      // Upper cap: Generate a random radius
      if(section < capArea) {
        randomZ = length / 2.0f;
        randomRadius = (float)Math.Sqrt(randomNumberGenerator.NextDouble()) * radius;

      // Side: Generate a random height
      } else if(section < capAndSideArea) {
        randomZ = ((float)randomNumberGenerator.NextDouble() - 0.5f) * length;
        randomRadius = radius;

      // Lower cap: Generate a random radius
      } else {
        randomZ = -length / 2.0f;
        randomRadius = (float)Math.Sqrt(randomNumberGenerator.NextDouble()) * radius;
      }

      // Now transform the point to cartesian coordinates and rotate it into
      // the global coordinate frame
      return Vector3.Transform(
        new Vector3(
          randomRadius * (float)Math.Cos(phi),
          randomRadius * (float)Math.Sin(phi),
          randomZ
        ),
        orientation
      );

    }

    /// <summary>Returns a random point within a cylinder</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="orientation">Orientation of the cylinder</param>
    /// <param name="radius">Radius of the cylinder</param>
    /// <param name="length">Length of the cylinder</param>
    /// <returns>A random point within the cylinder</returns>
    public static Vector3 GenerateRandomPointWithin(
      IRandom randomNumberGenerator,
      Matrix orientation, float radius, float length
    ) {

      Vector2 randomPoint = Areas.PointGenerators.Disc2PointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, radius
      );

      float z = (float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f;

      return Vector3.Transform(
        new Vector3(randomPoint.X, randomPoint.Y, z * length),
        orientation
      );

    }

  }

} // namespace Nuclex.Geometry.Volumes.Generators
