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

namespace Nuclex.Math {

  /// <summary>Possible paths to take between to angles</summary>
  public enum TangentialPath {

    /// <summary>Path in positive direction (from begin to end)</summary>
    Positive,
    /// <summary>Path in negative direction (from end to begin)</summary>
    Negative,
    /// <summary>Shortest path</summary>
    Shortest

  }

  /// <summary>Trigonometrical helper methods</summary>
  /// <remarks>
  ///   Unless otherwise specified, all methods taking angles work in radians.
  ///   A direction given in radians is commonly referred to by "phi" in this context.
  /// </remarks>
  public static class Trigonometry {

    /// <summary>Angle in radians of a full circle</summary>
    public const double Perigon = System.Math.PI * 2.0;

    /// <summary>Factor used to convert from degrees to radians</summary>
    public const double RadiansPerDegree = System.Math.PI / 180.0;

    /// <summary>Factor used to convert from radians to degrees</summary>
    public const double DegreesPerRadian = 180.0 / System.Math.PI;

    /// <summary>Converts an angle from radians to degrees</summary>
    /// <param name="radians">Angle in radians that will be converted</param>
    /// <returns>The input angle in degrees</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static double ToDegrees(double radians) {
      return radians * 180.0 / System.Math.PI;
    }

    /// <summary>Converts an angle from radians to degrees</summary>
    /// <param name="radians">Angle in radians that will be converted</param>
    /// <returns>The input angle in degrees</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static float ToDegrees(float radians) {
      return radians * 180.0f / (float)System.Math.PI;
    }

    /// <summary>Converts an angle from degrees to radians</summary>
    /// <param name="degrees">Angle in degrees that will be converted</param>
    /// <returns>The input angle in radians</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static double ToRadians(double degrees) {
      return degrees * System.Math.PI / 180.0;
    }

    /// <summary>Converts an angle from degrees to radians</summary>
    /// <param name="degrees">Angle in degrees that will be converted</param>
    /// <returns>The input angle in radians</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static float ToRadians(float degrees) {
      return degrees * (float)System.Math.PI / 180.0f;
    }

    /// <summary>Calculates the angle between two absolute directions</summary>
    /// <param name="firstPhi">First absolute direction in radians</param>
    /// <param name="secondPhi">Second absolute direction in radians</param>
    /// <returns>The angle between both directions</returns>
    public static double AngleBetween(double firstPhi, double secondPhi) {
      return AngleBetween(firstPhi, secondPhi, TangentialPath.Shortest);
    }

    /// <summary>Calculates the angle between two absolute directions</summary>
    /// <param name="firstPhi">First absolute direction in radians</param>
    /// <param name="secondPhi">Second absolute direction in radians</param>
    /// <param name="path">Path to take from first to second</param>
    /// <returns>The angle between both directions using the desired path</returns>
    /// <remarks>
    ///   See the remarks on the TangentialPath enumeration to determine the exact
    ///   behavior of this function. A positive path is always from the first angle
    ///   to the second, independent of whether this is clockwise or counter-clockwise.
    /// </remarks>
    public static double AngleBetween(double firstPhi, double secondPhi, TangentialPath path) {
      double difference = secondPhi - firstPhi;

      double angle;
      if(path == TangentialPath.Shortest) {

        // Just unroll the difference of both angles and we have the angle
        angle = Shared<UncheckedScalarMath>.Instance.Wrap(difference, 0.0, Perigon);

        // Calculate angle of shortest path if we got the one of the longest path
        if(angle >= System.Math.PI)
          angle -= Perigon;

      } else {

        // Unroll the difference depending on the sign of the resulting angle
        if(difference < 0.0) {

          angle = math.Wrap(difference, -Perigon, 0.0);
          if(path == TangentialPath.Negative)
            angle += Perigon;

        } else {

          angle = math.Wrap(difference, 0.0, Perigon);
          if(path == TangentialPath.Negative)
            angle -= Perigon;

        }
      }

      return angle;
    }

    /// <summary>Instance of the math package we are using</summary>
    private static UncheckedScalarMath math = Shared<UncheckedScalarMath>.Instance;

  }

} // namespace Nuclex.Math
