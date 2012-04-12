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

  /// <summary>Contains all Cylinder-to-Obb interference detection code</summary>
  public static class CylinderObbCollider {

    /// <summary>Test whether a cylinder and an OBB are overlapping</summary>
    /// <param name="cylinderTransform">Center and orientation of the cylinder</param>
    /// <param name="cylinderLength">Length of the cylinder</param>
    /// <param name="cylinderRadius">Radius of the cylinder</param>
    /// <param name="obbTransform">Position and orientation of the OBB</param>
    /// <param name="obbExtents">Extents of the OBB</param>
    /// <returns>True if the cylinder and the OBB are intersecting</returns>
    public static bool CheckContact(
      Matrix cylinderTransform, float cylinderLength, float cylinderRadius,
      Matrix obbTransform, Vector3 obbExtents 
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Find the contact location between a cylinder and an OBB</summary>
    /// <param name="cylinderTransform">Center and orientation of the cylinder</param>
    /// <param name="cylinderLength">Length of the cylinder</param>
    /// <param name="cylinderRadius">Radius of the cylinder</param>
    /// <param name="obbTransform">Position and orientation of the OBB</param>
    /// <param name="obbExtents">Extents of the OBB</param>
    /// <returns>A contact location if the cylinder touches the OBB</returns>
    public static Vector3? FindContact(
      Matrix cylinderTransform, float cylinderLength, float cylinderRadius,
      Matrix obbTransform, Vector3 obbExtents 
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
