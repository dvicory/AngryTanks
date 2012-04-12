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
using NUnit.Framework;

using Matrix33 = Nuclex.Math.Matrix33<double, Nuclex.Math.Generic.UncheckedScalarMath>;
using Vector3 = Nuclex.Math.Vector3<double, Nuclex.Math.Generic.UncheckedScalarMath>;

namespace Nuclex.Math {

  /// <summary>Tests the implementation of the Matrix44 class</summary>
  [TestFixture]
  public class Matrix33Test {

    /// <summary>Validates that the constants of the Matrix44 class work</summary>
    [Test]
    public void TestConstants() {
      Assert.AreEqual(
        new Matrix33(
          1.0, 0.0, 0.0,
          0.0, 1.0, 0.0,
          0.0, 0.0, 1.0
        ),
        Matrix33.Identity,
        "Identity matrix is properly initialized"
      );

      Assert.AreEqual(
        new Matrix33(
          0.0, 0.0, 0.0,
          0.0, 0.0, 0.0,
          0.0, 0.0, 0.0
        ),
        Matrix33.Zero,
        "All-Zero matrix is properly initialized"
      );

      Assert.AreEqual(
        new Matrix33(
          1.0, 1.0, 1.0,
          1.0, 1.0, 1.0,
          1.0, 1.0, 1.0
        ),
        Matrix33.One,
        "All-One matrix is properly initialized"
      );
    }

    /// <summary>Ensures that rotation matrices work as expected</summary>
    [Test]
    public void TestRotationMatrices() {
      double angle = Nuclex.Math.Trigonometry.ToRadians(45.0);

      // Ensure that the X rotation matrix is built as expected
      Matrix33 xRotated = Matrix33.Identity * Matrix33.BuildXRotation(angle);
      AssertHelper.AssertAreEqual(
        Vector3.Right, xRotated.Right,
        "X-Rotated matrix' right vector is still pointing right"
      );
      AssertHelper.AssertAreEqual(
        Vector3.Normalize(new Vector3(0, 1, -1)), xRotated.Up,
        "X-Rotated matrix' up vector is pointing diagonally"
      );
      AssertHelper.AssertAreEqual(
        Vector3.Normalize(new Vector3(0, 1, 1)), xRotated.Into,
        "X-Rotated matrix' in vector is pointing diagonally"
      );

      // Ensure that the Y rotation matrix is built as expected
      Matrix33 yRotated = Matrix33.Identity * Matrix33.BuildYRotation(angle);
      AssertHelper.AssertAreEqual(
        Vector3.Normalize(new Vector3(1, 0, 1)), yRotated.Right,
        "Y-Rotated matrix' right vector is pointing diagonally"
      );
      AssertHelper.AssertAreEqual(
        Vector3.Up, yRotated.Up,
        "Y-Rotated matrix' up vector is still pointing up"
      );
      AssertHelper.AssertAreEqual(
        Vector3.Normalize(new Vector3(-1, 0, 1)), yRotated.Into,
        "Y-Rotated matrix' in vector is pointing diagonally"
      );

      // Ensure that the Z rotation matrix is built as expected
      Matrix33 zRotated = Matrix33.Identity * Matrix33.BuildZRotation(angle);
      AssertHelper.AssertAreEqual(
        Vector3.Normalize(new Vector3(1, -1, 0)), zRotated.Right,
        "Z-Rotated matrix' right vector is pointing diagonally"
      );
      AssertHelper.AssertAreEqual(
        Vector3.Normalize(new Vector3(1, 1, 0)), zRotated.Up,
        "Z-Rotated matrix' up vector is pointing diagonally"
      );
      AssertHelper.AssertAreEqual(
        Vector3.In, zRotated.Into,
        "Z-Rotated matrix' in vector is still pointing in"
      );

    }

  }

} // namespace Nuclex.Math

#endif // UNITTEST
