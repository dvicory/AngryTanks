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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Input;

using Is = NMock2.Is;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Unit Test for the flat option control renderer</summary>
  [TestFixture]
  internal class FlatOptionControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.option = new OptionControl();
      this.option.Text = "Option";
      this.option.Bounds = new UniRectangle(10, 10, 100, 50);
      Screen.Desktop.Children.Add(this.option);

      this.renderer = new FlatOptionControlRenderer();
    }

    /// <summary>
    ///   Verifies that the option control renderer can render disabled options
    /// </summary>
    [Test]
    public void TestRenderDisabled() {
      this.option.Enabled = false;
      drawAndExpectState("off.disabled");

      this.option.Selected = true;
      drawAndExpectState("on.disabled");
    }

    /// <summary>
    ///   Verifies that the option control renderer can render normal options
    /// </summary>
    [Test]
    public void TestRenderNormal() {
      this.option.Enabled = true;
      drawAndExpectState("off.normal");

      this.option.Selected = true;
      drawAndExpectState("on.normal");
    }

    /// <summary>
    ///   Verifies that the option control renderer can render highlighted options
    /// </summary>
    [Test]
    public void TestRenderHighlighted() {
      Screen.InjectMouseMove(15.0f, 15.0f); // Move the mouse cursor over the option
      drawAndExpectState("off.highlighted");

      this.option.Selected = true;
      drawAndExpectState("on.highlighted");
    }

    /// <summary>
    ///   Verifies that the option control renderer can render depressed options
    /// </summary>
    [Test]
    public void TestRenderDepressed() {

      // Move the mouse cursor over the option and press it
      Screen.InjectMouseMove(15.0f, 15.0f);
      Screen.InjectMousePress(MouseButtons.Left);

      drawAndExpectState("off.depressed");

      this.option.Selected = true;
      drawAndExpectState("on.depressed");

    }

    /// <summary>
    ///   Lets the renderer draw the option control and verifies that the option used
    ///   the skin elements from the expected state
    /// </summary>
    /// <param name="expectedState">
    ///   Expected state the skin elements should be using
    /// </param>
    private void drawAndExpectState(string expectedState) {

      // Make sure the renderer draws at least a frame and the option's text
      Expect.Once.On(MockedGraphics).Method("DrawElement").With(
        Is.StringContaining(expectedState), Is.Anything
      );
      Expect.Once.On(MockedGraphics).Method("DrawString").With(
        Is.StringContaining(expectedState), Is.Anything, Is.EqualTo("Option")
      );

      // Let the renderer draw the option into the mocked graphics interface
      renderer.Render(this.option, MockedGraphics);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Option control renderer being tested</summary>
    private FlatOptionControlRenderer renderer;
    /// <summary>Option control used to test the option control renderer</summary>
    private OptionControl option;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
