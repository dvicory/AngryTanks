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

namespace Nuclex.Geometry {

  /// <summary>Interface for a random number generator</summary>
  public interface IRandom {

    /// <summary>
    ///   Returns a nonnegative random number less than the specified maximum
    /// </summary>
    /// <param name="maximumValue">
    ///   The exclusive upper bound of the random number to be generated. maxValue must
    ///   be greater than or equal to zero
    /// </param>
    /// <returns>
    ///   A 32-bit signed integer greater than or equal to zero, and less than maxValue;
    ///   that is, the range of return values ordinarily includes zero but not maxValue.
    ///   However, if maxValue equals zero, maxValue is returned
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   maximumValue is less than zero
    /// </exception>
    int Next(int maximumValue);

    /// <summary>Returns a random number between 0.0 and 1.0</summary>
    /// <returns>
    ///   A double-precision floating point number greater than or equal to 0.0,
    ///   and less than 1.0
    /// </returns>
    double NextDouble();

  }

} // namespace Nuclex.Geometry
