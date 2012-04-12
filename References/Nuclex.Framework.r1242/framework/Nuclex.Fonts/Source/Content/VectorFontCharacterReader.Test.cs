#region CPL License
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
#endregion

#if UNITTEST

using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using NUnit.Framework;

using Nuclex.Graphics;

namespace Nuclex.Fonts.Content {

  /// <summary>Unit tests for the vector font character reader</summary>
  [TestFixture]
  public class VectorFontCharacterReaderTest {

    /// <summary>
    ///   Tests whether the constructor if the vector font reader is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      VectorFontCharacterReader reader = new VectorFontCharacterReader();
      Assert.IsNotNull(reader); // nonsense; avoids compiler warning
    }

    // Actual reading is verified by the VectorFontReader test

  }

} // namespace Nuclex.Fonts.Content

#endif // UNITTEST
