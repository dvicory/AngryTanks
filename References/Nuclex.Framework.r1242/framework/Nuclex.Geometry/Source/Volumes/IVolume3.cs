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

namespace Nuclex.Geometry.Volumes {

  /// <summary>A point of contact between two volumes</summary>
  public struct ContactPoint {

    /// <summary>The absolute location where the contact occurs</summary>
    public Vector3 Location;
    /// <summary>The time at which the contact occurs</summary>
    public double Time;

  }

  /// <summary>Three-dimensional geometric body</summary>
  public interface IVolume3 {

    /// <summary>Accepts a visitor to access the concrete volume implementation</summary>
    /// <param name="visitor">Visitor to be accepted</param>
    void Accept(VolumeVisitor visitor);

    /// <summary>Smallest axis-aligned box that completely encloses the volume</summary>
    /// <remarks>
    ///   This always produces an optimal box which means a tight-fitting box is generated
    ///   that will touch the volume on each of its six sides. As a side effect, it is very
    ///   likely that this box needs to be recalculated whenever the volume changes its
    ///   orientation.
    /// </remarks>
    AxisAlignedBox3 BoundingBox { get; }

    /// <summary>Smallest sphere that completely encloses the volume</summary>
    /// <remarks>
    ///   Bounding spheres have the advantage to not change even when the volume is
    ///   rotated. That makes them ideal for dynamic objects that are not keeping their
    ///   original orientation.
    /// </remarks>
    Sphere3 BoundingSphere { get; }

    /// <summary>Amount of mass that the volume contains</summary>
    /// <remarks>
    ///   This is the mass the volume would have at a material density of 1.0
    /// </remarks>
    float Mass { get; }

    /// <summary>The volume's total surface area</summary>
    float SurfaceArea { get; }

    /// <summary>Center of the volume's mass</summary>
    Vector3 CenterOfMass { get; }

    /// <summary>The inetria tensor matrix of the volume</summary>
    Matrix InertiaTensor { get; }

    /// <summary>Locates the nearest point in the volume to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the volume to the specified location</returns>
    Vector3 ClosestPointTo(Vector3 location);

    /// <summary>Determines if the volume clips the circle</summary>
    /// <param name="sphere">Circle that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    bool Intersects(Sphere3 sphere);

    /// <summary>Determines if the volume clips the axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    bool Intersects(AxisAlignedBox3 box);

    /// <summary>Determines if the volume clips the box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    bool Intersects(Box3 box);

    /// <summary>Returns a random point on the volume's surface</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the volume's surface</returns>
    Vector3 RandomPointOnSurface(IRandom randomNumberGenerator);

    /// <summary>Returns a random point within the volume</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point within the volume</returns>
    Vector3 RandomPointWithin(IRandom randomNumberGenerator);

  }

} // namespace Nuclex.Geometry.Volumes
