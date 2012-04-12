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

  /// <summary>Provides logarithmical operations for generic numbers</summary>
  /// <typeparam name="ScalarType">
  ///   Type for which logarithmical operations are provided
  /// </typeparam>
  public interface ILogarithmicsProvider<ScalarType> {

    /// <summary>Calculates the mathematical power of a value</summary>
    /// <param name="number">Value to calculate the power of</param>
    /// <param name="power">Power to which to take the value</param>
    /// <returns>The resulting value</returns>
    ScalarType Pow(ScalarType number, ScalarType power);

    /// <summary>Calculates the square root of a number</summary>
    /// <param name="number">Value of which to calculate the square root</param>
    /// <returns>The square root of the input value</returns>
    ScalarType Sqrt(ScalarType number);

    /// <summary>Calculates the logarithm of a number</summary>
    /// <param name="number">Value of which to calculate the logarithm</param>
    /// <returns>The logarithm of the input value</returns>
    ScalarType Log(ScalarType number);

    /// <summary>Calculates the exponent of a number</summary>
    /// <param name="number">Value of which to calculate the exponent</param>
    /// <returns>The exponent of the input value</returns>
    ScalarType Exp(ScalarType number);

  }

} // namespace Nuclex.Math.Generic
