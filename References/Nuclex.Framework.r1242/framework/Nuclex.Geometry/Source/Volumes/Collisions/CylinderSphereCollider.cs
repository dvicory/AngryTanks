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

  /// <summary>Contains all Cylinder-to-Sphere interference detection code</summary>
  public static class CylinderSphereCollider {

    /// <summary>Tests whether a cylinder is touching a sphere</summary>
    /// <param name="cylinderTransform">Orientation and position of the cylinder</param>
    /// <param name="cylinderRadius">The cylinder's radius</param>
    /// <param name="cylinderLength">The cylinder's length</param>
    /// <param name="sphereCenter">Location of the sphere's center</param>
    /// <param name="sphereRadius">The sphere's radius</param>
    /// <returns>True if the objects are touching each other</returns>
    public static bool CheckContact(
      Matrix cylinderTransform, float cylinderRadius, float cylinderLength,
      Vector3 sphereCenter, float sphereRadius
    ) {

      // Determine the sphere's center in the cylinder's relative orientation
      Vector3 localSphereCenter = new Vector3(
        Vector3.Dot(sphereCenter, cylinderTransform.Right),
        Vector3.Dot(sphereCenter, cylinderTransform.Up),
        Vector3.Dot(sphereCenter, cylinderTransform.Forward)
      );

      Vector3 closest = Cylinder3.GetClosestPoint(
        cylinderRadius, cylinderLength, localSphereCenter
      );

      // If the closest point is within the sphere's radius, we've got a collision
      return (closest - localSphereCenter).LengthSquared() < (sphereRadius * sphereRadius);
    }

    /// <summary>Find the contact location between a cylinder and a sphere</summary>
    /// <param name="cylinderTransform">Orientation and position of the cylinder</param>
    /// <param name="cylinderRadius">The cylinder's radius</param>
    /// <param name="cylinderLength">The cylinder's length</param>
    /// <param name="sphereCenter">Location of the sphere's center</param>
    /// <param name="sphereRadius">The sphere's radius</param>
    /// <returns>A contact location if the cylinder touches the sphere</returns>
    public static Vector3? FindContact(
      Matrix cylinderTransform, float cylinderRadius, float cylinderLength,
      Vector3 sphereCenter, float sphereRadius
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
