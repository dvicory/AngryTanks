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

#if ENABLE_QUADTREES

#if UNITTEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Nuclex.Support.Plugins;

namespace Nuclex.Game.Space {

  /// <summary>Unit tests for the quad-tree index</summary>
  [TestFixture]
  internal class QuadTreeTest {

    #region class TestItem

    /// <summary>Simple test item used to test the quad-tree</summary>
    private class TestItem : IBoundingRectangleProvider {

      /// <summary>Initializes a new test item</summary>
      /// <param name="rectangle">Bounding rectangle of the test item</param>
      public TestItem(BoundingRectangle rectangle) {
        this.boundingRectangle = rectangle;
      }

      /// <summary>Bounding rectangle of the test item</summary>
      public BoundingRectangle BoundingRectangle {
        get {
          ++this.BoundRectangleLookups;
          return this.boundingRectangle;
        }
      }

      /// <summary>Number of times the bounding rectangles has been queried</summary>
      public int BoundRectangleLookups;
      /// <summary>The test item's bounding rectangle</summary>
      private BoundingRectangle boundingRectangle;

    }

    #endregion // class TestItem

    /// <summary>
    ///   Verifies that the quad-tree's simplified constructor is working
    /// </summary>
    [Test]
    public void TestSimpleConstructor() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      Assert.IsNotNull(quadTree); // nonsense; avoids compiler warning
    }

    /// <summary>
    ///   Verifies that the quad-tree's full constructor is working
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 4;
      Assert.IsNotNull(quadTree); // nonsense; avoids compiler warning
    }

    /// <summary>
    ///   Tests whether the quad-tree splits the root node if too many items are
    ///   added to it
    /// </summary>
    [Test]
    public void TestInsertSplitting() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 6;

      TestItem[] items = new TestItem[] {
        // Unsplittable node to test FirstSplittableItemIndex
        new TestItem(new BoundingRectangle(-10.0f, -10.0f, 10.0f, 10.0f)),
        // Four corners
        new TestItem(new BoundingRectangle(-90.0f, -90.0f, -10.0f, -10.0f)),
        new TestItem(new BoundingRectangle(10.0f, -90.0f, 90.0f, -10.0f)),
        new TestItem(new BoundingRectangle(-90.0f, 10.0f, -10.0f, 90.0f)),
        new TestItem(new BoundingRectangle(10.0f, 10.0f, 90.0f, 90.0f)),
        // Unsplittable node to test split skipping
        new TestItem(new BoundingRectangle(-15.0f, -15.0f, 15.0f, 15.0f)),
        // Causes the node to split
        new TestItem(new BoundingRectangle(-90.0f, -90.0f, 90.0f, 90.0f)),
        // To test insertion into child nodes
        new TestItem(new BoundingRectangle(-5.0f, -5.0f, 5.0f, 5.0f)),
        new TestItem(new BoundingRectangle(-90.0f, -90.0f, -85.0f, -85.0f)),
        new TestItem(new BoundingRectangle(85.0f, -90.0f, 90.0f, -85.0f)),
        new TestItem(new BoundingRectangle(-90.0f, 85.0f, -85.0f, 90.0f)),
        new TestItem(new BoundingRectangle(85.0f, 85.0f, 90.0f, 90.0f))
      };
      foreach(TestItem item in items) {
        quadTree.Insert(item);
      }

      // Remember the current bounding rectangle lookups for the items which
      // should be situated in other quad-tree nodes by now
      int lookupCount2 = items[2].BoundRectangleLookups;
      int lookupCount3 = items[3].BoundRectangleLookups;
      int lookupCount4 = items[4].BoundRectangleLookups;

      // Perform the query
      List<TestItem> result = new List<TestItem>();
      quadTree.Query(new BoundingRectangle(-80.0f, -80.0f, -20.0f, -20.0f), result);

      // Make sure that the quad-tree didn't check the items that should have
      // been placed into other nodes
      Assert.AreEqual(lookupCount2, items[2].BoundRectangleLookups);
      Assert.AreEqual(lookupCount3, items[3].BoundRectangleLookups);
      Assert.AreEqual(lookupCount4, items[4].BoundRectangleLookups);

      // Make sure the query results are correct
      Assert.AreEqual(2, result.Count);
      Assert.Contains(items[1], result);
      Assert.Contains(items[6], result);
    }

    /// <summary>
    ///   Verifies that the quad-tree can handle unsplittable items in its nodes
    /// </summary>
    [Test]
    public void TestInsertUnsplittable() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 3;

      for(int index = 0; index < 6; ++index) {
        quadTree.Insert(
          new TestItem(new BoundingRectangle(-10.0f, -10.0f, 10.0f, 10.0f))
        );
      }
    }

    /// <summary>Verifies that the tree can handle external items</summary>
    [Test]
    public void TestQueryExternalItems() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 2;

      TestItem[] items = new TestItem[] {
        new TestItem(new BoundingRectangle(-90.0f, -110.0f, -89.9f, -109.9f)), // 0: u1
        new TestItem(new BoundingRectangle(-80.0f, -110.0f, -79.9f, -109.9f)), // 1: u2
        new TestItem(new BoundingRectangle(80.0f, -110.0f, 80.1f, -109.9f)),   // 2: u3
        new TestItem(new BoundingRectangle(90.0f, -110.0f, 90.1f, -109.9f)),   // 3: u4
        new TestItem(new BoundingRectangle(-110.0f, -90.0f, -109.9f, -89.9f)), // 4: l1
        new TestItem(new BoundingRectangle(-110.0f, -80.0f, -109.9f, -79.9f)), // 5: l2
        new TestItem(new BoundingRectangle(-110.0f, 80.0f, -109.9f, 80.1f)),   // 6: l3
        new TestItem(new BoundingRectangle(-110.0f, 90.0f, -109.9f, 90.1f)),   // 7: l4
        new TestItem(new BoundingRectangle(-90.0f, 110.0f, -89.9f, 110.1f)),   // 8: b1
        new TestItem(new BoundingRectangle(-80.0f, 110.0f, -79.9f, 110.1f)),   // 9: b2
        new TestItem(new BoundingRectangle(80.0f, 110.0f, 80.1f, 110.1f)),     // 10: b3
        new TestItem(new BoundingRectangle(90.0f, 110.0f, 90.1f, 110.1f)),     // 11: b4
        new TestItem(new BoundingRectangle(110.0f, -90.0f, 110.1f, -89.9f)),   // 12: r1
        new TestItem(new BoundingRectangle(110.0f, -80.0f, 110.1f, -79.9f)),   // 13: r2
        new TestItem(new BoundingRectangle(110.0f, 80.0f, 110.1f, 80.1f)),     // 14: r3
        new TestItem(new BoundingRectangle(110.0f, 90.0f, 110.1f, 90.1f))      // 15: r4
      };
      foreach(TestItem item in items) {
        quadTree.Insert(item);
      }

      List<TestItem> result = new List<TestItem>();

      //
      // Perform some queries that would cause a normal quadtree to query all of
      // the contained items (because they're all outside the bounds of the tree)
      //

      // Upper left
      int lookupCount2 = items[2].BoundRectangleLookups;
      int lookupCount6 = items[6].BoundRectangleLookups;
      quadTree.Query(new BoundingRectangle(-95.0f, -95.0f, -85.0f, -85.0f), result);
      Assert.AreEqual(lookupCount2, items[2].BoundRectangleLookups);
      Assert.AreEqual(lookupCount6, items[6].BoundRectangleLookups);

      // Upper right
      int lookupCount1 = items[1].BoundRectangleLookups;
      int lookupCount14 = items[14].BoundRectangleLookups;
      quadTree.Query(new BoundingRectangle(85.0f, -95.0f, 95.0f, -85.0f), result);
      Assert.AreEqual(lookupCount1, items[1].BoundRectangleLookups);
      Assert.AreEqual(lookupCount14, items[14].BoundRectangleLookups);

      // Lower left      
      int lookupCount5 = items[5].BoundRectangleLookups;
      int lookupCount10 = items[10].BoundRectangleLookups;
      quadTree.Query(new BoundingRectangle(-95.0f, 85.0f, -85.0f, 95.0f), result);
      Assert.AreEqual(lookupCount5, items[5].BoundRectangleLookups);
      Assert.AreEqual(lookupCount10, items[10].BoundRectangleLookups);

      // Lower right
      int lookupCount9 = items[9].BoundRectangleLookups;
      int lookupCount13 = items[13].BoundRectangleLookups;
      quadTree.Query(new BoundingRectangle(85.0f, 85.0f, 95.0f, 95.0f), result);
      Assert.AreEqual(lookupCount9, items[9].BoundRectangleLookups);
      Assert.AreEqual(lookupCount13, items[13].BoundRectangleLookups);
    }

    /// <summary>Verifies that the tree can handle external items</summary>
    [Test]
    public void TestQueryEntireNodes() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 2;

      TestItem[] items = new TestItem[] {
        new TestItem(new BoundingRectangle(-30.0f, -30.0f, -29.9f, -29.9f)), // 0: tl-a
        new TestItem(new BoundingRectangle(-20.0f, -20.0f, -19.9f, -19.9f)), // 1: tl-b
        new TestItem(new BoundingRectangle(-10.0f, -10.0f, -9.9f, -9.9f)),   // 2: tl-c

        new TestItem(new BoundingRectangle(30.0f, -30.0f, 30.1f, -29.9f)),   // 3: tr-a
        new TestItem(new BoundingRectangle(20.0f, -20.0f, 20.1f, -19.9f)),   // 4: tr-b
        new TestItem(new BoundingRectangle(10.0f, -10.0f, 10.1f, -9.9f)),    // 5: tr-c

        new TestItem(new BoundingRectangle(-30.0f, 30.0f, -29.9f, 30.1f)),   // 6: br-a
        new TestItem(new BoundingRectangle(-20.0f, 20.0f, -19.9f, 20.1f)),   // 7: bl-b
        new TestItem(new BoundingRectangle(-10.0f, 10.0f, -9.9f, 10.1f)),    // 8: bl-c

        new TestItem(new BoundingRectangle(30.0f, 30.0f, 30.1f, 30.1f)),     // 9: br-a
        new TestItem(new BoundingRectangle(20.0f, 20.0f, 20.1f, 20.1f)),     // 10: br-b
        new TestItem(new BoundingRectangle(10.0f, 10.0f, 10.1f, 10.1f)),     // 11: br-c

        new TestItem(new BoundingRectangle(21.0f, 21.0f, 21.1f, 21.1f)),     // 12: c-a
        new TestItem(new BoundingRectangle(22.0f, 22.0f, 22.1f, 22.1f)),     // 13: c-b
        new TestItem(new BoundingRectangle(23.0f, 23.0f, 23.1f, 23.1f))      // 14: c-c
      };
      foreach(TestItem item in items) {
        quadTree.Insert(item);
      }

      // Perform some queries that would cause a normal quadtree to query all of
      // the contained items (because they're all outside the bounds of the tree)
      List<TestItem> result = new List<TestItem>();

      quadTree.Query(new BoundingRectangle(-60.0f, -60.0f, -15.0f, -15.0f), result);
      quadTree.Query(new BoundingRectangle(15.0f, -60.0f, 60.0f, -15.0f), result);
      quadTree.Query(new BoundingRectangle(-60.0f, 15.0f, -15.0f, 60.0f), result);
      quadTree.Query(new BoundingRectangle(15.0f, 15.0f, 60.0f, 60.0f), result);
    }

    /// <summary>
    ///   Tests whether the quad-tree correctly handles an overflowing node that
    ///   can not be split
    /// </summary>
    [Test]
    public void TestIncreaseCapacityDuringInsertion() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 2;

      TestItem[] items = new TestItem[] {
        new TestItem(new BoundingRectangle(-10.0f, -10.0f, 10.0f, 10.0f)),
        new TestItem(new BoundingRectangle(-20.0f, -20.0f, 20.0f, 20.0f)),
        new TestItem(new BoundingRectangle(-30.0f, -30.0f, 30.0f, 30.0f)),
        new TestItem(new BoundingRectangle(-40.0f, -40.0f, 40.0f, 40.0f))
      };
      foreach(TestItem item in items) {
        quadTree.Insert(item);
      }
    }

    /// <summary>
    ///   Tests whether the quad-tree correctly splits an overflowed node
    /// </summary>
    [Test]
    public void TestIncreaseCapacityDuringSplit() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 5;

      TestItem[] items = new TestItem[] {
        new TestItem(new BoundingRectangle(-60.0f, -60.0f, -40.0f, -40.0f)),
        new TestItem(new BoundingRectangle(-70.0f, -70.0f, -30.0f, -30.0f)),
        new TestItem(new BoundingRectangle(-80.0f, -80.0f, -20.0f, -20.0f)),
        new TestItem(new BoundingRectangle(-90.0f, -90.0f, -10.0f, -10.0f)),

        new TestItem(new BoundingRectangle(-160.0f, -160.0f, -140.0f, -140.0f)),
        new TestItem(new BoundingRectangle(-170.0f, -170.0f, -130.0f, -130.0f)),
        new TestItem(new BoundingRectangle(-180.0f, -180.0f, -120.0f, -120.0f)),
        new TestItem(new BoundingRectangle(-190.0f, -190.0f, -110.0f, -110.0f))
      };
      foreach(TestItem item in items) {
        quadTree.Insert(item);
      }
    }

    /// <summary>
    ///   Tests whether the quad-tree can cope with an attempt to remove an object
    ///   that it doesn't contain
    /// </summary>
    [Test]
    public void TestUnindexedItemRemoval() {
      QuadTree<TestItem> quadTree = new QuadTree<TestItem>(
        new BoundingRectangle(-100.0f, -100.0f, 100.0f, 100.0f)
      );
      quadTree.MaxItemsPerNode = 32;

      Assert.IsFalse(
        quadTree.Remove(new TestItem(new BoundingRectangle()))
      );
    }

  }

} // namespace Nuclex.Game.Space

#endif // UNITTEST

#endif // ENABLE_QUADTREES
