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
using System.Windows.Forms;

using NUnit.Framework;
using NMock2;

using Nuclex.Input.Devices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Nuclex.Input {

  /// <summary>Unit tests for the input manager</summary>
  [TestFixture]
  internal class InputManagerTest {

    #region interface IUpdateableSubscriber

    /// <summary>Subscriber to an updateable object</summary>
    public interface IUpdateableSubscriber {

      /// <summary>Called when the updateable's update order has been changed</summary>
      /// <param name="sender">Updateable who's update order changed</param>
      /// <param name="arguments">Not used</param>
      void UpdateOrderChanged(object sender, EventArgs arguments);

      /// <summary>Called when the updateable is enabled or disabled</summary>
      /// <param name="sender">Updateable that has been enabled or disabled</param>
      /// <param name="arguments">Not used</param>
      void EnabledChanged(object sender, EventArgs arguments);

    }

    #endregion // interface IUpdateableSubscriber

    /// <summary>Ensures that the default constructor is working</summary>
    [Test]
    public void TestDefaultConstructor() {
      using (var manager = new InputManager()) {
        Assert.IsNotNull(manager); // nonsense, avoids compiler warning
      }
    }

    /// <summary>Ensures that the service container constructor is working</summary>
    [Test]
    public void TestServiceConstructor() {
      var services = new GameServiceContainer();

      using (var manager = new InputManager(services)) {
        Assert.IsNotNull(services.GetService(typeof(IInputService)));
      }

      Assert.IsNull(services.GetService(typeof(IInputService)));
    }

    /// <summary>Verifies that the keyboards collection isn't empty</summary>
    [Test]
    public void TestKeyboardsCollection() {
      using (var manager = new InputManager()) {
        Assert.Greater(manager.Keyboards.Count, 0);
      }
    }

    /// <summary>Verifies that the mice collection isn't empty</summary>
    [Test]
    public void TestMiceCollection() {
      using (var manager = new InputManager()) {
        Assert.Greater(manager.Mice.Count, 0);
      }
    }

    /// <summary>Verifies that the game pads collection isn't empty</summary>
    [Test]
    public void TestGamePadsCollection() {
      using (var manager = new InputManager()) {
        Assert.Greater(manager.GamePads.Count, 0);
      }
    }

    /// <summary>Verifies that a mouse can be retrieved</summary>
    [Test]
    public void TestGetMouse() {
      using (var manager = new InputManager()) {
        Assert.IsNotNull(manager.GetMouse());
      }
    }

    /// <summary>Verifies that the main keyboard can be retrieved</summary>
    [Test]
    public void TestGetKeyboard() {
      using (var manager = new InputManager()) {
        Assert.IsNotNull(manager.GetKeyboard());
      }
    }

    /// <summary>Verifies that a chat pad can be retrieved</summary>
    [Test]
    public void TestGetChatPad() {
      using (var manager = new InputManager()) {
        Assert.IsNotNull(manager.GetKeyboard(PlayerIndex.One));
      }
    }

    /// <summary>Verifies that an XINPUT game pad can be retrieved</summary>
    [Test]
    public void TestGetXinputGamePad() {
      using (var manager = new InputManager()) {
        Assert.IsNotNull(manager.GetGamePad(PlayerIndex.One));
      }
    }

    /// <summary>Verifies that a DirectInput game pad can be retrieved</summary>
    [Test]
    public void TestGetDirectInputGamePad() {
      using (var manager = new InputManager()) {
        Assert.IsNotNull(manager.GetGamePad(ExtendedPlayerIndex.Five));
      }
    }

    /// <summary>Verifies that the snapshot system is working</summary>
    [Test]
    public void TestSnapshots() {
      using (var manager = new InputManager()) {
        Assert.AreEqual(0, manager.SnapshotCount);
        manager.TakeSnapshot();
        Assert.AreEqual(1, manager.SnapshotCount);
        manager.Update();
        Assert.AreEqual(0, manager.SnapshotCount);
      }
    }

    /// <summary>Verifies that the UpdateOrder property behaves correctly</summary>
    [Test]
    public void TestChangeUpdateOrder() {
      using (Mockery mockery = new Mockery()) {
        using (var manager = new InputManager()) {
          IUpdateableSubscriber updateable = mockery.NewMock<IUpdateableSubscriber>();
          manager.EnabledChanged += updateable.EnabledChanged;
          manager.UpdateOrderChanged += updateable.UpdateOrderChanged;

          Expect.Once.On(updateable).Method("UpdateOrderChanged").WithAnyArguments();
          manager.UpdateOrder = 123;
          Assert.AreEqual(123, manager.UpdateOrder);

          manager.UpdateOrderChanged -= updateable.UpdateOrderChanged;
          manager.EnabledChanged -= updateable.EnabledChanged;
        }

        mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>
    ///   Verifies that input manager implements the IGameComponent interface
    /// </summary>
    [Test]
    public void TestInitializeGameComponent() {
      using (var manager = new InputManager()) {
        ((IGameComponent)manager).Initialize();
      }
    }

    /// <summary>
    ///   Verifies that input manager provides an enabled property
    /// </summary>
    [Test]
    public void TestEnabledProperty() {
      using (var manager = new InputManager()) {
        Assert.IsTrue(((IUpdateable)manager).Enabled);
      }
    }

    /// <summary>
    ///   Verifies that the input manager can be updated via the IUpdateable interface
    /// </summary>
    [Test]
    public void TestUpdateViaIUpdateable() {
      using (var manager = new InputManager()) {
        ((IUpdateable)manager).Update(new GameTime());
      }
    }

  }

} // namespace Nuclex.Input

#endif // UNITTEST
