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

namespace Nuclex.Support {

  /// <summary>Unit Test for the strign segment class</summary>
  [TestFixture]
  public class StringSegmentTest {

    /// <summary>
    ///   Tests whether the default constructor of the StringSegment class throws the
    ///   right exception when being passed 'null' instead of a string
    /// </summary>
    [Test]
    public void TestNullStringInSimpleConstructor() {
      Assert.Throws<ArgumentNullException>(
        delegate() { new StringSegment(null); }
      );
    }

    /// <summary>
    ///   Tests whether the simple constructor of the StringSegment class accepts
    ///   an empty string
    /// </summary>
    [Test]
    public void TestEmptyStringInSimpleConstructor() {
      new StringSegment(string.Empty);
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed 'null' instead of a string
    /// </summary>
    [Test]
    public void TestNullStringInFullConstructor() {
      Assert.Throws<ArgumentNullException>(
        delegate() { new StringSegment(null, 0, 0); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class accepts
    ///   an empty string
    /// </summary>
    [Test]
    public void TestEmptyStringInFullConstructor() {
      new StringSegment(string.Empty, 0, 0);
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed an invalid start offset
    /// </summary>
    [Test]
    public void TestInvalidOffsetInConstructor() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new StringSegment(string.Empty, -1, 0); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed an invalid string length
    /// </summary>
    [Test]
    public void TestInvalidLengthInConstructor() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new StringSegment(string.Empty, 0, -1); }
      );
    }

    /// <summary>
    ///   Tests whether the full constructor of the StringSegment class throws the
    ///   right exception when being passed a string length that's too large
    /// </summary>
    [Test]
    public void TestExcessiveLengthInConstructor() {
      Assert.Throws<ArgumentException>(
        delegate() { new StringSegment("hello", 3, 3); }
      );
    }

    /// <summary>Tests whether the 'Text' property works as expected</summary>
    [Test]
    public void TestTextProperty() {
      StringSegment testSegment = new StringSegment("hello", 1, 3);
      Assert.AreEqual("hello", testSegment.Text);
    }

    /// <summary>Tests whether the 'Offset' property works as expected</summary>
    [Test]
    public void TestOffsetProperty() {
      StringSegment testSegment = new StringSegment("hello", 1, 3);
      Assert.AreEqual(1, testSegment.Offset);
    }

    /// <summary>Tests whether the 'Count' property works as expected</summary>
    [Test]
    public void TestCountProperty() {
      StringSegment testSegment = new StringSegment("hello", 1, 3);
      Assert.AreEqual(3, testSegment.Count);
    }

    /// <summary>
    ///   Tests whether two differing instances produce different hash codes
    /// </summary>
    [Test]
    public void TestHashCodeOnDifferingInstances() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);
      StringSegment howAreYouSegment = new StringSegment("how are you", 1, 9);

      Assert.AreNotEqual(
        helloWorldSegment.GetHashCode(), howAreYouSegment.GetHashCode()
      );
    }

    /// <summary>
    ///   Tests whether two equivalent instances produce an identical hash code
    /// </summary>
    [Test]
    public void TestHashCodeOnEquivalentInstances() {
      StringSegment helloWorld1Segment = new StringSegment("hello world", 2, 7);
      StringSegment helloWorld2Segment = new StringSegment("hello world", 2, 7);

      Assert.AreEqual(
        helloWorld1Segment.GetHashCode(), helloWorld2Segment.GetHashCode()
      );
    }

    /// <summary>Tests the equals method performing a comparison against null</summary>
    [Test]
    public void TestEqualsOnNull() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);

      Assert.IsFalse(
        helloWorldSegment.Equals(null)
      );
    }

    /// <summary>Tests the equality operator with differing instances</summary>
    [Test]
    public void TestEqualityOnDifferingInstances() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);
      StringSegment howAreYouSegment = new StringSegment("how are you", 1, 9);

      Assert.IsFalse(helloWorldSegment == howAreYouSegment);
    }

    /// <summary>Tests the equality operator with equivalent instances</summary>
    [Test]
    public void TestEqualityOnEquivalentInstances() {
      StringSegment helloWorld1Segment = new StringSegment("hello world", 2, 7);
      StringSegment helloWorld2Segment = new StringSegment("hello world", 2, 7);

      Assert.IsTrue(helloWorld1Segment == helloWorld2Segment);
    }

    /// <summary>Tests the inequality operator with differing instances</summary>
    [Test]
    public void TestInequalityOnDifferingInstances() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 2, 7);
      StringSegment howAreYouSegment = new StringSegment("how are you", 1, 9);

      Assert.IsTrue(helloWorldSegment != howAreYouSegment);
    }

    /// <summary>Tests the inequality operator with equivalent instances</summary>
    [Test]
    public void TestInequalityOnEquivalentInstances() {
      StringSegment helloWorld1Segment = new StringSegment("hello world", 2, 7);
      StringSegment helloWorld2Segment = new StringSegment("hello world", 2, 7);

      Assert.IsFalse(helloWorld1Segment != helloWorld2Segment);
    }

    /// <summary>Tests the ToString() method of the string segment</summary>
    [Test]
    public void TestToString() {
      StringSegment helloWorldSegment = new StringSegment("hello world", 4, 3);

      Assert.AreEqual("o w", helloWorldSegment.ToString());
    }

    /// <summary>
    ///   Tests the ToString() method of the string segment with an invalid string
    /// </summary>
    [Test]
    public void TestToStringWithInvalidString() {
      Assert.Throws<ArgumentNullException>(
        delegate() { new StringSegment(null, 4, 3); }
      );
    }

    /// <summary>
    ///   Tests the ToString() method of the string segment with an invalid offset
    /// </summary>
    [Test]
    public void TestToStringWithInvalidOffset() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new StringSegment("hello world", -4, 3); }
      );
    }

    /// <summary>
    ///   Tests the ToString() method of the string segment with an invalid count
    /// </summary>
    [Test]
    public void TestToStringWithInvalidCount() {
      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { new StringSegment("hello world", 4, -3); }
      );
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
