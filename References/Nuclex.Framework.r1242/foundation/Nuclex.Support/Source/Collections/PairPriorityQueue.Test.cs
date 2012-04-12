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

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the priority queue class</summary>
  [TestFixture]
  public class PairPriorityQueueTest {

    /// <summary>Tests to ensure the count property is properly updated</summary>
    [Test]
    public void TestCount() {
      PairPriorityQueue<float, string> testQueue = new PairPriorityQueue<float, string>();

      Assert.AreEqual(0, testQueue.Count);
      testQueue.Enqueue(12.34f, "a");
      Assert.AreEqual(1, testQueue.Count);
      testQueue.Enqueue(56.78f, "b");
      Assert.AreEqual(2, testQueue.Count);
      testQueue.Dequeue();
      Assert.AreEqual(1, testQueue.Count);
      testQueue.Enqueue(9.0f, "c");
      Assert.AreEqual(2, testQueue.Count);
      testQueue.Clear();
      Assert.AreEqual(0, testQueue.Count);
    }

    /// <summary>Tests to ensure that the priority collection actually sorts items</summary>
    [Test]
    public void TestOrdering() {
      PairPriorityQueue<float, string> testQueue = new PairPriorityQueue<float, string>();

      testQueue.Enqueue(1.0f, "a");
      testQueue.Enqueue(9.0f, "i");
      testQueue.Enqueue(2.0f, "b");
      testQueue.Enqueue(8.0f, "h");
      testQueue.Enqueue(3.0f, "c");
      testQueue.Enqueue(7.0f, "g");
      testQueue.Enqueue(4.0f, "d");
      testQueue.Enqueue(6.0f, "f");
      testQueue.Enqueue(5.0f, "e");

      Assert.AreEqual("i", testQueue.Dequeue().Item);
      Assert.AreEqual("h", testQueue.Dequeue().Item);
      Assert.AreEqual("g", testQueue.Dequeue().Item);
      Assert.AreEqual("f", testQueue.Dequeue().Item);
      Assert.AreEqual("e", testQueue.Dequeue().Item);
      Assert.AreEqual("d", testQueue.Dequeue().Item);
      Assert.AreEqual("c", testQueue.Dequeue().Item);
      Assert.AreEqual("b", testQueue.Dequeue().Item);
      Assert.AreEqual("a", testQueue.Dequeue().Item);
    }

    /// <summary>Tests to ensure that the priority collection's Peek() method works</summary>
    [Test]
    public void TestPeek() {
      PairPriorityQueue<float, string> testQueue = new PairPriorityQueue<float, string>();

      testQueue.Enqueue(1.0f, "a");
      testQueue.Enqueue(2.0f, "b");
      testQueue.Enqueue(0.0f, "c");

      Assert.AreEqual("b", testQueue.Peek().Item);
    }

    /// <summary>Tests whether the priority collection can copy itself into an array</summary>
    [Test]
    public void TestCopyTo() {
      PairPriorityQueue<float, string> testQueue = new PairPriorityQueue<float, string>();

      testQueue.Enqueue(1.0f, "a");
      testQueue.Enqueue(9.0f, "i");
      testQueue.Enqueue(2.0f, "b");
      testQueue.Enqueue(8.0f, "h");
      testQueue.Enqueue(3.0f, "c");
      testQueue.Enqueue(7.0f, "g");
      testQueue.Enqueue(4.0f, "d");
      testQueue.Enqueue(6.0f, "f");
      testQueue.Enqueue(5.0f, "e");

      PriorityItemPair<float, string>[] itemArray = new PriorityItemPair<float, string>[9];
      testQueue.CopyTo(itemArray, 0);

      CollectionAssert.AreEquivalent(testQueue, itemArray);
    }

    /// <summary>
    ///   Tests whether the priority collection provides a synchronization root
    /// </summary>
    [Test]
    public void TestSyncRoot() {
      PairPriorityQueue<int, int> testQueue = new PairPriorityQueue<int, int>();

      // If IsSynchronized returns true, SyncRoot is allowed to be null
      if(!testQueue.IsSynchronized) {
        lock(testQueue.SyncRoot) {
          testQueue.Clear();
        }
      }
    }

    /// <summary>
    ///   Tests whether the priority collection provides a working type-safe enumerator
    /// </summary>
    [Test]
    public void TestEnumerator() {
      PairPriorityQueue<float, string> testQueue = new PairPriorityQueue<float, string>();

      testQueue.Enqueue(1.0f, "a");
      testQueue.Enqueue(2.0f, "b");
      testQueue.Enqueue(0.0f, "c");

      List<PriorityItemPair<float, string>> testList =
        new List<PriorityItemPair<float,string>>();

      foreach(PriorityItemPair<float, string> entry in testQueue) {
        testList.Add(entry);
      }    

      CollectionAssert.AreEquivalent(testQueue, testList);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
