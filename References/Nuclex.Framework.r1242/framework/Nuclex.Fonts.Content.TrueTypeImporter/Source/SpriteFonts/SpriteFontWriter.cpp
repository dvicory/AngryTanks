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

#include "SpriteFontWriter.h"

using namespace std;

using namespace System::Collections::Generic;

using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  void SpriteFontWriter::Write(
    Microsoft::Xna::Framework::Content::Pipeline::Serialization::Compiler::ContentWriter ^output,
    Nuclex::Fonts::Content::SpriteFontContent ^fontContent
  ) {

    // Texture: Contains the glyph bitmaps
    output->WriteObject(fontContent->Texture);

    // List: Positioning of glyphs on texture
    output->WriteObject(fontContent->Glyphs);

    // List: Pen advancements and bounding boxes
    output->WriteObject(fontContent->Cropping);

    // List: Map of UTF16 character to font indices
    output->WriteObject(fontContent->CharacterMap);

    // Int32: Number of pixels between two lines
    output->Write(fontContent->LineSpacing);

    // Single: Number of pixels between two characters
    output->Write(fontContent->Spacing);

    // List: Kerning data for tightening letters using ABC widths
    output->WriteObject(fontContent->Kerning);

    // Boolean: Default character to use for unknown characters
    output->Write(fontContent->DefaultCharacter.HasValue);
    if(fontContent->DefaultCharacter.HasValue) {
      output->Write(fontContent->DefaultCharacter.Value);
    }

  }

  // ------------------------------------------------------------------------------------------- //

  System::String ^SpriteFontWriter::GetRuntimeReader(
#if defined(XNA_4)
      Microsoft::Xna::Framework::Content::Pipeline::TargetPlatform targetPlatform
#else
      Microsoft::Xna::Framework::TargetPlatform targetPlatform
#endif
  ) {

    // This will resolve to either the SpriteFontReader of the PC assembly
    // or to the BitmapFontReader of the XBox 360 assembly
#if XNA_4
    return "Microsoft.Xna.Framework.Content.SpriteFontReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553";
#else
    return "Microsoft.Xna.Framework.Content.SpriteFontReader";
#endif
    /*
    return ContentTypeWriter::GetStrongTypeName(
      Microsoft::Xna::Framework::Graphics::SpriteFont::typeid, targetPlatform
    );
    */
    //return Microsoft::Xna::Framework::Content::SpriteFontReader::typeid->AssemblyQualifiedName;

  }

  // ------------------------------------------------------------------------------------------- //

  System::String ^SpriteFontWriter::GetRuntimeType(
#if defined(XNA_4)
      Microsoft::Xna::Framework::Content::Pipeline::TargetPlatform targetPlatform
#else
      Microsoft::Xna::Framework::TargetPlatform targetPlatform
#endif
  ) {

    return Microsoft::Xna::Framework::Graphics::SpriteFont::typeid->AssemblyQualifiedName;

  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
