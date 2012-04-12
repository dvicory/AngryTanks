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

namespace Nuclex.Graphics.SpecialEffects {

  /// <summary>Manages the graphics resources for an indexed static mesh</summary>
  /// <typeparam name="VertexType">Type of the vertices used in the mesh</typeparam>
  public class IndexedStaticMesh<VertexType> : StaticMesh<VertexType>
    where VertexType : struct 
#if XNA_4
, IVertexType
#endif
    {

    /// <summary>Initializes a new graphics resource keeper for indexed meshes</summary>
    /// <param name="graphicsDevice">Graphics device the mesh will be rendered on</param>
    /// <param name="vertexCount">Number of vertices used by the mesh</param>
    /// <param name="indexCount">Number of indices in the mesh</param>
    protected IndexedStaticMesh(
      GraphicsDevice graphicsDevice,
      int vertexCount,
      int indexCount
    ) :
      base(graphicsDevice, vertexCount) {
      try {
        this.IndexBuffer = new IndexBuffer(
          base.GraphicsDevice, typeof(short), indexCount, BufferUsage.WriteOnly
        );
      }
      catch(Exception) {
        base.Dispose();
        throw;
      }
    }

#if !XNA_4

    /// <summary>Initializes a new graphics resource keeper for indexed meshes</summary>
    /// <param name="graphicsDevice">Graphics device the mesh will be rendered on</param>
    /// <param name="vertexDeclaration">Vertex declaration that will be used</param>
    /// <param name="stride">Distance, in bytes, from one vertex to the next</param>
    /// <param name="vertexCount">Number of vertices used by the mesh</param>
    /// <param name="indexCount">Number of indices in the mesh</param>
    protected IndexedStaticMesh(
      GraphicsDevice graphicsDevice,
      VertexDeclaration vertexDeclaration, int stride,
      int vertexCount,
      int indexCount
    ) :
      base(graphicsDevice, vertexDeclaration, stride, vertexCount) {
      try {
        this.IndexBuffer = new IndexBuffer(
          base.GraphicsDevice, typeof(short), indexCount, BufferUsage.WriteOnly
        );
      }
      catch(Exception) {
        base.Dispose();
        throw;
      }
    }

#endif

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public override void Dispose() {
      if(this.IndexBuffer != null) {
        this.IndexBuffer.Dispose();
        this.IndexBuffer = null;
      }
      base.Dispose();
    }

    /// <summary>Selects the indexed meshes' vertices and indices for drawing</summary>
    protected override void Select() {
      base.Select();
      this.GraphicsDevice.Indices = this.IndexBuffer;
    }

    /// <summary>Index buffer containing the indices of the static mesh</summary>
    protected IndexBuffer IndexBuffer;

  }

} // namespace Nuclex.Graphics.SpecialEffects
