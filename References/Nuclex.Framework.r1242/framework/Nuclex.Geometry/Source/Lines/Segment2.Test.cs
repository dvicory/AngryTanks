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

  /// <summary>Tests the implementation of the LineSegment2 class</summary>
  [TestFixture]
  public class Segment2Test {

    /// <summary>Validates the equality operator</summary>
    [Test]
    public void TestEqualityOperator() {
      Segment2 segment1 = new Segment2(new Vector2(100.0f, 200.0f), new Vector2(300.0f, 400.0f));
      Segment2 segment2 = new Segment2(segment1);

      Assert.AreEqual(segment1, segment2, "Copied segment is equal to the Startal segment");

      segment1.Start.X = 0.0f;
      Assert.AreNotEqual(segment1, segment2, "Modified copy is no longer equal to the Startal segment");
      segment1.Start.X = 100.0f;

      segment1.Start.Y = 0.0f;
      Assert.AreNotEqual(segment1, segment2, "Modified copy is no longer equal to the Startal segment");
      segment1.Start.Y = 200.0f;

      segment1.End.X = 0.0f;
      Assert.AreNotEqual(segment1, segment2, "Modified copy is no longer equal to the Startal segment");
      segment1.End.X = 300.0f;

      segment1.End.Y = 0.0f;
      Assert.AreNotEqual(segment1, segment2, "Modified copy is no longer equal to the Startal segment");
      segment1.End.Y = 400.0f;
    }

    /// <summary>Checks whether the equality operator properly handles null</summary>
    [Test]
    public void TestEqualityOperatorAgainstNull() {
      Segment2 line = new Segment2();
      
      Assert.IsFalse(line.Equals(null), "Initialized Line is not equal to null");
    }

    /// <summary>Tests whether the constructors are working properly</summary>
    [Test]
    public void TestConstructor() {
      Segment2 line = new Segment2(new Vector2(1.0f, 2.0f), new Vector2(3.0f, 4.0f));

      Assert.AreEqual(1.0f, line.Start.X, "X start is taken over from constructor");
      Assert.AreEqual(2.0f, line.Start.Y, "Y start is taken over from constructor");
      Assert.AreEqual(3.0f, line.End.X, "X end is taken over from constructor");
      Assert.AreEqual(4.0f, line.End.Y, "Y end is taken over from constructor");
    }

    /// <summary>Tests whether the closest point determination works on the X axis</summary>
    [Test]
    public void TestClosestPointHorizontal() {
      Segment2 line = new Segment2(new Vector2(0.0f, 100.0f), new Vector2(1.0f, 100.0f));

      Vector2 leftCap = line.ClosestPointTo(new Vector2(-2.0f, 200.0f));
      Assert.AreEqual(
        new Vector2(0.0f, 100.0f), leftCap, "Closest point beyond left end found"
      );

      Vector2 leftPoint = line.ClosestPointTo(new Vector2(0, 200));
      Assert.AreEqual(
        new Vector2(0.0f, 100.0f), leftPoint, "Closest point on left end found"
      );

      Vector2 midLeftRight = line.ClosestPointTo(new Vector2(0.5f, 200.0f));
      Assert.AreEqual(
        new Vector2(0.5f, 100.0f), midLeftRight, "Closest point inmidst of line found"
      );

      Vector2 rightPoint = line.ClosestPointTo(new Vector2(1.0f, 200.0f));
      Assert.AreEqual(
        new Vector2(1.0f, 100.0f), rightPoint, "Closest point on right end found"
      );

      Vector2 rightCap = line.ClosestPointTo(new Vector2(3.0f, 200.0f));
      Assert.AreEqual(
        new Vector2(1.0f, 100.0f), rightCap, "Closest point beyond right end found"
      );
    }

    /// <summary>Tests whether the closest point determination works on the Y axis</summary>
    [Test]
    public void TestClosestPointVertical() {
      Segment2 line = new Segment2(new Vector2(100.0f, 0.0f), new Vector2(100.0f, 1.0f));

      Vector2 leftCap = line.ClosestPointTo(new Vector2(200.0f, -2.0f));
      Assert.AreEqual(
        new Vector2(100.0f, 0.0f), leftCap, "Closest point beyond lower end found"
      );

      Vector2 leftPoint = line.ClosestPointTo(new Vector2(200.0f, 0.0f));
      Assert.AreEqual(
        new Vector2(100.0f, 0.0f), leftPoint, "Closest point on lower end found"
      );

      Vector2 midLeftRight = line.ClosestPointTo(new Vector2(200.0f, 0.5f));
      Assert.AreEqual(
        new Vector2(100.0f, 0.5f), midLeftRight, "Closest point inmidst of line found"
      );

      Vector2 rightPoint = line.ClosestPointTo(new Vector2(200.0f, 1.0f));
      Assert.AreEqual(
        new Vector2(100.0f, 1.0f), rightPoint, "Closest point on upper end found"
      );

      Vector2 rightCap = line.ClosestPointTo(new Vector2(200.0f, 3.0f));
      Assert.AreEqual(
        new Vector2(100.0f, 1.0f), rightCap, "Closest point beyond upper end found"
      );
    }

    /// <summary>Tests whether the class is serialized properly</summary>
    [Test]
    public void TestSerialization() {
      XmlSerializer serializer = new XmlSerializer(typeof(Segment2));

      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      Segment2 segment = new Segment2(new Vector2(123.0f, 456.0f), new Vector2(654.0f, 321.0f));
      serializer.Serialize(stream, segment);

      stream.Seek(0, System.IO.SeekOrigin.Begin);
      Segment2 restored = (Segment2)serializer.Deserialize(stream);

      Assert.AreEqual(segment, restored, "Deserialized segment matches serialized segment");
    }
  }

} // namespace Nuclex.Geometry.Ranges

#endif // UNITTEST