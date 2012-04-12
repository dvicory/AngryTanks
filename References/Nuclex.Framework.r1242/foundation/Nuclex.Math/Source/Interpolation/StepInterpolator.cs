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

namespace Nuclex.Math.Interpolation {

#if false
  /// <summary>Sample that the interpolator will step to</summary>
  public enum SampleAffinity {
    /// <summary>The interpolator always uses the left sample</summary>
    Left,
    /// <summary>The interpolator always uses the right sample</summary>
    Right,
    /// <summary>The interpolator uses the nearest sample</summary>
    Nearest
  }

  /// <summary>Performs a stepped interpolation</summary>
  /// <typeparam name="SampleType">Type of the value to be interpolated</typeparam>
  /// <typeparam name="SampleMathType">Math routines that are to be used</typeparam>
  public class StepInterpolator<TimeType, SampleType, SampleMathType> :
    IInterpolator<TimeType, SampleType>
    where SampleMathType :
      IArithmeticOperationsProvider<SampleType>,
      IComparisonProvider<SampleType>,
      IScalingProvider<SampleType>,
      new() {

    /// <summary>Initializes the step interpolator with a sample provider</summary>
    /// <param name="sampleProvider">Object through which to query for samples</param>
    public StepInterpolator(ISampleProvider<TimeType, SampleType> sampleProvider) :
      this(sampleProvider, SampleAffinity.Nearest) { }

    /// <summary>Initializes the step interpolator with a sample provider</summary>
    /// <param name="sampleProvider">Object through which to query for samples</param>
    /// <param name="affinity">Which sample to align steps to</param>
    public StepInterpolator(
      ISampleProvider<SampleType> sampleProvider, SampleAffinity affinity
    ) {
      this.sampleProvider = sampleProvider;
      this.affinity = affinity;
    }

    /// <summary>Interpolates the value expected at the given point in time</summary>
    /// <param name="time">Interpolation time</param>
    /// <returns>The interpolated value</returns>
    public SampleType Interpolate(TimeType time) {
    
      switch(this.affinity) {
        case SampleAffinity.Left: {
          // We can directly ask the sample provider for the left sample. If the time
          // lies before the left sample, it will also produce the right behavior
          // since sample providers are expected to clamp the input time.
          return this.sampleProvider.LocateSample(time, 0).Value;
        }

        case SampleAffinity.Left: {
          // We can directly ask the sample provider for the right sample. If the time
          // lies behind the right sample, it will also produce the right behavior
          // since sample providers are expected to clamp the input time.
          return this.sampleProvider.LocateSample(time, 1).Value;
        }
        
        case SampleAffinity.Nearest: {
          // Determine the sample left of the interpolation time. In case we are before
          // the firstmost sample we will directly return the first sample. This also
          // happens if the interpolation time exactly matches a sample and even
          // neatly catches rounding errors.
          Sample<TimeType, SampleType> left = this.sampleProvider.LocateSample(time, 0);
          if(time <= left.Time)
            return left.Value;

          // Determine the sample right of the interpolation time. In case we are after
          // the lastmost sample we will directly return the last sample. This also
          // happens if the interpolation time exactly matches a sample and even
          // neatly catches rounding errors.
          Sample<TimeType, SampleType> right = this.sampleProvider.LocateSample(time, 1);
          if(time >= right.Time)
            return right.Value;

          // If this point is reached, we are within the interpolation time range,
          // so we'll interpolate the value at the requested point in time now
          return Interpolate(
            left.Value, left.Time, right.Value, right.Time, time, this.affinity
          );
        }
      }

    }

    /// <summary>Perform a stepped interpolation of the value at the given point in time</summary>
    /// <param name="left">Sample value at time 0.0</param>
    /// <param name="right">Sample value at time 1.0</param>
    /// <param name="time">Time index to be interpolated (0.0 to 1.0)</param>
    /// <returns>The interpolated value</returns>
    public static SampleType Interpolate(
      Number<SampleType, SampleMathType> left, Number<SampleType, SampleMathType> right, double time
    ) {
      return Interpolate(left, right, time, SampleAffinity.Nearest);
    }

    /// <summary>Perform a stepped interpolation of the value at the given point in time</summary>
    /// <param name="left">Sample value at time 0.0</param>
    /// <param name="right">Sample value at time 1.0</param>
    /// <param name="time">Time index to be interpolated (0.0 to 1.0)</param>
    /// <param name="affinity">Which sample to align steps to</param>
    /// <returns>The interpolated value</returns>
    public static SampleType Interpolate(
      Number<SampleType, SampleMathType> left, Number<SampleType, SampleMathType> right,
      double time, SampleAffinity affinity
    ) {
      switch(affinity) {
        case SampleAffinity.Left: {
          return left;
        }
        case SampleAffinity.Right: {
          return right;
        }
        default: {
          if(time < 0.5) {
            return left;
          } else {
            return right;
          }
        }
      }
    }

    /// <summary>Performs a stepped interpolation between two arbitrily spaced samples</summary>
    /// <param name="left">Sample value at the starting time</param>
    /// <param name="leftTime">Starting time</param>
    /// <param name="right">Sample value at the ending time</param>
    /// <param name="rightTime">Ending time</param>
    /// <param name="time">Time index to be interpolated</param>
    /// <returns>The interpolated value</returns>
    public static SampleType Interpolate(
      Number<SampleType, SampleMathType> left, double leftTime,
      Number<SampleType, SampleMathType> right, double rightTime,
      double time
    ) {
      return Interpolate(left, right, (time - leftTime) / (rightTime - leftTime));
    }

    /// <summary>Performs a stepped interpolation between two arbitrily spaced samples</summary>
    /// <param name="left">Sample value at the starting time</param>
    /// <param name="leftTime">Starting time</param>
    /// <param name="right">Sample value at the ending time</param>
    /// <param name="rightTime">Ending time</param>
    /// <param name="time">Time index to be interpolated</param>
    /// <param name="affinity">Which sample to align steps to</param>
    /// <returns>The interpolated value</returns>
    public static SampleType Interpolate(
      Number<SampleType, SampleMathType> left, double leftTime,
      Number<SampleType, SampleMathType> right, double rightTime,
      double time, SampleAffinity affinity
    ) {
      return Interpolate(left, right, (time - leftTime) / (rightTime - leftTime), affinity);
    }

    /// <summary>Provides the interpolator with interpolation samples</summary>
    private ISampleProvider<TimeType, SampleType> sampleProvider;
    /// <summary>Which sample steps will be aligned to</summary>
    private SampleAffinity affinity;
  }
#endif


} // namespace Nuclex.Math.Interpolation
