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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Lines.Collisions {

  /// <summary>Contains all Line3 to Plane3 interference detection code</summary>
  public static class Line3Plane3Collider {

    /// <summary>Determines where a line will hit a plane, if at all</summary>
    /// <param name="lineOffset">
    ///   Offset of the line from the coordinate system's center
    /// </param>
    /// <param name="lineDirection">Direction and length of the line</param>
    /// <param name="planeOffset">
    ///   Offset of the plane from the coordinate system's center
    /// </param>
    /// <param name="planeNormal">Normal vector of the plane</param>
    /// <returns>
    ///   The intersection point between the line and the plane, if they touch
    /// </returns>
    public static LineContacts FindContacts(
      Vector3 lineOffset, Vector3 lineDirection,
      Vector3 planeOffset, Vector3 planeNormal
    ) {
      float dot = Vector3.Dot(planeNormal, lineDirection);

      // If the dot product is zero, the line is parallel to the plane and no contact occurs
      if(dot == 0.0) {
        return LineContacts.None;
      } else {
        return new LineContacts(
          -Vector3.Dot(planeNormal, lineOffset - planeOffset) / dot
        );
      }
    }

    #if SOLID_PLANES
    /// <summary>Determines where a line will hit a solid plane, if at all</summary>
    /// <param name="lineOffset">
    ///   Offset of the line from the coordinate system's center
    /// </param>
    /// <param name="lineDirection">Direction and length of the line</param>
    /// <param name="planeOffset">
    ///   Offset of the plane from the coordinate system's center
    /// </param>
    /// <param name="planeNormal">Normal vector of the plane</param>
    /// <returns>
    ///   The intersection point between the line and the plane, if they touch
    /// </returns>
    /// <remarks>
    ///   A solid plane is a plane that encompasses all of the space behind it (instead
    ///   of an infinitely thin layer).
    /// </remarks>
    public static LineContacts FindSolidPlaneContacts(
      Vector3 lineOffset, Vector3 lineDirection,
      Vector3 planeOffset, Vector3 planeNormal
    ) {
      float dot = Vector3.Dot(planeNormal, lineDirection);

      // If the dot product is zero, the line is parallel to the plane and no contact occurs
      if(dot == 0.0f) {
        return LineContacts.None;
      } else {
        float crossingPoint = -Vector3.Dot(planeNormal, lineOffset - planeOffset) / dot;
        if(dot > 0.0f) {
          return new LineContacts(crossingPoint, float.NaN);
        } else {
          return new LineContacts(float.NaN, crossingPoint);
        }          
      }
    }
    #endif // SOLID_PLANES

  }

} // namespace Nuclex.Geometry.Lines.Collisions
