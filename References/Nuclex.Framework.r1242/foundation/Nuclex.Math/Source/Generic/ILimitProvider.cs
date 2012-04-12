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

namespace Nuclex.Math.Generic {

  /// <summary>Provides methods for limiting values of the provided type</summary>
  /// <typeparam name="ScalarType">
  ///   Type for which limiting operations are provided
  /// </typeparam>
  public interface ILimitProvider<ScalarType> {

    /// <summary>Returns the smaller one of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The smaller of the two input value</returns>
    ScalarType Min(ScalarType first, ScalarType second);

    /// <summary>Returns the bigger one of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The bigger of the two input value</returns>
    ScalarType Max(ScalarType first, ScalarType second);

    /// <summary>Limits a value to specific range</summary>
    /// <param name="value">Value to be limited</param>
    /// <param name="minimum">Lower end of range (inclusive)</param>
    /// <param name="maximum">Upper end of range (exlusive)</param>
    /// <returns>The limited input value</returns>
    ScalarType Clamp(ScalarType value, ScalarType minimum, ScalarType maximum);

    /// <summary>Normalizes a value in a wraparound numeric range</summary>
    /// <param name="value">Value to be normalized</param>
    /// <param name="lower">Lower wraparound point (inclusive)</param>
    /// <param name="upper">Upper wraparound point (exclusive)</param>
    /// <returns>The normalized input value</returns>
    ScalarType Wrap(ScalarType value, ScalarType lower, ScalarType upper);

  }

} // namespace Nuclex.Math.Generic
