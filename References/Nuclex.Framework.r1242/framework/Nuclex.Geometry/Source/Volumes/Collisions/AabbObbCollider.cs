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

  /// <summary>Contains all Aabb-to-Obb interference detection code</summary>
  public static class AabbObbCollider {

    /// <summary>Checks an AABB for intersection with an OBB</summary>
    /// <param name="aabbExtents">Extents of the AABB</param>
    /// <param name="obbTransform">Orientation and position of the OBB</param>
    /// <param name="obbExtents">Extents of the OBB</param>
    /// <returns>True if the two boxes are overlapping</returns>
    /// <remarks>
    ///   <para>
    ///     This method is a helper method for the other intersection checks. It assumes the
    ///     AABB is sitting right in the center of the coordinate frame. In other words,
    ///     that the OBB has been transformed into the AABB's local coordinate frame.
    ///   </para>
    ///   <para>
    ///     Idea taken from the "Simple Intersection Tests for Games" article
    ///     on gamasutra by Gomez. The algorithm uses the separating axis test for
    ///     all 15 potential separating axes. If a separating axis can not be found,
    ///     the two boxes are overlapping.
    ///     (http://www.gamasutra.com/features/19991018/Gomez_1.htm)
    ///   </para>
    /// </remarks>
    public static bool CheckContact(
      Vector3 aabbExtents, Matrix obbTransform, Vector3 obbExtents
    ) {
      double ra, rb, t;

      // A's basis vectors
      for(int i = 0; i < 3; ++i) {
        ra = VectorHelper.Get(ref aabbExtents, i);
        rb =
          obbExtents.X * Math.Abs(MatrixHelper.Get(ref obbTransform, i, 0)) +
          obbExtents.Y * Math.Abs(MatrixHelper.Get(ref obbTransform, i, 1)) +
          obbExtents.Z * Math.Abs(MatrixHelper.Get(ref obbTransform, i, 2));

        //t = Math.Abs(VectorHelper.Get(ref obbPosition, i));
        t = Math.Abs(MatrixHelper.Get(ref obbTransform, 3, i));

        if(t > ra + rb)
          return false;
      }

      // B's basis vectors
      for(int k = 0; k < 3; ++k) {
        ra =
          aabbExtents.X * Math.Abs(MatrixHelper.Get(ref obbTransform, 0, k)) +
          aabbExtents.Y * Math.Abs(MatrixHelper.Get(ref obbTransform, 1, k)) +
          aabbExtents.Z * Math.Abs(MatrixHelper.Get(ref obbTransform, 2, k));

        rb = VectorHelper.Get(ref obbExtents, k);

        t = Math.Abs(
          obbTransform.M41 * MatrixHelper.Get(ref obbTransform, 0, k) +
          obbTransform.M42 * MatrixHelper.Get(ref obbTransform, 1, k) +
          obbTransform.M43 * MatrixHelper.Get(ref obbTransform, 2, k)
        );

        if(t > ra + rb)
          return false;
      }

      // L = A0 x B0
      ra =
        aabbExtents.Y * Math.Abs(obbTransform.M31) +
        aabbExtents.Z * Math.Abs(obbTransform.M21);
      rb =
        obbExtents.Y * Math.Abs(obbTransform.M13) +
        obbExtents.Z * Math.Abs(obbTransform.M12);
      t = Math.Abs(
        obbTransform.M43 * obbTransform.M21 - obbTransform.M42 * obbTransform.M31
      );
      if(t > ra + rb)
        return false;

      // L = A0 x B1
      ra =
        aabbExtents.Y * Math.Abs(obbTransform.M32) +
        aabbExtents.Z * Math.Abs(obbTransform.M22);
      rb =
        obbExtents.X * Math.Abs(obbTransform.M13) +
        obbExtents.Z * Math.Abs(obbTransform.M11);
      t = Math.Abs(
        obbTransform.M43 * obbTransform.M22 - obbTransform.M42 * obbTransform.M32
      );
      if(t > ra + rb)
        return false;

      // L = A0 x B2
      ra =
        aabbExtents.Y * Math.Abs(obbTransform.M33) +
        aabbExtents.Z * Math.Abs(obbTransform.M23);
      rb =
        obbExtents.X * Math.Abs(obbTransform.M12) +
        obbExtents.Y * Math.Abs(obbTransform.M11);
      t = Math.Abs(
        obbTransform.M43 * obbTransform.M23 - obbTransform.M42 * obbTransform.M33
      );
      if(t > ra + rb)
        return false;

      // L = A1 x B0
      ra =
        aabbExtents.X * Math.Abs(obbTransform.M31) +
        aabbExtents.Z * Math.Abs(obbTransform.M11);
      rb =
        obbExtents.Y * Math.Abs(obbTransform.M23) +
        obbExtents.Z * Math.Abs(obbTransform.M22);
      t = Math.Abs(
        obbTransform.M41 * obbTransform.M31 - obbTransform.M43 * obbTransform.M11
      );
      if(t > ra + rb)
        return false;

      // L = A1 x B1
      ra =
        aabbExtents.X * Math.Abs(obbTransform.M32) +
        aabbExtents.Z * Math.Abs(obbTransform.M12);
      rb =
        obbExtents.X * Math.Abs(obbTransform.M23) +
        obbExtents.Z * Math.Abs(obbTransform.M21);
      t = Math.Abs(
        obbTransform.M41 * obbTransform.M32 - obbTransform.M43 * obbTransform.M12
      );
      if(t > ra + rb)
        return false;

      // L = A1 x B2
      ra =
        aabbExtents.X * Math.Abs(obbTransform.M33) +
        aabbExtents.Z * Math.Abs(obbTransform.M13);
      rb =
        obbExtents.X * Math.Abs(obbTransform.M22) +
        obbExtents.Y * Math.Abs(obbTransform.M21);
      t = Math.Abs(
        obbTransform.M41 * obbTransform.M33 - obbTransform.M43 * obbTransform.M13
      );
      if(t > ra + rb)
        return false;

      // L = A2 x B0
      ra =
        aabbExtents.X * Math.Abs(obbTransform.M21) +
        aabbExtents.Y * Math.Abs(obbTransform.M11);
      rb =
        obbExtents.Y * Math.Abs(obbTransform.M33) +
        obbExtents.Z * Math.Abs(obbTransform.M32);
      t = Math.Abs(
        obbTransform.M42 * obbTransform.M11 - obbTransform.M41 * obbTransform.M21
      );
      if(t > ra + rb)
        return false;

      // L = A2 x B1
      ra =
        aabbExtents.X * Math.Abs(obbTransform.M22) +
        aabbExtents.Y * Math.Abs(obbTransform.M12);
      rb =
        obbExtents.X * Math.Abs(obbTransform.M33) +
        obbExtents.Z * Math.Abs(obbTransform.M31);
      t = Math.Abs(
        obbTransform.M42 * obbTransform.M12 - obbTransform.M41 * obbTransform.M22
      );
      if(t > ra + rb)
        return false;

      // L = A2 x B2
      ra =
        aabbExtents.X * Math.Abs(obbTransform.M23) +
        aabbExtents.Y * Math.Abs(obbTransform.M13);
      rb =
        obbExtents.X * Math.Abs(obbTransform.M32) +
        obbExtents.Y * Math.Abs(obbTransform.M31);
      t = Math.Abs(
        obbTransform.M42 * obbTransform.M13 - obbTransform.M41 * obbTransform.M23
      );
      if(t > ra + rb)
        return false;

      // No separating axis found, the two boxes overlap
      return true;
    }

    /// <summary>Find the contact location between an AABB and an OBB</summary>
    /// <param name="aabbMin">Minimum coordinates of the AABB</param>
    /// <param name="aabbMax">Maximum coordinates of the AABB</param>
    /// <param name="obbTransform">Orientation and position of the OBB</param>
    /// <param name="obbExtents">Extents of the OBB</param>
    /// <returns>A contact location if the axis aligned box touches the mesh</returns>
    public static Vector3? FindContact(
      Vector3 aabbMin, Vector3 aabbMax, Matrix obbTransform, Vector3 obbExtents
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
