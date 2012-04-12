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

using Nuclex.Math;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Math {

  /// <summary>Test for the auxiliary trigonometry function</summary>
  [TestFixture]
  public class TrigonometryTest {

    /// <summary>Checks the AngleBetween() method with a positive path</summary>
    [Test]
    public void TestPositiveAngleBetween() {

      Assert.AreEqual(
        Trigonometry.ToRadians(0.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(0.0), Trigonometry.ToRadians(0.0),
          TangentialPath.Positive
        ),
        double.Epsilon,
        "Forward angular offset between 0 and 0 degrees is 0"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(0.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(0.0), Trigonometry.ToRadians(360.0),
          TangentialPath.Positive
        ),
        double.Epsilon,
        "Forward angular offset between 0 and 360 degrees is 0"
      );

      // This has to return -360, otherwise the direction would be lost!
      Assert.AreEqual(
        Trigonometry.ToRadians(-360.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(360.0), Trigonometry.ToRadians(0.0),
          TangentialPath.Positive
        ),
        double.Epsilon,
        "Forward angular offset between 360 and 0 degrees is -360"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(270.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(-90.0), Trigonometry.ToRadians(180.0),
          TangentialPath.Positive
        ),
        double.Epsilon,
        "Forward angular offset between -90 and 180 degrees is 270"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(-270.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(180.0), Trigonometry.ToRadians(-90.0),
          TangentialPath.Positive
        ),
        double.Epsilon,
        "Forward angular offset between 180 and -90 degrees is -270"
      );

    }

    /// <summary>Checks the AngleBetween() method with a negative path</summary>
    [Test]
    public void TestNegativeAngleBetween() {

      // This has to return -360, otherwise the direction would be lost!
      Assert.AreEqual(
        Trigonometry.ToRadians(-360.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(0.0), Trigonometry.ToRadians(0.0),
          TangentialPath.Negative
        ),
        double.Epsilon,
        "Reverse angular offset between 0 and 0 degrees is -360"
      );

      // This has to return -360, otherwise the direction would be lost!
      Assert.AreEqual(
        Trigonometry.ToRadians(-360.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(0.0), Trigonometry.ToRadians(360.0),
          TangentialPath.Negative
        ),
        double.Epsilon,
        "Reverse angular offset between 0 and 360 degrees is -360"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(0.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(360.0), Trigonometry.ToRadians(0.0),
          TangentialPath.Negative
        ),
        double.Epsilon,
        "Reverse angular offset between 360 and 0 degrees is 0"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(-90.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(-90.0), Trigonometry.ToRadians(180.0),
          TangentialPath.Negative
        ),
        double.Epsilon,
        "Reverse angular offset between -90 and 180 degrees is -90"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(90.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(180.0), Trigonometry.ToRadians(-90.0),
          TangentialPath.Negative
        ),
        double.Epsilon,
        "Reverse angular offset between 180 and -90 degrees is 90"
      );

    }

    /// <summary>Checks the AngleBetween() method letting it determine the shortest path</summary>
    [Test]
    public void TestShortestAngleBetween() {

      Assert.AreEqual(
        Trigonometry.ToRadians(0.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(0.0), Trigonometry.ToRadians(0.0),
          TangentialPath.Shortest
        ),
        double.Epsilon,
        "Shortest angular offset between 0 and 0 degrees is zero"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(0.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(0.0), Trigonometry.ToRadians(360.0),
          TangentialPath.Shortest
        ),
        double.Epsilon,
        "Shortest angular offset between 0 and 360 degrees is 0"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(0.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(360.0), Trigonometry.ToRadians(0.0),
          TangentialPath.Shortest
        ),
        double.Epsilon,
        "Shortest angular offset between 360 and 0 degrees is 0"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(-90.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(-90.0), Trigonometry.ToRadians(180.0),
          TangentialPath.Shortest
        ),
        double.Epsilon,
        "Shortest angular offset between -90 and 180 degrees is -90"
      );

      Assert.AreEqual(
        Trigonometry.ToRadians(90.0),
        Trigonometry.AngleBetween(
          Trigonometry.ToRadians(180.0), Trigonometry.ToRadians(-90.0),
          TangentialPath.Shortest
        ),
        double.Epsilon,
        "Shortest angular offset between 180 and -90 degrees is 90"
      );

    }

  }

} // namespace Nuclex.Math

#endif // UNITTEST
