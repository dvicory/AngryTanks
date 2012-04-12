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

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Simple particle for reference and testing</summary>
  /// <remarks>
  ///   <para>
  ///     We're fusing the vertex structure with the particle structure here. This
  ///     allows the particle renderer to directly copy the particles into
  ///     the vertex buffer with the drawback that any particle fields not used
  ///     by the GPU will also be sent over the bus and stored in GPU memory.
  ///   </para>
  ///   <para>
  ///     Another option is to use different particle and vertex structures
  ///     and make your particle renderer convert one into the other. If your
  ///     particle structure has many fields not relevant to rendering, this may
  ///     be the faster route.
  ///   </para>
  /// </remarks>
  [StructLayout(LayoutKind.Sequential)]
  public struct SimpleParticle
#if XNA_4
 : IVertexType
#endif
 {

    /// <summary>Initializes a new simple particle</summary>
    /// <param name="position">Initial position of the particle</param>
    /// <param name="velocity">Velocity the particle is moving at</param>
    public SimpleParticle(Vector3 position, Vector3 velocity) {
      this.Position = position;
      this.Velocity = velocity;
    }

    /// <summary>Current position of the particle in space</summary>
    [VertexElement(VertexElementUsage.Position)]
    public Vector3 Position;

    /// <summary>Velocity the particle is moving at</summary>
    public Vector3 Velocity;

#if XNA_4

    /// <summary>Provides a declaration for this vertex type</summary>
    VertexDeclaration IVertexType.VertexDeclaration {
      get { return SimpleParticle.VertexDeclaration; }
    }

    /// <summary>Vertex declaration for this vertex structure</summary>
    public static readonly VertexDeclaration VertexDeclaration =
      new VertexDeclaration(
        Marshal.SizeOf(typeof(SimpleParticle)), // because of private data in vertex
        VertexDeclarationHelper.BuildElementList<SimpleParticle>()
      );

#else

    /// <summary>
    ///   Description of this vertex structure for creating a vertex declaration
    /// </summary>
    public static readonly VertexElement[] VertexElements =
      VertexDeclarationHelper.BuildElementList<SimpleParticle>();

    /// <summary>Offset, in bytes, from one particle to the next</summary>
    public static readonly int SizeInBytes =
      VertexDeclarationHelper.GetStride<SimpleParticle>();

#endif

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
