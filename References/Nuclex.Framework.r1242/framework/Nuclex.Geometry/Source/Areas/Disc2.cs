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

  /// <summary>A two-dimensional circle</summary>
  public class Disc2 : IArea2 {

    /// <summary>Initializes a new circle</summary>
    /// <param name="center">The center of the circle</param>
    /// <param name="radius">Radius the circle will have</param>
    [DebuggerStepThrough]
    public Disc2(Vector2 center, float radius) {
      this.Center = center;
      this.Radius = radius;
    }

    /// <summary>Surface area that the shape contains</summary>
    public float Area {
      get { return MathHelper.Pi * (this.Radius * this.Radius); }
    }

    /// <summary>The total length of the area's circumference</summary>
    public float CircumferenceLength {
      get { return 2.0f * MathHelper.Pi * this.Radius; }
    }

    /// <summary>The center of mass within the shape</summary>
    public Vector2 CenterOfMass {
      get { return Center; }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public AxisAlignedRectangle2 BoundingBox {
      get {
        return new AxisAlignedRectangle2(
          new Vector2(this.Center.X - this.Radius, this.Center.Y - this.Radius),
          new Vector2(this.Center.X + this.Radius, this.Center.Y + this.Radius)
        );
      }
    }

    /// <summary>Locates the nearest point on the disc to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point on the disc to the specified location</returns>
    public Vector2 ClosestPointTo(Vector2 location) {
      Vector2 offset = location - Center;
      float distance = offset.Length();

      if(distance > this.Radius) {
        float distanceFactor = this.Radius / offset.Length();
        return this.Center + offset * distanceFactor;
      } else {
        return location;
      }
    }

    /// <summary>Returns a random point on the area's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the area's perimeter</returns>
    public Vector2 RandomPointOnPerimeter(IRandom randomNumberGenerator) {
      return this.Center + PointGenerators.Disc2PointGenerator.GenerateRandomPointOnPerimeter(
        randomNumberGenerator, this.Radius
      );
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector2 RandomPointWithin(IRandom randomNumberGenerator) {
      return this.Center + PointGenerators.Disc2PointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, this.Radius
      );
    }

    // TODO: Use matrix. Solves elipsis. XNA Matrix is intended for both 2D and 3D.

    /// <summary>The center of the disc</summary>
    public Vector2 Center;
    /// <summary>Radius of the disc</summary>
    public float Radius;

  }

} // namespace Nuclex.Geometry.Areas
