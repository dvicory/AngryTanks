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

// FreeType contains an identifier named 'generic' which causes trouble
// with C++/CLI since 'generic' is a keyword. I don't know of another workaround,
// so I'll use a #define to rename every occurence within the library
// to generic_. Maybe something could be done with the extern {} directive?
#define generic generic_

  #include "ft2build.h"
  #include FT_FREETYPE_H
  #include FT_OUTLINE_H

#undef generic

// Declare FT_LibraryRec_ as an empty structure to get rid of the linker warning (LNK4248)
struct FT_LibraryRec_ {};

#include <string>

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Controls the lifetime of a FreeType library instance</summary>
  public ref class FreeTypeManager {

    /// <summary>
    ///   Ensures a FreeType library instance is available for as long as the user exists
    /// </summary>
    public: FreeTypeManager() {
      ++freeTypeUsers;
      if(freeTypeUsers == 1)
        initializeFreeType();
    }

    /// <summary>
    ///   Destroys the FreeType library instance if this was the last user.
    /// </summary>
    public: ~FreeTypeManager() {
      --freeTypeUsers;
      if(freeTypeUsers == 0)
        shutdownFreeType();
    }

    /// <summary>Returns a pointer to the FreeType library instance</summary>
    /// <returns>The FreeType library instance</returns>
    public: FT_Library getLibraryInstance() {
      return freeTypeLibrary;
    }

    /// <summary>Opens a font from a path or by name from the windows fonts</summary>
    /// <param name="pathOrFaceName">
    ///   Path of the file to read (eg. "Fonts/Arial.ttf") or name of the windows font
    ///   to be opened (eg. "Arial")
    /// </param>
    /// <param name="style">Font style to use</param>
    /// <return>The FreeType face structure for the requested font</return>
    public: FT_Face OpenFont(
      System::String ^pathOrFaceName,
      Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescriptionStyle style
    );

    /// <summary>Opens a font by name from the installed windows fonts</summary>
    /// <param name="faceName">Name of the font to open (eg. "Arial")</param>
    /// <param name="style">Font style to use</param>
    /// <return>The FreeType face structure for the requested font</return>
    public: FT_Face OpenWindowsFont(
      System::String ^faceName,
      Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescriptionStyle style
    );

    /// <summary>Retrieves the directory where windows stores its truetype fonts</summary>
    /// <returns>The directory window stores its fonts in</returns>
    private: static std::wstring getFontsDirectory();

    /// <summary>Constructs and initializes a new FreeType library instance</summary>
    private: static void initializeFreeType();

    /// <summary>Constructs and initializes a new FreeType library instance</summary>
    private: static void shutdownFreeType();

    /// <summary>Number of active users for the current FreeType library instance</summary>
    private: static int freeTypeUsers;
    /// <summary>Global FreeType library instance</summary>
    private: static FT_Library freeTypeLibrary;

  };

}}} // namespace Nuclex::Fonts::Content
