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

namespace Nuclex.Game.Space {

  /// <summary>Two-dimensional bounding rectangle</summary>
  public struct BoundingRectangle {

    /// <summary>Initializes a new two-dimensional bounding rectangle</summary>
    /// <param name="minX">X coordinate of the rectangle's left border</param>
    /// <param name="minY">Y coordinate of the rectangle's upper border</param>
    /// <param name="maxX">X coordinate of the rectangle's right border</param>
    /// <param name="maxY">Y coordinate of the rectangle's lower border</param>
    public BoundingRectangle(float minX, float minY, float maxX, float maxY) :
      this(new Vector2(minX, minY), new Vector2(maxX, maxY)) { }

    /// <summary>Initializes a new two-dimensional bounding rectangle</summary>
    /// <param name="min">Lesser coordinates of the bounding rectangle</param>
    /// <param name="max">Greater coordinates of the bounding rectangle</param>
    public BoundingRectangle(Vector2 min, Vector2 max) {
      this.Min = min;
      this.Max = max;
    }

    /// <summary>
    ///   Determines the containment type of a point to the bounding rectangle
    /// </summary>
    /// <param name="point">Point that will be checked for containment</param>
    /// <param name="result">The containment type of the point in the rectangle</param>
    public void Contains(ref Vector2 point, out ContainmentType result) {
      result = containsPoint(ref point) ?
        ContainmentType.Contains :
        ContainmentType.Disjoint;
    }

    /// <summary>Determines whether the bounding rectangle contains a point</summary>
    /// <param name="point">Point that will be checked for containment</param>
    /// <returns>True if the bounding rectangle contains the point</returns>
    public bool Contains(Vector2 point) {
      return containsPoint(ref point);
    }

    /// <summary>
    ///   Determines the containment type of another rectangle to the bounding rectangle
    /// </summary>
    /// <param name="rectangle">Rectangle that will be checked for containment</param>
    /// <param name="result">
    ///   The containment type of the other rectangle in the rectangle
    /// </param>
    public void Contains(ref BoundingRectangle rectangle, out ContainmentType result) {
      bool outside =
        (rectangle.Max.X <= this.Min.X) ||
        (rectangle.Min.X > this.Max.X) ||
        (rectangle.Max.Y <= this.Min.Y) ||
        (rectangle.Min.Y > this.Max.Y);

      if(outside) {
        result = ContainmentType.Disjoint;
        return;
      }

      bool contained =
        (rectangle.Min.X >= this.Min.X) &&
        (rectangle.Max.X < this.Max.X) &&
        (rectangle.Min.Y >= this.Min.Y) &&
        (rectangle.Max.Y < this.Max.Y);

      if(contained) {
        result = ContainmentType.Contains;
        return;
      }

      result = ContainmentType.Intersects;
      return;
    }

    /// <summary>
    ///   Determines whether the bounding rectangle contains another rectangle
    /// </summary>
    /// <param name="rectangle">Rectangle that will be checked for containment</param>
    /// <returns>True if the other rectangle is contained in the rectangle</returns>
    public bool Contains(BoundingRectangle rectangle) {
      return
        (rectangle.Min.X >= this.Min.X) &&
        (rectangle.Max.X < this.Max.X) &&
        (rectangle.Min.Y >= this.Min.Y) &&
        (rectangle.Max.Y < this.Max.Y);
    }

    /// <summary>
    ///   Builds the smallest bounding rectangle that contains the two
    ///   specified bounding rectangle.
    /// </summary>
    /// <param name="original">One of the bounding rectangles to contain</param>
    /// <param name="additional">One of the bounding rectangles to contain</param>
    /// <returns>The resulting merged bounding rectangle</returns>
    public static BoundingRectangle CreateMerged(
      BoundingRectangle original, BoundingRectangle additional
    ) {
      BoundingRectangle result;
      CreateMerged(ref original, ref additional, out result);
      return result;
    }

    /// <summary>
    ///   Builds the smallest bounding rectangle that contains the two
    ///   specified bounding rectangles.
    /// </summary>
    /// <param name="original">One of the bounding rectangles to contain</param>
    /// <param name="additional">One of the bounding rectangles to contain</param>
    /// <param name="result">The resulting merged bounding rectangle</param>
    public static void CreateMerged(
      ref BoundingRectangle original, ref BoundingRectangle additional,
      out BoundingRectangle result
    ) {
      result = new BoundingRectangle();
      result.Min = Vector2.Min(original.Min, additional.Min);
      result.Max = Vector2.Max(original.Max, additional.Max);
    }

    /// <summary>
    ///   Determines whether the rectangle intersects with another rectangle
    /// </summary>
    /// <param name="other">
    ///   Other rectangle that will be checked for intersection
    /// </param>
    /// <returns>True if the rectangles intersect</returns>
    public bool Intersects(BoundingRectangle other) {
      return intersectsRectangle(ref other);
    }

    /// <summary>
    ///   Determines whether the rectangle intersects with another rectangle
    /// </summary>
    /// <param name="other">
    ///   Other rectangle that will be checked for intersection
    /// </param>
    /// <param name="result">
    ///   Will be set to true if the rectangles intersects, otherwise false
    /// </param>
    public void Intersects(ref BoundingRectangle other, out bool result) {
      result = intersectsRectangle(ref other);
    }

    /// <summary>Determines whether another object is an identical bounding rectangle</summary>
    /// <param name="otherObject">Other object that will be compared</param>
    /// <returns>True if the other object is identical to the bounding rectangle</returns>
    public override bool Equals(object otherObject) {
      if(otherObject is BoundingRectangle) {
        return Equals((BoundingRectangle)otherObject);
      } else {
        return false;
      }
    }

    /// <summary>Determines whether another object is an identical bounding rectangle</summary>
    /// <param name="other">Other rectangle that will be compared</param>
    /// <returns>True if the other rectangle is identical to the bounding rectangle</returns>
    public bool Equals(BoundingRectangle other) {
      return
        (this.Min.X == other.Min.X) &&
        (this.Min.Y == other.Min.Y) &&
        (this.Max.X == other.Max.X) &&
        (this.Max.Y == other.Max.Y);
    }

    /// <summary>Checks two bounding rectangles for inequality</summary>
    /// <param name="first">First bounding rectangle that will be compared</param>
    /// <param name="second">Second bounding rectangle that will be compared</param>
    /// <returns>True if the rectangles are different</returns>
    public static bool operator !=(BoundingRectangle first, BoundingRectangle second) {
      return !(first == second);
    }

    /// <summary>Checks two bounding rectangles for equality</summary>
    /// <param name="first">First bounding rectangle that will be compared</param>
    /// <param name="second">Second bounding rectangle that will be compared</param>
    /// <returns>True if both rectangles are equal</returns>
    public static bool operator ==(BoundingRectangle first, BoundingRectangle second) {
      return first.Equals(second);
    }

    /// <summary>Gets a hash code for the bounding rectangle</summary>
    /// <returns>The hash code of the bounding rectangle</returns>
    public override int GetHashCode() {
      return
        this.Min.GetHashCode() ^
        this.Max.GetHashCode();
    }

    /// <summary>Converts the bounding rectangle into a human-readable string</summary>
    /// <returns>A human readable string describing the bounding rectangle</returns>
    public override string ToString() {
      string min = this.Min.ToString();
      string max = this.Max.ToString();

      StringBuilder builder = new StringBuilder(5 + min.Length + 5 + max.Length + 1);
      builder.Append("{Min:");
      builder.Append(min);
      builder.Append(" Max:");
      builder.Append(max);
      builder.Append('}');

      return builder.ToString();
    }

    /// <summary>Whether the bounding rectangle contains the specified point</summary>
    /// <param name="point">Point that is checked for containment</param>
    /// <returns>True if the point is contained in the bounding rectangle</returns>
    private bool containsPoint(ref Vector2 point) {
      return
        (point.X >= Min.X) &&
        (point.Y >= Min.Y) &&
        (point.X < Max.X) &&
        (point.Y < Max.Y);
    }

    /// <summary>Whether the bounding rectangle intersects with another rectangle</summary>
    /// <param name="rectangle">
    ///   Other rectangle that will be checked for intersection
    /// </param>
    /// <returns>True if the rectangles intersect each other</returns>
    private bool intersectsRectangle(ref BoundingRectangle rectangle) {
      return
        (rectangle.Max.X >= this.Min.X) &&
        (rectangle.Max.Y >= this.Min.Y) &&
        (rectangle.Min.X < this.Max.X) &&
        (rectangle.Min.Y < this.Max.Y);
    }

    /// <summary>Coordinates of the lesser side of the bounding rectangle</summary>
    public Vector2 Min;

    /// <summary>Coordinates of the greater side of the bounding rectangle</summary>
    public Vector2 Max;

  }

} // namespace Nuclex.Game.Space
