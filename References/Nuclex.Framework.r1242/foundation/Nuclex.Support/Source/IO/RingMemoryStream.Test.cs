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

namespace Nuclex.Support.IO {

  /// <summary>Unit Test for the ring buffer class</summary>
  [TestFixture]
  public class RingMemoryStreamTest {

    /// <summary>Prepares some test data for the units test methods</summary>
    [TestFixtureSetUp]
    public void Setup() {
      this.testBytes = new byte[20];
      for(int i = 0; i < 20; ++i)
        this.testBytes[i] = (byte)i;
    }

    /// <summary>
    ///   Ensures that the ring buffer blocks write attempts that would exceed its capacity
    /// </summary>
    [Test]
    public void TestWriteTooLargeChunk() {
      Assert.Throws<OverflowException>(
        delegate() { new RingMemoryStream(10).Write(this.testBytes, 0, 11); }
      );
    }

    /// <summary>
    ///   Ensures that the ring buffer still accepts write attempts that would fill the
    ///   entire buffer in one go.
    /// </summary>
    [Test]
    public void TestWriteBarelyFittingChunk() {
      new RingMemoryStream(10).Write(this.testBytes, 0, 10);
    }

    /// <summary>
    ///   Ensures that the ring buffer correctly manages write attempts that have to
    ///   be split at the end of the ring buffer
    /// </summary>
    [Test]
    public void TestWriteSplitBlock() {
      RingMemoryStream testRing = new RingMemoryStream(10);
      testRing.Write(this.testBytes, 0, 8);
      testRing.Read(this.testBytes, 0, 5);
      testRing.Write(this.testBytes, 0, 7);

      byte[] actual = new byte[10];
      testRing.Read(actual, 0, 10);
      Assert.AreEqual(new byte[] { 5, 6, 7, 0, 1, 2, 3, 4, 5, 6 }, actual);
    }

    /// <summary>
    ///   Ensures that the ring buffer correctly manages write attempts that write into
    ///   the gap after the ring buffer's data has become split
    /// </summary>
    [Test]
    public void TestWriteSplitAndLinearBlock() {
      RingMemoryStream testRing = new RingMemoryStream(10);
      testRing.Write(this.testBytes, 0, 8);
      testRing.Read(this.testBytes, 0, 5);
      testRing.Write(this.testBytes, 0, 5);
      testRing.Write(this.testBytes, 0, 2);

      byte[] actual = new byte[10];
      testRing.Read(actual, 0, 10);
      Assert.AreEqual(new byte[] { 5, 6, 7, 0, 1, 2, 3, 4, 0, 1 }, actual);
    }

    /// <summary>
    ///   Ensures that the ring buffer still detects write that would exceed its capacity
    ///   if they write into the gap after the ring buffer's data has become split
    /// </summary>
    [Test]
    public void TestWriteSplitAndLinearTooLargeBlock() {
      RingMemoryStream testRing = new RingMemoryStream(10);
      testRing.Write(this.testBytes, 0, 8);
      testRing.Read(this.testBytes, 0, 5);
      testRing.Write(this.testBytes, 0, 5);
      Assert.Throws<OverflowException>(
        delegate() { testRing.Write(this.testBytes, 0, 3); }
      );
    }

    /// <summary>Tests whether the ring buffer correctly handles fragmentation</summary>
    [Test]
    public void TestSplitBlockWrappedRead() {
      RingMemoryStream testRing = new RingMemoryStream(10);
      testRing.Write(this.testBytes, 0, 10);
      testRing.Read(this.testBytes, 0, 5);
      testRing.Write(this.testBytes, 0, 5);

      byte[] actual = new byte[10];
      testRing.Read(actual, 0, 10);
      Assert.AreEqual(new byte[] { 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 }, actual);
    }

    /// <summary>Tests whether the ring buffer correctly handles fragmentation</summary>
    [Test]
    public void TestSplitBlockLinearRead() {
      RingMemoryStream testRing = new RingMemoryStream(10);
      testRing.Write(this.testBytes, 0, 10);
      testRing.Read(this.testBytes, 0, 5);
      testRing.Write(this.testBytes, 0, 5);

      byte[] actual = new byte[5];
      testRing.Read(actual, 0, 5);
      Assert.AreEqual(new byte[] { 5, 6, 7, 8, 9 }, actual);
    }

    /// <summary>
    ///   Tests whether the ring buffer correctly returns partial data if more
    ///   data is requested than is contained in it.
    /// </summary>
    [Test]
    public void TestEndOfStream() {
      byte[] tempBytes = new byte[10];

      RingMemoryStream testRing = new RingMemoryStream(10);
      Assert.AreEqual(0, testRing.Read(tempBytes, 0, 5));

      testRing.Write(this.testBytes, 0, 5);
      Assert.AreEqual(5, testRing.Read(tempBytes, 0, 10));

      testRing.Write(this.testBytes, 0, 6);
      testRing.Read(tempBytes, 0, 5);
      testRing.Write(this.testBytes, 0, 9);
      Assert.AreEqual(10, testRing.Read(tempBytes, 0, 20));
    }

    /// <summary>
    ///   Validates that the ring buffer can extend its capacity without loosing data
    /// </summary>
    [Test]
    public void TestCapacityIncrease() {
      RingMemoryStream testRing = new RingMemoryStream(10);
      testRing.Write(this.testBytes, 0, 10);

      testRing.Capacity = 20;
      byte[] actual = new byte[10];
      testRing.Read(actual, 0, 10);

      Assert.AreEqual(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, actual);
    }

    /// <summary>
    ///   Validates that the ring buffer can reduce its capacity without loosing data
    /// </summary>
    [Test]
    public void TestCapacityDecrease() {
      RingMemoryStream testRing = new RingMemoryStream(20);
      testRing.Write(this.testBytes, 0, 10);

      testRing.Capacity = 10;
      byte[] actual = new byte[10];
      testRing.Read(actual, 0, 10);

      Assert.AreEqual(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, actual);
    }

    /// <summary>
    ///   Checks that an exception is thrown when the ring buffer's capacity is
    ///   reduced so much it would have to give up some of its contained data
    /// </summary>
    [Test]
    public void TestCapacityDecreaseException() {
      RingMemoryStream testRing = new RingMemoryStream(20);
      testRing.Write(this.testBytes, 0, 20);

      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() { testRing.Capacity = 10; }
      );
    }

    /// <summary>Tests whether the Capacity property returns the current capacity</summary>
    [Test]
    public void TestCapacity() {
      RingMemoryStream testRing = new RingMemoryStream(123);

      Assert.AreEqual(123, testRing.Capacity);
    }

    /// <summary>Ensures that the CanRead property returns true</summary>
    [Test]
    public void TestCanRead() {
      Assert.IsTrue(new RingMemoryStream(10).CanRead);
    }

    /// <summary>Ensures that the CanSeek property returns false</summary>
    [Test]
    public void TestCanSeek() {
      Assert.IsFalse(new RingMemoryStream(10).CanSeek);
    }

    /// <summary>Ensures that the CanWrite property returns true</summary>
    [Test]
    public void TestCanWrite() {
      Assert.IsTrue(new RingMemoryStream(10).CanWrite);
    }

    /// <summary>
    ///   Tests whether the auto reset feature works (resets the buffer pointer to the
    ///   left end of the buffer when it gets empty; mainly a performance feature).
    /// </summary>
    [Test]
    public void TestAutoReset() {
      byte[] tempBytes = new byte[10];
      RingMemoryStream testRing = new RingMemoryStream(10);

      testRing.Write(this.testBytes, 0, 8);
      testRing.Read(tempBytes, 0, 2);
      testRing.Read(tempBytes, 0, 2);
      testRing.Read(tempBytes, 0, 1);
      testRing.Read(tempBytes, 0, 1);

      Assert.AreEqual(2, testRing.Length);
    }

    /// <summary>
    ///   Verifies that an exception is thrown when the Position property of the ring
    ///   memory stream is used to retrieve the current file pointer position
    /// </summary>
    [Test]
    public void TestThrowOnRetrievePosition() {
      Assert.Throws<NotSupportedException>(
        delegate() { Console.WriteLine(new RingMemoryStream(10).Position); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown when the Position property of the ring
    ///   memory stream is used to modify the current file pointer position
    /// </summary>
    [Test]
    public void TestThrowOnAssignPosition() {
      Assert.Throws<NotSupportedException>(
        delegate() { new RingMemoryStream(10).Position = 0; }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown when the Seek() method of the ring memory
    ///   stream is attempted to be used
    /// </summary>
    [Test]
    public void TestThrowOnSeek() {
      Assert.Throws<NotSupportedException>(
        delegate() { new RingMemoryStream(10).Seek(0, SeekOrigin.Begin); }
      );
    }

    /// <summary>
    ///   Verifies that an exception is thrown when the SetLength() method of the ring
    ///   memory stream is attempted to be used
    /// </summary>
    [Test]
    public void TestThrowOnSetLength() {
      Assert.Throws<NotSupportedException>(
        delegate() { new RingMemoryStream(10).SetLength(10); }
      );
    }

    /// <summary>
    ///   Tests the Flush() method of the ring memory stream, which is either a dummy
    ///   implementation or has no side effects
    /// </summary>
    [Test]
    public void TestFlush() {
      new RingMemoryStream(10).Flush();
    }

    /// <summary>
    ///   Tests whether the length property is updated in accordance to the data written
    ///   into the ring memory stream
    /// </summary>
    [Test]
    public void TestLengthOnLinearBlock() {
      RingMemoryStream testRing = new RingMemoryStream(10);
      testRing.Write(new byte[10], 0, 10);

      Assert.AreEqual(10, testRing.Length);
    }

    /// <summary>
    ///   Tests whether the length property is updated in accordance to the data written
    ///   into the ring memory stream when the data is split within the stream
    /// </summary>
    [Test]
    public void TestLengthOnSplitBlock() {
      RingMemoryStream testRing = new RingMemoryStream(10);

      testRing.Write(new byte[10], 0, 10);
      testRing.Read(new byte[5], 0, 5);
      testRing.Write(new byte[5], 0, 5);

      Assert.AreEqual(10, testRing.Length);
    }

    /// <summary>Test data for the ring buffer unit tests</summary>
    private byte[] testBytes;

  }

} // namespace Nuclex.Support.IO

#endif // UNITTEST
