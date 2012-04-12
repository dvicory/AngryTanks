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

  /// <summary>Flat rectangle situated in 3D space</summary>
  public class Rectangle3 : IArea3 {

    /// <summary>Surface area that the shape contains</summary>
    public float Area {
      get {
        throw new NotImplementedException("Not implemented yet");
      }
    }

    /// <summary>The total length of the area's circumference</summary>
    public float CircumferenceLength {
      get {
        throw new NotImplementedException("Not implemented yet");
      }
    }

    /// <summary>The center of mass within the shape</summary>
    public Vector3 CenterOfMass {
      get {
        throw new NotImplementedException("Not implemented yet");
      }
    }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    public Volumes.AxisAlignedBox3 BoundingBox {
      get {
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
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Areas
