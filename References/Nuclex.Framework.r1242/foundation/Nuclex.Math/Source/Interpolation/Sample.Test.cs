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

using Nuclex.Support;

namespace Nuclex.Math.Interpolation {

  /// <summary>Unit test for the interpolation sample class</summary>
  [TestFixture]
  public class SampleTest {

    /// <summary>
    ///   Validates that the default constructor of the sample class is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      Sample<double, int> testSample = new Sample<double, int>();
      Assert.IsNotNull(testSample); // nonsense to avoid unused variable warning
    }
    
    /// <summary>Tests the complete constructor of the </summary>
    [Test]
    public void TestFullConstructor() {
      Sample<double, float> testSample = new Sample<double, float>(12.34, 56.78f);
      
      Assert.AreEqual(12.34, testSample.Time);
      Assert.AreEqual(56.78f, testSample.Value);
    }

  }

} // namespace Nuclex.Math.Interpolation

#endif // UNITTEST