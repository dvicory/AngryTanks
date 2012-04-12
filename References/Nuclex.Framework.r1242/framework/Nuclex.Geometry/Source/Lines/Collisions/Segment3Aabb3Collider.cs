using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Lines.Collisions {

  /// <summary>Contains all Segment3 to Aabb3 interference detection code</summary>
  public static class Segment3Aabb3Collider {
/*
    /// <summary>Determines where the range clips an axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the volume</returns>
    /// <remarks>
    ///   Taken from the article "Simple Intersection Tests for Games" on
    ///   Gamasutra by Gomez et al.
    /// </remarks>
    public bool Intersects(AxisAlignedBox3 box) {
      Vector3 lineDirection = (End - Start);
      float lineHalfLength = lineDirection.Length();
      lineDirection /= lineHalfLength;
      lineHalfLength /= 2.0f;

      // ALGORITHM: Use the separating axis theorem to see if the line segment and
      //            the box overlap. A line segment is a degenerate OBB.

      Vector3 distance = box.Center - (Start + End) / 2.0f;
      float r;

      // Do any of the principal axes form a separating axis?
      Vector3 scaledDimensions = new Vector3(
        box.Dimensions.X * lineHalfLength * Math.Abs(lineDirection.X),
        box.Dimensions.Y * lineHalfLength * Math.Abs(lineDirection.Y),
        box.Dimensions.Z * lineHalfLength * Math.Abs(lineDirection.Z)
      );

      if(Math.Abs(distance.X) > scaledDimensions.X)
        return false;

      if(Math.Abs(distance.Y) > scaledDimensions.Y)
        return false;

      if(Math.Abs(distance.Z) > scaledDimensions.Z)
        return false;

      // NOTE: Since the separating axis is perpendicular to the line in these
      //       last four cases, the line does not contribute to the projection.

      //l.cross(x-axis)?

      r =
        box.Dimensions.Y * Math.Abs(lineDirection.Z) +
        box.Dimensions.Z * Math.Abs(lineDirection.Y);

      if(Math.Abs(distance.Y * lineDirection.Z - distance.Z * lineDirection.Y) > r)
        return false;

      //l.cross(y-axis)?

      r =
        box.Dimensions.X * Math.Abs(lineDirection.Z) +
        box.Dimensions.Z * Math.Abs(lineDirection.X);

      if(Math.Abs(distance.Z * lineDirection.X - distance.X * lineDirection.Z) > r)
        return false;

      //l.cross(z-axis)?

      r =
        box.Dimensions.X * Math.Abs(lineDirection.Y) +
        box.Dimensions.Y * Math.Abs(lineDirection.X);

      if(Math.Abs(distance.X * lineDirection.Y - distance.Y * lineDirection.X) > r)
        return false;

      return true;
    }
*/
  }

} // namespace Nuclex.Geometry.Lines.Collisions

