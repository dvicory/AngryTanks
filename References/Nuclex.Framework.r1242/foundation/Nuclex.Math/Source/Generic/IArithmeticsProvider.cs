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

  /// <summary>Provides arithmetical operations</summary>
  /// <typeparam name="ScalarType">
  ///   The data type for which to provide arithmetical operations
  /// </typeparam>
  public interface IArithmeticOperationsProvider<ScalarType> {

    /// <summary>Returns the absolute value of a number</summary>
    /// <param name="number">Number to return the absolute value of</param>
    /// <returns>The absolute value of the number</returns>
    ScalarType Abs(ScalarType number);

    /// <summary>Returns the inverse value of a number</summary>
    /// <param name="number">Number to return the inverse value of</param>
    /// <returns>The inverse value of the number</returns>
    ScalarType Inv(ScalarType number);

    /// <summary>Negates a number</summary>
    /// <param name="number">Number to negate</param>
    /// <returns>The negated number</returns>
    ScalarType Negate(ScalarType number);

    /// <summary>Forms the sum of two numbers</summary>
    /// <param name="first">First number to be summed</param>
    /// <param name="second">Second number to be summed</param>
    /// <returns>The sum of both numbers</returns>
    ScalarType Add(ScalarType first, ScalarType second);

    /// <summary>Subtracts one number from the other</summary>
    /// <param name="number">Base from which to subtract</param>
    /// <param name="subtrahend">Number to be subtracted</param>
    /// <returns>The result of the subtraction</returns>
    ScalarType Subtract(ScalarType number, ScalarType subtrahend);

    /// <summary>The product of both numbers</summary>
    /// <param name="number">First factor of the multiplication</param>
    /// <param name="factor">Second factor of the multiplication</param>
    /// <returns>The product of the multiplication of both factors</returns>
    ScalarType Multiply(ScalarType number, ScalarType factor);

    /// <summary>Divides the number by another one</summary>
    /// <param name="number">Number to be divided</param>
    /// <param name="divisor">Divisor</param>
    /// <returns>The quotient of the number divided by the provided divisor</returns>
    ScalarType Divide(ScalarType number, ScalarType divisor);

  }

} // namespace Nuclex.Math.Generic
