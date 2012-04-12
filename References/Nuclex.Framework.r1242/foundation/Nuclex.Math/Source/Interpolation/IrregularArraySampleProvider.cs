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

namespace Nuclex.Math.Interpolation {
#if false
  /// <summary>Provides samples from an array of arbitrarily spaced samples</summary>
  /// <typeparam name="SampleType">Data type of the sample values</typeparam>
  public class IrregularArraySampleProvider<SampleType> : ISampleProvider<SampleType> {

    /// <summary>Initializes the irregular array sample provider</summary>
    /// <param name="samples">An array containing all the samples for the provider</param>
    public IrregularArraySampleProvider(Sample<TimeType, SampleType>[] samples) {
      this.samples = samples;
    }

    /// <summary>Initializes the sample provider from a time and a sample array</summary>
    /// <param name="times">Array containing the times of the sample values</param>
    /// <param name="values">Values being prevalent at the associated times</param>
    public IrregularArraySampleProvider(double[] times, SampleType[] values) {

      // Make sure both arrays are of equal size
      if((times.GetLowerBound(0) != values.GetLowerBound(0)) ||
         (times.GetUpperBound(0) != values.GetUpperBound(0)))
        throw new ArgumentException("Arrays do not have the same bounds");

      // Both arrays have to be mixed into one big array of samples
      this.samples = new Sample<TimeType, SampleType>[values.Length];
      for(int sample = 0; sample < values.Length; ++sample) {
        this.samples[sample].Time = times[sample];
        this.samples[sample].Value = values[sample];
      }

    }

    /// <summary>Queries for a sample</summary>
    /// <param name="time">Time of which to determine the sample</param>
    /// <param name="offset">
    ///    An offset of 0 means that the sample directly before or at the specified
    ///    time is returned. The sample provider is supposed to always refer to the
    ///    left (the past) sample, not to look for the nearest sample or something.
    ///    Offsets other than 0 are relative to this sample, +1 means the sample to
    ///    the right of that and -1 the sample to the left.
    /// </param>
    /// <returns>The sample relative to the specified time and offset</returns>
    public Sample<SampleType> LocateSample(double time, int offset) {
      int index;

      // Small optimization because typically interpolators will query multiple
      // offsets based on the same time index.
      if(time != this.lastAccessedTime) {

        // Create a temporary sample for the reference time
        Sample<SampleType> reference = new Sample<SampleType>();
        reference.Time = time;

        // MSDN states that BinarySearch works even when the searched value doesn't exist
        //
        //   Returns:
        //     "If value is not found and value is less than one or more elements in array,
        //      a negative number which is the bitwise complement of the index of the first
        //      element that is larger than value"
        index = Array.BinarySearch<Sample<SampleType>>(this.samples, reference);
        if(index < 0)
          index = ~index - 1;

        // Store the informations we found
        this.lastAccessedTime = time;
        this.lastAccessedIndex = index;

      } else {

        // Same time as before; use the cached index
        index = this.lastAccessedIndex;

      }

      return this.samples[
        System.Math.Min(System.Math.Max(index + offset, 0), this.samples.Length - 1)
      ];
    }

    /// <summary>Array containing the samples provided by this provider</summary>
    private Sample<SampleType>[] samples;
    /// <summary>Time that was queried for most recently</summary>
    private double lastAccessedTime;
    /// <summary>Index that has been determined in the most recent query</summary>
    private int lastAccessedIndex;

  }
#endif

} // namespace Nuclex.Math.Interpolation
