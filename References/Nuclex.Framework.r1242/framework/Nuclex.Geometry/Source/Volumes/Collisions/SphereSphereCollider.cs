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

namespace Nuclex.Geometry.Volumes.Collisions {

  /// <summary>Contains all Sphere-to-Sphere interference detection code</summary>
  public static class SphereSphereCollider {

    /// <summary>Test whether two spheres intersect with each other</summary>
    /// <param name="firstCenter">Center of the first sphere to test</param>
    /// <param name="firstRadius">Radius of the first sphere</param>
    /// <param name="secondCenter">Center of the second sphere to test</param>
    /// <param name="secondRadius">Radius of the second sphere</param>
    /// <returns>True if the spheres intersect each other</returns>
    public static bool CheckContact(
      Vector3 firstCenter, double firstRadius,
      Vector3 secondCenter, double secondRadius
    ) {
      double distance = (firstCenter - secondCenter).LengthSquared();
      double radii = firstRadius + secondRadius;

      // The spheres overlap if their combined radius is larger than the distance of
      // their centers
      return distance < (radii * radii);
    }

    /// <summary>Locate where two spheres intersect with each other</summary>
    /// <param name="firstCenter">Center of the first sphere to test</param>
    /// <param name="firstRadius">Radius of the first sphere</param>
    /// <param name="secondCenter">Center of the second sphere to test</param>
    /// <param name="secondRadius">Radius of the second sphere</param>
    /// <returns>A point in the center of the intersection, if any</returns>
    public static Vector3? FindContact(
      Vector3 firstCenter, double firstRadius,
      Vector3 secondCenter, double secondRadius
    ) {
      
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
