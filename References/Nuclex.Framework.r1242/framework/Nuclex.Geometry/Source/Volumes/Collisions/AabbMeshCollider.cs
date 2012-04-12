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

  /// <summary>Contains all Aabb-to-Mesh interference detection code</summary>
  public static class AabbMeshCollider {

    /// <summary>Test whether an axis aligned box and a mesh are overlapping</summary>
    /// <param name="aabbMin">Minimum coordinate of the axis aligned box</param>
    /// <param name="aabbMax">Maximum coordinate of the axis aligned box</param>
    /// <param name="triangleMesh">Mesh to be checked for overlap</param>
    /// <returns>True if the axis aligned box and the mesh are intersecting</returns>
    public static bool CheckContact(
      Vector3 aabbMin, Vector3 aabbMax, TriangleMesh3 triangleMesh
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Find the contact location between an axis aligned box and a mesh</summary>
    /// <param name="aabbMin">Minimum coordinate of the axis aligned box</param>
    /// <param name="aabbMax">Maximum coordinate of the axis aligned box</param>
    /// <param name="triangleMesh">Mesh to be checked for overlap</param>
    /// <returns>A contact location if the axis aligned box touches the mesh</returns>
    public static Vector3? FindContact(
      Vector3 aabbMin, Vector3 aabbMax, TriangleMesh3 triangleMesh
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
