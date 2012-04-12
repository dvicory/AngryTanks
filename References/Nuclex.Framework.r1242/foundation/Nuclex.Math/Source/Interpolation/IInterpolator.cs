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

  /// <summary>Interface for interpolators</summary>
  /// <typeparam name="TimeType">
  ///   Data type of the time specifications provided to the interpolator
  /// </typeparam>
  /// <typeparam name="SampleType">
  ///   Data type being interpolated by the interpolator
  /// </typeparam>
  public interface IInterpolator<TimeType, SampleType> {

    /// <summary>Interpolates the value expected for the given point in time</summary>
    /// <param name="time">Interpolation time</param>
    /// <returns>The interpolated value</returns>
    SampleType Interpolate(TimeType time);

  }

  /// <summary>Interface for interpolators</summary>
  /// <typeparam name="SampleType">
  ///   Data type being interpolated by the interpolator
  /// </typeparam>
  public interface IInterpolator<SampleType> : IInterpolator<double, SampleType> { }

} // namespace Nuclex.Math.Interpolation
