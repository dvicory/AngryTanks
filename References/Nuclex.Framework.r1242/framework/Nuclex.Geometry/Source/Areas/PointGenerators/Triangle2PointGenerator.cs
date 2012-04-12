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

  /// <summary>Point generator for triangle areas</summary>
  public static class Triangle2PointGenerator {

    /// <summary>Returns a random point on the perimeter of a triangle</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="a">Location of the triangle's first corner</param>
    /// <param name="b">Location of the triangle's second corner</param>
    /// <param name="c">Location of the triangle's third corner</param>
    /// <returns>A random point on the triangle's perimeter</returns>
    public static Vector2 GenerateRandomPointOnPerimeter(
      IRandom randomNumberGenerator, Vector2 a, Vector2 b, Vector2 c
    ) {
      Vector2 ab = (b - a);
      Vector2 bc = (c - b);
      Vector2 ca = (a - c);

      float lengthAB = ab.LengthSquared();
      float lengthBC = bc.LengthSquared();
      float lengthCA = ca.LengthSquared();

      float totalLength = lengthAB + lengthBC + lengthCA;
      float position = (float)randomNumberGenerator.NextDouble() * totalLength;

      if(position < lengthAB)
        return a + ab * (position / lengthAB);

      position -= lengthAB;
      if(position < lengthBC)
        return b + bc * (position / lengthBC);

      position -= lengthBC;
      return c + ca * (position / lengthCA);
    }

    /// <summary>Returns a random point within a triangle</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <param name="a">Location of the triangle's first corner</param>
    /// <param name="b">Location of the triangle's second corner</param>
    /// <param name="c">Location of the triangle's third corner</param>
    /// <returns>A random point within the triangle</returns>
    public static Vector2 GenerateRandomPointWithin(
      IRandom randomNumberGenerator, Vector2 a, Vector2 b, Vector2 c
    ) {

      // Treat triangle as the half of a parallelogram. Then calculate a
      // random point along the width and the height.
      //
      //       Width
      //    -----------
      //   a___________  
      //   /          /  /
      //  /          /  / Height
      // /__________/  /
      // b         c
      //
      // Now split the parallelogram along a-c and mirror any points that
      // end up on the other side.
      //
      //   xx = rand(1.0)
      //   yy = rand(1.0)
      //
      //   if((xx + yy) > 1.0) {
      //     xx = 1.0 - xx;
      //     yy = 1.0 - yy;
      //   }
      //
      // Then calculate the absolute coordinates of the point
      //    
      //   a. 
      //   /  . 
      //  /     .   
      // /________. 
      // b         c
      //
      // x = b.x + xx * (c.x - b.x) + (b.x - a.x) * yy
      // y = yy * (a.y - b.y)

      // This might be using the same approach, not sure though :)
      // http://vcg.isti.cnr.it/activities/geometryegraphics/pointintetraedro.html

      // Calculate random barycentric coordinates inside the unit
      // triangle (0,0)-(1,0)-(0,1). First, we take random x and y coordinates in
      // a box with side lengths ab, ac
      float x = (float)randomNumberGenerator.NextDouble();
      float y = (float)randomNumberGenerator.NextDouble();

      // The triangle covers exactly half of the box our random points are distributed
      // in. Instead of rejecting these coordinates, we mirror the other half of the box
      // back into the triangle (on the bc edge of the triangle)
      if(x + y > 1.0f) {
        x = 1.0f - x;
        y = 1.0f - y;
      }
      float z = 1.0f - x - y;

      // Convert the barycentric coordinates back into cartesian coordinates
      return (a * x) + (b * y) + (c * z);

    }

  }

} // namespace Nuclex.Geometry.Volumes.Generators
