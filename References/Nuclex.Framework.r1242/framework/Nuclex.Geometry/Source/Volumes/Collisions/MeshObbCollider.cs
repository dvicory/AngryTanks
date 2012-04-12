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

  /// <summary>Contains all Mesh-to-Obb interference detection code</summary>
  public static class MeshObbCollider {

    /// <summary>Test whether a mesh and an OBB are overlapping</summary>
    /// <param name="triangleMesh">Mesh to be checked for intersection</param>
    /// <param name="obbTransform">Position and orientation of the OBB</param>
    /// <param name="obbExtents">Extents of the OBB</param>
    /// <returns>True if the mesh and the OBB are intersecting</returns>
    public static bool CheckContact(
      TriangleMesh3 triangleMesh, Matrix obbTransform, Vector3 obbExtents
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Find the contact location between a mesh and an OBB</summary>
    /// <param name="triangleMesh">Mesh to be checked for intersection</param>
    /// <param name="obbTransform">Position and orientation of the OBB</param>
    /// <param name="obbExtents">Extents of the OBB</param>
    /// <returns>A contact location if the mesh touches the OBB</returns>
    public static Vector3? FindContact(
      TriangleMesh3 triangleMesh, Matrix obbTransform, Vector3 obbExtents
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
