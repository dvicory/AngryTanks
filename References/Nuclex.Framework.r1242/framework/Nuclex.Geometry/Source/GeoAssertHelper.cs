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

#if UNITTEST

using System;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Nuclex.Support;

// Decide:
// - Move (part of) this to Nuclex.Support?
// - Create new Assemblies Nuclex.NUnit.dll and Nuclex.NUnit.Xna.dll?

namespace Nuclex.Geometry {

  /// <summary>Contains assertions on geometrical objects</summary>
  public static class GeoAssertHelper {

    /// <summary>Ensures that two vectors are equal</summary>
    /// <param name="expected">Expected vector</param>
    /// <param name="actual">Actual vector</param>
    /// <param name="deltaUlps">Allowed deviation in representable floating point values</param>
    public static void AreAlmostEqual(Vector3 expected, Vector3 actual, int deltaUlps) {
      AreAlmostEqual(expected, actual, deltaUlps, null);
    }

    /// <summary>Ensures that two vectors are equal</summary>
    /// <param name="expected">Expected vector</param>
    /// <param name="actual">Actual vector</param>
    /// <param name="deltaUlps">Allowed deviation in representable floating point values</param>
    /// <param name="message">Message to display when the vectors are not equal</param>
    public static void AreAlmostEqual(
      Vector3 expected, Vector3 actual, int deltaUlps, string message
    ) {

      bool almostEqual =
        FloatHelper.AreAlmostEqual(expected.X, actual.X, deltaUlps) &&
        FloatHelper.AreAlmostEqual(expected.Y, actual.Y, deltaUlps) &&
        FloatHelper.AreAlmostEqual(expected.Z, actual.Z, deltaUlps);

      if(almostEqual)
        return;

      // Now we already know that the two vectors are not equal even within the allowed
      // deviation (delta argument). In order to force NUnit to output a good error
      // message, we now let NUnit do the job again fully well knowing that it will
      // fail. This allows for deltas and good NUnit integration at the same time.
      Assert.AreEqual(expected, actual, message);

    }

    /// <summary>Ensures that two axis aligned boxes are equal</summary>
    /// <param name="expected">Expected box</param>
    /// <param name="actual">Actual box</param>
    /// <param name="deltaUlps">Allowed deviation in representable floating point values</param>
    public static void AreAlmostEqual(
      Volumes.AxisAlignedBox3 expected, Volumes.AxisAlignedBox3 actual, int deltaUlps
    ) {
      AreAlmostEqual(expected, actual, deltaUlps, null);
    }

    /// <summary>Ensures that two axis aligned boxes are equal</summary>
    /// <param name="expected">Expected box</param>
    /// <param name="actual">Actual box</param>
    /// <param name="deltaUlps">Allowed deviation in representable floating point values</param>
    /// <param name="message">Message to display when the boxes are not equal</param>
    public static void AreAlmostEqual(
      Volumes.AxisAlignedBox3 expected, Volumes.AxisAlignedBox3 actual,
      int deltaUlps, string message
    ) {

      bool almostEqual =
        FloatHelper.AreAlmostEqual(expected.Min.X, actual.Min.X, deltaUlps) &&
        FloatHelper.AreAlmostEqual(expected.Min.Y, actual.Min.Y, deltaUlps) &&
        FloatHelper.AreAlmostEqual(expected.Min.Z, actual.Min.Z, deltaUlps) &&
        FloatHelper.AreAlmostEqual(expected.Max.X, actual.Max.X, deltaUlps) &&
        FloatHelper.AreAlmostEqual(expected.Max.Y, actual.Max.Y, deltaUlps) &&
        FloatHelper.AreAlmostEqual(expected.Max.Z, actual.Max.Z, deltaUlps);

      if(almostEqual)
        return;

      // Now we already know that the two boxes are not equal even within the allowed
      // deviation (delta argument). In order to force NUnit to output a good error
      // message, we now let NUnit do the job again fully well knowing that it will
      // fail. This allows for deltas and good NUnit integration at the same time.
      Assert.AreEqual(expected, actual, message);

    }

  }

} // namespace Nuclex.Geometry

#endif // UNITTEST
