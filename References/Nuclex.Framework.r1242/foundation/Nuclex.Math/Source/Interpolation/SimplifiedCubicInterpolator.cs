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

namespace Nuclex.Math.Interpolation {
                  
#if false
  /// <summary>Performs quadratic interpolations</summary>
  /// <typeparam name="SampleType">Type of the value to be interpolated</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public class SimplifiedCubicInterpolator<SampleType, MathType> : IInterpolator<SampleType>
    where MathType :
      IArithmeticOperationsProvider<SampleType>,
      IComparisonProvider<SampleType>,
      IScalingProvider<SampleType>,
      new() {

    /// <summary>Simplified hermite interpolation polynom</summary>
    public static readonly double[,] SimplifiedHermiteMatrix = new double[4, 2] {
      {  2, -2 },
      { -3,  3 },
      {  0,  0 },
      {  1,  0 }
    };

    /// <summary>Initializes the simplified cubic interpolator with a sample provider</summary>
    /// <param name="sampleProvider">Object through which to query for samples</param>
    public SimplifiedCubicInterpolator(ISampleProvider<SampleType> sampleProvider) :
      this(sampleProvider, SimplifiedHermiteMatrix) { }

    /// <summary>Initializes the simplified cubic interpolator with a sample provider</summary>
    /// <param name="sampleProvider">Object through which to query for samples</param>
    /// <param name="matrix">Matrix containing the interpolation polynom</param>
    public SimplifiedCubicInterpolator(
      ISampleProvider<SampleType> sampleProvider, double[/*4*/, /*2*/] matrix
    ) {
      validateMatrix(matrix);

      this.sampleProvider = sampleProvider;
      this.matrix = matrix;
    }

    /// <summary>Matrix containing the interpolation polynom that is used</summary>
    public double[/*4*/, /*2*/] Matrix {
      get {
        return this.matrix;
      }
      set {
        validateMatrix(value);
        this.matrix = value;
      }
    }

    /// <summary>Interpolates the value expected at the given point in time</summary>
    /// <param name="time">Interpolation time</param>
    /// <returns>The interpolated value</returns>
    public SampleType Interpolate(double time) {

      // Determine the sample left of the interpolation time. In case we are before
      // the firstmost sample we will directly return the first sample. This also
      // happens if the interpolation time exactly matches a sample and even
      // neatly catches rounding errors.
      Sample<SampleType> left = this.sampleProvider.LocateSample(time, 0);
      if(time <= left.Time)
        return left.Value;

      // Determine the sample right of the interpolation time. In case we are after
      // the lastmost sample we will directly return the last sample. This also
      // happens if the interpolation time exactly matches a sample and even
      // neatly catches rounding errors.
      Sample<SampleType> right = this.sampleProvider.LocateSample(time, 1);
      if(time >= right.Time)
        return right.Value;

      // If this point is reached, we are within the interpolation time range,
      // so we'll interpolate the value at the requested point in time now
      return Interpolate(this.matrix, left.Value, left.Time, right.Value, right.Time, time);

    }

    /// <summary>Perform a quadratic interpolation of the value at the given point in time</summary>
    /// <param name="matrix">Matrix containing the interpolation polynom</param>
    /// <param name="left">Sample value at time 0.0</param>
    /// <param name="right">Sample value at time 1.0</param>
    /// <param name="time">Time index to be interpolated (0.0 to 1.0)</param>
    /// <returns>The quadratically interpolated value</returns>
    public static SampleType Interpolate(
      double[/*4*/, /*2*/] matrix,
      Number<SampleType, MathType> left,
      Number<SampleType, MathType> right,
      double time
    ) {
      validateMatrix(matrix);

      // For the polynom we need the time, squared time and cubed time
      double time2 = time * time; // time²
      double time3 = time2 * time; // time³

      // Calculate the interpolated value
      return math.Add(
        math.Scale(
          left,
          matrix[0, 0] * time3 + matrix[1, 0] * time2 + matrix[2, 0] * time + matrix[3, 0]
        ),
        math.Scale(
          right,
          matrix[0, 1] * time3 + matrix[1, 1] * time2 + matrix[2, 1] * time + matrix[3, 1]
        )
      );
    }

    /// <summary>Performs a quadratic interpolation between two arbitrily spaced samples</summary>
    /// <param name="matrix">Matrix containing the interpolation polynom</param>
    /// <param name="left">Sample value at the starting time</param>
    /// <param name="leftTime">Starting time</param>
    /// <param name="right">Sample value at the ending time</param>
    /// <param name="rightTime">Ending time</param>
    /// <param name="time">Time index to be interpolated</param>
    /// <returns>The quadratically interpolated value</returns>
    public static SampleType Interpolate(
      double[/*4*/, /*2*/] matrix,
      Number<SampleType, MathType> left, double leftTime,
      Number<SampleType, MathType> right, double rightTime,
      double time
    ) {
      validateMatrix(matrix);
      return Interpolate(matrix, left, right, (time - leftTime) / (rightTime - leftTime));
    }

    /// <summary>Validates that a matrix is valid to be used as interpolation polynom</summary>
    /// <param name="matrix">Matrix which is to be validated</param>
    private static void validateMatrix(double[,] matrix) {
      bool isValidMatrix =
        (matrix.GetLowerBound(0) == 0) &&
        (matrix.GetUpperBound(0) == 3) &&
        (matrix.GetLowerBound(1) == 0) &&
        (matrix.GetUpperBound(1) >= 2);

      if(!isValidMatrix) {
        throw new ArgumentException("Interpolation polynome is not a 4 by 2+ matrix");
      }
    }

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

    /// <summary>Provides the interpolator with interpolation samples</summary>
    private ISampleProvider<SampleType> sampleProvider;
    /// <summary>Matrix containing the interpolation polynom</summary>
    private double[/*4*/, /*2*/] matrix;
  }
#endif

} // namespace Nuclex.Math.Interpolation
