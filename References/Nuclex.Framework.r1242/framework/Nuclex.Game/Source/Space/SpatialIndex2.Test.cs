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
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;

using NUnit.Framework;

namespace Nuclex.Game.Space {

  /// <summary>Unit tests for the 2D spatial index base class</summary>
  [TestFixture]
  internal class SpatialIndex2Test {

    #region class DummySpatialIndex2

    /// <summary>Dummy implementation of a 2D spatial index for the unit test</summary>
    private class DummySpatialIndex2 : SpatialIndex2<int> {

      /// <summary>Queries the spatial database for all objects in a region</summary>
      /// <param name="region">Region of which the items will be returned</param>
      /// <param name="items">
      ///   Collection that will receive all items in the query region
      /// </param>
      /// <remarks>
      ///   Use this method to avoid generating garbage by reusing the collection
      ///   the queried items are stored in.
      /// </remarks>
      public override void Query(BoundingRectangle region, ICollection<int> items) {
        for(int index = 0; index < 10; ++index) {
          items.Add(index);
        }
      }

      /// <summary>Inserts an item into the spatial database</summary>
      /// <param name="itemToAdd">Item that will be inserted</param>
      public override void Insert(int itemToAdd) {
        throw new NotImplementedException();
      }

      /// <summary>Removes an item from the spatial database</summary>
      /// <param name="itemToRemove">Item that will be removed</param>
      public override bool Remove(int itemToRemove) {
        throw new NotImplementedException();
      }

    }

    #endregion // class DummySpatialIndex2

    /// <summary>Verifies that the default enumerator is working</summary>
    [Test]
    public void TestDefaultEnumerator() {
      DummySpatialIndex2 index = new DummySpatialIndex2();
      CollectionAssert.AreEquivalent(
        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        index.Query(new BoundingRectangle())
      );
    }

  }

} // namespace Nuclex.Game.Space

#endif // UNITTEST
