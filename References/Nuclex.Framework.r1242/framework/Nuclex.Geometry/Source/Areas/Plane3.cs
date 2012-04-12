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

  /// <summary>Two-dimensional Plane</summary>
  public class Plane3 : IArea3 {

    /// <summary>Initializes a new Plane</summary>
    /// <param name="offset">Offset of the plane from the coordinate system's center</param>
    /// <param name="normal">Vector that is exactly perpendicular to the plane</param>
    [DebuggerStepThrough]
    public Plane3(Vector3 offset, Vector3 normal) {
      this.Offset = offset;
      this.Normal = normal;
    }

    /// <summary>Initializes a new Plane</summary>
    /// <param name="distance">Distance of the plane from the coordinate system's center</param>
    /// <param name="normal">Vector that is exactly perpendicular to the plane</param>
    [DebuggerStepThrough]
    public Plane3(float distance, Vector3 normal) : this(normal * distance, normal) { }

    /// <summary>Determines which side of a plane a point is on</summary>
    /// <param name="planeOffset">Offset of the plane</param>
    /// <param name="planeNormal">Normal vector of the plane</param>
    /// <param name="point">Location of the point to be checked</param>
    /// <returns>Which side of the plane the point is on</returns>
    public static Side GetSide(Vector3 planeOffset, Vector3 planeNormal, Vector3 point) {
      float side = Vector3.Dot(point - planeOffset, planeNormal);

      // We don't do neutral. The plane is infinitely thin, so the chance of hitting
      // the plane with an infinitely small point is about 0. The point is either
      // in front of the plane or behind the plane.
      if(side >= 0.0) {
        return Side.Positive;
      } else {
        return Side.Negative;
      }
    }

    /// <summary>Surface area that the shape contains</summary>
    public float Area {
      get { return float.PositiveInfinity; }
    }

    /// <summary>The total length of the area's circumference</summary>
    public float CircumferenceLength {
      get { return float.PositiveInfinity; }
    }

    /// <summary>The center of mass within the shape</summary>
    public Vector3 CenterOfMass {
      get { return this.Offset; }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public Volumes.AxisAlignedBox3 BoundingBox {
      get {

        // A plane perfectly resting on the Y/Z axes has zero thickness on the X axis
        if(this.Normal.X == 1.0f) {
          return new Volumes.AxisAlignedBox3(
            new Vector3(this.Offset.X, float.NegativeInfinity, float.NegativeInfinity),
            new Vector3(this.Offset.X, float.PositiveInfinity, float.PositiveInfinity)
          );
        }

        // A plane perfectly resting on the X/Z axes has zero thickness on the Y axis
        if(this.Normal.Y == 1.0f) {
          return new Volumes.AxisAlignedBox3(
            new Vector3(float.NegativeInfinity, this.Offset.Y, float.NegativeInfinity),
            new Vector3(float.PositiveInfinity, this.Offset.Y, float.PositiveInfinity)
          );
        }

        // A plane perfectly resting on the X/Y axes has zero thickness on the Z axis
        if(this.Normal.Z == 1.0f) {
          return new Volumes.AxisAlignedBox3(
            new Vector3(float.NegativeInfinity, float.NegativeInfinity, this.Offset.Z),
            new Vector3(float.PositiveInfinity, float.PositiveInfinity, this.Offset.Z)
          );
        }

        // Any plane not perfectly aligned to two coordinate axes can only fit into a
        // bounding box of infinite size in all 3 dimensions.
        return new Volumes.AxisAlignedBox3(
          new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity),
          new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)
        );

      }
    }

    /// <summary>Locates the nearest point in the shape to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the shape to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {
      float distance = Vector3.Dot(this.Normal, location - this.Offset);
      return location - distance * this.Normal;
    }

    /// <summary>Returns a random point on the area's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the area's perimeter</returns>
    public Vector3 RandomPointOnPerimeter(IRandom randomNumberGenerator) {
      Vector3 planePoint = PointGenerators.Plane3PointGenerator.RandomPointOnPerimeter(
        randomNumberGenerator,
        this.Normal
      );

      return planePoint + this.Offset;
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      Vector3 planePoint = PointGenerators.Plane3PointGenerator.RandomPointWithin(
        randomNumberGenerator,
        this.Normal
      );

      return planePoint + this.Offset;
    }

    /// <summary>Offset of the plane from the coordinate system's center</summary>
    public Vector3 Offset;
    /// <summary>Direction that is exactly perpendicular to the plane</summary>
    public Vector3 Normal;

  }

} // namespace Nuclex.Geometry.Areas
