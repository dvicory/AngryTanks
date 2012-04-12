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

namespace Nuclex.Math.Interpolation {

#if false
  /// <summary>Performs linear interpolations</summary>
  /// <typeparam name="SampleType">Data type of the values being interpolated</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public class LinearInterpolator<SampleType, MathType> : IInterpolator<SampleType>
    where MathType :
      IArithmeticOperationsProvider<SampleType>,
      IComparisonProvider<SampleType>,
      IScalingProvider<SampleType>,
      new() {

    /// <summary>Initializes a linear interpolator with a sample provider</summary>
    /// <param name="sampleProvider">Sample provide to use for querying samples</param>
    public LinearInterpolator(ISampleProvider<SampleType> sampleProvider) {
      this.sampleProvider = sampleProvider;
    }

    /// <summary>Interpolates the value at the given point in time</summary>
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
      return Interpolate(left.Value, left.Time, right.Value, right.Time, time);
    }

    /// <summary>Interpolates between two values linearly</summary>
    /// <param name="left">Sample value at time 0.0</param>
    /// <param name="right">Sample value at time 1.0</param>
    /// <param name="time">Time index to be interpolated (0.0 to 1.0)</param>
    /// <returns>The linearly interpolated value</returns>
    public static SampleType Interpolate(
      Number<SampleType, MathType> left, Number<SampleType, MathType> right, double time
    ) {
      return left + math.Scale(right - left, time);
    }

    /// <summary>Interpolates between two arbitrily spaced samples linearly</summary>
    /// <param name="left">Sample value at the starting time</param>
    /// <param name="leftTime">Starting time</param>
    /// <param name="right">Sample value at the ending time</param>
    /// <param name="rightTime">Ending time</param>
    /// <param name="time">Time index to be interpolated</param>
    /// <returns>The linearly interpolated value</returns>
    public static SampleType Interpolate(
      Number<SampleType, MathType> left, double leftTime,
      Number<SampleType, MathType> right, double rightTime,
      double time
    ) {
      return Interpolate(left, right, (time - leftTime) / (rightTime - leftTime));
    }

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

    /// <summary>The provider used to query interpolation samples</summary>
    private ISampleProvider<SampleType> sampleProvider;

  }
#endif

} // namespace Nuclex.Math.Interpolation
