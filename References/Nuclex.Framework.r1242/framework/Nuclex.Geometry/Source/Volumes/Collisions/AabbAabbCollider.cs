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

  /// <summary>Contains all Aabb-to-Aabb interference detection code</summary>
  public static class AabbAabbCollider {

    /// <summary>Test whether two axis aligned boxes are overlapping</summary>
    /// <param name="firstMin">Minimum coordinate of first box</param>
    /// <param name="firstMax">Maximum coordinate of first box</param>
    /// <param name="secondMin">Minimum coordinate of second box</param>
    /// <param name="secondMax">Maximum coordinate of second box</param>
    /// <returns>True if the boxes are intersecting each other</returns>
    public static bool CheckContact(
      Vector3 firstMin, Vector3 firstMax,
      Vector3 secondMin, Vector3 secondMax
    ) {
      return
        (firstMin.X < secondMax.X) && (firstMax.X > secondMin.X) &&
        (firstMin.Y < secondMax.Y) && (firstMax.Y > secondMin.Y) &&
        (firstMin.Z < secondMax.Z) && (firstMax.Z > secondMin.Z);
    }

    /// <summary>Find the contact location between two axis aligned boxes</summary>
    /// <param name="firstMin">Minimum coordinate of first box</param>
    /// <param name="firstMax">Maximum coordinate of first box</param>
    /// <param name="secondMin">Minimum coordinate of second box</param>
    /// <param name="secondMax">Maximum coordinate of second box</param>
    /// <returns>A contact location if the boxes touch each other</returns>
    public static Vector3? FindContact(
      Vector3 firstMin, Vector3 firstMax,
      Vector3 secondMin, Vector3 secondMax
    ) {
      // Extract the intersecting area of the two boxes
      Vector3 min = Vector3.Max(firstMin, secondMin);
      Vector3 max = Vector3.Min(firstMax, secondMax);

      // If the boxes don't touch we don't have an intersection
      if((max.X < min.X) || (max.Y < min.Y) || (max.Z < min.Z))
        return null;

      // The contact point is in the center of the intersecting area
      return (min + max) / 2.0f;
    }

    /// <summary>Determines the time when the box will hit another box</summary>
    /// <param name="firstMin">Minimum coordinate of first box</param>
    /// <param name="firstMax">Maximum coordinate of first box</param>
    /// <param name="secondMin">Minimum coordinate of second box</param>
    /// <param name="secondMax">Maximum coordinate of second box</param>
    /// <param name="secondVelocity">
    ///   Velocity with which the second box is moving relative to the first box
    /// </param>
    /// <returns>The point of first contact, if any</returns>
    /// <remarks>
    ///   <para>
    ///     Conventional tests that resort to stepping often fail to detect collisions
    ///     between fast-moving objects. This impact determination test will always
    ///     detect a collision if it occurs, giving the exact time of the impact.
    ///   </para>
    ///   <para>
    ///     This is a simplified test that assumes a linear trajectory and does
    ///     not take object rotation into account. It is well suited to use on
    ///     two bounding boxes in order to determine if a collision between the
    ///     shapes contained is possible at all.
    ///   </para>
    ///   <para>
    ///     Idea taken from the "Simple Intersection Tests for Games" article
    ///     on gamasutra by Gomez.
    ///     (http://www.gamasutra.com/features/19991018/Gomez_1.htm)
    ///   </para>
    /// </remarks>
    internal static float? FindContact(
      Vector3 firstMin, Vector3 firstMax,
      Vector3 secondMin, Vector3 secondMax, Vector3 secondVelocity
    ) {
      Vector3 secondContact = new Vector3(0.0f, 0.0f, 0.0f);
      Vector3 lastContact = new Vector3(1.0f, 1.0f, 1.0f);

      // This could be done in a loop if the Vector3 class provided an indexing
      // operator for accessing the vector components. Sadly, as of XNA 1.1,
      // this is not the case, so that leaves us with this mess :)

      // X axis
      if(firstMax.X < secondMin.X && secondVelocity.X < 0.0f)
        secondContact.X = (firstMax.X - secondMin.X) / secondVelocity.X;
      else if(secondMax.X < firstMin.X && secondVelocity.X > 0.0f)
        secondContact.X = (firstMin.X - secondMax.X) / secondVelocity.X;

      if(secondMax.X > firstMin.X && secondVelocity.X < 0.0f)
        lastContact.X = (firstMin.X - secondMax.X) / secondVelocity.X;
      else if(firstMax.X > secondMin.X && secondVelocity.X > 0.0f)
        lastContact.X = (firstMax.X - secondMin.X) / secondVelocity.X;

      // Y axis
      if(firstMax.Y < secondMin.Y && secondVelocity.Y < 0.0f)
        secondContact.Y = (firstMax.Y - secondMin.Y) / secondVelocity.Y;
      else if(secondMax.Y < firstMin.Y && secondVelocity.Y > 0.0f)
        secondContact.Y = (firstMin.Y - secondMax.Y) / secondVelocity.Y;

      if(secondMax.Y > firstMin.Y && secondVelocity.Y < 0.0f)
        lastContact.Y = (firstMin.Y - secondMax.Y) / secondVelocity.Y;
      else if(firstMax.Y > secondMin.Y && secondVelocity.Y > 0.0f)
        lastContact.Y = (firstMax.Y - secondMin.Y) / secondVelocity.Y;

      // Z axis
      if(firstMax.Z < secondMin.Z && secondVelocity.Z < 0.0f)
        secondContact.Z = (firstMax.Z - secondMin.Z) / secondVelocity.Z;
      else if(secondMax.Z < firstMin.Z && secondVelocity.Z > 0.0f)
        secondContact.Z = (firstMin.Z - secondMax.Z) / secondVelocity.Z;

      if(secondMax.Z > firstMin.Z && secondVelocity.Z < 0.0f)
        lastContact.Z = (firstMin.Z - secondMax.Z) / secondVelocity.Z;
      else if(firstMax.Z > secondMin.Z && secondVelocity.Z > 0.0f)
        lastContact.Z = (firstMax.Z - secondMin.Z) / secondVelocity.Z;

      // We now extract the exact time of the box' entry into the other box
      // as well as the time of exit (if any)
      float entry = Math.Max(secondContact.X, Math.Max(secondContact.Y, secondContact.Z));
      float exit = Math.Min(lastContact.X, Math.Min(lastContact.Y, lastContact.Z));

      if(entry > exit)
        return null;

      return entry;
    }

  }

} // namespace Nuclex.Geometry.Volumes.Collisions
