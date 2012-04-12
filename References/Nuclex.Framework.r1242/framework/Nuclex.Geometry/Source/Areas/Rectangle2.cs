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

  /// <summary>Two-dimensional box with arbitrary orientation</summary>
  public class Rectangle2 : IArea2 {

    /// <summary>Surface area that the shape contains</summary>
    public float Area {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    /// <summary>The total length of the area's circumference</summary>
    public float CircumferenceLength {
      get { return (Width * 2.0f) + (Height * 2.0f); }
    }

    /// <summary>The center of mass within the shape</summary>
    public Vector2 CenterOfMass {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public AxisAlignedRectangle2 BoundingBox {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    /// <summary>Locates the nearest point in the area to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the area to the specified location</returns>
    public Vector2 ClosestPointTo(Vector2 location) {
      throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>Width of the rectangle</summary>
    public float Width {
      get {
        float rightX = this.Transform.M11;
        float rightY = this.Transform.M12;
        return (rightX * rightX) + (rightY * rightY);
      }
    }

    /// <summary>Height of the rectangle</summary>
    public float Height {
      get {
        float upX = this.Transform.M21;
        float upY = this.Transform.M22;
        return (upX * upX) + (upY * upY);
      }
    }

    /// <summary>Returns a random point on the area's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the area's perimeter</returns>
    public Vector2 RandomPointOnPerimeter(IRandom randomNumberGenerator) {
      // TODO: Extend Rectangle2PointGenerator to work with matrices, too
      /*
      float totalCircumference = CircumferenceLength;
      float randomPoint = (float)randomNumberGenerator.NextDouble() * totalCircumference;

      Vector2 randomPoint
      if(
      x*x + y*y
      float xScale = this.Transform.M11 
      */
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector2 RandomPointWithin(IRandom randomNumberGenerator) {
      Vector2 randomPoint = new Vector2(
        (float)randomNumberGenerator.NextDouble(),
        (float)randomNumberGenerator.NextDouble()
      );

      Vector2 result;
      Vector2.Transform(ref randomPoint, ref this.Transform, out result);
      return result;
    }

    /// <summary>Orientation, position and scale of the rectangle</summary>
    private Matrix Transform;

  }

} // namespace Nuclex.Geometry.Areas
