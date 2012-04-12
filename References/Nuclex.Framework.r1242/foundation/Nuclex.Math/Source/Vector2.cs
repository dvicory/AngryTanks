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

using Nuclex.Math.Generic;
using Nuclex.Support;

namespace Nuclex.Math {

  /// <summary>The components of a 2D vector</summary>
  public enum Vector2Components : int {
    /// <summary>The X coordinate of the vector</summary>
    X,
    /// <summary>The Y coordinate of the vector</summary>
    Y
  }

  /// <summary>Simple 2D vector which also doubles as point class</summary>
  /// <typeparam name="ScalarType">Type the vector uses for its components</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public struct Vector2<ScalarType, MathType>
    where MathType :
      IAbsoluteValueProvider<ScalarType>,
      IArithmeticOperationsProvider<ScalarType>,
      IComparisonProvider<ScalarType>,
      ILimitProvider<ScalarType>,
      ILogarithmicsProvider<ScalarType>,
      IScalingProvider<ScalarType>,
      ITrigonometricsProvider<ScalarType>,
      new() {

    /// <summary>A vector whose elements have been initialized to 0</summary>
    public static Vector2<ScalarType, MathType> Zero {
      [DebuggerStepThrough]
      get { return new Vector2<ScalarType, MathType>(); }
    }

    /// <summary>A vector whose elements have been initialized to 1</summary>
    public static Vector2<ScalarType, MathType> One {
      [DebuggerStepThrough]
      get { return new Vector2<ScalarType, MathType>(math.One, math.One); }
    }

    /// <summary>A unit-length vector pointing right</summary>
    public static Vector2<ScalarType, MathType> Right {
      [DebuggerStepThrough]
      get { return new Vector2<ScalarType, MathType>(math.One, math.Zero); }
    }

    /// <summary>A unit-length vector pointing upwards</summary>
    public static Vector2<ScalarType, MathType> Up {
      [DebuggerStepThrough]
      get { return new Vector2<ScalarType, MathType>(math.Zero, math.One); }
    }

    /// <summary>Constructs a new vector from the specified components</summary>
    /// <param name="x">Location of the vector on the X axis</param>
    /// <param name="y">Location of the vector on the Y axis</param>
    [DebuggerStepThrough]
    public Vector2(ScalarType x, ScalarType y) {
      this.X = x;
      this.Y = y;
    }

    /// <summary>Normalizes the vector to a length of 1</summary>
    /// <param name="vector">Vector to be normalized</param>
    /// <returns>The normalized vector</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> Normalize(Vector2<ScalarType, MathType> vector) {
      Number<ScalarType, MathType> length = vector.Length;

      if(length == math.Zero) {
        return new Vector2<ScalarType, MathType>();
      } else {
        return vector / length;
      }
    }

    /// <summary>Forms the scalar product of two vectors</summary>
    /// <param name="first">First vector for the multiplication</param>
    /// <param name="second">Second vector for the multiplication</param>
    /// <returns>The scalar product of the multiplication of both vectors</returns>
    [DebuggerStepThrough]
    public static ScalarType DotProduct(
      Vector2<ScalarType, MathType> first, Vector2<ScalarType, MathType> second
    ) {
      return first.X * second.X + first.Y * second.Y;
    }

    /// <summary>Forms the cross product of two vectors</summary>
    /// <param name="first">First vector for the multiplication</param>
    /// <param name="second">Second vector for the multiplication</param>
    /// <returns>The cross product of the multiplication of both vectors</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> CrossProduct(
      Vector2<ScalarType, MathType> first, Vector2<ScalarType, MathType> second
    ) {
      return new Vector2<ScalarType, MathType>(
        (first.Y * second.X) - (first.X * second.Y),
        (first.X * second.Y) - (first.Y * second.X)
      );
    }

    /// <summary>Returns a vector consisting of the absolute values of the input vector</summary>
    /// <param name="vector">Vector whose elements to use the absolute values of</param>
    /// <returns>New vector constructed from the absolute values of input vector</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> Abs(Vector2<ScalarType, MathType> vector) {
      return new Vector2<ScalarType, MathType>(
        math.Abs(vector.X), math.Abs(vector.Y)
      );
    }

    /// <summary>Returns a vector consisting of the maximum values of two vectors</summary>
    /// <param name="first">First vector to be considered</param>
    /// <param name="second">Second vector to be considered</param>
    /// <returns>A vector with the element-wise maximum values of both input vectors</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> Max(
      Vector2<ScalarType, MathType> first, Vector2<ScalarType, MathType> second
    ) {
      return new Vector2<ScalarType, MathType>(
        math.Max(first.X, second.X),
        math.Max(first.Y, second.Y)
      );
    }

    /// <summary>Returns a vector consisting of the minimum values of two vectors</summary>
    /// <param name="first">First vector to be considered</param>
    /// <param name="second">Second vector to be considered</param>
    /// <returns>A vector with the element-wise minimum values of both input vectors</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> Min(
      Vector2<ScalarType, MathType> first, Vector2<ScalarType, MathType> second
    ) {
      return new Vector2<ScalarType, MathType>(
        math.Min(first.X, second.X),
        math.Min(first.Y, second.Y)
      );
    }

    /// <summary>Rotates the vector by an arbitrary angle</summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Rotation angle</param>
    /// <returns>The rotated vector</returns>
    public static Vector2<ScalarType, MathType> Rotate(
      Vector2<ScalarType, MathType> vector, double angle
    ) {
      Number<ScalarType, MathType> sinAngle = math.Sin(angle);
      Number<ScalarType, MathType> cosAngle = math.Cos(angle);

      return new Vector2<ScalarType, MathType>(
        (vector.X * cosAngle) - (vector.Y * sinAngle),
        (vector.Y * cosAngle) + (vector.X * sinAngle)
      );
    }

    /// <summary>Access a component of the vector by its index</summary>
    /// <param name="component">Index of the component to access</param>
    public ScalarType this[int component] {
      [DebuggerStepThrough]
      get {
        switch(component) {
          case (int)Vector2Components.X: { return this.X; }
          case (int)Vector2Components.Y: { return this.Y; }
          default: {
            throw new ArgumentOutOfRangeException(
              "Vector component index is out of range", "component"
            );
          }
        }
      }
      [DebuggerStepThrough]
      set {
        switch(component) {
          case (int)Vector2Components.X: { this.X = value; break; }
          case (int)Vector2Components.Y: { this.Y = value; break; }
          default: {
            throw new ArgumentOutOfRangeException(
              "Vector component index is out of range", "component"
            );
          }
        }
      }
    }

    /// <summary>The length of the vector</summary>
    public ScalarType Length {
      [DebuggerStepThrough]
      get { return math.Sqrt(SquaredLength); }
    }

    /// <summary>The squared length of the vector (cheaper to calculate than the length)</summary>
    public ScalarType SquaredLength {
      [DebuggerStepThrough]
      get { return DotProduct(this, this); }
    }

    /// <summary>The vector clockwise rotated by 90 degrees</summary>
    public Vector2<ScalarType, MathType> Perpendicular {
      [DebuggerStepThrough]
      get { return new Vector2<ScalarType, MathType>(Y, -X); }
    }

    /// <summary>The vector coounter clockwise rotated by 90 degrees</summary>
    public Vector2<ScalarType, MathType> InvPerpendicular {
      [DebuggerStepThrough]
      get { return new Vector2<ScalarType, MathType>(-Y, X); }
    }

    /// <summary>Determines if an object is identical to the vector</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the vector</returns>
    public override bool Equals(object obj) {
      if(obj is Vector2<ScalarType, MathType>) {
        return (this == (Vector2<ScalarType, MathType>)obj);
      } else {
        return false;
      }
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      return X.GetHashCode() + Y.GetHashCode();
    }

    /// <summary>Converts the instance to a string</summary>
    /// <returns>A string describing this instance</returns>
    public override string ToString() {
      return "{" + X.ToString() + "; " + Y.ToString() + "}";
    }

    /// <summary>Negates the vector's elements</summary>
    /// <param name="vector">vector to negate</param>
    /// <returns>The negated vector</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> operator -(
      Vector2<ScalarType, MathType> vector
    ) {
      return new Vector2<ScalarType, MathType>(-vector.X, -vector.Y);
    }

    /// <summary>Forms the sum of two vectors</summary>
    /// <param name="first">First vector to be summed</param>
    /// <param name="second">Second vector to be summed</param>
    /// <returns>The sum of both vectors</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> operator +(
      Vector2<ScalarType, MathType> first, Vector2<ScalarType, MathType> second
    ) {
      return new Vector2<ScalarType, MathType>(first.X + second.X, first.Y + second.Y);
    }

    /// <summary>Subtracts one vector from another</summary>
    /// <param name="vector">Base vector from which the subtraction occurs</param>
    /// <param name="subtrahend">Vector to be subtracted</param>
    /// <returns>The result of the subtraction</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> operator -(
      Vector2<ScalarType, MathType> vector, Vector2<ScalarType, MathType> subtrahend
    ) {
      return new Vector2<ScalarType, MathType>(vector.X - subtrahend.X, vector.Y - subtrahend.Y);
    }

    /// <summary>Calculates the product of a vector and a scalar</summary>
    /// <param name="vector">First factor of the multiplication</param>
    /// <param name="factor">Scalar value by which to scale the vector</param>
    /// <returns>The product of the multiplication of the vector with the scalar</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> operator *(
      Vector2<ScalarType, MathType> vector, ScalarType factor
    ) {
      return new Vector2<ScalarType, MathType>(vector.X * factor, vector.Y * factor);
    }

    /// <summary>Calculates the product of two vectors</summary>
    /// <param name="vector">First factor of the multiplication</param>
    /// <param name="factor">Second factor of the multiplication</param>
    /// <returns>The product of the multiplication of both vector's elements</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> operator *(
      Vector2<ScalarType, MathType> vector, Vector2<ScalarType, MathType> factor
    ) {
      return new Vector2<ScalarType, MathType>(vector.X * factor.X, vector.Y * factor.Y);
    }

    /// <summary>Divides a vector by a scalar</summary>
    /// <param name="vector">Vector to be divided</param>
    /// <param name="divisor">The divisor</param>
    /// <returns>The quotient of the vector and the scalar</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> operator /(
      Vector2<ScalarType, MathType> vector, ScalarType divisor
    ) {
      return new Vector2<ScalarType, MathType>(vector.X / divisor, vector.Y / divisor);
    }

    /// <summary>Element-wise division of a vector by another vector</summary>
    /// <param name="vector">Vector to be divided</param>
    /// <param name="divisor">Vector containing the divisors for each axis</param>
    /// <returns>The quotient vector of the vector and the divisor vector</returns>
    [DebuggerStepThrough]
    public static Vector2<ScalarType, MathType> operator /(
      Vector2<ScalarType, MathType> vector, Vector2<ScalarType, MathType> divisor
    ) {
      return new Vector2<ScalarType, MathType>(vector.X / divisor.X, vector.Y / divisor.Y);
    }

    /// <summary>Determines if two vectors are equal</summary>
    /// <param name="first">First vector to be compared</param>
    /// <param name="second">Second vector to be compared</param>
    /// <returns>True if both vectors are equal</returns>
    [DebuggerStepThrough]
    public static bool operator ==(
      Vector2<ScalarType, MathType> first, Vector2<ScalarType, MathType> second
    ) {
      return (first.X == second.X) && (first.Y == second.Y);
    }

    /// <summary>Determines if two vectors are unequal</summary>
    /// <param name="first">First vector to be compared</param>
    /// <param name="second">Second vector to be compared</param>
    /// <returns>True if both vectors are unequal</returns>
    [DebuggerStepThrough]
    public static bool operator !=(
      Vector2<ScalarType, MathType> first, Vector2<ScalarType, MathType> second
    ) {
      return (first.X != second.X) || (first.Y != second.Y);
    }

    /// <summary>The components of the vector</summary>
    public Number<ScalarType, MathType> X, Y;

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

  }

} // namespace Nuclex.Math.Generic
