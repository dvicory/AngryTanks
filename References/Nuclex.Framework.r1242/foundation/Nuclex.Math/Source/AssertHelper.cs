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
using System.Collections.Generic;

using Nuclex.Support;

using Vector3 = Nuclex.Math.Vector3<double, Nuclex.Math.Generic.UncheckedScalarMath>;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Math {

  /// <summary>Helper class for unit testing assertions</summary>
  internal static class AssertHelper {

    /// <summary>Asserts that two vectors are equal within the accepted deviation</summary>
    /// <param name="expected">Value the vector is expected to have</param>
    /// <param name="actual">Actual value that the vector has</param>
    /// <param name="message">Message to describe the assertion</param>
    public static void AssertAreEqual(Vector3 expected, Vector3 actual, string message) {

      // Determine whether the values are within a reasonable range to the expected results
      bool almostEqual =
        FloatHelper.AreAlmostEqual(expected.X, actual.X, 1) &&
        FloatHelper.AreAlmostEqual(expected.Y, actual.Y, 1) &&
        FloatHelper.AreAlmostEqual(expected.Z, actual.Z, 1);

      // If the values are off, let NUnit do its (zero-tolerance) own check which we
      // already know will fail, but which saves us from writing our own NUnit assertion
      if(!almostEqual) {
        Assert.AreEqual(expected, actual, message);
      }

    }

  }

} // namespace Nuclex.Math

#endif // UNITTEST