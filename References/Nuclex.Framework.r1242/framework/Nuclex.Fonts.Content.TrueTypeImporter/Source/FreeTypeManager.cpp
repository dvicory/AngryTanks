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

#include "FreeTypeManager.h"
#include <shlobj.h>
#include "clix.h"

using namespace std;
using namespace clix;

using namespace System;

using namespace Microsoft::Xna::Framework;
using namespace Microsoft::Xna::Framework::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  namespace { // anonymous

    /// <summary>Returns the textual name of a FontDescriptionStyle</summary>
    /// <param name="style">Style whose string name to return</param>
    /// <return>The textual name for the specified FontDescriptionStyle</return>
    string styleNameFromFontDescriptionStyle(FontDescriptionStyle style) {
      if(style == FontDescriptionStyle::Bold)
        return "Bold";
      else if(style == FontDescriptionStyle::Italic)
        return "Italic";
      else if(style == (FontDescriptionStyle::Bold | FontDescriptionStyle::Italic))
        return "Bold, Italic";
      else
        return "Regular";
    }

  } // anonymous namespace

  // ------------------------------------------------------------------------------------------- //

  void FreeTypeManager::initializeFreeType() {
    FT_Library newFreeTypeLibrary;

    // Try to initialize the freetype library
    FT_Error error = ::FT_Init_FreeType(&newFreeTypeLibrary);
    if(error)
      throw gcnew System::Exception(L"Could not initialize FreeType library");

    freeTypeLibrary = newFreeTypeLibrary;
  }

  // ------------------------------------------------------------------------------------------- //

  void FreeTypeManager::shutdownFreeType() {

    // Release the current library instance
    ::FT_Done_FreeType(freeTypeLibrary);
    freeTypeLibrary = 0;

  }

  // ------------------------------------------------------------------------------------------- //

  std::wstring FreeTypeManager::getFontsDirectory() {

    // We need the shell's memory allocator to free the item id list afterwards
    IMalloc *shellMallocPointer;
    if(FAILED(SHGetMalloc(&shellMallocPointer)))
      throw gcnew Exception(L"Failed to obtain shell malloc interface");

    LPITEMIDLIST itemIDListPointer;

    // Query for the windows fonts folder
    if(FAILED(::SHGetSpecialFolderLocation(NULL, CSIDL_FONTS, &itemIDListPointer)))
      throw gcnew Exception(L"Could not locate the windows fonts folder");

    wchar_t path[MAX_PATH];
    try {
      // Try to obtain the file system path of the folder
      if(FAILED(::SHGetPathFromIDList(itemIDListPointer, path)))
        throw gcnew Exception(L"Error retrieving the windows fonts folder path");
    }
    finally {
      shellMallocPointer->Free(itemIDListPointer);
    }

    return std::wstring(path) + L"\\";

  }

  // ------------------------------------------------------------------------------------------- //

  FT_Face FreeTypeManager::OpenFont(
    System::String ^pathOrFaceName,
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescriptionStyle style
  ) {

    // Allow a path to a truetype font to be specified instead of the windows
    // font name so we can easily ship custom fonts with our applications
    if(System::IO::File::Exists(pathOrFaceName)) {

      FT_Face face;

      // Get the ANSI filename of the font
      string filename = marshalString<E_ANSI>(pathOrFaceName);

      // Try to open the font with FreeType
      FT_Open_Args openArgs;
      openArgs.flags = FT_OPEN_PATHNAME;
      openArgs.pathname = const_cast<char *>(filename.c_str());

      FT_Error error = ::FT_Open_Face(freeTypeLibrary, &openArgs, 0, &face);
      if(error)
        throw gcnew Exception("Error openening font file");

      return face;

    } else {

      return OpenWindowsFont(pathOrFaceName, style);

    }

  }

  // ------------------------------------------------------------------------------------------- //

  FT_Face FreeTypeManager::OpenWindowsFont(
    System::String ^faceName,
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescriptionStyle style
  ) {
    wstring fontsFolderPath = getFontsDirectory();
    string ansiFaceName = marshalString<E_ANSI>(faceName);

    string ansiStyle = styleNameFromFontDescriptionStyle(style);

    // Use the Win32 API to enumerate all files in the windows fonts directory
    WIN32_FIND_DATA findFileData;
    HANDLE searchHandle = ::FindFirstFile(
      (fontsFolderPath + L"*.ttf").c_str(), &findFileData
    );

    // If there is no fonts directory (or it's empty -- not expected for any windows
    // installation), punch out an error message
    if(!searchHandle || (searchHandle == INVALID_HANDLE_VALUE))
      throw gcnew Exception(L"Windows fonts directory could not be found");

    try {
      int findResult;

      // Check all files on whether they match our criteria
      do {
        // Get the ANSI filename of the font (seems like FreeType cannot cope with
        // unicode paths...)
        string filename = marshalString<E_ANSI>(
          marshalString<E_UTF16>(
            fontsFolderPath + findFileData.cFileName
          )
        );

        // Try to open the font with FreeType
        FT_Open_Args openArgs;
        openArgs.flags = FT_OPEN_PATHNAME;
        openArgs.pathname = const_cast<char *>(filename.c_str());

        FT_Face face;
        FT_Error error = ::FT_Open_Face(freeTypeLibrary, &openArgs, -1, &face);

        // If FreeType was able to open the font, check whether it is the font
        // we're looking for and if it is, return its path
        if(!error) {
          int faceCount = face->num_faces;
          ::FT_Done_Face(face);

          for(int faceIndex = 0; faceIndex < faceCount; ++faceIndex) {
            FT_Error error = ::FT_Open_Face(freeTypeLibrary, &openArgs, faceIndex, &face);
            if(!error) {
              if((ansiFaceName == face->family_name) && (ansiStyle == face->style_name))
                return face;

              ::FT_Done_Face(face);
            }
          }
          
        }

        // Advance to the next file that needs to be checked
        findResult = ::FindNextFile(searchHandle, &findFileData);
      } while(findResult && (findResult != ERROR_NO_MORE_FILES));
    }
    finally {
      ::FindClose(searchHandle);
    }

    // If we reach this point, the specified font could not be found
    throw gcnew Exception(L"Font could not be found");
  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
