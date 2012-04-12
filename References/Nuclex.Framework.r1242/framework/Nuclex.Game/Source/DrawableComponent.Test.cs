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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

using Nuclex.Graphics;
using Nuclex.Testing.Xna;

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Game {

  /// <summary>Unit test for the drawable component class</summary>
  [TestFixture]
  internal class DrawableComponentTest {

    #region interface IDrawableComponentSubscriber

    /// <summary>Interface for a subscriber to the DrawableComponent's events</summary>
    public interface IDrawableComponentSubscriber {

      /// <summary>
      ///   Called when the component's drawing order has changed
      /// </summary>
      /// <param name="sender">Component whose drawing order property has changed</param>
      /// <param name="arguments">Not used</param>
      void DrawOrderChanged(object sender, EventArgs arguments);

      /// <summary>
      ///   Called when the Component's visible property has changed
      /// </summary>
      /// <param name="sender">Component whose visible property has changed</param>
      /// <param name="arguments">Not used</param>
      void VisibleChanged(object sender, EventArgs arguments);

    }

    #endregion // interface IDrawableComponentSubscriber

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();

      this.testComponent = new DrawableComponent();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>
    ///   Verifies that the constructor of the drawable component is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      DrawableComponent testComponent = new DrawableComponent();
    }

    /// <summary>Tests whether the Initialize() method is working</summary>
    [Test]
    public void TestInitialize() {
      this.testComponent.Initialize();
    }

    /// <summary>
    ///   Tests whether the drawable component can draw itself
    /// </summary>
    [Test]
    public void TestDraw() {
      this.testComponent.Initialize();
      this.testComponent.Draw(new GameTime());
    }

    /// <summary>Verifies that the DrawOrder property is working correctly</summary>
    [Test]
    public void TestDrawOrder() {
      this.testComponent.DrawOrder = 4321;
      Assert.AreEqual(4321, this.testComponent.DrawOrder);
    }

    /// <summary>
    ///   Verifies that the DrawOrder change event is triggered when the drawing order
    ///   of the component is changed
    /// </summary>
    [Test]
    public void TestDrawOrderChangedEvent() {
      IDrawableComponentSubscriber subscriber = mockSubscriber(this.testComponent);
      
      Expect.Once.On(subscriber).Method("DrawOrderChanged").WithAnyArguments();
      this.testComponent.DrawOrder = 4321;
      
      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Verifies that the Visible property is working correctly</summary>
    [Test]
    public void TestVisible() {
      this.testComponent.Visible = false;
      Assert.IsFalse(this.testComponent.Visible);
      this.testComponent.Visible = true;
      Assert.IsTrue(this.testComponent.Visible);
    }

    /// <summary>
    ///   Verifies that the visible change event is triggered when the visibility
    ///   of the component is changed
    /// </summary>
    [Test]
    public void TestVisibleChangedEvent() {
      this.testComponent.Visible = false;

      IDrawableComponentSubscriber subscriber = mockSubscriber(this.testComponent);
      Expect.Once.On(subscriber).Method("VisibleChanged").WithAnyArguments();
      this.testComponent.Visible = true;
      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the events of a tracker</summary>
    /// <param name="drawableComponent">Component to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IDrawableComponentSubscriber mockSubscriber(
      DrawableComponent drawableComponent
    ) {
      IDrawableComponentSubscriber mockedSubscriber =
        this.mockery.NewMock<IDrawableComponentSubscriber>();

      drawableComponent.DrawOrderChanged += new DeviceEventHandler(
        mockedSubscriber.DrawOrderChanged
      );
      drawableComponent.VisibleChanged += new DeviceEventHandler(
        mockedSubscriber.VisibleChanged
      );

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;
    /// <summary>Component being tested</summary>
    private DrawableComponent testComponent;

  }

} // namespace Nuclex.Game

#endif // UNITTEST
