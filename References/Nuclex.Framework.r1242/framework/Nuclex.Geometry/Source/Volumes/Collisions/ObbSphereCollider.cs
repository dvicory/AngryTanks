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

  /// <summary>Contains all Obb-to-Sphere interference detection code</summary>
  public static class ObbSphereCollider {

    /// <summary>Test whether an oriented box and a sphere intersect each other</summary>
    /// <param name="boxTransform">Orientation and position of the box</param>
    /// <param name="boxExtents">Extents of the box to be tested</param>
    /// <param name="sphereCenter">Center of the sphere relative to the box' center</param>
    /// <param name="sphereRadius">Radius of the sphere</param>
    /// <returns>True if the sphere overlaps with the oriented box</returns>
    public static bool CheckContact(
      Matrix boxTransform, Vector3 boxExtents, 
      Vector3 sphereCenter, float sphereRadius
    ) {

      // Translate the sphere into box coordinates  
      Vector3 local = new Vector3(
        Vector3.Dot(sphereCenter, boxTransform.Right),
        Vector3.Dot(sphereCenter, boxTransform.Up),
        Vector3.Dot(sphereCenter, boxTransform.Forward)
      );

      // Now it's a simple aabb check
      return AabbSphereCollider.CheckContact(
        -boxExtents, boxExtents, sphereCenter, sphereRadius
      );

    }

    /// <summary>Find the contact location between an OBB and a sphere</summary>
    /// <param name="boxTransform">Orientation and position of the box</param>
    /// <param name="boxExtents">Extents of the box to be tested</param>
    /// <param name="sphereCenter">Center of the sphere relative to the box' center</param>
    /// <param name="sphereRadius">Radius of the sphere</param>
    /// <returns>A contact location if the OBB and the sphere are intersecting</returns>
    public static Vector3? FindContact(
      Matrix boxTransform, Vector3 boxExtents,
      Vector3 sphereCenter, float sphereRadius
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
