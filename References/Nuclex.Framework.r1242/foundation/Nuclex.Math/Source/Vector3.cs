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

  /// <summary>The components of a 3D vector</summary>
  public enum Vector3Components : int {
    /// <summary>The X coordinate of the vector</summary>
    X,
    /// <summary>The Y coordinate of the vector</summary>
    Y,
    /// <summary>The Z coordinate of the vector</summary>
    Z
  }

  /// <summary>Simple 3D vector which also doubles as point class</summary>
  /// <typeparam name="ScalarType">Type the vector uses for its components</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public struct Vector3<ScalarType, MathType>
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
    public static Vector3<ScalarType, MathType> Zero {
      [DebuggerStepThrough]
      get { return new Vector3<ScalarType, MathType>(); }
    }

    /// <summary>A vector whose elements have been initialized to 1</summary>
    public static Vector3<ScalarType, MathType> One {
      [DebuggerStepThrough]
      get {
        return new Vector3<ScalarType, MathType>(
          math.One, math.One, math.One
        );
      }
    }

    /// <summary>A unit-length vector pointing right</summary>
    public static Vector3<ScalarType, MathType> Right {
      [DebuggerStepThrough]
      get {
        return new Vector3<ScalarType, MathType>(
          math.One, math.Zero, math.Zero
        );
      }
    }

    /// <summary>A unit-length vector pointing upwards</summary>
    public static Vector3<ScalarType, MathType> Up {
      [DebuggerStepThrough]
      get {
        return new Vector3<ScalarType, MathType>(
          math.Zero, math.One, math.Zero
        );
      }
    }

    /// <summary>A unit-length vector pointing inwards</summary>
    public static Vector3<ScalarType, MathType> In {
      [DebuggerStepThrough]
      get {
        return new Vector3<ScalarType, MathType>(
          math.Zero, math.Zero, math.One
        );
      }
    }

    /// <summary>Constructs a new vector from the specified components</summary>
    /// <param name="x">Location of the vector on the X axis</param>
    /// <param name="y">Location of the vector on the Y axis</param>
    /// <param name="z">Location of the vector on the Z axis</param>
    [DebuggerStepThrough]
    public Vector3(ScalarType x, ScalarType y, ScalarType z) {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    /// <summary>Normalizes the vector to a length of 1</summary>
    /// <param name="vector">Vector to be normalized</param>
    /// <returns>The normalized vector</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> Normalize(
      Vector3<ScalarType, MathType> vector
    ) {
      Number<ScalarType, MathType> length = vector.Length;

      if(length == math.Zero) {
        return new Vector3<ScalarType, MathType>();
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
      Vector3<ScalarType, MathType> first, Vector3<ScalarType, MathType> second
    ) {
      return first.X * second.X + first.Y * second.Y + first.Z * second.Z;
    }

    /// <summary>Forms the cross product of two vectors</summary>
    /// <param name="first">First vector for the multiplication</param>
    /// <param name="second">Second vector for the multiplication</param>
    /// <returns>The cross product of the multiplication of both vectors</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> CrossProduct(
      Vector3<ScalarType, MathType> first, Vector3<ScalarType, MathType> second
    ) {
      return new Vector3<ScalarType, MathType>(
        first.Y * second.Z - first.Z * second.Y,
        first.Z * second.X - first.X * second.Z,
        first.X * second.Y - first.Y * second.X
      );
    }

    /// <summary>Returns a vector consisting of the absolute values of the input vector</summary>
    /// <param name="vector">Vector whose elements to use the absolute values of</param>
    /// <returns>New vector constructed from the absolute values of input vector</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> Abs(Vector3<ScalarType, MathType> vector) {
      return new Vector3<ScalarType, MathType>(
        math.Abs(vector.X), math.Abs(vector.Y), math.Abs(vector.Z)
      );
    }

    /// <summary>Returns a vector consisting of the minimum values of two vectors</summary>
    /// <param name="first">First vector to be considered</param>
    /// <param name="second">Second vector to be considered</param>
    /// <returns>A vector with the element-wise minimum values of both input vectors</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> Min(
      Vector3<ScalarType, MathType> first, Vector3<ScalarType, MathType> second
    ) {
      return new Vector3<ScalarType, MathType>(
        math.Min(first.X, second.X),
        math.Min(first.Y, second.Y),
        math.Min(first.Z, second.Z)
      );
    }

    /// <summary>Returns a vector consisting of the maximum values of two vectors</summary>
    /// <param name="first">First vector to be considered</param>
    /// <param name="second">Second vector to be considered</param>
    /// <returns>A vector with the element-wise maximum values of both input vectors</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> Max(
      Vector3<ScalarType, MathType> first, Vector3<ScalarType, MathType> second
    ) {
      return new Vector3<ScalarType, MathType>(
        math.Max(first.X, second.X),
        math.Max(first.Y, second.Y),
        math.Max(first.Z, second.Z)
      );
    }

    /// <summary>Rotates the vector by an arbitrary angle around the X axis</summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Rotation angle</param>
    /// <returns>The rotated vector</returns>
    public static Vector3<ScalarType, MathType> RotateX(
      Vector3<ScalarType, MathType> vector, double angle
    ) {
      Number<ScalarType, MathType> sinAngle = math.Sin(angle);
      Number<ScalarType, MathType> cosAngle = math.Cos(angle);

      return new Vector3<ScalarType, MathType>(
        vector.X,
        vector.Y * cosAngle + vector.Z * sinAngle,
        vector.Z * cosAngle - vector.Y * sinAngle
      );
    }

    /// <summary>Rotates the vector by an arbitrary angle around the Y axis</summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Rotation angle</param>
    /// <returns>The rotated vector</returns>
    public static Vector3<ScalarType, MathType> RotateY(
      Vector3<ScalarType, MathType> vector, double angle
    ) {
      Number<ScalarType, MathType> sinAngle = math.Sin(angle);
      Number<ScalarType, MathType> cosAngle = math.Cos(angle);

      return new Vector3<ScalarType, MathType>(
        vector.X * cosAngle + vector.Z * sinAngle,
        vector.Y,
        vector.Z * cosAngle - vector.X * sinAngle
      );
    }

    /// <summary>Rotates the vector by an arbitrary angle around the Z axis</summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Rotation angle</param>
    /// <returns>The rotated vector</returns>
    public static Vector3<ScalarType, MathType> RotateZ(
      Vector3<ScalarType, MathType> vector, double angle
    ) {
      Number<ScalarType, MathType> sinAngle = math.Sin(angle);
      Number<ScalarType, MathType> cosAngle = math.Cos(angle);

      return new Vector3<ScalarType, MathType>(
        vector.X * cosAngle + vector.Y * sinAngle,
        vector.Y * cosAngle - vector.X * sinAngle,
        vector.Z
      );
    }

    /// <summary>Rotates the vector by an arbitrary angle around a free axis</summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="axis">Rotation axis</param>
    /// <param name="angle">Rotation angle</param>
    /// <returns>The rotated vector</returns>
    public static Vector3<ScalarType, MathType> RotateAround(
      Vector3<ScalarType, MathType> axis, Vector3<ScalarType, MathType> vector, double angle
    ) {
      axis = Normalize(axis);

      Number<ScalarType, MathType> sinAngle = math.Sin(angle);
      Number<ScalarType, MathType> cosAngle = math.Cos(angle);
      Number<ScalarType, MathType> oneMinusCos = math.One - cosAngle;

      return new Vector3<ScalarType, MathType>(

        (oneMinusCos * axis.X * axis.X + cosAngle * vector.X) +
        (oneMinusCos * axis.X * axis.Y - sinAngle * axis.Z * vector.Y) +
        (oneMinusCos * axis.Z * axis.X + sinAngle * axis.Y * vector.Z),

        (oneMinusCos * axis.X * axis.Y + sinAngle * axis.Z * vector.X) +
        (oneMinusCos * axis.Y * axis.Y + cosAngle * vector.Y) +
        (oneMinusCos * axis.Y * axis.Z - sinAngle * axis.X * vector.Z),

        (oneMinusCos * axis.Z * axis.X - sinAngle * axis.Y * vector.X) +
        (oneMinusCos * axis.Y * axis.Z + sinAngle * axis.X * vector.Y) +
        (oneMinusCos * axis.Z * axis.Z + cosAngle * vector.Z)

      );

    }

    /// <summary>Access a component of the vector by its index</summary>
    /// <param name="component">Index of the component to access</param>
    public ScalarType this[int component] {
      [DebuggerStepThrough]
      get {
        switch(component) {
          case (int)Vector3Components.X: { return this.X; }
          case (int)Vector3Components.Y: { return this.Y; }
          case (int)Vector3Components.Z: { return this.Z; }
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
          case (int)Vector3Components.X: { this.X = value; break; }
          case (int)Vector3Components.Y: { this.Y = value; break; }
          case (int)Vector3Components.Z: { this.Z = value; break; }
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

    /// <summary>Determines if an object is identical to the vector</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the vector</returns>
    public override bool Equals(object obj) {
      if(obj is Vector3<ScalarType, MathType>) {
        return (this == (Vector3<ScalarType, MathType>)obj);
      } else {
        return false;
      }
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      unchecked { return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode(); }
    }

    /// <summary>Converts the instance to a string</summary>
    /// <returns>A string describing this instance</returns>
    public override string ToString() {
      return "{" + X.ToString() + "; " + Y.ToString() + "; " + Z.ToString() + "}";
    }

    /// <summary>Negates the vector's elements</summary>
    /// <param name="vector">vector to negate</param>
    /// <returns>The negated vector</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> operator -(
      Vector3<ScalarType, MathType> vector
    ) {
      return new Vector3<ScalarType, MathType>(-vector.X, -vector.Y, -vector.Z);
    }

    /// <summary>Forms the sum of two vectors</summary>
    /// <param name="first">First vector to be summed</param>
    /// <param name="second">Second vector to be summed</param>
    /// <returns>The sum of both vectors</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> operator +(
      Vector3<ScalarType, MathType> first, Vector3<ScalarType, MathType> second
    ) {
      return new Vector3<ScalarType, MathType>(
        first.X + second.X, first.Y + second.Y, first.Z + second.Z
      );
    }

    /// <summary>Subtracts one vector from another</summary>
    /// <param name="vector">Base vector from which the subtraction occurs</param>
    /// <param name="subtrahend">Vector to be subtracted</param>
    /// <returns>The result of the subtraction</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> operator -(
      Vector3<ScalarType, MathType> vector, Vector3<ScalarType, MathType> subtrahend
    ) {
      return new Vector3<ScalarType, MathType>(
        vector.X - subtrahend.X,
        vector.Y - subtrahend.Y,
        vector.Z - subtrahend.Z
      );
    }

    /// <summary>Calculates the product of a vector and a scalar</summary>
    /// <param name="vector">First factor of the multiplication</param>
    /// <param name="factor">Scalar value by which to scale the vector</param>
    /// <returns>The product of the multiplication of the vector with the scalar</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> operator *(
      Vector3<ScalarType, MathType> vector, Number<ScalarType, MathType> factor
    ) {
      return new Vector3<ScalarType, MathType>(
        vector.X * factor, vector.Y * factor, vector.Z * factor
      );
    }

    /// <summary>Calculates the product of two vectors</summary>
    /// <param name="vector">First factor of the multiplication</param>
    /// <param name="factor">Second factor of the multiplication</param>
    /// <returns>The product of the multiplication of both vector's elements</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> operator *(
      Vector3<ScalarType, MathType> vector, Vector3<ScalarType, MathType> factor
    ) {
      return new Vector3<ScalarType, MathType>(
        vector.X * factor.X,
        vector.Y * factor.Y,
        vector.Z * factor.Z
      );
    }

    /// <summary>Divides a vector by a scalar</summary>
    /// <param name="vector">Vector to be divided</param>
    /// <param name="divisor">The divisor</param>
    /// <returns>The quotient of the vector and the scalar</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> operator /(
      Vector3<ScalarType, MathType> vector, Number<ScalarType, MathType> divisor
    ) {
      return new Vector3<ScalarType, MathType>(
        vector.X / divisor,
        vector.Y / divisor,
        vector.Z / divisor
      );
    }

    /// <summary>Element-wise division of a vector by another vector</summary>
    /// <param name="vector">Vector to be divided</param>
    /// <param name="divisor">Vector containing the divisors for each axis</param>
    /// <returns>The quotient vector of the vector and the divisor vector</returns>
    [DebuggerStepThrough]
    public static Vector3<ScalarType, MathType> operator /(
      Vector3<ScalarType, MathType> vector, Vector3<ScalarType, MathType> divisor
    ) {
      return new Vector3<ScalarType, MathType>(
        vector.X / divisor.X,
        vector.Y / divisor.Y,
        vector.Z / divisor.Z
      );
    }

    /// <summary>Determines if two vectors are equal</summary>
    /// <param name="first">First vector to be compared</param>
    /// <param name="second">Second vector to be compared</param>
    /// <returns>True if both vectors are equal</returns>
    [DebuggerStepThrough]
    public static bool operator ==(
      Vector3<ScalarType, MathType> first, Vector3<ScalarType, MathType> second
    ) {
      return (first.X == second.X) && (first.Y == second.Y) && (first.Z == second.Z);
    }

    /// <summary>Determines if two vectors are unequal</summary>
    /// <param name="first">First vector to be compared</param>
    /// <param name="second">Second vector to be compared</param>
    /// <returns>True if both vectors are unequal</returns>
    [DebuggerStepThrough]
    public static bool operator !=(
      Vector3<ScalarType, MathType> first, Vector3<ScalarType, MathType> second
    ) {
      return (first.X != second.X) || (first.Y != second.Y) || (first.Z != second.Z);
    }

    /// <summary>Internally used math package</summary>
    private static MathType math = Shared<MathType>.Instance;

    /// <summary>The components of the vector</summary>
    public Number<ScalarType, MathType> X, Y, Z;

  }

} // namespace Nuclex.Math
