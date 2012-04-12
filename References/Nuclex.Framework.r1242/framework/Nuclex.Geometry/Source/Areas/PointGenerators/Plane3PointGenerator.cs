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

  /// <summary>Point generator for planes</summary>
  public static class Plane3PointGenerator {

    /// <summary>Returns a random point on a plane's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="normal">Normal of the plane around which to create a random point</param>
    /// <returns>A random point on the plane's perimeter</returns>
    public static Vector3 RandomPointOnPerimeter(IRandom randomNumberGenerator, Vector3 normal) {
      return generateRandomPlanePoint(randomNumberGenerator, normal);
    }

    /// <summary>Returns a random point within a plane</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="normal">Normal of the plane in which to create a random point</param>
    /// <returns>A random point inside the area</returns>
    public static Vector3 RandomPointWithin(IRandom randomNumberGenerator, Vector3 normal) {
      return generateRandomPlanePoint(randomNumberGenerator, normal);
    }

    /// <summary>Returns a random point around or within the plane</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="normal">Normal of the plane</param>
    /// <returns>A random point around or within the plane</returns>
    /// <remarks>
    ///   <para>
    ///     Since the plane extends to infinity in each direction, the chance of hitting
    ///     a value outside of what can be expressed with a meager float (and actually,
    ///     any finite numbering system) is infinitely larger than being within the range
    ///     we can express, so we can assume the chance of the result being infinity is
    ///     about 1.
    ///   </para>
    ///   <para>
    ///     The same applies to both, finding a point on the plane's perimeter as well as
    ///     finding a point within the plane, so this method is used for both cases.
    ///   </para>
    /// </remarks>
    private static Vector3 generateRandomPlanePoint(
      IRandom randomNumberGenerator, Vector3 normal
    ) {

      // Randomly select a corner of the plane
      int side = randomNumberGenerator.Next(4);

      // Depending on the randomly selected corner, get the normalized coordinates
      // of that corner of the plane (if we involved infinity here, the result would
      // be NaN as soon as a multiplication by negative infinity took place)
      Vector2 corner;
      switch(side) {
        case 0: {
          corner.X = -1.0f;
          corner.Y = -1.0f;
          break;
        }
        case 1: {
          corner.X = +1.0f;
          corner.Y = -1.0f;
          break;
        }
        case 2: {
          corner.X = +1.0f;
          corner.Y = +1.0f;
          break;
        }
        case 3: {
          corner.X = -1.0f;
          corner.Y = +1.0f;
          break;
        }
        default: {
          throw new InvalidOperationException("Random number generator malfunctioned");
        }
      }

      // Determine an up and a right vector from the plane's normal
      Vector3 right = VectorHelper.GetPerpendicularVector(normal);
      Vector3 up = Vector3.Cross(normal, right);

      // Now transform the corner from plane coordinates into the world coordinate frame
      Vector3 unitPerimeterPoint = (up * corner.X) + (right * corner.Y);

      // The randomly chosen corner now is zero in one dimension and infinity in all the
      // others if the plane was exactly aligned to one of the three unit vectors, or
      // infinity in all dimensions if the plane was arbitrary.
      return new Vector3(
        infinitize(unitPerimeterPoint.X),
        infinitize(unitPerimeterPoint.Y),
        infinitize(unitPerimeterPoint.Z)
      );

    }

    /// <summary>Scales any value but zero to positive or negative infinity</summary>
    /// <param name="value">Value that will be scaled to infinity if not zero</param>
    /// <returns>The input value scaled to infinity if it wasn't zero</returns>
    private static float infinitize(float value) {
      if(value < 0.0f) {
        return float.NegativeInfinity;
      } else if(value > 0.0f) {
        return float.PositiveInfinity;
      } else {
        return 0.0f;
      }
    }

  }

} // namespace Nuclex.Geometry.Volumes.Generators
