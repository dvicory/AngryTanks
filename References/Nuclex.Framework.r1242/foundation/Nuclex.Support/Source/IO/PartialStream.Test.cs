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

#if UNITTEST

using System;
using System.IO;

using NUnit.Framework;

namespace Nuclex.Support.IO {

  /// <summary>Unit Test for the partial stream</summary>
  [TestFixture]
  public class PartialStreamTest {

    #region class TestStream

    /// <summary>Testing stream that allows specific features to be disabled</summary>
    private class TestStream : Stream {

      /// <summary>Initializes a new test stream</summary>
      /// <param name="wrappedStream">Stream that will be wrapped</param>
      /// <param name="allowRead">Whether to allow reading from the stream</param>
      /// <param name="allowWrite">Whether to allow writing to the stream</param>
      /// <param name="allowSeek">Whether to allow seeking within the stream</param>
      public TestStream(
        Stream wrappedStream, bool allowRead, bool allowWrite, bool allowSeek
      ) {
        this.stream = wrappedStream;
        this.readAllowed = allowRead;
        this.writeAllowed = allowWrite;
        this.seekAllowed = allowSeek;
      }

      /// <summary>Whether data can be read from the stream</summary>
      public override bool CanRead {
        get { return this.readAllowed; }
      }

      /// <summary>Whether the stream supports seeking</summary>
      public override bool CanSeek {
        get { return this.seekAllowed; }
      }

      /// <summary>Whether data can be written into the stream</summary>
      public override bool CanWrite {
        get { return this.writeAllowed; }
      }

      /// <summary>
      ///   Clears all buffers for this stream and causes any buffered data to be written
      ///   to the underlying device.
      /// </summary>
      public override void Flush() {
        ++this.flushCallCount;
        this.stream.Flush();
      }

      /// <summary>Length of the stream in bytes</summary>
      public override long Length {
        get {
          enforceSeekAllowed();
          return this.stream.Length;
        }
      }

      /// <summary>Absolute position of the file pointer within the stream</summary>
      /// <exception cref="NotSupportedException">
      ///   At least one of the chained streams does not support seeking
      /// </exception>
      public override long Position {
        get {
          enforceSeekAllowed();
          return this.stream.Position;
        }
        set {
          enforceSeekAllowed();
          this.stream.Position = value;
        }
      }

      /// <summary>
      ///   Reads a sequence of bytes from the stream and advances the position of
      ///   the file pointer by the number of bytes read.
      /// </summary>
      /// <param name="buffer">Buffer that will receive the data read from the stream</param>
      /// <param name="offset">
      ///   Offset in the buffer at which the stream will place the data read
      /// </param>
      /// <param name="count">Maximum number of bytes that will be read</param>
      /// <returns>
      ///   The number of bytes that were actually read from the stream and written into
      ///   the provided buffer
      /// </returns>
      public override int Read(byte[] buffer, int offset, int count) {
        enforceReadAllowed();
        return this.stream.Read(buffer, offset, count);
      }

      /// <summary>Changes the position of the file pointer</summary>
      /// <param name="offset">
      ///   Offset to move the file pointer by, relative to the position indicated by
      ///   the <paramref name="origin" /> parameter.
      /// </param>
      /// <param name="origin">
      ///   Reference point relative to which the file pointer is placed
      /// </param>
      /// <returns>The new absolute position within the stream</returns>
      public override long Seek(long offset, SeekOrigin origin) {
        enforceSeekAllowed();
        return this.stream.Seek(offset, origin);
      }

      /// <summary>Changes the length of the stream</summary>
      /// <param name="value">New length the stream shall have</param>
      public override void SetLength(long value) {
        enforceSeekAllowed();
        this.stream.SetLength(value);
      }

      /// <summary>
      ///   Writes a sequence of bytes to the stream and advances the position of
      ///   the file pointer by the number of bytes written.
      /// </summary>
      /// <param name="buffer">
      ///   Buffer containing the data that will be written to the stream
      /// </param>
      /// <param name="offset">
      ///   Offset in the buffer at which the data to be written starts
      /// </param>
      /// <param name="count">Number of bytes that will be written into the stream</param>
      public override void Write(byte[] buffer, int offset, int count) {
        enforceWriteAllowed();
        this.stream.Write(buffer, offset, count);
      }

      /// <summary>Number of times the Flush() method has been called</summary>
      public int FlushCallCount {
        get { return this.flushCallCount; }
      }

      /// <summary>Throws an exception if reading is not allowed</summary>
      private void enforceReadAllowed() {
        if(!this.readAllowed) {
          throw new NotSupportedException("Reading has been disabled");
        }
      }

      /// <summary>Throws an exception if writing is not allowed</summary>
      private void enforceWriteAllowed() {
        if(!this.writeAllowed) {
          throw new NotSupportedException("Writing has been disabled");
        }
      }

      /// <summary>Throws an exception if seeking is not allowed</summary>
      private void enforceSeekAllowed() {
        if(!this.seekAllowed) {
          throw new NotSupportedException("Seeking has been disabled");
        }
      }

      /// <summary>Stream being wrapped for testing</summary>
      private Stream stream;
      /// <summary>whether to allow reading from the wrapped stream</summary>
      private bool readAllowed;
      /// <summary>Whether to allow writing to the wrapped stream</summary>
      private bool writeAllowed;
      /// <summary>Whether to allow seeking within the wrapped stream</summary>
      private bool seekAllowed;
      /// <summary>Number of times the Flush() method has been called</summary>
      private int flushCallCount;

    }

    #endregion // class TestStream

    /// <summary>Tests whether the partial stream constructor is working</summary>
    [Test]
    public void TestConstructor() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);
        PartialStream partialStream = new PartialStream(memoryStream, 23, 100);
        Assert.AreEqual(100, partialStream.Length);
      }
    }

    /// <summary>
    ///   Verifies that the partial stream constructor throws an exception if
    ///   it's invoked with an invalid start offset
    /// </summary>
    [Test]
    public void TestThrowOnInvalidStartInConstructor() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);
        Assert.Throws<ArgumentException>(
          delegate() { Console.WriteLine(new PartialStream(memoryStream, -1, 10)); }
        );
      }
    }

    /// <summary>
    ///   Verifies that the partial stream constructor throws an exception if
    ///   it's invoked with an invalid start offset
    /// </summary>
    [Test]
    public void TestThrowOnInvalidLengthInConstructor() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);
        Assert.Throws<ArgumentException>(
          delegate() { Console.WriteLine(new PartialStream(memoryStream, 100, 24)); }
        );
      }
    }

    /// <summary>
    ///   Verifies that the partial stream constructor throws an exception if
    ///   it's invoked with a start offset on an unseekable stream
    /// </summary>
    [Test]
    public void TestThrowOnUnseekableStreamWithOffsetInConstructor() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);
        TestStream testStream = new TestStream(memoryStream, true, true, false);
        Assert.Throws<ArgumentException>(
          delegate() { Console.WriteLine(new PartialStream(testStream, 23, 100)); }
        );
      }
    }

    /// <summary>
    ///   Tests whether the CanRead property reports its status correctly
    /// </summary>
    [Test]
    public void TestCanReadProperty() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        TestStream yesStream = new TestStream(memoryStream, true, true, true);
        TestStream noStream = new TestStream(memoryStream, false, true, true);

        Assert.IsTrue(new PartialStream(yesStream, 0, 0).CanRead);
        Assert.IsFalse(new PartialStream(noStream, 0, 0).CanRead);
      }
    }

    /// <summary>
    ///   Tests whether the CanWrite property reports its status correctly
    /// </summary>
    [Test]
    public void TestCanWriteProperty() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        TestStream yesStream = new TestStream(memoryStream, true, true, true);
        TestStream noStream = new TestStream(memoryStream, true, false, true);

        Assert.IsTrue(new PartialStream(yesStream, 0, 0).CanWrite);
        Assert.IsFalse(new PartialStream(noStream, 0, 0).CanWrite);
      }
    }

    /// <summary>
    ///   Tests whether the CanSeek property reports its status correctly
    /// </summary>
    [Test]
    public void TestCanSeekProperty() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        TestStream yesStream = new TestStream(memoryStream, true, true, true);
        TestStream noStream = new TestStream(memoryStream, true, true, false);

        Assert.IsTrue(new PartialStream(yesStream, 0, 0).CanSeek);
        Assert.IsFalse(new PartialStream(noStream, 0, 0).CanSeek);
      }
    }

    /// <summary>
    ///   Tests whether the CompleteStream property returns the original stream
    /// </summary>
    [Test]
    public void TestCompleteStreamProperty() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        PartialStream partialStream = new PartialStream(memoryStream, 0, 0);
        Assert.AreSame(memoryStream, partialStream.CompleteStream);
      }
    }

    /// <summary>Tests whether the Flush() method can be called</summary>
    [Test]
    public void TestFlush() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        PartialStream partialStream = new PartialStream(memoryStream, 0, 0);
        partialStream.Flush();
      }
    }

    /// <summary>
    ///   Tests whether the Position property correctly reports the file pointer position
    /// </summary>
    [Test]
    public void TestGetPosition() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);
        PartialStream partialStream = new PartialStream(memoryStream, 23, 100);

        Assert.AreEqual(0, partialStream.Position);

        byte[] test = new byte[10];
        int bytesRead = partialStream.Read(test, 0, 10);

        Assert.AreEqual(10, bytesRead);
        Assert.AreEqual(10, partialStream.Position);

        bytesRead = partialStream.Read(test, 0, 10);

        Assert.AreEqual(10, bytesRead);
        Assert.AreEqual(20, partialStream.Position);
      }
    }

    /// <summary>
    ///   Tests whether the Position property is correctly updated
    /// </summary>
    [Test]
    public void TestSetPosition() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);
        PartialStream partialStream = new PartialStream(memoryStream, 23, 100);

        Assert.AreEqual(0, partialStream.Position);
        partialStream.Position = 7;
        Assert.AreEqual(partialStream.Position, 7);
        partialStream.Position = 14;
        Assert.AreEqual(partialStream.Position, 14);
      }
    }

    /// <summary>
    ///   Tests whether the Position property throws an exception if the stream does
    ///   not support seeking.
    /// </summary>
    [Test]
    public void TestThrowOnGetPositionOnUnseekableStream() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        TestStream testStream = new TestStream(memoryStream, true, true, false);

        PartialStream partialStream = new PartialStream(testStream, 0, 0);
        Assert.Throws<NotSupportedException>(
          delegate() { Console.WriteLine(partialStream.Position); }
        );
      }
    }

    /// <summary>
    ///   Tests whether the Position property throws an exception if the stream does
    ///   not support seeking.
    /// </summary>
    [Test]
    public void TestThrowOnSetPositionOnUnseekableStream() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        TestStream testStream = new TestStream(memoryStream, true, true, false);

        PartialStream partialStream = new PartialStream(testStream, 0, 0);
        Assert.Throws<NotSupportedException>(
          delegate() { partialStream.Position = 0; }
        );
      }
    }

    /// <summary>
    ///   Tests whether the Read() method throws an exception if the stream does
    ///   not support reading
    /// </summary>
    [Test]
    public void TestThrowOnReadFromUnreadableStream() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        TestStream testStream = new TestStream(memoryStream, false, true, true);
        PartialStream partialStream = new PartialStream(testStream, 0, 0);

        byte[] test = new byte[10];
        Assert.Throws<NotSupportedException>(
          delegate() { Console.WriteLine(partialStream.Read(test, 0, 10)); }
        );
      }
    }

    /// <summary>
    ///   Tests whether the Seek() method of the partial stream is working
    /// </summary>
    [Test]
    public void TestSeeking() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(20);
        PartialStream partialStream = new PartialStream(memoryStream, 0, 20);

        Assert.AreEqual(7, partialStream.Seek(-13, SeekOrigin.End));
        Assert.AreEqual(14, partialStream.Seek(7, SeekOrigin.Current));
        Assert.AreEqual(11, partialStream.Seek(11, SeekOrigin.Begin));
      }
    }

    /// <summary>
    ///   Tests whether the Seek() method throws an exception if an invalid
    ///   reference point is provided
    /// </summary>
    [Test]
    public void TestThrowOnInvalidSeekReferencePoint() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        PartialStream partialStream = new PartialStream(memoryStream, 0, 0);
        Assert.Throws<ArgumentException>(
          delegate() { partialStream.Seek(1, (SeekOrigin)12345); }
        );
      }
    }

    /// <summary>
    ///   Verifies that the partial stream throws an exception if the attempt is
    ///   made to change the length of the stream
    /// </summary>
    [Test]
    public void TestThrowOnLengthChange() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        PartialStream partialStream = new PartialStream(memoryStream, 0, 0);
        Assert.Throws<NotSupportedException>(
          delegate() { partialStream.SetLength(123); }
        );
      }
    }

    /// <summary>
    ///   Tests whether the Read() method returns 0 bytes if the attempt is made
    ///   to read data from an invalid position
    /// </summary>
    [Test]
    public void TestReadFromInvalidPosition() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);

        memoryStream.Position = 1123;
        byte[] test = new byte[10];

        Assert.AreEqual(0, memoryStream.Read(test, 0, 10));
      }
    }

    /// <summary>Verifies that the Read() method is working</summary>
    [Test]
    public void TestReadFromPartialStream() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);

        memoryStream.Position = 100;
        memoryStream.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 0, 10);

        PartialStream partialStream = new PartialStream(memoryStream, 95, 10);

        byte[] buffer = new byte[15];
        int bytesRead = partialStream.Read(buffer, 0, 15);

        Assert.AreEqual(10, bytesRead);
        Assert.AreEqual(
          new byte[] { 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0 }, buffer
        );
      }
    }

    /// <summary>Verifies that the Write() method is working</summary>
    [Test]
    public void TestWriteToPartialStream() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(123);

        memoryStream.Position = 60;
        memoryStream.Write(new byte[] { 11, 12, 13, 14, 15 }, 0, 5);

        PartialStream partialStream = new PartialStream(memoryStream, 50, 15);
        partialStream.Position = 3;
        partialStream.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 0, 10);

        byte[] buffer = new byte[17];
        memoryStream.Position = 49;
        int bytesRead = memoryStream.Read(buffer, 0, 17);

        Assert.AreEqual(17, bytesRead);
        Assert.AreEqual(
          new byte[] { 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 14, 15, 0 },
          buffer
        );
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if the Write() method of the partial stream
    ///   is attempted to be used to extend the partial stream's length
    /// </summary>
    [Test]
    public void TestThrowOnExtendPartialStream() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        memoryStream.SetLength(25);

        PartialStream partialStream = new PartialStream(memoryStream, 10, 10);
        partialStream.Position = 5;
        Assert.Throws<NotSupportedException>(
          delegate() { partialStream.Write(new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 6); }
        );
      }
    }

  }

} // namespace Nuclex.Support.IO

#endif // UNITTEST
