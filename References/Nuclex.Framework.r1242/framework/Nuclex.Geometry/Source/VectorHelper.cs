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

namespace Nuclex.Geometry {

  /// <summary>Provides helper methods for vectors</summary>
  public static class VectorHelper {

    /// <summary>Calculates the absolute values of each vector component</summary>
    /// <param name="vector">Vector of which to calculate the absolute</param>
    /// <returns>A new vector with the absolute value of the input vector</returns>
    public static Vector3 Abs(Vector3 vector) {
      return new Vector3(
        Math.Abs(vector.X),
        Math.Abs(vector.Y),
        Math.Abs(vector.Z)
      );
    }

    /// <summary>Retrieves a vector component by its index</summary>
    /// <param name="vector">Vector to retrieve a component of</param>
    /// <param name="component">Index of the component to retrieve</param>
    /// <returns>The value of the vector's indicated component</returns>
    public static float Get(ref Vector3 vector, int component) {
      switch(component) {
        case 0: { return vector.X; }
        case 1: { return vector.Y; }
        case 2: { return vector.Z; }
        default: {
          throw new ArgumentOutOfRangeException("Vector component index out of range");
        }
      }
    }

    /// <summary>Sets a vector component by its index</summary>
    /// <param name="vector">Vector to set a component of</param>
    /// <param name="component">Index of the component to set</param>
    /// <param name="value">Value to assign to the vector's indicated component</param>
    public static void Set(ref Vector3 vector, int component, float value) {
      switch(component) {
        case 0: { vector.X = value; break; }
        case 1: { vector.Y = value; break; }
        case 2: { vector.Z = value; break; }
        default: {
          throw new ArgumentOutOfRangeException("Vector component index out of range");
        }
      }
    }

    /// <summary>Returns a vector that is perpendicular to the input vector</summary>
    /// <param name="vector">Vector to which a perpendicular vector will be found</param>
    /// <returns>A vector that is perpendicular to the input vector</returns>
    /// <remarks>
    ///   <para>
    ///     This method does not care for the orientation of the resulting vector, so it
    ///     shouldn't be used for billboards or to orient a view matrix. On the other hand,
    ///     if you don't care for the orientation of the resulting vector, only that it is
    ///     perpendicular, this method can provide better numerical stability and
    ///     performance than a generic LookAt() method.
    ///   </para>
    ///   <para>
    ///     References:
    ///     http://www.gamedev.net/community/forums/topic.asp?topic_id=445164
    ///     http://www.gamedev.net/community/forums/topic.asp?topic_id=518142
    ///   </para>
    /// </remarks>
    public static Vector3 GetPerpendicularVector(Vector3 vector) {
      Vector3 result;
      GetPerpendicularVector(ref vector, out result);
      return result;
    }

    /// <summary>Returns a vector that is perpendicular to the input vector</summary>
    /// <param name="vector">Vector to which a perpendicular vector will be found</param>
    /// <param name="perpendicular">
    ///   Output parameter that receives a vector perpendicular to the provided vector
    /// </param>
    /// <remarks>
    ///   <para>
    ///     This method does not care for the orientation of the resulting vector, so it
    ///     shouldn't be used for billboards or to orient a view matrix. On the other hand,
    ///     if you don't care for the orientation of the resulting vector, only that it is
    ///     perpendicular, this method can provide better numerical stability and
    ///     performance than a generic LookAt() method.
    ///   </para>
    ///   <para>
    ///     References:
    ///     http://www.gamedev.net/community/forums/topic.asp?topic_id=445164
    ///     http://www.gamedev.net/community/forums/topic.asp?topic_id=518142
    ///   </para>
    /// </remarks>
    public static void GetPerpendicularVector(ref Vector3 vector, out Vector3 perpendicular) {
      float absX = Math.Abs(vector.X);
      float absY = Math.Abs(vector.Y);
      float absZ = Math.Abs(vector.Z);

      if(absX < absY) {
        if(absZ < absX) {
          perpendicular = new Vector3(vector.Y, -vector.X, 0.0f);
        } else {
          perpendicular = new Vector3(0.0f, vector.Z, -vector.Y);
        }
      } else {
        if(absZ < absY) {
          perpendicular = new Vector3(vector.Y, -vector.X, 0.0f);
        } else {
          perpendicular = new Vector3(vector.Z, 0.0f, -vector.X);
        }
      }
    }

    /// <summary>
    ///   Builds a vector consisting of the larger absolute values from both inputs
    /// </summary>
    /// <param name="first">First vector to look for the larger absolute values in</param>
    /// <param name="second">Second vector to look for the larger absolute values in</param>
    /// <returns>
    ///   A vector consisting of the larger absolute values from both input vectors
    /// </returns>
    /// <remarks>
    ///   This works similar to the Vector2.Max() method, but it will not use
    ///   the element from the input vector that has the greater value, but look
    ///   for the element with the greater absolute value. Thus, for the inputs
    ///   {-10, -1} and {1, 10} it would return {-10, 10}.
    /// </remarks>
    public static Vector2 AbsMax(Vector2 first, Vector2 second) {
      return new Vector2(
        (Math.Abs(first.X) >= Math.Abs(second.X)) ? first.X : second.X,
        (Math.Abs(first.Y) >= Math.Abs(second.Y)) ? first.Y : second.Y
      );
    }

    /// <summary>
    ///   Builds a vector consisting of the larger absolute values from both inputs
    /// </summary>
    /// <param name="first">First vector to look for the larger absolute values in</param>
    /// <param name="second">Second vector to look for the larger absolute values in</param>
    /// <returns>
    ///   A vector consisting of the larger absolute values from both input vectors
    /// </returns>
    /// <remarks>
    ///   This works similar to the Vector2.Max() method, but it will not use
    ///   the element from the input vector that has the greater value, but look
    ///   for the element with the greater absolute value. Thus, for the inputs
    ///   {-10, -1, 10} and {1, 10, -10} it would return {-10, 10, 10}.
    /// </remarks>
    public static Vector3 AbsMax(Vector3 first, Vector3 second) {
      return new Vector3(
        (Math.Abs(first.X) >= Math.Abs(second.X)) ? first.X : second.X,
        (Math.Abs(first.Y) >= Math.Abs(second.Y)) ? first.Y : second.Y,
        (Math.Abs(first.Z) >= Math.Abs(second.Z)) ? first.Z : second.Z
      );
    }

  }

} // namespace Nuclex.Geometry
