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
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Audio.Verification {

  /// <summary>Unit Test for the AccurateRip database query and utility class</summary>
  [TestFixture]
  public class AccurateRipTest {

    /// <summary>Validates that the primary disc id calculation algorithm is correct</summary>
    [Test]
    public void TestDiscId1Calculation() {

      Assert.AreEqual(
        0x00000002, // 2
        AccurateRip.CalculateDiscId1(new int[] { 2 })
      );

      Assert.AreEqual(
        0x000013cc, // 2 + 260 + 520 + 1763 + 2523
        AccurateRip.CalculateDiscId1(
          new int[] {
            182 / 75, // 2
            19527 / 75, // 260
            39015 / 75, // 520
            132282 / 75, // 1763
            189270 / 75 // 2523
          }
        )
      );

    }

    /// <summary>
    ///   Validates that the secondary disc id calculation algorithm is correct
    /// </summary>
    [Test]
    public void TestDiscId2Calculation() {

      Assert.AreEqual(
        0x00000002, // 2*1
        AccurateRip.CalculateDiscId2(new int[] { 2 })
      );

      Assert.AreEqual(
        0x000054f5, // 2*1 + 260*2 + 520*3 + 1763*4 + 2523*5
        AccurateRip.CalculateDiscId2(
          new int[] {
            182 / 75, // 2
            19527 / 75, // 260
            39015 / 75, // 520
            132282 / 75, // 1763
            189270 / 75 // 2523
          }
        )
      );

    }

  }

} // namespace Nuclex.Audio.Verification

#endif // UNITTEST
