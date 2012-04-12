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

using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;

using Nuclex.UserInterface.Input;
using Nuclex.Input;

using Is = NUnit.Framework.Is;
using Iz = NMock2.Is;

namespace Nuclex.UserInterface {

  /// <summary>Unit Test for the Screen class</summary>
  [TestFixture]
  internal class ScreenTest {

    #region class DelegatingControl

    /// <summary>Control that delegates input to another input receiver</summary>
    public class DelegatingControl : Controls.Control {

      /// <summary>Initializes a new input delegating control</summary>
      /// <param name="receiver">Receiver to which the input is delegated</param>
      public DelegatingControl(IInputReceiver receiver) {
        this.receiver = receiver;
      }

      /// <summary>Called when a command was sent to the control</summary>
      /// <param name="command">Command the control should perform</param>
      /// <returns>Whether the command has been processed by the control</returns>
      protected override bool OnCommand(Command command) {
        this.receiver.InjectCommand(command);
        return true;
      }

      /// <summary>Called when the mouse position is updated</summary>
      /// <param name="x">X coordinate of the mouse cursor on the GUI</param>
      /// <param name="y">Y coordinate of the mouse cursor on the GUI</param>
      protected override void OnMouseMoved(float x, float y) {
        this.receiver.InjectMouseMove(x, y);
      }

      /// <summary>Called when a mouse button has been pressed down</summary>
      /// <param name="button">Index of the button that has been pressed</param>
      protected override void OnMousePressed(MouseButtons button) {
        this.receiver.InjectMousePress(button);
      }

      /// <summary>Called when a mouse button has been released again</summary>
      /// <param name="button">Index of the button that has been released</param>
      protected override void OnMouseReleased(MouseButtons button) {
        this.receiver.InjectMouseRelease(button);
      }

      /// <summary>Called when the mouse wheel has been rotated</summary>
      /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
      protected override void OnMouseWheel(float ticks) {
        this.receiver.InjectMouseWheel(ticks);
      }

      /// <summary>Called when a key on the keyboard has been pressed down</summary>
      /// <param name="keyCode">Code of the key that was pressed</param>
      protected override bool OnKeyPressed(Keys keyCode) {
        this.receiver.InjectKeyPress(keyCode);
        return true;
      }

      /// <summary>Called when a key on the keyboard has been released again</summary>
      /// <param name="keyCode">Code of the key that was released</param>
      protected override void OnKeyReleased(Keys keyCode) {
        this.receiver.InjectKeyRelease(keyCode);
      }

      /// <summary>Input receiver all received input is delegated to</summary>
      private IInputReceiver receiver;

    }

    #endregion // class DelegatingControl

    #region class GamePadTestControl

    /// <summary>Control used to test game pad notifications</summary>
    private class GamePadTestControl : Controls.Control {

      /// <summary>Called when a button on the game pad has been pressed</summary>
      /// <param name="button">Button that has been pressed</param>
      /// <returns>
      ///   True if the button press was handled by the control, otherwise false.
      /// </returns>
      protected override bool OnButtonPressed(Buttons button) {
        ++this.HeldButtonCount;
        base.OnButtonPressed(button);
        return true;
      }

      /// <summary>Called when a button on the game pad has been released</summary>
      /// <param name="button">Button that has been released</param>
      protected override void OnButtonReleased(Buttons button) {
        --this.HeldButtonCount;
        base.OnButtonReleased(button);
      }

      /// <summary>Number of game pad buttons being held down</summary>
      public int HeldButtonCount;

    }

    #endregion // class GamePadTestControl

    #region class KeyboardTestControl

    /// <summary>Control used to test keyboard notifications</summary>
    private class KeyboardTestControl : Controls.Control {

      /// <summary>Called when a key on the keyboard has been pressed down</summary>
      /// <param name="keyCode">Code of the key that was pressed</param>
      /// <returns>
      ///   True if the key press was handled by the control, otherwise false.
      /// </returns>
      protected override bool OnKeyPressed(Keys keyCode) {
        ++this.HeldKeyCount;
        base.OnKeyPressed(keyCode);
        return true;
      }

      /// <summary>Called when a key on the keyboard has been released again</summary>
      /// <param name="keyCode">Code of the key that was released</param>
      protected override void OnKeyReleased(Keys keyCode) {
        --this.HeldKeyCount;
        base.OnKeyReleased(keyCode);
      }

      /// <summary>Number of keys being held down</summary>
      public int HeldKeyCount;

    }

    #endregion // class KeyboardTestControl

    #region class MouseTestControl

    /// <summary>Control used to test mouse notifications</summary>
    private class MouseTestControl : Controls.Control {

      /// <summary>Called when a key on the keyboard has been pressed down</summary>
      /// <param name="keyCode">Code of the key that was pressed</param>
      /// <returns>
      ///   True if the key press was handled by the control, otherwise false.
      /// </returns>
      protected override bool OnKeyPressed(Keys keyCode) { return true; }

      /// <summary>Called when a mouse button has been pressed down</summary>
      /// <param name="button">Index of the button that has been pressed</param>
      /// <returns>Whether the control has processed the mouse press</returns>
      protected override void OnMousePressed(MouseButtons button) {
        this.HeldMouseButtons |= button;
        base.OnMousePressed(button);
      }

      /// <summary>Called when a mouse button has been released again</summary>
      /// <param name="button">Index of the button that has been released</param>
      protected override void OnMouseReleased(MouseButtons button) {
        this.HeldMouseButtons &= ~button;
        base.OnMouseReleased(button);
      }

      /// <summary>Called when the mouse wheel has been rotated</summary>
      /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
      protected override void OnMouseWheel(float ticks) {
        this.MouseWheelTicks += ticks;
        base.OnMouseWheel(ticks);
      }

      /// <summary>Mouse buttons being held down</summary>
      public MouseButtons HeldMouseButtons;
      /// <summary>Ticks the mouse wheel has been rotated by</summary>
      public float MouseWheelTicks;

    }

    #endregion // class GamePadTestControl

    #region class CommandTestControl

    /// <summary>Control for testing command routing</summary>
    private class CommandTestControl : Controls.Control {

      /// <summary>Called when an input command was sent to the control</summary>
      /// <param name="command">Input command that has been triggered</param>
      /// <returns>Whether the command has been processed by the control</returns>
      protected override bool OnCommand(Command command) {
        this.LastCommand = command;
        return true;
      }

      public Command LastCommand;

    }

    #endregion // class CommandTestControl

    #region interface IFocusChangeSubscriber

    /// <summary>
    ///   Interface for a subscriber to a Control's FocusChanged event
    /// </summary>
    public interface IFocusChangeSubscriber {

      /// <summary>Called when the focused control has changed</summary>
      /// <param name="sender">Screen that is reporting the focus change</param>
      /// <param name="arguments">Contains the control that is now focused</param>
      void FocusChanged(object sender, Controls.ControlEventArgs arguments);

    }

    #endregion // interface IFocusChangeSubscriber

    /// <summary>Initialization routine executed before each test is run</summary>
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

    /// <summary>Tests the default constructor of the screen class</summary>
    [Test]
    public void TestDefaultConstructor() {
      Screen myScreen = new Screen();

      Assert.That(myScreen.Width, Is.EqualTo(0.0f));
      Assert.That(myScreen.Height, Is.EqualTo(0.0f));
    }

    /// <summary>Tests the full featured constructor of the screen class</summary>
    [Test]
    public void TestFullConstructor() {
      Screen myScreen = new Screen(12.34f, 56.78f);

      Assert.That(myScreen.Width, Is.EqualTo(12.34f));
      Assert.That(myScreen.Height, Is.EqualTo(56.78f));
    }

    /// <summary>
    ///   Verifies that an action input is processed by the screen
    /// </summary>
    [Test]
    public void TestCommandProcessing() {
      Screen testScreen = new Screen();
      testScreen.InjectCommand(Command.Accept);
      testScreen.InjectCommand(Command.Cancel);
      // No exception means success
    }

    /// <summary>
    ///   Tests whether the Gui reference is properly propagated to all controls
    ///   and their children in a control tree.
    /// </summary>
    [Test]
    public void TestScreenPropagationOnInsertion() {
      Screen myScreen = new Screen();
      Controls.Control child = new Controls.Control();

      myScreen.Desktop.Children.Add(child);

      Assert.AreSame(myScreen, child.Screen);
    }

    /// <summary>
    ///   Verifies that button presses are propagated down the control tree
    /// </summary>
    [Test]
    public void TestInjectButtonPress() {
      Screen screen = new Screen();
      GamePadTestControl control = new GamePadTestControl();
      screen.Desktop.Children.Add(control);

      screen.InjectButtonPress(Buttons.A);
      Assert.AreEqual(1, control.HeldButtonCount);
    }

    /// <summary>
    ///   Verifies that button press notifications are routed to the activated control
    ///   instead of searching for a control to handle the press
    /// </summary>
    [Test]
    public void TestButtonPressWithActivatedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      GamePadTestControl child1 = new GamePadTestControl();
      GamePadTestControl child2 = new GamePadTestControl();
      child2.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      screen.Desktop.Children.Add(child1);
      screen.Desktop.Children.Add(child2);

      // Click on child 2
      screen.InjectMouseMove(50.0f, 50.0f);
      screen.InjectMousePress(MouseButtons.Left);

      // Now child 2 should be receiving the input instead of child 1
      screen.InjectButtonPress(Buttons.A);
      Assert.AreEqual(0, child1.HeldButtonCount);
      Assert.AreEqual(1, child2.HeldButtonCount);
      screen.InjectButtonRelease(Buttons.A);
      Assert.AreEqual(0, child1.HeldButtonCount);
      Assert.AreEqual(0, child2.HeldButtonCount);
    }

    /// <summary>
    ///   Verifies that button press notifications are sent to the focused control
    ///   first when looking the a control that handles the notification
    /// </summary>
    [Test]
    public void TestButtonPressWithFocusedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      GamePadTestControl child1 = new GamePadTestControl();
      GamePadTestControl child2 = new GamePadTestControl();
      screen.Desktop.Children.Add(child1);
      screen.Desktop.Children.Add(child2);

      screen.FocusedControl = child2;

      screen.InjectButtonPress(Buttons.A);
      Assert.AreEqual(0, child1.HeldButtonCount);
      Assert.AreEqual(1, child2.HeldButtonCount);
      screen.InjectButtonRelease(Buttons.A);
      Assert.AreEqual(0, child1.HeldButtonCount);
      Assert.AreEqual(0, child2.HeldButtonCount);
    }

    /// <summary>
    ///   Verifies that key presses are propagated down the control tree
    /// </summary>
    [Test]
    public void TestInjectKeyPress() {
      Screen screen = new Screen();
      KeyboardTestControl control = new KeyboardTestControl();
      screen.Desktop.Children.Add(control);

      screen.InjectKeyPress(Keys.A);
      Assert.AreEqual(1, control.HeldKeyCount);
    }

    /// <summary>
    ///   Verifies that key press notifications are routed to the activated control
    ///   instead of searching for a control to handle the press
    /// </summary>
    [Test]
    public void TestKeyPressWithActivatedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      KeyboardTestControl child1 = new KeyboardTestControl();
      KeyboardTestControl child2 = new KeyboardTestControl();
      child2.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      screen.Desktop.Children.Add(child1);
      screen.Desktop.Children.Add(child2);

      // Click on child 2
      screen.InjectMouseMove(50.0f, 50.0f);
      screen.InjectMousePress(MouseButtons.Left);

      // Now child 2 should be receiving the input instead of child 1
      screen.InjectKeyPress(Keys.A);
      Assert.AreEqual(0, child1.HeldKeyCount);
      Assert.AreEqual(1, child2.HeldKeyCount);
      screen.InjectKeyRelease(Keys.A);
      Assert.AreEqual(0, child1.HeldKeyCount);
      Assert.AreEqual(0, child2.HeldKeyCount);
    }

    /// <summary>
    ///   Verifies that key press notifications are sent to the focused control
    ///   first when looking the a control that handles the notification
    /// </summary>
    [Test]
    public void TestKeyPressWithFocusedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      KeyboardTestControl child1 = new KeyboardTestControl();
      KeyboardTestControl child2 = new KeyboardTestControl();
      screen.Desktop.Children.Add(child1);
      screen.Desktop.Children.Add(child2);

      screen.FocusedControl = child2;

      screen.InjectKeyPress(Keys.A);
      Assert.AreEqual(0, child1.HeldKeyCount);
      Assert.AreEqual(1, child2.HeldKeyCount);
      screen.InjectKeyRelease(Keys.A);
      Assert.AreEqual(0, child1.HeldKeyCount);
      Assert.AreEqual(0, child2.HeldKeyCount);
    }

    /// <summary>
    ///   Verifies that mouse pressed are routed to the activated control
    /// </summary>
    [Test]
    public void TestMousePressWithActivatedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      MouseTestControl child = new MouseTestControl();
      child.Bounds = new UniRectangle(55.0f, 10.0f, 35.0f, 80.0f);
      screen.Desktop.Children.Add(child);

      screen.InjectKeyPress(Keys.A);

      screen.InjectMousePress(MouseButtons.Left);
      Assert.AreEqual(MouseButtons.Left, child.HeldMouseButtons);
    }

    /// <summary>
    ///   Verifies that the FocusChanged event is triggered when the control
    ///   in focus changes
    /// </summary>
    [Test]
    public void TestFocusChangeEvent() {
      Screen screen = new Screen(100.0f, 100.0f);
      IFocusChangeSubscriber mockedSubscriber = mockSubscriber(screen);

      Controls.Control child1 = new Controls.Control();
      Controls.Control child2 = new Controls.Control();
      screen.Desktop.Children.Add(child1);
      screen.Desktop.Children.Add(child2);

      Expect.Once.On(mockedSubscriber).Method("FocusChanged").WithAnyArguments();
      screen.FocusedControl = child1;
      Expect.Once.On(mockedSubscriber).Method("FocusChanged").WithAnyArguments();
      screen.FocusedControl = child2;

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Verifies that mouse wheel rotations are propagated down the control tree
    /// </summary>
    [Test]
    public void TestMouseWheel() {
      Screen screen = new Screen();
      MouseTestControl control = new MouseTestControl();
      control.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      screen.Desktop.Children.Add(control);

      screen.InjectMouseMove(50.0f, 50.0f);
      screen.InjectMouseWheel(12.34f);
      Assert.AreEqual(12.34f, control.MouseWheelTicks);
    }

    /// <summary>
    ///   Verifies that mouse wheel rotations are sent to the activated control first
    /// </summary>
    [Test]
    public void TestMouseWheelWithActivatedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      MouseTestControl control = new MouseTestControl();
      control.Bounds = new UniRectangle(10.0f, 10.0f, 80.0f, 80.0f);
      screen.Desktop.Children.Add(control);

      screen.InjectKeyPress(Keys.A);
      screen.InjectMouseWheel(12.34f);
      Assert.AreEqual(12.34f, control.MouseWheelTicks);
    }

    /// <summary>
    ///   Verifies that entered characters are sent to the focused control
    /// </summary>
    [Test]
    public void TestInjectCharacter() {
      Screen screen = new Screen(100.0f, 100.0f);
      Controls.Desktop.InputControl control = new Controls.Desktop.InputControl();
      screen.Desktop.Children.Add(control);

      screen.InjectCharacter('a');
      Assert.AreEqual(string.Empty, control.Text);

      screen.FocusedControl = control;

      screen.InjectCharacter('a');
      Assert.AreEqual("a", control.Text);
    }

    /// <summary>Tests whether focus can be changed using the keyboard</summary>
    [Test]
    public void TestFocusSwitching() {
      Screen screen = new Screen(100.0f, 100.0f);
      Controls.Desktop.ButtonControl center = new Controls.Desktop.ButtonControl();
      center.Bounds = new UniRectangle(40, 40, 20, 20);
      Controls.Desktop.ButtonControl left = new Controls.Desktop.ButtonControl();
      left.Bounds = new UniRectangle(10, 51, 20, 20);
      Controls.Desktop.ButtonControl right = new Controls.Desktop.ButtonControl();
      right.Bounds = new UniRectangle(70, 29, 20, 20);
      Controls.Desktop.ButtonControl up = new Controls.Desktop.ButtonControl();
      up.Bounds = new UniRectangle(29, 10, 20, 20);
      Controls.Desktop.ButtonControl down = new Controls.Desktop.ButtonControl();
      down.Bounds = new UniRectangle(51, 70, 20, 20);

      screen.Desktop.Children.Add(center);
      screen.Desktop.Children.Add(left);
      screen.Desktop.Children.Add(right);
      screen.Desktop.Children.Add(up);
      screen.Desktop.Children.Add(down);

      screen.FocusedControl = center;
      screen.InjectKeyPress(Keys.Down);
      Assert.AreSame(down, screen.FocusedControl);

      screen.FocusedControl = center;
      screen.InjectKeyPress(Keys.Up);
      Assert.AreSame(up, screen.FocusedControl);

      screen.FocusedControl = center;
      screen.InjectKeyPress(Keys.Left);
      Assert.AreSame(left, screen.FocusedControl);

      screen.FocusedControl = center;
      screen.InjectKeyPress(Keys.Right);
      Assert.AreSame(right, screen.FocusedControl);
    }

    /// <summary>
    ///   Verifies that the screen skips an unfocusable control and jumps to the
    ///   next focusable control.
    /// </summary>
    [Test]
    public void TestFocusSwitchingWithUnfocusableControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      Controls.Desktop.ButtonControl one = new Controls.Desktop.ButtonControl();
      one.Bounds = new UniRectangle(40, 10, 20, 20);
      Controls.Control two = new Controls.Control();
      two.Bounds = new UniRectangle(40, 40, 20, 20);
      Controls.Desktop.ButtonControl three = new Controls.Desktop.ButtonControl();
      three.Bounds = new UniRectangle(40, 70, 20, 20);

      screen.Desktop.Children.Add(one);
      screen.Desktop.Children.Add(two);
      screen.Desktop.Children.Add(three);

      screen.FocusedControl = one;
      screen.InjectKeyPress(Keys.Down);
      screen.InjectKeyRelease(Keys.Down);
      Assert.AreSame(three, screen.FocusedControl);
    }

    /// <summary>
    ///   Verifies that if the focused control handles a directional command, no
    ///   focus switching will occur
    /// </summary>
    [Test]
    public void TestNoFocusChangeOnHandledDirectionalCommand() {
      Screen screen = new Screen(100.0f, 100.0f);
      IInputReceiver mockedReceiver = mockReceiver(screen);
      DelegatingControl one = new DelegatingControl(mockedReceiver);
      one.Bounds = new UniRectangle(40, 10, 20, 20);
      Controls.Desktop.ButtonControl two = new Controls.Desktop.ButtonControl();
      two.Bounds = new UniRectangle(40, 40, 20, 20);

      screen.Desktop.Children.Add(one);
      screen.Desktop.Children.Add(two);

      Expect.Once.On(mockedReceiver).Method("InjectCommand").WithAnyArguments();

      screen.FocusedControl = one;
      screen.InjectCommand(Command.Down);

      Assert.AreSame(one, screen.FocusedControl);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>
    ///   Verifies that the screen can handle a focus switch request without any
    ///   focused control
    /// </summary>
    [Test]
    public void TestFocusSwitchingWithoutFocusedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      screen.InjectCommand(Command.Down);
      // No exception means success
    }

    /// <summary>
    ///   Verifies that the screen can handle a focus switch when the control
    ///   currently assigned as the focused control has been disconnected from the tree
    /// </summary>
    [Test]
    public void TestFocusSwitchingWithDisconnectedControl() {
      Screen screen = new Screen(100.0f, 100.0f);
      screen.FocusedControl = new Controls.Control();
      screen.InjectCommand(Command.Down);
      // No exception means success
    }

    /// <summary>
    ///   Verifies that classic focus switching is still supported
    /// </summary>
    [Test]
    public void TestClassicFocusSwitching() {
      Screen screen = new Screen(100.0f, 100.0f);
      screen.InjectCommand(Command.SelectPrevious);
      screen.InjectCommand(Command.SelectNext);
      // TODO: Implement classic focus switching and do some real testing here
    }

    /// <summary>
    ///   Verifies that classic focus switching is still supported
    /// </summary>
    [Test]
    public void TestAcceptAndCancelFromKeyboard() {
      Screen screen = new Screen(100.0f, 100.0f);
      CommandTestControl test = new CommandTestControl();
      screen.Desktop.Children.Add(test);
      screen.FocusedControl = test;

      screen.InjectKeyPress(Keys.Escape);
      Assert.AreEqual(Command.Cancel, test.LastCommand);

      screen.InjectKeyPress(Keys.Enter);
      Assert.AreEqual(Command.Accept, test.LastCommand);
    }

    /// <summary>Mocks a receiver for the input processing of a control</summary>
    /// <param name="screen">Screen to mock an input receiver on</param>
    /// <returns>The mocked input receiver</returns>
    private IInputReceiver mockReceiver(Screen screen) {
      IInputReceiver mockedReceiver = this.mockery.NewMock<IInputReceiver>();

      DelegatingControl delegatingControl = new DelegatingControl(mockedReceiver);
      screen.Desktop.Children.Add(delegatingControl);
      screen.FocusedControl = delegatingControl;

      return mockedReceiver;
    }

    /// <summary>Mocks a subscriber to the screen's events</summary>
    /// <param name="screen">Screen to mock an event subcriber to</param>
    /// <returns>The mocked event subscriber</returns>
    private IFocusChangeSubscriber mockSubscriber(Screen screen) {
      IFocusChangeSubscriber mockedSubscriber =
        this.mockery.NewMock<IFocusChangeSubscriber>();

      screen.FocusChanged += new EventHandler<Controls.ControlEventArgs>(
        mockedSubscriber.FocusChanged
      );

      return mockedSubscriber;
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;

  }

} // namespace Nuclex.UserInterface

#endif // UNITTEST
