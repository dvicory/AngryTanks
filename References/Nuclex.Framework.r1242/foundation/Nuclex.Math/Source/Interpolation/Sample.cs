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

  /// <summary>Interpolation sample</summary>
  /// <typeparam name="TimeType">Data type of the time values</typeparam>
  /// <typeparam name="SampleType">Data type being interpolated</typeparam>
  public struct Sample<TimeType, SampleType> {

    /// <summary>Initializes thesample</summary>
    /// <param name="value">Value at the given time</param>
    /// <param name="time">Time at which the specified value is prevalent</param>
    public Sample(TimeType time, SampleType value) {
      this.Time = time;
      this.Value = value;
    }

    /// <summary>Time at which this sample is located</summary>
    public TimeType Time;

    /// <summary>Interpolation sample value</summary>
    public SampleType Value;

  }

} // namespace Nuclex.Math.Interpolation
