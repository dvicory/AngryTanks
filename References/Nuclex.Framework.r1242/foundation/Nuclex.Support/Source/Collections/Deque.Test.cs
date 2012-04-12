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

#if UNITTEST

using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the double ended queue</summary>
  [TestFixture]
  public class DequeTest {

    /// <summary>Verifies that the AddLast() method of the deque is working</summary>
    [Test]
    public void TestAddLast() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(item, intDeque[item]);
      }
    }

    /// <summary>Verifies that the AddFirst() method of the deque is working</summary>
    [Test]
    public void TestAddFirst() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddFirst(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(47 - item, intDeque[item]);
      }
    }

    /// <summary>
    ///   Verifies that the RemoveFirst() method of the deque is working
    /// </summary>
    [Test]
    public void TestRemoveFirst() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(item, intDeque.First);
        Assert.AreEqual(48 - item, intDeque.Count);
        intDeque.RemoveFirst();
      }
    }

    /// <summary>
    ///   Verifies that the RemoveLast() method of the deque is working
    /// </summary>
    [Test]
    public void TestRemoveLast() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 48; ++item) {
        intDeque.AddLast(item);
      }

      for(int item = 0; item < 48; ++item) {
        Assert.AreEqual(47 - item, intDeque.Last);
        Assert.AreEqual(48 - item, intDeque.Count);
        intDeque.RemoveLast();
      }
    }

    /// <summary>Verifies that the Insert() method works in all cases</summary>
    /// <remarks>
    ///   We have several different cases here that will be tested. The deque can
    ///   shift items to the left or right (depending on which end is closer to
    ///   the insertion point) and the insertion point may fall in an only partially
    ///   occupied block, requiring elaborate index calculations
    /// </remarks>
    [Test]
    public void TestInsert() {
      for(int testedIndex = 0; testedIndex <= 96; ++testedIndex) {
        Deque<int> intDeque = createDeque(96);

        intDeque.Insert(testedIndex, 12345);

        Assert.AreEqual(97, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        Assert.AreEqual(12345, intDeque[testedIndex]);
        for(int index = testedIndex + 1; index < 97; ++index) {
          Assert.AreEqual(index - 1, intDeque[index]);
        }
      }
    }

    /// <summary>
    ///   Verifies that the Insert() method works in all cases when the deque doesn't
    ///   start at a block boundary
    /// </summary>
    [Test]
    public void TestInsertNonNormalized() {
      for(int testedIndex = 0; testedIndex <= 96; ++testedIndex) {
        Deque<int> intDeque = createNonNormalizedDeque(96);

        intDeque.Insert(testedIndex, 12345);

        Assert.AreEqual(97, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        Assert.AreEqual(12345, intDeque[testedIndex]);
        for(int index = testedIndex + 1; index < 97; ++index) {
          Assert.AreEqual(index - 1, intDeque[index]);
        }
      }
    }

    /// <summary>Verifies the the RemoveAt() method works in all cases</summary>
    [Test]
    public void TestRemoveAt() {
      for(int testedIndex = 0; testedIndex < 96; ++testedIndex) {
        Deque<int> intDeque = new Deque<int>(16);
        for(int item = 0; item < 96; ++item) {
          intDeque.AddLast(item);
        }

        intDeque.RemoveAt(testedIndex);

        Assert.AreEqual(95, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        for(int index = testedIndex; index < 95; ++index) {
          Assert.AreEqual(index + 1, intDeque[index]);
        }
      }
    }

    /// <summary>
    ///   Verifies the the RemoveAt() method works in all cases when the deque doesn't
    ///   start at a block boundary
    /// </summary>
    [Test]
    public void TestRemoveAtNonNormalized() {
      for(int testedIndex = 0; testedIndex < 96; ++testedIndex) {
        Deque<int> intDeque = new Deque<int>(16);
        for(int item = 4; item < 96; ++item) {
          intDeque.AddLast(item);
        }
        intDeque.AddFirst(3);
        intDeque.AddFirst(2);
        intDeque.AddFirst(1);
        intDeque.AddFirst(0);

        intDeque.RemoveAt(testedIndex);

        Assert.AreEqual(95, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        for(int index = testedIndex; index < 95; ++index) {
          Assert.AreEqual(index + 1, intDeque[index]);
        }
      }
    }

    /// <summary>
    ///   Tests whether the RemoveAt() method keeps the state of the deque intact when
    ///   it has to remove a block from the left end of the deque
    /// </summary>
    [Test]
    public void TestRemoveAtEmptiesLeftBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 1; item <= 16; ++item) {
        intDeque.AddLast(item);
      }
      intDeque.AddFirst(0);
      intDeque.RemoveAt(3);

      Assert.AreEqual(16, intDeque.Count);

      for(int index = 0; index < 3; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      for(int index = 3; index < 16; ++index) {
        Assert.AreEqual(index + 1, intDeque[index]);
      }
    }

    /// <summary>
    ///   Tests whether the RemoveAt() method keeps the state of the deque intact when
    ///   it has to remove a block from the right end of the deque
    /// </summary>
    [Test]
    public void TestRemoveAtEmptiesRightBlock() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item <= 16; ++item) {
        intDeque.AddLast(item);
      }
      intDeque.RemoveAt(13);

      Assert.AreEqual(16, intDeque.Count);

      for(int index = 0; index < 13; ++index) {
        Assert.AreEqual(index, intDeque[index]);
      }
      for(int index = 13; index < 16; ++index) {
        Assert.AreEqual(index + 1, intDeque[index]);
      }
    }

    /// <summary>
    ///   Validates that an exception is thrown if the 'First' property is accessed
    ///   in an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnAccessFirstInEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { Console.WriteLine(intDeque.First); }
      );
    }

    /// <summary>
    ///   Validates that an exception is thrown if the 'Last' property is accessed
    ///   in an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnAccessLastInEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { Console.WriteLine(intDeque.Last); }
      );
    }

    /// <summary>
    ///   Validates that an exception is thrown if the first item is attempted to be 
    ///   removed from an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnRemoveFirstFromEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { intDeque.RemoveFirst(); }
      );
    }

    /// <summary>
    ///   Validates that an exception is thrown if the last item is attempted to be 
    ///   removed from an empty deque
    /// </summary>
    [Test]
    public void TestThrowOnRemoveLastFromEmptyDeque() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<InvalidOperationException>(
        delegate() { intDeque.RemoveLast(); }
      );
    }

    /// <summary>
    ///   Verifies that items can be assigned by their index
    /// </summary>
    [Test]
    public void TestIndexAssignment() {
      Deque<int> intDeque = createDeque(32);
      intDeque[16] = 12345;
      intDeque[17] = 54321;

      for(int index = 0; index < 16; ++index) {
        intDeque.RemoveFirst();
      }

      Assert.AreEqual(12345, intDeque.First);
      intDeque.RemoveFirst();
      Assert.AreEqual(54321, intDeque.First);
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an invalid index is accessed
    /// </summary>
    [Test]
    public void TestThrowOnInvalidIndex() {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < 32; ++item) {
        intDeque.AddLast(item);
      }

      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { Console.WriteLine(intDeque[32]); }
      );
    }

    /// <summary>Tests the IndexOf() method</summary>
    [Test, TestCase(0), TestCase(16), TestCase(32), TestCase(48)]
    public void TestIndexOf(int count) {
      Deque<int> intDeque = new Deque<int>(16);
      for(int item = 0; item < count; ++item) {
        intDeque.AddLast(item);
      }

      for(int item = 0; item < count; ++item) {
        Assert.AreEqual(item, intDeque.IndexOf(item));
      }
      Assert.AreEqual(-1, intDeque.IndexOf(count));
    }

    /// <summary>
    ///   Tests the IndexOf() method with the deque not starting at a block boundary
    /// </summary>
    [Test, TestCase(0), TestCase(16), TestCase(32), TestCase(48)]
    public void TestIndexOfNonNormalized(int count) {
      Deque<int> intDeque = createNonNormalizedDeque(count);

      for(int item = 0; item < count; ++item) {
        Assert.AreEqual(item, intDeque.IndexOf(item));
      }
      Assert.AreEqual(-1, intDeque.IndexOf(count));
    }

    /// <summary>Verifies that the deque's enumerator works</summary>
    [Test]
    public void TestEnumerator() {
      Deque<int> intDeque = createNonNormalizedDeque(40);

      for(int testRun = 0; testRun < 2; ++testRun) {
        using(IEnumerator<int> enumerator = intDeque.GetEnumerator()) {
          for(int index = 0; index < intDeque.Count; ++index) {
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(index, enumerator.Current);
          }

          Assert.IsFalse(enumerator.MoveNext());

          enumerator.Reset();
        }
      }
    }

    /// <summary>Verifies that the deque's enumerator works</summary>
    [Test]
    public void TestObjectEnumerator() {
      Deque<int> intDeque = createNonNormalizedDeque(40);

      for(int testRun = 0; testRun < 2; ++testRun) {
        IEnumerator enumerator = ((IEnumerable)intDeque).GetEnumerator();
        for(int index = 0; index < intDeque.Count; ++index) {
          Assert.IsTrue(enumerator.MoveNext());
          Assert.AreEqual(index, enumerator.Current);
        }

        Assert.IsFalse(enumerator.MoveNext());

        enumerator.Reset();
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if the enumerator is accessed in
    ///   an invalid position
    /// </summary>
    [Test]
    public void TestThrowOnInvalidEnumeratorPosition() {
      Deque<int> intDeque = createNonNormalizedDeque(40);

      using(IEnumerator<int> enumerator = intDeque.GetEnumerator()) {
        Assert.Throws<InvalidOperationException>(
          delegate() { Console.WriteLine(enumerator.Current); }
        );

        while(enumerator.MoveNext()) { }

        Assert.Throws<InvalidOperationException>(
          delegate() { Console.WriteLine(enumerator.Current); }
        );
      }
    }

    /// <summary>Tests whether a small deque can be cleared</summary>
    [Test]
    public void TestClearSmallDeque() {
      Deque<int> intDeque = createDeque(12);
      intDeque.Clear();
      Assert.AreEqual(0, intDeque.Count);
    }

    /// <summary>Tests whether a large deque can be cleared</summary>
    [Test]
    public void TestClearLargeDeque() {
      Deque<int> intDeque = createDeque(40);
      intDeque.Clear();
      Assert.AreEqual(0, intDeque.Count);
    }

    /// <summary>Verifies that the non-typesafe Add() method is working</summary>
    [Test]
    public void TestAddObject() {
      Deque<int> intDeque = new Deque<int>();
      Assert.AreEqual(0, ((IList)intDeque).Add(123));
      Assert.AreEqual(1, intDeque.Count);
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the non-typesafe Add() method is
    ///   used to add an incompatible object into the deque
    /// </summary>
    [Test]
    public void TestThrowOnAddIncompatibleObject() {
      Deque<int> intDeque = new Deque<int>();
      Assert.Throws<ArgumentException>(
        delegate() { ((IList)intDeque).Add("Hello World"); }
      );
    }

    /// <summary>Verifies that the Add() method is working</summary>
    [Test]
    public void TestAdd() {
      Deque<int> intDeque = new Deque<int>();
      ((IList<int>)intDeque).Add(123);
      Assert.AreEqual(1, intDeque.Count);
    }

    /// <summary>Tests whether the Contains() method is working</summary>
    [Test]
    public void TestContains() {
      Deque<int> intDeque = createDeque(16);
      Assert.IsTrue(intDeque.Contains(14));
      Assert.IsFalse(intDeque.Contains(16));
    }

    /// <summary>Tests the non-typesafe Contains() method</summary>
    [Test]
    public void TestContainsObject() {
      Deque<int> intDeque = createDeque(16);
      Assert.IsTrue(((IList)intDeque).Contains(14));
      Assert.IsFalse(((IList)intDeque).Contains(16));
      Assert.IsFalse(((IList)intDeque).Contains("Hello World"));
    }

    /// <summary>Tests the non-typesafe Contains() method</summary>
    [Test]
    public void TestIndexOfObject() {
      Deque<int> intDeque = createDeque(16);
      Assert.AreEqual(14, ((IList)intDeque).IndexOf(14));
      Assert.AreEqual(-1, ((IList)intDeque).IndexOf(16));
      Assert.AreEqual(-1, ((IList)intDeque).IndexOf("Hello World"));
    }

    /// <summary>Tests wether the non-typesafe Insert() method is working</summary>
    [Test]
    public void TestInsertObject() {
      for(int testedIndex = 0; testedIndex <= 96; ++testedIndex) {
        Deque<int> intDeque = createDeque(96);

        ((IList)intDeque).Insert(testedIndex, 12345);

        Assert.AreEqual(97, intDeque.Count);

        for(int index = 0; index < testedIndex; ++index) {
          Assert.AreEqual(index, intDeque[index]);
        }
        Assert.AreEqual(12345, intDeque[testedIndex]);
        for(int index = testedIndex + 1; index < 97; ++index) {
          Assert.AreEqual(index - 1, intDeque[index]);
        }
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if an incompatible object is inserted
    ///   into the deque
    /// </summary>
    [Test]
    public void TestThrowOnInsertIncompatibleObject() {
      Deque<int> intDeque = createDeque(12);
      Assert.Throws<ArgumentException>(
        delegate() { ((IList)intDeque).Insert(8, "Hello World"); }
      );
    }

    /// <summary>Validates that the IsFixedObject property is set to false</summary>
    [Test]
    public void TestIsFixedObject() {
      Deque<int> intDeque = new Deque<int>();
      Assert.IsFalse(((IList)intDeque).IsFixedSize);
    }

    /// <summary>Validates that the IsSynchronized property is set to false</summary>
    [Test]
    public void TestIsSynchronized() {
      Deque<int> intDeque = new Deque<int>();
      Assert.IsFalse(((IList)intDeque).IsSynchronized);
    }

    /// <summary>
    ///   Verifies that items can be assigned by their index using the non-typesafe
    ///   IList interface
    /// </summary>
    [Test]
    public void TestObjectIndexAssignment() {
      Deque<int> intDeque = createDeque(32);

      ((IList)intDeque)[16] = 12345;
      ((IList)intDeque)[17] = 54321;

      Assert.AreEqual(12345, ((IList)intDeque)[16]);
      Assert.AreEqual(54321, ((IList)intDeque)[17]);
    }

    /// <summary>
    ///   Tests whether an exception is thrown if an incompatible object is assigned
    ///   to the deque
    /// </summary>
    [Test]
    public void TestIncompatibleObjectIndexAssignment() {
      Deque<int> intDeque = createDeque(2);
      Assert.Throws<ArgumentException>(
        delegate() { ((IList)intDeque)[0] = "Hello World"; }
      );
    }

    /// <summary>Verifies that the Remove() method is working correctly</summary>
    [Test]
    public void TestRemove() {
      Deque<int> intDeque = createDeque(16);
      Assert.AreEqual(16, intDeque.Count);
      Assert.IsTrue(intDeque.Remove(13));
      Assert.IsFalse(intDeque.Remove(13));
      Assert.AreEqual(15, intDeque.Count);
    }

    /// <summary>Tests the non-typesafe remove method</summary>
    [Test]
    public void TestRemoveObject() {
      Deque<int> intDeque = createDeque(10);
      Assert.IsTrue(intDeque.Contains(8));
      Assert.AreEqual(10, intDeque.Count);
      ((IList)intDeque).Remove(8);
      Assert.IsFalse(intDeque.Contains(8));
      Assert.AreEqual(9, intDeque.Count);
    }

    /// <summary>
    ///   Tests the non-typesafe remove method used to remove an incompatible object
    /// </summary>
    [Test]
    public void TestRemoveIncompatibleObject() {
      Deque<int> intDeque = createDeque(10);
      ((IList)intDeque).Remove("Hello World"); // should simply do nothing
      Assert.AreEqual(10, intDeque.Count);
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    /// </summary>
    [Test]
    public void TestSynchronization() {
      Deque<int> intDeque = new Deque<int>();

      if(!(intDeque as ICollection).IsSynchronized) {
        lock((intDeque as ICollection).SyncRoot) {
          Assert.AreEqual(0, intDeque.Count);
        }
      }
    }

    /// <summary>
    ///   Validates that the IsReadOnly property of the deque returns false
    /// </summary>
    [Test]
    public void TestIsReadOnly() {
      Deque<int> intDeque = new Deque<int>();
      Assert.IsFalse(((IList)intDeque).IsReadOnly);
      Assert.IsFalse(((ICollection<int>)intDeque).IsReadOnly);
    }

    /// <summary>Tests the non-typesafe CopyTo() method</summary>
    [Test]
    public void TestCopyToObjectArray() {
      Deque<int> intDeque = createNonNormalizedDeque(40);

      int[] array = new int[40];
      ((ICollection)intDeque).CopyTo(array, 0);

      Assert.AreEqual(intDeque, array);
    }

    /// <summary>Tests the CopyTo() method</summary>
    [Test]
    public void TestCopyToArray() {
      Deque<int> intDeque = createDeque(12);
      intDeque.RemoveFirst();
      intDeque.RemoveFirst();

      int[] array = new int[14];
      intDeque.CopyTo(array, 4);

      Assert.AreEqual(
        new int[] { 0, 0, 0, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
        array
      );
    }

    /// <summary>
    ///   Verifies that the non-typesafe CopyTo() method throws an exception if
    ///   the array is of an incompatible type
    /// </summary>
    [Test]
    public void TestThrowOnCopyToIncompatibleObjectArray() {
      Deque<int> intDeque = createDeque(4);

      short[] array = new short[4];
      Assert.Throws<ArgumentException>(
        delegate() { ((ICollection)intDeque).CopyTo(array, 4); }
      );
    }

    /// <summary>
    ///   Verifies that the CopyTo() method throws an exception if the target array
    ///   is too small
    /// </summary>
    [Test]
    public void TestThrowOnCopyToTooSmallArray() {
      Deque<int> intDeque = createDeque(8);
      Assert.Throws<ArgumentException>(
        delegate() { intDeque.CopyTo(new int[7], 0); }
      );
    }
    
#if DEBUG
    /// <summary>
    ///   Tests whether the deque enumerator detects when it runs out of sync
    /// </summary>
    [Test]
    public void TestInvalidatedEnumeratorDetection() {
      Deque<int> intDeque = createDeque(8);
      using(IEnumerator<int> enumerator = intDeque.GetEnumerator()) {
        Assert.IsTrue(enumerator.MoveNext());
        intDeque.AddFirst(12345);
        Assert.Throws<InvalidOperationException>(
          delegate() { enumerator.MoveNext(); }
        );
      }      
    }
#endif

    /// <summary>
    ///   Creates a deque whose first element does not coincide with a block boundary
    /// </summary>
    /// <param name="count">Number of items the deque will be filled with</param>
    /// <returns>The newly created deque</returns>
    private static Deque<int> createNonNormalizedDeque(int count) {
      Deque<int> intDeque = new Deque<int>(16);

      for(int item = 4; item < count; ++item) {
        intDeque.AddLast(item);
      }
      if(count > 3) { intDeque.AddFirst(3); }
      if(count > 2) { intDeque.AddFirst(2); }
      if(count > 1) { intDeque.AddFirst(1); }
      if(count > 0) { intDeque.AddFirst(0); }

      return intDeque;
    }

    /// <summary>Creates a deque filled with the specified number of items
    /// </summary>
    /// <param name="count">Number of items the deque will be filled with</param>
    /// <returns>The newly created deque</returns>
    private static Deque<int> createDeque(int count) {
      Deque<int> intDeque = new Deque<int>(16);

      for(int item = 0; item < count; ++item) {
        intDeque.AddLast(item);
      }

      return intDeque;
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
