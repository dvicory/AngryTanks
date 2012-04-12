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
using System.Diagnostics;

namespace Nuclex.Math.Generic {

  /// <summary>Checked math operations for integer values</summary>
  public partial struct CheckedScalarMath :
    IComparisonProvider<int>,
    IDoubleConversionProvider<int>,
    IIntConversionProvider<int>,
    ILimitProvider<int>,
    ITrigonometricsProvider<int>,
    IAbsoluteValueProvider<int>,
    IScalingProvider<int>,
    IBitOperationsProvider<int>,
    IArithmeticOperationsProvider<int>,
    ILogarithmicsProvider<int> {

    //
    // IComparisonProvider implementation
    //

    /// <summary>Checks whether two values are equal</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the two value are equal</returns>
    [DebuggerStepThrough]
    public bool Equal(int first, int second) { return first == second; }
    /// <summary>Checks whether the first value is greater than the second value</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the first value is greater than the second one</returns>
    [DebuggerStepThrough]
    public bool GreaterThan(int first, int second) { return first > second; }
    /// <summary>Checks whether the first value is less than the second value</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the first value is less than the second one</returns>
    [DebuggerStepThrough]
    public bool LessThan(int first, int second) { return first < second; }
    /// <summary>Checks whether the scalar is a positive value</summary>
    /// <param name="value">Value that will be checked for positiveness</param>
    /// <returns>True if the value is positive</returns>
    [DebuggerStepThrough]
    public bool IsPositive(int value) { return value >= 0; }
    /// <summary>Checks whether the scalar is a negative value</summary>
    /// <param name="value">Value that will be checked for negativeness</param>
    /// <returns>True if the value is positive</returns>
    [DebuggerStepThrough]
    public bool IsNegative(int value) { return value < 0; }

    //
    // IDoubleConversionProvider implementation
    //

    /// <summary>Converts a double to the generic data type</summary>
    /// <param name="value">Value to convert</param>
    /// <returns>The input value as the generic data type</returns>
    [DebuggerStepThrough]
    int IDoubleConversionProvider<int>.FromDouble(double value) { return (int)value; }
    /// <summary>Converts a generic data type into a double</summary>
    /// <param name="value">Generic value to convert</param>
    /// <returns>The generic input value as a double</returns>
    [DebuggerStepThrough]
    public double ToDouble(int value) { return value; }

    //
    // IIntConversionProvider implementation
    //

    /// <summary>Converts an integer to the generic data type</summary>
    /// <param name="value">Value to convert</param>
    /// <returns>The input value as the generic data type</returns>
    [DebuggerStepThrough]
    int IIntConversionProvider<int>.FromInteger(int value) {
      unchecked { return value; }
    }
    /// <summary>Converts a generic data type into an integer</summary>
    /// <param name="value">Generic value to convert</param>
    /// <returns>The generic input value as an integer</returns>
    [DebuggerStepThrough]
    public int ToInteger(int value) {
      unchecked { return value; }
    }

    //
    // ILimitProvider implementation
    //

    /// <summary>Returns the smaller one of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The smaller of the two input value</returns>
    [DebuggerStepThrough]
    public int Min(int first, int second) { return first < second ? first : second; }
    /// <summary>Returns the bigger one of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The bigger of the two input value</returns>
    [DebuggerStepThrough]
    public int Max(int first, int second) { return first > second ? first : second; }
    /// <summary>Limits a value to specific range</summary>
    /// <param name="value">Value to be limited</param>
    /// <param name="minimum">Lower end of range (inclusive)</param>
    /// <param name="maximum">Upper end of range (exlusive)</param>
    /// <returns>The limited input value</returns>
    [DebuggerStepThrough]
    public int Clamp(int value, int minimum, int maximum) {
      if(value < minimum)
        return minimum;
      else if(value > maximum)
        return maximum;
      else
        return value;
    }
    /// <summary>Normalizes a value in a wraparound numeric range</summary>
    /// <param name="value">Value to be normalized</param>
    /// <param name="lower">Lower wraparound point (inclusive)</param>
    /// <param name="upper">Upper wraparound point (exclusive)</param>
    /// <returns>The normalized input value</returns>
    [DebuggerStepThrough]
    public int Wrap(int value, int lower, int upper) {
      if(upper <= lower)
        throw new ArithmeticException("Rotary bounds are of negative or zero size");

      int distance = upper - lower;
      int times = (value - lower) / distance;

      return value - (times * distance);
    }

    //
    // ITrigonometricOperationsProvider implementation
    //

    /// <summary>Calculates the sine of the specified angle</summary>
    /// <param name="angle">Angle whose sine is calculated</param>
    /// <returns>The sine of the provided angle</returns>
    [DebuggerStepThrough]
    int ITrigonometricsProvider<int>.Sin(double angle) {
      return (int)System.Math.Sin(angle);
    }
    /// <summary>Calculates the cosine of the specified angle</summary>
    /// <param name="angle">Angle whose cosine is calculated</param>
    /// <returns>The cosine of the provided angle</returns>
    [DebuggerStepThrough]
    int ITrigonometricsProvider<int>.Cos(double angle) {
      return (int)System.Math.Cos(angle);
    }
    /// <summary>Calculates the arcus sine of the specified angle</summary>
    /// <param name="angle">Angle whose arcus sine is calculated</param>
    /// <returns>The arcus sine of the provided angle</returns>
    [DebuggerStepThrough]
    int ITrigonometricsProvider<int>.Asin(double angle) {
      return (int)System.Math.Asin(angle);
    }
    /// <summary>Calculates the arcus cosine of the specified angle</summary>
    /// <param name="angle">Angle whose arcus cosine is calculated</param>
    /// <returns>The arcus cosine of the provided angle</returns>
    [DebuggerStepThrough]
    int ITrigonometricsProvider<int>.Acos(double angle) {
      return (int)System.Math.Acos(angle);
    }
    /// <summary>Calculates the tangent of the specified angle</summary>
    /// <param name="value">Angle whose tangent is calculated</param>
    /// <returns>The tangent of the provided angle</returns>
    [DebuggerStepThrough]
    int ITrigonometricsProvider<int>.Tan(int value) {
      return (int)System.Math.Tan(value);
    }
    /// <summary>Calculates the arcus tangent of the specified angle</summary>
    /// <param name="value">Angle whose arcus tangent is calculated</param>
    /// <returns>The arcus tangent of the provided angle</returns>
    [DebuggerStepThrough]
    int ITrigonometricsProvider<int>.Atan(int value) {
      return (int)System.Math.Atan(value);
    }
    /// <summary>Calculates the arcus tangent of the provided vector</summary>
    /// <param name="y">Length of the vector on the Y axis</param>
    /// <param name="x">Length of the vector on the X axis</param>
    /// <returns>The tangent of the input vector</returns>
    [DebuggerStepThrough]
    double ITrigonometricsProvider<int>.Atan2(int x, int y) {
      return System.Math.Atan2(x, y);
    }

    //
    // IAbsoluteNumberProvider implementation
    //

    /// <summary>The number 0</summary>
    int IAbsoluteValueProvider<int>.Zero {
      [DebuggerStepThrough]
      get { return 0; }
    }
    /// <summary>The number 1</summary>
    int IAbsoluteValueProvider<int>.One {
      [DebuggerStepThrough]
      get { return 1; }
    }
    /// <summary>The lower end of the value range that can be expressed by the data type</summary>
    int IAbsoluteValueProvider<int>.MinValue {
      [DebuggerStepThrough]
      get { return int.MinValue; }
    }
    /// <summary>The upper end of the value range that can be expressed by the data type</summary>
    int IAbsoluteValueProvider<int>.MaxValue {
      [DebuggerStepThrough]
      get { return int.MaxValue; }
    }
    /// <summary>The smallest change in value that can be expressed with the data type</summary>
    int IAbsoluteValueProvider<int>.Epsilon {
      [DebuggerStepThrough]
      get { return 1; }
    }

    //
    // IScalingProvider implementation
    //

    /// <summary>Scales a value by an arbitrary factor</summary>
    /// <param name="number">Value to be scaled</param>
    /// <param name="factor">Scaling factor</param>
    /// <returns>The scaled value</returns>
    [DebuggerStepThrough]
    public int Scale(int number, double factor) { return (int)(number * factor); }
    /// <summary>Unscales a value by an arbitrary factor</summary>
    /// <param name="number">Value to be unscaled</param>
    /// <param name="divisor">Unscaling factor</param>
    /// <returns>The unscaled value</returns>
    [DebuggerStepThrough]
    public int Unscale(int number, double divisor) { return (int)(number / divisor); }

    //
    // IBinaryOperationsProvider implementation
    //

    /// <summary>Binarily ANDs the bits of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The result of the binary ANDing</returns>
    [DebuggerStepThrough]
    public int And(int first, int second) { return first & second; }
    /// <summary>Binarily ORs the bits of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The result of the binary ORing</returns>
    [DebuggerStepThrough]
    public int Or(int first, int second) { return first | second; }
    /// <summary>Binarily exclusive ORs the bits of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The result of the binary exclusive ORing</returns>
    [DebuggerStepThrough]
    public int Xor(int first, int second) { return first ^ second; }
    /// <summary>Binarily negates the bits of a number</summary>
    /// <param name="value">Value to be negated</param>
    /// <returns>The result of the binary negation</returns>
    [DebuggerStepThrough]
    public int Not(int value) { return ~value; }

    //
    // IArithmeticOperationsProvider implementation
    //

    /// <summary>Returns the absolute value of a number</summary>
    /// <param name="number">Number to return the absolute value of</param>
    /// <returns>The absolute value of the number</returns>
    [DebuggerStepThrough]
    public int Abs(int number) { return System.Math.Abs(number); }
    /// <summary>Returns the inverse value of a number</summary>
    /// <param name="number">Number to return the inverse value of</param>
    /// <returns>The inverse value of the number</returns>
    [DebuggerStepThrough]
    public int Inv(int number) { return 1 / number; }
    /// <summary>Negates a number</summary>
    /// <param name="number">Number to negate</param>
    /// <returns>The negated number</returns>
    [DebuggerStepThrough]
    public int Negate(int number) { return -number; }
    /// <summary>Forms the sum of two numbers</summary>
    /// <param name="first">First number to be summed</param>
    /// <param name="second">Second number to be summed</param>
    /// <returns>The sum of both numbers</returns>
    [DebuggerStepThrough]
    public int Add(int first, int second) { return first + second; }
    /// <summary>Subtracts one number from the other</summary>
    /// <param name="number">Base from which to subtract</param>
    /// <param name="subtrahend">Number to be subtracted</param>
    /// <returns>The result of the subtraction</returns>
    [DebuggerStepThrough]
    public int Subtract(int number, int subtrahend) { return number - subtrahend; }
    /// <summary>The product of both numbers</summary>
    /// <param name="number">First factor of the multiplication</param>
    /// <param name="factor">Second factor of the multiplication</param>
    /// <returns>The product of the multiplication of both factors</returns>
    [DebuggerStepThrough]
    public int Multiply(int number, int factor) { return number * factor; }
    /// <summary>Divides the number by another one</summary>
    /// <param name="number">Number to be divided</param>
    /// <param name="divisor">Divisor</param>
    /// <returns>The quotient of the number divided by the provided divisor</returns>
    [DebuggerStepThrough]
    public int Divide(int number, int divisor) { return number / divisor; }

    //
    // ILogarithmicOperationsProvider implementation
    //

    /// <summary>Calculates the mathematical power of a value</summary>
    /// <param name="number">Value to calculate the power of</param>
    /// <param name="power">Power to which to take the value</param>
    /// <returns>The resulting value</returns>
    [DebuggerStepThrough]
    public int Pow(int number, int power) {
      return (int)System.Math.Pow(number, power);
    }
    /// <summary>Calculates the square root of a number</summary>
    /// <param name="number">Value of which to calculate the square root</param>
    /// <returns>The square root of the input value</returns>
    [DebuggerStepThrough]
    public int Sqrt(int number) { return (int)System.Math.Sqrt(number); }
    /// <summary>Calculates the logarithm of a number</summary>
    /// <param name="number">Value of which to calculate the logarithm</param>
    /// <returns>The logarithm of the input value</returns>
    [DebuggerStepThrough]
    public int Log(int number) { return (int)System.Math.Log(number); }
    /// <summary>Calculates the exponent of a number</summary>
    /// <param name="number">Value of which to calculate the exponent</param>
    /// <returns>The exponent of the input value</returns>
    [DebuggerStepThrough]
    public int Exp(int number) { return (int)System.Math.Exp(number); }

  }

} // namespace Nuclex.Math.Generics
