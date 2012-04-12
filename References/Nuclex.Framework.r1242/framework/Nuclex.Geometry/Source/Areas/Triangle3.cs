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
using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Areas {

  /*
   * Idea: Swept triangle intersection
   * 
   * Test against original triangle
   * Find lines that are facing away from to movement vector
   * Sweep-test lines against desired object
   * 
   */

  /// <summary>Three-dimensional triangle</summary>
  public class Triangle3 : IArea3 {

    /// <summary>Initializes a new triangle</summary>
    /// <param name="a">First corner of the triangle</param>
    /// <param name="b">Second corner of the triangle</param>
    /// <param name="c">Third corner of the triangle</param>
    [DebuggerStepThrough]
    public Triangle3(Vector3 a, Vector3 b, Vector3 c) {
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
    public Vector3 CenterOfMass {
      get { return (A + B + C) / 3.0f; }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public Volumes.AxisAlignedBox3 BoundingBox {
      get {
        return new Volumes.AxisAlignedBox3(
          new Vector3(
            Math.Min(Math.Min(A.X, B.X), C.X),
            Math.Min(Math.Min(A.Y, B.Y), C.Y),
            Math.Min(Math.Min(A.Z, B.Z), C.Z)
          ),
          new Vector3(
            Math.Max(Math.Max(A.X, B.X), C.X),
            Math.Max(Math.Max(A.Y, B.Y), C.Y),
            Math.Max(Math.Max(A.Z, B.Z), C.Z)
          )
        );
      }
    }

    /// <summary>Locates the nearest point in the shape to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the shape to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Returns a random point on the area's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the area's perimeter</returns>
    public Vector3 RandomPointOnPerimeter(IRandom randomNumberGenerator) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>The three corner points of the triangle</summary>
    public Vector3 A, B, C;

  }

} // namespace Nuclex.Geometry.Areas
