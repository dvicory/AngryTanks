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
using System.Collections;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the read only list wrapper</summary>
  [TestFixture]
  public class ReadOnlyListTest {

    /// <summary>
    ///   Verifies that the copy constructor of the read only list works
    /// </summary>
    [Test]
    public void TestCopyConstructor() {
      int[] integers = new int[] { 12, 34, 56, 78 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      CollectionAssert.AreEqual(integers, testList);
    }

    /// <summary>Verifies that the IsReadOnly property returns true</summary>
    [Test]
    public void TestIsReadOnly() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[0]);

      Assert.IsTrue(testList.IsReadOnly);
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the read only list works
    /// </summary>
    [Test]
    public void TestCopyToArray() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(inputIntegers);

      int[] outputIntegers = new int[testList.Count];
      testList.CopyTo(outputIntegers, 0);

      CollectionAssert.AreEqual(inputIntegers, outputIntegers);
    }

    /// <summary>
    ///   Checks whether the Contains() method of the read only list is able to
    ///   determine if the list contains an item
    /// </summary>
    [Test]
    public void TestContains() {
      int[] integers = new int[] { 1234, 6789 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.IsTrue(testList.Contains(1234));
      Assert.IsFalse(testList.Contains(4321));
    }

    /// <summary>
    ///   Checks whether the IndexOf() method of the read only list is able to
    ///   determine if the index of an item in the list
    /// </summary>
    [Test]
    public void TestIndexOf() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.AreEqual(0, testList.IndexOf(12));
      Assert.AreEqual(1, testList.IndexOf(34));
      Assert.AreEqual(2, testList.IndexOf(67));
      Assert.AreEqual(3, testList.IndexOf(89));
    }

    /// <summary>
    ///   Checks whether the indexer method of the read only list is able to
    ///   retrieve items from the list
    /// </summary>
    [Test]
    public void TestRetrieveByIndexer() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.AreEqual(12, testList[0]);
      Assert.AreEqual(34, testList[1]);
      Assert.AreEqual(67, testList[2]);
      Assert.AreEqual(89, testList[3]);
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Insert() method
    ///   is called via the generic IList&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnInsertViaGenericIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList<int>).Insert(0, 12345); }
      );
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its RemoveAt() method
    ///   is called via the generic IList&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaGenericIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[1]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList<int>).RemoveAt(0); }
      );
    }

    /// <summary>
    ///   Checks whether the indexer method of the read only list will throw an exception
    ///   if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestRetrieveByIndexerViaGenericIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.AreEqual(12, (testList as IList<int>)[0]);
      Assert.AreEqual(34, (testList as IList<int>)[1]);
      Assert.AreEqual(67, (testList as IList<int>)[2]);
      Assert.AreEqual(89, (testList as IList<int>)[3]);
    }

    /// <summary>
    ///   Checks whether the indexer method of the read only list will throw an exception
    ///   if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestThrowOnReplaceByIndexerViaGenericIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[1]);

      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList<int>)[0] = 12345; }
      );
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Add() method
    ///   is called via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnAddViaGenericICollection() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testList as ICollection<int>).Add(12345); }
      );
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Clear() method
    ///   is called via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnClearViaGenericICollection() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[1]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testList as ICollection<int>).Clear(); }
      );
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Remove() method
    ///   is called via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaGenericICollection() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testList as ICollection<int>).Remove(89); }
      );
    }

    /// <summary>
    ///   Tests whether the typesafe enumerator of the read only list is working
    /// </summary>
    [Test]
    public void TestTypesafeEnumerator() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(inputIntegers);

      List<int> outputIntegers = new List<int>();
      foreach(int value in testList) {
        outputIntegers.Add(value);
      }

      CollectionAssert.AreEqual(inputIntegers, outputIntegers);
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Clear() method
    ///   is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnClearViaIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[1]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList).Clear(); }
      );
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Add() method
    ///   is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnAddViaIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList).Add(12345); }
      );
    }

    /// <summary>
    ///   Checks whether the Contains() method of the read only list is able to
    ///   determine if the list contains an item
    /// </summary>
    [Test]
    public void TestContainsViaIList() {
      int[] integers = new int[] { 1234, 6789 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.IsTrue((testList as IList).Contains(1234));
      Assert.IsFalse((testList as IList).Contains(4321));
    }

    /// <summary>
    ///   Checks whether the IndexOf() method of the read only list is able to
    ///   determine if the index of an item in the list
    /// </summary>
    [Test]
    public void TestIndexOfViaIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.AreEqual(0, (testList as IList).IndexOf(12));
      Assert.AreEqual(1, (testList as IList).IndexOf(34));
      Assert.AreEqual(2, (testList as IList).IndexOf(67));
      Assert.AreEqual(3, (testList as IList).IndexOf(89));
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Insert() method
    ///   is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnInsertViaIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList).Insert(0, 12345); }
      );
    }

    /// <summary>
    ///   Checks whether the IsFixedSize property of the read only list returns the
    ///   expected result for a read only list based on a fixed array
    /// </summary>
    [Test]
    public void TestIsFixedSizeViaIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.IsTrue((testList as IList).IsFixedSize);
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Remove() method
    ///   is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaIList() {
      int[] integers = new int[] { 1234, 6789 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList).Remove(6789); }
      );
    }

    /// <summary>
    ///   Checks whether the read only list will throw an exception if its Remove() method
    ///   is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveAtViaIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[1]);

      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList).RemoveAt(0); }
      );
    }

    /// <summary>
    ///   Checks whether the indexer method of the read only list will throw an exception
    ///   if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestRetrieveByIndexerViaIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(integers);

      Assert.AreEqual(12, (testList as IList)[0]);
      Assert.AreEqual(34, (testList as IList)[1]);
      Assert.AreEqual(67, (testList as IList)[2]);
      Assert.AreEqual(89, (testList as IList)[3]);
    }

    /// <summary>
    ///   Checks whether the indexer method of the read only list will throw an exception
    ///   if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestThrowOnReplaceByIndexerViaIList() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[1]);

      Assert.Throws<NotSupportedException>(
        delegate() { (testList as IList)[0] = 12345; }
      );
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the read only list works if invoked via
    ///   the ICollection interface
    /// </summary>
    [Test]
    public void TestCopyToArrayViaICollection() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      ReadOnlyList<int> testList = new ReadOnlyList<int>(inputIntegers);

      int[] outputIntegers = new int[testList.Count];
      (testList as ICollection).CopyTo(outputIntegers, 0);

      CollectionAssert.AreEqual(inputIntegers, outputIntegers);
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    /// </summary>
    [Test]
    public void TestSynchronization() {
      ReadOnlyList<int> testList = new ReadOnlyList<int>(new int[0]);

      if(!(testList as ICollection).IsSynchronized) {
        lock((testList as ICollection).SyncRoot) {
          Assert.AreEqual(0, testList.Count);
        }
      }
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
