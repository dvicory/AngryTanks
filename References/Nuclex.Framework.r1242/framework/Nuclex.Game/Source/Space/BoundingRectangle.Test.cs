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
using System.IO;

using Microsoft.Xna.Framework;

using NUnit.Framework;

namespace Nuclex.Game.Space {

  /// <summary>Unit tests for the bounding rectangle class</summary>
  [TestFixture]
  internal class BoundingRectangleTest {

    /// <summary>
    ///   Verifies that the bounding rectangle's default constructor is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      BoundingRectangle rectangle = new BoundingRectangle();
      Assert.AreEqual(Vector2.Zero, rectangle.Min);
      Assert.AreEqual(Vector2.Zero, rectangle.Max);
    }

    /// <summary>
    ///   Verifies that the bounding rectangle's constructor from two vectors is working
    /// </summary>
    /// 
    [Test]
    public void TestVectorConstructor() {
      Vector2 first = new Vector2(1.2f, 3.4f);
      Vector2 second = new Vector2(5.6f, 7.8f);
      BoundingRectangle rectangle = new BoundingRectangle(first, second);

      Assert.AreEqual(first, rectangle.Min);
      Assert.AreEqual(second, rectangle.Max);
    }

    /// <summary>
    ///   Verifies that the bounding rectangle's constructor from individual
    ///   coordinates is working
    /// </summary>
    /// 
    [Test]
    public void TestCoordinateConstructor() {
      BoundingRectangle rectangle = new BoundingRectangle(
        1.2f, 3.4f, 5.6f, 7.8f
      );

      Assert.AreEqual(1.2f, rectangle.Min.X);
      Assert.AreEqual(3.4f, rectangle.Min.Y);
      Assert.AreEqual(5.6f, rectangle.Max.X);
      Assert.AreEqual(7.8f, rectangle.Max.Y);
    }

    /// <summary>
    ///   Verifies that the hash code generation of the bounding rectangle works
    /// </summary>
    [Test]
    public void TestHashCode() {
      Vector2 first = new Vector2(1.2f, 3.4f);
      Vector2 second = new Vector2(5.6f, 7.8f);
      BoundingRectangle rectangle1 = new BoundingRectangle(first, second);
      BoundingRectangle rectangle2 = new BoundingRectangle(first, second);

      Assert.AreEqual(
        rectangle1.GetHashCode(),
        rectangle2.GetHashCode()
      );
    }

    /// <summary>Tests whether rectangles can be compared for equality</summary>
    [Test]
    public void TestEquals() {
      Vector2 first = new Vector2(1.2f, 3.4f);
      Vector2 second = new Vector2(5.6f, 7.8f);
      BoundingRectangle rectangle1 = new BoundingRectangle(first, second);
      BoundingRectangle rectangle2 = new BoundingRectangle(first, second);

      Assert.IsTrue(rectangle1.Equals(rectangle2));
      Assert.IsFalse(rectangle2.Equals(new BoundingRectangle()));
    }

    /// <summary>
    ///   Tests whether the equality comparison of boxed instances is working
    /// </summary>
    [Test]
    public void TestEqualsObject() {
      Vector2 first = new Vector2(1.2f, 3.4f);
      Vector2 second = new Vector2(5.6f, 7.8f);
      BoundingRectangle rectangle1 = new BoundingRectangle(first, second);
      BoundingRectangle rectangle2 = new BoundingRectangle(first, second);

      Assert.IsTrue(rectangle1.Equals((object)rectangle2));
      Assert.IsFalse(rectangle2.Equals((object)(new BoundingRectangle())));
    }

    /// <summary>
    ///   Tests whether the equality comparison against null is handled correctly
    /// </summary>
    [Test]
    public void TestEqualsNull() {
      BoundingRectangle rectangle = new BoundingRectangle();
      Assert.IsFalse(rectangle.Equals(null));
    }

    /// <summary>
    ///   Tests whether the equality comparison against null is handled correctly
    /// </summary>
    [Test]
    public void TestEqualsWithIncompatibleObject() {
      BoundingRectangle rectangle = new BoundingRectangle();
      Assert.IsFalse(rectangle.Equals("Hello World"));
    }

    /// <summary>Tests whether the bounding rectangle's equality operator works</summary>
    [Test]
    public void TestEqualityOperator() {
      Vector2 first = new Vector2(1.2f, 3.4f);
      Vector2 second = new Vector2(5.6f, 7.8f);
      BoundingRectangle rectangle1 = new BoundingRectangle(first, second);
      BoundingRectangle rectangle2 = new BoundingRectangle(first, second);

      Assert.IsTrue(rectangle1 == rectangle2);
      Assert.IsFalse(rectangle1 == new BoundingRectangle());
    }

    /// <summary>Tests whether the bounding rectangle's equality operator works</summary>
    [Test]
    public void TestInequalityOperator() {
      Vector2 first = new Vector2(1.2f, 3.4f);
      Vector2 second = new Vector2(5.6f, 7.8f);
      BoundingRectangle rectangle1 = new BoundingRectangle(first, second);
      BoundingRectangle rectangle2 = new BoundingRectangle(first, second);

      Assert.IsFalse(rectangle1 != rectangle2);
      Assert.IsTrue(rectangle1 != new BoundingRectangle());
    }

    /// <summary>Tests whether the bounding rectangle's equality operator works</summary>
    [Test]
    public void TestCreateMerged() {
      BoundingRectangle rectangle1 = new BoundingRectangle(
        new Vector2(10.0f, 20.0f), new Vector2(30.0f, 40.0f)
      );
      BoundingRectangle rectangle2 = new BoundingRectangle(
        new Vector2(50.0f, 60.0f), new Vector2(70.0f, 80.0f)
      );

      BoundingRectangle merged = BoundingRectangle.CreateMerged(
        rectangle1, rectangle2
      );

      Assert.AreEqual(new Vector2(10.0f, 20.0f), merged.Min);
      Assert.AreEqual(new Vector2(70.0f, 80.0f), merged.Max);
    }

    /// <summary>Verifies that containment of points can be tested for</summary>
    [Test]
    public void TestContainsPoint() {
      BoundingRectangle rectangle = new BoundingRectangle(
        new Vector2(10.0f, 20.0f), new Vector2(30.0f, 40.0f)
      );

      Assert.IsFalse(rectangle.Contains(new Vector2(9.9f, 19.9f)));
      Assert.IsTrue(rectangle.Contains(new Vector2(10.1f, 20.1f)));

      Assert.IsTrue(rectangle.Contains(new Vector2(29.9f, 39.9f)));
      Assert.IsFalse(rectangle.Contains(new Vector2(30.1f, 40.1f)));
    }

    /// <summary>Verifies that containment of points can be tested for</summary>
    [Test]
    public void TestContainsPointByReference() {
      BoundingRectangle rectangle = new BoundingRectangle(
        new Vector2(10.0f, 20.0f), new Vector2(30.0f, 40.0f)
      );

      ContainmentType type;

      Vector2 point = new Vector2(9.9f, 19.9f);
      rectangle.Contains(ref point, out type);
      Assert.AreEqual(ContainmentType.Disjoint, type);

      point = new Vector2(10.1f, 20.1f);
      rectangle.Contains(ref point, out type);
      Assert.AreEqual(ContainmentType.Contains, type);

      point = new Vector2(29.9f, 39.9f);
      rectangle.Contains(ref point, out type);
      Assert.AreEqual(ContainmentType.Contains, type);

      point = new Vector2(30.1f, 40.1f);
      rectangle.Contains(ref point, out type);
      Assert.AreEqual(ContainmentType.Disjoint, type);
    }

    /// <summary>Verifies that containment of rectangles can be tested for</summary>
    [Test]
    public void TestContainsRectangle() {
      BoundingRectangle rectangle = new BoundingRectangle(
        new Vector2(10.0f, 20.0f), new Vector2(30.0f, 40.0f)
      );

      Assert.IsFalse(rectangle.Contains(new BoundingRectangle(5.0f, 15.0f, 15.0f, 25.0f)));
      Assert.IsFalse(rectangle.Contains(new BoundingRectangle(30.1f, 40.1f, 35.0f, 45.0f)));
      Assert.IsTrue(rectangle.Contains(new BoundingRectangle(10.1f, 20.1f, 29.9f, 39.9f)));
    }

    /// <summary>Verifies that containment of rectangles can be tested for</summary>
    [Test]
    public void TestContainsRectangleByReference() {
      BoundingRectangle rectangle = new BoundingRectangle(
        new Vector2(10.0f, 20.0f), new Vector2(30.0f, 40.0f)
      );

      ContainmentType type;

      BoundingRectangle touching = new BoundingRectangle(5.0f, 15.0f, 15.0f, 25.0f);
      rectangle.Contains(ref touching, out type);
      Assert.AreEqual(ContainmentType.Intersects, type);

      BoundingRectangle outside = new BoundingRectangle(30.1f, 40.1f, 35.0f, 45.0f);
      rectangle.Contains(ref outside, out type);
      Assert.AreEqual(ContainmentType.Disjoint, type);

      BoundingRectangle contained = new BoundingRectangle(10.1f, 20.1f, 29.9f, 39.9f);
      rectangle.Contains(ref contained, out type);
      Assert.AreEqual(ContainmentType.Contains, type);
    }

    /// <summary>
    ///   Verifies that the intersection check with other rectangles is working
    /// </summary>
    [Test]
    public void TestIntersectsRectangle() {
      BoundingRectangle rectangle = new BoundingRectangle(
        10.0f, 20.0f, 30.0f, 40.0f
      );
      BoundingRectangle nonTouching = new BoundingRectangle(
        31.0f, 41.0f, 50.0f, 60.0f
      );
      BoundingRectangle touching = new BoundingRectangle(
        29.0f, 39.0f, 50.0f, 60.0f
      );

      Assert.IsFalse(rectangle.Intersects(nonTouching));
      Assert.IsTrue(rectangle.Intersects(touching));
    }

    /// <summary>
    ///   Verifies that the intersection check with other rectangles is working
    /// </summary>
    [Test]
    public void TestIntersectsRectangleByReference() {
      BoundingRectangle rectangle = new BoundingRectangle(
        10.0f, 20.0f, 30.0f, 40.0f
      );
      BoundingRectangle nonTouching = new BoundingRectangle(
        31.0f, 41.0f, 50.0f, 60.0f
      );
      BoundingRectangle touching = new BoundingRectangle(
        29.0f, 39.0f, 50.0f, 60.0f
      );

      bool result;

      rectangle.Intersects(ref nonTouching, out result);
      Assert.IsFalse(result);

      rectangle.Intersects(ref touching, out result);
      Assert.IsTrue(result);
    }

    /// <summary>Tests the ToString() method</summary>
    [Test]
    public void TestToString() {
      BoundingRectangle rectangle = new BoundingRectangle();
      Assert.IsNotNull(rectangle.ToString());
    }

  }

} // namespace Nuclex.Game.Space

#endif // UNITTEST
