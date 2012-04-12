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

#include "NuclexSpriteFontDescriptionProcessor.h"
#include "CygonRectanglePacker.h"
#include "../clix.h" // string converter

using namespace std;
using namespace clix;

using namespace System;
using namespace System::Collections::Generic;

using namespace Microsoft::Xna::Framework;
using namespace Microsoft::Xna::Framework::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Processors;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  namespace { // anonymous

    /// <summary>Copies a font character's bitmap onto a larger bitmap</summary>
    /// <param name="destination">Target bitmap to copy the character to</param>
    /// <param name="character">Character bitmap to be copied</param>
    /// <param name="placement">Location on the target bitmap to copy the character to</param>
    void copyCharacterBitmap(
      PixelBitmapContent<Color> ^destination,
      PixelBitmapContent<Color> ^character, Point placement
    ) {
      Rectangle sourceRegion(0, 0, character->Width, character->Height);
      Rectangle destinationRegion(
        placement.X, placement.Y, character->Width, character->Height
      );

      BitmapContent::Copy(character, sourceRegion, destination, destinationRegion);
    }

  } // anonymous namespace

  // ------------------------------------------------------------------------------------------- //

  SpriteFontContent ^NuclexSpriteFontDescriptionProcessor::Process(
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^input,
    Microsoft::Xna::Framework::Content::Pipeline::ContentProcessorContext ^context
  ) {
    SpriteFontContent ^content = gcnew SpriteFontContent();
    FreeTypeFontRasterizer rasterizer(input);

    // Determine how large the texture for this font needs to be. If the required
    // texture would become too large, this method will throw an exception and cause
    // the importer to fail right here.
    int textureSize = determineOptimalTextureSize(input, rasterizer);

    // We can now be sure that all characters can be fitted onto a texture of the
    // given size when they are fed to the rectangle packer in the same order.
    CygonRectanglePacker packer(textureSize, textureSize);
    PixelBitmapContent<Color> ^texture = gcnew PixelBitmapContent<Color>(
      textureSize - 1, textureSize - 1 // -1 to leave upper left border empty
    );

    // Import all characters and fill the SpriteFontContent instance with the data.
    for each(wchar_t character in input->Characters) {

      // Regardless of whether this character has an associated bitmap, we add it
      // to our mapping table so its glyph index and cropping informations can be looked up.
      content->CharacterMap->Add(character);

      // Render the character to a bitmap. If the character has no visible pixels,
      // this method will return a null pointer instead.
      PixelBitmapContent<Color> ^characterBitmap = rasterizer.Rasterize(character);

      // If we got a valid bitmap (meaning the character has visible pixels, see
      // comment above), put it on our font texture.
      if(characterBitmap != nullptr) {

        Rectangle characterBoundingBox = rasterizer.MeasureCharacter(character);
        Point placement = packer.Pack(
          characterBoundingBox.Width + 1, characterBoundingBox.Height + 1 
        );
        placement.X += 1; // for empty upper left in texture
        placement.Y += 1; // for empty upper left in texture
        characterBoundingBox.X = placement.X;
        characterBoundingBox.Y = placement.Y;

        content->Glyphs->Add(characterBoundingBox);
        copyCharacterBitmap(texture, characterBitmap, placement);

      } else {
        content->Glyphs->Add(Rectangle::Empty);
      }

      // Finally, add the cropping informations of the character
      Point offset = rasterizer.GetOffset(character);
      Point advancement = rasterizer.GetAdvancement(character);
      content->Cropping->Add(Rectangle(offset.X, offset.Y, advancement.X, advancement.Y));
      content->Kerning->Add(Vector3(0.0f, static_cast<float>(advancement.X), 0.0f));
        
    }

    content->Texture->Mipmaps = texture;
    content->Spacing = input->Spacing;
    content->LineSpacing = rasterizer.LineHeight;
    content->DefaultCharacter = input->DefaultCharacter;

    return content;
  }

  // ------------------------------------------------------------------------------------------- //
  
  int NuclexSpriteFontDescriptionProcessor::determineOptimalTextureSize(
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^input,
    Nuclex::Fonts::Content::FreeTypeFontRasterizer %rasterizer
  ) {

    // Build a list of the dimensions of all characters we are to import
    List<Rectangle> ^characterRectangles = gcnew List<Rectangle>();
    for each(wchar_t character in input->Characters) {
      Rectangle characterBoundingBox = rasterizer.MeasureCharacter(character);
      if((characterBoundingBox.Width > 0) && (characterBoundingBox.Height > 0))
        characterRectangles->Add(characterBoundingBox);
    }

    // Now determine the smallest texture that can hold all the characters
    // so we don't waste precious GPU memory with unused texture space.
    for(int power = 6; power < 12; ++power) {

      // Create a packer for the currently tested texture size
      int textureSize = (1 << power); // power of 2

      {

        CygonRectanglePacker packer(textureSize, textureSize);

        // Try to cramp all the characters into the current texture
        bool failed = false;
        for each(Rectangle characterRectangle in characterRectangles) {      
          Point placement;

          bool result = packer.TryPack(
            characterRectangle.Width + 1, characterRectangle.Height + 1, placement
          );
          if(!result) {
            failed = true;
            break;
          }
        }

        // If we were able to arrange all characters on the texture, we have found
        // the optimal texture size!
        if(!failed)
          return textureSize;

      }

    }

    // There was no texture size at which all characters could fit, throw an exception
    // and cause the importer to fail, telling the user that his font description is bad.
    throw gcnew OutOfSpaceException("Font size too big or too many characters to import");

  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
