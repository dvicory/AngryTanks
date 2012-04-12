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

  /// <summary>Unit Test for the flat progress bar control renderer</summary>
  [TestFixture]
  internal class FlatProgressControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.progress = new ProgressControl();
      this.progress.Progress = 0.5f;
      this.progress.Bounds = new UniRectangle(10, 10, 100, 100);
      Screen.Desktop.Children.Add(this.progress);

      this.renderer = new FlatProgressControlRenderer();
    }

    /// <summary>Verifies that the renderer is able to render the progress bar</summary>
    [Test]
    public void TestRenderNormal() {

      // Make sure the renderer draws at least two frames
      Expect.AtLeast(2).On(MockedGraphics).Method("DrawElement").WithAnyArguments();

      // Let the renderer draw the progress bar into the mocked graphics interface
      renderer.Render(this.progress, MockedGraphics);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Progress bar renderer being tested</summary>
    private FlatProgressControlRenderer renderer;
    /// <summary>Progress bar used to test the progress bar renderer</summary>
    private ProgressControl progress;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
