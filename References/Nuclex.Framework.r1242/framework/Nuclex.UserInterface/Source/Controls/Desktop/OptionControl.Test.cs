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

using Nuclex.Support;

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the option control class</summary>
  [TestFixture]
  internal class OptionControlTest {

    #region interface IOptionSubscriber

    /// <summary>Interface for a subscriber to the option control's events</summary>
    public interface IOptionSubscriber {
    
      /// <summary>Called when the option control's state changes</summary>
      /// <param name="sender">Option control whose state has changed</param>
      /// <param name="arguments">Not used</param>
      void Changed(object sender, EventArgs arguments);

    }
    
    #endregion // interface IOptionSubscriber

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

    /// <summary>Verifies whether the option can be changed using the mouse</summary>
    [Test]
    public void TestOptionToggleByMouse() {
      OptionControl option = new OptionControl();
      option.Bounds = new UniRectangle(10, 10, 100, 100);

      IOptionSubscriber mockedSubscriber = mockSubscriber(option);
      Expect.Once.On(mockedSubscriber).Method("Changed").WithAnyArguments();

      Assert.IsFalse(option.Selected);

      // Move the mouse over the button and do a left-click
      option.ProcessMouseMove(0, 0, 50, 50);
      option.ProcessMousePress(MouseButtons.Left);
      option.ProcessMouseRelease(MouseButtons.Left);

      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.IsTrue(option.Selected);
    }

    /// <summary>
    ///   Tests whether a button press can be aborted at the last second by moving
    ///   the mouse away from the button before the mouse button is released
    /// </summary>
    [Test]
    public void TestLastSecondAbortByMouse() {
      OptionControl option = new OptionControl();
      option.Bounds = new UniRectangle(10, 10, 100, 100);

      IOptionSubscriber mockedSubscriber = mockSubscriber(option);
      Expect.Never.On(mockedSubscriber).Method("Changed").WithAnyArguments();

      Assert.IsFalse(option.Selected);

      // Move the mouse over the button and do a left-click
      option.ProcessMouseMove(0, 0, 50, 50);
      option.ProcessMousePress(MouseButtons.Left);
      option.ProcessMouseMove(0, 0, 5, 5); // outside of the button
      option.ProcessMouseRelease(MouseButtons.Left);

      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.IsFalse(option.Selected);
    }

    /// <summary>Mocks a subscriber for the events of an option</summary>
    /// <param name="option">Option to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IOptionSubscriber mockSubscriber(OptionControl option) {
      IOptionSubscriber mockedSubscriber = this.mockery.NewMock<IOptionSubscriber>();

      option.Changed += new EventHandler(mockedSubscriber.Changed);

      return mockedSubscriber;
    }

    /// <summary>Manages mocked interfaces and verifies expectations</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
