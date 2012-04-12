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
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Fonts {

  /// <summary>Stores the vertices for a string as filled 3D text</summary>
  public class FilledText : Text {

    /// <summary>Initializes a new filled string mesh</summary>
    /// <param name="font">Font from which the vertices will be taken</param>
    /// <param name="text">String of which to build a mesh</param>
    internal FilledText(VectorFont font, string text) {
      resizeVertexAndIndexArrays(font, text);
      buildStringMesh(font, text);
    }

    /// <summary>Builds the combined mesh for the letters in this string</summary>
    /// <param name="font">Vector font to take the vertex data from</param>
    /// <param name="text">String of which a mesh is to be built</param>
    private void buildStringMesh(VectorFont font, string text) {
      Vector2 position = Vector2.Zero;

      int baseVertexIndex = 0; // base index in the vertex array
      int baseIndexIndex = 0; // base index in the index array

      for(int characterIndex = 0; characterIndex < text.Length; ++characterIndex) {
        int fontCharacterIndex;

        // Only add this character to the mesh if there is an actual font character
        // for this unicode symbol in the font (characters not imported by the user
        // will be silently skipped -- this is the only sane option imho)
        if(font.CharacterMap.TryGetValue(text[characterIndex], out fontCharacterIndex)) {
          VectorFontCharacter character = font.Characters[fontCharacterIndex];

          // Import all of the vertices of this font (we need both the outline vertices
          // as well as the mesh supporting vertices)
          for(int index = 0; index < character.Vertices.Count; ++index) {
            this.vertices[baseVertexIndex + index] = new VertexPositionNormalTexture(
              new Vector3(character.Vertices[index] + position, 0.0f),
              Vector3.Forward, Vector2.Zero
            );
          }

          // Now bend the indices to the new location of the vertices in our
          // concatenated vertex array
          for(int faceIndex = 0; faceIndex < character.Faces.Count; ++faceIndex) {
            this.indices[baseIndexIndex + faceIndex * 3 + 0] =
              (short)(character.Faces[faceIndex].FirstVertexIndex + baseVertexIndex);
            this.indices[baseIndexIndex + faceIndex * 3 + 1] =
              (short)(character.Faces[faceIndex].SecondVertexIndex + baseVertexIndex);
            this.indices[baseIndexIndex + faceIndex * 3 + 2] =
              (short)(character.Faces[faceIndex].ThirdVertexIndex + baseVertexIndex);
          }

          // Advance the index pointers
          baseIndexIndex += character.Faces.Count * 3;
          baseVertexIndex += character.Vertices.Count;

          // Update the position to the next character
          position += character.Advancement;

        } // if
      } // for

      this.width = position.X;
      this.height = font.LineHeight;
      this.primitiveType = PrimitiveType.TriangleList;
    }

    /// <summary>Reserves the required space in the vertex and index arrays</summary>
    /// <param name="font">Font the vertices for the letters will be taken from</param>
    /// <param name="text">String of which a mesh will be built</param>
    private void resizeVertexAndIndexArrays(VectorFont font, string text) {
      int vertexCount = 0;
      int indexCount = 0;

      // Count the vertices and indices requires for all characters in the string
      for(int index = 0; index < text.Length; ++index) {
        int fontCharacterIndex;

        // Try to find the current character in the font's character map. If it isn't
        // there, we'll ignore it, just like the mesh creation routine does.
        if(font.CharacterMap.TryGetValue(text[index], out fontCharacterIndex)) {
          VectorFontCharacter character = font.Characters[fontCharacterIndex];

          vertexCount += character.Vertices.Count;
          indexCount += character.Faces.Count * 3;
        }
      }

      this.vertices = new VertexPositionNormalTexture[vertexCount];
      this.indices = new short[indexCount];
    }

  }

} // namespace Nuclex.Fonts
