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

  /// <summary>Provides comparison operations on the given type</summary>
  /// <typeparam name="ScalarType">
  ///   Type for which comparison operations are provided
  /// </typeparam>
  public interface IComparisonProvider<ScalarType> {

    /// <summary>Checks whether two values are equal</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the two value are equal</returns>
    bool Equal(ScalarType first, ScalarType second);

    /// <summary>Checks whether the first value is greater than the second value</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the first value is greater than the second one</returns>
    bool GreaterThan(ScalarType first, ScalarType second);

    /// <summary>Checks whether the first value is less than the second value</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the first value is less than the second one</returns>
    bool LessThan(ScalarType first, ScalarType second);

    /// <summary>Checks whether the scalar is a positive value</summary>
    /// <param name="value">Value that will be checked for positiveness</param>
    /// <returns>True if the value is positive</returns>
    bool IsPositive(ScalarType value);

    /// <summary>Checks whether the scalar is a negative value</summary>
    /// <param name="value">Value that will be checked for negativeness</param>
    /// <returns>True if the value is positive</returns>
    bool IsNegative(ScalarType value);

  }

} // namespace Nuclex.Math.Generic
