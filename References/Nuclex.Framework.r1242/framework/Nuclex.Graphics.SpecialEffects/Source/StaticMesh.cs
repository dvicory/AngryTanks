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
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Nuclex.Graphics.SpecialEffects {

  /// <summary>Base class for objects requiring static geometry</summary>
  public class StaticMesh<VertexType> : IDisposable
    where VertexType : struct
#if XNA_4
, IVertexType
#endif
 {

    /// <summary>Initializes a new graphics resource keeper for a static mesh</summary>
    /// <param name="graphicsDevice">Graphics device the mesh lives on</param>
    /// <param name="vertexCount">Number of vertices that will be required</param>
    protected StaticMesh(
      GraphicsDevice graphicsDevice, int vertexCount
    ) {
      this.GraphicsDevice = graphicsDevice;

#if XNA_4
      // Create a new vertex buffer with the requested size
      this.VertexBuffer = new VertexBuffer(
        graphicsDevice, typeof(VertexType), vertexCount, BufferUsage.WriteOnly
      );
#else
      // Create the vertex declaration
      this.stride = VertexDeclarationHelper.GetStride<VertexType>();
      this.VertexDeclaration = new VertexDeclaration(
        graphicsDevice, VertexDeclarationHelper.BuildElementList<VertexType>()
      );

      try {
        this.ownsVertexDeclaration = true;

        // Create a new vertex buffer with the requested size
        this.VertexBuffer = new VertexBuffer(
          graphicsDevice, typeof(VertexType), vertexCount, BufferUsage.WriteOnly
        );
      }
      catch(Exception) {
        this.VertexDeclaration.Dispose();
        this.VertexDeclaration = null;
        throw;
      }
#endif
    }

#if !XNA_4
    /// <summary>Initializes a new graphics resource keeper for a static mesh</summary>
    /// <param name="graphicsDevice">Graphics device the mesh lives on</param>
    /// <param name="vertexDeclaration">Vertex declaration that will be used</param>
    /// <param name="stride">Distance, in bytes, from one vertex to the next</param>
    /// <param name="vertexCount">Number of vertices that will be required</param>
    protected StaticMesh(
      GraphicsDevice graphicsDevice,
      VertexDeclaration vertexDeclaration, int stride,
      int vertexCount
    ) {
      this.GraphicsDevice = graphicsDevice;

      // Take over the vertex declaration we were provided with
      this.stride = stride;
      this.VertexDeclaration = vertexDeclaration;

      // Create a new vertex buffer with the requested size
      this.VertexBuffer = new VertexBuffer(
        graphicsDevice, typeof(VertexType), vertexCount, BufferUsage.WriteOnly
      );
    }
#endif

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public virtual void Dispose() {
      if(this.VertexBuffer != null) {
        this.VertexBuffer.Dispose();
        this.VertexBuffer = null;
      }
#if !XNA_4
      if(this.VertexDeclaration != null) {
        if(this.ownsVertexDeclaration) {
          this.VertexDeclaration.Dispose();
        }
        this.VertexDeclaration = null;
      }
#endif
    }

    /// <summary>Selects the static meshes' vertices for drawing</summary>
    protected virtual void Select() {
#if XNA_4
      this.GraphicsDevice.SetVertexBuffer(this.VertexBuffer);
#else
      this.GraphicsDevice.Vertices[0].SetSource(
        this.VertexBuffer, 0, this.stride
      );
#endif
    }

    /// <summary>Graphics device the mesh is being rendered on</summary>
    protected GraphicsDevice GraphicsDevice;
    /// <summary>Vertex buffer containing the vertices for the static mesh</summary>
    protected VertexBuffer VertexBuffer;
#if !XNA_4
    /// <summary>Vertex declaration being used for these vertices</summary>
    protected VertexDeclaration VertexDeclaration;
    /// <summary>
    ///   Whether the static mesh is responsible for disposing the vertex declaration
    /// </summary>
    private bool ownsVertexDeclaration;
    /// <summary>Distance, in bytes, from one vertex to the next</summary>
    private int stride;
#endif
  }

} // namespace Nuclex.Graphics.SpecialEffects
