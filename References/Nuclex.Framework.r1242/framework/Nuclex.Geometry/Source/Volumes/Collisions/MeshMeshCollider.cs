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

  /// <summary>Contains all Mesh-to-Mesh interference detection code</summary>
  public static class MeshMeshCollider {

    /// <summary>Test whether a two meshes are overlapping</summary>
    /// <param name="firstMesh">First mesh to be checked for overlap</param>
    /// <param name="secondMesh">Second mesh to be checked for overlap</param>
    /// <returns>True if the two meshes are intersecting</returns>
    public static bool CheckContact(
      TriangleMesh3 firstMesh, TriangleMesh3 secondMesh
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Find the contact location between two meshes</summary>
    /// <param name="firstMesh">First mesh to be checked for overlap</param>
    /// <param name="secondMesh">Second mesh to be checked for overlap</param>
    /// <returns>A contact location if the two meshes touch each other</returns>
    public static Vector3? FindContact(
      TriangleMesh3 firstMesh, TriangleMesh3 secondMesh
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
