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
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the transforming read only collection wrapper</summary>
  [TestFixture]
  public class TransformingReadOnlyCollectionTest {

    #region class StringTransformer

    /// <summary>Test implementation of a transforming collection</summary>
    private class StringTransformer : TransformingReadOnlyCollection<int, string> {

      /// <summary>Initializes a new int-to-string transforming collection</summary>
      /// <param name="items">Items the transforming collection will contain</param>
      public StringTransformer(IList<int> items) : base(items) { }

      /// <summary>Transforms an item into the exposed type</summary>
      /// <param name="item">Item to be transformed</param>
      /// <returns>The transformed item</returns>
      /// <remarks>
      ///   This method is used to transform an item in the wrapped collection into
      ///   the exposed item type whenever the user accesses an item. Expect it to
      ///   be called frequently, because the TransformingReadOnlyCollection does
      ///   not cache or otherwise store the transformed items.
      /// </remarks>
      protected override string Transform(int item) {
        if(item == 42) {
          return null;
        }

        return item.ToString();
      }

    }

    #endregion // class StringTransformer

    /// <summary>
    ///   Verifies that the copy constructor of the transforming read only collection works
    /// </summary>
    [Test]
    public void TestCopyConstructor() {
      int[] integers = new int[] { 12, 34, 56, 78 };
      StringTransformer testCollection = new StringTransformer(integers);

      string[] strings = new string[] { "12", "34", "56", "78" };
      CollectionAssert.AreEqual(strings, testCollection);
    }

    /// <summary>Verifies that the IsReadOnly property returns true</summary>
    [Test]
    public void TestIsReadOnly() {
      StringTransformer testCollection = new StringTransformer(new int[0]);

      Assert.IsTrue(testCollection.IsReadOnly);
    }

    /// <summary>
    ///   Verifies that the CopyTo() method of the transforming read only collection works
    /// </summary>
    [Test]
    public void TestCopyToArray() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      StringTransformer testCollection = new StringTransformer(inputIntegers);

      string[] inputStrings = new string[] { "12", "34", "56", "78" };
      string[] outputStrings = new string[testCollection.Count];
      testCollection.CopyTo(outputStrings, 0);

      CollectionAssert.AreEqual(inputStrings, outputStrings);
    }

    /// <summary>
    ///   Verifies that the CopyTo() method of the transforming read only collection throws
    ///   an exception if the target array is too small to hold the collection's contents
    /// </summary>
    [Test]
    public void TestThrowOnCopyToTooSmallArray() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      StringTransformer testCollection = new StringTransformer(inputIntegers);

      string[] outputStrings = new string[testCollection.Count - 1];
      Assert.Throws<ArgumentException>(
        delegate() { testCollection.CopyTo(outputStrings, 0); }
      );
    }

    /// <summary>
    ///   Checks whether the Contains() method of the transforming read only collection
    ///   is able to determine if the collection contains an item
    /// </summary>
    [Test]
    public void TestContains() {
      int[] integers = new int[] { 1234, 6789 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.IsTrue(testCollection.Contains("1234"));
      Assert.IsFalse(testCollection.Contains("4321"));
    }

    /// <summary>
    ///   Checks whether the IndexOf() method of the transforming read only collection
    ///   is able to determine if the index of an item in the collection
    /// </summary>
    [Test]
    public void TestIndexOf() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.AreEqual(0, testCollection.IndexOf("12"));
      Assert.AreEqual(1, testCollection.IndexOf("34"));
      Assert.AreEqual(2, testCollection.IndexOf("67"));
      Assert.AreEqual(3, testCollection.IndexOf("89"));
    }

    /// <summary>
    ///   Checks whether the IndexOf() method of the transforming read only collection
    ///   can cope with queries for 'null' when no 'null' item is contained on it
    /// </summary>
    [Test]
    public void TestIndexOfWithNullItemNotContained() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.AreEqual(-1, testCollection.IndexOf(null));
    }

    /// <summary>
    ///   Checks whether the IndexOf() method of the transforming read only collection
    ///   can cope with queries for 'null' when a 'null' item is contained on it
    /// </summary>
    [Test]
    public void TestIndexOfWithNullItemContained() {
      int[] integers = new int[] { 12, 34, 67, 89, 42 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.AreEqual(4, testCollection.IndexOf(null));
    }

    /// <summary>
    ///   Verifies that the Enumerator of the transforming read only collection correctly
    ///   implements the Reset() method
    /// </summary>
    [Test]
    public void TestEnumeratorReset() {
      int[] integers = new int[] { 1234, 6789 };
      StringTransformer testCollection = new StringTransformer(integers);

      IEnumerator<string> stringEnumerator = testCollection.GetEnumerator();
      Assert.IsTrue(stringEnumerator.MoveNext());
      Assert.IsTrue(stringEnumerator.MoveNext());
      Assert.IsFalse(stringEnumerator.MoveNext());

      stringEnumerator.Reset();

      Assert.IsTrue(stringEnumerator.MoveNext());
      Assert.IsTrue(stringEnumerator.MoveNext());
      Assert.IsFalse(stringEnumerator.MoveNext());
    }

    /// <summary>
    ///   Checks whether the indexer method of the transforming read only collection
    ///   is able to retrieve items from the collection
    /// </summary>
    [Test]
    public void TestRetrieveByIndexer() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.AreEqual("12", testCollection[0]);
      Assert.AreEqual("34", testCollection[1]);
      Assert.AreEqual("67", testCollection[2]);
      Assert.AreEqual("89", testCollection[3]);
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Insert() method is called via the generic IList&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnInsertViaGenericIList() {
      StringTransformer testCollection = new StringTransformer(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList<string>).Insert(0, "12345"); }
      );
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its RemoveAt() method is called via the generic IList&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaGenericIList() {
      StringTransformer testCollection = new StringTransformer(new int[1]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList<string>).RemoveAt(0); }
      );
    }

    /// <summary>
    ///   Checks whether the indexer method of the transforming read only collection will
    ///   throw an exception if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestRetrieveByIndexerViaGenericIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.AreEqual("12", (testCollection as IList<string>)[0]);
      Assert.AreEqual("34", (testCollection as IList<string>)[1]);
      Assert.AreEqual("67", (testCollection as IList<string>)[2]);
      Assert.AreEqual("89", (testCollection as IList<string>)[3]);
    }

    /// <summary>
    ///   Checks whether the indexer method of the transforming read only collection
    ///   will throw an exception if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestThrowOnReplaceByIndexerViaGenericIList() {
      StringTransformer testCollection = new StringTransformer(new int[1]);

      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList<string>)[0] = "12345"; }
      );
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Add() method is called via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnAddViaGenericICollection() {
      StringTransformer testCollection = new StringTransformer(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as ICollection<string>).Add("12345"); }
      );
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Clear() method is called via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnClearViaGenericICollection() {
      StringTransformer testCollection = new StringTransformer(new int[1]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as ICollection<string>).Clear(); }
      );
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Remove() method is called via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaGenericICollection() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as ICollection<string>).Remove("89"); }
      );
    }

    /// <summary>
    ///   Tests whether the typesafe enumerator of the read only collection is working
    /// </summary>
    [Test]
    public void TestTypesafeEnumerator() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      StringTransformer testCollection = new StringTransformer(inputIntegers);

      List<string> outputStrings = new List<string>();
      foreach(string value in testCollection) {
        outputStrings.Add(value);
      }

      string[] inputStrings = new string[] { "12", "34", "56", "78" };
      CollectionAssert.AreEqual(inputStrings, outputStrings);
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Clear() method is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnClearViaIList() {
      StringTransformer testCollection = new StringTransformer(new int[1]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList).Clear(); }
      );
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Add() method is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnAddViaIList() {
      StringTransformer testCollection = new StringTransformer(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList).Add("12345"); }
      );
    }

    /// <summary>
    ///   Checks whether the Contains() method of the transforming read only collection
    ///   is able to determine if the collection contains an item
    /// </summary>
    [Test]
    public void TestContainsViaIList() {
      int[] integers = new int[] { 1234, 6789 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.IsTrue((testCollection as IList).Contains("1234"));
      Assert.IsFalse((testCollection as IList).Contains("4321"));
    }

    /// <summary>
    ///   Checks whether the IndexOf() method of the transforming read only collection
    ///   is able to determine if the index of an item in the collection
    /// </summary>
    [Test]
    public void TestIndexOfViaIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.AreEqual(0, (testCollection as IList).IndexOf("12"));
      Assert.AreEqual(1, (testCollection as IList).IndexOf("34"));
      Assert.AreEqual(2, (testCollection as IList).IndexOf("67"));
      Assert.AreEqual(3, (testCollection as IList).IndexOf("89"));
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Insert() method is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnInsertViaIList() {
      StringTransformer testCollection = new StringTransformer(new int[0]);
      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList).Insert(0, "12345"); }
      );
    }

    /// <summary>
    ///   Checks whether the IsFixedSize property of the transforming read only collection
    ///   returns the expected result for a transforming read only collection based on
    ///   a fixed array
    /// </summary>
    [Test]
    public void TestIsFixedSizeViaIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.IsTrue((testCollection as IList).IsFixedSize);
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Remove() method is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaIList() {
      int[] integers = new int[] { 1234, 6789 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList).Remove("6789"); }
      );
    }

    /// <summary>
    ///   Checks whether the transforming read only collection will throw an exception
    ///   if its Remove() method is called via the IList interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveAtViaIList() {
      StringTransformer testCollection = new StringTransformer(new int[1]);

      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList).RemoveAt(0); }
      );
    }

    /// <summary>
    ///   Checks whether the indexer method of the transforming read only collection
    ///   will throw an exception if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestRetrieveByIndexerViaIList() {
      int[] integers = new int[] { 12, 34, 67, 89 };
      StringTransformer testCollection = new StringTransformer(integers);

      Assert.AreEqual("12", (testCollection as IList)[0]);
      Assert.AreEqual("34", (testCollection as IList)[1]);
      Assert.AreEqual("67", (testCollection as IList)[2]);
      Assert.AreEqual("89", (testCollection as IList)[3]);
    }

    /// <summary>
    ///   Checks whether the indexer method of the transforming read only collection
    ///   will throw an exception if it is attempted to be used for replacing an item
    /// </summary>
    [Test]
    public void TestThrowOnReplaceByIndexerViaIList() {
      StringTransformer testCollection = new StringTransformer(new int[1]);

      Assert.Throws<NotSupportedException>(
        delegate() { (testCollection as IList)[0] = "12345"; }
      );
    }

    /// <summary>
    ///   Verifies that the CopyTo() method of the transforming read only collection
    ///   works if invoked via the ICollection interface
    /// </summary>
    [Test]
    public void TestCopyToArrayViaICollection() {
      int[] inputIntegers = new int[] { 12, 34, 56, 78 };
      StringTransformer testCollection = new StringTransformer(inputIntegers);

      string[] outputStrings = new string[testCollection.Count];
      (testCollection as ICollection).CopyTo(outputStrings, 0);

      string[] inputStrings = new string[] { "12", "34", "56", "78" };
      CollectionAssert.AreEqual(inputStrings, outputStrings);
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    /// </summary>
    [Test]
    public void TestSynchronization() {
      StringTransformer testCollection = new StringTransformer(new int[0]);

      if(!(testCollection as ICollection).IsSynchronized) {
        lock((testCollection as ICollection).SyncRoot) {
          Assert.AreEqual(0, testCollection.Count);
        }
      }
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    ///   on transforming read only collections based on IList&lt;&gt;s that do not
    ///   implement the ICollection interface
    /// </summary>
    [Test]
    public void TestSynchronizationOfIListWithoutICollection() {
      Mockery mockery = new Mockery();
      IList<int> mockedIList = mockery.NewMock<IList<int>>();
      StringTransformer testCollection = new StringTransformer(mockedIList);

      if(!(testCollection as ICollection).IsSynchronized) {
        lock((testCollection as ICollection).SyncRoot) {
          Expect.Once.On(mockedIList).GetProperty("Count").Will(Return.Value(12345));
          int count = testCollection.Count;
          Assert.AreEqual(12345, count); // ;-)
        }
      }
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
