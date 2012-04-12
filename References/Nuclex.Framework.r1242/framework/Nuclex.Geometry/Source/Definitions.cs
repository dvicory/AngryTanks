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
using Microsoft.Xna.Framework;

namespace Nuclex.Geometry {

  /// <summary>Sides of a plane in 3D space</summary>
  public enum Side {

    /// <summary>Negative half space (away from the plane's normal vector)</summary>
    Negative = -1,
    /// <summary>Positive half space (same side as the plane's normal vector)</summary>
    Positive = +1,

  }

  /// <summary>Targetted specifications of the library</summary>
  internal static class Specifications {

    /// <summary>Maximum allowed deviation from perfect accuracy</summary>
    /// <remarks>
    ///   This value indicates the maximum error that may be introduced with any given
    ///   calculation. If it is exceeded, the algorithm that produced the result should
    ///   be checked for numerical stability.
    /// </remarks>
    //public const float MaximumDeviation = 0.00025f;
    public const int MaximumDeviation = 2; // representable floating point numbers

    /// <summary>Distance where intersection tests do not give stable results</summary>
    /// <remarks>
    ///   If two geometrical objects are very close to each other, floating point
    ///   inaccuracies can lead to unstable results for intersection tests. This value
    ///   indicates how close two objects need to be for this to occur.
    /// </remarks>
    public const float HullAccuracy = MaximumDeviation;

    /// <summary>Number of samples used to unit-test probabilistic functions</summary>
    /// <remarks>
    ///   Some functions are intended to return randomness, like all variants of the
    ///   RandomPointOnSurface() method for geometric volumes. To unit-test these
    ///   functions, the best way is to generate a large number of random samples and
    ///   then see if certain criteria of these points are met (containment, average
    ///   value and more).
    /// </remarks>
    public const int ProbabilisticFunctionSamples = 1024;

    /// <summary>Deviation allowed for probabilistic function unit tests</summary>
    /// <remarks>
    ///   Acceptable deviation of the averaged samples from the perfect median value.
    ///   The more samples you run, the less deviation occurs.
    /// </remarks>
    public const float ProbabilisticFunctionDeviation = 0.1f;

  }

} // namespace Nuclex.Geometry
