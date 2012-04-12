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

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.Support;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the choice control class</summary>
  [TestFixture]
  internal class ChoiceControlTest {

    #region interface IChoiceSubscriber

    /// <summary>Interface for a subscriber to the choice control's events</summary>
    public interface IChoiceSubscriber {

      /// <summary>Called when the choice control's state changes</summary>
      /// <param name="sender">Choice control whose state has changed</param>
      /// <param name="arguments">Not used</param>
      void Changed(object sender, EventArgs arguments);

    }

    #endregion // interface IChoiceSubscriber

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();

      this.parent = new Control();
      this.child1 = new ChoiceControl();
      this.child1.Bounds = new UniRectangle(10, 10, 100, 100);
      this.child2 = new ChoiceControl();
      this.child2.Bounds = new UniRectangle(10, 110, 100, 100);

      this.parent.Children.Add(this.child1);
      this.parent.Children.Add(this.child2);
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>Verifies that the option can work without a parent</summary>
    [Test]
    public void TestChoiceWithoutParent() {
      ChoiceControl choice = new ChoiceControl();
      choice.Bounds = new UniRectangle(10, 10, 100, 100);

      IChoiceSubscriber mockedSubscriber = mockSubscriber(choice);
      Expect.Once.On(mockedSubscriber).Method("Changed").WithAnyArguments();

      Assert.IsFalse(choice.Selected);

      // Move the mouse over the choice and do a left-click
      choice.ProcessMouseMove(0, 0, 50, 50);
      choice.ProcessMousePress(MouseButtons.Left);
      choice.ProcessMouseRelease(MouseButtons.Left);

      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.IsTrue(choice.Selected);
    }

    /// <summary>
    ///   Verifies that a selected choice remains selected if the user clicks on it
    /// </summary>
    [Test]
    public void TestClickOnSelectedChoice() {
      ChoiceControl choice = new ChoiceControl();
      choice.Bounds = new UniRectangle(10, 10, 100, 100);

      choice.Selected = true;

      IChoiceSubscriber mockedSubscriber = mockSubscriber(choice);
      Expect.Never.On(mockedSubscriber).Method("Changed").WithAnyArguments();

      Assert.IsTrue(choice.Selected);

      // Move the mouse over the choice and do a left-click
      choice.ProcessMouseMove(0, 0, 50, 50);
      choice.ProcessMousePress(MouseButtons.Left);
      choice.ProcessMouseRelease(MouseButtons.Left);

      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.IsTrue(choice.Selected);
    }

    /// <summary>
    ///   Verifies that all other choices in the same parent are disabled when
    ///   a choice is clicked on
    /// </summary>
    [Test]
    public void TestToggleChoicesByMouse() {
      this.child1.Selected = true;

      IChoiceSubscriber mockedSubscriber1 = mockSubscriber(this.child1);
      IChoiceSubscriber mockedSubscriber2 = mockSubscriber(this.child2);

      Expect.Once.On(mockedSubscriber1).Method("Changed").WithAnyArguments();
      Expect.Once.On(mockedSubscriber2).Method("Changed").WithAnyArguments();

      Assert.IsTrue(this.child1.Selected);
      Assert.IsFalse(this.child2.Selected);

      // Move the mouse over the choice and do a left-click
      this.child2.ProcessMouseMove(0, 0, 50, 150);
      this.child2.ProcessMousePress(MouseButtons.Left);
      this.child2.ProcessMouseRelease(MouseButtons.Left);

      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.IsFalse(this.child1.Selected);
      Assert.IsTrue(this.child2.Selected);
    }

    /// <summary>
    ///   Tests whether a choice click can be aborted at the last second by moving
    ///   the mouse away from the choice before the mouse button is released
    /// </summary>
    [Test]
    public void TestLastSecondAbortByMouse() {
      this.child1.Selected = true;

      IChoiceSubscriber mockedSubscriber1 = mockSubscriber(this.child1);
      IChoiceSubscriber mockedSubscriber2 = mockSubscriber(this.child2);

      Expect.Never.On(mockedSubscriber1).Method("Changed").WithAnyArguments();
      Expect.Never.On(mockedSubscriber2).Method("Changed").WithAnyArguments();

      Assert.IsTrue(this.child1.Selected);
      Assert.IsFalse(this.child2.Selected);

      // Move the mouse over the button and do a left-click
      this.child2.ProcessMouseMove(0, 0, 50, 150);
      this.child2.ProcessMousePress(MouseButtons.Left);
      this.child2.ProcessMouseMove(0, 0, 5, 5); // outside of the button
      this.child2.ProcessMouseRelease(MouseButtons.Left);

      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.IsTrue(this.child1.Selected);
      Assert.IsFalse(this.child2.Selected);
    }

    /// <summary>Mocks a subscriber for the events of a choice</summary>
    /// <param name="choice">Choice to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private IChoiceSubscriber mockSubscriber(ChoiceControl choice) {
      IChoiceSubscriber mockedSubscriber = this.mockery.NewMock<IChoiceSubscriber>();

      choice.Changed += new EventHandler(mockedSubscriber.Changed);

      return mockedSubscriber;
    }

    /// <summary>Manages mocked interfaces and verifies expectations</summary>
    private Mockery mockery;

    /// <summary>Parent control the choice controls are contained in</summary>
    private Control parent;
    /// <summary>First choice control in the parent</summary>
    private ChoiceControl child1;
    /// <summary>Second choice control in the parent</summary>
    private ChoiceControl child2;

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
