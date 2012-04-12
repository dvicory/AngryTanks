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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the strongly typed weak reference class</summary>
  [TestFixture]
  public class WeakReferenceTest {

    #region class Dummy

    /// <summary>Dummy class for testing the shared instance provider</summary>
    [Serializable]
    private class Dummy {
      /// <summary>Initializes a new dummy</summary>
      public Dummy() { }
    }

    #endregion // class Dummy

    /// <summary>Tests whether the simple constructor works</summary>
    [Test]
    public void TestSimpleConstructor() {
      new WeakReference<Dummy>(new Dummy());
    }

    /// <summary>Test whether the full constructor works</summary>
    [Test]
    public void TestFullConstructor() {
      new WeakReference<Dummy>(new Dummy(), false);
    }

    /// <summary>
    ///   Test whether the target object can be retrieved from the weak reference
    /// </summary>
    [Test]
    public void TestTargetRetrieval() {
      Dummy strongReference = new Dummy();
      WeakReference<Dummy> weakReference = new WeakReference<Dummy>(strongReference);

      // We can not just call GC.Collect() and base our test on the assumption that
      // the garbage collector will actually collect the Dummy instance. This is up
      // to the garbage collector to decide. But we can keep a strong reference in
      // parallel and safely assume that the WeakReference will not be invalidated!
      Assert.AreSame(strongReference, weakReference.Target);
    }

    /// <summary>
    ///   Test whether the target object can be reassigned in the weak reference
    /// </summary>
    [Test]
    public void TestTargetReassignment() {
      Dummy strongReference1 = new Dummy();
      Dummy strongReference2 = new Dummy();
      WeakReference<Dummy> weakReference = new WeakReference<Dummy>(strongReference1);

      Assert.AreSame(strongReference1, weakReference.Target);
      weakReference.Target = strongReference2;
      Assert.AreSame(strongReference2, weakReference.Target);
    }

    /// <summary>
    ///   Test whether the target object can be reassigned in the weak reference
    /// </summary>
    [Test]
    public void TestSerialization() {
      BinaryFormatter formatter = new BinaryFormatter();

      using(MemoryStream memory = new MemoryStream()) {
        WeakReference<Dummy> weakReference1 = new WeakReference<Dummy>(new Dummy());

        formatter.Serialize(memory, weakReference1);
        memory.Position = 0;
        object weakReference2 = formatter.Deserialize(memory);

        // We cannot make any more predictions but for the type of the weak reference.
        // The pointee might have been garbage collected just now or the serializer
        // might have decided not to serialize the pointee at all (which is a valid
        // decision if the serializer found no strong reference to the pointee) in
        // another of the object graph.
        Assert.IsNotNull(weakReference2 as WeakReference<Dummy>);
      }
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
