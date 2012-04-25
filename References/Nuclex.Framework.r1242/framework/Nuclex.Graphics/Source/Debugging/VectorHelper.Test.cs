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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using NUnit.Framework;

namespace Nuclex.Graphics.Debugging {

  /// <summary>Unit Test for the vector helper class</summary>
  [TestFixture]
  internal class VectorHelperTest {

    /// <summary>
    ///   Verifies that the vector helper can determine an arbitrary perpendicular vector
    ///   to another vector
    /// </summary>
    [Test]
    public void TestGetPerpendicularVector() {
      Vector3[] testVectors = new Vector3[] {
        Vector3.Right,
        Vector3.Up,
        Vector3.Backward,
        Vector3.Normalize(Vector3.One),
        Vector3.Normalize(new Vector3(1.0f, 0.0f, 0.0f)),
        Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f)),
        Vector3.Normalize(new Vector3(0.0f, 0.0f, 1.0f)),
        Vector3.Normalize(new Vector3(1.0f, 1.0f, 0.0f)),
        Vector3.Normalize(new Vector3(1.0f, 0.0f, 1.0f)),
        Vector3.Normalize(new Vector3(0.0f, 1.0f, 1.0f)),
        Vector3.Normalize(new Vector3(1.0f, 2.0f, 3.0f)),
        Vector3.Normalize(new Vector3(3.0f, 1.0f, 2.0f)),
        Vector3.Normalize(new Vector3(2.0f, 3.0f, 1.0f))
      };

      const double ninetyDegrees = Math.PI / 2.0;
      for (int index = 0; index < testVectors.Length; ++index) {
        Vector3 perpendicular = VectorHelper.GetPerpendicularVector(testVectors[index]);

        double angle = Math.Acos(Vector3.Dot(testVectors[index], perpendicular));
        Assert.That(
          angle, Is.EqualTo(ninetyDegrees).Within(10).Ulps
        );
      }
    }

  }

} // namespace Nuclex.Graphics.Debugging

#endif // UNITTEST
