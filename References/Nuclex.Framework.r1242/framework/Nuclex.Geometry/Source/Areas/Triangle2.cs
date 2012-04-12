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

namespace Nuclex.Geometry.Areas {

  /// <summary>Two-dimensional triangle</summary>
  public class Triangle2 : IArea2 {

    /// <summary>Initializes a new triangle</summary>
    /// <param name="a">First corner of the triangle</param>
    /// <param name="b">Second corner of the triangle</param>
    /// <param name="c">Third corner of the triangle</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Triangle2(Vector2 a, Vector2 b, Vector2 c) {
      A = a;
      B = b;
      C = c;
    }

    /// <summary>Surface area that the shape contains</summary>
    /// <remarks>
    ///   <para>
    ///     Heron's triangle area formular states that, given s = (a + b + c) / s the
    ///     area of a triangle can be calculated as
    ///   </para>
    ///   <code>
    ///              _________________________________
    ///     area = \/ s * (s - a) * (s - b) * (s - c)   
    ///
    ///   </code>
    ///   <para>
    ///     In a paper by W. Kahan this method is proven to be numerically unstable
    ///     for floating point numbers. He recommends to use the following formula
    ///     instead, where the lengths a, b and c have to be sorted in ascending order.
    ///   </para>
    ///   <code>
    ///                     ______________________________________________________________
    ///     area = 0.25 * \/ (a + (b + c)) * (c - (a - b)) * (c + (a - b)) * (a + b - c))
    ///
    ///   </code>
    /// </remarks>
    public float Area {
      get {
        float a = (B - A).Length();
        float b = (C - B).Length();
        float c = (A - C).Length();

        float s = (a + b + c) / 2.0f;
        return (float)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
      }
    }

    /// <summary>The total length of the area's circumference</summary>
    public float CircumferenceLength {
      get {
        return (float)Math.Sqrt(
          (B - A).LengthSquared() + (C - B).LengthSquared() + (A - C).LengthSquared()
        );
      }
    }

    /// <summary>The center of mass within the shape</summary>
    public Vector2 CenterOfMass {
      get {
        return (A + B + C) / 3.0f;
      }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public AxisAlignedRectangle2 BoundingBox {
      get {
        return new AxisAlignedRectangle2(
          new Vector2(
            Math.Min(Math.Min(A.X, B.X), C.X),
            Math.Min(Math.Min(A.Y, B.Y), C.Y)
          ),
          new Vector2(
            Math.Max(Math.Max(A.X, B.X), C.X),
            Math.Max(Math.Max(A.Y, B.Y), C.Y)
          )
        );
      }
    }

    /// <summary>Determines whether the triangle's points are in clockwise order</summary>
    /// <remarks>
    ///   This method assumes a normal cartesian coordinate system with the X axis
    ///   extending to the right and the Y axis extending upwards.
    /// </remarks>
    public bool IsClockwiseTriangle {
      get { return Lines.Segment2.Orientation(A, B, C) == Side.Negative; }
    }

    /// <summary>Locates the nearest point on the triangle to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point on the triangle to the specified location</returns>
    public Vector2 ClosestPointTo(Vector2 location) {

      // Check whether the point is on the opposite side of the A-B line to C.
      // If so, the point is outside of the triangle
      if(
        Lines.Segment2.Orientation(A, B, location) !=
        Lines.Segment2.Orientation(A, B, C)
      ) {
        return (new Lines.Segment2(A, B)).ClosestPointTo(location);
      }

      // Check whether the point is on the opposite side of the B-C line to A.
      // If so, the point is outside of the triangle
      if(
        Lines.Segment2.Orientation(B, C, location) !=
        Lines.Segment2.Orientation(B, C, A)
      ) {
        return (new Lines.Segment2(B, C)).ClosestPointTo(location);
      }

      // Check whether the point is on the opposite side of the C-A line to B.
      // If so, the point is outside of the triangle
      if(
        Lines.Segment2.Orientation(C, A, location) !=
        Lines.Segment2.Orientation(C, A, B)
      ) {
        return (new Lines.Segment2(C, A)).ClosestPointTo(location);
      }

      // The point is inside of the triangle
      return location;
    }

    /// <summary>Returns a random point on the area's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the area's perimeter</returns>
    public Vector2 RandomPointOnPerimeter(IRandom randomNumberGenerator) {
      return PointGenerators.Triangle2PointGenerator.GenerateRandomPointOnPerimeter(
        randomNumberGenerator, this.A, this.B, this.C
      );
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector2 RandomPointWithin(IRandom randomNumberGenerator) {
      return PointGenerators.Triangle2PointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, this.A, this.B, this.C
      );
    }

    /// <summary>The three corner points of the triangle</summary>
    public Vector2 A, B, C;
  }

} // namespace Nuclex.Geometry.Areas
