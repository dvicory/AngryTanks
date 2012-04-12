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

  /// <summary>Contains all Aabb-to-Cylinder interference detection code</summary>
  public static class AabbCylinderCollider {

    /// <summary>Test whether an axis aligned box and a cylinder are overlapping</summary>
    /// <param name="aabbMin">Minimum coordinate of the axis aligned box</param>
    /// <param name="aabbMax">Maximum coordinate of the axis aligned box</param>
    /// <param name="cylinderTransform">Center and orientation of the cylinder</param>
    /// <param name="cylinderLength">Length of the cylinder</param>
    /// <param name="cylinderRadius">Radius of the cylinder</param>
    /// <returns>True if the cylinder and the box are intersecting each other</returns>
    public static bool CheckContact(
      Vector3 aabbMin, Vector3 aabbMax,
      Matrix cylinderTransform, float cylinderLength, float cylinderRadius
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Find the contact location between a cylinder and axis aligned box</summary>
    /// <param name="aabbMin">Minimum coordinate of the axis aligned box</param>
    /// <param name="aabbMax">Maximum coordinate of the axis aligned box</param>
    /// <param name="cylinderTransform">Center and orientation of the cylinder</param>
    /// <param name="cylinderLength">Length of the cylinder</param>
    /// <param name="cylinderRadius">Radius of the cylinder</param>
    /// <returns>A contact location if the cylinder touches the axis aligned box</returns>
    public static Vector3? FindContact(
      Vector3 aabbMin, Vector3 aabbMax,
      Matrix cylinderTransform, float cylinderLength, float cylinderRadius
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
