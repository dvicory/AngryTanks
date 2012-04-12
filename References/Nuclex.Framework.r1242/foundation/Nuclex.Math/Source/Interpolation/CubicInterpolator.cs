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
  /// <summary>Performs cubic interpolations</summary>
  /// <typeparam name="SampleType">Type of the value to be interpolated</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public class CubicInterpolator<SampleType, MathType> : IInterpolator<SampleType>
    where MathType :
      IArithmeticOperationsProvider<SampleType>,
      IComparisonProvider<SampleType>,
      IScalingProvider<SampleType>,
      new() {

    /// <summary>Hermite interpolation polynom</summary>
    public static readonly double[,] HermiteMatrix = new double[4, 4] {
      {  2, -2,  1,  1 },
      { -3,  3, -2, -1 },
      {  0,  0,  1,  0 },
      {  1,  0,  0,  0 }
    };

    /// <summary>Bezier interpolation polynom</summary>
    public static readonly double[,] BezierMatrix = new double[4, 4] {
      { -1,  3, -3,  1 },
      {  3, -6,  3,  0 },
      { -3,  3,  0,  0 },
      {  1,  0,  0,  0 }
    };

    /// <summary>Overhauser interpolation polynom</summary>
    public static readonly double[,] OverhauserMatrix = new double[4, 4] {
      { -1,  3, -3,  1 },
      {  2, -5,  4,  1 },
      { -1,  0,  1,  0 },
      {  0,  2,  0,  0 }
    };

    /// <summary>Initializes the cubic interpolator with a sample provider</summary>
    /// <param name="sampleProvider">Object through which to query for samples</param>
    public CubicInterpolator(ISampleProvider<SampleType> sampleProvider) :
      this(sampleProvider, HermiteMatrix) { }

    /// <summary>Initializes the cubic interpolator with a custom interpolation polynom</summary>
    /// <param name="sampleProvider">Object through which to obtain samples</param>
    /// <param name="matrix">Matrix containing the interpolation polynom</param>
    public CubicInterpolator(
      ISampleProvider<SampleType> sampleProvider,
      double[/*4*/, /*4*/] matrix
    ) {
      this.sampleProvider = sampleProvider;

      validateMatrix(matrix);
      this.matrix = matrix;
    }

    /// <summary>Matrix containing the interpolation polynom that is used</summary>
    public double[/*4*/, /*4*/] Matrix {
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

      Sample<SampleType> beforeLeft = this.sampleProvider.LocateSample(time, -1);

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

      Sample<SampleType> afterRight = this.sampleProvider.LocateSample(time, 2);

      // To simplify calculated we'll package the samples in some variables
      Number<SampleType, MathType> beforeLeftValue = beforeLeft.Value;
      Number<SampleType, MathType> leftValue = left.Value;
      Number<SampleType, MathType> rightValue = right.Value;
      Number<SampleType, MathType> afterRightValue = afterRight.Value;

      // Determine the tangent at the left sample. If the left sample was the
      // first sample in the interpolation set, we use a tagent that would continue the
      // path set by the two samples we have so as to not interpolate towards 0
      SampleType leftTangent = rightValue - leftValue;
      if(beforeLeft.Time < left.Time) {
        leftTangent = math.Add(
          math.Scale(leftValue - beforeLeftValue, 0.5 / (left.Time - beforeLeft.Time)),
          math.Scale(rightValue - leftValue, 0.5 / (left.Time - beforeLeft.Time))
        );
      }

      // Determine the tangent at the right sample. If the right sample was the
      // last sample in the interpolation set, we use a tagent that would continue the
      // path set by the two samples we have so as to not interpolate towards 0
      SampleType rightTangent = rightValue - leftValue;
      if(afterRight.Time > right.Time) {
        rightTangent = math.Add(
          math.Scale(rightValue - leftValue, 0.5 / (afterRight.Time - right.Time)),
          math.Scale(afterRightValue - rightValue, 0.5 / (afterRight.Time - right.Time))
        );
      }

      // For the polynom we need the time, squared time and cubed time
      time = (time - left.Time) / (right.Time - left.Time);
      double time2 = time * time; // time²
      double time3 = time2 * time; // time³

      // Calculate the interpolated value
      return math.Add(
        math.Add(
          math.Scale(
            left.Value,
            this.matrix[0, 0] * time3 +
              this.matrix[1, 0] * time2 +
              this.matrix[2, 0] * time +
              this.matrix[3, 0]
          ),
          math.Scale(
            right.Value,
            this.matrix[0, 1] * time3 +
              this.matrix[1, 1] * time2 +
              this.matrix[2, 1] * time +
              this.matrix[3, 1]
          )
        ),
        math.Add(
          math.Scale(
            leftTangent,
            this.matrix[0, 2] * time3 +
              this.matrix[1, 2] * time2 +
              this.matrix[2, 2] * time +
              this.matrix[3, 2]
          ),
          math.Scale(
            rightTangent,
            this.matrix[0, 3] * time3 +
              this.matrix[1, 3] * time2 +
              this.matrix[2, 3] * time +
              this.matrix[3, 3]
          )
        )
      );

    }

    /// <summary>Validates that a matrix is valid to be used as interpolation polynom</summary>
    /// <param name="matrix">Matrix which is to be validated</param>
    private static void validateMatrix(double[,] matrix) {
      bool isValidMatrix =
        (matrix.GetLowerBound(0) == 0) &&
        (matrix.GetUpperBound(0) == 3) &&
        (matrix.GetLowerBound(1) == 0) &&
        (matrix.GetUpperBound(1) == 3);

      if(!isValidMatrix) {
        throw new ArgumentException("Interpolation polynome is not a 4 by 4 matrix");
      }
    }

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

    /// <summary>Provides the interpolator with interpolation samples</summary>
    private ISampleProvider<SampleType> sampleProvider;
    /// <summary>Matrix containing the interpolation polynom</summary>
    private double[/*4*/, /*4*/] matrix;
  }
#endif

} // namespace Nuclex.Math.Interpolation
