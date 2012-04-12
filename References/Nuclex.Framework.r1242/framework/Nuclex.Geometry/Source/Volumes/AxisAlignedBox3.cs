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

  /// <summary>Three-dimensional axis aligned box</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class AxisAlignedBox3 : IVolume3 {

    /// <summary>Initializes a new box volume</summary>
    /// <param name="min">Lower left back bounds of the box</param>
    /// <param name="max">Upper right front bounds of the box</param>
    [System.Diagnostics.DebuggerStepThrough]
    public AxisAlignedBox3(Vector3 min, Vector3 max) {
      Min = min;
      Max = max;
    }

    /// <summary>Initializes a new box volume as copy of an existing box</summary>
    /// <param name="other">Existing box that will be copied</param>
    [System.Diagnostics.DebuggerStepThrough]
    public AxisAlignedBox3(AxisAlignedBox3 other)
      : this(other.Min, other.Max) { }

    /// <summary>Accepts a visitor to access the concrete volume implementation</summary>
    /// <param name="visitor">Visitor to be accepted</param>
    public void Accept(VolumeVisitor visitor) {
      visitor.Visit(this);
    }

    /// <summary>Smallest box that encloses the volume in its entirety</summary>
    /// <remarks>
    ///   This always produces an optimal box which means a tight-fitting box is generated
    ///   that will touch the volume on each of its six sides. As a side effect, it is very
    ///   likely that this box needs to be recalculated whenever the volume changes its
    ///   orientation.
    /// </remarks>
    public AxisAlignedBox3 BoundingBox {
      get { return new AxisAlignedBox3(Min, Max); } // Create a copy to be on the safe side...
    }
    /// <summary>Smallest sphere that encloses the volume in its entirety</summary>
    /// <remarks>
    ///   Bounding spheres have the advantage to not change even when the volume is
    ///   rotated. That makes them ideal for dynamic objects that are not keeping their
    ///   original orientation.
    /// </remarks>
    public Sphere3 BoundingSphere {
      get {
        Vector3 center = Center;
        return new Sphere3(center, (this.Max - center).Length());
      }
    }

    /// <summary>Amount of mass that the volume contains</summary>
    public float Mass {
      get { return Width * Height * Depth; }
    }
    /// <summary>The volume's total surface area</summary>
    public float SurfaceArea {
      get {
        float width = Width;
        float height = Height;
        float depth = Depth;

        return 2.0f * (width * height + width * depth + height * depth);
      }
    }
    /// <summary>Center of the volume's mass</summary>
    public Vector3 CenterOfMass {
      get {
        return new Vector3(
          (Min.X + Max.X) / 2.0f,
          (Min.Y + Max.Y) / 2.0f,
          (Min.Z + Max.Z) / 2.0f
        );
      }
    }
    /// <summary>The inertia tensor matrix of the volume</summary>
    public Matrix InertiaTensor {
      get {
        // TODO: Check that this is correct.
        //       http://www.gamedev.net/community/forums/topic.asp?topic_id=57001
        //       (Careful, there are different methods depending on whether your box
        //       is centered on its corner or on its center of mass)
        float widthSquared = this.Dimensions.X * this.Dimensions.X;
        float heightSquared = this.Dimensions.Y * this.Dimensions.Y;
        float depthSquared = this.Dimensions.Z * this.Dimensions.Z;
        return new Matrix(
          (heightSquared + depthSquared) / 3.0f, 0.0f, 0.0f, 0.0f,
          0.0f, (widthSquared + depthSquared) / 3.0f, 0.0f,  0.0f,
          0.0f, 0.0f, (widthSquared + heightSquared) / 3.0f, 0.0f,
          0.0f, 0.0f, 0.0f,                                  1.0f
        );
      }
    }

    /// <summary>Locates the nearest point in the volume to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the volume to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {
      return new Vector3(
        Math.Min(Math.Max(location.X, Min.X), Max.X),
        Math.Min(Math.Max(location.Y, Min.Y), Max.Y),
        Math.Min(Math.Max(location.Z, Min.Z), Max.Z)
      );
    }

    /// <summary>The width of the box (x axis)</summary>
    public float Width {
      [System.Diagnostics.DebuggerStepThrough]
      get { return Max.X - Min.X; }
    }

    /// <summary>The height of the box (y axis)</summary>
    public float Height {
      [System.Diagnostics.DebuggerStepThrough]
      get { return Max.Y - Min.Y; }
    }

    /// <summary>The depth of the box (z axis)</summary>
    public float Depth {
      [System.Diagnostics.DebuggerStepThrough]
      get { return Max.Z - Min.Z; }
    }

    /// <summary>Vector containing the extents of the box</summary>
    public Vector3 Extents {
      [System.Diagnostics.DebuggerStepThrough]
      get { return this.Dimensions / 2.0f; }
    }

    /// <summary>Vector containing the dimensions of the box</summary>
    public Vector3 Dimensions {
      [System.Diagnostics.DebuggerStepThrough]
      get { return Max - Min; }
    }

    /// <summary>The center of the box</summary>
    public Vector3 Center {
      [System.Diagnostics.DebuggerStepThrough]
      get { return (Min + Max) / 2; }
    }

    /// <summary>Determines if the volume clips the circle</summary>
    /// <param name="sphere">Circle that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Sphere3 sphere) {
      return Collisions.AabbSphereCollider.CheckContact(
        this.Min, this.Max, sphere.Center, sphere.Radius
      );
    }

    /// <summary>Determines if the volume clips the axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(AxisAlignedBox3 box) {
      return Collisions.AabbAabbCollider.CheckContact(
        this.Min, this.Max, box.Min, box.Max
      );
    }

    /// <summary>Determines if the volume clips the box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Box3 box) {
      return Collisions.AabbObbCollider.CheckContact(
        Extents, box.Transform, box.Extents
      );
    }

    /// <summary>Returns a random point on the volume's surface</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the volume's surface</returns>
    public Vector3 RandomPointOnSurface(IRandom randomNumberGenerator) {
      return PointGenerators.AabbPointGenerator.GenerateRandomPointOnSurface(
        randomNumberGenerator, this.Extents
      ) + this.Center;
    }

    /// <summary>Returns a random point within the volume</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point within the volume</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      return PointGenerators.AabbPointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, this.Extents
      ) + this.Center;
    }

    /// <summary>Determines if two axis aligned boxes are equal</summary>
    /// <param name="first">First axis aligned box to be compared</param>
    /// <param name="second">Second axis aligned box to be compared</param>
    /// <returns>True if both axis aligned boxes are equal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator ==(AxisAlignedBox3 first, AxisAlignedBox3 second) {
      return (first.Min == second.Min) && (first.Max == second.Max);
    }

    /// <summary>Determines if two axis aligned boxes are unequal</summary>
    /// <param name="first">First axis aligned box to be compared</param>
    /// <param name="second">Second axis aligned box to be compared</param>
    /// <returns>True if both axis aligned boxes are unequal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator !=(AxisAlignedBox3 first, AxisAlignedBox3 second) {
      return (first.Min != second.Min) || (first.Max != second.Max);
    }

    /// <summary>Determines if an object is identical to the axis aligned box</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the axis aligned box</returns>
    public override bool Equals(object obj) {
      AxisAlignedBox3 box = obj as AxisAlignedBox3;
      if(!ReferenceEquals(box, null))
        return (this == box);
      else
        return false;
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      return Min.GetHashCode() ^ Max.GetHashCode();
    }

    /// <summary>Converts the axis aligned box to a readable string representation</summary>
    /// <returns>The axis aligned box as a string</returns>
    public override string ToString() {
      return "{ " + Min.ToString() + " - " + Max.ToString() + " }";
    }

    /// <summary>Lower left back bounds of the box</summary>
    public Vector3 Min;
    /// <summary>Upper right front bounds of the box</summary>
    public Vector3 Max;
  }

} // namespace Nuclex.Geometry.Volumes
