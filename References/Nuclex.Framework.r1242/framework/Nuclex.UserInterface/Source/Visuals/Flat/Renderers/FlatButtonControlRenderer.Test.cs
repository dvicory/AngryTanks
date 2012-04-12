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

using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Input;

using Is = NMock2.Is;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers {

  /// <summary>Unit Test for the flat button control renderer</summary>
  [TestFixture]
  internal class FlatButtonControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.button = new ButtonControl();
      this.button.Text = "Test";
      this.button.Bounds = new UniRectangle(10, 10, 100, 100);
      Screen.Desktop.Children.Add(this.button);

      this.renderer = new FlatButtonControlRenderer();
    }

    /// <summary>
    ///   Verifies that the button renderer can render disabled buttons
    /// </summary>
    [Test]
    public void TestRenderDisabled() {
      this.button.Enabled = false;
      drawAndExpectState("disabled");
    }

    /// <summary>
    ///   Verifies that the button renderer can render normal buttons
    /// </summary>
    [Test]
    public void TestRenderNormal() {
      this.button.Enabled = true;
      drawAndExpectState("normal");
    }

    /// <summary>
    ///   Verifies that the button renderer can render highlighted buttons
    /// </summary>
    [Test]
    public void TestRenderHighlighted() {
      Screen.InjectMouseMove(15.0f, 15.0f); // Move the mouse cursor over the button
      drawAndExpectState("highlighted");
    }

    /// <summary>
    ///   Verifies that the button renderer can render depressed buttons
    /// </summary>
    [Test]
    public void TestRenderDepressed() {

      // Move the mouse cursor over the button and press it
      Screen.InjectMouseMove(15.0f, 15.0f);
      Screen.InjectMousePress(MouseButtons.Left);

      drawAndExpectState("depressed");

    }

    /// <summary>
    ///   Lets the renderer draw the button and verifies that the button used
    ///   the skin elements from the expected state
    /// </summary>
    /// <param name="expectedState">
    ///   Expected state the skin elements should be using
    /// </param>
    private void drawAndExpectState(string expectedState) {

      // Make sure the renderer draws at least a frame and the button's text
      Expect.Once.On(MockedGraphics).Method("DrawElement").With(
        Is.StringContaining(expectedState), Is.Anything
      );
      Expect.Once.On(MockedGraphics).Method("DrawString").With(
        Is.StringContaining(expectedState), Is.Anything, Is.EqualTo("Test")
      );

      // Let the renderer draw the button into the mocked graphics interface
      renderer.Render(this.button, MockedGraphics);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Button renderer being tested</summary>
    private FlatButtonControlRenderer renderer;
    /// <summary>Button used to test the button renderer</summary>
    private ButtonControl button;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
