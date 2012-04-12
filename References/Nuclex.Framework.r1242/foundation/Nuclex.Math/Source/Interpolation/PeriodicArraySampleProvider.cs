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

  /// <summary>Provides samples from a regularly spaced sample array</summary>
  /// <typeparam name="TimeType">Data type of the time values</typeparam>
  /// <typeparam name="SampleType">Data type of the sample values</typeparam>
  /// <typeparam name="TimeMathType">Math routines that are to be used</typeparam>
  public class PeriodicArraySampleProvider<TimeType, SampleType, TimeMathType> :
    ISampleProvider<TimeType, SampleType>
    where TimeMathType :
      IArithmeticOperationsProvider<TimeType>,
      IComparisonProvider<TimeType>,
      IIntConversionProvider<TimeType>,
      new() {

    /// <summary>Initializes the periodally spaced sample provider</summary>
    /// <param name="samples">Array containing the sample values</param>
    /// <param name="interval">Time between each two samples</param>
    /// <param name="start">Point in time where the first sample is located</param>
    public PeriodicArraySampleProvider(
      SampleType[] samples, TimeType interval, TimeType start
    ) {
      if(!math.IsPositive(interval))
        throw new ArgumentException("Interval has to be a positive non-negative value");

      this.samples = samples;
      this.interval = interval;
      this.start = start;
    }

    /// <summary>Queries for a sample</summary>
    /// <param name="time">Time of which to determine the sample</param>
    /// <param name="offset">
    ///    An offset of 0 means that the sample directly before or at the specified
    ///    time is returned. The sample provider is supposed to always refer to the
    ///    left (the past) sample, not to look for the nearest sample of something.
    ///    Offsets other than 0 are relative to this sample, +1 means the sample to
    ///    the right of that and -1 the sample to the left.
    /// </param>
    /// <returns>The sample relative to the specified time and offset</returns>
    public Sample<TimeType, SampleType> LocateSample(TimeType time, int offset) {
      int index = math.ToInteger(
        math.Divide(math.Subtract(time, this.start), this.interval)
      );

      // Clamp the index to our sample array length
      index = System.Math.Min(System.Math.Max(index + offset, 0), this.samples.Length - 1);

      return new Sample<TimeType, SampleType>(
        math.Add(math.Multiply(math.FromInteger(index), this.interval), this.start),
        this.samples[index]
      );
    }

    /// <summary>Array containing the samples provided by this sample provider</summary>
    private SampleType[] samples;
    /// <summary>Time index of the first sample</summary>
    private TimeType start;
    /// <summary>Interval from one sample to the next</summary>
    private TimeType interval;

    /// <summary>Instance of the math package we are using</summary>
    private static TimeMathType math = Shared<TimeMathType>.Instance;

  }

} // namespace Nuclex.Math.Interpolation
