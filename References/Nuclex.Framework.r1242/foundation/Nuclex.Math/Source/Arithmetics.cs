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

namespace Nuclex.Math {

  /// <summary>Arithmetical auxiliary functions</summary>
  public static class Arithmetics {

    /// <summary>Determines the nearest multiple of a number to another number</summary>
    /// <param name="value">Number to which to determine the nearest multiple</param>
    /// <param name="factor">Factors whose nearest multiple will be returned</param>
    /// <returns>The nearest multiple of the specified factor</returns>
    public static double NearestMultiple(double value, double factor) {
      return factor * System.Math.Round(value / factor);
    }

    /// <summary>Checks whether a double is within a specific range of another double</summary>
    /// <param name="value">Comparison value</param>
    /// <param name="other">Value to compare to</param>
    /// <param name="range">Allowed range within the value is allowed to be</param>
    /// <returns>True if both values are within the specified range of each other</returns>
    public static bool InRange(double value, double other, double range) {
      return (value > other - range) && (value < other + range);
    }

  }

} // namespace Nuclex.Math
