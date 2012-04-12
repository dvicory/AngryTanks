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

  /// <summary>Three-dimensional box with arbitrary orientation</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Box3 : IVolume3 {

    /// <summary>Initializes a new instance of the oriented box</summary>
    /// <param name="transform">
    ///   Transformation matrix that defines the box' rotation and translation
    /// </param>
    /// <param name="extents">The extents (half the dimensions) of the box</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Box3(Matrix transform, Vector3 extents) {
      this.Transform = transform;
      this.Extents = extents;
    }

    /// <summary>Initializes a new oriented box as copy of an existing box</summary>
    /// <param name="other">Existing box that will be copied</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Box3(Box3 other)
      : this(other.Transform, other.Extents) { }

    /// <summary>Accepts a visitor to access the concrete volume implementation</summary>
    /// <param name="visitor">Visitor to be accepted</param>
    public void Accept(VolumeVisitor visitor) {
      visitor.Visit(this);
    }

    /// <summary>Smallest box that encloses the volume in its entirety</summary>
    /// <remarks>
    ///   <para>
    ///     This always produces an optimal box which means a tight-fitting box is generated
    ///     that will touch the volume on each of its six sides. As a side effect, it is very
    ///     likely that this box needs to be recalculated whenever the volume changes its
    ///     orientation.
    ///   </para>
    ///   <para>
    ///     This method was actually thought up by myself when all googling did not
    ///     reveal a clever way to avoid the expensive matrix-vector multiplications.
    ///     Feel free to use it in any way you see fit.
    ///   </para>
    /// </remarks>
    public AxisAlignedBox3 BoundingBox {
      get {
        // We just calculate one half of the oriented box and obtain the other
        // by mirroring these points
        Vector3[] corners = new Vector3[] {
          new Vector3( this.Extents.X,  this.Extents.Y,  this.Extents.Z),
          new Vector3(-this.Extents.X,  this.Extents.Y,  this.Extents.Z),
          new Vector3( this.Extents.X, -this.Extents.Y,  this.Extents.Z),
          new Vector3( this.Extents.X,  this.Extents.Y, -this.Extents.Z)
        };

        // Transform all points and calculate the maximum distance on each
        // axis in positive direction by using its absolute value
        Vector3 aabbExtents = new Vector3();
        foreach(Vector3 corner in corners)
          aabbExtents = Vector3.Max(
            aabbExtents, VectorHelper.Abs(Vector3.TransformNormal(corner, this.Transform))
          );

        // Now we can just mirror the other direction
        return new AxisAlignedBox3(
          this.Center - aabbExtents,
          this.Center + aabbExtents
        );
      }
    }
    /// <summary>Smallest sphere that encloses the volume in its entirety</summary>
    /// <remarks>
    ///   Bounding spheres have the advantage to not change even when the volume is
    ///   rotated. That makes them ideal for dynamic objects that are not keeping their
    ///   original orientation.
    /// </remarks>
    public Sphere3 BoundingSphere {
      get { return new Sphere3(this.Center, this.Extents.Length()); }
    }

    /// <summary>Amount of mass that the volume contains</summary>
    public float Mass {
      get { return Dimensions.X * Dimensions.Y * Dimensions.Z; }
    }
    /// <summary>The volume's total surface area</summary>
    public float SurfaceArea {
      get {
        // x 4  because the extents are half as wide as the dimensions
        //      if you make your screen half as wide and half as high you've got one
        //      quarter of what it had been before, so we multiply by 4
        // x 2  because we've got two faces

        return 4.0f * 2.0f * (
          (Extents.X * Extents.Y) +
          (Extents.X * Extents.Z) +
          (Extents.Y * Extents.Z)
        );
      }
    }
    /// <summary>Center of the volume's mass</summary>
    public Vector3 CenterOfMass {
      get { return Center; }
    }
    /// <summary>The inetria tensor matrix of the volume</summary>
    public Matrix InertiaTensor {
      get {
        float width = this.Extents.X;
        float height = this.Extents.Y;
        float depth = this.Extents.Z;

        return new Matrix(
          (height * height + depth * depth) / 3.0f, 0.0f, 0.0f, 0.0f,
          0.0f, (width * width + depth * depth) / 3.0f, 0.0f, 0.0f,
          0.0f, 0.0f, (width * width + height * height) / 3.0f, 0.0f,
          0.0f, 0.0f, 0.0f, 1.0f
        );
      }
    }

    /// <summary>Locates the nearest point in the volume to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the volume to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {

      // We transform the point to the coordinate frame of the oriented box
      Vector3 difference = location - this.Center;
      Vector3 offset = new Vector3(
        Vector3.Dot(difference, this.Transform.Right),
        Vector3.Dot(difference, this.Transform.Up),
        Vector3.Dot(difference, this.Transform.Forward)
      );

      Vector3 local = new Vector3(
        Math.Min(Math.Max(offset.X, -this.Extents.X), this.Extents.X),
        Math.Min(Math.Max(offset.Y, -this.Extents.Y), this.Extents.Y),
        Math.Min(Math.Max(offset.Z, -this.Extents.Z), this.Extents.Z)
      );

      return Vector3.Transform(local, this.Transform) + this.Center;
    }

    /// <summary>Determines if the volume clips the circle</summary>
    /// <param name="sphere">Circle that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Sphere3 sphere) {
      return Collisions.ObbSphereCollider.CheckContact(
        this.Transform, this.Extents, sphere.Center, sphere.Radius
      );
    }

    /// <summary>Determines if the volume clips the axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(AxisAlignedBox3 box) {
      return Collisions.AabbObbCollider.CheckContact(
        box.Extents, this.Transform, this.Extents
      );
    }

    /// <summary>Determines if the volume clips the box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Box3 box) {
      return Collisions.ObbObbCollider.CheckContact(
        this.Transform, this.Extents, box.Transform, box.Extents
      );
    }

    /// <summary>Returns a random point on the volume's surface</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the volume's surface</returns>
    public Vector3 RandomPointOnSurface(IRandom randomNumberGenerator) {
      return PointGenerators.ObbPointGenerator.GenerateRandomPointOnSurface(
        randomNumberGenerator, this.Transform, this.Extents
      ) + this.Center;
    }

    /// <summary>Returns a random point within the volume</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point within the volume</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      return PointGenerators.ObbPointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, this.Transform, this.Extents
      ) + this.Center;
    }

    /// <summary>The dimensions of this box</summary>
    public Vector3 Dimensions {
      get { return Extents * 2.0f; }
    }

    /// <summary>Determines if two oriented boxes are equal</summary>
    /// <param name="first">First oriented box to be compared</param>
    /// <param name="second">Second oriented box to be compared</param>
    /// <returns>True if both axis oriented are equal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator ==(Box3 first, Box3 second) {
      return
        (first.Transform == second.Transform) &&
        (first.Center == second.Center) &&
        (first.Extents == second.Extents);
    }

    /// <summary>Determines if two oriented boxes are unequal</summary>
    /// <param name="first">First oriented box to be compared</param>
    /// <param name="second">Second oriented box to be compared</param>
    /// <returns>True if both oriented boxes are unequal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator !=(Box3 first, Box3 second) {
      return
        (first.Transform != second.Transform) ||
        (first.Center != second.Center) ||
        (first.Extents != second.Extents);
    }

    /// <summary>Determines if an object is identical to the oriented box</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the oriented box</returns>
    public override bool Equals(object obj) {
      if(obj is Box3)
        return this == (obj as Box3);
      else
        return false;
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      return Transform.GetHashCode() ^ Center.GetHashCode() ^ Extents.GetHashCode();
    }

    /// <summary>Converts the oriented box to a readable string representation</summary>
    /// <returns>The axis oriented as a string</returns>
    public override string ToString() {
      return
        "{ " +
          Transform.ToString() + " C:" + Center.ToString() + " E:" + Extents.ToString() +
        " }";
    }

    /// <summary>Location of the box' center</summary>
    public Vector3 Center {
      get { return this.Transform.Translation; }
      set { this.Transform.Translation = value; }
    }

    /// <summary>Orientation of the box in 3D space</summary>
    public Matrix Transform;
    /// <summary>Box dimensions in the box' local coordinate system</summary>
    /// <remarks>
    ///   These are the extents, not the dimensions. The dimensions are the
    ///   total length of the box on each of its three local coordinate axes while
    ///   the extents refer to the distance of each side from the center of the
    ///   box, much like the radius and the diameter of a sphere.
    /// </remarks>
    public Vector3 Extents;
  }

} // namespace Nuclex.Geometry.Volumes
