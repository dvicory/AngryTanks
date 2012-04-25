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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Nuclex.Graphics.Debugging {

  /// <summary>Generates vertices for a solid (filled) arrow</summary>
  internal static class SolidArrowVertexGenerator {

    /// <summary>Number of vertices this generator produces</summary>
    internal const int VertexCount = 144;

    /// <summary>Outputs the vertices for a solid arrow into the specified array</summary>
    /// <param name="vertices">Array to write the arrow vertices into</param>
    /// <param name="startIndex">Index in the array to begin writing at</param>
    /// <param name="origin">
    ///   Location at which to draw the arrow (this will form the exact center of
    ///   the drawn arrow's base)
    /// </param>
    /// <param name="direction">
    ///   Direction the arrow is pointing into. The arrow's size is relative to
    ///   the length of this vector.
    /// </param>
    /// <param name="color">Color for the arrow</param>
    internal static void Generate(
      VertexPositionColor[] vertices, int startIndex,
      Vector3 origin, Vector3 direction, Color color
    ) {

      // Build a vector pointing in an arbitrary direction that is perpendicular to
      // the direction the arrow is pointing at. This can be done by simply juggling
      // the vector elements around by one place.
      Vector3 normalizedDirection = Vector3.Normalize(direction);
      Vector3 up = VectorHelper.GetPerpendicularVector(normalizedDirection);
      Vector3 right = Vector3.Cross(normalizedDirection, up);
      
      float length = direction.Length();
      up *= length;
      right *= length;

      // Generate supporting points for the cylinder shape of the arrow's base
      Vector3[/*8*/] ring = new Vector3[8];
      for(int segment = 0; segment < 8; ++segment) {
        double angle = (double)segment / 4.0 * Math.PI;
        ring[segment] = rotateAroundAxis(up * 0.1f, normalizedDirection, angle);
      }

      int index = startIndex;

      Vector3 twoThird = origin + (direction * 0.667f);
      Vector3 end = origin + direction;

      for(int segment = 0; segment < 8; ++segment) {

        int nextSegment = (segment + 1) % 8;

        // Triangle on the cap of cylinder at the arrow's base
        vertices[index].Position = origin;
        vertices[index].Color = color;
        vertices[index + 1].Position = origin + ring[nextSegment];
        vertices[index + 1].Color = color;
        vertices[index + 2].Position = origin + ring[segment];
        vertices[index + 2].Color = color;

        // Side wall segment of the cylinder at the arrow's base
        vertices[index + 3].Position = twoThird + ring[segment];
        vertices[index + 3].Color = color;
        vertices[index + 4].Position = origin + ring[segment];
        vertices[index + 4].Color = color;
        vertices[index + 5].Position = origin + ring[nextSegment];
        vertices[index + 5].Color = color;
        vertices[index + 6].Position = twoThird + ring[segment];
        vertices[index + 6].Color = color;
        vertices[index + 7].Position = origin + ring[nextSegment];
        vertices[index + 7].Color = color;
        vertices[index + 8].Position = twoThird + ring[nextSegment];
        vertices[index + 8].Color = color;

        // On the arrowhead, alternate between close and far vertices to
        // create four 'blades' making it easier to spot the arrow's direction than
        // with a completely rounded cap.
        bool isEvenSegment = (segment & 1) == 0;
        float scale = isEvenSegment ? 3.0f : 1.0f;
        float nextScale = isEvenSegment ? 1.0f : 3.0f;

        // Segment on underside of arrowhead
        vertices[index + 9].Position = twoThird + ring[segment] * scale;
        vertices[index + 9].Color = color;
        vertices[index + 10].Position = twoThird + ring[segment];
        vertices[index + 10].Color = color;
        vertices[index + 11].Position = twoThird + ring[nextSegment];
        vertices[index + 11].Color = color;
        vertices[index + 12].Position = twoThird + ring[segment] * scale;
        vertices[index + 12].Color = color;
        vertices[index + 13].Position = twoThird + ring[nextSegment];
        vertices[index + 13].Color = color;
        vertices[index + 14].Position = twoThird + ring[nextSegment] * nextScale;
        vertices[index + 14].Color = color;

        // Segment on the blade of the arrowhead
        vertices[index + 15].Position = end;
        vertices[index + 15].Color = color;
        vertices[index + 16].Position = twoThird + ring[segment] * scale;
        vertices[index + 16].Color = color;
        vertices[index + 17].Position = twoThird + ring[nextSegment] * nextScale;
        vertices[index + 17].Color = color;

        index += 18;

      } // for segment 1 .. 8

    }

    /// <summary>Rotates a point around an arbitrary axis defined by a vector</summary>
    /// <param name="pointToRotate">Point to be rotated</param>
    /// <param name="axis">Axis to rotate the point around</param>
    /// <param name="angle">Angle, in radians, to rotate the point by</param>
    /// <returns>The point rotated by the specified amount around the give axis</returns>
    private static Vector3 rotateAroundAxis(
      Vector3 pointToRotate, Vector3 axis, double angle
    ) {

      // Precalculate some values to make the actual math more readable
      float sinAngle = (float)Math.Sin(angle);
      float cosAngle = (float)Math.Cos(angle);

      float xx = axis.X * pointToRotate.X;
      float xy = axis.X * pointToRotate.Y;
      float xz = axis.X * pointToRotate.Z;
      float yx = axis.Y * pointToRotate.X;
      float yy = axis.Y * pointToRotate.Y;
      float yz = axis.Y * pointToRotate.Z;
      float zx = axis.Z * pointToRotate.X;
      float zy = axis.Z * pointToRotate.Y;
      float zz = axis.Z * pointToRotate.Z;

      // Apply the rotation and return the resulting coordinates as a vector
      return new Vector3(

        axis.X * (xx + yy + zz) +
          (pointToRotate.X * (axis.Y * axis.Y + axis.Z * axis.Z) -
          axis.X * (yy + zz)) * cosAngle +
          (-zy + yz) * sinAngle,

        axis.Y * (xx + yy + zz) +
          (pointToRotate.Y * (axis.X * axis.X + axis.Z * axis.Z) -
          axis.Y * (xx + zz)) * cosAngle +
          (zx - xz) * sinAngle,

        axis.Z * (xx + yy + zz) +
          (pointToRotate.Z * (axis.X * axis.X + axis.Y * axis.Y) -
          axis.Z * (xx + yy)) * cosAngle +
          (-yx + xy) * sinAngle

      ); // new Vector3

    }

  }

} // namespace Nuclex.Graphics.Debugging
