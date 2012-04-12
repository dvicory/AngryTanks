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

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.Math {

  /// <summary>Test for the arithmetical auxiliary functions</summary>
  [TestFixture]
  public class ArithmeticsTest {

    /// <summary>Checks whether the nearest multiple is properly determined</summary>
    [Test]
    public void TestNearestMultiple() {

      Assert.AreEqual(
        30.0,
        Arithmetics.NearestMultiple(28.0, 15.0),
        "Nearest mulitple of 15 to 28 is 30"
      );

      Assert.AreEqual(
        15.0,
        Arithmetics.NearestMultiple(17.0, 15.0),
        "Nearest mulitple of 15 to 17 is 15"
      );

      Assert.AreEqual(
        30.0,
        Arithmetics.NearestMultiple(22.5, 15.0),
        "Nearest mulitple of 15 to 22.5 is 30"
      );

      Assert.AreEqual(
        -30.0,
        Arithmetics.NearestMultiple(-28.0, 15.0),
        "Nearest mulitple of 15 to -28 is -30"
      );

      Assert.AreEqual(
        -15.0,
        Arithmetics.NearestMultiple(-17.0, 15.0),
        "Nearest mulitple of 15 to -17 is -15"
      );

      Assert.AreEqual(
        -30.0,
        Arithmetics.NearestMultiple(-22.5, 15.0),
        "Nearest mulitple of 15 to -22.5 is -30"
      );

    }

    /// <summary>Tests whether the Wrap() method is working properly</summary>
    [Test]
    public void TestWrap() {
      Assert.AreEqual(
        -90.0,
        Shared<Generic.UncheckedScalarMath>.Instance.Wrap(
          270.0, -360.0, 0.0
        ),
        "Wrap respects negative wrap ranges in doubles"
      );
      Assert.AreEqual(
        -90,
        Shared<Generic.UncheckedScalarMath>.Instance.Wrap(
          270, -360, 0
        ),
        "Wrap respects negative wrap ranges in integers"
      );
    }

  }

} // namespace Nuclex.Math

#endif // UNITTEST