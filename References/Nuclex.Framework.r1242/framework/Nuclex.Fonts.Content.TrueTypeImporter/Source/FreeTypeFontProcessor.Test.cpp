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

#define WIN32_LEAN_AND_MEAN
#define WIN32_EXTRALEN
#include <windows.h>

#include "clix.h"

using namespace std;

using namespace System;
using namespace NUnit::Framework;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  /// <summary>Unit test for the FreeType font processor class</summary>
  [TestFixture]
  public ref class FreeTypeFontProcessorTest {

    /// <summary>
    ///   Ensures that the font processor is able to discover windows built-in fonts
    ///   by their name.
    /// </summary>
    public: [Test] void TestWindowsFontDiscovery() {

      // Check to see whether the 'Arial' font is found (which should really be
      // installed on any windows system you can find)
      testLoadFont("Arial");

      // Test some more fonts that should be shipped with any recent windows version
      testLoadFont("Courier New");
      testLoadFont("Lucida Console");
      testLoadFont("Tahoma");
      testLoadFont("Verdana");

    }

    /// <summary>Tests whether the specified font name can be found and loaded</summary>
    /// <param name="fontname">Name of the font to test for loadability</param>
    private: void testLoadFont(System::String ^fontname) {
      wstring unicodeFontname = clix::marshalString<clix::E_UTF16>(fontname);

      // Check to see whether the specified font is installed
      // TODO: This check seems flawed. CreateFont() will happily load the font
      //       "omg stoopid nonsense" on my Windows XP SP2 system.
      HFONT hFont = ::CreateFont(
        10, 10, 0, 0,
        FW_NORMAL, FALSE, FALSE, FALSE,
        DEFAULT_CHARSET, OUT_DEFAULT_PRECIS,
        CLIP_DEFAULT_PRECIS, DEFAULT_QUALITY, FF_DONTCARE,
        unicodeFontname.c_str()
      );
      if(hFont != NULL)
        ::DeleteObject(hFont);

      // If the font is missing we bail out. Silently skipping the test is not an
      // option, so we leave it to the user to either install the font or to disable
      // the test in his testing tool if he insists on not having the font on his system.
      Assert::IsTrue(hFont != NULL, "Font " + fontname + " is required for this test");

      // This is the real test: See whether the font processor is able to locate
      // the windows font only given its public name.
      FontDescription ^fontDescription = gcnew FontDescription(fontname, 10.0f, 0.0f);
      try {
        FreeTypeFontProcessor fontProcessor(fontDescription);
      }
      finally {
        delete fontDescription;
      }
    }

  };


}}} // namespace Nuclex::Fonts::Content
