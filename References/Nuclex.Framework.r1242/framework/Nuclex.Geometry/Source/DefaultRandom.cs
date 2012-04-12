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

  /// <summary>
  ///   Default random number generator wrapping the built-in .NET one
  /// </summary>
  public class DefaultRandom : IRandom {

    /// <summary>
    ///   Initializes a new random number generator, using a time-dependent seed value
    /// </summary>
    public DefaultRandom() {
      this.random = new Random();
    }

    /// <summary>
    ///   Initializes a new random number generator, using the specified seed value
    /// </summary>
    /// <param name="seed">
    ///   A number used to calculate a starting value for the pseudo-random number
    ///   sequence. If a negative number is specified, the absolute value of
    ///   the number is used.
    /// </param>
    /// <exception cref="System.OverflowException">
    ///   Seed is System.Int32.MinValue, which causes an overflow when its absolute
    ///   value is calculated
    /// </exception>
    public DefaultRandom(int seed) {
      this.random = new Random(seed);
    }

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
    public int Next(int maximumValue) {
      return this.random.Next(maximumValue);
    }

    /// <summary>Returns a random number between 0.0 and 1.0</summary>
    /// <returns>
    ///   A double-precision floating point number greater than or equal to 0.0,
    ///   and less than 1.0
    /// </returns>
    public double NextDouble() {
      return this.random.NextDouble();
    }

    /// <summary>The .NET random number generator being wrapped</summary>
    private Random random;

  }

} // namespace Nuclex.Geometry
