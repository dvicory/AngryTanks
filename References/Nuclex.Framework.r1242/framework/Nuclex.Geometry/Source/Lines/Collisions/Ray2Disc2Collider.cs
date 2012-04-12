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

  /// <summary>Contains all Ray2 to Disc2 interference detection code</summary>
  public static class Ray2Disc2Collider {

    /// <summary>Determines where a ray will hit a disc, if at all</summary>
    /// <param name="rayStart">Starting point of the ray</param>
    /// <param name="rayDirection">Direction into which the ray extends</param>
    /// <param name="discRadius">Radius of the disc</param>
    /// <returns>The intersection points between the ray and the box, if any</returns>
    public static LineContacts FindContacts(
      Vector2 rayStart, Vector2 rayDirection, float discRadius
    ) {
      LineContacts contacts = Line2Disc2Collider.FindContacts(
        rayStart, rayDirection, discRadius
      );

      // If the line has entered the box before the reference point, this means
      // that the ray starts within the box, thus, its first contact occurs immediately
      if(!float.IsNaN(contacts.EntryTime)) {
        if(contacts.ExitTime < 0.0f) { // Entry & exit before the ray's beginning?
          return LineContacts.None;
        } else if(contacts.EntryTime < 0.0f) { // Only entry before ray's beginning?
          contacts.EntryTime = 0.0f;
        }
      }

      return contacts;
    }

    /// <summary>Determines where a ray will hit a disc, if at all</summary>
    /// <param name="rayStart">Starting point of the ray</param>
    /// <param name="rayDirection">Direction into which the ray extends</param>
    /// <param name="discCenter">Center of the disc</param>
    /// <param name="discRadius">Radius of the disc</param>
    /// <returns>The intersection points between the ray and the box, if any</returns>
    public static LineContacts FindContacts(
      Vector2 rayStart, Vector2 rayDirection, Vector2 discCenter, float discRadius
    ) {
      LineContacts contacts = Line2Disc2Collider.FindContacts(
        rayStart, rayDirection, discCenter, discRadius
      );

      // If the line has entered the box before the reference point, this means
      // that the ray starts within the box, thus, its first contact occurs immediately
      if(!float.IsNaN(contacts.EntryTime)) {
        if(contacts.ExitTime < 0.0f) { // Entry & exit before the ray's beginning?
          return LineContacts.None;
        } else if(contacts.EntryTime < 0.0f) { // Only entry before ray's beginning?
          contacts.EntryTime = 0.0f;
        }
      }

      return contacts;
    }

  }

} // namespace Nuclex.Geometry.Lines.Collisions
