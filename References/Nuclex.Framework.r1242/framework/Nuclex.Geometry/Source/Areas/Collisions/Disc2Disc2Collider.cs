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

  /// <summary>Contains all Disc2-to-Disc2 interference detection code</summary>
  public static class Disc2Disc2Collider {

    /// <summary>Test whether two discs are overlapping</summary>
    /// <param name="firstCenter">Center of the first disc</param>
    /// <param name="firstRadius">Radius of the first disc</param>
    /// <param name="secondCenter">Center of the second disc</param>
    /// <param name="secondRadius">Radius of the second disc</param>
    /// <returns>True if the discs are intersecting each other</returns>
    public static bool CheckContact(
      Vector2 firstCenter, float firstRadius,
      Vector2 secondCenter, float secondRadius
    ) {
      float distanceSquared = (secondCenter - firstCenter).LengthSquared();
      float combinedRadii = firstRadius + secondRadius;
      return (combinedRadii * combinedRadii) > distanceSquared;
    }

    /// <summary>Find the contact location between two discs</summary>
    /// <param name="firstCenter">Center of the first disc</param>
    /// <param name="firstRadius">Radius of the first disc</param>
    /// <param name="secondCenter">Center of the second disc</param>
    /// <param name="secondRadius">Radius of the second disc</param>
    /// <returns>A contact location if the discs touch each other</returns>
    public static Vector2? FindContact(
      Vector2 firstCenter, float firstRadius,
      Vector2 secondCenter, float secondRadius
    ) {
      Vector2 offset = (secondCenter - firstCenter);
      
      float distanceSquared = offset.LengthSquared();
      float combinedRadii = firstRadius + secondRadius;
      if(distanceSquared > (combinedRadii * combinedRadii))
        return null;

      Vector2 firstCircumferencePoint = firstCenter + offset * firstRadius;
      Vector2 secondCircumferencePoint = secondCenter - offset * secondRadius;

      return (firstCircumferencePoint + secondCircumferencePoint) / 2.0f;
    }

    /// <summary>Determines whether a disc will hit another box</summary>
    /// <param name="firstCenter">Center of the first disc</param>
    /// <param name="firstRadius">Radius of the first disc</param>
    /// <param name="secondCenter">Center of the second disc</param>
    /// <param name="secondRadius">Radius of the second disc</param>
    /// <param name="secondVelocity">
    ///   Velocity with which the second disc is moving relative to the first disc
    /// </param>
    /// <returns>True if the second disc will hit the first disc</returns>
    public static bool CheckContact(
      Vector2 firstCenter, float firstRadius,
      Vector2 secondCenter, float secondRadius, float secondVelocity
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Determines the time when a disc will hit another disc</summary>
    /// <param name="firstCenter">Center of the first disc</param>
    /// <param name="firstRadius">Radius of the first disc</param>
    /// <param name="secondCenter">Center of the second disc</param>
    /// <param name="secondRadius">Radius of the second disc</param>
    /// <param name="secondVelocity">
    ///   Velocity with which the second disc is moving relative to the first disc
    /// </param>
    /// <returns>The point of first contact, if any</returns>
    public static float? FindContact(
      Vector2 firstCenter, float firstRadius,
      Vector2 secondCenter, float secondRadius, float secondVelocity
    ) {
      throw new NotImplementedException("Not implemented yet");
    }

  }

} // namespace Nuclex.Geometry.Areas.Collisions
