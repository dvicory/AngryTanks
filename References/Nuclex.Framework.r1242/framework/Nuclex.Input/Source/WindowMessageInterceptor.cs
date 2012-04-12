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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Input;

using Nuclex.Input.Devices;

using NativeWindow = System.Windows.Forms.NativeWindow;
using Message = System.Windows.Forms.Message;

namespace Nuclex.Input {

  /// <summary>Intercepts input-related messages for another window</summary>
  internal class WindowMessageInterceptor :
    NativeWindow, IKeyboardMessageSource, IMouseMessageSource, IDisposable {

    /// <summary>Flags that will be added to the result of WM_GETDLGCODE</summary>
    private const int DlgCodeFlags =
      (UnsafeNativeMethods.DLGC_WANTALLKEYS | UnsafeNativeMethods.DLGC_WANTCHARS);

    /// <summary>Triggered when a key has been pressed down</summary>
    public event KeyboardKeyEventDelegate KeyPressed;

    /// <summary>Triggered when a key has been released again</summary>
    public event KeyboardKeyEventDelegate KeyReleased;

    /// <summary>Triggered when the user has entered a character</summary>
    public event KeyboardCharacterEventDelegate CharacterEntered;

    /// <summary>Triggered when a mouse button has been pressed</summary>
    public event MouseButtonEventDelegate MouseButtonPressed;

    /// <summary>Triggered when a mouse button has been released</summary>
    public event MouseButtonEventDelegate MouseButtonReleased;

    /// <summary>Triggered when the mouse has been moved</summary>
    public event MouseMoveEventDelegate MouseMoved;

    /// <summary>Triggered when the mouse wheel has been rotated</summary>
    public event MouseWheelEventDelegate MouseWheelRotated;

    /// <summary>Initializes a new window message interceptor</summary>
    /// <param name="windowHandle">
    ///   Handle of the window for which messages will be intercepted
    /// </param>
    public WindowMessageInterceptor(IntPtr windowHandle) {

      // Set up the information structure for TrackMouseEvent()
      this.mouseEventTrackData = new UnsafeNativeMethods.TRACKMOUSEEVENT();
      this.mouseEventTrackData.structureSize = Marshal.SizeOf(this.mouseEventTrackData);
      this.mouseEventTrackData.flags = UnsafeNativeMethods.TME_LEAVE;
      this.mouseEventTrackData.trackWindowHandle = windowHandle;

      // Attach the input capturer to the window
      AssignHandle(windowHandle);

#if USE_WM_INPUT
      // Register this window for the WM_INPUT message in order to receive
      // high definition mouse input
      registerForRawInput(windowHandle);
#endif
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {

      // If this isn't a repetitive call to Dispose(), disconnect the NativeWindow instance
      // from the XNA GameWindow handle
      if (!this.disposed) {
        ReleaseHandle(); // Do not destroy the window because we don't own it!
        this.disposed = true;
      }

      // The NativeWindow class does some processing in its finalizer which I think
      // might not anticipate the finalizer being completely avoided. So to be on the
      // safe side, we let its finalizer run even when we have been disposed!
      // GC.SuppressFinalize(this);

    }

    /// <summary>
    ///   Overridden window message callback used to capture input for the window
    /// </summary>
    /// <param name="message">Window message sent to the window</param>
    protected override void WndProc(ref Message message) {
      base.WndProc(ref message);

      // Process the message differently based on its message id
      switch (message.Msg) {

        // Window is being asked which types of input it can process
        case (int)UnsafeNativeMethods.WindowMessages.WM_GETDLGCODE: {
          int returnCode = message.Result.ToInt32();
          returnCode |= DlgCodeFlags;
          message.Result = new IntPtr(returnCode);
          break;
        }
#if USE_WM_INPUT
        // Raw input data is being sent to the window
        case (int)UnsafeNativeMethods.WindowMessages.WM_INPUT: {
          processWmInput(ref message);
          break;
        }
#endif
        // Key on the keyboard was pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_KEYDOWN: {
          int virtualKeyCode = message.WParam.ToInt32();
          // bool repetition = (message.LParam.ToInt32() & WM_KEYDOWN_WASDOWN) != 0;
          OnKeyPressed((Keys)virtualKeyCode);
          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_KEYUP: {
          int virtualKeyCode = message.WParam.ToInt32();
          OnKeyReleased((Keys)virtualKeyCode);
          break;
        }

        // Character has been entered on the keyboard
        case (int)UnsafeNativeMethods.WindowMessages.WM_CHAR: {
          char character = (char)message.WParam.ToInt32();
          OnCharacterEntered(character);
          break;
        }

        // Mouse has been moved
        case (int)UnsafeNativeMethods.WindowMessages.WM_MOUSEMOVE: {
          if (!this.trackingMouse) {
            int result = UnsafeNativeMethods.TrackMouseEvent(ref this.mouseEventTrackData);
            Debug.Assert(
              result != 0,
              "Could not set up registration for mouse events",
              "The TrackMouseEvent() function failed, which means the game will not " +
              "detect when the mouse leaves the game window. This might result in " +
              "the assumed mouse position remaining somewhere near the window border " +
              "even though the mouse has been moved away from the game window."
            );
            this.trackingMouse = (result != 0);
          }

          short x = (short)(message.LParam.ToInt32() & 0xFFFF);
          short y = (short)(message.LParam.ToInt32() >> 16);
          OnMouseMoved((float)x, (float)y);
          break;
        }

        // Left mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONDBLCLK: {
          OnMouseButtonPressed(MouseButtons.Left);
          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONUP: {
          OnMouseButtonReleased(MouseButtons.Left);
          break;
        }

        // Right mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONDBLCLK: {
          OnMouseButtonPressed(MouseButtons.Right);
          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONUP: {
          OnMouseButtonReleased(MouseButtons.Right);
          break;
        }

        // Middle mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONDBLCLK: {
          OnMouseButtonPressed(MouseButtons.Middle);
          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONUP: {
          OnMouseButtonReleased(MouseButtons.Middle);
          break;
        }

        // Extended mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONDBLCLK: {
          short button = (short)(message.WParam.ToInt32() >> 16);
          if (button == 1)
            OnMouseButtonPressed(MouseButtons.X1);
          if (button == 2)
            OnMouseButtonPressed(MouseButtons.X2);

          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONUP: {
          short button = (short)(message.WParam.ToInt32() >> 16);
          if (button == 1)
            OnMouseButtonReleased(MouseButtons.X1);
          if (button == 2)
            OnMouseButtonReleased(MouseButtons.X2);

          break;
        }

        // Mouse wheel rotated
        case (int)UnsafeNativeMethods.WindowMessages.WM_MOUSEHWHEEL: {
          short ticks = (short)(message.WParam.ToInt32() >> 16);
          OnMouseWheelRotated((float)ticks / 120.0f);
          break;
        }

        // Mouse has left the window's client area
        case (int)UnsafeNativeMethods.WindowMessages.WM_MOUSELEAVE: {
          OnMouseMoved(-1.0f, -1.0f);
          this.trackingMouse = false;
          break;
        }

      }

    }

    /// <summary>Fires the KeyPressed event</summary>
    /// <param name="key">Key that has been pressed</param>
    protected void OnKeyPressed(Keys key) {
      if (KeyPressed != null) {
        KeyPressed(key);
      }
    }

    /// <summary>Fires the KeyReleased event</summary>
    /// <param name="key">Key that has been released</param>
    protected void OnKeyReleased(Keys key) {
      if (KeyReleased != null) {
        KeyReleased(key);
      }
    }

    /// <summary>Fires the CharacterEntered event</summary>
    /// <param name="character">Character that has been entered</param>
    protected void OnCharacterEntered(char character) {
      if (CharacterEntered != null) {
        CharacterEntered(character);
      }
    }

    /// <summary>Fires the MouseButtonPressed event</summary>
    /// <param name="buttons">Mouse buttons that have been pressed</param>
    protected void OnMouseButtonPressed(MouseButtons buttons) {
      if (MouseButtonPressed != null) {
        MouseButtonPressed(buttons);
      }
    }

    /// <summary>Fires the MouseButtonReleased event</summary>
    /// <param name="buttons">Mouse buttons that have been released</param>
    protected void OnMouseButtonReleased(MouseButtons buttons) {
      if (MouseButtonReleased != null) {
        MouseButtonReleased(buttons);
      }
    }

    /// <summary>Fires the MouseMoved event</summary>
    /// <param name="x">New X coordinate of the mouse</param>
    /// <param name="y">New Y coordinate of the mouse</param>
    protected void OnMouseMoved(float x, float y) {
      if (MouseMoved != null) {
        MouseMoved(x, y);
      }
    }

    /// <summary>Fires the MouseWheelRotated event</summary>
    /// <param name="ticks">Number of ticks the mouse wheel has been rotated</param>
    protected void OnMouseWheelRotated(float ticks) {
      if (MouseWheelRotated != null) {
        MouseWheelRotated(ticks);
      }
    }

#if USE_WM_INPUT

    /// <summary>Reports high definition mouse movement</summary>
    /// <param name="x">Relative movement on the X axis</param>
    /// <param name="y">Relative movement on the Y axis</param>
    protected void OnMouseMoveHiDef(float x, float y) { }

    /// <summary>
    ///   Registers the window to receive high definition mouse input through WM_INPUT
    /// </summary>
    /// <param name="windowHandle">Handle of the window that will receive WM_INPUT</param>
    private void registerForRawInput(IntPtr windowHandle) {
      UnsafeNativeMethods.RAWINPUTDEVICE rawInputDevice;

      rawInputDevice.UsagePage = UnsafeNativeMethods.HID_USAGE_PAGE_GENERIC;
      rawInputDevice.Usage = UnsafeNativeMethods.HID_USAGE_GENERIC_MOUSE;
      rawInputDevice.Flags = 0;
      rawInputDevice.WindowHandle = windowHandle;

      bool result = UnsafeNativeMethods.RegisterRawInputDevices(
        ref rawInputDevice,
        1,
        Marshal.SizeOf(rawInputDevice)
      );
      if (!result) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }

    /// <summary>Processes a WM_INPUT message</summary>
    /// <param name="message">WM_INPUT message that will be processed</param>
    private void processWmInput(ref Message message) {

      // Retrieve the raw input data sent to the window
      int size = sizeOfRawInput;
      int result = UnsafeNativeMethods.GetRawInputData(
        message.LParam,
        UnsafeNativeMethods.RID_INPUT,
        out this.rawInput,
        ref size,
        sizeOfRawInputHeader
      );
      if (result < 0) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      // Only process data sent to the mouse
      if (this.rawInput.Header.Type == UnsafeNativeMethods.RIM_TYPEMOUSE) {
        bool isRelativeMouseMovement =
          (this.rawInput.Mouse.Flags & UnsafeNativeMethods.MOUSE_MOVE_RELATIVE) != 0;

        if (isRelativeMouseMovement) {
          int x = this.rawInput.Mouse.LastX;
          int y = this.rawInput.Mouse.LastY;
          OnMouseMoveHiDef(x, y);
        } else {
          int x = this.previousMouseX - this.rawInput.Mouse.LastX;
          int y = this.previousMouseY - this.rawInput.Mouse.LastY;
          this.previousMouseX = this.rawInput.Mouse.LastX;
          this.previousMouseY = this.rawInput.Mouse.LastY;
          OnMouseMoveHiDef(x, y);
        }
      }
    }

    /// <summary>Cached size of the RAWINPUT structure</summary>
    private static readonly int sizeOfRawInput = Marshal.SizeOf(
      typeof(UnsafeNativeMethods.RAWINPUT)
    );

    /// <summary>Cached size of the RAWINPUTHEADER structure</summary>
    private static readonly int sizeOfRawInputHeader = Marshal.SizeOf(
      typeof(UnsafeNativeMethods.RAWINPUTHEADER)
    );

    /// <summary>Stores raw input data for the WM_INPUT message</summary>
    private UnsafeNativeMethods.RAWINPUT rawInput;

    /// <summary>Reported absolute mouse X coordinate from WM_INPUT</summary>
    private int previousMouseX;
    /// <summary>Reported absolute mouse Y coordinate from WM_INPUT</summary>
    private int previousMouseY;

#endif // USE_WM_INPUT

    /// <summary>
    ///   Provides Informations about how the mouse cusor should be tracked on
    ///   the window to the TrackMouseEvent() function.
    /// </summary>
    private UnsafeNativeMethods.TRACKMOUSEEVENT mouseEventTrackData;

    /// <summary>True when the mouse cursor is currently being tracked</summary>
    private bool trackingMouse;

    /// <summary>True when the object has been disposed</summary>
    private bool disposed;

  }

} // namespace Nuclex.Input

#endif // !NO_DIRECTINPUT
