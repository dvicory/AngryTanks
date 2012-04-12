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

#include "../FreeTypeFontProcessor.h"

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Rasterizes font characters using the FreeType library</summary>
  public ref class FreeTypeFontRasterizer : public FreeTypeFontProcessor {

    /// <summary>Initializes a new freetype font rasterizer</summary>
    /// <param name="fontDescription">Description of the font to rasterize</param>
    public: FreeTypeFontRasterizer(
      Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^fontDescription
    );

    /// <summary>Rasterizes the specified character</summary>
    /// <param name="character">Character that will be rasterized</param>
    /// <returns>A bitmap containing the rasterized character</returns>
    public: Microsoft::Xna::Framework::Content::Pipeline::Graphics::PixelBitmapContent<
#if defined(XNA_4)
      Microsoft::Xna::Framework::Color
#else
      Microsoft::Xna::Framework::Graphics::Color
#endif
    > ^Rasterize(wchar_t character);

    /// <summary>
    ///   Returns the positioning of a character's bitmap relative to the font's baseline
    /// </summary>
    /// <param name="character">Character whose positioning will be returned</param>
    /// <returns>The positioning of the character relative to its baseline</returns>
    public: Microsoft::Xna::Framework::Point GetOffset(wchar_t character);

    /// <summary>
    ///   Returns the number of pixels required to advance to the position
    ///   where the next character should be rendered.
    /// </summary>
    /// <param name="character">Character whose advancement will be returned</param>
    /// <returns>The advancement from the character to the next character</returns>
    public: Microsoft::Xna::Framework::Point GetAdvancement(wchar_t character);

  };

}}} // namespace Nuclex::Fonts::Content
