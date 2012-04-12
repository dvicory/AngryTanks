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

#include "VectorFontCharacterContent.h"
#include "../FreeTypeManager.h" // freetype API

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Stores vectorial font data for freely scalable text</summary>
  public ref class VectorFontContent sealed {

    #pragma region struct KerningPair

    /// <summary>Pair of characters for kerning informations</summary>
    public: value struct KerningPair {

      /// <summary>Initializes a new kerning pair</summary>
      /// <param name="left">Left character of the kerning pair</param>
      /// <param name="right">Right character of the kerning pair</param>
      public: KerningPair(wchar_t left, wchar_t right) :
        Left(left), Right(right) {}

      /// <summary>The left character in the kerning pair</summary>
      public: wchar_t Left;
      /// <summary>The right character in the kerning pair</summary>
      public: wchar_t Right;

      /// <summary>Returns a hash code for the kerning pair</summary>
      /// <returns>A hash code for the kerning pair</returns>
      public: virtual int GetHashCode() override {
        return ((int)this->Left) * 65536 + ((int)this->Right);
      }

      /// <summary>Compares this object to another object</summary>
      /// <param name="other">Object to compare to</param>
      /// <returns>True if both objects are identical</returns>
      public: virtual bool Equals(System::Object ^other) override {
        Nuclex::Fonts::Content::VectorFontContent::KerningPair ^kerningPair =
          dynamic_cast<Nuclex::Fonts::Content::VectorFontContent::KerningPair ^>(
            other
          );
 
        if(kerningPair == nullptr)
          return false;
 
        return
          (kerningPair->Left == this->Left) &&
          (kerningPair->Right == this->Right);
      }

    };

    #pragma endregion // struct KerningPair

    /// <summary>Initializes a new VectorFontContent instance</summary>
    internal: VectorFontContent() {

      this->lineHeight = 0.0f;

      this->characters =
        gcnew System::Collections::Generic::List<VectorFontCharacterContent ^>();

      this->characterMap =
        gcnew System::Collections::Generic::Dictionary<wchar_t, int>();

      this->kerningTable = gcnew System::Collections::Generic::Dictionary<
        Nuclex::Fonts::Content::VectorFontContent::KerningPair,
        Microsoft::Xna::Framework::Vector2
      >();

    }

    /// <summary>Height of a line of text in this font</summary>
    internal: property float LineHeight {
      float get() { return this->lineHeight; }
      void set(float value) { this->lineHeight = value; }
    }

    /// <summary>Maps unicode characters to their sprite indices</summary>
    /// <remarks>
    ///   Sprite fonts only contain a user-configurable subset of the unicode character
    ///   set. Thus, the first sprite in the font might not correspond to the first
    ///   character in the unicode table and worse, their might be gaps between the
    ///   ranges of characters the user configured to be imported. This dictionary
    ///   stores the sprite index for all unicode characters that have been imported.
    /// </remarks>
    internal: property System::Collections::Generic::Dictionary<wchar_t, int> ^CharacterMap {
      System::Collections::Generic::Dictionary<wchar_t, int> ^get() {
        return this->characterMap;
      }
    }

    /// <summary>Glyphs contained in this font</summary>
    internal: property System::Collections::Generic::List<
      VectorFontCharacterContent ^
    > ^Characters {
      System::Collections::Generic::List<VectorFontCharacterContent ^> ^get() {
        return this->characters;
      }
    }

    /// <summary>
    ///   Kerning table for adjusting the positions of specific character combinations
    /// </summary>
    /// <remarks>
    ///   Certain character combination, such as the two consecutive characters 'AV'
    ///   have diagonal shapes that would cause the characters to visually appear
    ///   is if they were further apart from each other. Kerning adjusts the distances
    ///   between such characters to keep the perceived character distance at the
    ///   same level for all character combinations.
    /// </remarks>
    internal: property System::Collections::Generic::Dictionary<
      Nuclex::Fonts::Content::VectorFontContent::KerningPair,
      Microsoft::Xna::Framework::Vector2
    > ^KerningTable {
      System::Collections::Generic::Dictionary<
        Nuclex::Fonts::Content::VectorFontContent::KerningPair,
        Microsoft::Xna::Framework::Vector2
      > ^get() {
        return this->kerningTable;
      }
    }

    /// <summary>Height of a single line of text in this font</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "LineHeight", AllowNull = false
      )
    ]
    float lineHeight;


    /// <summary>Maps unicode characters to their sprite indices</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "CharacterMap", AllowNull = false
      )
    ]
    System::Collections::Generic::Dictionary<wchar_t, int> ^characterMap;

    /// <summary>Characters contained in this font</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Characters", AllowNull = false
      )
    ]
    System::Collections::Generic::List<VectorFontCharacterContent ^> ^characters;

    /// <summary>
    ///   Kerning table for adjusting the positions of specific character combinations
    /// </summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "KerningTable", AllowNull = false
      )
    ]
    System::Collections::Generic::Dictionary<
      Nuclex::Fonts::Content::VectorFontContent::KerningPair,
      Microsoft::Xna::Framework::Vector2
    > ^kerningTable;

  };

}}} // namespace Nuclex::Fonts::Content
