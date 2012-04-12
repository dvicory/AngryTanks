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

  /// <summary>Contains all Obb-to-Obb interference detection code</summary>
  public static class ObbObbCollider {

    /// <summary>Test whether two oriented boxes intersect with each other</summary>
    /// <param name="firstBoxTransform">Orientation and position of the first box</param>
    /// <param name="firstBoxExtents">Extents of the first box to test</param>
    /// <param name="secondBoxTransform">Orientation and position of the second box</param>
    /// <param name="secondBoxExtents">Extents of the second box to test</param>
    /// <returns>True if the oriented boxes are overlapping</returns>
    public static bool CheckContact(
      Matrix firstBoxTransform, Vector3 firstBoxExtents,
      Matrix secondBoxTransform, Vector3 secondBoxExtents
    ) {
/*
      // We rotate the second box into the local coordinate frame of the first box so
      // this check becomes an AABB to OBB test

      // We transform the second box position to the coordinate frame of the first box
      Vector3 local = new Vector3(
        Vector3.Dot(secondBoxCenter, firstBoxOrientation.Right),
        Vector3.Dot(secondBoxCenter, firstBoxOrientation.Up),
        Vector3.Dot(secondBoxCenter, firstBoxOrientation.Forward)
      );

      Matrix relativeOrientation = firstBoxOrientation * Matrix.Invert(secondBoxOrientation);

      // Next, we rotate the orientation matrix of the second box to obtain the relative
      // orientation in the coordinate frame of the first box
      Matrix relativeOrientation = new Matrix();
      for(int row = 0; row < 3; ++row) {
        for(int col = 0; col < 3; ++col) {
          relativeOrientation[row, col] =
            Vector3.Dot(secondBoxOrientation[row], firstBoxOrientation[col]);
        }
      }

      // Now we can do an easier axis aligned box to oriented box intersection check
      return Collisions.AabbObbCollider.CheckContact(
        firstBoxExtents, secondBoxExtents, local, relativeOrientation
      );
*/
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Find the contact location between two oriented boxes</summary>
    /// <param name="firstBoxTransform">Orientation and position of the first box</param>
    /// <param name="firstBoxExtents">Extents of the first box to test</param>
    /// <param name="secondBoxTransform">Orientation and position of the second box</param>
    /// <param name="secondBoxExtents">Extents of the second box to test</param>
    /// <returns>A contact location if the two OBBs are touching each other</returns>
    public static Vector3? FindContact(
      Matrix firstBoxTransform, Vector3 firstBoxExtents,
      Matrix secondBoxTransform, Vector3 secondBoxExtents
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
