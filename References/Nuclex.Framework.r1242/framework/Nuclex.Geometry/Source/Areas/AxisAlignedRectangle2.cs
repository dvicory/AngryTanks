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

  /// <summary>Two-dimensional axis aligned rectangle</summary>
  public class AxisAlignedRectangle2 : IArea2 {

    /// <summary>Initializes the axis aligned rectangle</summary>
    /// <param name="min">Lower left bounds of the rectangle</param>
    /// <param name="max">Upper right bounds of the rectangle</param>
    [DebuggerStepThrough]
    public AxisAlignedRectangle2(Vector2 min, Vector2 max) {
      Min = min;
      Max = max;
    }

    /// <summary>The width of the rectangle</summary>
    public float Width {
      get { return this.Max.X - this.Min.X; }
    }

    /// <summary>The height of the rectangle</summary>
    public float Height {
      get { return this.Max.Y - this.Min.Y; }
    }

    /// <summary>Surface area that the shape contains</summary>
    public float Area {
      get { return Width * Height; }
    }

    /// <summary>The total length of the area's circumference</summary>
    public float CircumferenceLength {
      get { return Width * 2 + Height * 2; }
    }

    /// <summary>The center of mass within the shape</summary>
    public Vector2 CenterOfMass {
      get { return (this.Min + this.Max) / 2.0f; }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public AxisAlignedRectangle2 BoundingBox {
      get { return this; }
    }

    /// <summary>Locates the nearest point in the area to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the area to the specified location</returns>
    public Vector2 ClosestPointTo(Vector2 location) {
      return new Vector2(
        Math.Min(Math.Max(location.X, Min.X), Max.X),
        Math.Min(Math.Max(location.Y, Min.Y), Max.Y)
      );
    }

    /// <summary>Returns a random point on the area's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the area's perimeter</returns>
    public Vector2 RandomPointOnPerimeter(IRandom randomNumberGenerator) {
      Vector2 center = (this.Max + this.Min) / 2.0f;
      Vector2 extents = (this.Max - center);

      return PointGenerators.Rectangle2PointGenerator.GenerateRandomPointOnPerimeter(
        randomNumberGenerator, this.Min, this.Max
      );
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector2 RandomPointWithin(IRandom randomNumberGenerator) {
      Vector2 center = (this.Max + this.Min) / 2.0f;
      Vector2 extents = (this.Max - center);

      return center + PointGenerators.Rectangle2PointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, extents
      );
    }

    /// <summary>Top left bounds of the box</summary>
    public Vector2 Min;
    /// <summary>Bottom right bounds of the box</summary>
    public Vector2 Max;

  }

} // namespace Nuclex.Geometry.Areas
