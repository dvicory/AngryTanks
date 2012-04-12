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

  /// <summary>Trigonometrical operations on a given data type</summary>
  /// <typeparam name="ScalarType">
  ///   Type for which trigonometrical operations are provided
  /// </typeparam>
  /// <remarks>
  ///   Throughout this library, angles are always expressed as doubles, so only
  ///   cartesian coordinates which might be the result of these operations
  ///   actually use the generic data type.
  /// </remarks>
  public interface ITrigonometricsProvider<ScalarType> {

    /// <summary>Calculates the sine of the specified angle</summary>
    /// <param name="angle">Angle whose sine is calculated</param>
    /// <returns>The sine of the provided angle</returns>
    ScalarType Sin(double angle);

    /// <summary>Calculates the cosine of the specified angle</summary>
    /// <param name="angle">Angle whose cosine is calculated</param>
    /// <returns>The cosine of the provided angle</returns>
    ScalarType Cos(double angle);

    /// <summary>Calculates the arcus sine of the specified angle</summary>
    /// <param name="angle">Angle whose arcus sine is calculated</param>
    /// <returns>The arcus sine of the provided angle</returns>
    ScalarType Asin(double angle);

    /// <summary>Calculates the arcus cosine of the specified angle</summary>
    /// <param name="angle">Angle whose arcus cosine is calculated</param>
    /// <returns>The arcus cosine of the provided angle</returns>
    ScalarType Acos(double angle);

    /// <summary>Calculates the tangent of the specified angle</summary>
    /// <param name="value">Angle whose tangent is calculated</param>
    /// <returns>The tangent of the provided angle</returns>
    ScalarType Tan(ScalarType value);

    /// <summary>Calculates the arcus tangent of the specified angle</summary>
    /// <param name="value">Angle whose arcus tangent is calculated</param>
    /// <returns>The arcus tangent of the provided angle</returns>
    ScalarType Atan(ScalarType value);

    /// <summary>Calculates the arcus tangent of the provided vector</summary>
    /// <param name="y">Length of the vector on the Y axis</param>
    /// <param name="x">Length of the vector on the X axis</param>
    /// <returns>The tangent of the input vector</returns>
    double Atan2(ScalarType y, ScalarType x);

  }

} // namespace Nuclex.Math.Generic
