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

  /// <summary>Contains all Ray3 to Plane3 interference detection code</summary>
  public static class Ray3Plane3Collider {

    /// <summary>Determines where a ray will hit a plane, if at all</summary>
    /// <param name="rayStart">Starting point of the ray</param>
    /// <param name="rayDirection">Direction into which the ray extends</param>
    /// <param name="planeOffset">Offset of the plane</param>
    /// <param name="planeNormal">Normal vector of the plane</param>
    /// <returns>The intersection points between the ray and the plane, if any</returns>
    public static LineContacts FindContacts(
      Vector3 rayStart, Vector3 rayDirection,
      Vector3 planeOffset, Vector3 planeNormal
    ) {
      LineContacts contacts = Line3Plane3Collider.FindContacts(
        rayStart, rayDirection, planeOffset, planeNormal
      );

      // If the contact occured before the starting offset of the ray,
      // no collision took place since we're a ray
      if(!float.IsNaN(contacts.EntryTime)) {
        if(contacts.EntryTime < 0.0f) {
          return LineContacts.None;
        }
      }

      return contacts;
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
