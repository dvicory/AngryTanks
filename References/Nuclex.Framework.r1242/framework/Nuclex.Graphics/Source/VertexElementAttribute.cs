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

using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics {

  /// <summary>
  ///   Describes the usage and purpose of an element in a vertex structure
  /// </summary>
  /// <remarks>
  ///   Based on ideas from Michael Popoloski's article on gamedev.net:
  ///   http://www.gamedev.net/reference/programming/features/xnaVertexElement/
  /// </remarks>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public sealed class VertexElementAttribute : Attribute {

    /// <summary>Initializes a new vertex element attribute</summary>
    /// <param name="usage">What purpose the vertex element will serve</param>
    public VertexElementAttribute(VertexElementUsage usage) {
      this.usage = usage;
    }

    /// <summary>Initializes a new vertex element attribute</summary>
    /// <param name="usage">What purpose the vertex element will serve</param>
    /// <param name="format">Format in in which the data for this element is provided</param>
    public VertexElementAttribute(VertexElementUsage usage, VertexElementFormat format) :
      this(usage) {
      this.format = format;
      this.formatProvided = true;
    }

    /// <summary>Index of the vertex buffer that will contain this element</summary>
    /// <remarks>
    ///   Data for vertices can come from multiple vertex buffers. For example, you can store
    ///   the positions of your vertices in one vertex buffer and then store the texture
    ///   coordinates in an entirely different vertex buffer, only to combine them at rendering
    ///   time by using the two vertex buffers simultaneously two feed your shader.
    /// </remarks>
    public int Stream {
      get { return this.stream; }
      set { this.stream = value; }
    }

    /// <summary>What purpose the vertex element is going to be used for</summary>
    public VertexElementUsage Usage {
      get { return this.usage; }
    }

    /// <summary>Format the vertex element's data is provided in</summary>
    public VertexElementFormat Format {
      get { return this.format; }
    }

#if !XNA_4
    /// <summary>Method used to interpret the element's data</summary>
    public VertexElementMethod Method {
      get { return this.method; }
      set { this.method = value; }
    }
#endif

    /// <summary>Index of the element when multiple elements share the same usage</summary>
    public byte UsageIndex {
      get { return this.usageIndex; }
      set { this.usageIndex = value; }
    }

    /// <summary>True if a format has been provided for the vertex element</summary>
    internal bool FormatProvided {
      get { return this.formatProvided; }
    }

    /// <summary>Index of the vertex buffer that will contain this element</summary>
    private int stream;
    /// <summary>What purpose the vertex element is going to be used for</summary>
    private VertexElementUsage usage;
    /// <summary>Format the vertex element's data is provided in</summary>
    private VertexElementFormat format;
#if !XNA_4
    /// <summary>Method used to interpret the element's data</summary>
    private VertexElementMethod method;
#endif
    /// <summary>Index of the element when multiple elements share the same usage</summary>
    private byte usageIndex;
    /// <summary>Whether a format has been provided for this element</summary>
    private bool formatProvided;

  }

} // namespace Nuclex.Graphics
