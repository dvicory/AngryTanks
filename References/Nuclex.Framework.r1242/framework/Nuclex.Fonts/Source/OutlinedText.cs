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

  /// <summary>Stores the vertices for an outlined string</summary>
  public class OutlinedText : Text {

    /// <summary>Initializes a new outlined string</summary>
    /// <param name="font">Font from which the vertices will be taken</param>
    /// <param name="text">String of which to build a mesh</param>
    internal OutlinedText(VectorFont font, string text) {
      resizeVertexAndIndexArrays(font, text);
      buildStringOutline(font, text);
    }

    /// <summary>Builds the combined outline for the letters in this string</summary>
    /// <param name="font">Vector font to take the vertex data from</param>
    /// <param name="text">String of which a mesh is to be built</param>
    private void buildStringOutline(VectorFont font, string text) {
      Vector2 position = Vector2.Zero;

      int baseVertexIndex = 0; // base index in the vertex array
      int baseIndexIndex = 0; // base index in the index array

      for (int characterIndex = 0; characterIndex < text.Length; ++characterIndex) {
        int fontCharacterIndex;

        // Only add this character to the mesh if there is an actual font character
        // for this unicode symbol in the font (characters not imported by the user
        // will be silently skipped -- this is the only sane option imho)
        if (font.CharacterMap.TryGetValue(text[characterIndex], out fontCharacterIndex)) {
          VectorFontCharacter character = font.Characters[fontCharacterIndex];

          if (character.Outlines.Count > 0) {
            VectorFontCharacter.Outline finalOutline =
              character.Outlines[character.Outlines.Count - 1];

            // We calculate the vertex count from the outline instead of just taking
            // the size of the vertex array because we might not need all vertices to
            // render an outline (some have been added by the tessellation processor and
            // are only used as supporting vertices to fill the faces)
            int outlineVertexCount = finalOutline.StartVertexIndex + finalOutline.VertexCount;

            // Import all of the vertices of this font (we need both the outline vertices
            // as well as the mesh supporting vertices)
            for (int index = 0; index < outlineVertexCount; ++index) {
              this.vertices[baseVertexIndex + index] = new VertexPositionNormalTexture(
                new Vector3(character.Vertices[index] + position, 0.0f),
                Vector3.Forward, Vector2.Zero
              );
            }

            // Transform the outline index ranges into single big line list pointing
            // to the vertices we just imported from the font
            for (int index = 0; index < character.Outlines.Count; ++index) {
              VectorFontCharacter.Outline outline = character.Outlines[index];

              // Set up the indices for the outline exluding the final connection from
              // the outline's end to its start
              for (int lineIndex = 0; lineIndex < outline.VertexCount - 1; ++lineIndex) {
                this.indices[baseIndexIndex + lineIndex * 2 + 0] =
                  (short)(baseVertexIndex + outline.StartVertexIndex + lineIndex);
                this.indices[baseIndexIndex + lineIndex * 2 + 1] =
                  (short)(baseVertexIndex + outline.StartVertexIndex + lineIndex + 1);
              }

              // Add the connection from the end of the outline to its start
              this.indices[baseIndexIndex + outline.VertexCount * 2 - 2] =
                (short)(baseVertexIndex + outline.StartVertexIndex + outline.VertexCount - 1);
              this.indices[baseIndexIndex + outline.VertexCount * 2 - 1] =
                (short)(baseVertexIndex + outline.StartVertexIndex);

              // Advance the index pointer for the next run
              baseIndexIndex += outline.VertexCount * 2;
            }

            // Adjust the base vertex index for the next character
            baseVertexIndex += outlineVertexCount;
          }

          // Advance to the next character
          position += character.Advancement;

        } // if
      } // for

      this.width = position.X;
      this.height = font.LineHeight;
      this.primitiveType = PrimitiveType.LineList;
    }

    /// <summary>Reserves the required space in the vertex and index arrays</summary>
    /// <param name="font">Font the vertices for the letters will be taken from</param>
    /// <param name="text">String of which a mesh will be built</param>
    private void resizeVertexAndIndexArrays(VectorFont font, string text) {
      int vertexCount = 0;
      int indexCount = 0;

      // Count the vertices and indices requires for all characters in the string
      for (int index = 0; index < text.Length; ++index) {
        int fontCharacterIndex;

        // Try to find the current character in the font's character map. If it isn't
        // there, we'll ignore it, just like the vertex creation routine does.
        if (font.CharacterMap.TryGetValue(text[index], out fontCharacterIndex)) {
          VectorFontCharacter character = font.Characters[fontCharacterIndex];

          // There may be empty characters (characters without a visual representation
          // in the font, so we need to check this before accessing the outline array
          if (character.Outlines.Count > 0) {
            VectorFontCharacter.Outline finalOutline =
              character.Outlines[character.Outlines.Count - 1];

            // We calculate the vertex count from the outline instead of just taking
            // the size of the vertex array because we might not need all vertices to
            // render an outline (some have been added by the tessellation processor and
            // are only used as supporting vertices to fill the faces)
            int outlineVertexCount = finalOutline.StartVertexIndex + finalOutline.VertexCount;

            // Advance the counters
            vertexCount += outlineVertexCount;
            indexCount += outlineVertexCount * 2;
          }
        }
      }

      this.vertices = new VertexPositionNormalTexture[vertexCount];
      this.indices = new short[indexCount];
    }

  }

} // namespace Nuclex.Fonts
