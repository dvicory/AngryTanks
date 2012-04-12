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

using System;
using System.Collections.Generic;

#if !NO_DIRECTINPUT

using SlimDX.DirectInput;

namespace Nuclex.Input.Devices {

  partial class GamePadConverter {

    /// <summary>Converts a joystick state field to a game pad state field</summary>
    /// <param name="gamePadState">Game pad state the field will be written to</param>
    /// <param name="joystickState">Joystick state the field will be written to</param>
    private delegate void ConverterDelegate(
      FlatGamePadState gamePadState, ref JoystickState joystickState
    );

    #region class AxisConverter

    /// <summary>Base class for the joystick axis converters</summary>
    private class AxisConverter {

      /// <summary>Initializes a new X axis converter</summary>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public AxisConverter(int min, int max) {
        this.Center = (min + max) / 2;
        this.Min = (float)(min - this.Center);
        this.Max = (float)(max - this.Center);
      }

      /// <summary>The centered position of the axis</summary>
      protected int Center;
      /// <summary>Positive range of the axis</summary>
      protected float Min;
      /// <summary>Negative range of the axis</summary>
      protected float Max;

    }

    #endregion // class AxisConverter

    #region class XAxisConverter

    /// <summary>Converts the value of the joystick's X axis</summary>
    private class XAxisConverter : AxisConverter {

      /// <summary>Initializes a new X axis converter</summary>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public XAxisConverter(int min, int max) : base(min, max) { }

      /// <summary>Retrieves the current value of the axis</summary>
      /// <param name="gamePadState">Game pad state that will receive the value</param>
      /// <param name="joystickState">State from which the axis is retrieved</param>
      public void Convert(
        FlatGamePadState gamePadState, ref JoystickState joystickState
      ) {
        if (joystickState.X < base.Center) {
          gamePadState.LeftThumbStick.X = (float)(base.Center - joystickState.X) / base.Min;
        } else {
          gamePadState.LeftThumbStick.X = (float)(joystickState.X - base.Center) / base.Max;
        }
      }

    }

    #endregion // class XAxisGetter

    #region class YAxisConverter

    /// <summary>Converts the value of the joystick's Y axis</summary>
    private class YAxisConverter : AxisConverter {

      /// <summary>Initializes a new Y axis converter</summary>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public YAxisConverter(int min, int max) : base(min, max) { }

      /// <summary>Retrieves the current value of the axis</summary>
      /// <param name="gamePadState">Game pad state that will receive the value</param>
      /// <param name="joystickState">State from which the axis is retrieved</param>
      public void Convert(
        FlatGamePadState gamePadState, ref JoystickState joystickState
      ) {
        if (joystickState.X < 0) {
          gamePadState.LeftThumbStick.Y = (float)(base.Center - joystickState.Y) / base.Min;
        } else {
          gamePadState.LeftThumbStick.Y = (float)(joystickState.Y - base.Center) / base.Max;
        }
      }

    }

    #endregion // class YAxisConverter

    #region class RotationalXAxisConverter

    /// <summary>Converts the value of the joystick's rotational X axis</summary>
    private class RotationalXAxisConverter : AxisConverter {

      /// <summary>Initializes a new rotational X axis converter</summary>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public RotationalXAxisConverter(int min, int max) : base(min, max) { }

      /// <summary>Retrieves the current value of the axis</summary>
      /// <param name="gamePadState">Game pad state that will receive the value</param>
      /// <param name="joystickState">State from which the axis is retrieved</param>
      public void Convert(
        FlatGamePadState gamePadState, ref JoystickState joystickState
      ) {
        if (joystickState.RotationX < 0) {
          gamePadState.RightThumbStick.X =
            (float)(base.Center - joystickState.RotationX) / base.Min;
        } else {
          gamePadState.RightThumbStick.X =
            (float)(joystickState.RotationX - base.Center) / base.Max;
        }
      }

    }

    #endregion // class RotationalXAxisConverter

    #region class RotationalYAxisConverter

    /// <summary>Converts the value of the joystick's rotational Y axis</summary>
    private class RotationalYAxisConverter : AxisConverter {

      /// <summary>Initializes a new rotational Y axis converter</summary>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public RotationalYAxisConverter(int min, int max) : base(min, max) { }

      /// <summary>Retrieves the current value of the axis</summary>
      /// <param name="gamePadState">Game pad state that will receive the value</param>
      /// <param name="joystickState">State from which the axis is retrieved</param>
      public void Convert(
        FlatGamePadState gamePadState, ref JoystickState joystickState
      ) {
        if (joystickState.RotationY < 0) {
          gamePadState.RightThumbStick.Y =
            (float)(base.Center - joystickState.RotationY) / base.Min;
        } else {
          gamePadState.RightThumbStick.Y =
            (float)(joystickState.RotationY - base.Center) / base.Max;
        }
      }

    }

    #endregion // class RotationalYAxisConverter

    #region class RotationalZAxisLeftTriggerConverter

    /// <summary>Converts the value of the joystick's rotational Z axis</summary>
    private class RotationalZAxisLeftTriggerConverter : AxisConverter {

      /// <summary>Initializes a new rotational Z axis converter</summary>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public RotationalZAxisLeftTriggerConverter(int min, int max) : base(min, max) { }

      /// <summary>Retrieves the current value of the axis</summary>
      /// <param name="gamePadState">Game pad state that will receive the value</param>
      /// <param name="joystickState">State from which the axis is retrieved</param>
      public void Convert(
        FlatGamePadState gamePadState, ref JoystickState joystickState
      ) {
        if (joystickState.RotationZ < 0) {
          gamePadState.LeftTrigger =
            (float)(base.Center - joystickState.RotationZ) / base.Min;
        } else {
          gamePadState.LeftTrigger =
            (float)(joystickState.RotationZ - base.Center) / base.Max;
        }
      }

    }

    #endregion // class RotationZAxisLeftTriggerConverter

    #region class SliderLeftTriggerConverter

    /// <summary>Converts the value of the joystick's slider axis</summary>
    private class SliderLeftTriggerConverter {

      /// <summary>Initializes a new rotational Y axis converter</summary>
      /// <param name="sliderIndex">Index of the slider that will be converted</param>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public SliderLeftTriggerConverter(int sliderIndex, int min, int max) {
        this.sliderIndex = sliderIndex;
        this.min = min;
        this.max = (float)(max - min);
      }

      /// <summary>Retrieves the current value of the axis</summary>
      /// <param name="gamePadState">Game pad state that will receive the value</param>
      /// <param name="joystickState">State from which the axis is retrieved</param>
      public void Convert(
        FlatGamePadState gamePadState, ref JoystickState joystickState
      ) {
        int slider = joystickState.GetSliders()[this.sliderIndex];
        gamePadState.LeftTrigger = (float)(slider - this.min) / this.max;
      }

      /// <summary>Index of the slider that will be converted</summary>
      private int sliderIndex;
      /// <summary>Negative range of the axis</summary>
      private int min;
      /// <summary>Positive range of the axis</summary>
      private float max;

    }

    #endregion // class SliderLeftTriggerConverter

    #region class SliderRightTriggerConverter

    /// <summary>Converts the value of the joystick's slider axis</summary>
    private class SliderRightTriggerConverter {

      /// <summary>Initializes a new rotational Y axis converter</summary>
      /// <param name="sliderIndex">Index of the slider that will be converted</param>
      /// <param name="min">Negative range of the axis</param>
      /// <param name="max">Positive range of the axis</param>
      public SliderRightTriggerConverter(int sliderIndex, int min, int max) {
        this.sliderIndex = sliderIndex;
        this.min = min;
        this.max = (float)(max - min);
      }

      /// <summary>Retrieves the current value of the axis</summary>
      /// <param name="gamePadState">Game pad state that will receive the value</param>
      /// <param name="joystickState">State from which the axis is retrieved</param>
      public void Convert(
        FlatGamePadState gamePadState, ref JoystickState joystickState
      ) {
        int slider = joystickState.GetSliders()[this.sliderIndex];
        gamePadState.RightTrigger = (float)(slider - this.min) / this.max;
      }

      /// <summary>Index of the slider that will be converted</summary>
      private int sliderIndex;
      /// <summary>Negative range of the axis</summary>
      private int min;
      /// <summary>Positive range of the axis</summary>
      private float max;

    }

    #endregion // class SliderRightTriggerConverter

  }

} // namespace Nuclex.Input.Devices

#endif // !NO_DIRECTINPUT