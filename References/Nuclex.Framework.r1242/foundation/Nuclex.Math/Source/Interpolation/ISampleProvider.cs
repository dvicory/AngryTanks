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

  /// <summary>Interface used to dynamically query samples</summary>
  /// <typeparam name="TimeType">Data type of the time values</typeparam>
  /// <typeparam name="SampleType">Data type of the sample values</typeparam>
  public interface ISampleProvider<TimeType, SampleType> {

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
    Sample<TimeType, SampleType> LocateSample(TimeType time, int offset);

  }

  /// <summary>Interface used to dynamically query samples</summary>
  /// <typeparam name="SampleType">Data type of the sample values</typeparam>
  public interface ISampleProvider<SampleType> : ISampleProvider<double, SampleType> { }

} // namespace Nuclex.Math.Interpolation
