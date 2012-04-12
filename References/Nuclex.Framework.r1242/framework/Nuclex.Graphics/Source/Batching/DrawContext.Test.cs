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

namespace Nuclex.Graphics.Batching {

  /// <summary>Unit tests for the drawing context</summary>
  [TestFixture]
  internal class DrawContextTest {

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

    /// <summary>
    ///   Compares the test drawing context against an incompatible object
    /// </summary>
    [Test]
    public void TestEqualsWithIncompatibleObject() {
      TestDrawContext testContext = new TestDrawContext();
      Assert.IsFalse(testContext.Equals(123));
    }

    /// <summary>
    ///   Verifies that testing the drawing context against itself results in 
    ///   the comparison reporting equality
    /// </summary>
    [Test]
    public void TestEqualsWithSameObject() {
      TestDrawContext testContext = new TestDrawContext();
      Assert.IsTrue(testContext.Equals((object)testContext));
    }

    /// <summary>
    ///   Verifies that testing the drawing context against a different instance
    ///   results the comparison reporting inequality
    /// </summary>
    [Test]
    public void TestEqualsWithDifferentObject() {
      TestDrawContext testContext1 = new TestDrawContext();
      TestDrawContext testContext2 = new TestDrawContext();
      Assert.IsFalse(testContext1.Equals((object)testContext2));
    }

    /// <summary>
    ///   Tests the hash code calculation method of the drawing context
    /// </summary>
    [Test]
    public void TestGetHashcode() {
      TestDrawContext testContext = new TestDrawContext();
      Assert.AreEqual(testContext.GetHashCode(), testContext.GetHashCode());
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
