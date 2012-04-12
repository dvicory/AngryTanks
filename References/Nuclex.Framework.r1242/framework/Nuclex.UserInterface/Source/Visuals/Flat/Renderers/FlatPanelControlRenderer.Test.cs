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

using Nuclex.UserInterface.Controls.Arcade;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Unit Test for the flat panel control renderer</summary>
  [TestFixture]
  internal class FlatPanelControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.panel = new PanelControl();
      this.panel.Bounds = new UniRectangle(10, 10, 100, 100);
      Screen.Desktop.Children.Add(this.panel);

      this.renderer = new FlatPanelControlRenderer();
    }

    /// <summary>Verifies that the renderer is able to render the panel</summary>
    [Test]
    public void TestRenderNormal() {

      // Make sure the renderer draws at least a frame and the panel's title
      Expect.Once.On(MockedGraphics).Method("DrawElement").WithAnyArguments();

      // Let the renderer draw the panel into the mocked graphics interface
      renderer.Render(this.panel, MockedGraphics);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Panel renderer being tested</summary>
    private FlatPanelControlRenderer renderer;
    /// <summary>Panel used to test the panel renderer</summary>
    private PanelControl panel;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
