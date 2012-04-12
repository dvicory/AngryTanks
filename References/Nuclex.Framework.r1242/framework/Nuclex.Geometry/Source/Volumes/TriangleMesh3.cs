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
using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Volumes {

  /// <summary>Three-dimensional triangle mesh</summary>
  public class TriangleMesh3 : IVolume3 {

    /// <summary>Initializes a triangle mesh from a sequentiel list of triangles</summary>
    /// <param name="vertices">Array of vertices to construct triangles from</param>
    /// <remarks>
    ///   This variant of the constructor generates a mesh from the vertex array by
    ///   building triangles of every three vertices in the list. The triangles are
    ///   required to form a closed, convex hull.
    /// </remarks>
    TriangleMesh3(Vector3[] vertices)
      : this(vertices, generateIndices(vertices.Length)) { }

    /// <summary>Initializes a triangle mesh from an indexed list of triangles</summary>
    /// <param name="vertices">Vertices to construct triangles from</param>
    /// <param name="indices">Indices to the vertices to construct triangles from</param>
    /// <remarks>
    ///   This variant of the constructor generates a mesh from the vertex array by
    ///   taking every three indices in the index array and then generating a triangle
    ///   by the three vertices with the given index. The triangles are required to
    ///   form a closed, convex hull.
    /// </remarks>
    TriangleMesh3(Vector3[] vertices, int[] indices) {
      this.Vertices = vertices;
      this.Indices = indices;

      computeMassProperties();
    }

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
        if(this.Vertices == null) {
          return new AxisAlignedBox3(Vector3.Zero, Vector3.Zero);

        } else {
          Vector3 min, max;
          min = max = this.Vertices[0];

          foreach(Vector3 vertex in this.Vertices) {
            if(vertex.X < min.X)
              min.X = vertex.X;
            if(vertex.Y < min.Y)
              min.Y = vertex.Y;
            if(vertex.Z < min.Z)
              min.Z = vertex.Z;
            if(vertex.X > max.X)
              max.X = vertex.X;
            if(vertex.Y > max.Y)
              max.Y = vertex.Y;
            if(vertex.Z > max.Z)
              max.Z = vertex.Z;
          }

          return new AxisAlignedBox3(min, max);
        }
      }
    }
    /// <summary>Smallest sphere that encloses the volume in its entirety</summary>
    /// <remarks>
    ///   Bounding spheres have the advantage to not change even when the volume is
    ///   rotated. That makes them ideal for dynamic objects that are not keeping their
    ///   original orientation.
    /// </remarks>
    public Sphere3 BoundingSphere {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    /// <summary>Amount of mass that the volume contains</summary>
    public float Mass {
      get { return this.mass; }
    }
    /// <summary>The volume's total surface area</summary>
    public float SurfaceArea {
      get { return this.surfaceArea; }
    }
    /// <summary>Center of the volume's mass</summary>
    public Vector3 CenterOfMass {
      get { return this.centerOfMass; }
    }
    /// <summary>The inetria tensor matrix of the volume</summary>
    public Matrix InertiaTensor {
      get { return this.inertiaTensor; }
    }

    /// <summary>Locates the nearest point in the volume to some arbitrary location</summary>
    /// <param name="location">Location to which the closest point is determined</param>
    /// <returns>The closest point in the volume to the specified location</returns>
    public Vector3 ClosestPointTo(Vector3 location) {
      throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>Determines if the volume clips the circle</summary>
    /// <param name="sphere">Circle that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Sphere3 sphere) {
      return Collisions.MeshSphereCollider.CheckContact(
        this, sphere.Center, sphere.Radius
      );
    }

    /// <summary>Determines if the volume clips the axis aligned box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(AxisAlignedBox3 box) {
      return Collisions.AabbMeshCollider.CheckContact(
        box.Min, box.Max, this
      );
    }

    /// <summary>Determines if the volume clips the box</summary>
    /// <param name="box">Box that will be checked for intersection</param>
    /// <returns>True if the objects overlap</returns>
    public bool Intersects(Box3 box) {
      return Collisions.MeshObbCollider.CheckContact(
        this, box.Transform, box.Extents
      );
    }

    /// <summary>Returns a random point on the volume's surface</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point on the volume's surface</returns>
    public Vector3 RandomPointOnSurface(IRandom randomNumberGenerator) {

      // This number is used to select the triangle on which the random point will be
      // generated. Is is relative to the total surface area of the mesh, so smaller
      // triangles cover a smaller numeric range to achieve uniform distribution.
      float areaIndex = (float)randomNumberGenerator.NextDouble();
      areaIndex *= this.surfaceArea;

      // MSDN states that BinarySearch can be used even when the searched value doesn't exist
      //
      //   Returns:
      //     "If value is not found and value is less than one or more elements in array,
      //      a negative number which is the bitwise complement of the index of the first
      //      element that is larger than value"
      int index = Array.BinarySearch<float>(this.accumulatedTriangleSurface, areaIndex);
      if(index < 0)
        index = ~index - 1;

      // Construct a triangle from the vertices and generate a random point in its area
      return Areas.PointGenerators.Triangle3PointGenerator.GenerateRandomPointWithin(
        randomNumberGenerator,
        this.Vertices[this.Indices[index * 3 + 0]],
        this.Vertices[this.Indices[index * 3 + 1]],
        this.Vertices[this.Indices[index * 3 + 2]]
      );

    }

    /// <summary>Returns a random point within the volume</summary>
    /// <param name="randomNumberGenerator">Random number generator that will be used</param>
    /// <returns>A random point within the volume</returns>
    public Vector3 RandomPointWithin(IRandom randomNumberGenerator) {
      throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>Enumerates the triangles contained in the mesh</summary>
    public IEnumerable<Areas.Triangle3> Triangles {
      get {
        int indicesIndex = 0;

        while(indicesIndex <= (this.Indices.Length - 3)) {
          yield return new Areas.Triangle3(
            this.Vertices[this.Indices[indicesIndex + 0]],
            this.Vertices[this.Indices[indicesIndex + 1]],
            this.Vertices[this.Indices[indicesIndex + 2]]
          );

          indicesIndex += 3;
        }
      }
    }

    /// <summary>Location at which mesh is placed in space</summary>
    /// <remarks>
    ///   This doesn't take the center of mass or any other physical property into account,
    ///   it's merely an offset for the original vertex coordinates. Otherwise you would have
    ///   a hard time aligning your physical mesh with the visual one :)
    /// </remarks>
    public Vector3 Location {
      get { return this.Transform.Translation; }
      set { this.Transform.Translation = value; }
    }

    /// <summary>Orientation as position of the mesh in space</summary>
    public Matrix Transform;

    /// <summary>Compute or recompute the mass properties of this triangle mesh</summary>
    /// <remarks>
    ///   <para>
    ///     The code has been translated to C# from David Eberly's document titled
    ///     "Polyhedral Mass Properties (Revisited)" which is in turn based on
    ///     Brian Mirtich's "Fast and accurate computation of polyhedral mass properties".
    ///     You can find David Eberly's document at
    ///       http://www.geometrictools.com/Documentation/PolyhedralMassProperties.pdf
    ///   </para>
    /// </remarks>
    private void computeMassProperties() {

      int triangleCount = (this.Indices.Length - (this.Indices.Length % 3)) / 3;
      this.accumulatedTriangleSurface = new float[triangleCount];
      this.surfaceArea = 0.0f;

      // Order: 1, x, y, z, x^2, y^2, z^2, xy, yz, zx
      float[] integral = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

      int triangleIndex = 0;
      foreach(Areas.Triangle3 triangle in this.Triangles) {

        // Get edges and cross product of edges
        Vector3 ab = triangle.B - triangle.A;
        Vector3 ac = triangle.C - triangle.A;
        Vector3 cross = Vector3.Cross(ab, ac);

        // Compute integral terms
        Vector3 t0 = triangle.A + triangle.B;
        Vector3 t1 = triangle.A * triangle.A;
        Vector3 t2 = t1 + triangle.B * t0;

        Vector3 f1 = t0 + triangle.C;
        Vector3 f2 = t2 + triangle.C * f1;
        Vector3 f3 = triangle.A * t1 + triangle.B * t2 + triangle.C * f2;

        Vector3 g0 = f2 + triangle.A * (f1 + triangle.A);
        Vector3 g1 = f2 + triangle.B * (f1 + triangle.B);
        Vector3 g2 = f2 + triangle.C * (f1 + triangle.C);

        // Update integrals
        integral[0] += cross.X * f1.X;

        integral[1] += cross.X * f2.X;
        integral[2] += cross.Y * f2.Y;
        integral[3] += cross.Z * f2.Z;

        integral[4] += cross.X * f3.X;
        integral[5] += cross.Y * f3.Y;
        integral[6] += cross.Z * f3.Z;

        integral[7] += cross.X * (
          triangle.A.Y * g0.X + triangle.B.Y * g1.X + triangle.C.Y * g2.X
        );
        integral[8] += cross.Y * (
          triangle.A.Z * g0.Y + triangle.B.Z * g1.Y + triangle.C.Z * g2.Y
        );
        integral[9] += cross.Z * (
          triangle.A.X * g0.Z + triangle.B.X * g1.Z + triangle.C.X * g2.Z
        );

        // Update the surface area accumulation array
        this.surfaceArea += triangle.CircumferenceLength;
        this.accumulatedTriangleSurface[triangleIndex] = this.surfaceArea;

      }

      integral[0] /= 6.0f;
      integral[1] /= 24.0f;
      integral[2] /= 24.0f;
      integral[3] /= 24.0f;
      integral[4] /= 60.0f;
      integral[5] /= 60.0f;
      integral[6] /= 60.0f;
      integral[7] /= 120.0f;
      integral[8] /= 120.0f;
      integral[9] /= 120.0f;

      this.mass = integral[0];

      // Center of mass
      this.centerOfMass.X = integral[1] / this.mass;
      this.centerOfMass.Y = integral[2] / this.mass;
      this.centerOfMass.Z = integral[3] / this.mass;

      // Inertia tensor relative to center of mass
      this.inertiaTensor.M11 = integral[5] + integral[6] - this.mass * (
        this.centerOfMass.Y * this.centerOfMass.Y + this.centerOfMass.Z * this.centerOfMass.Z
      );
      this.inertiaTensor.M22 = integral[4] + integral[6] - this.mass * (
        this.centerOfMass.Z * this.centerOfMass.Z + this.centerOfMass.X * this.centerOfMass.X
      );
      this.inertiaTensor.M33 = integral[4] + integral[5] - this.mass * (
        this.centerOfMass.X * this.centerOfMass.X + this.centerOfMass.Y * this.centerOfMass.Y
      );
      this.inertiaTensor.M21 =
        -(integral[7] - this.mass * this.centerOfMass.X * this.centerOfMass.Y);
      this.inertiaTensor.M32 =
        -(integral[8] - this.mass * this.centerOfMass.Y * this.centerOfMass.Z);
      this.inertiaTensor.M31 =
        -(integral[9] - this.mass * this.centerOfMass.Z * this.centerOfMass.X);

      // Compute volume of arbitrary, non-convex meshes:
      // http://www.gamedev.net/community/forums/topic.asp?topic_id=520749

    }

    /// <summary>Generates sequential indices for a triangle vertex array</summary>
    /// <param name="count">Number of indices to generate</param>
    /// <returns>An array containing sequential indices</returns>
    private static int[] generateIndices(int count) {
      int[] indices = new int[count];
      for(int indicesIndex = 0; indicesIndex < count; ++indicesIndex)
        indices[indicesIndex] = indicesIndex;

      return indices;
    }

    /// <summary>The vertices that make up the mesh</summary>
    protected Vector3[] Vertices;
    /// <summary>Indices of the vertices from which to build triangles</summary>
    protected int[] Indices;

    /// <summary>Total amount of surface area of this mesh</summary>
    protected float surfaceArea;
    /// <summary>The center of mass in mesh coordinates</summary>
    protected Vector3 centerOfMass;
    /// <summary>The mass of this mesh assuming a material density of 1.0</summary>
    protected float mass;
    /// <summary>The meshes inertia tensor matrix, useful for physics calculations</summary>
    protected Matrix inertiaTensor;
    /// <summary>Accumulated surface of the triangles in this mesh</summary>
    /// <remarks>
    ///   This is used to perform a binary search in the RandomPointOnSurface() method
    /// </remarks>
    protected float[] accumulatedTriangleSurface;
  }

} // namespace Nuclex.Geometry.Volumes
