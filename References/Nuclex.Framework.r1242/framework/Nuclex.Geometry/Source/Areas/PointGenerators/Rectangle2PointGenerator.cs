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

  /// <summary>Point generator for rectangle areas</summary>
  public static class Rectangle2PointGenerator {

    /// <summary>Returns a random point on the perimeter of a rectangle</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="min">Lower left coordinate of the rectangle</param>
    /// <param name="max">Upper right coordinate of the rectangle</param>
    /// <returns>A random point on the rectangle's perimeter</returns>
    public static Vector2 GenerateRandomPointOnPerimeter(
      IRandom randomNumberGenerator, Vector2 min, Vector2 max
    ) {
      float width = max.X - min.X;
      float height = max.Y - min.Y;

      // Calculate a position on the perimeter of the rectangle as if the rectangle's
      // outside was a single straight line
      float position;
      {
        float totalPerimeterLength = width * 2.0f + height * 2.0f;
        float randomNumber = (float)randomNumberGenerator.NextDouble();
        position = randomNumber * totalPerimeterLength;
      }

      // Is the calculated position on the lower border of the rectangle?
      if(position < width) {
        return new Vector2(min.X + position, min.Y);
      } else {
        position -= width;
      }

      // Is it on the right border?
      if(position < height) {
        return new Vector2(max.X, min.Y + position);
      } else {
        position -= height;
      }

      // Is it on the upper border?
      if(position < width) {
        return new Vector2(min.X + position, max.Y);
      } else {
        position -= width;
      }

      // It must be on the left border
      return new Vector2(min.X, min.Y + position);
    }

    /// <summary>Returns a random point on the perimeter of a rectangle</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="extents">Extents of the rectangle</param>
    /// <returns>A random point on the rectangle's perimeter</returns>
    public static Vector2 GenerateRandomPointOnPerimeter(
      IRandom randomNumberGenerator, Vector2 extents
    ) {
      return GenerateRandomPointOnPerimeter(randomNumberGenerator, -extents, extents);
    }

    /// <summary>Returns a random point within a rectangle</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="extents">Extents of the rectangle</param>
    /// <returns>A random point within the rectangle</returns>
    public static Vector2 GenerateRandomPointWithin(
      IRandom randomNumberGenerator, Vector2 extents
    ) {

      float x = (float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f;
      float y = (float)randomNumberGenerator.NextDouble() * 2.0f - 1.0f;

      return new Vector2(x * extents.X, y * extents.Y);

    }


  }

} // namespace Nuclex.Geometry.Volumes.Generators
