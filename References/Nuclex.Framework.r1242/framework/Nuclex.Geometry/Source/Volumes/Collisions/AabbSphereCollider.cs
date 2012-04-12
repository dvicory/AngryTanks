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

  /// <summary>Contains all Aabb-to-Sphere interference detection code</summary>
  public static class AabbSphereCollider {

    /// <summary>Test whether a sphere and an axis aligned box intersect each other</summary>
    /// <param name="aabbMin">Minimum coordinate of the axis aligned box to test</param>
    /// <param name="aabbMax">Maximum coordinate of the axis aligned box to test</param>
    /// <param name="sphereCenter">Center to the sphere to test</param>
    /// <param name="sphereRadius">Radius to the sphere to test</param>
    /// <returns>True if the axis aligned box intersects with the sphere</returns>
    /// <remarks>
    ///   Idea taken from the "Simple Intersection Tests for Games" article
    ///   on gamasutra by Gomez.
    /// </remarks>
    public static bool CheckContact(
      Vector3 aabbMin, Vector3 aabbMax, Vector3 sphereCenter, float sphereRadius
    ) {
      float totalDistance = 0;
      
      // Accumulate the distance of the sphere's center on each axis
      for(int i = 0;i < 3;++i) {

        // If the sphere's center is outside the aabb, we've got distance on this axis
        if(VectorHelper.Get(ref sphereCenter, i) < VectorHelper.Get(ref aabbMin, i)) {
          float borderDistance =
            VectorHelper.Get(ref aabbMin, i) - VectorHelper.Get(ref sphereCenter, i);

          totalDistance += borderDistance * borderDistance;

        } else if(VectorHelper.Get(ref sphereCenter, i) > VectorHelper.Get(ref aabbMax, i)) {
          float borderDistance =
            VectorHelper.Get(ref sphereCenter, i) - VectorHelper.Get(ref aabbMax, i);

          totalDistance += borderDistance * borderDistance;

        }
        // Otherwise the sphere's center is within the box on this axis, so the
        // distance will be 0 and we do not need to accumulate anything at all

      }

      // If the distance to the box is lower than the sphere's radius, both are overlapping
      return (totalDistance <= (sphereRadius * sphereRadius));
    }

    /// <summary>Find the contact location between an axis aligned box and a mesh</summary>
    /// <param name="aabbMin">Minimum coordinate of the axis aligned box to test</param>
    /// <param name="aabbMax">Maximum coordinate of the axis aligned box to test</param>
    /// <param name="sphereCenter">Center to the sphere to test</param>
    /// <param name="sphereRadius">Radius to the sphere to test</param>
    /// <returns>A contact location if the axis aligned box touches the mesh</returns>
    public static Vector3? FindContact(
      Vector3 aabbMin, Vector3 aabbMax, Vector3 sphereCenter, float sphereRadius
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
