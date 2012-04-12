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

#include "VectorFontWriter.h"
#include "../clix.h" // string converter

using namespace std;
using namespace clix;

using namespace System::Collections::Generic;

using namespace Microsoft::Xna::Framework;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  void VectorFontWriter::Write(
    Microsoft::Xna::Framework::Content::Pipeline::Serialization::Compiler::ContentWriter ^output,
    Nuclex::Fonts::Content::VectorFontContent ^fontContent
  ) {

    // Single: height of a single line of text
    output->Write(fontContent->LineHeight);

    // Object: UTF16 to font index map
    output->WriteObject(fontContent->CharacterMap);

    // Object: Character sizes, pen advancements and bitmap positioning
    output->WriteObject(fontContent->Characters);
    
    // Int32: Number of kerning table entries
    output->Write(fontContent->KerningTable->Count);

    // Save the kerning pairs in the kerning table manually
    // (easier and more straightforward than making a Dictionary saveable)
    for each(
      KeyValuePair<VectorFontContent::KerningPair, Vector2> kerningEntry in
        fontContent->KerningTable
    ) {
      output->Write(kerningEntry.Key.Left);
      output->Write(kerningEntry.Key.Right);
      output->Write(kerningEntry.Value);
    }
  }

  // ------------------------------------------------------------------------------------------- //

  System::String ^VectorFontWriter::GetRuntimeReader(
#if defined(XNA_4)
      Microsoft::Xna::Framework::Content::Pipeline::TargetPlatform targetPlatform
#else
      Microsoft::Xna::Framework::TargetPlatform targetPlatform
#endif
  ) {

    // This will resolve to either the VectorFontReader of the PC assembly
    // or to the BitmapFontReader of the XBox 360 assembly
    return 
      "Nuclex.Fonts.Content.VectorFontReader, "
      "Nuclex.Fonts, "
      "Version=2.0.0.0, "
      "Culture=neutral, "
      "PublicKeyToken=null";

  }

  // ------------------------------------------------------------------------------------------- //

  System::String ^VectorFontWriter::GetRuntimeType(
#if defined(XNA_4)
      Microsoft::Xna::Framework::Content::Pipeline::TargetPlatform targetPlatform
#else
      Microsoft::Xna::Framework::TargetPlatform targetPlatform
#endif
  ) {

    return
      "Nuclex.Fonts.VectorFont, "
      "Nuclex.Fonts, "
      "Version=2.0.0.0, "
      "Culture=neutral, "
      "PublicKeyToken=null";

  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
