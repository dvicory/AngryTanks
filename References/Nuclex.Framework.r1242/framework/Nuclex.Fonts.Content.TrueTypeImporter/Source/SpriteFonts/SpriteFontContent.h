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

#include "../FreeTypeManager.h" // freetype API

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Sprite font content class compatible to its XNA pendant</summary>
  public ref class SpriteFontContent sealed {

    /// <summary>Initializes a new SpriteFontContent instance</summary>
    internal: SpriteFontContent() {

      this->texture =
        gcnew Microsoft::Xna::Framework::Content::Pipeline::Graphics::Texture2DContent();

      this->glyphs =
        gcnew System::Collections::Generic::List<Microsoft::Xna::Framework::Rectangle>();

      this->cropping =
        gcnew System::Collections::Generic::List<Microsoft::Xna::Framework::Rectangle>();

      this->characterMap =
        gcnew System::Collections::Generic::List<wchar_t>();

      this->kerning =
        gcnew System::Collections::Generic::List<Microsoft::Xna::Framework::Vector3>();

    }

    /// <summary>Texture content element holding the glyph sprites</summary>
    internal: property Microsoft::Xna::Framework::Content::Pipeline::
      Graphics::Texture2DContent ^Texture {

      Microsoft::Xna::Framework::Content::Pipeline::Graphics::Texture2DContent ^get() {
        return this->texture;
      }

    }

    /// <summary>Position of the glyph sprites on the font texture</summary>
    /// <remarks>
    ///   This list contains the integer texture coordinates of the glyph sprites on the
    ///   font texture. These can not be computed on-the-fly because, in order to save
    ///   memory and increase efficiency, glyph sprites are arranged on the font texture
    ///   in arbitrary locations to use the least space possible.
    /// </remarks>
    internal: property System::Collections::Generic::List<
      Microsoft::Xna::Framework::Rectangle
    > ^Glyphs {
      System::Collections::Generic::List<Microsoft::Xna::Framework::Rectangle> ^get() {
        return this->glyphs;
      }
    }

    /// <summary>Offset and character advancement informations</summary>
    /// <remarks>
    ///   The term 'cropping' is actually misleading here. What XNA stores in these
    ///   rectangles are two entirely different things that have nothing to do with
    ///   cropping:
    ///   (1) The upper left corner of the rectangle contains a character's offset from
    ///       the pen position. A dot, for example, might only use a 2x2 pixel texture
    ///       that is moved to the text's baseline by means of the offset.
    ///   (2) The width and height contain the advancement, the amount of pixels the pen
    ///       is moved forward when the character has been rendered. At the time of this
    ///       writing, the XNA framework ignores the height value completely.
    /// </remarks>
    internal: property System::Collections::Generic::List<
      Microsoft::Xna::Framework::Rectangle
    > ^Cropping {
      System::Collections::Generic::List<Microsoft::Xna::Framework::Rectangle> ^get() {
        return this->cropping;
      }
    }

    /// <summary>Maps unicode characters to their sprite indices</summary>
    /// <remarks>
    ///   Sprite fonts only contain a user-configurable subset of the unicode character
    ///   set. Thus, the first sprite in the font might not correspond to the first
    ///   character in the unicode table and worse, their might be gaps between the
    ///   ranges of characters the user configured to be imported. This dictionary
    ///   stores the sprite index for all unicode characters that have been imported.
    /// </remarks>
    internal: property System::Collections::Generic::List<wchar_t> ^CharacterMap {
      System::Collections::Generic::List<wchar_t> ^get() {
        return this->characterMap;
      }
    }

    /// <summary>Number of pixels from one line to the next</summary>
    internal: property int LineSpacing {
      int get() {
        return this->lineSpacing;
      }
      void set(int value) {
        this->lineSpacing = value;
      }
    }

    /// <summary>Number of pixels between two consecutive characters</summary>
    internal: property float Spacing {
      float get() {
        return this->spacing;
      }
      void set(float value) {
        this->spacing = value;
      }
    }

    /// <summary>Kerning information for tightening letters with common diagonals</summary>
    /// <remarks>
    ///   XNA misuses the term 'kerning' to refer to plain ABC spacing of characters
    ///   (a = empty space before a character, b = width of the black parts of a character
    ///   and c = empty space that follows a character). Actual kerning would require
    ///   a table that told us to move 'V's following 'A's closer because they wont overlap.
    /// </remarks>
    internal: property System::Collections::Generic::List<
      Microsoft::Xna::Framework::Vector3
    > ^Kerning {
      System::Collections::Generic::List<Microsoft::Xna::Framework::Vector3> ^get() {
        return this->kerning;
      }
    }
    
    /// <summary>Default character for unknown glyphs</summary>
    internal: property System::Nullable<wchar_t> DefaultCharacter {
      System::Nullable<wchar_t> get() {
        return this->defaultCharacter;
      }
      void set(System::Nullable<wchar_t> value) {
        this->defaultCharacter = value;
      }
    }

    /// <summary>Texture content element holding the glyph sprites</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Texture", AllowNull = false
      )
    ]
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::Texture2DContent ^texture;

    /// <summary>Position of the glyph sprites on the font texture</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Glyphs", AllowNull = false
      )
    ]
    System::Collections::Generic::List<Microsoft::Xna::Framework::Rectangle> ^glyphs;

    /// <summary>Offset and character advancement informations</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Cropping", AllowNull = false
      )
    ]
    System::Collections::Generic::List<Microsoft::Xna::Framework::Rectangle> ^cropping;

    /// <summary>Maps unicode characters to their sprite indices</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "CharacterMap", AllowNull = false
      )
    ]
    System::Collections::Generic::List<wchar_t> ^characterMap;

    /// <summary>Number of pixels from one line to the next</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "LineSpacing", AllowNull = false
      )
    ]
    int lineSpacing;

    /// <summary>Number of pixels between two consecutive characters</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Spacing", AllowNull = false
      )
    ]
    float spacing;

    /// <summary>Kerning information for tightening letters with common diagonals</summary>
    private:
    [
      Microsoft::Xna::Framework::Content::ContentSerializer(
        ElementName = "Kerning", AllowNull = false
      )
    ]
    System::Collections::Generic::List<Microsoft::Xna::Framework::Vector3> ^kerning;

    /// <summary>Default character for unknown glyphs</summary>
    private: System::Nullable<wchar_t> defaultCharacter;

  };

}}} // namespace Nuclex::Fonts::Content
