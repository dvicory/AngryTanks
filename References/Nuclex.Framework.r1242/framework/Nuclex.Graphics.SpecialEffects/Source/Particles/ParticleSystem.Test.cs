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
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Unit tests for the particle system</summary>
  [TestFixture]
  internal class ParticleSystemTest {

    #region class ThrowingAffector

    /// <summary>Dummy particle affector that throws an exception</summary>
    private class ThrowingAffector : IParticleAffector<SimpleParticle> {

      /// <summary>
      ///   Whether the affector can do multiple updates in a single step without
      ///   changing the outcome of the simulation
      /// </summary>
      public bool IsCoalescable { get { return false; } }

      /// <summary>Applies the affector's effect to a series of particles</summary>
      /// <param name="particles">Particles the affector will be applied to</param>
      /// <param name="start">Index of the first particle that will be affected</param>
      /// <param name="count">Number of particles that will be affected</param>
      /// <param name="updates">Number of updates to perform in the affector</param>
      public void Affect(SimpleParticle[] particles, int start, int count, int updates) {
        throw new KeyNotFoundException("Simulated error");
      }

    }

    #endregion // class ThrowingAffector

    #region class TestAffector

    /// <summary>Dummy particle affector for unit testing</summary>
    private class TestAffector : IParticleAffector<SimpleParticle> {

      /// <summary>Initializes a new test affector</summary>
      /// <param name="coalescable">Whether the test affector is coalescable</param>
      public TestAffector(bool coalescable) {
        this.coalescable = coalescable;
      }

      /// <summary>
      ///   Whether the affector can do multiple updates in a single step without
      ///   changing the outcome of the simulation
      /// </summary>
      public bool IsCoalescable { get { return this.coalescable; } }

      /// <summary>Applies the affector's effect to a series of particles</summary>
      /// <param name="particles">Particles the affector will be applied to</param>
      /// <param name="start">Index of the first particle that will be affected</param>
      /// <param name="count">Number of particles that will be affected</param>
      /// <param name="updates">Number of updates to perform in the affector</param>
      public void Affect(SimpleParticle[] particles, int start, int count, int updates) {
        this.LastUpdateCount = updates;
      }

      /// <summary>Number of updates the last Affect() call requested</summary>
      public int LastUpdateCount;
      /// <summary>Whether this affector is coalescable</summary>
      private bool coalescable;

    }

    #endregion // class TestAffector

    #region class WaitAffector

    /// <summary>Dummy particle affector for unit testing that holds execution</summary>
    private class WaitAffector : IParticleAffector<SimpleParticle>, IDisposable {

      /// <summary>Initializes a new wait affector</summary>
      public WaitAffector() {
        this.waitEvent = new ManualResetEvent(false);
      }

      /// <summary>Immediately releases all resources used by the instance</summary>
      public void Dispose() {
        if(this.waitEvent != null) {
          this.waitEvent.Close();
          this.waitEvent = null;
        }
      }

      /// <summary>
      ///   Whether the affector can do multiple updates in a single step without
      ///   changing the outcome of the simulation
      /// </summary>
      public bool IsCoalescable { get { return false; } }

      /// <summary>Applies the affector's effect to a series of particles</summary>
      /// <param name="particles">Particles the affector will be applied to</param>
      /// <param name="start">Index of the first particle that will be affected</param>
      /// <param name="count">Number of particles that will be affected</param>
      /// <param name="updates">Number of updates to perform in the affector</param>
      public void Affect(SimpleParticle[] particles, int start, int count, int updates) {
        this.waitEvent.WaitOne();
      }

      /// <summary>Lets the wait affector stop execution</summary>
      public void Halt() { this.waitEvent.Reset(); }

      /// <summary>Lets the wait affector continue execution</summary>
      public void Continue() { this.waitEvent.Set(); }

      /// <summary>Event the wait affector will wait for</summary>
      private ManualResetEvent waitEvent;

    }

    #endregion // class TestAffector

    #region class DummyAsyncResult

    /// <summary>Dummy asynchronous result</summary>
    private class DummyAsyncResult : IAsyncResult {

      /// <summary>User defined state from the Begin() method</summary>
      public object AsyncState {
        get { throw new NotImplementedException(); }
      }

      /// <summary>Wait handle that can be used to wait for the process</summary>
      public WaitHandle AsyncWaitHandle {
        get { throw new NotImplementedException(); }
      }

      /// <summary>Whether the process has finished synchronously</summary>
      public bool CompletedSynchronously {
        get { throw new NotImplementedException(); }
      }

      /// <summary>True if the process has finished</summary>
      public bool IsCompleted {
        get { throw new NotImplementedException(); }
      }

    }

    #endregion // class DummyAsyncResult

    #region class DummyCallbackReceiver

    /// <summary>Dummy receiver that count the number of callbacks</summary>
    private class DummyCallbackReceiver {

      /// <summary>Initializes a new dummy callback receiver</summary>
      public DummyCallbackReceiver() {
        this.callbackEvent = new AutoResetEvent(false);
      }

      /// <summary>Callback which counts its number of invocations</summary>
      /// <param name="state">User defined state from the Begin() method</param>
      public void Callback(object state) {
        Interlocked.Increment(ref this.callbackCallCount);
        this.callbackEvent.Set();
      }

      /// <summary>Number of calls to the Callback() method</summary>
      public int CallbackCallCount {
        get { return Thread.VolatileRead(ref this.callbackCallCount); }
      }

      /// <summary>Event that will be triggered when the callback executes</summary>
      public AutoResetEvent CallbackEvent { get { return this.callbackEvent; } }

      /// <summary>Number of calls to the Callback() method</summary>
      private int callbackCallCount;
      /// <summary>Event that will be triggered when the callback executes</summary>
      private AutoResetEvent callbackEvent;

    }

    #endregion // class DummyCallbackReceiver

    /// <summary>Verifies that the particle system's constructor is working</summary>
    [Test]
    public void TestConstructor() {
      using(ParticleSystem<int> system = new ParticleSystem<int>(100)) {
        Assert.AreEqual(100, system.Capacity);
      }
    }

    /// <summary>Verifies that the AddParticles() method is working</summary>
    [Test]
    public void TestAddParticles() {
      using(ParticleSystem<int> system = new ParticleSystem<int>(10)) {
        for(int index = 0; index < 12; ++index) {
          system.AddParticle(index);
        }

        Assert.AreEqual(10, system.Particles.Count);

        for(int index = 0; index < 10; ++index) {
          Assert.Contains(index, system.Particles.Array);
        }
      }
    }

    /// <summary>
    ///   Validates that the Update() method can run a simulation with multiple affectors
    /// </summary>
    [Test]
    public void TestUpdateWithMultipleAffectors() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        for(int index = 0; index < 10; ++index) {
          system.AddParticle(new SimpleParticle());
        }

        system.Affectors.Add(
          new GravityAffector<SimpleParticle>(SimpleParticleModifier.Default)
        );
        system.Affectors.Add(
          new MovementAffector<SimpleParticle>(SimpleParticleModifier.Default)
        );

        system.Update(2);

        float newY = -GravityAffector<SimpleParticle>.StandardEarthGravity * 3.0f / 60.0f;
        for(int index = 0; index < 10; ++index) {
          Assert.That(
            system.Particles.Array[0].Position.Y, Is.EqualTo(newY).Within(4).Ulps
          );
        }
      }
    }

    /// <summary>
    ///   Verifies that the particle system's Add() and Remove() methods are working
    /// </summary>
    [Test]
    public void TestAddAndRemove() {
      using(ParticleSystem<int> system = new ParticleSystem<int>(10)) {
        for(int index = 0; index < 10; ++index) {
          system.AddParticle(index);
        }

        system.RemoveParticle(8);
        system.RemoveParticle(6);
        system.RemoveParticle(4);
        system.RemoveParticle(2);

        CollectionAssert.AreEquivalent(
          new int[] { 0, 1, 3, 5, 7, 9 }, toArray(system.Particles)
        );
      }
    }

    /// <summary>
    ///   Verifies that the particle system can prune dead particles
    /// </summary>
    [Test]
    public void TestPrune() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        for(int index = 0; index < 10; ++index) {
          SimpleParticle particle = new SimpleParticle();
          particle.Position.Y = (float)index;
          system.AddParticle(particle);
        }

        system.Prune(
          delegate(ref SimpleParticle particle) {
            return (particle.Position.Y < 5.0f);
          }
        );

        Assert.AreEqual(5, system.Particles.Count);
        for(int index = 0; index < system.Particles.Count; ++index) {
          Assert.Less(system.Particles.Array[index].Position.Y, 5.0f);
        }
      }
    }

    /// <summary>
    ///   Verifies that the particle system coalesces even a non-coalescable update if
    ///   there is only one non-coalescable update.
    /// </summary>
    [Test]
    public void TestCoalesceSingleNoncoalescableUpdate() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        for(int index = 0; index < 10; ++index) {
          system.AddParticle(new SimpleParticle());
        }

        TestAffector noncoalescable = new TestAffector(false);
        TestAffector coalescable = new TestAffector(true);
        system.Affectors.Add(noncoalescable);
        system.Affectors.Add(coalescable);

        system.Update(2);

        Assert.AreEqual(2, coalescable.LastUpdateCount);
        Assert.AreEqual(2, noncoalescable.LastUpdateCount);
      }
    }

    /// <summary>
    ///   Verifies that the particle system can prune dead particles asynchronously
    /// </summary>
    [Test]
    public void TestAsynchronousPrune() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        for(int index = 0; index < 15; ++index) {
          SimpleParticle particle = new SimpleParticle();
          particle.Position.Y = (float)index;
          system.AddParticle(particle);
        }

        for(int repetition = 0; repetition < 3; ++repetition) {
          IAsyncResult asyncResult = system.BeginPrune(
            delegate(ref SimpleParticle particle) {
              Thread.Sleep(0); // yield time slice
              return (particle.Position.Y < 5.0f);
            },
            null, null
          );
          system.EndPrune(asyncResult);
        }

        Assert.AreEqual(5, system.Particles.Count);
        for(int index = 0; index < system.Particles.Count; ++index) {
          Assert.Less(system.Particles.Array[index].Position.Y, 5.0f);
        }
      }
    }

    /// <summary>
    ///   Tests whether an exception happening during the asynchronous prune process is
    ///   caught and delivered to the caller of the EndPrune() method
    /// </summary>
    [Test]
    public void TestThrowOnExceptionDuringAsynchronousPrune() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        system.AddParticle(new SimpleParticle());

        IAsyncResult asyncResult = system.BeginPrune(
          delegate(ref SimpleParticle particle) { throw new KeyNotFoundException(); },
          null, null
        );
        Assert.Throws<KeyNotFoundException>(
          delegate() { system.EndPrune(asyncResult); }
        );
      }
    }

    /// <summary>
    ///   Tests whether an exception is thrown when EndPrune() is called with
    ///   an async result that was not returned by BeginPrune()
    /// </summary>
    [Test]
    public void TestThrowOnEndPruneWithWrongAsyncResult() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        IAsyncResult asyncResult = system.BeginPrune(
          delegate(ref SimpleParticle particle) { return false; }, null, null
        );
        try {
          Assert.Throws<ArgumentException>(
            delegate() { system.EndPrune(new DummyAsyncResult()); }
          );
        }
        finally {
          system.EndPrune(asyncResult);
        }
      }
    }

    /// <summary>
    ///   Verifies that the particle system can handle multiple asynchronous prunes
    /// </summary>
    [Test]
    public void TestMultipleAsynchronousPrunes() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        DummyCallbackReceiver receiver = new DummyCallbackReceiver();
        for(int run = 0; run < 3; ++run) {
          IAsyncResult asyncResult = system.BeginPrune(
            delegate(ref SimpleParticle particle) { return false; },
            receiver.Callback, receiver
          );
          try {
            Assert.IsNotNull(asyncResult.AsyncWaitHandle);
            Assert.AreSame(asyncResult.AsyncState, receiver);
            Assert.IsFalse(asyncResult.CompletedSynchronously);
          }
          finally {
            system.EndPrune(asyncResult);
          }

          Assert.IsTrue(receiver.CallbackEvent.WaitOne(1000));
          Assert.AreEqual(run + 1, receiver.CallbackCallCount);
        }
      }
    }

    /// <summary>
    ///   Verifies that the particle system can update its particle asynchronously
    /// </summary>
    [Test]
    public void TestAsynchronousUpdate() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(40)
      ) {
        system.Affectors.Add(
          new MovementAffector<SimpleParticle>(SimpleParticleModifier.Default)
        );

        for(int index = 0; index < 40; ++index) {
          SimpleParticle particle = new SimpleParticle();
          particle.Position.Y = (float)index;
          particle.Velocity.X = 1.2f;
          system.AddParticle(particle);
        }

        IAsyncResult asyncResult = system.BeginUpdate(2, 4, null, null);
        system.EndUpdate(asyncResult);

        for(int index = 0; index < system.Particles.Count; ++index) {
          Assert.AreEqual(2.4f, system.Particles.Array[index].Position.X);
        }
      }
    }

    /// <summary>
    ///   Tests whether an exception happening during the asynchronous prune process is
    ///   caught and delivered to the caller of the EndPrune() method
    /// </summary>
    [Test]
    public void TestThrowOnExceptionDuringAsynchronousUpdate() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        system.Affectors.Add(new ThrowingAffector());
        system.AddParticle(new SimpleParticle());

        IAsyncResult asyncResult = system.BeginUpdate(2, 4, null, null);
        Assert.Throws<KeyNotFoundException>(
          delegate() { system.EndUpdate(asyncResult); }
        );
      }
    }

    /// <summary>
    ///   Tests whether an exception is thrown when EndPrune() is called with
    ///   an async result that was not returned by BeginPrune()
    /// </summary>
    [Test]
    public void TestThrowOnEndUpdateWithWrongAsyncResult() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        IAsyncResult asyncResult = system.BeginUpdate(2, 4, null, null);
        try {
          Assert.Throws<ArgumentException>(
            delegate() { system.EndUpdate(new DummyAsyncResult()); }
          );
        }
        finally {
          system.EndUpdate(asyncResult);
        }
      }
    }

    /// <summary>
    ///   Verifies that the particle system can handle multiple asynchronous prunes
    /// </summary>
    [Test]
    public void TestMultipleAsynchronousUpdates() {
      using(
        ParticleSystem<SimpleParticle> system = new ParticleSystem<SimpleParticle>(10)
      ) {
        using(WaitAffector waitAffector = new WaitAffector()) {
          for(int index = 0; index < 40; ++index) {
            system.AddParticle(new SimpleParticle());
          }
          system.Affectors.Add(waitAffector);

          DummyCallbackReceiver receiver = new DummyCallbackReceiver();
          for(int run = 0; run < 3; ++run) {
            waitAffector.Halt();
            IAsyncResult asyncResult = system.BeginUpdate(
              2, 4, receiver.Callback, receiver
            );
            try {
              Assert.IsNotNull(asyncResult.AsyncWaitHandle);
              Assert.AreSame(asyncResult.AsyncState, receiver);
              Assert.IsFalse(asyncResult.CompletedSynchronously);
            }
            finally {
              waitAffector.Continue();
              system.EndUpdate(asyncResult);
            }

            Assert.IsTrue(receiver.CallbackEvent.WaitOne(1000));
            Assert.AreEqual(run + 1, receiver.CallbackCallCount);
          }
        }
      }
    }

    /// <summary>Returns a subsection of an array as its own array</summary>
    /// <typeparam name="ItemType">Type of the items stored in the array</typeparam>
    /// <param name="array">Array from which to create a subsection</param>
    /// <param name="start">Start of the subsection in the array</param>
    /// <param name="count">Number of items that will be copied</param>
    /// <returns>A subsection of the provided array</returns>
    private static ItemType[] subArray<ItemType>(ItemType[] array, int start, int count) {
      ItemType[] segment = new ItemType[count];
      Array.Copy(array, start, segment, 0, count);
      return segment;
    }

    /// <summary>Turns an array segment into a new array</summary>
    /// <typeparam name="ItemType">Type of the items stored in the array</typeparam>
    /// <param name="segment">Array segment from which a new array will be created</param>
    /// <returns>A new array with all the items from the array segment</returns>
    private static ItemType[] toArray<ItemType>(ArraySegment<ItemType> segment) {
      return subArray(segment.Array, segment.Offset, segment.Count);
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles

#endif // UNITTEST
