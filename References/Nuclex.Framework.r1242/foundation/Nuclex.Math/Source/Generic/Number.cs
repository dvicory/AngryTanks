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

using System.Diagnostics;

using Nuclex.Support;

namespace Nuclex.Math.Generic {

  /// <summary>Provides a generic data type with implicit operators</summary>
  /// <typeparam name="NumericalType">Generic numerical data type</typeparam>
  /// <typeparam name="MathType">Set of math routines to be used on it</typeparam>
  public struct Number<NumericalType, MathType>
    where MathType :
      IArithmeticOperationsProvider<NumericalType>,
      IComparisonProvider<NumericalType>,
      new() {

    /// <summary>Initializes the wrapped number to the direct generic number</summary>
    /// <param name="number">The directly given generic number</param>
    [DebuggerStepThrough]
    public Number(NumericalType number) {
      this.value = number;
    }

    /// <summary>Implicit conversion from a direct number to this wrapper</summary>
    /// <param name="number">Direct generic number to be wrapped</param>
    /// <returns>The wrapped generic number</returns>
    [DebuggerStepThrough]
    public static implicit operator Number<NumericalType, MathType>(NumericalType number) {
      return new Number<NumericalType, MathType>(number);
    }

    /// <summary>Implicit conversion from this wrapper to a direct number</summary>
    /// <param name="number">Wrapped to be unwrapped</param>
    /// <returns>The unwrapped direct generic number</returns>
    [DebuggerStepThrough]
    public static implicit operator NumericalType(Number<NumericalType, MathType> number) {
      return number.value;
    }

    /// <summary>Negates the number</summary>
    /// <param name="number">Number to be negated</param>
    /// <returns>The negated number</returns>
    [DebuggerStepThrough]
    public static Number<NumericalType, MathType> operator -(
      Number<NumericalType, MathType> number
    ) {
      return math.Negate(number);
    }

    /// <summary>Forms the sum of two numbers</summary>
    /// <param name="first">First number to sum</param>
    /// <param name="second">Second number to sum</param>
    /// <returns>The sum of both numbers</returns>
    [DebuggerStepThrough]
    public static Number<NumericalType, MathType> operator +(
      Number<NumericalType, MathType> first, Number<NumericalType, MathType> second
    ) {
      return math.Add(first, second);
    }

    /// <summary>Subtracts one number from the other</summary>
    /// <param name="number">Base number from which to subtract</param>
    /// <param name="subtrahend">Number to be subtracted</param>
    /// <returns>The result of the subtraction</returns>
    [DebuggerStepThrough]
    public static Number<NumericalType, MathType> operator -(
      Number<NumericalType, MathType> number, Number<NumericalType, MathType> subtrahend
    ) {
      return math.Subtract(number, subtrahend);
    }

    /// <summary>Forms the product of two numebrs</summary>
    /// <param name="number">First multiplication factor</param>
    /// <param name="factor">Second multiplication factor</param>
    /// <returns>The product of the multiplication of both vectors</returns>
    [DebuggerStepThrough]
    public static Number<NumericalType, MathType> operator *(
      Number<NumericalType, MathType> number, Number<NumericalType, MathType> factor
    ) {
      return math.Multiply(number, factor);
    }

    /// <summary>Divides one number by another one</summary>
    /// <param name="number">Number to be divided</param>
    /// <param name="divisor">Divisor by which to divide</param>
    /// <returns>The quotient of the number and the divisor</returns>
    [DebuggerStepThrough]
    public static Number<NumericalType, MathType> operator /(
      Number<NumericalType, MathType> number, Number<NumericalType, MathType> divisor
    ) {
      return math.Divide(number, divisor);
    }

    /// <summary>Checks if two numbers are equal</summary>
    /// <param name="a">First number to be compared</param>
    /// <param name="b">Second number to be compared</param>
    /// <returns>True if both numbers are equal</returns>
    [DebuggerStepThrough]
    public static bool operator ==(
      Number<NumericalType, MathType> a, Number<NumericalType, MathType> b
    ) {
      return math.Equal(a, b);
    }

    /// <summary>Check if two numbers are unequal</summary>
    /// <param name="a">First number to be compared</param>
    /// <param name="b">Second number to be compared</param>
    /// <returns>True if both numbers are unequal</returns>
    [DebuggerStepThrough]
    public static bool operator !=(
      Number<NumericalType, MathType> a, Number<NumericalType, MathType> b
    ) {
      return !math.Equal(a, b);
    }

    /// <summary>Checks if another object is identical to this object</summary>
    /// <param name="a">Other object to compare this instance to</param>
    /// <returns>True if both the other object is identical to this one</returns>
    public override bool Equals(object a) {
      if(a is NumericalType)
        return a.Equals((NumericalType)a);
      else
        return false;
    }

    /// <summary>Tests whether the first value is greater than the second value</summary>
    /// <param name="a">First value to compare</param>
    /// <param name="b">Second value to compare</param>
    /// <returns>True if the first value is greater than the second value</returns>
    public static bool operator >(
      Number<NumericalType, MathType> a, Number<NumericalType, MathType> b
    ) {
      return math.GreaterThan(a, b);
    }

    /// <summary>Tests whether the first value is less than the second value</summary>
    /// <param name="a">First value to compare</param>
    /// <param name="b">Second value to compare</param>
    /// <returns>True if the first value is less than the second value</returns>
    public static bool operator <(
      Number<NumericalType, MathType> a, Number<NumericalType, MathType> b
    ) {
      return math.LessThan(a, b);
    }

    /// <summary>Converts the wrapped generic number into a string</summary>
    /// <returns>The string representation of the generic number</returns>
    public override string ToString() {
      return value.ToString();
    }

    /// <summary>Provides a hashing value for the instance</summary>
    /// <returns>The hashing value for this instance</returns>
    public override int GetHashCode() {
      return value.GetHashCode();
    }

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

    /// <summary>The direct generic value that is wrapped by this instance</summary>
    private readonly NumericalType value;

  }

} // namespace Nuclex.Math.Generic
