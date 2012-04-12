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

  /// <summary>Unit Test for the flat list control renderer</summary>
  [TestFixture]
  internal class FlatListControlRendererTest : ControlRendererTest {

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public override void Setup() {
      base.Setup();

      this.list = new ListControl();
      this.list.Items.Add("Hello");
      this.list.Items.Add("World");
      this.list.SelectedItems.Add(0);
      this.list.SelectionMode = ListSelectionMode.Single;
      this.list.Bounds = new UniRectangle(10, 10, 100, 100);
      Screen.Desktop.Children.Add(this.list);

      this.renderer = new FlatListControlRenderer();
    }

#if false // Lists can't be disabled currently
    /// <summary>
    ///   Verifies that the list renderer can render disabled lists
    /// </summary>
    [Test]
    public void TestRenderDisabled() {
      this.list.Enabled = false;
      drawAndExpectState("disabled");
    }
#endif

    /// <summary>
    ///   Verifies that the list renderer can render normal lists
    /// </summary>
    [Test, Ignore("Fails for unknown reasons. Investigate.")]
    public void TestRenderNormal() {
      drawAndExpectState();
    }

    /// <summary>
    ///   Tests whether the renderer can provide the list with the row the user
    ///   has clicked on
    /// </summary>
    [Test, Ignore("Fails for unknown reasons. Investigate.")]
    public void TestClickItem() {
      drawAndExpectState();

      // Click on the second item. The selection should now change.
      Screen.InjectMouseMove(25.0f, 25.0f);
      Screen.InjectMousePress(MouseButtons.Left);
      
      Assert.AreEqual(1, this.list.SelectedItems.Count);
      Assert.AreEqual(1, this.list.SelectedItems[0]);
    }

    /// <summary>
    ///   Lets the renderer draw the list and verifies that the list used
    ///   the skin elements from the expected state
    /// </summary>
    private void drawAndExpectState() {

      // Make sure the renderer draws the list frame
      Expect.Once.On(MockedGraphics).Method("DrawElement").WithAnyArguments();
      Expect.AtLeastOnce.On(MockedGraphics).Method("SetClipRegion").WithAnyArguments();
      Expect.AtLeast(0).On(MockedGraphics).Method("MeasureString").WithAnyArguments().Will(
        Return.Value(new RectangleF(0.0f, 0.0f, 10.0f, 10.0f))
      );
      Expect.AtLeast(0).On(MockedGraphics).Method("DrawElement").WithAnyArguments();
      Expect.AtLeast(2).On(MockedGraphics).Method("DrawString").WithAnyArguments();

      // Let the renderer draw the list into the mocked graphics interface
      renderer.Render(this.list, MockedGraphics);

      // And verify the expected drawing commands were issues
      Mockery.VerifyAllExpectationsHaveBeenMet();

    }

    /// <summary>List renderer being tested</summary>
    private FlatListControlRenderer renderer;
    /// <summary>List used to test the list renderer</summary>
    private ListControl list;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers

#endif // UNITTEST
