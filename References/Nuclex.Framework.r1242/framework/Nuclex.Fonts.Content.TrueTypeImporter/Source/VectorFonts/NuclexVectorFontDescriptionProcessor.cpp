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

#include "NuclexVectorFontDescriptionProcessor.h"
#include "FreeTypeFontVectorizer.h"
#include "FreeTypeFontTessellator.h"

using namespace std;

using namespace System;
using namespace System::Collections::Generic;

using namespace Microsoft::Xna::Framework;
using namespace Microsoft::Xna::Framework::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Processors;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  VectorFontContent ^NuclexVectorFontDescriptionProcessor::Process(
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^input,
    Microsoft::Xna::Framework::Content::Pipeline::ContentProcessorContext ^context
  ) {
    VectorFontContent ^content = gcnew VectorFontContent();
    FreeTypeFontVectorizer vectorizer(input);

    content->LineHeight = static_cast<float>(vectorizer.LineHeight);

    // Build vector representations of all characters the user wishes to import
    for each(wchar_t character in input->Characters) {

      // Decompose the character into a series of outlines consisting of straight
      // line segments only.
      VectorFontCharacterContent ^characterContent = vectorizer.Vectorize(character);

      // Now the tessellator can build triangle meshes from the character's shapes.
      // This will fill the Faces array of the character as well as possibly generate
      // some additional supporting vertices inside the outline.
      FreeTypeFontTessellator::Tessellate(characterContent);

      // Extract the advancement for this character and assign it to the content instance.
      characterContent->Advancement = vectorizer.GetAdvancement(character);

      // All done, put the character into the font's character list and associate
      // its list index with the unicode character being represented in the character map.
      content->CharacterMap->Add(character, content->Characters->Count);
      content->Characters->Add(characterContent);

      // Extract the kerning informations of this character versus any other character
      // including itself.
      for each(wchar_t kerningCharacter in input->Characters) {
        Microsoft::Xna::Framework::Vector2 kerning =
          vectorizer.GetKerning(character, kerningCharacter);

        // If there is kerning information for this character pair, add it to the font
        if((kerning.X != 0) || (kerning.Y != 0)) {
          content->KerningTable->Add(
            Nuclex::Fonts::Content::VectorFontContent::KerningPair(
              character, kerningCharacter
            ),
            kerning
          );
        }
      }

    }

    return content;
  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
