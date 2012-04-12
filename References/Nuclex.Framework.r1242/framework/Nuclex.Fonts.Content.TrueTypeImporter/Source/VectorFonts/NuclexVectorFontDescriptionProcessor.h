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

#include "VectorFontContent.h"

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Processes Vector font descriptions for usage with the XNA framework</summary>
  [
    Microsoft::Xna::Framework::Content::Pipeline::ContentProcessorAttribute(
      DisplayName = "Vector Font - Nuclex Framework"
    )
  ]
  public ref class NuclexVectorFontDescriptionProcessor :
    Microsoft::Xna::Framework::Content::Pipeline::ContentProcessor<
      Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^,
      Nuclex::Fonts::Content::VectorFontContent ^
    > {

    /// <summary>Creates an XNA Vector font from the font description</summary>
    /// <param name="input">Provided font description to create the Vector font from</param>
    /// <param name="context">Additional informations for the content processor</param>
    /// <returns>The generated XNA Vector font</returns>
    public: virtual Nuclex::Fonts::Content::VectorFontContent ^Process(
      Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^input,
      Microsoft::Xna::Framework::Content::Pipeline::ContentProcessorContext ^context
    ) override;

  };

}}} // namespace Nuclex::Fonts::Content
