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

  /// <summary>Flat disc situated in 3D space</summary>
  public class Disc3 : IArea3 {

    /// <summary>Initializes a new flat disc in 3D space</summary>
    /// <param name="transform">
    ///   Transformation matrix definining the disc's position and orientation
    /// </param>
    /// <param name="radius">Radius of the disc</param>
    [DebuggerStepThrough]
    public Disc3(Matrix transform, float radius) {
      this.Transform = transform;
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
    public Vector3 CenterOfMass {
      get { return this.Transform.Translation; }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public Volumes.AxisAlignedBox3 BoundingBox {
      get {
        //float maxX = Math.Abs(Math.Min(this.Transform.M11, this.Transform.M21));
        throw new NotImplementedException("Not implemented yet");
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
      //Vector3 
    
      //Vector2 randomPoint = PointGenerators.Disc2PointGenerator.GenerateRandomPointOnPerimeter(
      //  randomNumberGenerator, this.Radius
      //);
    
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Contains the position, orientation and scale of the disc</summary>
    public Matrix Transform;
    /// <summary>Radius of the disc</summary>
    public float Radius;

  }

} // namespace Nuclex.Geometry.Areas
