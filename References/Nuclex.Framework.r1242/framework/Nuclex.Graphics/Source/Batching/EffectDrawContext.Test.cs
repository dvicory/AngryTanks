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

  /// <summary>Unit tests for the Effect drawing context</summary>
  [TestFixture]
  internal class EffectDrawContextTest {

    #region class TestDrawContext

    /// <summary>Drawing context used for the unit test</summary>
    private class TestDrawContext : DrawContext {

      /// <summary>Number of passes this draw context requires for rendering</summary>
      public override int Passes {
        get { return 123; }
      }

#if !XNA_4

      /// <summary>Begins the drawing cycle</summary>
      public override void Begin() { }

      /// <summary>Ends the drawing cycle</summary>
      public override void End() { }

      /// <summary>Prepares the graphics device for drawing</summary>
      /// <param name="pass">Index of the pass to begin rendering</param>
      /// <remarks>
      ///   Should only be called between the normal Begin() and End() methods.
      /// </remarks>
      public override void BeginPass(int pass) { }

      /// <summary>Restores the graphics device after drawing has finished</summary>
      /// <remarks>
      ///   Should only be called between the normal Begin() and End() methods.
      /// </remarks>
      public override void EndPass() { }

#else

      /// <summary>Prepares the graphics device for drawing</summary>
      /// <param name="pass">Index of the pass to begin rendering</param>
      /// <remarks>
      ///   Should only be called between the normal Begin() and End() methods.
      /// </remarks>
      public override void Apply(int pass) { }

#endif

      /// <summary>Tests whether another draw context is identical to this one</summary>
      /// <param name="otherContext">Other context to check for equality</param>
      /// <returns>True if the other context is identical to this one</returns>
      public override bool Equals(DrawContext otherContext) {
        return ReferenceEquals(this, otherContext);
      }

    }

    #endregion // class TestDrawContext

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
#if XNA_4
        using(BasicEffect effect = new BasicEffect(service.GraphicsDevice)) {
          TestEffectDrawContext test = new TestEffectDrawContext(effect);
          Assert.GreaterOrEqual(test.Passes, 1);
        }
#else
        using(EffectPool pool = new EffectPool()) {
          using(BasicEffect effect = new BasicEffect(service.GraphicsDevice, pool)) {
            TestEffectDrawContext test = new TestEffectDrawContext(effect);
            Assert.GreaterOrEqual(test.Passes, 1);
          }
        }
#endif
      }
    }

    /// <summary>
    ///   Verifies that the Begin() and End() methods of the drawing context are working
    ///   as expected
    /// </summary>
    [Test]
    public void TestBeginEnd() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
#if XNA_4
        using(BasicEffect effect = new BasicEffect(service.GraphicsDevice)) {
          TestEffectDrawContext test = new TestEffectDrawContext(effect);

          for(int pass = 0; pass < test.Passes; ++pass) {
            test.Apply(pass);
          }
        }
#else
        using(EffectPool pool = new EffectPool()) {
          using(BasicEffect effect = new BasicEffect(service.GraphicsDevice, pool)) {
            TestEffectDrawContext test = new TestEffectDrawContext(effect);

            test.Begin();
            try {
              for(int pass = 0; pass < test.Passes; ++pass) {
                test.BeginPass(pass);
                test.EndPass();
              }
            }
            finally {
              test.End();
            }
          }
        }
#endif
      }
    }

    /// <summary>
    ///   Verifies that the used effect can be obtained using the 'Effect' property
    /// </summary>
    [Test]
    public void TestEffectRetrieval() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
#if XNA_4
        using(BasicEffect effect = new BasicEffect(service.GraphicsDevice)) {
          TestEffectDrawContext test = new TestEffectDrawContext(effect);

          Assert.AreSame(effect, test.Effect);
        }
#else
        using(EffectPool pool = new EffectPool()) {
          using(BasicEffect effect = new BasicEffect(service.GraphicsDevice, pool)) {
            TestEffectDrawContext test = new TestEffectDrawContext(effect);

            Assert.AreSame(effect, test.Effect);
          }
        }
#endif
      }
    }

    /// <summary>
    ///   Verifies that testing the drawing context against itself results in 
    ///   the comparison reporting equality
    /// </summary>
    [Test]
    public void TestEqualsWithSameObject() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
#if XNA_4
        using(BasicEffect effect = new BasicEffect(service.GraphicsDevice)) {
          TestEffectDrawContext test = new TestEffectDrawContext(effect);
          Assert.IsTrue(test.Equals((object)test));
        }
#else
        using(EffectPool pool = new EffectPool()) {
          using(BasicEffect effect = new BasicEffect(service.GraphicsDevice, pool)) {
            TestEffectDrawContext test = new TestEffectDrawContext(effect);
            Assert.IsTrue(test.Equals((object)test));
          }
        }
#endif
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
#if XNA_4
        using(BasicEffect effect1 = new BasicEffect(service.GraphicsDevice)) {
          using(BasicEffect effect2 = new BasicEffect(service.GraphicsDevice)) {
            TestEffectDrawContext test1 = new TestEffectDrawContext(effect1);
            TestEffectDrawContext test2 = new TestEffectDrawContext(effect2);
            Assert.IsFalse(test1.Equals((object)test2));
          }
        }
#else
        using(EffectPool pool = new EffectPool()) {
          using(BasicEffect effect1 = new BasicEffect(service.GraphicsDevice, pool)) {
            using(BasicEffect effect2 = new BasicEffect(service.GraphicsDevice, pool)) {
              TestEffectDrawContext test1 = new TestEffectDrawContext(effect1);
              TestEffectDrawContext test2 = new TestEffectDrawContext(effect2);
              Assert.IsFalse(test1.Equals((object)test2));
            }
          }
        }
#endif
      }
    }

    /// <summary>
    ///   Verifies that testing the drawing context against an instance of a different
    ///   drawing context is reported as inequality
    /// </summary>
    [Test]
    public void TestEqualsWithIncpmpatibleDrawContext() {
      MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
      using(IDisposable keeper = service.CreateDevice()) {
#if XNA_4
        using(BasicEffect effect = new BasicEffect(service.GraphicsDevice)) {
          TestEffectDrawContext test1 = new TestEffectDrawContext(effect);
          TestDrawContext test2 = new TestDrawContext();
          Assert.IsFalse(test1.Equals((object)test2));
        }
#else
        using(EffectPool pool = new EffectPool()) {
          using(BasicEffect effect = new BasicEffect(service.GraphicsDevice, pool)) {
            TestEffectDrawContext test1 = new TestEffectDrawContext(effect);
            TestDrawContext test2 = new TestDrawContext();
            Assert.IsFalse(test1.Equals((object)test2));
          }
        }
#endif
      }
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
