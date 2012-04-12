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

#include "../FreeTypeFontProcessor.h"
#include "VectorFontContent.h"

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Vectorizes font characters using the FreeType library</summary>
  public ref class FreeTypeFontVectorizer : public FreeTypeFontProcessor {

    /// <summary>Initializes a new freetype font vectorizer</summary>
    /// <param name="fontDescription">Description of the font to vectorize</param>
    public: FreeTypeFontVectorizer(
      Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^fontDescription
    );

    /// <summary>Builds a vector representation of the character</summary>
    /// <param name="character">
    ///   Character for which to generate a vector representation
    /// </param>
    /// <returns>A set of lists describing the outlines of the character</returns>
    /// <remarks>
    ///   There can be multiple outlines in a character for two reasons. For once,
    ///   the character may consist of disjoint shapes, like the equals sign ('='),
    ///   which has two shapes with no connection inbetween them. The other case
    ///   are shapes with holes in them. For example, the 'O' character was
    ///   two outlines, one describing its exterior border and one describing
    ///   its interior border. 
    /// </remarks>
    public: VectorFontCharacterContent ^Vectorize(wchar_t character);

/*
    /// <summary>Constructs a kerning table for the characters in this font</summary>
    /// <returns>The kerning table for this font's characters</returns>
    public: System::Collections::Generic::Dictionary<
      Nuclex::Fonts::Content::Pipeline::VectorFontContent::KerningPair,
      Microsoft::Xna::Framework::Vector2
    > ^BuildKerningTable();
*/

    /// <summary>
    ///   Returns the number of pixels required to advance to the position
    ///   where the next character should be rendered.
    /// </summary>
    /// <param name="character">Character whose advancement will be returned</param>
    /// <returns>The advancement from the character to the next character</returns>
    public: Microsoft::Xna::Framework::Vector2 GetAdvancement(wchar_t character);

  };

}}} // namespace Nuclex::Fonts::Content
