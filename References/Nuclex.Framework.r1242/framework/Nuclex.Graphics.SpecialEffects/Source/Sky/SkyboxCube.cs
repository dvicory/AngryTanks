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
using Microsoft.Xna.Framework.Content;

namespace Nuclex.Graphics.SpecialEffects.Sky {

  /// <summary>Renders a skybox consisting of 6 separate faces</summary>
  /// <remarks>
  ///   <para>
  ///     This class doesn't make any assumptions about the effect and texture you're
  ///     using to render the sky box, it simply takes care of the vertex buffer setup
  ///     and allows you to conveniently render a skybox using your own textures,
  ///     effects and graphics device settings.
  ///   </para>
  ///   <para>
  ///     The skybox vertices do not provide any texture coordinates because the
  ///     texture 
  ///   </para>
  /// </remarks>
  public class SkyboxCube : IDisposable {

    /// <summary>Initializes as new skybox cube</summary>
    /// <param name="graphicsDevice">Graphics device the skybox cube lives on</param>
    public SkyboxCube(GraphicsDevice graphicsDevice) {
      this.graphicsDevice = graphicsDevice;

#if !XNA_4
      // Set up a vertex declaration for the skybox vertices
      this.vertexDeclaration = new VertexDeclaration(
        graphicsDevice,
        SkyboxVertex.VertexElements
      );
#endif
      this.vertexBuffer = new VertexBuffer(
        graphicsDevice, typeof(SkyboxVertex), vertices.Length, BufferUsage.None
      );
      this.vertexBuffer.SetData<SkyboxVertex>(vertices);
    }

    /// <summary>
    ///   Immediately releases all resources owned by the instance
    /// </summary>
    public void Dispose() {
#if !XNA_4
      if(this.vertexDeclaration != null) {
        this.vertexDeclaration.Dispose();
        this.vertexDeclaration = null;
      }
#endif
      if(this.vertexBuffer != null) {
        this.vertexBuffer.Dispose();
        this.vertexBuffer = null;
      }
    }

    /// <summary>
    ///   Prepares the skybox for drawing by selecting its vertex buffer and adjusting
    ///   the state of the graphics device as neccessary
    /// </summary>
    public void AssignVertexBuffer() {
#if XNA_4
      this.graphicsDevice.SetVertexBuffer(this.vertexBuffer);
#else
      this.graphicsDevice.Vertices[0].SetSource(
        this.vertexBuffer, 0, SkyboxVertex.Stride
      );
      this.graphicsDevice.VertexDeclaration = this.vertexDeclaration;
#endif
    }

    /// <summary>Draws the northern face of the skybox</summary>
    public void DrawNorthernFace() {
      this.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
    }

    /// <summary>Draws the eastern face of the skybox</summary>
    public void DrawEasternFace() {
      this.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 4, 2);
    }

    /// <summary>Draws the southern face of the skybox</summary>
    public void DrawSouthernFace() {
      this.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 8, 2);
    }

    /// <summary>Draws the western face of the skybox</summary>
    public void DrawWesternFace() {
      this.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 2);
    }

    /// <summary>Draws the upper face of the skybox</summary>
    public void DrawUpperFace() {
      this.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 16, 2);
    }

    /// <summary>Draws the lower face of the skybox</summary>
    public void DrawLowerFace() {
      this.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 20, 2);
    }

    /// <summary>Vertices used to construct a skybox</summary>
    private static readonly SkyboxVertex[] vertices = {

      // Northern face
      new SkyboxVertex(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(1.0f, 0.0f)),
      new SkyboxVertex(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(0.0f, 0.0f)),
      new SkyboxVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1.0f, 1.0f)),
      new SkyboxVertex(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(0.0f, 1.0f)),

      // Eastern face
      new SkyboxVertex(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1.0f, 0.0f)),
      new SkyboxVertex(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(0.0f, 0.0f)),
      new SkyboxVertex(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1.0f, 1.0f)),
      new SkyboxVertex(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(0.0f, 1.0f)),

      // Southern face
      new SkyboxVertex(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1.0f, 0.0f)),
      new SkyboxVertex(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0.0f, 0.0f)),
      new SkyboxVertex(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1.0f, 1.0f)),
      new SkyboxVertex(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0.0f, 1.0f)),

      // Western face
      new SkyboxVertex(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(1.0f, 0.0f)),
      new SkyboxVertex(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0.0f, 0.0f)),
      new SkyboxVertex(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(1.0f, 1.0f)),
      new SkyboxVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0.0f, 1.0f)),

      // Upper face
      new SkyboxVertex(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(1.0f, 0.0f)),
      new SkyboxVertex(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(0.0f, 0.0f)),
      new SkyboxVertex(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(1.0f, 1.0f)),
      new SkyboxVertex(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(0.0f, 1.0f)),

      // Lower face
      new SkyboxVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1.0f, 0.0f)),
      new SkyboxVertex(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(0.0f, 0.0f)),
      new SkyboxVertex(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(1.0f, 1.0f)),
      new SkyboxVertex(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(0.0f, 1.0f))

    };

    /// <summary>GraphicsDevice the skybox is rendered with</summary>
    private GraphicsDevice graphicsDevice;
    /// <summary>Vertex buffer storing the vertices of the skybox</summary>
    private VertexBuffer vertexBuffer;
#if !XNA_4
    /// <summary>Vertex declaration for the skybox vertices</summary>
    private VertexDeclaration vertexDeclaration;
#endif
  }

} // namespace Nuclex.Graphics.SpecialEffects.Sky
