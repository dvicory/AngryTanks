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

using Microsoft.Xna.Framework;

namespace Nuclex.Fonts {

  /// <summary>Stores the data of a character in a vector font</summary>
  /// <remarks>
  ///   <para>
  ///     Each character in a vector font has an array of vertices that store the
  ///     outline points for the font and in some cases contains additional
  ///     supporting vertices required to draw filled text with triangles.
  ///   </para>
  ///   <para>
  ///     You can either access this data any make use of it for your own purposes,
  ///     or use one of the vector font's provided methods for constructing an
  ///     outline font, a flat font or an extruded font.
  ///   </para>
  /// </remarks>
  public class VectorFontCharacter {

    #region struct Outline

    /// <summary>Stores the starting index and the vertex count of a character outline</summary>
    public struct Outline {

      /// <summary>Initializes a new character outline</summary>
      /// <param name="startVertexIndex">Index of the vertex with which the outline starts</param>
      /// <param name="vertexCount">Number of vertices in this outline</param>
      public Outline(int startVertexIndex, int vertexCount) {
        this.StartVertexIndex = startVertexIndex;
        this.VertexCount = vertexCount;
      }

      /// <summary>Index of the vertex with which the outline begins</summary>
      public int StartVertexIndex;
      /// <summary>Total number of vertices the outline consists of</summary>
      public int VertexCount;

    }

    #endregion // struct Outline

    #region struct Face

    /// <summary>Stores three vertex indices forming a triangle</summary>
    public struct Face {

      /// <summary>Initializes a new character face triangle</summary>
      /// <param name="firstVertexIndex">Index of the triangle's first vertex</param>
      /// <param name="secondVertexIndex">Index of the triangle's second vertex</param>
      /// <param name="thirdVertexIndex">Index of the triangle's third vertex</param>
      public Face(int firstVertexIndex, int secondVertexIndex, int thirdVertexIndex) {
        this.FirstVertexIndex = firstVertexIndex;
        this.SecondVertexIndex = secondVertexIndex;
        this.ThirdVertexIndex = thirdVertexIndex;
      }

      /// <summary>Index of the first vertex of the triangle</summary>
      public int FirstVertexIndex;
      /// <summary>Index of the second vertex of the triangle</summary>
      public int SecondVertexIndex;
      /// <summary>Index of the third vertex of the triangle</summary>
      public int ThirdVertexIndex;
    }

    #endregion // struct Face

    /// <summary>Initializes new vector font character</summary>
    /// <param name="advancement">
    ///   By what to advance the pen after the character was drawn
    /// </param>
    /// <param name="vertices">Vertices used by this character</param>
    /// <param name="outlines">Vertex indices for drawing the character's outline</param>
    /// <param name="faces">Vertex indices for filling the character</param>
    internal VectorFontCharacter(
      Vector2 advancement, List<Vector2> vertices, List<Outline> outlines, List<Face> faces
    ) {
      this.advancement = advancement;
      this.vertices = vertices;
      this.outlines = outlines;
      this.faces = faces;
    }

    /// <summary>By how much to advance the cursor after drawing this character</summary>
    public Vector2 Advancement {
      get { return this.advancement; }
    }

    /// <summary>Vertices for this character</summary>
    /// <remarks>
    ///   This contains the vertices required to draw the outline of the character
    ///   as well as supporting vertices required to draw the character's face as
    ///   a series of triangles. If you're only interested in a character's outlines,
    ///   you can ignore any vertices with an index above the EndVertex of
    ///   the lastmost outline contained in the Outlines list.
    /// </remarks>
    public List<Vector2> Vertices {
      get { return this.vertices; }
    }

    /// <summary>
    ///   Specifies which vertices have to be connected to draw the outlines
    ///   of the character.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A character can have more than one outline. For example, the equals sign ('=')
    ///     has two unconnected shapes that require two outlines to be drawn. In this
    ///     case, you'd find two outlines, the first one specifying the starting and ending
    ///     vertex for the first stroke and the second one specifying the starting and
    ///     ending vertex for the second stroke.
    ///   </para>
    ///   <para>
    ///     The vertex range specified by each outline should be handled as a single
    ///     line strip (draw a line from the first to the second vertex, then from the
    ///     second to the third, and so on). The final vertex needs to be connected
    ///     to the first vertex again to close the outline.
    ///   </para>
    /// </remarks>
    public List<Outline> Outlines {
      get { return this.outlines; }
    }

    /// <summary>
    ///   Specifies between which vertices triangles have to be drawn to draw a
    ///   polygon-filled character.
    /// </summary>
    public List<Face> Faces {
      get { return this.faces; }
    }

    /// <summary>How far to advance the cursor after this character is rendered</summary>
    private Vector2 advancement;
    /// <summary>Vertices used by this character</summary>
    private List<Vector2> vertices;
    /// <summary>Vertex index ranges to use for drawing the character's outlines</summary>
    private List<Outline> outlines;
    /// <summary>Vertex indices to use for filling the character with triangles</summary>
    private List<Face> faces;

  }

} // namespace Nuclex.Fonts
