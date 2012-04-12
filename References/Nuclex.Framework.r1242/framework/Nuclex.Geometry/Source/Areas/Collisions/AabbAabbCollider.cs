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

namespace Nuclex.Geometry.Areas.Collisions {

  /// <summary>Contains all Aabb-to-Aabb interference detection code</summary>
  public static class AabbAabbCollider {

    /// <summary>Test whether two axis aligned boxes are overlapping</summary>
    /// <param name="firstMin">Minimum coordinate of first box</param>
    /// <param name="firstMax">Maximum coordinate of first box</param>
    /// <param name="secondMin">Minimum coordinate of second box</param>
    /// <param name="secondMax">Maximum coordinate of second box</param>
    /// <returns>True if the boxes are intersecting each other</returns>
    public static bool CheckContact(
      Vector2 firstMin, Vector2 firstMax,
      Vector2 secondMin, Vector2 secondMax
    ) {
      return
        (firstMin.X < secondMax.X) && (firstMax.X > secondMin.X) &&
        (firstMin.Y < secondMax.Y) && (firstMax.Y > secondMin.Y);
    }

    /// <summary>Find the contact location between two axis aligned boxes</summary>
    /// <param name="firstMin">Minimum coordinate of first box</param>
    /// <param name="firstMax">Maximum coordinate of first box</param>
    /// <param name="secondMin">Minimum coordinate of second box</param>
    /// <param name="secondMax">Maximum coordinate of second box</param>
    /// <returns>A contact location if the boxes touch each other</returns>
    public static Vector2? FindContact(
      Vector2 firstMin, Vector2 firstMax,
      Vector2 secondMin, Vector2 secondMax
    ) {
      // Extract the intersecting area of the two boxes
      Vector2 min = Vector2.Max(firstMin, secondMin);
      Vector2 max = Vector2.Min(firstMax, secondMax);

      // If the boxes don't touch we don't have an intersection
      if((max.X < min.X) || (max.Y < min.Y))
        return null;

      // The contact point is in the center of the intersecting area
      return (min + max) / 2.0f;
    }

    /// <summary>Determines whether a box will hit another box</summary>
    /// <param name="firstMin">Minimum coordinate of first box</param>
    /// <param name="firstMax">Maximum coordinate of first box</param>
    /// <param name="secondMin">Minimum coordinate of second box</param>
    /// <param name="secondMax">Maximum coordinate of second box</param>
    /// <param name="secondVelocity">
    ///   Velocity with which the second box is moving relative to the first box
    /// </param>
    /// <returns>True if the second box will hit the first box</returns>
    public static bool CheckContact(
      Vector2 firstMin, Vector2 firstMax,
      Vector2 secondMin, Vector2 secondMax, Vector2 secondVelocity
    ) {
      return FindContact(firstMin, firstMax, secondMin, secondMax, secondVelocity).HasValue;
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
    ///   </para>
    /// </remarks>
    public static float? FindContact(
      Vector2 firstMin, Vector2 firstMax,
      Vector2 secondMin, Vector2 secondMax, Vector2 secondVelocity
    ) {
      Vector2 secondContact = new Vector2(0.0f, 0.0f);
      Vector2 lastContact = new Vector2(1.0f, 1.0f);

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

      // We now extract the exact time of the box' entry into the other box
      // as well as the time of exit (if any)
      float entry = Math.Max(secondContact.X, secondContact.Y);
      float exit = Math.Min(lastContact.X, lastContact.Y);

      if(entry > exit)
        return null;

      return entry;
    }

  }

} // namespace Nuclex.Geometry.Areas.Collisions
