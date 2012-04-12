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
using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Volumes {

  /// <summary>A two-dimensional circle</summary>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Sphere3 : IVolume3 {

    /// <summary>Initializes a new sphere</summary>
    /// <param name="center">The center of the circle</param>
    /// <param name="radius">Radius the circle will have</param>
    [DebuggerStepThrough]
    public Sphere3(Vector3 center, float radius) {
      Center = center;
      Radius = radius;
    }

    /// <summary>Initializes a new sphere as copy of an existing sphere</summary>
    /// <param name="other">Existing sphere that will be copied</param>
    [DebuggerStepThrough]
    public Sphere3(Sphere3 other)
      : this(other.Center, other.Radius) { }

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
      get {
        return new AxisAlignedBox3(
          new Vector3(Center.X - Radius, Center.Y - Radius, Center.Z - Radius),
          new Vector3(Center.X + Radius, Center.Y + Radius, Center.Z + Radius)
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
      get { return new Sphere3(Center, Radius); } // Create a copy to be on the safe side...
    }

    /// <summary>Amount of mass that the volume contains</summary>
    public float Mass {
      get { return 4.0f / 3.0f * MathHelper.Pi * (this.Radius * this.Radius * this.Radius); }
    }
    /// <summary>The volume's total surface area</summary>
    public float SurfaceArea {
      get { return 4.0f * MathHelper.Pi * (this.Radius * this.Radius); }
    }
    /// <summary>Center of the volume's mass</summary>
    public Vector3 CenterOfMass {
      get { return Center; }
    }
    /// <summary>The inertia tensor matrix of the volume</summary>
    public Matrix InertiaTensor {
      get {
        // 2 * (r ^ 2) / 5
        float r = 0.4f * this.Radius * this.Radius;
        return new Matrix(
          r,    0.0f, 0.0f, 0.0f,
          0.0f, r,    0.0f, 0.0f,
          0.0f, 0.0f, r,    0.0f,
          0.0f, 0.0f, 0.0f, 1.0f
        );
      }
    }

    /// <summary>Locates the nearest point in the volume to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the volume to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {
      Vector3 offset = location - Center;
      float distance = offset.Length();

      if(distance < Radius)
        return location;
      else
        return this.Center + (offset * (this.Radius / distance));
    }

    /// <summary>Determines whether a point is inside the sphere</summary>
    /// <param name="point">Point to be checked</param>
    /// <returns>True if the point lies within the sphere</returns>
    public bool Contains(Vector3 point) {
      float distance = (point - this.Center).Length();
      return (distance * distance) < this.Radius;
    }

    /// <summary>Determines if the volume clips the circle</summary>
    /// <param name="sphere">Circle that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Sphere3 sphere) {
      return Collisions.SphereSphereCollider.CheckContact(
        this.Center, this.Radius, sphere.Center, sphere.Radius
      );
    }

    /// <summary>Determines if the volume clips the axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(AxisAlignedBox3 box) {
      return Collisions.AabbSphereCollider.CheckContact(
        box.Min, box.Max, this.Center, this.Radius
      );
    }

    /// <summary>Determines if the volume clips the box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Box3 box) {
      return Collisions.ObbSphereCollider.CheckContact(
        box.Transform, box.Extents, this.Center, this.Radius
      );
    }

    /// <summary>Returns a random point on the volume's surface</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the volume's surface</returns>
    public Vector3 RandomPointOnSurface(IRandom randomNumberGenerator) {
      return PointGenerators.SpherePointGenerator.GenerateRandomPointOnSurface(
        randomNumberGenerator, this.Radius
      ) + this.Center;
    }

    /// <summary>Returns a random point within the volume</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point within the volume</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      return PointGenerators.SpherePointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, this.Radius
      ) + this.Center;
    }

    /// <summary>Determines if the volume will impact on a sphere</summary>
    /// <param name="thisVelocity">Velocity with which this volume is moving</param>
    /// <param name="sphere">Sphere that will be checked for intersection</param>
    /// <returns>The point of first contact, if any</returns>
    /// <remarks>
    ///   <para>
    ///     Conventional tests that resort to stepping often fail to detect collisions
    ///     between fast-moving objects. This impact determination test will always
    ///     detect a collision if it occurs, giving the exact time of the impact.
    ///   </para>
    ///   <para>
    ///     This is a simplified test that assumes a linear trajectory and does
    ///     not take off-center object rotation into account. It is well suited to use
    ///     on two bounding spheres in order to determine if a collision between the
    ///     shape contained is possible at all.
    ///   </para>
    ///   <para>
    ///     Ideas taken from the "Simple Intersection Tests for Games" article
    ///     on gamasutra by Gomez.
    ///   </para>
    /// </remarks>
    public float[] LocateImpact(Vector3 thisVelocity, Sphere3 sphere) {
      Vector3 distance = Center - sphere.Center;
      float radii = Radius + sphere.Radius;
      float radii2 = radii * radii;

      // Already inside the other circle 
      if(distance.LengthSquared() < radii2)
        return new float[] { 0.0f };

      float a = thisVelocity.LengthSquared();
      float b = Vector3.Dot(thisVelocity, distance) * 2.0f;
      float c = distance.LengthSquared() - radii2;
      float q = b * b - 4.0f * a * c;

      // If the other sphere is not crossing our location, then no impact will happen
      if(q < 0.0)
        return null;

      float sq = (float)Math.Sqrt(q);
      float d = 1.0f / (2.0f * a);
      float r1 = (-b + sq) * d;
      float r2 = (-b - sq) * d;

      if(r1 < r2)
        return new float[] { r1 };
      else
        return new float[] { r2 };
    }

    /// <summary>Determines if two spheres are equal</summary>
    /// <param name="first">First sphere to be compared</param>
    /// <param name="second">Second sphere to be compared</param>
    /// <returns>True if both spheres are equal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator ==(Sphere3 first, Sphere3 second) {
      return (first.Center == second.Center) && (first.Radius == second.Radius);
    }

    /// <summary>Determines if two spheres are unequal</summary>
    /// <param name="first">First sphere to be compared</param>
    /// <param name="second">Second sphere to be compared</param>
    /// <returns>True if both spheres are unequal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator !=(Sphere3 first, Sphere3 second) {
      return (first.Center != second.Center) || (first.Radius != second.Radius);
    }

    /// <summary>Determines if an object is identical to the sphere</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the sphere</returns>
    public override bool Equals(object obj) {
      if(obj is Sphere3)
        return this == (obj as Sphere3);
      else
        return false;
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      return Center.GetHashCode() ^ Radius.GetHashCode();
    }

    /// <summary>Converts the sphere to a readable string representation</summary>
    /// <returns>The sphere as a string</returns>
    public override string ToString() {
      return "{ " + Center.ToString() + " R:" + Radius + " }";
    }

    /// <summary>The center of the circle</summary>
    public Vector3 Center;
    /// <summary>Radius of the circle</summary>
    public float Radius;
  }

} // namespace Nuclex.Geometry.Volumes
