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

  /// <summary>Unit Test for the weak collection wrapper</summary>
  [TestFixture]
  public class WeakCollectionTest {

    #region class Dummy

    /// <summary>Dummy class used to test the weakly referencing collection</summary>
    private class Dummy {

      /// <summary>Initializes a new dummy</summary>
      /// <param name="value">Value that will be stored by the dummy</param>
      public Dummy(int value) {
        this.Value = value;
      }

      /// <summary>
      ///   Determines whether the specified System.Object is equal to
      ///   the current Dummy object.
      /// </summary>
      /// <param name="otherAsObject">
      ///   The System.Object to compare with the current Dummy object
      /// </param>
      /// <returns>
      ///   True if the specified System.Object is equal to the current Dummy object;
      ///   otherwise, false.
      /// </returns>
      public override bool Equals(object otherAsObject) {
        Dummy other = otherAsObject as Dummy;
        if(other == null) {
          return false;
        }
        return this.Value.Equals(other.Value);
      }

      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>A hash code for the current System.Object.</returns>
      public override int GetHashCode() {
        return this.Value.GetHashCode();
      }

      /// <summary>Some value that can be used for testing</summary>
      public int Value;

    }

    #endregion // class Dummy

    /// <summary>Verifies that the constructor of the weak collection is working</summary>
    [Test]
    public void TestConstructor() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Assert.IsNotNull(dummies);
    }

    /// <summary>
    ///   Test whether the non-typesafe Add() method of the weak collection works
    /// </summary>
    [Test]
    public void TestAddAsObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Dummy oneTwoThreeDummy = new Dummy(12345);
      (dummies as IList).Add((object)oneTwoThreeDummy);

      CollectionAssert.Contains(dummies, oneTwoThreeDummy);
    }

    /// <summary>
    ///   Test whether the non-typesafe Add() method throws an exception if an object is
    ///   added that is not compatible to the collection's item type
    /// </summary>
    [Test]
    public void TestThrowOnAddIncompatibleObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Assert.Throws<ArgumentException>(
        delegate() { (dummies as IList).Add(new object()); }
      );
    }

    /// <summary>
    ///   Test whether the generic Add() method of the weak collection works
    /// </summary>
    [Test]
    public void TestAdd() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Dummy oneTwoThreeDummy = new Dummy(12345);
      dummies.Add(oneTwoThreeDummy);

      CollectionAssert.Contains(dummies, oneTwoThreeDummy);
    }

    /// <summary>Tests whether the Clear() method works</summary>
    [Test]
    public void TestClear() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Dummy oneTwoThreeDummy = new Dummy(12345);
      dummies.Add(oneTwoThreeDummy);
      Dummy threeTwoOneDummy = new Dummy(54321);
      dummies.Add(threeTwoOneDummy);

      Assert.AreEqual(2, dummies.Count);

      dummies.Clear();

      Assert.AreEqual(0, dummies.Count);
    }

    /// <summary>Tests whether the Contains() method works</summary>
    [Test]
    public void TestContains() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Dummy oneTwoThreeDummy = new Dummy(12345);
      dummies.Add(oneTwoThreeDummy);
      Dummy threeTwoOneDummy = new Dummy(54321);

      Assert.IsTrue(dummies.Contains(oneTwoThreeDummy));
      Assert.IsFalse(dummies.Contains(threeTwoOneDummy));
    }

    /// <summary>Tests whether the non-typesafe Contains() method works</summary>
    [Test]
    public void TestContainsWithObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Dummy oneTwoThreeDummy = new Dummy(12345);
      dummies.Add(oneTwoThreeDummy);
      Dummy threeTwoOneDummy = new Dummy(54321);

      Assert.IsTrue((dummies as IList).Contains((object)oneTwoThreeDummy));
      Assert.IsFalse((dummies as IList).Contains((object)threeTwoOneDummy));
    }

    /// <summary>
    ///   Verifies that the Enumerator of the dummy collection correctly
    ///   implements the Reset() method
    /// </summary>
    [Test]
    public void TestEnumeratorReset() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      IEnumerator<Dummy> dummyEnumerator = dummies.GetEnumerator();
      Assert.IsTrue(dummyEnumerator.MoveNext());
      Assert.IsTrue(dummyEnumerator.MoveNext());
      Assert.IsFalse(dummyEnumerator.MoveNext());

      dummyEnumerator.Reset();

      Assert.IsTrue(dummyEnumerator.MoveNext());
      Assert.IsTrue(dummyEnumerator.MoveNext());
      Assert.IsFalse(dummyEnumerator.MoveNext());
    }

    /// <summary>Verifies that the IndexOf() method is working as intended</summary>
    [Test]
    public void TestIndexOf() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);
      Dummy sevenEightNineDummy = new Dummy(789);

      Assert.AreEqual(0, dummies.IndexOf(oneTwoThreeDummy));
      Assert.AreEqual(1, dummies.IndexOf(fourFiveSixDummy));
      Assert.AreEqual(-1, dummies.IndexOf(sevenEightNineDummy));
    }

    /// <summary>
    ///   Verifies that the non-typesafe IndexOf() method is working as intended
    /// </summary>
    [Test]
    public void TestIndexOfWithObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);
      Dummy sevenEightNineDummy = new Dummy(789);

      Assert.AreEqual(0, (dummies as IList).IndexOf((object)oneTwoThreeDummy));
      Assert.AreEqual(1, (dummies as IList).IndexOf((object)fourFiveSixDummy));
      Assert.AreEqual(-1, (dummies as IList).IndexOf((object)sevenEightNineDummy));
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an incompatible object is passed to
    ///   the non-typesafe variant of the IndexOf() method
    /// </summary>
    [Test]
    public void TestThrowOnIndexOfWithIncompatibleObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Assert.Throws<ArgumentException>(
        delegate() { Assert.IsNull((dummies as IList).IndexOf(new object())); }
      );
    }

    /// <summary>Test whether the IndexOf() method can cope with null references</summary>
    [Test]
    public void TestIndexOfNull() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Assert.AreEqual(-1, dummies.IndexOf(null));
      dummies.Add(null);
      Assert.AreEqual(0, dummies.IndexOf(null));
    }

    /// <summary>
    ///   Verifies that the CopyTo() method of the weak collection works
    /// </summary>
    [Test]
    public void TestCopyToArray() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Dummy[] inputDummies = new Dummy[] { oneTwoThreeDummy, fourFiveSixDummy };
      Dummy[] outputDummies = new Dummy[dummies.Count];

      dummies.CopyTo(outputDummies, 0);

      CollectionAssert.AreEqual(inputDummies, outputDummies);
    }

    /// <summary>
    ///   Verifies that the CopyTo() method of the weak collection throws an exception
    ///   if the target array is too small to hold the collection's contents
    /// </summary>
    [Test]
    public void TestThrowOnCopyToTooSmallArray() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Dummy[] outputStrings = new Dummy[dummies.Count - 1];
      Assert.Throws<ArgumentException>(
        delegate() { dummies.CopyTo(outputStrings, 0); }
      );
    }

    /// <summary>
    ///   Verifies that the CopyTo() method of the transforming read only collection
    ///   works if invoked via the ICollection interface
    /// </summary>
    [Test]
    public void TestCopyToArrayViaICollection() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Dummy[] inputDummies = new Dummy[] { oneTwoThreeDummy, fourFiveSixDummy };
      Dummy[] outputDummies = new Dummy[dummies.Count];

      (dummies as ICollection).CopyTo(outputDummies, 0);

      CollectionAssert.AreEqual(inputDummies, outputDummies);
    }

    /// <summary>
    ///   Verifies that the Insert() method correctly shifts items in the collection
    /// </summary>
    [Test]
    public void TestInsert() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);

      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Insert(0, fourFiveSixDummy);

      Assert.AreEqual(2, dummies.Count);
      Assert.AreSame(fourFiveSixDummy, dummies[0]);
      Assert.AreSame(oneTwoThreeDummy, dummies[1]);
    }

    /// <summary>
    ///   Verifies that the non-typesafe Insert() method correctly shifts items in
    ///   the collection
    /// </summary>
    [Test]
    public void TestInsertObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);

      Dummy fourFiveSixDummy = new Dummy(456);
      (dummies as IList).Insert(0, (object)fourFiveSixDummy);

      Assert.AreEqual(2, dummies.Count);
      Assert.AreSame(fourFiveSixDummy, dummies[0]);
      Assert.AreSame(oneTwoThreeDummy, dummies[1]);
    }

    /// <summary>
    ///   Verifies that the non-typesafe Insert() method correctly shifts items in
    ///   the collection
    /// </summary>
    [Test]
    public void TestThrowOnInsertIncompatibleObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);

      Dummy fourFiveSixDummy = new Dummy(456);
      Assert.Throws<ArgumentException>(
        delegate() { (dummies as IList).Insert(0, new object()); }
      );
    }

    /// <summary>
    ///   Checks whether the IsFixedSize property of the weak collection returns
    ///   the expected result for a weak collection based on a fixed array
    /// </summary>
    [Test]
    public void TestIsFixedSizeViaIList() {
      Dummy oneTwoThreeDummy = new Dummy(123);
      Dummy fourFiveSixDummy = new Dummy(456);

      WeakReference<Dummy>[] dummyReferences = new WeakReference<Dummy>[] {
        new WeakReference<Dummy>(oneTwoThreeDummy),
        new WeakReference<Dummy>(fourFiveSixDummy)
      };
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(dummyReferences);

      Assert.IsTrue((dummies as IList).IsFixedSize);
    }

    /// <summary>
    ///   Tests whether the IsReadOnly property of the weak collection works
    /// </summary>
    [Test]
    public void TestIsReadOnly() {
      Dummy oneTwoThreeDummy = new Dummy(123);
      Dummy fourFiveSixDummy = new Dummy(456);

      List<WeakReference<Dummy>> dummyReferences = new List<WeakReference<Dummy>>();
      dummyReferences.Add(new WeakReference<Dummy>(oneTwoThreeDummy));
      dummyReferences.Add(new WeakReference<Dummy>(fourFiveSixDummy));

      ReadOnlyList<WeakReference<Dummy>> readOnlyDummyReferences =
        new ReadOnlyList<WeakReference<Dummy>>(dummyReferences);

      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(dummyReferences);
      WeakCollection<Dummy> readOnlydummies = new WeakCollection<Dummy>(
        readOnlyDummyReferences
      );

      Assert.IsFalse(dummies.IsReadOnly);
      Assert.IsTrue(readOnlydummies.IsReadOnly);
    }

    /// <summary>
    ///   Tests whether the IsSynchronized property of the weak collection works
    /// </summary>
    [Test]
    public void TestIsSynchronized() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      Assert.IsFalse((dummies as IList).IsSynchronized);
    }

    /// <summary>Tests the indexer of the weak collection</summary>
    [Test]
    public void TestIndexer() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Assert.AreSame(oneTwoThreeDummy, dummies[0]);
      Assert.AreSame(fourFiveSixDummy, dummies[1]);

      dummies[0] = fourFiveSixDummy;

      Assert.AreSame(fourFiveSixDummy, dummies[0]);
    }

    /// <summary>Tests the non-typesafe indexer of the weak collection</summary>
    [Test]
    public void TestIndexerWithObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Assert.AreSame((object)oneTwoThreeDummy, (dummies as IList)[0]);
      Assert.AreSame((object)fourFiveSixDummy, (dummies as IList)[1]);

      (dummies as IList)[0] = (object)fourFiveSixDummy;

      Assert.AreSame((object)fourFiveSixDummy, (dummies as IList)[0]);
    }

    /// <summary>
    ///   Tests whether the non-typesafe indexer of the weak collection throws
    ///   the correct exception if an incompatible object is assigned
    /// </summary>
    [Test]
    public void TestThrowOnIndexerWithIncompatibleObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);

      Assert.Throws<ArgumentException>(
        delegate() { (dummies as IList)[0] = new object(); }
      );
    }

    /// <summary>Tests the Remove() method of the weak collection</summary>
    [Test]
    public void TestRemove() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Assert.AreEqual(2, dummies.Count);
      Assert.IsTrue(dummies.Remove(oneTwoThreeDummy));
      Assert.AreEqual(1, dummies.Count);
      Assert.IsFalse(dummies.Remove(oneTwoThreeDummy));
    }

    /// <summary>Tests the non-typesafe Remove() method of the weak collection</summary>
    [Test]
    public void TestRemoveObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Assert.AreEqual(2, dummies.Count);
      (dummies as IList).Remove((object)oneTwoThreeDummy);
      Assert.AreEqual(1, dummies.Count);
    }

    /// <summary>
    ///   Tests whether a null object can be managed by and removed from the weak collection
    /// </summary>
    [Test]
    public void TestRemoveNull() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      dummies.Add(null);

      Assert.AreEqual(1, dummies.Count);
      Assert.IsTrue(dummies.Remove(null));
      Assert.AreEqual(0, dummies.Count);
    }

    /// <summary>
    ///   Tests whether the non-typesafe Remove() method of the weak collection throws
    ///   an exception if an object is tried to be removed that is incompatible with
    ///   the collection's item type
    /// </summary>
    [Test]
    public void TestThrowOnRemoveIncompatibleObject() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Assert.Throws<ArgumentException>(
        delegate() { (dummies as IList).Remove(new object()); }
      );
    }

    /// <summary>Tests the RemoveAt() method of the weak collection</summary>
    [Test]
    public void TestRemoveAt() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );
      Dummy oneTwoThreeDummy = new Dummy(123);
      dummies.Add(oneTwoThreeDummy);
      Dummy fourFiveSixDummy = new Dummy(456);
      dummies.Add(fourFiveSixDummy);

      Assert.AreSame(oneTwoThreeDummy, dummies[0]);
      dummies.RemoveAt(0);
      Assert.AreSame(fourFiveSixDummy, dummies[0]);
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    /// </summary>
    [Test]
    public void TestSynchronization() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new List<WeakReference<Dummy>>()
      );

      if(!(dummies as ICollection).IsSynchronized) {
        lock((dummies as ICollection).SyncRoot) {
          Assert.AreEqual(0, dummies.Count);
        }
      }
    }

    private class ListWithoutICollection : IList<WeakReference<Dummy>> {
      public int IndexOf(WeakReference<Dummy> item) { throw new NotImplementedException(); }
      public void Insert(int index, WeakReference<Dummy> item) {
        throw new NotImplementedException();
      }
      public void RemoveAt(int index) { throw new NotImplementedException(); }
      public WeakReference<Dummy> this[int index] {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }
      public void Add(WeakReference<Dummy> item) { throw new NotImplementedException(); }
      public void Clear() { throw new NotImplementedException(); }
      public bool Contains(WeakReference<Dummy> item) { throw new NotImplementedException(); }
      public void CopyTo(WeakReference<Dummy>[] array, int arrayIndex) {
        throw new NotImplementedException();
      }
      public int Count { get { return 12345; } }
      public bool IsReadOnly { get { throw new NotImplementedException(); } }
      public bool Remove(WeakReference<Dummy> item) { throw new NotImplementedException(); }
      public IEnumerator<WeakReference<Dummy>> GetEnumerator() {
        throw new NotImplementedException();
      }
      IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    ///   on transforming read only collections based on IList&lt;&gt;s that do not
    ///   implement the ICollection interface
    /// </summary>
    [Test]
    public void TestSynchronizationOfIListWithoutICollection() {
      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(
        new ListWithoutICollection()
      );

      if(!(dummies as ICollection).IsSynchronized) {
        lock((dummies as ICollection).SyncRoot) {
          int count = dummies.Count;
          Assert.AreEqual(12345, count); // ;-)
        }
      }
    }

    /// <summary>Tests the RemoveDeadItems() method</summary>
    [Test]
    public void TestRemoveDeadItems() {
      List<WeakReference<Dummy>> dummyReferences = new List<WeakReference<Dummy>>();

      Dummy oneTwoThreeDummy = new Dummy(123);
      dummyReferences.Add(new WeakReference<Dummy>(oneTwoThreeDummy));

      dummyReferences.Add(new WeakReference<Dummy>(null));

      Dummy fourFiveSixDummy = new Dummy(456);
      dummyReferences.Add(new WeakReference<Dummy>(fourFiveSixDummy));

      WeakCollection<Dummy> dummies = new WeakCollection<Dummy>(dummyReferences);

      Assert.AreEqual(3, dummies.Count);

      dummies.RemoveDeadItems();

      Assert.AreEqual(2, dummies.Count);
      Assert.AreSame(oneTwoThreeDummy, dummies[0]);
      Assert.AreSame(fourFiveSixDummy, dummies[1]);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
