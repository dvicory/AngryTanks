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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;
using Is = NMock2.Is;

using Nuclex.UserInterface.Controls;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Unit Test for the flat label control renderer</summary>
  [TestFixture]
  internal class FlatLabelControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.label = new LabelControl();
      this.label.Text = "Test";
      this.label.Bounds = new UniRectangle(10, 10, 100, 100);
      Screen.Desktop.Children.Add(this.label);

      this.renderer = new FlatLabelControlRenderer();
    }

    /// <summary>Verifies that the renderer is able to render the label</summary>
    [Test]
    public void TestRenderNormal() {

      // Make sure the renderer draws at least the label's text
      Expect.Once.On(MockedGraphics).Method("DrawString").With(
        Is.Anything, Is.Anything, Is.EqualTo("Test")
      );

      // Let the renderer draw the label into the mocked graphics interface
      renderer.Render(this.label, MockedGraphics);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Label renderer being tested</summary>
    private FlatLabelControlRenderer renderer;
    /// <summary>Label used to test the label renderer</summary>
    private LabelControl label;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
