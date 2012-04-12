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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the read only dictionary wrapper</summary>
  [TestFixture]
  public class ReadOnlyDictionaryTest {

    /// <summary>
    ///   Verifies that the copy constructor of the read only dictionary works
    /// </summary>
    [Test]
    public void TestCopyConstructor() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      CollectionAssert.AreEqual(numbers, testDictionary);
    }

    /// <summary>Verifies that the IsReadOnly property returns true</summary>
    [Test]
    public void TestIsReadOnly() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.IsTrue(testDictionary.IsReadOnly);
    }

    /// <summary>
    ///   Checks whether the Contains() method of the read only dictionary is able to
    ///   determine if the dictionary contains an item
    /// </summary>
    [Test]
    public void TestContains() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.IsTrue(
        testDictionary.Contains(new KeyValuePair<int, string>(42, "forty-two"))
      );
      Assert.IsFalse(
        testDictionary.Contains(new KeyValuePair<int, string>(24, "twenty-four"))
      );
    }

    /// <summary>
    ///   Checks whether the Contains() method of the read only dictionary is able to
    ///   determine if the dictionary contains a key
    /// </summary>
    [Test]
    public void TestContainsKey() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.IsTrue(testDictionary.ContainsKey(42));
      Assert.IsFalse(testDictionary.ContainsKey(24));
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the read only dictionary works
    /// </summary>
    [Test]
    public void TestCopyToArray() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      KeyValuePair<int, string>[] items = new KeyValuePair<int, string>[numbers.Count];

      testDictionary.CopyTo(items, 0);

      CollectionAssert.AreEqual(numbers, items);
    }

    /// <summary>
    ///   Tests whether the typesafe enumerator of the read only dictionary is working
    /// </summary>
    [Test]
    public void TestTypesafeEnumerator() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      List<KeyValuePair<int, string>> outputItems = new List<KeyValuePair<int, string>>();
      foreach(KeyValuePair<int, string> item in testDictionary) {
        outputItems.Add(item);
      }

      CollectionAssert.AreEqual(numbers, outputItems);
    }

    /// <summary>
    ///   Tests whether the keys collection of the read only dictionary can be queried
    /// </summary>
    [Test]
    public void TestGetKeysCollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      ICollection<int> inputKeys = numbers.Keys;
      ICollection<int> keys = testDictionary.Keys;
      CollectionAssert.AreEqual(inputKeys, keys);
    }

    /// <summary>
    ///   Tests whether the values collection of the read only dictionary can be queried
    /// </summary>
    [Test]
    public void TestGetValuesCollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      ICollection<string> inputValues = numbers.Values;
      ICollection<string> values = testDictionary.Values;
      CollectionAssert.AreEqual(inputValues, values);
    }

    /// <summary>
    ///   Tests whether the TryGetValue() method of the read only dictionary is working
    /// </summary>
    [Test]
    public void TestTryGetValue() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      string value;

      Assert.IsTrue(testDictionary.TryGetValue(42, out value));
      Assert.AreEqual("forty-two", value);

      Assert.IsFalse(testDictionary.TryGetValue(24, out value));
      Assert.AreEqual(null, value);
    }

    /// <summary>
    ///   Tests whether the retrieval of values using the indexer of the read only
    ///   dictionary is working
    /// </summary>
    [Test]
    public void TestRetrieveValueByIndexer() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.AreEqual("forty-two", testDictionary[42]);
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the indexer of the read only dictionary
    ///   is used to attempt to retrieve a non-existing value
    /// </summary>
    [Test]
    public void TestThrowOnRetrieveNonExistingValueByIndexer() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<KeyNotFoundException>(
        delegate() { Console.WriteLine(testDictionary[24]); }
      );
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Add() method is called via the generic IDictionary&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnAddViaGenericIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as IDictionary<int, string>).Add(10, "ten"); }
      );
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Remove() method is called via the generic IDictionary&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaGenericIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as IDictionary<int, string>).Remove(3); }
      );
    }

    /// <summary>
    ///   Tests whether the TryGetValue() method of the read only dictionary is working
    /// </summary>
    [Test]
    public void TestRetrieveValueByIndexerViaGenericIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.AreEqual("forty-two", (testDictionary as IDictionary<int, string>)[42]);
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   indexer is used to insert an item via the generic IDictionar&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnReplaceByIndexerViaGenericIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as IDictionary<int, string>)[24] = "twenty-four"; }
      );
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Clear() method is called via the IDictionary interface
    /// </summary>
    [Test]
    public void TestThrowOnClearViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as IDictionary).Clear(); }
      );
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Add() method is called via the IDictionary interface
    /// </summary>
    [Test]
    public void TestThrowOnAddViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);
      
      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as IDictionary).Add(24, "twenty-four"); }
      );
    }

    /// <summary>
    ///   Checks whether the Contains() method of the read only dictionary is able to
    ///   determine if the dictionary contains an item via the IDictionary interface
    /// </summary>
    [Test]
    public void TestContainsViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.IsTrue((testDictionary as IDictionary).Contains(42));
      Assert.IsFalse((testDictionary as IDictionary).Contains(24));
    }

    /// <summary>
    ///   Checks whether the GetEnumerator() method of the read only dictionary returns
    ///   a working enumerator if accessed via the IDictionary interface
    /// </summary>
    [Test]
    public void TestEnumeratorViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Dictionary<int, string> outputNumbers = new Dictionary<int, string>();
      foreach(DictionaryEntry entry in (testDictionary as IDictionary)) {
        (outputNumbers as IDictionary).Add(entry.Key, entry.Value);
      }

      CollectionAssert.AreEquivalent(numbers, outputNumbers);
    }

    /// <summary>
    ///   Checks whether the IsFixedSize property of the read only dictionary returns
    ///   the expected result for a read only dictionary based on a dynamic dictionary
    /// </summary>
    [Test]
    public void TestIsFixedSizeViaIList() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.IsFalse((testDictionary as IDictionary).IsFixedSize);
    }

    /// <summary>
    ///   Tests whether the keys collection of the read only dictionary can be queried
    ///   via the IDictionary interface
    /// </summary>
    [Test]
    public void TestGetKeysCollectionViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      ICollection inputKeys = (numbers as IDictionary).Keys;
      ICollection keys = (testDictionary as IDictionary).Keys;
      CollectionAssert.AreEqual(inputKeys, keys);
    }

    /// <summary>
    ///   Tests whether the values collection of the read only dictionary can be queried
    ///   via the IDictionary interface
    /// </summary>
    [Test]
    public void TestGetValuesCollectionViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      ICollection inputValues = (numbers as IDictionary).Values;
      ICollection values = (testDictionary as IDictionary).Values;
      CollectionAssert.AreEqual(inputValues, values);
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Remove() method is called via the IDictionary interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as IDictionary).Remove(3); }
      );
    }

    /// <summary>
    ///   Tests whether the retrieval of values using the indexer of the read only
    ///   dictionary is working via the IDictionary interface
    /// </summary>
    [Test]
    public void TestRetrieveValueByIndexerViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.AreEqual("forty-two", (testDictionary as IDictionary)[42]);
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   indexer is used to insert an item via the IDictionary interface
    /// </summary>
    [Test]
    public void TestThrowOnReplaceByIndexerViaIDictionary() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as IDictionary)[24] = "twenty-four"; }
      );
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Add() method is used via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnAddViaGenericICollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() {
          (testDictionary as ICollection<KeyValuePair<int, string>>).Add(
            new KeyValuePair<int, string>(24, "twenty-four")
          );
        }
      );
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Clear() method is used via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnClearViaGenericICollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() { (testDictionary as ICollection<KeyValuePair<int, string>>).Clear(); }
      );
    }

    /// <summary>
    ///   Checks whether the read only dictionary will throw an exception if its
    ///   Remove() method is used via the generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestThrowOnRemoveViaGenericICollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      Assert.Throws<NotSupportedException>(
        delegate() {
          (testDictionary as ICollection<KeyValuePair<int, string>>).Remove(
            new KeyValuePair<int, string>(42, "fourty-two")
          );
        }
      );
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the read only dictionary works when called
    ///   via the the ICollection interface
    /// </summary>
    [Test]
    public void TestCopyToArrayViaICollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      DictionaryEntry[] entries = new DictionaryEntry[numbers.Count];
      (testDictionary as ICollection).CopyTo(entries, 0);

      KeyValuePair<int, string>[] items = new KeyValuePair<int, string>[numbers.Count];
      for(int index = 0; index < entries.Length; ++index) {
        items[index] = new KeyValuePair<int, string>(
          (int)entries[index].Key, (string)entries[index].Value
        );
      }
      CollectionAssert.AreEquivalent(numbers, items);
    }

    /// <summary>
    ///   Verifies that the IsSynchronized property and the SyncRoot property are working
    /// </summary>
    [Test]
    public void TestSynchronization() {
      Dictionary<int, string> numbers = createTestDictionary();
      ReadOnlyDictionary<int, string> testDictionary = makeReadOnly(numbers);

      if(!(testDictionary as ICollection).IsSynchronized) {
        lock((testDictionary as ICollection).SyncRoot) {
          Assert.AreEqual(numbers.Count, testDictionary.Count);
        }
      }
    }

    /// <summary>
    ///   Test whether the read only dictionary can be serialized
    /// </summary>
    [Test]
    public void TestSerialization() {
      BinaryFormatter formatter = new BinaryFormatter();

      using(MemoryStream memory = new MemoryStream()) {
        Dictionary<int, string> numbers = createTestDictionary();
        ReadOnlyDictionary<int, string> testDictionary1 = makeReadOnly(numbers);

        formatter.Serialize(memory, testDictionary1);
        memory.Position = 0;
        object testDictionary2 = formatter.Deserialize(memory);

        CollectionAssert.AreEquivalent(testDictionary1, (IEnumerable)testDictionary2);
      }
    }

    /// <summary>
    ///   Creates a new read-only dictionary filled with some values for testing
    /// </summary>
    /// <returns>The newly created read-only dictionary</returns>
    private static Dictionary<int, string> createTestDictionary() {
      Dictionary<int, string> numbers = new Dictionary<int, string>();
      numbers.Add(1, "one");
      numbers.Add(2, "two");
      numbers.Add(3, "three");
      numbers.Add(42, "forty-two");
      return new Dictionary<int, string>(numbers);
    }

    /// <summary>
    ///   Creates a new read-only dictionary filled with some values for testing
    /// </summary>
    /// <returns>The newly created read-only dictionary</returns>
    private static ReadOnlyDictionary<int, string> makeReadOnly(
      IDictionary<int, string> dictionary
    ) {
      return new ReadOnlyDictionary<int, string>(dictionary);
    }

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
