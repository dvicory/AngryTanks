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

using Nuclex.Geometry.Volumes;
using Nuclex.Geometry.Areas;

namespace Nuclex.Geometry.Lines {

  /// <summary>One-dimensional range situated in 3D space</summary>
  /// <remarks>
  ///   This is the generalized interface to all kinds of lines (including rays and
  ///   line segments). Be careful not to mistake this for an infinite Line like it
  ///   is represented by the Line3 class.
  /// </remarks>
  public interface ILine3 {

    /// <summary>Determines the closest point on the range to the specified location</summary>
    /// <param name="location">Random location to which the closest point is determined</param>
    /// <returns>The closest point within the range</returns>
    Vector3 ClosestPointTo(Vector3 location);

    /// <summary>Determines where the range clips a sphere</summary>
    /// <param name="sphere">Sphere that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the sphere</returns>
    LineContacts FindContacts(Sphere3 sphere);

    /// <summary>Determines where the range clips an axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the box</returns>
    LineContacts FindContacts(AxisAlignedBox3 box);

    /// <summary>Determines where the range clips a box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>The times at which the range enters or leaves the box</returns>
    LineContacts FindContacts(Box3 box);

    /// <summary>Determines where the range clips a plane</summary>
    /// <param name="plane">Plane that will be checked for intersection</param>
    /// <returns>The times at which the range touches the plane, if at all</returns>
    LineContacts FindContacts(Plane3 plane);

    /// <summary>Determines where the range clips a triangle</summary>
    /// <param name="triangle">Triangle that will be checked for intersection</param>
    /// <returns>The times at which the range touches the triangle, if at all</returns>
    LineContacts FindContacts(Triangle3 triangle);

  }

} // namespace Nuclex.Geometry.Ranges
