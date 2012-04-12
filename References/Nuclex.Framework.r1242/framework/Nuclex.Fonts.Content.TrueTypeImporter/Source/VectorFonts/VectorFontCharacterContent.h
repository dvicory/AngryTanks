#pragma region CPL License
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
#pragma endregion

#pragma once

#include "../FreeTypeManager.h" // freetype API

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Stores informations about a glyph in a vector font</summary>
  public ref class VectorFontCharacterContent sealed {

    #pragma region struct Outline

    /// <summary>Stores the starting index and the vertex count of a character outline</summary>
    public: value struct Outline {

      /// <summary>Initializes a new character outline</summary>
      /// <param name="startVertexIndex">Index of the vertex with which the outline starts</param>
      /// <param name="vertexCount">Number of vertices in this outline</param>
      public: Outline(int startVertexIndex, int vertexCount) :
        StartVertexIndex(startVertexIndex),
        VertexCount(vertexCount) {}
        
      /// <summary>Index of the vertex with which the outline begins</summary>
      public: int StartVertexIndex;
      /// <summary>Total number of vertices the outline consists of</summary>
      public: int VertexCount;

    };

    #pragma endregion // struct Outline

    #pragma region struct Face

    /// <summary>Stores three vertex indices forming a triangle</summary>
    public: value struct Face {

      /// <summary>Initializes a new character face triangle</summary>
      /// <param name="firstVertexIndex">Index of the triangle's first vertex</param>
      /// <param name="secondVertexIndex">Index of the triangle's second vertex</param>
      /// <param name="thirdVertexIndex">Index of the triangle's third vertex</param>
      public: Face(int firstVertexIndex, int secondVertexIndex, int thirdVertexIndex) :
        FirstVertexIndex(firstVertexIndex),
        SecondVertexIndex(secondVertexIndex),
        ThirdVertexIndex(thirdVertexIndex) {}

      /// <summary>Index of the first vertex of the triangle</summary>
      public: int FirstVertexIndex;
      /// <summary>Index of the second vertex of the triangle</summary>
      public: int SecondVertexIndex;
      /// <summary>Index of the third vertex of the triangle</summary>
      public: int ThirdVertexIndex;
    };

    #pragma endregion // struct Face

    /// <summary>Initializes a new Character instance</summary>
    internal: VectorFontCharacterContent() {
      this->vertices = gcnew System::Collections::Generic::List<
        Microsoft::Xna::Framework::Vector2
      >();
      this->outlines = gcnew System::Collections::Generic::List<
        Nuclex::Fonts::Content::VectorFontCharacterContent::Outline
      >();
      this->faces = gcnew System::Collections::Generic::List<
        Nuclex::Fonts::Content::VectorFontCharacterContent::Face
      >();
    }

    /// <summary>By how much to advance the cursor after drawing this character</summary>
    internal: property Microsoft::Xna::Framework::Vector2 Advancement {
      Microsoft::Xna::Framework::Vector2 get() { return this->advancement; }
      void set(Microsoft::Xna::Framework::Vector2 value) { this->advancement = value; }
    }

    /// <summary>Vertices for this character</summary>
    /// <remarks>
    ///   This contains the vertices required to draw the outline of the character
    ///   as well as supporting vertices required to draw the character's face as
    ///   a series of triangles. If you're only interested in a character's outlines,
    ///   you can ignore any vertices with an index above the EndVertex of
    ///   the lastmost outline contained in the Outlines list.
    /// </remarks>
    internal: property System::Collections::Generic::List<
      Microsoft::Xna::Framework::Vector2
    > ^Vertices {
      System::Collections::Generic::List<Microsoft::Xna::Framework::Vector2> ^get() {
        return this->vertices;
      }
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
    internal: property System::Collections::Generic::List<
      Nuclex::Fonts::Content::VectorFontCharacterContent::Outline
    > ^Outlines {
      System::Collections::Generic::List<
        Nuclex::Fonts::Content::VectorFontCharacterContent::Outline
      > ^get() {
        return this->outlines;
      }
    }

    /// <summary>
    ///   Specifies between which vertices triangles have to be drawn to draw a
    ///   polygon-filled character.
    /// </summary>
    internal: property System::Collections::Generic::List<
      Nuclex::Fonts::Content::VectorFontCharacterContent::Face
    > ^Faces {
      System::Collections::Generic::List<
        Nuclex::Fonts::Content::VectorFontCharacterContent::Face
      > ^get() {
        return this->faces;
      }
    }

    /// <summary>By how much to advance the cursor after drawing this character</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Advancement", AllowNull = false
      )
    ]
    Microsoft::Xna::Framework::Vector2 advancement;

    /// <summary>Vertices for this character</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Vertices", AllowNull = false
      )
    ]
    System::Collections::Generic::List<Microsoft::Xna::Framework::Vector2> ^vertices;

    /// <summary>Vertex ranges to be connected for drawing the character's outlines</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Outlines", AllowNull = false
      )
    ]
    System::Collections::Generic::List<
      Nuclex::Fonts::Content::VectorFontCharacterContent::Outline
    > ^outlines;

    /// <summary>Vertex indices to be connected for drawing the character's faces</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Faces", AllowNull = false
      )
    ]
    System::Collections::Generic::List<
      Nuclex::Fonts::Content::VectorFontCharacterContent::Face
    > ^faces;

  };

}}} // namespace Nuclex::Fonts::Content
