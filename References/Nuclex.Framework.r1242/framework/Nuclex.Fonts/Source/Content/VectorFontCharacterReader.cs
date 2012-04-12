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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Nuclex.Fonts;

namespace Nuclex.Fonts.Content {

  /// <summary>XNA framework content reader for VectorFont characters</summary>
  public class VectorFontCharacterReader : ContentTypeReader<VectorFontCharacter> {

    /// <summary>Load a vector font character from a stored XNA asset</summary>
    /// <param name="input">Reader from which the asset can be read</param>
    /// <param name="existingInstance">Optional existing instance we are reloading</param>
    /// <returns>The loaded VectorFont character</returns>
    protected override VectorFontCharacter Read(
      ContentReader input, VectorFontCharacter existingInstance
    ) {

      // Read the vertices for this font and the value for the cursor advancement
      List<Vector2> vertices = input.ReadObject<List<Vector2>>();
      Vector2 advancement = input.ReadObject<Vector2>();

      // Load the font's outline index records
      int outlineCount = input.ReadInt32();
      List<VectorFontCharacter.Outline> outlines = new List<VectorFontCharacter.Outline>(
        outlineCount
      );
      for(int index = 0; index < outlineCount; ++index) {
        int startVertexIndex = input.ReadInt32();
        int vertexCount = input.ReadInt32();
        outlines.Add(new VectorFontCharacter.Outline(startVertexIndex, vertexCount));
      }

      // Load the font's face index records
      int faceCount = input.ReadInt32();
      List<VectorFontCharacter.Face> faces = new List<VectorFontCharacter.Face>(faceCount);
      for(int index = 0; index < faceCount; ++index) {
        int firstVertexIndex = input.ReadInt32();
        int secondVertexIndex = input.ReadInt32();
        int thirdVertexIndex = input.ReadInt32();
        faces.Add(
          new VectorFontCharacter.Face(firstVertexIndex, secondVertexIndex, thirdVertexIndex)
        );
      }

      // All done, we can construct a new character from the loaded data
      return new VectorFontCharacter(advancement, vertices, outlines, faces);

    }

  }

} // namespace Nuclex.Fonts.Content
