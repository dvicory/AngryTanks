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
using System.IO;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Plugins {

  /// <summary>Unit Test for the instance employer class</summary>
  [TestFixture]
  public class InstanceEmployerTest {

    #region class Base

    /// <summary>
    ///   Abstract base class to serve as abstract product for testing the instance employer
    /// </summary>
    private abstract class Base { }

    #endregion // class Base

    #region class Derived

    /// <summary>
    ///   Class derived from the abstract base to serve as concrete product for
    ///   testing the instance employer
    /// </summary>
    private class Derived : Base { }

    #endregion // class Derived

    #region class GenericDerived

    /// <summary>
    ///   Generic class derived from the abstract base to serve as concrete product
    ///   for testing the instance employer
    /// </summary>
    private class GenericDerived<SomeType> : Base { }

    #endregion // class GenericDerived

    #region class Unrelated

    /// <summary>Unrelated class used to test the instance employer</summary>
    private class Unrelated { }

    #endregion // class Unrelated

    /// <summary>
    ///   Tests whether the instance employer can detect employable types
    /// </summary>
    [Test]
    public void TestCanEmploy() {
      InstanceEmployer<Base> testEmployer = new InstanceEmployer<Base>();

      Assert.IsFalse(testEmployer.CanEmploy(typeof(Base)));
      Assert.IsTrue(testEmployer.CanEmploy(typeof(Derived)));
      Assert.IsFalse(testEmployer.CanEmploy(typeof(Unrelated)));
    }


    /// <summary>
    ///   Tests whether the instance employer throws an exception when it is asked to
    ///   employ an abstract class
    /// </summary>
    [Test]
    public void TestThrowOnEmployAbstractClass() {
      InstanceEmployer<Base> testEmployer = new InstanceEmployer<Base>();

      Assert.Throws<MissingMethodException>(
        delegate() { testEmployer.Employ(typeof(Base)); }
      );
    }

    /// <summary>
    ///   Tests whether the instance employer throws an exception when it is asked to
    ///   employ a class that is not the product type or a derivative thereof
    /// </summary>
    [Test]
    public void TestThrowOnEmployUnrelatedClass() {
      InstanceEmployer<Base> testEmployer = new InstanceEmployer<Base>();

      Assert.Throws<InvalidCastException>(
        delegate() { testEmployer.Employ(typeof(Unrelated)); }
      );
    }

    /// <summary>
    ///   Tests whether the instance employer throws an exception when it is asked to
    ///   employ a class that requires generic parameters for instantiation
    /// </summary>
    [Test]
    public void TestThrowOnEmployGenericClass() {
      InstanceEmployer<Base> testEmployer = new InstanceEmployer<Base>();

      Assert.Throws<ArgumentException>(
        delegate() { testEmployer.Employ(typeof(GenericDerived<>)); }
      );
    }

    /// <summary>
    ///   Tests whether the instance employer can employ a class derived from the product
    /// </summary>
    [Test]
    public void TestEmployClassDerivedFromProduct() {
      InstanceEmployer<Base> testEmployer = new InstanceEmployer<Base>();

      testEmployer.Employ(typeof(Derived));

      Assert.AreEqual(1, testEmployer.Instances.Count);
      Assert.AreEqual(typeof(Derived), testEmployer.Instances[0].GetType());
      Assert.IsInstanceOf<Derived>(testEmployer.Instances[0]);
    }

    /// <summary>
    ///   Tests whether the instance employer can employ the product class itself if it
    ///   isn't abstract
    /// </summary>
    [Test]
    public void TestEmployProduct() {
      InstanceEmployer<Unrelated> testEmployer = new InstanceEmployer<Unrelated>();

      testEmployer.Employ(typeof(Unrelated));

      Assert.AreEqual(1, testEmployer.Instances.Count);
      Assert.AreEqual(typeof(Unrelated), testEmployer.Instances[0].GetType());
      Assert.IsInstanceOf<Unrelated>(testEmployer.Instances[0]);
    }

  }

} // namespace Nuclex.Support.Plugins

#endif // UNITTEST
