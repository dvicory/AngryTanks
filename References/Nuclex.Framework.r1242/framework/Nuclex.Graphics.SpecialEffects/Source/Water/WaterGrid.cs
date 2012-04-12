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
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics.SpecialEffects.Water {

  /// <summary>Creates and manages a segmented grid of vertices</summary>
  public class WaterGrid : StaticMesh<WaterVertex> {

    /// <summary>Initializes a new grid drawn as a single quad</summary>
    /// <param name="graphicsDevice">Graphics device the grid will be created on</param>
    /// <param name="min">
    ///   Lesser coordinates of the world region that will be covered by the grid
    /// </param>
    /// <param name="max">
    ///   Greater coordinates of the world region that will be covered by the grid
    /// </param>
    public WaterGrid(GraphicsDevice graphicsDevice, Vector2 min, Vector2 max) :
      this(graphicsDevice, min, max, 1, 1) { }

    /// <summary>Initializes a new grid consisting of one or more quads</summary>
    /// <param name="graphicsDevice">Graphics device the grid will be created on</param>
    /// <param name="min">
    ///   Lesser coordinates of the world region that will be covered by the grid
    /// </param>
    /// <param name="max">
    ///   Greater coordinates of the world region that will be covered by the grid
    /// </param>
    /// <param name="segmentsX">
    ///   Number of segments the grid will have on the X axis
    /// </param>
    /// <param name="segmentsZ">
    ///   Number of segments the grid will have on the Z axis
    /// </param>
    public WaterGrid(
      GraphicsDevice graphicsDevice,
      Vector2 min, Vector2 max,
      int segmentsX, int segmentsZ
    ) :
      base(graphicsDevice, getIndexCount(segmentsX, segmentsZ)) {        

      this.graphicsDevice = graphicsDevice;

      // Set up a vertex declaration for the water vertices
#if !XNA_4
      this.vertexDeclaration = new VertexDeclaration(
        graphicsDevice, WaterVertex.VertexElements
      );
#endif

      // Create and fill the vertex buffer
      {
        WaterVertex[] vertices = buildVertexArray(min, max, segmentsX, segmentsZ);
        this.vertexCount = vertices.Length;
        this.vertexBuffer = new VertexBuffer(
          graphicsDevice, typeof(WaterVertex), vertices.Length, BufferUsage.None
        );
        this.vertexBuffer.SetData<WaterVertex>(vertices);
      }

      // Create and fill the index buffer
      {
        short[] indices = TriangleStripIndexBuilder.BuildAlternatingStrip(
          segmentsX, segmentsZ
        );
        this.indexCount = indices.Length;
        this.indexBuffer = new IndexBuffer(
          graphicsDevice, typeof(short), indices.Length, BufferUsage.None
        );
        this.indexBuffer.SetData<short>(indices);
      }
    }

    /// <summary>
    ///   Immediately releases all resources owned by the instance
    /// </summary>
    public override void Dispose() {
      if(this.vertexDeclaration != null) {
        this.vertexDeclaration.Dispose();
        this.vertexDeclaration = null;
      }
      if(this.indexBuffer != null) {
        this.indexBuffer.Dispose();
        this.indexBuffer = null;
      }
      if(this.vertexBuffer != null) {
        this.vertexBuffer.Dispose();
        this.vertexBuffer = null;
      }
      
      base.Dispose();
    }

    /// <summary>Number of vertices in the grid</summary>
    public int VertexCount {
      get { return this.vertexCount; }
    }

    /// <summary>Number of indices required to draw the grid</summary>
    public int IndexCount {
      get { return this.indexCount; }
    }

    /// <summary>Number of primitives that need to be drawn for the grid</summary>
    public int PrimitiveCount {
      get { return this.indexCount - 2; }
    }

    /// <summary>The type of primitive used to render the grid</summary>
    public PrimitiveType PrimitiveType {
      get { return PrimitiveType.TriangleStrip; }
    }

    /// <summary>Vertex buffer containing the vertices making up the plane</summary>
    public new VertexBuffer VertexBuffer {
      get { return this.vertexBuffer; }
    }

    /// <summary>Index buffer used to build polygons from the vertices</summary>
    public IndexBuffer IndexBuffer {
      get { return this.indexBuffer; }
    }

#if !XNA_4

    /// <summary>Vertex declaration describing the format of the vertex structure</summary>
    public new VertexDeclaration VertexDeclaration {
      get { return this.vertexDeclaration; }
    }

#endif

    /// <summary>
    ///   Creates a flat plane of vertices optimized for triangle strip drawing
    /// </summary>
    /// <param name="min">X and Y coordinates for the upper left side of the plane</param>
    /// <param name="max">X and Y coordinates for the lower right side of the plane</param>
    /// <param name="segmentsX">Number of horizontal subdivisions in the plane</param>
    /// <param name="segmentsZ">Number of vertical subdivisions in the plane</param>
    private static WaterVertex[] buildVertexArray(
      Vector2 min, Vector2 max, int segmentsX, int segmentsZ
    ) {
      int verticesX = segmentsX + 1;
      int verticesZ = segmentsZ + 1;
      WaterVertex[] vertices = new WaterVertex[verticesX * verticesZ];

      int vertexIndex = 0;
      Vector3 position = Vector3.Zero;
      for(int z = segmentsZ; z >= 0; --z) {
        float relativeZ = (float)z / (float)segmentsZ;
        position.Z = MathHelper.Lerp(min.Y, max.Y, relativeZ);

        for(int x = 0; x <= segmentsX; ++x) {
          float relativeX = (float)x / (float)segmentsX;
          position.X = MathHelper.Lerp(min.X, max.X, relativeX);

          vertices[vertexIndex].Position = position;
          vertices[vertexIndex].TextureCoordinate = new Vector2(
            relativeX * 100.0f, relativeZ * 100.0f
            // TODO: Magic number here. Implement configurable texture coordinate scale
          );

          ++vertexIndex;
        }
      }

      return vertices;
    }

    /// <summary>
    ///   Calculates the number of indices required for the water surface
    /// </summary>
    /// <param name="segmentsX">Number of segments in X direction</param>
    /// <param name="segmentsZ">Number of segments in Z direction</param>
    /// <returns>The number of indices required</returns>
    private static int getIndexCount(int segmentsX, int segmentsZ) {
      return TriangleStripIndexBuilder.CountAlternatingStripIndices(segmentsX, segmentsZ);
    }

    /// <summary>Number of vertices in the grid</summary>
    private int vertexCount;
    /// <summary>Number of indices required to draw the grid</summary>
    private int indexCount;

    /// <summary>Vertex declaration for the vertices in the grid</summary>
    private VertexDeclaration vertexDeclaration;
    /// <summary>Vertex buffer containing the grid vertices</summary>
    private VertexBuffer vertexBuffer;
    /// <summary>Index buffer containing the indices to the vertex buffer</summary>
    private IndexBuffer indexBuffer;
    /// <summary>Graphics device the grid will be rendered with</summary>
    private GraphicsDevice graphicsDevice;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Water
