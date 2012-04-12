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
using NMock2;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the observable dictionary wrapper</summary>
  [TestFixture]
  public class ObservableDictionaryTest {

    #region interface IObservableDictionarySubscriber

    /// <summary>Interface used to test the observable dictionary</summary>
    public interface IObservableDictionarySubscriber {

      /// <summary>Called when the dictionary is about to clear its contents</summary>
      /// <param name="sender">Dictionary that is clearing its contents</param>
      /// <param name="arguments">Not used</param>
      void Clearing(object sender, EventArgs arguments);

      /// <summary>Called when the dictionary has been clear of its contents</summary>
      /// <param name="sender">Dictionary that was cleared of its contents</param>
      /// <param name="arguments">Not used</param>
      void Cleared(object sender, EventArgs arguments);

      /// <summary>Called when an item is added to the dictionary</summary>
      /// <param name="sender">Dictionary to which an item is being added</param>
      /// <param name="arguments">Contains the item that is being added</param>
      void ItemAdded(object sender, ItemEventArgs<KeyValuePair<int, string>> arguments);

      /// <summary>Called when an item is removed from the dictionary</summary>
      /// <param name="sender">Dictionary from which an item is being removed</param>
      /// <param name="arguments">Contains the item that is being removed</param>
      void ItemRemoved(object sender, ItemEventArgs<KeyValuePair<int, string>> arguments);

    }

    #endregion // interface IObservableDictionarySubscriber

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new Mockery();

      this.mockedSubscriber = this.mockery.NewMock<IObservableDictionarySubscriber>();

      this.observedDictionary = new ObservableDictionary<int, string>();
      this.observedDictionary.Add(1, "one");
      this.observedDictionary.Add(2, "two");
      this.observedDictionary.Add(3, "three");
      this.observedDictionary.Add(42, "forty-two");

      this.observedDictionary.Clearing +=
        new EventHandler(this.mockedSubscriber.Clearing);
      this.observedDictionary.Cleared +=
        new EventHandler(this.mockedSubscriber.Cleared);
      this.observedDictionary.ItemAdded +=
        new EventHandler<ItemEventArgs<KeyValuePair<int, string>>>(
          this.mockedSubscriber.ItemAdded
        );
      this.observedDictionary.ItemRemoved +=
        new EventHandler<ItemEventArgs<KeyValuePair<int, string>>>(
          this.mockedSubscriber.ItemRemoved
        );
    }

    /// <summary>
    ///   Verifies that the default constructor of the observable dictionary works
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      ObservableDictionary<int, string> testDictionary =
        new ObservableDictionary<int, string>();

      Assert.AreEqual(0, testDictionary.Count);
    }

    /// <summary>
    ///   Verifies that the copy constructor of the observable dictionary works
    /// </summary>
    [Test]
    public void TestCopyConstructor() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      CollectionAssert.AreEqual(numbers, testDictionary);
    }

    /// <summary>Verifies that the IsReadOnly property is working</summary>
    [Test]
    public void TestIsReadOnly() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      Assert.IsFalse(testDictionary.IsReadOnly);
    }

    /// <summary>
    ///   Checks whether the Contains() method of the observable dictionary is able to
    ///   determine if the dictionary contains an item
    /// </summary>
    [Test]
    public void TestContains() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      Assert.IsTrue(
        testDictionary.Contains(new KeyValuePair<int, string>(42, "forty-two"))
      );
      Assert.IsFalse(
        testDictionary.Contains(new KeyValuePair<int, string>(24, "twenty-four"))
      );
    }

    /// <summary>
    ///   Checks whether the Contains() method of the observable dictionary is able to
    ///   determine if the dictionary contains a key
    /// </summary>
    [Test]
    public void TestContainsKey() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      Assert.IsTrue(testDictionary.ContainsKey(42));
      Assert.IsFalse(testDictionary.ContainsKey(24));
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the observable dictionary works
    /// </summary>
    [Test]
    public void TestCopyToArray() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      KeyValuePair<int, string>[] items = new KeyValuePair<int, string>[numbers.Count];

      testDictionary.CopyTo(items, 0);

      CollectionAssert.AreEqual(numbers, items);
    }

    /// <summary>
    ///   Tests whether the typesafe enumerator of the observable dictionary is working
    /// </summary>
    [Test]
    public void TestTypesafeEnumerator() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      List<KeyValuePair<int, string>> outputItems = new List<KeyValuePair<int, string>>();
      foreach(KeyValuePair<int, string> item in testDictionary) {
        outputItems.Add(item);
      }

      CollectionAssert.AreEqual(numbers, outputItems);
    }

    /// <summary>
    ///   Tests whether the keys collection of the observable dictionary can be queried
    /// </summary>
    [Test]
    public void TestGetKeysCollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      ICollection<int> inputKeys = numbers.Keys;
      ICollection<int> keys = testDictionary.Keys;
      CollectionAssert.AreEquivalent(inputKeys, keys);
    }

    /// <summary>
    ///   Tests whether the values collection of the observable dictionary can be queried
    /// </summary>
    [Test]
    public void TestGetValuesCollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      ICollection<string> inputValues = numbers.Values;
      ICollection<string> values = testDictionary.Values;
      CollectionAssert.AreEquivalent(inputValues, values);
    }

    /// <summary>
    ///   Tests whether the TryGetValue() method of the observable dictionary is working
    /// </summary>
    [Test]
    public void TestTryGetValue() {
      string value;

      Assert.IsTrue(this.observedDictionary.TryGetValue(42, out value));
      Assert.AreEqual("forty-two", value);

      Assert.IsFalse(this.observedDictionary.TryGetValue(24, out value));
      Assert.AreEqual(null, value);
    }

    /// <summary>
    ///   Tests whether the retrieval of values using the indexer of the observable
    ///   dictionary is working
    /// </summary>
    [Test]
    public void TestRetrieveValueByIndexer() {
      Assert.AreEqual("forty-two", this.observedDictionary[42]);
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the indexer of the observable dictionary
    ///   is used to attempt to retrieve a non-existing value
    /// </summary>
    [Test]
    public void TestRetrieveNonExistingValueByIndexer() {
      Assert.Throws<KeyNotFoundException>(
        delegate() { Console.WriteLine(this.observedDictionary[24]); }
      );
    }

    /// <summary>
    ///   Checks whether the Add() methods works via the generic
    ///   IDictionary&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestAddViaGenericIDictionary() {
      Expect.Once.On(this.mockedSubscriber).Method("ItemAdded").WithAnyArguments();
      (this.observedDictionary as IDictionary<int, string>).Add(10, "ten");
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      CollectionAssert.Contains(
        this.observedDictionary, new KeyValuePair<int, string>(10, "ten")
      );
    }

    /// <summary>
    ///   Checks whether the Remove() method works via the generic
    ///   IDictionary&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestRemoveViaGenericIDictionary() {
      Expect.Once.On(this.mockedSubscriber).Method("ItemRemoved").WithAnyArguments();
      (this.observedDictionary as IDictionary<int, string>).Remove(3);
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      CollectionAssert.DoesNotContain(this.observedDictionary.Keys, 3);
    }

    /// <summary>
    ///   Tests whether the TryGetValue() method of the observable dictionary is working
    /// </summary>
    [Test]
    public void TestRetrieveValueByIndexerViaGenericIDictionary() {
      Assert.AreEqual(
        "forty-two", (this.observedDictionary as IDictionary<int, string>)[42]
      );
    }

    /// <summary>
    ///   Verifies that the indexer can be used to insert an item via the generic
    ///   IDictionar&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestReplaceByIndexerViaGenericIDictionary() {
      Expect.Once.On(this.mockedSubscriber).Method("ItemRemoved").WithAnyArguments();
      Expect.Once.On(this.mockedSubscriber).Method("ItemAdded").WithAnyArguments();
      (this.observedDictionary as IDictionary<int, string>)[42] = "two and fourty";
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.AreEqual("two and fourty", this.observedDictionary[42]);
    }

    /// <summary>
    ///   Checks whether the Clear() method of observable dictionary is working
    /// </summary>
    [Test]
    public void TestClearViaIDictionary() {
      Expect.Once.On(this.mockedSubscriber).Method("Clearing").WithAnyArguments();
      Expect.Once.On(this.mockedSubscriber).Method("Cleared").WithAnyArguments();
      (this.observedDictionary as IDictionary).Clear();
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.AreEqual(0, this.observedDictionary.Count);
    }

    /// <summary>
    ///   Checks whether the Add() method works via the IDictionary interface
    /// </summary>
    [Test]
    public void TestAddViaIDictionary() {
      Expect.Once.On(this.mockedSubscriber).Method("ItemAdded").WithAnyArguments();
      (this.observedDictionary as IDictionary).Add(24, "twenty-four");
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      CollectionAssert.Contains(
        this.observedDictionary, new KeyValuePair<int, string>(24, "twenty-four")
      );
    }

    /// <summary>
    ///   Checks whether the Contains() method of the observable dictionary is able to
    ///   determine if the dictionary contains an item via the IDictionary interface
    /// </summary>
    [Test]
    public void TestContainsViaIDictionary() {
      Assert.IsTrue((this.observedDictionary as IDictionary).Contains(42));
      Assert.IsFalse((this.observedDictionary as IDictionary).Contains(24));
    }

    /// <summary>
    ///   Checks whether the GetEnumerator() method of the observable dictionary
    ///   returns a working enumerator if accessed via the IDictionary interface
    /// </summary>
    [Test]
    public void TestEnumeratorViaIDictionary() {
      Dictionary<int, string> outputNumbers = new Dictionary<int, string>();
      foreach(DictionaryEntry entry in (this.observedDictionary as IDictionary)) {
        (outputNumbers as IDictionary).Add(entry.Key, entry.Value);
      }

      CollectionAssert.AreEquivalent(this.observedDictionary, outputNumbers);
    }

    /// <summary>
    ///   Checks whether the IsFixedSize property of the observable dictionary returns
    ///   the expected result for a read only dictionary based on a dynamic dictionary
    /// </summary>
    [Test]
    public void TestIsFixedSizeViaIList() {
      Assert.IsFalse((this.observedDictionary as IDictionary).IsFixedSize);
    }

    /// <summary>
    ///   Tests whether the keys collection of the observable dictionary can be queried
    ///   via the IDictionary interface
    /// </summary>
    [Test]
    public void TestGetKeysCollectionViaIDictionary() {
      ICollection keys = (this.observedDictionary as IDictionary).Keys;
      Assert.AreEqual(this.observedDictionary.Count, keys.Count);
    }

    /// <summary>
    ///   Tests whether the values collection of the observable dictionary can be queried
    ///   via the IDictionary interface
    /// </summary>
    [Test]
    public void TestGetValuesCollectionViaIDictionary() {
      ICollection values = (this.observedDictionary as IDictionary).Values;
      Assert.AreEqual(this.observedDictionary.Count, values.Count);
    }

    /// <summary>
    ///   Checks whether Remove() method works via the IDictionary interface
    /// </summary>
    [Test]
    public void TestRemoveViaIDictionary() {
      Expect.Once.On(this.mockedSubscriber).Method("ItemRemoved").WithAnyArguments();
      (this.observedDictionary as IDictionary).Remove(3);
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      CollectionAssert.DoesNotContain(this.observedDictionary.Keys, 3);
    }

    /// <summary>
    ///   Tests whether the retrieval of values using the indexer of the observable
    ///   dictionary is working via the IDictionary interface
    /// </summary>
    [Test]
    public void TestRetrieveValueByIndexerViaIDictionary() {
      Assert.AreEqual("forty-two", (this.observedDictionary as IDictionary)[42]);
    }

    /// <summary>
    ///   Verifies the indexer can be used to insert an item via the IDictionary interface
    /// </summary>
    [Test]
    public void TestReplaceByIndexerViaIDictionary() {
      Expect.Once.On(this.mockedSubscriber).Method("ItemRemoved").WithAnyArguments();
      Expect.Once.On(this.mockedSubscriber).Method("ItemAdded").WithAnyArguments();
      (this.observedDictionary as IDictionary)[42] = "two and fourty";
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.AreEqual("two and fourty", this.observedDictionary[42]);
    }

    /// <summary>
    ///   Checks whether Add() method is working via the generic
    ///   ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestAddViaGenericICollection() {
      Expect.Once.On(this.mockedSubscriber).Method("ItemAdded").WithAnyArguments();
      (this.observedDictionary as ICollection<KeyValuePair<int, string>>).Add(
        new KeyValuePair<int, string>(24, "twenty-four")
      );
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      CollectionAssert.Contains(
        this.observedDictionary, new KeyValuePair<int, string>(24, "twenty-four")
      );
    }

    /// <summary>
    ///   Checks whether the Clear() method is working via the generic
    ///   ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestClearViaGenericICollection() {
      Expect.Once.On(this.mockedSubscriber).Method("Clearing").WithAnyArguments();
      Expect.Once.On(this.mockedSubscriber).Method("Cleared").WithAnyArguments();
      (this.observedDictionary as ICollection<KeyValuePair<int, string>>).Clear();
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      Assert.AreEqual(0, this.observedDictionary.Count);
    }

    /// <summary>
    ///   Checks whether the Remove() method is working via the
    ///   generic ICollection&lt;&gt; interface
    /// </summary>
    [Test]
    public void TestRemoveViaGenericICollection() {
      IEnumerator<KeyValuePair<int, string>> enumerator =
        (this.observedDictionary as ICollection<KeyValuePair<int, string>>).GetEnumerator();
      enumerator.MoveNext();
      Expect.Once.On(this.mockedSubscriber).Method("ItemRemoved").WithAnyArguments();
      (this.observedDictionary as ICollection<KeyValuePair<int, string>>).Remove(
        enumerator.Current
      );
      this.mockery.VerifyAllExpectationsHaveBeenMet();

      CollectionAssert.DoesNotContain(this.observedDictionary, enumerator.Current);
    }

    /// <summary>
    ///   Verifies that the CopyTo() of the observable dictionary works when called
    ///   via the the ICollection interface
    /// </summary>
    [Test]
    public void TestCopyToArrayViaICollection() {
      Dictionary<int, string> numbers = createTestDictionary();
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

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
      ObservableDictionary<int, string> testDictionary = makeObservable(numbers);

      if(!(testDictionary as ICollection).IsSynchronized) {
        lock((testDictionary as ICollection).SyncRoot) {
          Assert.AreEqual(numbers.Count, testDictionary.Count);
        }
      }
    }

    /// <summary>
    ///   Test whether the observable dictionary can be serialized
    /// </summary>
    [Test]
    public void TestSerialization() {
      BinaryFormatter formatter = new BinaryFormatter();

      using(MemoryStream memory = new MemoryStream()) {
        Dictionary<int, string> numbers = createTestDictionary();
        ObservableDictionary<int, string> testDictionary1 = makeObservable(numbers);

        formatter.Serialize(memory, testDictionary1);
        memory.Position = 0;
        object testDictionary2 = formatter.Deserialize(memory);

        CollectionAssert.AreEquivalent(testDictionary1, (IEnumerable)testDictionary2);
      }
    }

    /// <summary>
    ///   Creates a new observable dictionary filled with some values for testing
    /// </summary>
    /// <returns>The newly created observable dictionary</returns>
    private static Dictionary<int, string> createTestDictionary() {
      Dictionary<int, string> numbers = new Dictionary<int, string>();
      numbers.Add(1, "one");
      numbers.Add(2, "two");
      numbers.Add(3, "three");
      numbers.Add(42, "forty-two");
      return new Dictionary<int, string>(numbers);
    }

    /// <summary>
    ///   Creates a new observable dictionary filled with some values for testing
    /// </summary>
    /// <returns>The newly created observable dictionary</returns>
    private static ObservableDictionary<int, string> makeObservable(
      IDictionary<int, string> dictionary
    ) {
      return new ObservableDictionary<int, string>(dictionary);
    }

    /// <summary>Mock object factory</summary>
    private Mockery mockery;
    /// <summary>The mocked observable collection subscriber</summary>
    private IObservableDictionarySubscriber mockedSubscriber;
    /// <summary>An observable dictionary to which a mock will be subscribed</summary>
    private ObservableDictionary<int, string> observedDictionary;

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
