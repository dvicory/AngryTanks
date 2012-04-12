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
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

using Nuclex.Testing.Xna;
using Nuclex.UserInterface.Controls;

namespace Nuclex.UserInterface.Visuals.Flat {

  /// <summary>Unit tests for flat GUI visualizer</summary>
  [TestFixture]
  internal class FlatGuiVisualizerTest {

    #region class ResourceFileKeeper

    /// <summary>
    ///   Temporarily extracts the unit test resources to the local file system
    /// </summary>
    private class ResourceFileKeeper : IDisposable {

      /// <summary>Initializes a new resource file keeper</summary>
      public ResourceFileKeeper() {
        this.tempFile = Path.GetTempFileName();
        try {
          string tempPath = this.tempFile + ".dir";
          Directory.CreateDirectory(tempPath);
          this.tempPath = tempPath;

          extractResource(Resources.UnitTestResources.UnitTestSkin, "UnitTest.skin.xml");
          extractResource(Resources.UnitTestResources.BadColorSkin, "BadColor.skin.xml");
          extractResource(Resources.UnitTestResources.UnitTestFont, "UnitTestFont.xnb");
        }
        catch(Exception) {
          Dispose();
          throw;
        }
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.tempPath != null) {
          File.Delete(Path.Combine(this.tempPath, "UnitTest.skin.xml"));
          File.Delete(Path.Combine(this.tempPath, "BadColor.skin.xml"));
          File.Delete(Path.Combine(this.tempPath, "UnitTestFont.xnb"));
          Directory.Delete(this.tempPath);
          this.tempPath = null;
        }
        if(this.tempFile != null) {
          File.Delete(this.tempFile);
          this.tempFile = null;
        }
      }

      /// <summary>Directory into which the resources were extracted</summary>
      public string ResourcePath {
        get { return this.tempPath; }
      }

      /// <summary>Extracts the specified resource to the provided file</summary>
      /// <param name="resource">Resource that will be extracted</param>
      /// <param name="file">File to which the resource will be written</param>
      private void extractResource(byte[] resource, string file) {
        using(
          FileStream stream = new FileStream(
            Path.Combine(this.tempPath, file),
            FileMode.Create, FileAccess.Write, FileShare.None
          )
        ) {
          stream.Write(resource, 0, resource.Length);
        }
      }

      /// <summary>
      ///   Temporary file created by the OS and kept around until the instance is
      ///   disposed to occupy the temp file name
      /// </summary>
      private string tempFile;
      /// <summary>
      ///   Path to the temporary directory the resources are extracted into
      /// </summary>
      private string tempPath;

    }

    #endregion // classs ResourceFileKeeper

    #region class NonRenderableControl

    /// <summary>Control for which no renderer exists</summary>
    private class NonRenderableControl : Controls.Control { }

    #endregion // class NonRenderableControl

    #region class NotAControlRenderer

    /// <summary>Dummy class that is not a control renderer</summary>
    private class NotAControlRenderer : IFlatControlRenderer { }

    #endregion // class NotAControlRenderer

    #region class LabelControlRenderer1

    /// <summary>First of two ambiguous renderers for the label control</summary>
    private class LabelControlRenderer1 : IFlatControlRenderer<LabelControl> {
      public void Render(LabelControl control, IFlatGuiGraphics graphics) { }
    }

    #endregion // class LabelControlRenderer1

    #region class LabelControlRenderer2

    /// <summary>Second of two ambiguous renderers for the label control</summary>
    private class LabelControlRenderer2 : IFlatControlRenderer<LabelControl> {
      public void Render(LabelControl control, IFlatGuiGraphics graphics) { }
    }

    #endregion // class LabelControlRenderer2

    #region MultiInterfaceControlRenderer

    /// <summary>Control renderer which implements multiple interfaces</summary>
    private class MultiInterfaceControlRenderer :
      IFlatControlRenderer<LabelControl>, IDisposable {
      public void Render(LabelControl control, IFlatGuiGraphics graphics) { }
      public void Dispose() { }
    }

    #endregion // class MultiInterfaceControlRenderer

    #region class NonConstructableRenderer

    /// <summary>Control renderer which cannot be constructed publicly</summary>
    private class NonConstructableRenderer : IFlatControlRenderer<LabelControl> {
      private NonConstructableRenderer() { }
      public void Render(LabelControl control, IFlatGuiGraphics graphics) { }
    }
    
    #endregion // class NonConstrutableRenderer

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(
        DeviceType.Reference
      );
      this.mockedGraphicsDeviceService.CreateDevice();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that a flat GUI visualizer can be constructed from a file
    /// </summary>
    [Test]
    public void TestConstructFromFile() {
      using(ResourceFileKeeper keeper = new ResourceFileKeeper()) {
        using(
          FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromFile(
            this.mockedGraphicsDeviceService.ServiceProvider,
            Path.Combine(keeper.ResourcePath, "UnitTest.skin.xml")
          )
        ) {
          Assert.IsNotNull(visualizer); // nonsense to avoid compiler warning
        }
      }
    }

    /// <summary>
    ///   Verifies that a flat GUI visualizer can handle exceptions during loading
    ///   from a file
    /// </summary>
    [Test]
    public void TestThrowOnConstructFromBrokenFile() {
      using(ResourceFileKeeper keeper = new ResourceFileKeeper()) {
        Assert.Throws<ArgumentException>(
          delegate() {
            using(
              FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromFile(
                this.mockedGraphicsDeviceService.ServiceProvider,
                Path.Combine(keeper.ResourcePath, "BadColor.skin.xml")
              )
            ) { }
          }
        );
      }
    }

    /// <summary>
    ///   Verifies that a flat GUI visualizer can be constructed from a resource
    /// </summary>
    [Test]
    public void TestConstructFromResource() {
      using(
        FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromResource(
          this.mockedGraphicsDeviceService.ServiceProvider,
          Resources.UnitTestResources.ResourceManager, "UnitTestSkin"
        )
      ) {
        Assert.IsNotNull(visualizer); // nonsense to avoid compiler warning
      }
    }

    /// <summary>
    ///   Verifies that a flat GUI visualizer can handle exceptions when attempting
    ///   to load a non-existing resource
    /// </summary>
    [Test]
    public void TestThrowOnConstructFromNonExistingResource() {
      Assert.Throws<ArgumentException>(
        delegate() {
          using(
            FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromResource(
              this.mockedGraphicsDeviceService.ServiceProvider,
              Resources.UnitTestResources.ResourceManager, "DoesnExistSkin"
            )
          ) { }
        }
      );
    }

    /// <summary>
    ///   Verifies that a flat GUI visualizer can handle exceptions during loading
    ///   from a resource containing a broken skin description
    /// </summary>
    [Test]
    public void TestThrowOnConstructFromBrokenResource() {
      Assert.Throws<ArgumentException>(
        delegate() {
          using(
            FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromResource(
              this.mockedGraphicsDeviceService.ServiceProvider,
              Resources.UnitTestResources.ResourceManager, "BadColorSkin"
            )
          ) { }
        }
      );
    }

    /// <summary>
    ///   Verifies that a flat GUI visualizer can handle exceptions during loading
    ///   from a resource containing a broken skin description
    /// </summary>
    [Test]
    public void TestDraw() {
      using(
        FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromResource(
          this.mockedGraphicsDeviceService.ServiceProvider,
          Resources.UnitTestResources.ResourceManager, "UnitTestSkin"
        )
      ) {
        Screen screen = new Screen(800, 600);
        screen.Desktop.Children.Add(new Controls.Control());
        screen.Desktop.Children.Add(new Controls.LabelControl());

        visualizer.Draw(screen);
      }
    }

    /// <summary>
    ///   Tests whether the visualizer can cope with a non-renderable control
    /// </summary>
    [Test]
    public void TestDrawNonRenderableControl() {
      using(
        FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromResource(
          this.mockedGraphicsDeviceService.ServiceProvider,
          Resources.UnitTestResources.ResourceManager, "UnitTestSkin"
        )
      ) {
        Screen screen = new Screen(800, 600);
        screen.Desktop.Children.Add(new NonRenderableControl());

        visualizer.Draw(screen);
      }
    }

    /// <summary>
    ///   Tests whether the visualizer can cope with a non-renderable control
    /// </summary>
    [Test]
    public void TestEmployAmbiguousRenderers() {
      FlatGuiVisualizer.ControlRendererEmployer employer =
        new FlatGuiVisualizer.ControlRendererEmployer();

      employer.Employ(typeof(LabelControlRenderer1));
      employer.Employ(typeof(LabelControlRenderer2));
    }

    /// <summary>
    ///   Tests whether the visualizer can cope with a non-renderable control
    /// </summary>
    [Test]
    public void TestCannotEmployNonRendererType() {
      FlatGuiVisualizer.ControlRendererEmployer employer =
        new FlatGuiVisualizer.ControlRendererEmployer();

      Assert.IsFalse(employer.CanEmploy(typeof(NotAControlRenderer)));
    }

    /// <summary>
    ///   Tests whether the visualizer can cope with a non-renderable control
    /// </summary>
    [Test]
    public void TestCannotEmployNonConstructableRenderer() {
      FlatGuiVisualizer.ControlRendererEmployer employer =
        new FlatGuiVisualizer.ControlRendererEmployer();

      Assert.IsFalse(employer.CanEmploy(typeof(NonConstructableRenderer)));
    }

    /// <summary>
    ///   Verifies that the visualizer's control renderer repository can be accessed
    /// </summary>
    [Test]    
    public void TestAccessControlRepository() {
      using(
        FlatGuiVisualizer visualizer = FlatGuiVisualizer.FromResource(
          this.mockedGraphicsDeviceService.ServiceProvider,
          Resources.UnitTestResources.ResourceManager, "UnitTestSkin"
        )
      ) {
        // The visualizer should have added its own assembly to the list automatically
        Assert.AreEqual(1, visualizer.RendererRepository.LoadedAssemblies.Count);
      }
    }

    /// <summary>Mocked graphics device used for running the unit tests</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;

  }

} // namespace Nuclex.UserInterface.Visuals.Flat

#endif // UNITTEST
