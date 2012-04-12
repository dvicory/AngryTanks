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

  /// <summary>Contains all Cylinder-to-Cylinder interference detection code</summary>
  public static class CylinderCylinderCollider {

    /// <summary>Test whether an axis aligned box and a cylinder are overlapping</summary>
    /// <param name="firstTransform">Center and orientation of the first cylinder</param>
    /// <param name="firstLength">Length of the first cylinder</param>
    /// <param name="firstRadius">Radius of the first cylinder</param>
    /// <param name="secondTransform">Center and orientation of the second cylinder</param>
    /// <param name="secondLength">Length of the second cylinder</param>
    /// <param name="secondRadius">Radius of the second cylinder</param>
    /// <returns>True if the cylinder and the box are intersecting each other</returns>
    public static bool CheckContact(
      Matrix firstTransform, float firstLength, float firstRadius,
      Matrix secondTransform, float secondLength, float secondRadius
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Find the contact location between a cylinder and axis aligned box</summary>
    /// <param name="firstTransform">Center and orientation of the first cylinder</param>
    /// <param name="firstLength">Length of the first cylinder</param>
    /// <param name="firstRadius">Radius of the first cylinder</param>
    /// <param name="secondTransform">Center and orientation of the second cylinder</param>
    /// <param name="secondLength">Length of the second cylinder</param>
    /// <param name="secondRadius">Radius of the second cylinder</param>
    /// <returns>A contact location if the cylinder touches the axis aligned box</returns>
    public static Vector3? FindContact(
      Matrix firstTransform, float firstLength, float firstRadius,
      Matrix secondTransform, float secondLength, float secondRadius
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
