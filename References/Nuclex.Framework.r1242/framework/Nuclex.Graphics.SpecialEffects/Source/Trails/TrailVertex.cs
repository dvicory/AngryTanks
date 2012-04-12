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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.Graphics;

namespace Nuclex.Graphics.SpecialEffects.Trails {

  /// <summary>Contains the definitions for a trail vertex</summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct TrailVertex
#if XNA_4
 : IVertexType
#endif
 {

    /// <summary>Initializs a new trail vertex</summary>
    /// <param name="position">Position of the vertex in space</param>
    /// <param name="textureCoordinate">Texture coordinates at this vertex</param>
    public TrailVertex(Vector3 position, Vector2 textureCoordinate) {
      this.Position = position;
      this.TextureCoordinate = textureCoordinate;
    }

    /// <summary>Coordinates of the vertex</summary>
    [VertexElement(VertexElementUsage.Position)]
    public Vector3 Position;

    /// <summary>Texture coordinates for all texture layers</summary>
    [VertexElement(VertexElementUsage.TextureCoordinate)]
    public Vector2 TextureCoordinate;

#if XNA_4

    /// <summary>Provides a declaration for this vertex type</summary>
    VertexDeclaration IVertexType.VertexDeclaration {
      get { return TrailVertex.VertexDeclaration; }
    }

    /// <summary>Vertex declaration for this vertex structure</summary>
    public static readonly VertexDeclaration VertexDeclaration =
      new VertexDeclaration(VertexDeclarationHelper.BuildElementList<TrailVertex>());

#else

    /// <summary>Number of bytes from one vertex to the next</summary>
    public static readonly int Stride = VertexDeclarationHelper.GetStride<TrailVertex>();

    /// <summary>Vertex elements of this type of vertex</summary>
    public static readonly VertexElement[] VertexElements =
      VertexDeclarationHelper.BuildElementList<TrailVertex>();

#endif

  }

} // namespace Nuclex.Graphics.SpecialEffects.Trails
