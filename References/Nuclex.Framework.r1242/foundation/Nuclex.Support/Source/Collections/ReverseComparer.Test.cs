#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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

#if UNITTEST

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the ReverseComparer helper class</summary>
  [TestFixture]
  public class ReverseComparerTest {

    #region class FortyTwoComparer

    /// <summary>Special comparer in which 42 is larger than everything</summary>
    private class FortyTwoComparer : IComparer<int> {

      /// <summary>Compares the left value to the right value</summary>
      /// <param name="left">Value on the left side</param>
      /// <param name="right">Value on the right side</param>
      /// <returns>The relationship of the two values</returns>
      public int Compare(int left, int right) {

        // Is there a 42 in the arguments?
        if(left == 42) {
          if(right == 42) {
            return 0; // both are equal
          } else {
            return +1; // left is larger
          }
        } else if(right == 42) {
          return -1; // right is larger
        }

        // No 42 encountered, proceed as normal
        return Math.Sign(left - right);

      }

    }

    #endregion // class FortyTwoComparer

    /// <summary>
    ///   Tests whether the default constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      new ReverseComparer<int>();
    }

    /// <summary>
    ///   Tests whether the full constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      new ReverseComparer<int>(new FortyTwoComparer());
    }

    /// <summary>
    ///   Tests whether the full constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestReversedDefaultComparer() {
      Comparer<int> comparer = Comparer<int>.Default;
      ReverseComparer<int> reverseComparer = new ReverseComparer<int>(comparer);

      Assert.Greater(0, comparer.Compare(10, 20));
      Assert.Less(0, comparer.Compare(20, 10));

      Assert.Less(0, reverseComparer.Compare(10, 20));
      Assert.Greater(0, reverseComparer.Compare(20, 10));
    }

    /// <summary>
    ///   Tests whether the full constructor of the reverse comparer works
    /// </summary>
    [Test]
    public void TestReversedCustomComparer() {
      FortyTwoComparer fortyTwoComparer = new FortyTwoComparer();
      ReverseComparer<int> reverseFortyTwoComparer = new ReverseComparer<int>(
        fortyTwoComparer
      );

      Assert.Less(0, fortyTwoComparer.Compare(42, 84));
      Assert.Greater(0, fortyTwoComparer.Compare(84, 42));

      Assert.Greater(0, reverseFortyTwoComparer.Compare(42, 84));
      Assert.Less(0, reverseFortyTwoComparer.Compare(84, 42));
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
