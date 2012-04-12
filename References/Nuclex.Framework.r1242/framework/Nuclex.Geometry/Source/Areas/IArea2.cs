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

namespace Nuclex.Geometry.Areas {

  /// <summary>Informations about the point of first contact between two bodies</summary>
  public struct ImpactPoint {
    /// <summary>Whether a contact could be found at all</summary>
    public bool Found;
    /// <summary>The absolute location where the contact occurs</summary>
    public Vector2 AbsoluteLocation;
    /// <summary>The time at which the contact occurs</summary>
    public float Time;
  }

  /// <summary>Two-dimensional geometric body in 2D space</summary>
  /// <remarks>
  ///   An area by definition and of course also in the context of this library is a
  ///   two-dimensional region. This region could a either be located in actual
  ///   2D space (like drawn on a piece of paper) or be located in as a flat object
  ///   in 3D space (like the piece of paper located in the real world). This
  ///   class represents the first case, a 2D object in 2D space.
  /// </remarks>
  public interface IArea2 {

    /*
    /// <summary>Accepts a visitor to access the concrete area implementation</summary>
    /// <param name="visitor">Visitor to be accepted</param>
    void Accept(IArea2Visitor visitor);
    */

    /// <summary>Surface area that the shape contains</summary>
    float Area { get; }

    /// <summary>The total length of the area's circumference</summary>
    float CircumferenceLength { get; }

    /// <summary>The center of mass within the shape</summary>
    Vector2 CenterOfMass { get; }

    /// <summary>Smallest rectangle that encloses the shape in its entirety</summary>
    AxisAlignedRectangle2 BoundingBox { get; }

    /// <summary>Locates the nearest point in the shape to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the shape to the specified location</returns>
    Vector2 ClosestPointTo(Vector2 location);

    /// <summary>Returns a random point on the area's perimeter</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the area's perimeter</returns>
    Vector2 RandomPointOnPerimeter(IRandom randomNumberGenerator);

    /// <summary>Returns a random point inside the area</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point inside the area</returns>
    Vector2 RandomPointWithin(IRandom randomNumberGenerator);

  }

} // namespace Nuclex.Geometry.Areas
