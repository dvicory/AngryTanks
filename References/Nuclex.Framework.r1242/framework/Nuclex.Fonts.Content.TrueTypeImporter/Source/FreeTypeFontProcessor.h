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

#include <string>

#include "FreeTypeManager.h" // freetype API

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Rasterizes font characters using the FreeType library</summary>
  public ref class FreeTypeFontProcessor {

    /// <summary>Initializes a new freetype font rasterizer</summary>
    /// <param name="fontDescription">Description of the font to rasterize</param>
    public: FreeTypeFontProcessor(
      Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^fontDescription
    );

    /// <summary>Immediately releases any resources owned by the font</summary>
    public: ~FreeTypeFontProcessor();

    /// <summary>Finalizer for when the instance is garbage collected</summary>
    public: !FreeTypeFontProcessor();

    /// <summary>Retrieves kerning information between the two characters</summary>
    /// <param name="leftCharacter">Left character of the kerning pair</param>
    /// <param name="rightCharacter">Right character of the kerning pair</param>
    /// <returns>The distance adjustment for the right character</returns>
    public: Microsoft::Xna::Framework::Vector2 GetKerning(
      wchar_t leftCharacter, wchar_t rightCharacter
    );

    /// <summary>Measures the dimensions of a character</summary>
    /// <param name="character">Character whose dimensions will be returned</param>
    /// <returns>The dimensions of the specified character</returns>
    public: Microsoft::Xna::Framework::Rectangle MeasureCharacter(wchar_t character);

    /// <summary>Height of a line of text with this font and size</summary>
    public: property int LineHeight {
      int get() {
        int fixedPointHeight = FT_MulFix(
          this->freeTypeFace->height, this->freeTypeFace->size->metrics.y_scale
        );
        return (fixedPointHeight >> 6);
      }
    }

    /// <summary>Loads and renders the specified character in FreeType</summary>
    /// <param name="character">Character to load and render</param>
    protected: void LoadCharacter(wchar_t character);

    /// <summary>
    ///   FreeType manager that controls the FreeType library instance lifetime
    /// </summary>
    private: FreeTypeManager freeTypeManager;
    /// <summary>FreeType face representing the rasterizer's font</summary>
    protected: FT_Face freeTypeFace;
    /// <summary>The glyph currently loaded by FreeType</summary>
    private: System::Nullable<wchar_t> loadedGlyph;

  };

}}} // namespace Nuclex::Fonts::Content
