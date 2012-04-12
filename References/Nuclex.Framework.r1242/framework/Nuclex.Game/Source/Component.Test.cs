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

#if XNA_4
using DeviceEventHandler = System.EventHandler<System.EventArgs>;
#else
using DeviceEventHandler = System.EventHandler;
#endif

namespace Nuclex.Game {

  /// <summary>Unit test for the component class</summary>
  [TestFixture]
  internal class ComponentTest {

    #region interface IComponentSubscriber

    /// <summary>Interface for a subscriber to the Component's events</summary>
    public interface IComponentSubscriber {

      /// <summary>
      ///   Called when the value of the Enabled property has changed
      /// </summary>
      /// <param name="sender">Component whose Enabled property has changed</param>
      /// <param name="arguments">Not used</param>
      void EnabledChanged(object sender, EventArgs arguments);

      /// <summary>
      ///   Called when the Component's update order has changed
      /// </summary>
      /// <param name="sender">Component whose update order property has changed</param>
      /// <param name="arguments">Not used</param>
      void UpdateOrderChanged(object sender, EventArgs arguments);

    }

    #endregion // interface IComponentSubscriber

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>Verifies that new component instances can be created</summary>
    [Test]
    public void TestConstructor() {
      Component testComponent = new Component();
      Assert.IsNotNull(testComponent); // nonsense, avoids compiler warning
    }

    /// <summary>
    ///   Tests whether the Initialize() method of the Component class can be called
    /// </summary>
    [Test]
    public void TestInitialize() {
      Component testComponent = new Component();
      testComponent.Initialize();
      // No exception means success
    }

    /// <summary>
    ///   Tests whether the Update() method of the Component class can be called
    /// </summary>
    [Test]
    public void TestUpdate() {
      Component testComponent = new Component();
      testComponent.Update(new GameTime());
      // No exception means success
    }

    /// <summary>
    ///   Tests whether the update order of the Component class can be changed
    /// </summary>
    [Test]
    public void TestChangeUpdateOrder() {
      Component testComponent = new Component();
      testComponent.UpdateOrder = 123;
      Assert.AreEqual(123, testComponent.UpdateOrder);
    }

    /// <summary>
    ///   Tests whether changing the update order of the Component instance causes
    ///   the update order change event to be triggered
    /// </summary>
    [Test]
    public void TestUpdateOrderChangeEvent() {
      Component testComponent = new Component();
      IComponentSubscriber mockedSubscriber = mockSubscriber(testComponent);

      Expect.Once.On(mockedSubscriber).Method("UpdateOrderChanged").WithAnyArguments();
      testComponent.UpdateOrder = 123;
      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Tests whether Component can be enabled or disabled
    /// </summary>
    [Test]
    public void TestEnableDisable() {
      Component testComponent = new Component();
      testComponent.Enabled = true;
      Assert.IsTrue(testComponent.Enabled);
      testComponent.Enabled = false;
      Assert.IsFalse(testComponent.Enabled);
    }

    /// <summary>
    ///   Tests whether enabled or disabling the Component instance causes the
    ///   'enabled changed' event to be triggered
    /// </summary>
    [Test]
    public void TestEnabledChangeEvent() {
      Component testComponent = new Component();
      testComponent.Enabled = true;

      IComponentSubscriber mockedSubscriber = mockSubscriber(testComponent);

      Expect.Once.On(mockedSubscriber).Method("EnabledChanged").WithAnyArguments();
      testComponent.Enabled = false;
      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Mocks a subscriber for the events of a tracker</summary>
    /// <param name="component">Component to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IComponentSubscriber mockSubscriber(Component component) {
      IComponentSubscriber mockedSubscriber = this.mockery.NewMock<IComponentSubscriber>();

      component.EnabledChanged += new DeviceEventHandler(mockedSubscriber.EnabledChanged);
      component.UpdateOrderChanged += new DeviceEventHandler(
        mockedSubscriber.UpdateOrderChanged
      );

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.Game

#endif // UNITTEST
