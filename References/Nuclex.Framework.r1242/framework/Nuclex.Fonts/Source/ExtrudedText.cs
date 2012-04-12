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

using TextVertex = Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture;

namespace Nuclex.Fonts {

  /// <summary>Stores the vertices for an extruded string mesh</summary>
  /// <remarks>
  ///   The extruded mesh will always be extruded by 1.0 units centered about the
  ///   middle of the extrusion. This allows you to scale the text's extrusion
  ///   level at rendering time for free using the transformation matrix of the
  ///   rendered text.
  /// </remarks>
  public class ExtrudedText : Text {

    /// <summary>Initializes a new extruded string mesh</summary>
    /// <param name="font">Font from which the vertices will be taken</param>
    /// <param name="text">String of which to build a mesh</param>
    internal ExtrudedText(VectorFont font, string text) {
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
          // as well as the mesh supporting vertices). Each vertex is imported two times,
          // once with a z coordinate of -0.5 and once with a z coordinate of +0.5
          for(int vertexIndex = 0; vertexIndex < character.Vertices.Count; ++vertexIndex) {
            Vector2 adjustedPosition = character.Vertices[vertexIndex] + position;
            Vector3 frontPosition = new Vector3(adjustedPosition, -0.5f);
            Vector3 backPosition = new Vector3(adjustedPosition, +0.5f);

            Vector2 textureCoordinates = new Vector2(
              (frontPosition.X / font.LineHeight + frontPosition.Z) / 2.0f,
              frontPosition.Y / font.LineHeight
            );

            this.vertices[baseVertexIndex + vertexIndex * 2 + 0] = new TextVertex(
              frontPosition, Vector3.Forward, textureCoordinates
            );
            this.vertices[baseVertexIndex + vertexIndex * 2 + 1] = new TextVertex(
              backPosition, Vector3.Backward, textureCoordinates
            );
          }

          // Transform the outline index ranges into single big line list pointing
          // to the vertices we just imported from the font
          for(int index = 0; index < character.Outlines.Count; ++index) {
            VectorFontCharacter.Outline outline = character.Outlines[index];
            int startIndex, endIndex;

            // Set up the indices for the outline exluding the final connection from
            // the outline's end to its start
            for(int lineIndex = 0; lineIndex < outline.VertexCount - 1; ++lineIndex) {
              startIndex = baseVertexIndex + (outline.StartVertexIndex + lineIndex) * 2;
              endIndex = startIndex + 2;

              this.indices[baseIndexIndex + lineIndex * 6 + 0] = (short)(startIndex + 0);
              this.indices[baseIndexIndex + lineIndex * 6 + 1] = (short)(startIndex + 1);
              this.indices[baseIndexIndex + lineIndex * 6 + 2] = (short)(endIndex + 0);
              this.indices[baseIndexIndex + lineIndex * 6 + 3] = (short)(endIndex + 0);
              this.indices[baseIndexIndex + lineIndex * 6 + 4] = (short)(startIndex + 1);
              this.indices[baseIndexIndex + lineIndex * 6 + 5] = (short)(endIndex + 1);
            }

            int lastLineIndex = outline.VertexCount - 1;
            startIndex = baseVertexIndex + outline.StartVertexIndex * 2;
            endIndex = startIndex + lastLineIndex * 2;

            this.indices[baseIndexIndex + lastLineIndex * 6 + 0] = (short)(startIndex + 0);
            this.indices[baseIndexIndex + lastLineIndex * 6 + 1] = (short)(startIndex + 1);
            this.indices[baseIndexIndex + lastLineIndex * 6 + 2] = (short)(endIndex + 0);
            this.indices[baseIndexIndex + lastLineIndex * 6 + 3] = (short)(endIndex + 0);
            this.indices[baseIndexIndex + lastLineIndex * 6 + 4] = (short)(startIndex + 1);
            this.indices[baseIndexIndex + lastLineIndex * 6 + 5] = (short)(endIndex + 1);

            // Advance the index pointer for the next run
            baseIndexIndex += outline.VertexCount * 6;
          }

          // Add the indices for the vertices of font's faces, once for the front face and
          // once for the back face we generated.
          for(int faceIndex = 0; faceIndex < character.Faces.Count; ++faceIndex) {
            this.indices[baseIndexIndex + faceIndex * 6 + 0] =
              (short)(character.Faces[faceIndex].FirstVertexIndex * 2 + baseVertexIndex);
            this.indices[baseIndexIndex + faceIndex * 6 + 1] =
              (short)(character.Faces[faceIndex].SecondVertexIndex * 2 + baseVertexIndex);
            this.indices[baseIndexIndex + faceIndex * 6 + 2] =
              (short)(character.Faces[faceIndex].ThirdVertexIndex * 2 + baseVertexIndex);

            this.indices[baseIndexIndex + faceIndex * 6 + 3] =
              (short)(character.Faces[faceIndex].FirstVertexIndex * 2 + baseVertexIndex + 1);
            this.indices[baseIndexIndex + faceIndex * 6 + 4] =
              (short)(character.Faces[faceIndex].SecondVertexIndex * 2 + baseVertexIndex + 1);
            this.indices[baseIndexIndex + faceIndex * 6 + 5] =
              (short)(character.Faces[faceIndex].ThirdVertexIndex * 2 + baseVertexIndex + 1);
          }

          // Adjust the base vertex index for the next character
          baseIndexIndex += character.Faces.Count * 6;
          baseVertexIndex += character.Vertices.Count * 2;

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

          vertexCount += character.Vertices.Count * 2; // multiply by 2 for front and
          indexCount += character.Faces.Count * 3 * 2; // back face of the mesh

          // There may be empty characters (characters without a visual representation
          // in the font, so we need to check this before accessing the outline array
          if(character.Outlines.Count > 0) {
            VectorFontCharacter.Outline finalOutline =
              character.Outlines[character.Outlines.Count - 1];

            int outlineVertexCount = finalOutline.StartVertexIndex + finalOutline.VertexCount;
            indexCount += outlineVertexCount * 6; // 2 triangles each
          }
        }
      }

      this.vertices = new VertexPositionNormalTexture[vertexCount];
      this.indices = new short[indexCount];
    }

  }

} // namespace Nuclex.Fonts
