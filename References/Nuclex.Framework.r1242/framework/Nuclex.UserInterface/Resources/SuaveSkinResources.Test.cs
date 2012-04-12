#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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
using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

namespace Nuclex.UserInterface.Resources {

  /// <summary>Unit tests for the debug drawer resources</summary>
  [TestFixture]
  public class SuaveSkinResourcesTest {

    /// <summary>Verifies that an instance of the resource class can be created</summary>
    [Test]
    public void TestResourceClassCreation() {
      SuaveSkinResources resources = new SuaveSkinResources();
      Assert.IsNotNull(resources); // nonsense; avoids compiler warning
    }

    /// <summary>Tests the 'Culture' property of the resource class</summary>
    [Test]
    public void TestAssignCulture() {
      SuaveSkinResources.Culture = CultureInfo.CurrentCulture;
      Assert.AreSame(CultureInfo.CurrentCulture, SuaveSkinResources.Culture);
    }

    /// <summary>Verifies that the default font can be accessed</summary>
    [Test]
    public void TestDefaultFont() {
      Assert.IsNotNull(SuaveSkinResources.DefaultFont);
    }

    /// <summary>Verifies that the title font can be accessed</summary>
    [Test]
    public void TestTitleFont() {
      Assert.IsNotNull(SuaveSkinResources.TitleFont);
    }

    /// <summary>Verifies that the GUI element sprite sheet can be accessed</summary>
    [Test]
    public void TestSuaveSheet() {
      Assert.IsNotNull(SuaveSkinResources.SuaveSheet);
    }

    /// <summary>Verifies that the XML skin description can be accessed</summary>
    [Test]
    public void TestSuaveSkin() {
      Assert.IsNotNull(SuaveSkinResources.SuaveSkin);
    }

  }

} // namespace Nuclex.UserInterface.Resources

#endif // UNITTEST
