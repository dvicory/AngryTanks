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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.Batching {

  /// <summary>Unit tests for the BasicEffect drawing context</summary>
  [TestFixture]
  internal class BasicEffectDrawContextTest {

    #region class TestEffectDrawContext

    /// <summary>Drawing context used for the unit test</summary>
    private class TestEffectDrawContext : EffectDrawContext {

      /// <summary>Initializes a new test effect drawing context</summary>
      /// <param name="effect">Effect that will be used for testing</param>
      public TestEffectDrawContext(Effect effect) : base(effect) { }

    }

    #endregion // class TestEffectDrawContext

    /// <summary>Verifies that the constructor of the drawing context is working</summary>
    [Test]
    public void TestConstructor() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
        using(IDisposable context = new BasicEffectDrawContext(service.GraphicsDevice)) {
          Assert.IsNotNull(context);
        }
      }
    }

    /// <summary>
    ///   Verifies that testing the drawing context against a different instance
    ///   results the comparison reporting inequality
    /// </summary>
    [Test]
    public void TestEqualsWithDifferentObject() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
        using(
          BasicEffectDrawContext test1 = new BasicEffectDrawContext(service.GraphicsDevice)
        ) {
          using(
            BasicEffectDrawContext test2 = new BasicEffectDrawContext(service.GraphicsDevice)
          ) {
            Assert.IsTrue(test1.Equals((object)test2));
          }
        }
      }
    }

    /// <summary>
    ///   Verifies that testing the drawing context against another drawing context with
    ///   an incompatible effect type results in the comparison reporting inequality
    /// </summary>
    [Test]
    public void TestEqualsWithIncompatibleEffect() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
        using(
          BasicEffectDrawContext test1 = new BasicEffectDrawContext(service.GraphicsDevice)
        ) {
          TestEffectDrawContext test2 = new TestEffectDrawContext(null);

          Assert.IsFalse(test1.Equals((object)test2));
        }
      }
    }

    /// <summary>
    ///   Verifies that the used effect can be obtained using the 'BasicEffect' property
    /// </summary>
    [Test]
    public void TestEffectRetrieval() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
        using(
          BasicEffectDrawContext test = new BasicEffectDrawContext(service.GraphicsDevice)
        ) {
          Assert.AreSame(test.Effect, test.BasicEffect);
        }
      }
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
