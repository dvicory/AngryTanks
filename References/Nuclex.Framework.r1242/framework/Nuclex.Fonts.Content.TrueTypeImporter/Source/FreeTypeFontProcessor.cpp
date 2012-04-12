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

#include "FreeTypeFontProcessor.h"

using namespace std;

using namespace System;

using namespace Microsoft::Xna::Framework;
using namespace Microsoft::Xna::Framework::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  FreeTypeFontProcessor::FreeTypeFontProcessor(
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^fontDescription
  ) {

    // Ask the FreeTypeManager to load the requested font. This will first check whether
    // a file with the given name exists and otherwise search the installed windows
    // fonts for a font with the name provided (to keep compatibile with the behavior
    // of the standard XNA SpriteFont importer)
    this->freeTypeFace = this->freeTypeManager.OpenFont(
      fontDescription->FontName, fontDescription->Style
    );
      
    // FreeType uses a 26.6 fixed point format instead of floats!
    int fixedPointSize = static_cast<int>(fontDescription->Size * 64.0f);
    FT_Error error = FT_Set_Char_Size(
      this->freeTypeFace, 0, fixedPointSize, 72, 72
    );
    if(error)
      throw gcnew Exception("Could not set font size in FreeType");

  }

  // ------------------------------------------------------------------------------------------- //

  FreeTypeFontProcessor::~FreeTypeFontProcessor() {
    this->!FreeTypeFontProcessor();
    GC::SuppressFinalize(this);
  }

  // ------------------------------------------------------------------------------------------- //

  FreeTypeFontProcessor::!FreeTypeFontProcessor() {

    // Release the FreeType font face we were using, if any
    if(this->freeTypeFace != 0) {
      ::FT_Done_Face(this->freeTypeFace);
      this->freeTypeFace = 0;
    }

  }

  // ------------------------------------------------------------------------------------------- //

  Microsoft::Xna::Framework::Vector2 FreeTypeFontProcessor::GetKerning(
    wchar_t leftCharacter, wchar_t rightCharacter
  ) {
    FT_Vector kerning;
  
    // Retrieve the kerning distance from FreeType
    FT_Error error = FT_Get_Kerning(
      this->freeTypeFace, leftCharacter, rightCharacter, FT_KERNING_DEFAULT, &kerning
    );
    if(error) {
      throw gcnew Exception("Could not obtain kerning information");
    }
    
    // The kerning informations are returned in the usual 26.6 fixed point format
    return Microsoft::Xna::Framework::Vector2(
      static_cast<float>(kerning.x) / 64.0f, static_cast<float>(kerning.y) / 64.0f
    );
  }

  // ------------------------------------------------------------------------------------------- //

  Microsoft::Xna::Framework::Rectangle FreeTypeFontProcessor::MeasureCharacter(
    wchar_t character
  ) {

    // Prepare the character in FreeType
    LoadCharacter(character);

    // Store the glyph's dimensions in a rectangle and return it
    FT_Bitmap &bitmap = this->freeTypeFace->glyph->bitmap;
    return Microsoft::Xna::Framework::Rectangle(0, 0, bitmap.width, bitmap.rows);

  }

  // ------------------------------------------------------------------------------------------- //

  void FreeTypeFontProcessor::LoadCharacter(wchar_t character) {

    // If we have the specified glyph already loaded, do nothing. Only the lastmost
    // character is kept in memory, but typically, different calls will be accessing
    // the same character several times in succession, so this provides a decent
    // performance boost.
    if(this->loadedGlyph.HasValue)
      if(this->loadedGlyph.Value == character)
        return;

    // Try to load the selected character
    FT_Error error = ::FT_Load_Char(this->freeTypeFace, character, FT_LOAD_RENDER);
    if(error)
      throw gcnew Exception("Error loading glyph in FreeType");

    this->loadedGlyph = character;

  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
