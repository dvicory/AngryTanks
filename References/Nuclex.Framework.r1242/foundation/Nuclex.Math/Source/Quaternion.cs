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

using Nuclex.Math.Generic;
using Nuclex.Support;

namespace Nuclex.Math {

  /// <summary>The components of a quaternion</summary>
  public enum QuaternionComponents : int {
    /// <summary>The X component of the quaternion</summary>
    X,
    /// <summary>The Y component of the quaternion</summary>
    Y,
    /// <summary>The Z component of the quaternion</summary>
    Z,
    /// <summary>The W component of the quaternion</summary>
    W
  }

  /// <summary>Quaternion for expressing rotations and orientations</summary>
  /// <typeparam name="ScalarType">Type that the quaternion uses for its components</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public struct Quaternion<ScalarType, MathType>
    where MathType :
      IAbsoluteValueProvider<ScalarType>,
      IArithmeticOperationsProvider<ScalarType>,
      IComparisonProvider<ScalarType>,
      ILimitProvider<ScalarType>,
      ILogarithmicsProvider<ScalarType>,
      IScalingProvider<ScalarType>,
      ITrigonometricsProvider<ScalarType>,
      new() {

    /// <summary>Builds a quaternion with the same orientation as a matrix</summary>
    /// <param name="matrix">Matrix from which the orientation is taken</param>
    /// <returns>A quaternion with an identical orientation as the matrix</returns>
    /// <remarks>
    ///   Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
    ///   article "Quaternion Calculus and Fast Animation".
    /// </remarks>
    public static Quaternion<ScalarType, MathType> FromMatrix(
      Matrix33<ScalarType, MathType> matrix
    ) {
      Quaternion<ScalarType, MathType> quaternion = new Quaternion<ScalarType, MathType>();

      Number<ScalarType, MathType> trace = n(matrix[0][0]) + n(matrix[1][1]) + n(matrix[2][2]);
      Number<ScalarType, MathType> root;

      if(trace > math.Zero) {
        root = math.Sqrt(trace + math.One);
        quaternion.W = math.Scale(root, 0.5);
        root = math.Scale(math.Inv(root), 0.5);

        quaternion.X = n(matrix[2][1]) - n(matrix[1][2]) * root;
        quaternion.Y = n(matrix[0][2]) - n(matrix[2][0]) * root;
        quaternion.Z = n(matrix[1][0]) - n(matrix[0][1]) * root;

      } else {

        int[] next = new int[] { 1, 2, 0 };

        int i = 0;
        if(n(matrix[1][1]) > n(matrix[0][0]))
          i = 1;
        if(n(matrix[2][2]) > n(matrix[i][i]))
          i = 2;

        int j = next[i];
        int k = next[j];

        root = math.Sqrt(n(matrix[i][i]) - n(matrix[j][j]) - n(matrix[k][k]) + math.One);
        quaternion[i] = math.Scale(root, 0.5);
        root = math.Scale(math.Inv(root), 0.5);

        quaternion.W = (n(matrix[k][j]) - n(matrix[j][k])) * root;
        quaternion[j] = (n(matrix[j][i]) + n(matrix[i][j])) * root;
        quaternion[k] = (n(matrix[k][i]) + n(matrix[i][k])) * root;
      }

      return quaternion;
    }

    /// <summary>Construct a quaternion from the given euler angles</summary>
    /// <param name="yaw">Rotation angle around the Y axis</param>
    /// <param name="pitch">Rotation angle around the X axis</param>
    /// <param name="roll">Rotation angle around the Z axis</param>
    /// <returns>A quaternion with an orientation equivalent to the input angles</returns>
    public static Quaternion<ScalarType, MathType> FromEulerAngles(
      double yaw, double pitch, double roll
    ) {
      double halfYaw = yaw / 2.0;
      double halfPitch = pitch / 2.0;
      double halfRoll = roll / 2.0;

      Number<ScalarType, MathType> cosYaw = math.Cos(halfYaw);
      Number<ScalarType, MathType> sinYaw = math.Sin(halfYaw);
      Number<ScalarType, MathType> cosPitch = math.Cos(halfPitch);
      Number<ScalarType, MathType> sinPitch = math.Sin(halfPitch);
      Number<ScalarType, MathType> cosRoll = math.Cos(halfRoll);
      Number<ScalarType, MathType> sinRoll = math.Sin(halfRoll);

      return new Quaternion<ScalarType, MathType>(
        cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw,
        cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw,
        sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw,
        cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw
      );
    }

    /// <summary>Normalizes the quaternion to a length of 1</summary>
    /// <param name="quaternion">Quaternion to be normalized</param>
    /// <returns>The normalized quaternion</returns>
    public static Quaternion<ScalarType, MathType> Normalize(
      Quaternion<ScalarType, MathType> quaternion
    ) {
      Number<ScalarType, MathType> length = quaternion.Length;

      if(length == math.Zero) {
        return new Quaternion<ScalarType, MathType>(); // 1, 0, 0, 0);
      } else {
        return new Quaternion<ScalarType, MathType>(
          quaternion.X / length, quaternion.Y / length,
          quaternion.Z / length, quaternion.W / length
        );
      }
    }

    /// <summary>Forms the scalar product of two quaternions</summary>
    /// <param name="first">First quaternion for the multiplication</param>
    /// <param name="second">Second quaternion for the multiplication</param>
    /// <returns>The scalar product of the multiplication of both quaternions</returns>
    public ScalarType DotProduct(
      Quaternion<ScalarType, MathType> first, Quaternion<ScalarType, MathType> second
    ) {
      return first.X * second.X + first.Y * second.Y + first.Z * second.Z + first.W * second.W;
    }

    /// <summary>Initializes the quaternion instance</summary>
    /// <param name="x">X component of the quaternion</param>
    /// <param name="y">Y component of the quaternion</param>
    /// <param name="z">Z component of the quaternion</param>
    public Quaternion(ScalarType x, ScalarType y, ScalarType z) :
      this(x, y, z, math.One) { }

    /// <summary>Initializes the quaternion instance</summary>
    /// <param name="x">X component of the quaternion</param>
    /// <param name="y">Y component of the quaternion</param>
    /// <param name="z">Z component of the quaternion</param>
    /// <param name="w">W component of the quaternion</param>
    public Quaternion(ScalarType x, ScalarType y, ScalarType z, ScalarType w) {
      this.X = x;
      this.Y = y;
      this.Z = z;
      this.W = w;
    }

    /// <summary>Multiplies two quaternions</summary>
    /// <param name="quaternion">Quaternion to be multiplied</param>
    /// <param name="factor">Quaternion which serves as multiplication factor</param>
    /// <returns>The product of the two quaternions</returns>
    public static Quaternion<ScalarType, MathType> operator *(
      Quaternion<ScalarType, MathType> quaternion, Quaternion<ScalarType, MathType> factor
    ) {
      return new Quaternion<ScalarType, MathType>(
        factor.W * quaternion.X + factor.X * quaternion.W +
          factor.Y * quaternion.Z - factor.Z * quaternion.Y,

        factor.W * quaternion.Y - factor.X * quaternion.Z +
          factor.Y * quaternion.W + factor.Z * quaternion.X,

        factor.W * quaternion.Z + factor.X * quaternion.Y -
          factor.Y * quaternion.X + factor.Z * quaternion.W,

        factor.W * quaternion.W - factor.X * quaternion.X -
          factor.Y * quaternion.Y - factor.Z * quaternion.Z
      );
    }

    /// <summary>The squared length of the quaternion</summary>
    public ScalarType SquaredLength {
      get { return DotProduct(this, this); }
    }

    /// <summary>The length of the quaternion</summary>
    public ScalarType Length {
      get { return math.Sqrt(SquaredLength); }
    }

    /// <summary>Access a component of the quaternion by its index</summary>
    /// <param name="component">Index of the component to access</param>
    public ScalarType this[int component] {
      get {
        switch(component) {
          case (int)QuaternionComponents.X: { return this.X; }
          case (int)QuaternionComponents.Y: { return this.Y; }
          case (int)QuaternionComponents.Z: { return this.Z; }
          case (int)QuaternionComponents.W: { return this.W; }
          default: {
            throw new ArgumentOutOfRangeException("Vector component index is out of range");
          }
        }
      }
      set {
        switch(component) {
          case (int)QuaternionComponents.X: { this.X = value; break; }
          case (int)QuaternionComponents.Y: { this.Y = value; break; }
          case (int)QuaternionComponents.Z: { this.Z = value; break; }
          case (int)QuaternionComponents.W: { this.W = value; break; }
          default: {
            throw new ArgumentOutOfRangeException("Vector component index is out of range");
          }
        }
      }
    }

    /// <summary>The components of the quaternion</summary>
    public Number<ScalarType, MathType> X, Y, Z, W;

    /// <summary>Construct a number wrapper for the generic value</summary>
    /// <param name="value">Value that is wrapped</param>
    /// <returns>The wrapped input value</returns>
    private static Number<ScalarType, MathType> n(ScalarType value) { return value; }

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

  }

} // namespace Nuclex.Math.Generic
