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

using Nuclex.Support;

namespace Nuclex.Geometry {

  /// <summary>Unit Test for the default random number generator</summary>
  [TestFixture]
  public class DefaultRandomTest {

    /// <summary>Verifies that the default constructor works</summary>
    [Test]
    public void TestDefaultConstructor() {
      new DefaultRandom();
    }

    /// <summary>Verifies that the constructor with explicit seed value works</summary>
    [Test]
    public void TestSeedConstructor() {
      new DefaultRandom(12345);
    }

    /// <summary>Verifies that the seed value has the desired effect</summary>
    [Test]
    public void TestReseed() {
      DefaultRandom one = new DefaultRandom(123456789);
      DefaultRandom two = new DefaultRandom(123456789);

      Assert.AreEqual(one.Next(int.MaxValue), two.Next(int.MaxValue));
    }

    /// <summary>
    ///   Verifies that the random number generator can generate integers within a range
    /// </summary>
    [Test]
    public void TestNext() {
      DefaultRandom randomNumberGenerator = new DefaultRandom();
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        Assert.That(randomNumberGenerator.Next(12345), Is.LessThan(12345));
      }
    }

    /// <summary>
    ///   Verifies that the random number generator can generate doubles within a range
    /// </summary>
    [Test]
    public void TestNextDouble() {
      DefaultRandom randomNumberGenerator = new DefaultRandom();
      for(int index = 0; index < Specifications.ProbabilisticFunctionSamples; ++index) {
        double randomNumber = randomNumberGenerator.NextDouble();

        Assert.That(randomNumber, Is.GreaterThanOrEqualTo(0.0));
        Assert.That(randomNumber, Is.LessThan(1.0));
      }
    }

  }

} // namespace Nuclex.Geometry

#endif // UNITTEST
