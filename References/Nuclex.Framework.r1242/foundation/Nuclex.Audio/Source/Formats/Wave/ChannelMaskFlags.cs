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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Nuclex.Audio.Formats.Wave {

  /// <summary>Valid flags for the channels provided flags</summary>
  [Flags]
  public enum ChannelMaskFlags : uint {
    /// <summary>Channel for the front left speaker has been provided</summary>
    SPEAKER_FRONT_LEFT = 0x1,
    /// <summary>Channel for the front right speaker has been provided</summary>
    SPEAKER_FRONT_RIGHT = 0x2,
    /// <summary>Channel for the front center has been provided</summary>
    SPEAKER_FRONT_CENTER = 0x4,
    /// <summary>Channel for a low frequency speaker (subwoofer) has been provided</summary>
    SPEAKER_LOW_FREQUENCY = 0x8,
    /// <summary>Channel for the back left speaker has been provided</summary>
    SPEAKER_BACK_LEFT = 0x10,
    /// <summary>Channel for the back right speaker has been provided</summary>
    SPEAKER_BACK_RIGHT = 0x20,
    /// <summary>Channel for the front left of center speaker has been provided</summary>
    SPEAKER_FRONT_LEFT_OF_CENTER = 0x40,
    /// <summary>Channel for the front right of center speaker has been provided</summary>
    SPEAKER_FRONT_RIGHT_OF_CENTER = 0x80,
    /// <summary>Channel for the back center speaker has been provided</summary>
    SPEAKER_BACK_CENTER = 0x100,
    /// <summary>Channel for the side left speaker has been provided</summary>
    SPEAKER_SIDE_LEFT = 0x200,
    /// <summary>Channel for the side right speaker has been provided</summary>
    SPEAKER_SIDE_RIGHT = 0x400,
    /// <summary>Channel for the top center speaker has been provided</summary>
    SPEAKER_TOP_CENTER = 0x800,
    /// <summary>Channel for the top front left speaker has been provided</summary>
    SPEAKER_TOP_FRONT_LEFT = 0x1000,
    /// <summary>Channel for the top front center speaker has been provided</summary>
    SPEAKER_TOP_FRONT_CENTER = 0x2000,
    /// <summary>Channel for the top front right speaker has been provided</summary>
    SPEAKER_TOP_FRONT_RIGHT = 0x4000,
    /// <summary>Channel for the top back left speaker has been provided</summary>
    SPEAKER_TOP_BACK_LEFT = 0x8000,
    /// <summary>Channel for the top back center speaker has been provided</summary>
    SPEAKER_TOP_BACK_CENTER = 0x10000,
    /// <summary>Channel for the top back right speaker has been provided</summary>
    SPEAKER_TOP_BACK_RIGHT = 0x20000,
    /// <summary>Reserved for future use</summary>
    SPEAKER_RESERVED = 0x80000000
  }

} // namespace Nuclex.Audio.Formats.Wave
