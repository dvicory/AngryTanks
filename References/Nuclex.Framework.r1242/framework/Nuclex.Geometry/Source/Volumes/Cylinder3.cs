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

  /// <summary>Three-dimensional cylinder with arbitrary orientation</summary>
  /// <remarks>
  ///   With an identity matrix, the cylinder extents along the Y axis in both directions,
  ///   the translational part of the matrix will be equivalent to the cylinder's center.
  /// </remarks>
#if !NO_SERIALIZATION
  [Serializable]
#endif
  public class Cylinder3 : IVolume3 {

    /// <summary>Initializes a new instance of the cylinder</summary>
    /// <param name="transform">Orientation and position of cylinder</param>
    /// <param name="radius">The radius of the cylinder</param>
    /// <param name="length">The length of the cylinder</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Cylinder3(Matrix transform, float radius, float length) {
      this.Transform = transform;
      this.Radius = radius;
      this.Height = length;
    }

    /// <summary>Initializes a new cylinder as copy of an existing cylinder</summary>
    /// <param name="other">Existing cylinder that will be copied</param>
    [System.Diagnostics.DebuggerStepThrough]
    public Cylinder3(Cylinder3 other)
      : this(other.Transform, other.Radius, other.Height) { }

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
      get { throw new NotImplementedException("Not implemented yet"); }
    }

    /// <summary>Smallest sphere that encloses the volume in its entirety</summary>
    /// <remarks>
    ///   Bounding spheres have the advantage to not change even when the volume is
    ///   rotated. That makes them ideal for dynamic objects that are not keeping their
    ///   original orientation.
    /// </remarks>
    public Sphere3 BoundingSphere {
      get {
        // We can obtain the minimum diameter by taking the distance from the cylinder's
        // center of mass to a random point on the perimeter of one of its caps
        return new Sphere3(
          this.Center, new Vector2(this.Height / 2.0f, this.Radius).Length()
        );
      }
    }

    /// <summary>Amount of mass that the volume contains</summary>
    public float Mass {
      get {
        // Formula for cylinder volume: pi * (r ^ 2) * h
        return MathHelper.Pi * (this.Radius * this.Radius) * this.Height;
      }
    }

    /// <summary>The volume's total surface area</summary>
    public float SurfaceArea {
      get {
        // Formula for cylinder surface area: 2 * pi * (r ^ 2) + 2 * pi * r * h
        return
          (MathHelper.TwoPi * this.Radius * this.Height) + // cylinder side
          (MathHelper.TwoPi * this.Radius * this.Radius); // upper and lower cap
      }
    }

    /// <summary>Center of the volume's mass</summary>
    public Vector3 CenterOfMass {
      get { return this.Center; }
    }

    /// <summary>The inertia tensor matrix of the volume</summary>
    public Matrix InertiaTensor {
      get {
        // TODO: Check that this is correct.
        //       http://eta.physics.uoguelph.ca/tutorials/torque/Q.torque.inertia.html
        //       http://www.gamedev.net/community/forums/topic.asp?topic_id=57001
        //       http://www.physicsforums.com/showthread.php?t=175182
        //       (Careful, there are different methods depending on whether your cylinder
        //       is centered on its cap or on its center of mass)
        float length = (this.Radius * this.Radius) / 4.0f + (this.Height * this.Height) / 3.0f;
        float width = (this.Radius * this.Radius) / 2.0f;
        return new Matrix(
          width, 0.0f,   0.0f,  0.0f,
          0.0f,  length, 0.0f,  0.0f,
          0.0f,  0.0f,   width, 0.0f,
          0.0f,  0.0f,   0.0f,  0.0f
        );
      }
    }

    /// <summary>Locates the nearest point in the volume to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the volume to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Determines if the volume clips the circle</summary>
    /// <param name="sphere">Circle that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Sphere3 sphere) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Determines if the volume clips the axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(AxisAlignedBox3 box) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Determines if the volume clips the box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Box3 box) {
      throw new NotImplementedException("Not implemented yet");
    }

    /// <summary>Returns a random point on the volume's surface</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the volume's surface</returns>
    public Vector3 RandomPointOnSurface(IRandom randomNumberGenerator) {
      return PointGenerators.CylinderPointGenerator.GenerateRandomPointOnSurface(
        randomNumberGenerator, this.Transform, this.Radius, this.Height
      ) + this.Center;
    }

    /// <summary>Returns a random point within the volume</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point within the volume</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      return PointGenerators.CylinderPointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator, this.Transform, this.Radius, this.Height
      ) + this.Center;
    }

    /// <summary>Determines if two oriented boxes are equal</summary>
    /// <param name="first">First oriented box to be compared</param>
    /// <param name="second">Second oriented box to be compared</param>
    /// <returns>True if both axis oriented are equal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator ==(Cylinder3 first, Cylinder3 second) {
      return
        (first.Transform == second.Transform) &&
        (first.Radius == second.Radius) &&
        (first.Height == second.Height);
    }

    /// <summary>Determines if two oriented boxes are unequal</summary>
    /// <param name="first">First oriented box to be compared</param>
    /// <param name="second">Second oriented box to be compared</param>
    /// <returns>True if both oriented boxes are unequal</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool operator !=(Cylinder3 first, Cylinder3 second) {
      return
        (first.Transform != second.Transform) ||
        (first.Radius != second.Radius) ||
        (first.Height != second.Height);
    }

    /// <summary>Determines if an object is identical to the oriented box</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the oriented box</returns>
    public override bool Equals(object obj) {
      if(obj is Cylinder3)
        return this == (obj as Cylinder3);
      else
        return false;
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      return
        Transform.GetHashCode() ^ Center.GetHashCode() ^
        Radius.GetHashCode() ^ Height.GetHashCode();
    }

    /// <summary>Converts the cylinder to a readable string representation</summary>
    /// <returns>The cylinder as a string</returns>
    public override string ToString() {
      return
        "{ " +
          Transform.ToString() + " R:" + Radius.ToString() + " H:" + Height.ToString() +
        " }";
    }

    /// <summary>Determines the closest point in the cylinder to another point</summary>
    /// <param name="cylinderRadius">The cylinder's radius</param>
    /// <param name="cylinderLength">The cylinder's length</param>
    /// <param name="location">Location to which to determine the closest point</param>
    /// <returns>The closest point to the given location</returns>
    /// <remarks>
    ///   <para>
    ///     This method works entirely in the cylinder's coordinate frame. To use
    ///     this function on a cylinder that is not axis-aligned, translate the
    ///     reference location into the cylinder's coordinate frame before and
    ///     apply the cylinder's transformation matrix to the result.
    ///   </para>
    ///   <para>
    ///     This design decision allows algorithms which are not interested in
    ///     rotating the resulting closest point back into the global coodinate frame
    ///     to save some time (think of intersection tests as an example). For a
    ///     convenient closest point determination see the appropriate instance
    ///     method of this class.
    ///   </para>
    /// </remarks>
    public static Vector3 GetClosestPoint(
      float cylinderRadius, float cylinderLength, Vector3 location
    ) {
      float cylinderExtents = cylinderLength / 2.0f;
      float cylinderRadius2 = cylinderRadius * cylinderRadius;

      // We'll take the point on the cylinder's Z axis closest to the sphere...
      Vector3 axisPoint = new Vector3(0.0f, 0.0f, location.Z);

      // ...walk towards the sphere as far as the cylinder's radius permits...
      Vector3 radiusPoint = location - axisPoint;
      if(radiusPoint.LengthSquared() > cylinderRadius2)
        radiusPoint = axisPoint + Vector3.Normalize(radiusPoint) * cylinderRadius;
      else
        radiusPoint = axisPoint + radiusPoint;

      // ...and finally clamp this to the cylinder length to obtain the closest point
      return new Vector3(
        radiusPoint.X,
        radiusPoint.Y,
        Math.Min(Math.Max(radiusPoint.Z, -cylinderExtents), cylinderExtents)
      );
    }

    /// <summary>Location of the cylinder's center</summary>
    public Vector3 Center {
      get { return this.Transform.Translation; }
      set { this.Transform.Translation = value; }
    }

    /// <summary>Orientation of the cylinder in 3D space</summary>
    public Matrix Transform;
    /// <summary>Radius of the cylinder</summary>
    public float Radius;
    /// <summary>Length of the cylinder</summary>
    public float Height;
  }

} // namespace Nuclex.Geometry.Volumes
