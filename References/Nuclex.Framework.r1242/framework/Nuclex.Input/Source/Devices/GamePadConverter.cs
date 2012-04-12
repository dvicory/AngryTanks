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

#if !NO_DIRECTINPUT

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using SlimDX.DirectInput;

using XnaGamePadButtons = Microsoft.Xna.Framework.Input.GamePadButtons;

namespace Nuclex.Input.Devices {

  /// <summary>Converts DirectInput joystick states into XNA game pad states</summary>
  internal partial class GamePadConverter {

    #region class FlatGamePadState

    /// <summary>Stores all fields of a GamePadState in a flat structure</summary>
    private class FlatGamePadState {
      /// <summary>Position of the left thumb stick</summary>
      public Vector2 LeftThumbStick;
      /// <summary>Position of the right thumb stick</summary>
      public Vector2 RightThumbStick;
      /// <summary>Level of the left trigger</summary>
      public float LeftTrigger;
      /// <summary>Level of the right trigger</summary>
      public float RightTrigger;
      /// <summary>State of the game pad's digital buttons</summary>
      public Buttons Buttons;
    }

    #endregion // class FlatGamePadState

    /// <summary>Initializes a new game pad converter</summary>
    /// <param name="joystick">Joystick whose data will be converted</param>
    public GamePadConverter(Joystick joystick) {
      this.joystick = joystick;
      this.gamePadState = new FlatGamePadState();

      this.axisCount = Math.Min(this.joystick.Capabilities.AxesCount, 4);
      this.hasPovHat = (this.joystick.Capabilities.PovCount > 0);
      if (this.hasPovHat) {
        this.buttonCount = Math.Min(this.joystick.Capabilities.ButtonCount, 10);
      } else {
        this.buttonCount = Math.Min(this.joystick.Capabilities.ButtonCount, 14);
      }

      this.convertDelegate = convertButtonsAndPov;
      addAxisConverters();
      addSliderConverters();
    }

    /// <summary>Converts the provided joystick state into a game pad state</summary>
    /// <param name="joystickState">Joystick state that will be converted</param>
    /// <returns>A game pad state matching the provided joystick state</returns>
    public GamePadState Convert(ref JoystickState joystickState) {
      this.convertDelegate(this.gamePadState, ref joystickState);

      return new GamePadState(
        new GamePadThumbSticks(
          this.gamePadState.LeftThumbStick,
          this.gamePadState.RightThumbStick
        ),
        new GamePadTriggers(
          this.gamePadState.LeftTrigger,
          this.gamePadState.RightTrigger
        ),
        new XnaGamePadButtons(
          this.gamePadState.Buttons
        ),
        new GamePadDPad(
          (this.gamePadState.Buttons & Buttons.DPadUp) != 0 ?
            ButtonState.Pressed : ButtonState.Released,
          (this.gamePadState.Buttons & Buttons.DPadDown) != 0 ?
            ButtonState.Pressed : ButtonState.Released,
          (this.gamePadState.Buttons & Buttons.DPadLeft) != 0 ?
            ButtonState.Pressed : ButtonState.Released,
          (this.gamePadState.Buttons & Buttons.DPadRight) != 0 ?
            ButtonState.Pressed : ButtonState.Released
        )
      );
    }

    /// <summary>Sets up the converters for the game pads axes</summary>
    private void addAxisConverters() {
      IList<DeviceObjectInstance> axes = this.joystick.GetObjects(
        ObjectDeviceType.AbsoluteAxis
      );

      // If this game pad has an X axis, use it for the left thumb stick
      if (countInstances(axes, ObjectGuid.XAxis) > 0) {
        int id = (int)getInstance(axes, ObjectGuid.XAxis, 0).ObjectType;
        ObjectProperties properties = this.joystick.GetObjectPropertiesById(id);
        convertDelegate += new XAxisConverter(
          properties.LowerRange, properties.UpperRange
        ).Convert;
      }

      // If this game pad has an Y axis, use it for the left thumb stick
      if (countInstances(axes, ObjectGuid.YAxis) > 0) {
        int id = (int)getInstance(axes, ObjectGuid.YAxis, 0).ObjectType;
        ObjectProperties properties = this.joystick.GetObjectPropertiesById(id);
        convertDelegate += new YAxisConverter(
          properties.LowerRange, properties.UpperRange
        ).Convert;
      }

      // If this game pad has a rotational X axis, use it for the right thumb stick
      if (countInstances(axes, ObjectGuid.RotationalXAxis) > 0) {
        int id = (int)getInstance(axes, ObjectGuid.RotationalXAxis, 0).ObjectType;
        ObjectProperties properties = this.joystick.GetObjectPropertiesById(id);
        convertDelegate += new RotationalXAxisConverter(
          properties.LowerRange, properties.UpperRange
        ).Convert;
      }

      // If this game pad has a rotational Y axis, use it for the right thumb stick
      if (countInstances(axes, ObjectGuid.RotationalYAxis) > 0) {
        int id = (int)getInstance(axes, ObjectGuid.RotationalYAxis, 0).ObjectType;
        ObjectProperties properties = this.joystick.GetObjectPropertiesById(id);
        convertDelegate += new RotationalYAxisConverter(
          properties.LowerRange, properties.UpperRange
        ).Convert;
      }
    }

    /// <summary>Adds the game pad converters for the sliders / triggers</summary>
    private void addSliderConverters() {
      IList<DeviceObjectInstance> axes = this.joystick.GetObjects(
        ObjectDeviceType.AbsoluteAxis
      );

      int sliderCount = countInstances(axes, ObjectGuid.Slider);
      if (sliderCount < 2) {

        // Less than two sliders but an Rz axis? Rz axis is left trigger.
        if (countInstances(axes, ObjectGuid.RotationalZAxis) > 0) {

          int id = (int)getInstance(axes, ObjectGuid.RotationalZAxis, 0).ObjectType;
          ObjectProperties properties = this.joystick.GetObjectPropertiesById(id);
          convertDelegate += new RotationalZAxisLeftTriggerConverter(
            properties.LowerRange, properties.UpperRange
          ).Convert;

          // Rz axis plus one real slider? Make it the right trigger.
          if (sliderCount > 0) {
            id = (int)getInstance(axes, ObjectGuid.Slider, 0).ObjectType;
            properties = this.joystick.GetObjectPropertiesById(id);
            convertDelegate += new SliderRightTriggerConverter(
              0, properties.LowerRange, properties.UpperRange
            ).Convert;
          }

        } else if (sliderCount > 0) { // No Rz axis, only one slider. It's the left trigger.

          int id = (int)getInstance(axes, ObjectGuid.Slider, 0).ObjectType;
          ObjectProperties properties = this.joystick.GetObjectPropertiesById(id);
          convertDelegate += new SliderLeftTriggerConverter(
            0, properties.LowerRange, properties.UpperRange
          ).Convert;

        }

      }

      // Two sliders? Make them the left and right triggers.
      if (sliderCount >= 2) {

        int id = (int)getInstance(axes, ObjectGuid.Slider, 0).ObjectType;
        ObjectProperties properties = this.joystick.GetObjectPropertiesById(id);
        convertDelegate += new SliderLeftTriggerConverter(
          0, properties.LowerRange, properties.UpperRange
        ).Convert;

        id = (int)getInstance(axes, ObjectGuid.Slider, 1).ObjectType;
        properties = this.joystick.GetObjectPropertiesById(id);
        convertDelegate += new SliderLeftTriggerConverter(
          1, properties.LowerRange, properties.UpperRange
        ).Convert;

      }
    }

    /// <summary>Converts all digital buttons and the controller's PoV hat</summary>
    /// <param name="gamePadState">Game pad state that will receive the results</param>
    /// <param name="joystickState">Joystick state the values are taken from</param>
    private void convertButtonsAndPov(
      FlatGamePadState gamePadState, ref JoystickState joystickState
    ) {
      Buttons pressedButtons = 0;

      // Try to match up the joystick's buttons with those that would be on
      // an XBox 360 controller.
      bool[] buttons = joystickState.GetButtons();
      for (int index = 0; index < this.buttonCount; ++index) {
        if (buttons[index]) {
          pressedButtons |= buttonOrder[index];
        }
      }

      // If this controller has a Point-of-View hat, we interpret the hat as
      // 4 additional buttons. If not, the buttonCount property is limited to
      // 14, allowing an additional 4 normal buttons on the controller to act
      // as if they were a PoV hat.
      if (this.hasPovHat) {
        int pov = joystickState.GetPointOfViewControllers()[0];

        // PoV hats report either -1 or 65535 when they're centered and their
        // position in degrees times 100 if they're not centered.
        if ((ushort)(pov) != 0xFFFF) {
          bool right = ((pov > 0) && (pov < 18000));
          bool down = ((pov > 9000) && (pov < 27000));
          bool left = ((pov > 18000) && (pov < 36000));
          bool up = ((pov > 27000) || (pov < 9000));

          if (right) { pressedButtons |= Buttons.DPadRight; }
          if (down) { pressedButtons |= Buttons.DPadDown; }
          if (left) { pressedButtons |= Buttons.DPadLeft; }
          if (up) { pressedButtons |= Buttons.DPadUp; }
        }
      }

      this.gamePadState.Buttons = pressedButtons;
    }

    /// <summary>Counts the number of input objects of a certain type</summary>
    /// <param name="list">List in which the input objects will be counted</param>
    /// <param name="typeGuid">The guid of the input objects to count</param>
    /// <returns>The number of input objects of the specified type</returns>
    private static int countInstances(
      IList<DeviceObjectInstance> list, Guid typeGuid
    ) {
      int count = 0;

      for (int index = 0; index < list.Count; ++index) {
        if (list[index].ObjectTypeGuid == typeGuid) {
          ++count;
        }
      }

      return count;
    }

    /// <summary>Returns the nth input object of the specified type</summary>
    /// <param name="list">List the input objects will be looked up in</param>
    /// <param name="typeGuid">Guid of the input objects to look up</param>
    /// <param name="wantedIndex">The index of the desired input object</param>
    /// <returns>The input object according to the parameters specified</returns>
    private static DeviceObjectInstance getInstance(
      IList<DeviceObjectInstance> list, Guid typeGuid, int wantedIndex
    ) {
      int count = 0;

      for (int index = 0; index < list.Count; ++index) {
        if (list[index].ObjectTypeGuid == typeGuid) {
          if (count == wantedIndex) {
            return list[index];
          }
          ++count;
        }
      }

      throw new IndexOutOfRangeException("Instance index out of range");
    }

    /// <summary>Order in which the buttons reported by DirectInput appear</summary>
    /// <remarks>
    ///   Tested this with an XBox 360 game pad. An older game pad used a completely
    ///   arbitrary order and there's no way to find out which button resembles what,
    ///   so I'm hoping that the XBox 360's DirectInput driver sets an inofficial
    ///   standard and others copy the order in which its buttons are listed.
    /// </remarks>
    private static readonly Buttons[] buttonOrder = new Buttons[] {
      Buttons.A,
      Buttons.B,
      Buttons.X,
      Buttons.Y,
      Buttons.LeftShoulder,
      Buttons.RightShoulder,
      Buttons.Back,
      Buttons.Start,
      Buttons.LeftStick,
      Buttons.RightStick,
      Buttons.DPadUp,
      Buttons.DPadDown,
      Buttons.DPadLeft,
      Buttons.DPadRight
    };

    /// <summary>Joystick the converter is working with</summary>
    private Joystick joystick;

    /// <summary>The number of axes on the DirectInput joystick</summary>
    private int axisCount;
    /// <summary>The number of buttons on the DirectInput joystick</summary>
    private int buttonCount;
    /// <summary>Whether the DirectInput joystick has a POV hat (or DPAD)</summary>
    private bool hasPovHat;

    /// <summary>Multi cast delegate invoking all needed conversion methods</summary>
    private ConverterDelegate convertDelegate;

    /// <summary>Modifiable game pad state in which the conversion methods work</summary>
    private FlatGamePadState gamePadState;

  }

} // namespace Nuclex.Input.Devices

#endif // !NO_DIRECTINPUT
