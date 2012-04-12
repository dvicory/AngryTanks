#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2008 Nuclex Development Labs

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

using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Fonts {

  /// <summary>Base class for vector font texts</summary>
  public abstract class Text {

    /// <summary>Vertices containing the text's outline or face coordinates</summary>
    public VertexPositionNormalTexture[] Vertices {
      get { return this.vertices; }
    }

    /// <summary>
    ///   Indices describing which vertices to connect by lines or triangles
    /// </summary>
    public short[] Indices {
      get { return this.indices; }
    }

    /// <summary>Type of primitives to draw</summary>
    public PrimitiveType PrimitiveType {
      get { return this.primitiveType; }
    }

    /// <summary>Total width of the string in world units</summary>
    public float Width {
      get { return this.width; }
    }

    /// <summary>Total height of the string in world units</summary>
    public float Height {
      get { return this.height; }
    }

    /// <summary>Vertices containing the text's outline or face coordinates</summary>
    protected VertexPositionNormalTexture[] vertices;
    /// <summary>
    ///   Indices describing which vertices to connect by lines or triangles
    /// </summary>
    protected short[] indices;
    /// <summary>Type of primitives to draw</summary>
    protected PrimitiveType primitiveType;
    /// <summary>Total width of the string in world units</summary>
    protected float width;
    /// <summary>Total height of the string in world units</summary>
    protected float height;

  }

} // namespace Nuclex.Fonts
