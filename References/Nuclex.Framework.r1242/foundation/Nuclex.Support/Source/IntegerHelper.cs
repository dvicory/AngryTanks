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

namespace Nuclex.Support {

  /// <summary>Helper methods for working with integer types</summary>
  public static class IntegerHelper {

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static long NextPowerOf2(long value) {
      return (long)NextPowerOf2((ulong)value);
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static ulong NextPowerOf2(ulong value) {
      if(value == 0)
        return 1;

      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      value |= value >> 32;
      ++value;

      return value;
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static int NextPowerOf2(int value) {
      return (int)NextPowerOf2((uint)value);
    }

    /// <summary>Returns the next highest power of 2 from the specified value</summary>
    /// <param name="value">Value of which to return the next highest power of 2</param>
    /// <returns>The next highest power of 2 to the value</returns>
    public static uint NextPowerOf2(uint value) {
      if(value == 0)
        return 1;

      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      ++value;

      return value;
    }

  }

} // namespace Nuclex.Support
