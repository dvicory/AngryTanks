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

using System.Xml.Serialization;

using Microsoft.Xna.Framework;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Geometry.Lines {

  /// <summary>Tests the implementation of the Ray2 class</summary>
  [TestFixture]
  public class Ray2Test {

    /// <summary>Validates the equality operator</summary>
    [Test]
    public void TestEqualityOperator() {
      Ray2 ray1 = new Ray2(new Vector2(100.0f, 200.0f), new Vector2(300.0f, 400.0f));
      Ray2 ray2 = new Ray2(ray1);

      Assert.AreEqual(ray1, ray2, "Copied ray is equal to the original ray");

      ray1.Origin.X = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Origin.X = 100.0f;

      ray1.Origin.Y = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Origin.Y = 200.0f;

      ray1.Direction.X = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Direction.X = 300.0f;

      ray1.Direction.Y = 0.0f;
      Assert.AreNotEqual(ray1, ray2, "Modified copy is no longer equal to the original ray");
      ray1.Direction.Y = 400.0f;
    }

    /// <summary>Checks whether the equality operator properly handles null</summary>
    [Test]
    public void TestEqualityOperatorAgainstNull() {
      Ray2 line = new Ray2();
      
      Assert.IsFalse(line.Equals(null), "Initialized Line is not equal to null");
    }

    /// <summary>Tests whether the constructors are working properly</summary>
    [Test]
    public void TestConstructor() {
      Ray2 ray = new Ray2(new Vector2(2.0f, 3.0f), new Vector2(1.0f, 0.0f));

      Assert.AreEqual(2.0f, ray.Origin.X, "X origin is taken over from constructor");
      Assert.AreEqual(3.0f, ray.Origin.Y, "Y origin is taken over from constructor");
      Assert.AreEqual(1.0f, ray.Direction.X, "X direction is taken over from constructor");
      Assert.AreEqual(0.0f, ray.Direction.Y, "Y direction is taken over from constructor");

      ray = new Ray2(new Vector2(2.0f, 3.0f), new Vector2(0.0f, 1.0f));

      Assert.AreEqual(2.0f, ray.Origin.X, "X origin is taken over from constructor");
      Assert.AreEqual(3.0f, ray.Origin.Y, "Y origin is taken over from constructor");
      Assert.AreEqual(0.0f, ray.Direction.X, "X direction is taken over from constructor");
      Assert.AreEqual(1.0f, ray.Direction.Y, "Y direction is taken over from constructor");
    }

    /// <summary>Tests whether the closest point determination works on the X axis</summary>
    [Test]
    public void TestClosestPointHorizontal() {
      Ray2 ray = new Ray2(new Vector2(0.0f, 100.0f), Vector2.UnitX);

      Vector2 leftCap = ray.ClosestPointTo(new Vector2(-2.0f, 200.0f));
      Assert.AreEqual(
        new Vector2(0.0f, 100.0f), leftCap, "Closest point beyond left end found"
      );

      Vector2 leftPoint = ray.ClosestPointTo(new Vector2(0.0f, 200.0f));
      Assert.AreEqual(
        new Vector2(0.0f, 100.0f), leftPoint, "Closest point on left end found"
      );

      Vector2 midLeftRight = ray.ClosestPointTo(new Vector2(0.5f, 200.0f));
      Assert.AreEqual(
        new Vector2(0.5f, 100.0f), midLeftRight, "Closest point inmidst of line found"
      );

      Vector2 rightPoint = ray.ClosestPointTo(new Vector2(1.0f, 200.0f));
      Assert.AreEqual(
        new Vector2(1.0f, 100.0f), rightPoint, "Closest point on right end found"
      );

      Vector2 rightCap = ray.ClosestPointTo(new Vector2(3.0f, 200.0f));
      Assert.AreEqual(
        new Vector2(3.0f, 100.0f), rightCap, "Closest point beyond right end found"
      );
    }

    /// <summary>Tests whether the closest point determination works on the Y axis</summary>
    [Test]
    public void TestClosestPointVertical() {
      Ray2 ray = new Ray2(new Vector2(100.0f, 0.0f), Vector2.UnitY);

      Vector2 leftCap = ray.ClosestPointTo(new Vector2(200.0f, -2.0f));
      Assert.AreEqual(new Vector2(100.0f, 0.0f), leftCap, "Closest point beyond lower end found");

      Vector2 leftPoint = ray.ClosestPointTo(new Vector2(200.0f, 0.0f));
      Assert.AreEqual(new Vector2(100.0f, 0.0f), leftPoint, "Closest point on lower end found");

      Vector2 midLeftRight = ray.ClosestPointTo(new Vector2(200.0f, 0.5f));
      Assert.AreEqual(new Vector2(100.0f, 0.5f), midLeftRight, "Closest point inmidst of line found");

      Vector2 rightPoint = ray.ClosestPointTo(new Vector2(200.0f, 1.0f));
      Assert.AreEqual(new Vector2(100.0f, 1.0f), rightPoint, "Closest point on upper end found");

      Vector2 rightCap = ray.ClosestPointTo(new Vector2(200.0f, 3.0f));
      Assert.AreEqual(new Vector2(100.0f, 3.0f), rightCap, "Closest point beyond upper end found");
    }

    /// <summary>Tests whether the class is serialized properly</summary>
    [Test]
    public void TestSerialization() {
      XmlSerializer serializer = new XmlSerializer(typeof(Ray2));

      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      Ray2 ray = new Ray2(new Vector2(123.0f, 456.0f), new Vector2(654.0f, 321.0f));
      serializer.Serialize(stream, ray);

      stream.Seek(0, System.IO.SeekOrigin.Begin);
      Ray2 restored = (Ray2)serializer.Deserialize(stream);

      Assert.AreEqual(ray, restored, "Deserialized ray matches serialized ray");
    }
  }

} // namespace Nuclex.Geometry.Ranges

#endif // UNITTEST