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

  /// <summary>Unit Test for the flat choice control renderer</summary>
  [TestFixture]
  internal class FlatChoiceControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.choice = new ChoiceControl();
      this.choice.Text = "Choice";
      this.choice.Bounds = new UniRectangle(10, 10, 100, 50);
      Screen.Desktop.Children.Add(this.choice);

      this.renderer = new FlatChoiceControlRenderer();
    }

    /// <summary>
    ///   Verifies that the choice control renderer can render disabled choices
    /// </summary>
    [Test]
    public void TestRenderDisabled() {
      this.choice.Enabled = false;
      drawAndExpectState("off.disabled");
      
      this.choice.Selected = true;
      drawAndExpectState("on.disabled");
    }

    /// <summary>
    ///   Verifies that the choice control renderer can render normal choices
    /// </summary>
    [Test]
    public void TestRenderNormal() {
      this.choice.Enabled = true;
      drawAndExpectState("off.normal");

      this.choice.Selected = true;
      drawAndExpectState("on.normal");
    }

    /// <summary>
    ///   Verifies that the choice control renderer can render highlighted choices
    /// </summary>
    [Test]
    public void TestRenderHighlighted() {
      Screen.InjectMouseMove(15.0f, 15.0f); // Move the mouse cursor over the choice
      drawAndExpectState("off.highlighted");

      this.choice.Selected = true;
      drawAndExpectState("on.highlighted");
    }

    /// <summary>
    ///   Verifies that the choice control renderer can render depressed choices
    /// </summary>
    [Test]
    public void TestRenderDepressed() {

      // Move the mouse cursor over the choice and press it
      Screen.InjectMouseMove(15.0f, 15.0f);
      Screen.InjectMousePress(MouseButtons.Left);

      drawAndExpectState("off.depressed");

      this.choice.Selected = true;
      drawAndExpectState("on.depressed");

    }

    /// <summary>
    ///   Lets the renderer draw the choice control and verifies that the choice used
    ///   the skin elements from the expected state
    /// </summary>
    /// <param name="expectedState">
    ///   Expected state the skin elements should be using
    /// </param>
    private void drawAndExpectState(string expectedState) {

      // Make sure the renderer draws at least a frame and the choice's text
      Expect.Once.On(MockedGraphics).Method("DrawElement").With(
        Is.StringContaining(expectedState), Is.Anything
      );
      Expect.Once.On(MockedGraphics).Method("DrawString").With(
        Is.StringContaining(expectedState), Is.Anything, Is.EqualTo("Choice")
      );

      // Let the renderer draw the choice into the mocked graphics interface
      renderer.Render(this.choice, MockedGraphics);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>Choice control renderer being tested</summary>
    private FlatChoiceControlRenderer renderer;
    /// <summary>Choice control used to test the choice control renderer</summary>
    private ChoiceControl choice;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
