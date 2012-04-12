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

  /// <summary>Binary math routines</summary>
  /// <typeparam name="ScalarType">
  ///   Data type for which to provide binary operations
  /// </typeparam>
  public interface IBitOperationsProvider<ScalarType> {

    /// <summary>Binarily ANDs the bits of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The result of the binary ANDing</returns>
    ScalarType And(ScalarType first, ScalarType second);

    /// <summary>Binarily ORs the bits of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The result of the binary ORing</returns>
    ScalarType Or(ScalarType first, ScalarType second);

    /// <summary>Binarily exclusive ORs the bits of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The result of the binary exclusive ORing</returns>
    ScalarType Xor(ScalarType first, ScalarType second);

    /// <summary>Binarily negates the bits of a number</summary>
    /// <param name="value">Value to be negated</param>
    /// <returns>The result of the binary negation</returns>
    ScalarType Not(ScalarType value);

  }

} // namespace Nuclex.Math.Generic
