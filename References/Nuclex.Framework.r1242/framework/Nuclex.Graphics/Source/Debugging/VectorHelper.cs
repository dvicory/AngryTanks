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
using System.Text;

using Microsoft.Xna.Framework;

namespace Nuclex.Graphics.Debugging {

  internal static class VectorHelper {

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

      if (absX < absY) {
        if (absZ < absX) {
          perpendicular = new Vector3(vector.Y, -vector.X, 0.0f);
        } else {
          perpendicular = new Vector3(0.0f, vector.Z, -vector.Y);
        }
      } else {
        if (absZ < absY) {
          perpendicular = new Vector3(vector.Y, -vector.X, 0.0f);
        } else {
          perpendicular = new Vector3(vector.Z, 0.0f, -vector.X);
        }
      }
    }

  }

} // namespace Nuclex.Graphics.Debugging
