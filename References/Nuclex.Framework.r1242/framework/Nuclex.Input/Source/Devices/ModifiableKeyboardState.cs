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
using System.Reflection;

using Microsoft.Xna.Framework.Input;

#if !XBOX360

namespace Nuclex.Input.Devices {

  /// <summary>An XNA KeyboardState that can be modified</summary>
  internal class ModifiableKeyboardState {

    /// <summary>Initializes the static fields of the class</summary>
    static ModifiableKeyboardState() {
      addPressedKeyMethod = typeof(KeyboardState).GetMethod(
        "AddPressedKey", BindingFlags.Instance | BindingFlags.NonPublic
      );
      removePressedKeyMethod = typeof(KeyboardState).GetMethod(
        "RemovePressedKey", BindingFlags.Instance | BindingFlags.NonPublic
      );

      // Set up a parameter array to avoid dynamic boxing of reflection
      // parameters which would cause a lot of garbage to build up
      boxedKeys = new object[255][];
      for (int key = (int)Keys.None; key <= (int)Keys.OemClear; ++key) {
        boxedKeys[key] = new object[] { key };
      }
    }

    /// <summary>Initializes a new modifiable keyboard state</summary>
    public ModifiableKeyboardState() : this(new KeyboardState()) { }

    /// <summary>Builds a modifiable keyboard state from the provided state</summary>
    /// <param name="state">State that will become the initial state</param>
    public ModifiableKeyboardState(KeyboardState state) {
      this.boxedState = state;
    }

    /// <summary>Adds a pressed key to the keyboard state</summary>
    /// <param name="key">Key that will be registered as pressed</param>
    public void AddPressedKey(int key) {
      addPressedKeyMethod.Invoke(boxedState, boxedKeys[key]);
    }

    /// <summary>Removes a pressed key to the keyboard state</summary>
    /// <param name="key">Key that will be registered as released</param>
    public void RemovePressedKey(int key) {
      removePressedKeyMethod.Invoke(boxedState, boxedKeys[key]);
    }

    /// <summary>Converts a modifiable keyboard state into a keyboard state</summary>
    /// <param name="state">The modifiable keyboard state that will be converted</param>
    /// <returns>The equivalent keyboard state</returns>
    public static implicit operator KeyboardState(ModifiableKeyboardState state) {
      return (KeyboardState)state.boxedState;
    }

    /// <summary>Reflection info for the KeyboardState.AddPressedKey() method</summary>
    private static readonly MethodInfo addPressedKeyMethod;

    /// <summary>Reflection info for the KeyboardState.RemovePressedKey() method</summary>
    private static readonly MethodInfo removePressedKeyMethod;

    /// <summary>Contains prepared parameter arrays for all valid keys</summary>
    private static readonly object[][] boxedKeys;

    /// <summary>The KeyboardState instance being modified</summary>
    private object boxedState;

  }

} // namespace Nuclex.Input.Devices

#endif // !XBOX360
