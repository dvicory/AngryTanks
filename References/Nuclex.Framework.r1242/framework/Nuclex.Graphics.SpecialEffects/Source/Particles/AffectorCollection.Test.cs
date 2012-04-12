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

#if UNITTEST

using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Unit tests for the affector collection</summary>
  [TestFixture]
  internal class AffectorCollectionTest {

    #region class CoalescableAffector

    /// <summary>A dummy particle affector that can be coalesced</summary>
    private class CoalescableAffector : IParticleAffector<int> {

      /// <summary>
      ///   Whether the affector can do multiple updates in a single step without
      ///   changing the outcome of the simulation
      /// </summary>
      public bool IsCoalescable {
        get { return true; }
      }

      /// <summary>Applies the affector's effect to a series of particles</summary>
      /// <param name="particles">Particles the affector will be applied to</param>
      /// <param name="start">Index of the first particle that will be affected</param>
      /// <param name="count">Number of particles that will be affected</param>
      /// <param name="updates">Number of updates to perform in the affector</param>
      public void Affect(int[] particles, int start, int count, int updates) { }

    }

    #endregion // class CoalescableAffector

    #region class NoncoalescableAffector

    /// <summary>A dummy particle affector that can not be coalesced</summary>
    private class NoncoalescableAffector : IParticleAffector<int> {

      /// <summary>
      ///   Whether the affector can do multiple updates in a single step without
      ///   changing the outcome of the simulation
      /// </summary>
      public bool IsCoalescable {
        get { return false; }
      }

      /// <summary>Applies the affector's effect to a series of particles</summary>
      /// <param name="particles">Particles the affector will be applied to</param>
      /// <param name="start">Index of the first particle that will be affected</param>
      /// <param name="count">Number of particles that will be affected</param>
      /// <param name="updates">Number of updates to perform in the affector</param>
      public void Affect(int[] particles, int start, int count, int updates) { }

    }

    #endregion // class NoncoalescableAffector

    /// <summary>Unit test for the affector collection's enumerator</summary>
    [TestFixture]
    public class EnumeratorTest {

      /// <summary>
      ///   Tests whether an exception is thrown if the 'Current' property is called
      ///   with the enumerator at an invalid location
      /// </summary>
      [Test]
      public void TestThrowOnCurrentFromInvalidPosition() {
        AffectorCollection<int> affectors = new AffectorCollection<int>();
        using(
          IEnumerator<IParticleAffector<int>> enumerator = affectors.GetEnumerator()
        ) {
          Assert.Throws<InvalidOperationException>(
            delegate() { Assert.AreNotEqual(enumerator.Current, enumerator.Current); }
          );
        }
      }

      /// <summary>
      ///   Verifies that the enumerator behaves correctly if it is advanced
      ///   past its end
      /// </summary>
      [Test]
      public void TestAdvanceEnumeratorPastEnd() {
        AffectorCollection<int> affectors = new AffectorCollection<int>();
        affectors.Add(new CoalescableAffector());
        affectors.Add(new NoncoalescableAffector());
        using(
          IEnumerator<IParticleAffector<int>> enumerator = affectors.GetEnumerator()
        ) {
          Assert.IsTrue(enumerator.MoveNext());
          Assert.IsTrue(enumerator.MoveNext());
          Assert.IsFalse(enumerator.MoveNext());
          Assert.IsFalse(enumerator.MoveNext());
          Assert.IsFalse(enumerator.MoveNext());
        }
      }

      /// <summary>
      ///   Verifies that the affectors can be retrieved through the enumerator
      /// </summary>
      [Test]
      public void TestCurrent() {
        NoncoalescableAffector affector = new NoncoalescableAffector();
        AffectorCollection<int> affectors = new AffectorCollection<int>();
        affectors.Add(affector);

        using(
          IEnumerator<IParticleAffector<int>> enumerator = affectors.GetEnumerator()
        ) {
          Assert.IsTrue(enumerator.MoveNext());
          Assert.AreSame(affector, enumerator.Current);
          Assert.IsFalse(enumerator.MoveNext());
        }
      }

      /// <summary>
      ///   Verifies that the affectors can be retrieved through the enumerator
      ///   through a non-typesafe IEnumerator interface
      /// </summary>
      [Test]
      public void TestCurrentAsObject() {
        NoncoalescableAffector affector = new NoncoalescableAffector();
        AffectorCollection<int> affectors = new AffectorCollection<int>();
        affectors.Add(affector);

        IEnumerator enumerator = (affectors as ICollection).GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreSame(affector, enumerator.Current);
        Assert.IsFalse(enumerator.MoveNext());
      }

      /// <summary>
      ///   Verifies that the affectors can be retrieved through the enumerator
      /// </summary>
      [Test]
      public void TestReset() {
        AffectorCollection<int> affectors = new AffectorCollection<int>();
        affectors.Add(new CoalescableAffector());

        using(
          IEnumerator<IParticleAffector<int>> enumerator = affectors.GetEnumerator()
        ) {
          Assert.IsTrue(enumerator.MoveNext());
          Assert.IsFalse(enumerator.MoveNext());

          enumerator.Reset();

          Assert.IsTrue(enumerator.MoveNext());
          Assert.IsFalse(enumerator.MoveNext());
        }
      }

    }

    /// <summary>
    ///   Verifies that the constructor of the affector collection is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      Assert.IsNotNull(affectors); // nonsense; prevents compiler warning
    }

    /// <summary>
    ///   Verifies that the Add() method splits affectors in coalescable and
    ///   non-coalescable affectors
    /// </summary>
    [Test]
    public void TestAdd() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(new CoalescableAffector());
      affectors.Add(new NoncoalescableAffector());

      Assert.AreEqual(2, affectors.Count);
      Assert.AreEqual(1, affectors.CoalescableAffectors.Count);
      Assert.AreEqual(1, affectors.NoncoalescableAffectors.Count);
    }

    /// <summary>
    ///   Tests whether the Remove() method correctly removes affectors from
    ///   the internal lists of coalescable and non-coalescable affectors.
    /// </summary>
    [Test]
    public void TestRemove() {
      CoalescableAffector coalescable = new CoalescableAffector();
      NoncoalescableAffector noncoalescable = new NoncoalescableAffector();

      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(coalescable);
      affectors.Add(noncoalescable);

      Assert.IsTrue(affectors.Remove(coalescable));
      Assert.IsFalse(affectors.Remove(coalescable));

      Assert.IsTrue(affectors.Remove(noncoalescable));
      Assert.IsFalse(affectors.Remove(noncoalescable));
    }

    /// <summary>Verifies that the affector collection is not read-only</summary>
    [Test]
    public void TestIsReadOnly() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      Assert.IsFalse(affectors.IsReadOnly);
    }

    /// <summary>
    ///   Tests whether the Contains() method of the affector collection is
    ///   working correctly
    /// </summary>
    [Test]
    public void TestContains() {
      CoalescableAffector coalescable = new CoalescableAffector();
      NoncoalescableAffector noncoalescable = new NoncoalescableAffector();

      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(coalescable);
      affectors.Add(noncoalescable);

      Assert.IsTrue(affectors.Contains(coalescable));
      Assert.IsFalse(affectors.Contains(new CoalescableAffector()));

      Assert.IsTrue(affectors.Contains(noncoalescable));
      Assert.IsFalse(affectors.Contains(new NoncoalescableAffector()));
    }

    /// <summary>
    ///   Verifies that the typesafe CopyTo() method is working correctly
    /// </summary>
    [Test]
    public void TestCopyTo() {
      CoalescableAffector coalescable = new CoalescableAffector();
      NoncoalescableAffector noncoalescable = new NoncoalescableAffector();

      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(coalescable);
      affectors.Add(noncoalescable);

      IParticleAffector<int>[] array = new IParticleAffector<int>[3];
      affectors.CopyTo(array, 1);

      Assert.IsNull(array[0]);
      Assert.Contains(coalescable, array);
      Assert.Contains(noncoalescable, array);
    }

    /// <summary>
    ///   Verifies that the typesafe CopyTo() method is working correctly
    /// </summary>
    [Test]
    public void TestThrowOnCopyTo() {
      CoalescableAffector coalescable = new CoalescableAffector();
      NoncoalescableAffector noncoalescable = new NoncoalescableAffector();

      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(coalescable);
      affectors.Add(noncoalescable);

      IParticleAffector<int>[] array = new IParticleAffector<int>[3];
      Assert.Throws<IndexOutOfRangeException>(
        delegate() { affectors.CopyTo(array, 2); }
      );
    }

    /// <summary>
    ///   Verifies that the non-typesafe CopyTo() method is working correctly
    /// </summary>
    [Test]
    public void TestCopyToAsObject() {
      CoalescableAffector coalescable = new CoalescableAffector();
      NoncoalescableAffector noncoalescable = new NoncoalescableAffector();

      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(coalescable);
      affectors.Add(noncoalescable);

      object[] array = new object[3];
      (affectors as ICollection).CopyTo(array, 1);

      Assert.IsNull(array[0]);
      Assert.Contains(coalescable, array);
      Assert.Contains(noncoalescable, array);
    }

    /// <summary>
    ///   Verifies that the typesafe CopyTo() method is working correctly
    /// </summary>
    [Test]
    public void TestThrowOnCopyToAsObject() {
      CoalescableAffector coalescable = new CoalescableAffector();
      NoncoalescableAffector noncoalescable = new NoncoalescableAffector();

      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(coalescable);
      affectors.Add(noncoalescable);

      object[] array = new object[3];
      Assert.Throws<IndexOutOfRangeException>(
        delegate() { (affectors as ICollection).CopyTo(array, 2); }
      );
    }

    /// <summary>whether the collection can clear its contents</summary>
    [Test]
    public void TestClear() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      affectors.Add(new CoalescableAffector());
      affectors.Add(new NoncoalescableAffector());

      affectors.Clear();
      Assert.AreEqual(0, affectors.Count);
      Assert.AreEqual(0, affectors.CoalescableAffectors.Count);
      Assert.AreEqual(0, affectors.NoncoalescableAffectors.Count);
    }

    /// <summary>
    ///   Tests whether the SyncRoot property returns a lockable object
    /// </summary>
    [Test]
    public void TestSyncRoot() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      lock((affectors as ICollection).SyncRoot) { }
    }

    /// <summary>
    ///   Tests whether the IsSynchronized property returns the right result
    /// </summary>
    [Test]
    public void TestIsSynchronized() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      Assert.IsFalse((affectors as ICollection).IsSynchronized);
    }

    /// <summary>
    ///   Tests whether a typesafe enumerator can be obtained for the collection
    /// </summary>
    [Test]
    public void TestGetEnumerator() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      using(IEnumerator<IParticleAffector<int>> enumerator = affectors.GetEnumerator()) {
        Assert.IsNotNull(enumerator);
        Assert.IsFalse(enumerator.MoveNext());
      }
    }

    /// <summary>
    ///   Tests whether a non-typesafe enumerator can be obtained for the collection
    /// </summary>
    [Test]
    public void TestGetEnumeratorForObject() {
      AffectorCollection<int> affectors = new AffectorCollection<int>();
      IEnumerator enumerator = (affectors as ICollection).GetEnumerator();
      Assert.IsNotNull(enumerator);
      Assert.IsFalse(enumerator.MoveNext());
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles

#endif // UNITTEST
