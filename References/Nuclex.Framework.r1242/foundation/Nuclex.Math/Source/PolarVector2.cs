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

using Nuclex.Math.Generic;
using Nuclex.Support;

namespace Nuclex.Math {

  /// <summary>The components of a 2D vector</summary>
  public enum PolarVector2Components : int {
    /// <summary>The radius from the coordinate system's center</summary>
    Radius,
    /// <summary>The angle of the vector</summary>
    Phi
  }

  /// <summary>2D vector in polar coordinates</summary>
  /// <typeparam name="ScalarType">Type of the the vector uses for its radius</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public struct PolarVector2<ScalarType, MathType>
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
    public static PolarVector2<ScalarType, MathType> Zero {
      get { return new PolarVector2<ScalarType, MathType>(); }
    }

    /// <summary>A unit-length vector pointing right</summary>
    public static PolarVector2<ScalarType, MathType> Right {
      get { return new PolarVector2<ScalarType, MathType>(0.0, math.One); }
    }

    /// <summary>A unit-length vector pointing upwards</summary>
    public static PolarVector2<ScalarType, MathType> Up {
      get { return new PolarVector2<ScalarType, MathType>(System.Math.PI / 2.0, math.One); }
    }

    /// <summary>Normalizes the vector to a length of 1</summary>
    /// <param name="vector">Vector to be normalized</param>
    /// <returns>The normalized vector</returns>
    public static PolarVector2<ScalarType, MathType> Normalize(
      PolarVector2<ScalarType, MathType> vector
    ) {
      return new PolarVector2<ScalarType, MathType>(vector.Phi, math.One);
    }

    /// <summary>Rotates the vector by an arbitrary angle</summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Rotation angle</param>
    /// <returns>The rotated vector</returns>
    public static PolarVector2<ScalarType, MathType> Rotate(
      PolarVector2<ScalarType, MathType> vector, double angle
    ) {
      return new PolarVector2<ScalarType, MathType>(
        Shared<UncheckedScalarMath>.Instance.Wrap(
          vector.Phi + angle, 0.0, Trigonometry.Perigon
        ),
        vector.Radius
      );
    }

    /// <summary>Converts a normal cartesian vector into a polar vector</summary>
    /// <param name="vector">Cartesian vector to convert</param>
    /// <returns>An equivalent polar vector</returns>
    public static explicit operator PolarVector2<ScalarType, MathType>(
      Vector2<ScalarType, MathType> vector
    ) {
      return new PolarVector2<ScalarType, MathType>(
        math.Atan2(vector.Y, vector.X),
        vector.Length
      );
    }

    /// <summary>Converts a polar vector into a normal cartesian vector</summary>
    /// <param name="vector">Polar vector to convert</param>
    /// <returns>An equivalent cartesian vector</returns>
    public static explicit operator Vector2<ScalarType, MathType>(
      PolarVector2<ScalarType, MathType> vector
    ) {
      return Vector2<ScalarType, MathType>.Rotate(
        Vector2<ScalarType, MathType>.Right,
        vector.Phi
      ) * vector.Radius;
    }

    /// <summary>Constructs a new vector from the specified components</summary>
    /// <param name="phi">Direction at which the vector points</param>
    /// <param name="radius">Radius of the vector from the coordinate system's center</param>
    public PolarVector2(double phi, ScalarType radius) {
      Phi = phi;
      Radius = radius;
    }

    /// <summary>The length of the vector (cheaper to calculate than the squared length)</summary>
    public ScalarType Length {
      get { return Radius; }
    }

    /// <summary>The squared length of the vector </summary>
    public ScalarType SquaredLength {
      get { return Radius * Radius; }
    }

    /// <summary>The vector clockwise rotated by 90 degrees</summary>
    public PolarVector2<ScalarType, MathType> Perpendicular {
      get {
        return PolarVector2<ScalarType, MathType>.Rotate(this, Trigonometry.Perigon / 2.0);
      }
    }

    /// <summary>The vector coounter clockwise rotated by 90 degrees</summary>
    public PolarVector2<ScalarType, MathType> InvPerpendicular {
      get {
        return PolarVector2<ScalarType, MathType>.Rotate(this, -Trigonometry.Perigon / 2.0);
      }
    }

    /// <summary>Determines if an object is identical to the vector</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the vector</returns>
    public override bool Equals(object obj) {
      if(obj is PolarVector2<ScalarType, MathType>)
        return this == (PolarVector2<ScalarType, MathType>)obj;
      else
        return false;
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      return Phi.GetHashCode() + Radius.GetHashCode();
    }

    /// <summary>Returns a vector that points into the exact oppsite direction</summary>
    /// <param name="vector">Vector of which to return the opposite direction</param>
    /// <returns>The inverted vector</returns>
    public static PolarVector2<ScalarType, MathType> operator -(
      PolarVector2<ScalarType, MathType> vector
    ) {
      return PolarVector2<ScalarType, MathType>.Rotate(vector, Trigonometry.Perigon);
    }

    /// <summary>Calculates the product of a vector and a scalar</summary>
    /// <param name="vector">First factor of the multiplication</param>
    /// <param name="factor">Scalar value by which to scale the vector</param>
    /// <returns>The product of the multiplication of the vector with the scalar</returns>
    public static PolarVector2<ScalarType, MathType> operator *(
      PolarVector2<ScalarType, MathType> vector, ScalarType factor
    ) {
      return new PolarVector2<ScalarType, MathType>(vector.Phi, vector.Radius * factor);
    }

    /// <summary>Divides a vector by a scalar</summary>
    /// <param name="vector">Vector to be divided</param>
    /// <param name="divisor">The divisor</param>
    /// <returns>The quotient of the vector and the scalar</returns>
    public static PolarVector2<ScalarType, MathType> operator /(
      PolarVector2<ScalarType, MathType> vector, ScalarType divisor
    ) {
      return new PolarVector2<ScalarType, MathType>(vector.Phi, vector.Radius / divisor);
    }

    /// <summary>Determines if two vectors are equal</summary>
    /// <param name="first">First vector to be compared</param>
    /// <param name="second">Second vector to be compared</param>
    /// <returns>True if both vectors are equal</returns>
    public static bool operator ==(
      PolarVector2<ScalarType, MathType> first, PolarVector2<ScalarType, MathType> second
    ) {
      return (first.Phi == second.Phi) && (first.Radius == second.Radius);
    }

    /// <summary>Determines if two vectors are unequal</summary>
    /// <param name="first">First vector to be compared</param>
    /// <param name="second">Second vector to be compared</param>
    /// <returns>True if both vectors are unequal</returns>
    public static bool operator !=(
      PolarVector2<ScalarType, MathType> first, PolarVector2<ScalarType, MathType> second
    ) {
      return (first.Phi != second.Phi) || (first.Radius != second.Radius);
    }

    /// <summary>The phi component of the vector</summary>
    public double Phi;
    /// <summary>The radius component of the vector</summary>
    public Number<ScalarType, MathType> Radius;

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

  }

} // namespace Nuclex.Math
