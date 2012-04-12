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

  /// <summary>Unchecked math operations for float values</summary>
  public partial struct UncheckedScalarMath :
    IComparisonProvider<float>,
    IDoubleConversionProvider<float>,
    IIntConversionProvider<float>,
    ILimitProvider<double>,
    ITrigonometricsProvider<float>,
    IAbsoluteValueProvider<float>,
    IScalingProvider<float>,
    IArithmeticOperationsProvider<float>,
    ILogarithmicsProvider<float> {

    //
    // IComparisonProvider implementation
    //

    /// <summary>Checks whether two values are equal</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the two value are equal</returns>
    [DebuggerStepThrough]
    public bool Equal(float first, float second) { return first == second; }
    /// <summary>Checks whether the first value is greater than the second value</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the first value is greater than the second one</returns>
    [DebuggerStepThrough]
    public bool GreaterThan(float first, float second) { return first > second; }
    /// <summary>Checks whether the first value is less than the second value</summary>
    /// <param name="first">The first value to compare</param>
    /// <param name="second">The second value to compare</param>
    /// <returns>Whether the first value is less than the second one</returns>
    [DebuggerStepThrough]
    public bool LessThan(float first, float second) { return first < second; }
    /// <summary>Checks whether the scalar is a positive value</summary>
    /// <param name="value">Value that will be checked for positiveness</param>
    /// <returns>True if the value is positive</returns>
    [DebuggerStepThrough]
    public bool IsPositive(float value) { return value >= 0.0f; }
    /// <summary>Checks whether the scalar is a negative value</summary>
    /// <param name="value">Value that will be checked for negativeness</param>
    /// <returns>True if the value is positive</returns>
    [DebuggerStepThrough]
    public bool IsNegative(float value) { return value < 0.0f; }

    //
    // IDoubleConversionProvider implementation
    //

    /// <summary>Converts a double to the generic data type</summary>
    /// <param name="value">Value to convert</param>
    /// <returns>The input value as the generic data type</returns>
    [DebuggerStepThrough]
    float IDoubleConversionProvider<float>.FromDouble(double value) {
      unchecked { return (float)value; }
    }
    /// <summary>Converts a generic data type into a double</summary>
    /// <param name="value">Generic value to convert</param>
    /// <returns>The generic input value as a double</returns>
    [DebuggerStepThrough]
    double IDoubleConversionProvider<float>.ToDouble(float value) {
      unchecked { return value; }
    }

    //
    // IIntConversionProvider implementation
    //

    /// <summary>Converts an integer to the generic data type</summary>
    /// <param name="value">Value to convert</param>
    /// <returns>The input value as the generic data type</returns>
    [DebuggerStepThrough]
    float IIntConversionProvider<float>.FromInteger(int value) {
      unchecked { return (float)value; }
    }
    /// <summary>Converts a generic data type into an integer</summary>
    /// <param name="value">Generic value to convert</param>
    /// <returns>The generic input value as an integer</returns>
    [DebuggerStepThrough]
    public int ToInteger(float value) {
      unchecked { return (int)value; }
    }

    //
    // ILimitProvider implementation
    //

    /// <summary>Returns the smaller one of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The smaller of the two input value</returns>
    [DebuggerStepThrough]
    public float Min(float first, float second) { return first < second ? first : second; }
    /// <summary>Returns the bigger one of two values</summary>
    /// <param name="first">First value</param>
    /// <param name="second">Second value</param>
    /// <returns>The bigger of the two input value</returns>
    [DebuggerStepThrough]
    public float Max(float first, float second) { return first > second ? first : second; }
    /// <summary>Limits a value to specific range</summary>
    /// <param name="value">Value to be limited</param>
    /// <param name="minimum">Lower end of range (inclusive)</param>
    /// <param name="maximum">Upper end of range (exlusive)</param>
    /// <returns>The limited input value</returns>
    [DebuggerStepThrough]
    public float Clamp(float value, float minimum, float maximum) {
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
    public float Wrap(float value, float lower, float upper) {
      unchecked {
        if(upper <= lower)
          throw new ArithmeticException("Rotary bounds are of negative or zero size");

        float distance = upper - lower;
        float times = (float)System.Math.Floor((value - lower) / distance);

        return value - (times * distance);
      }
    }

    //
    // ITrigonometricOperationsProvider implementation
    //

    /// <summary>Calculates the sine of the specified angle</summary>
    /// <param name="angle">Angle whose sine is calculated</param>
    /// <returns>The sine of the provided angle</returns>
    [DebuggerStepThrough]
    float ITrigonometricsProvider<float>.Sin(double angle) {
      unchecked { return (float)System.Math.Sin(angle); }
    }

    /// <summary>Calculates the cosine of the specified angle</summary>
    /// <param name="angle">Angle whose cosine is calculated</param>
    /// <returns>The cosine of the provided angle</returns>
    [DebuggerStepThrough]
    float ITrigonometricsProvider<float>.Cos(double angle) {
      unchecked { return (float)System.Math.Cos(angle); }
    }

    /// <summary>Calculates the arcus sine of the specified angle</summary>
    /// <param name="angle">Angle whose arcus sine is calculated</param>
    /// <returns>The arcus sine of the provided angle</returns>
    [DebuggerStepThrough]
    float ITrigonometricsProvider<float>.Asin(double angle) {
      unchecked { return (float)System.Math.Asin(angle); }
    }

    /// <summary>Calculates the arcus cosine of the specified angle</summary>
    /// <param name="angle">Angle whose arcus cosine is calculated</param>
    /// <returns>The arcus cosine of the provided angle</returns>
    [DebuggerStepThrough]
    float ITrigonometricsProvider<float>.Acos(double angle) {
      unchecked { return (float)System.Math.Acos(angle); }
    }

    /// <summary>Calculates the tangent of the specified angle</summary>
    /// <param name="value">Angle whose tangent is calculated</param>
    /// <returns>The tangent of the provided angle</returns>
    [DebuggerStepThrough]
    float ITrigonometricsProvider<float>.Tan(float value) {
      unchecked { return (float)System.Math.Tan(value); }
    }

    /// <summary>Calculates the arcus tangent of the specified angle</summary>
    /// <param name="value">Angle whose arcus tangent is calculated</param>
    /// <returns>The arcus tangent of the provided angle</returns>
    [DebuggerStepThrough]
    float ITrigonometricsProvider<float>.Atan(float value) {
      unchecked { return (float)System.Math.Atan(value); }
    }

    /// <summary>Calculates the arcus tangent of the provided vector</summary>
    /// <param name="y">Length of the vector on the Y axis</param>
    /// <param name="x">Length of the vector on the X axis</param>
    /// <returns>The tangent of the input vector</returns>
    [DebuggerStepThrough]
    double ITrigonometricsProvider<float>.Atan2(float x, float y) {
      unchecked { return System.Math.Atan2(x, y); }
    }

    //
    // IAbsoluteNumberProvider implementation
    //

    /// <summary>The number 0</summary>
    float IAbsoluteValueProvider<float>.Zero {
      [DebuggerStepThrough]
      get { return 0; }
    }
    /// <summary>The number 1</summary>
    float IAbsoluteValueProvider<float>.One {
      [DebuggerStepThrough]
      get { return 1; }
    }
    /// <summary>The lower end of the value range that can be expressed by the data type</summary>
    float IAbsoluteValueProvider<float>.MinValue {
      [DebuggerStepThrough]
      get { return float.MinValue; }
    }
    /// <summary>The upper end of the value range that can be expressed by the data type</summary>
    float IAbsoluteValueProvider<float>.MaxValue {
      [DebuggerStepThrough]
      get { return float.MaxValue; }
    }
    /// <summary>The smallest change in value that can be expressed with the data type</summary>
    float IAbsoluteValueProvider<float>.Epsilon {
      [DebuggerStepThrough]
      get { return float.Epsilon; }
    }

    //
    // IScalingProvider implementation
    //

    /// <summary>Scales a value by an arbitrary factor</summary>
    /// <param name="number">Value to be scaled</param>
    /// <param name="factor">Scaling factor</param>
    /// <returns>The scaled value</returns>
    [DebuggerStepThrough]
    public float Scale(float number, double factor) {
      unchecked { return (float)(number * factor); }
    }
    /// <summary>Unscales a value by an arbitrary factor</summary>
    /// <param name="number">Value to be unscaled</param>
    /// <param name="divisor">Unscaling factor</param>
    /// <returns>The unscaled value</returns>
    [DebuggerStepThrough]
    public float Unscale(float number, double divisor) {
      unchecked { return (float)(number / divisor); }
    }

    //
    // IArithmeticOperationsProvider implementation
    //

    /// <summary>Returns the absolute value of a number</summary>
    /// <param name="number">Number to return the absolute value of</param>
    /// <returns>The absolute value of the number</returns>
    [DebuggerStepThrough]
    public float Abs(float number) {
      unchecked { return System.Math.Abs(number); }
    }
    /// <summary>Returns the inverse value of a number</summary>
    /// <param name="number">Number to return the inverse value of</param>
    /// <returns>The inverse value of the number</returns>
    [DebuggerStepThrough]
    public float Inv(float number) {
      unchecked { return 1.0f / number; }
    }
    /// <summary>Negates a number</summary>
    /// <param name="number">Number to negate</param>
    /// <returns>The negated number</returns>
    [DebuggerStepThrough]
    public float Negate(float number) {
      unchecked { return -number; }
    }
    /// <summary>Forms the sum of two numbers</summary>
    /// <param name="first">First number to be summed</param>
    /// <param name="second">Second number to be summed</param>
    /// <returns>The sum of both numbers</returns>
    [DebuggerStepThrough]
    public float Add(float first, float second) {
      unchecked { return first + second; }
    }
    /// <summary>Subtracts one number from the other</summary>
    /// <param name="number">Base from which to subtract</param>
    /// <param name="subtrahend">Number to be subtracted</param>
    /// <returns>The result of the subtraction</returns>
    [DebuggerStepThrough]
    public float Subtract(float number, float subtrahend) {
      unchecked { return number - subtrahend; }
    }
    /// <summary>The product of both numbers</summary>
    /// <param name="number">First factor of the multiplication</param>
    /// <param name="factor">Second factor of the multiplication</param>
    /// <returns>The product of the multiplication of both factors</returns>
    [DebuggerStepThrough]
    public float Multiply(float number, float factor) {
      unchecked { return number * factor; }
    }
    /// <summary>Divides the number by another one</summary>
    /// <param name="number">Number to be divided</param>
    /// <param name="divisor">Divisor</param>
    /// <returns>The quotient of the number divided by the provided divisor</returns>
    [DebuggerStepThrough]
    public float Divide(float number, float divisor) {
      unchecked { return number / divisor; }
    }

    //
    // ILogarithmicOperationsProvider implementation
    //

    /// <summary>Calculates the mathematical power of a value</summary>
    /// <param name="number">Value to calculate the power of</param>
    /// <param name="power">Power to which to take the value</param>
    /// <returns>The resulting value</returns>
    [DebuggerStepThrough]
    public float Pow(float number, float power) {
      unchecked { return (float)System.Math.Pow(number, power); }
    }
    /// <summary>Calculates the square root of a number</summary>
    /// <param name="number">Value of which to calculate the square root</param>
    /// <returns>The square root of the input value</returns>
    [DebuggerStepThrough]
    public float Sqrt(float number) {
      unchecked { return (float)System.Math.Sqrt(number); }
    }
    /// <summary>Calculates the logarithm of a number</summary>
    /// <param name="number">Value of which to calculate the logarithm</param>
    /// <returns>The logarithm of the input value</returns>
    [DebuggerStepThrough]
    public float Log(float number) {
      unchecked { return (float)System.Math.Log(number); }
    }
    /// <summary>Calculates the exponent of a number</summary>
    /// <param name="number">Value of which to calculate the exponent</param>
    /// <returns>The exponent of the input value</returns>
    [DebuggerStepThrough]
    public float Exp(float number) {
      unchecked { return (float)System.Math.Exp(number); }
    }

  }

} // namespace Nuclex.Math.Generics
