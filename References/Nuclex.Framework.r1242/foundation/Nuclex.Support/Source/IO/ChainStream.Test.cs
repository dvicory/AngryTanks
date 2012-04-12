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

  /// <summary>Unit Test for the stream chainer</summary>
  [TestFixture]
  public class ChainStreamTest {

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

    /// <summary>
    ///   Tests whether the stream chainer correctly partitions a long write request
    ///   over its chained streams and appends any remaining data to the end of
    ///   the last chained stream.
    /// </summary>
    [Test]
    public void TestPartitionedWrite() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();

      byte[] testData = new byte[20];
      for(int index = 0; index < testData.Length; ++index) {
        testData[index] = (byte)(index + 1);
      }

      chainer.Position = 5;
      chainer.Write(testData, 0, testData.Length);

      Assert.AreEqual(
        new byte[] { 0, 0, 0, 0, 0, 1, 2, 3, 4, 5 },
        ((MemoryStream)chainer.ChainedStreams[0]).ToArray()
      );
      Assert.AreEqual(
        new byte[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
        ((MemoryStream)chainer.ChainedStreams[1]).ToArray()
      );
    }

    /// <summary>
    ///   Tests whether the stream chainer correctly partitions a long read request
    ///   over its chained streams.
    /// </summary>
    [Test]
    public void TestPartitionedRead() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();

      ((MemoryStream)chainer.ChainedStreams[0]).Write(
        new byte[] { 1, 2, 3, 4, 5 }, 0, 5
      );
      ((MemoryStream)chainer.ChainedStreams[1]).Write(
        new byte[] { 6, 7, 8, 9, 10 }, 0, 5
      );

      chainer.Position = 3;
      byte[] buffer = new byte[15];
      int bytesRead = chainer.Read(buffer, 0, 14);

      Assert.AreEqual(14, bytesRead);
      Assert.AreEqual(new byte[] { 4, 5, 0, 0, 0, 0, 0, 6, 7, 8, 9, 10, 0, 0, 0 }, buffer);
    }

    /// <summary>
    ///   Tests whether the stream chainer can handle a stream resize
    /// </summary>
    [Test]
    public void TestWriteAfterResize() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();

      // The first stream has a size of 10 bytes, so this goes into the second stream
      chainer.Position = 11;
      chainer.Write(new byte[] { 12, 34 }, 0, 2);

      // Now we resize the first stream to 15 bytes, so this goes into the first stream
      ((MemoryStream)chainer.ChainedStreams[0]).SetLength(15);
      chainer.Write(new byte[] { 56, 78, 11, 22 }, 0, 4);

      Assert.AreEqual(
        new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 56, 78 },
        ((MemoryStream)chainer.ChainedStreams[0]).ToArray()
      );
      Assert.AreEqual(
        new byte[] { 11, 22, 34, 0, 0, 0, 0, 0, 0, 0 },
        ((MemoryStream)chainer.ChainedStreams[1]).ToArray()
      );
    }

    /// <summary>
    ///   Tests writing to a stream chainer that contains an unseekable stream
    /// </summary>
    [Test]
    public void TestWriteToUnseekableStream() {
      MemoryStream firstStream = new MemoryStream();

      // Now the second stream _does_ support seeking. If the stream chainer ignores
      // that, it would overwrite data in the second stream.
      MemoryStream secondStream = new MemoryStream();
      secondStream.Write(new byte[] { 0, 9, 8, 7, 6 }, 0, 5);
      secondStream.Position = 0;

      TestStream testStream = new TestStream(firstStream, true, true, false);
      ChainStream chainer = new ChainStream(new Stream[] { testStream, secondStream });

      chainer.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
      Assert.IsFalse(chainer.CanSeek);
      Assert.AreEqual(0, firstStream.Length);
      Assert.AreEqual(new byte[] { 0, 9, 8, 7, 6, 1, 2, 3, 4, 5 }, secondStream.ToArray());
    }

    /// <summary>
    ///   Tests reading from a stream chainer that contains an unseekable stream
    /// </summary>
    [Test]
    public void TestReadFromUnseekableStream() {
      MemoryStream firstStream = new MemoryStream();

      // Now the second stream _does_ support seeking. If the stream chainer ignores
      // that, it would overwrite data in the second stream.
      MemoryStream secondStream = new MemoryStream();
      secondStream.Write(new byte[] { 0, 9, 8, 7, 6 }, 0, 5);
      secondStream.Position = 3;

      TestStream testStream = new TestStream(firstStream, true, true, false);
      ChainStream chainer = new ChainStream(new Stream[] { testStream, secondStream });

      Assert.IsFalse(chainer.CanSeek);

      byte[] buffer = new byte[5];
      int readByteCount = chainer.Read(buffer, 0, 3);

      Assert.AreEqual(3, readByteCount);
      Assert.AreEqual(new byte[] { 0, 9, 8, 0, 0 }, buffer);

      readByteCount = chainer.Read(buffer, 0, 3);

      Assert.AreEqual(2, readByteCount);
      Assert.AreEqual(new byte[] { 7, 6, 8, 0, 0 }, buffer);
    }

    /// <summary>
    ///   Tests reading from a stream chainer that contains an unreadable stream
    /// </summary>
    [Test]
    public void TestThrowOnReadFromUnreadableStream() {
      MemoryStream memoryStream = new MemoryStream();
      TestStream testStream = new TestStream(memoryStream, false, true, true);
      ChainStream chainer = new ChainStream(new Stream[] { testStream });
      Assert.Throws<NotSupportedException>(
        delegate() { chainer.Read(new byte[5], 0, 5); }
      );
    }

    /// <summary>
    ///   Tests writing to a stream chainer that contains an unwriteable stream
    /// </summary>
    [Test]
    public void TestThrowOnWriteToUnwriteableStream() {
      MemoryStream memoryStream = new MemoryStream();
      TestStream testStream = new TestStream(memoryStream, true, false, true);
      ChainStream chainer = new ChainStream(new Stream[] { testStream });
      Assert.Throws<NotSupportedException>(
        delegate() { chainer.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5); }
      );
    }

    /// <summary>
    ///   Verifies that the stream chainer throws an exception if the attempt is
    ///   made to change the length of the stream
    /// </summary>
    [Test]
    public void TestThrowOnLengthChange() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();
      Assert.Throws<NotSupportedException>(
        delegate() { chainer.SetLength(123); }
      );
    }

    /// <summary>
    ///   Verifies that the CanRead property is correctly determined by the stream chainer
    /// </summary>
    [Test]
    public void TestCanRead() {
      MemoryStream yesStream = new MemoryStream();
      TestStream noStream = new TestStream(yesStream, false, true, true);

      Stream[] yesGroup = new Stream[] { yesStream, yesStream, yesStream, yesStream };
      Stream[] partialGroup = new Stream[] { yesStream, yesStream, noStream, yesStream };
      Stream[] noGroup = new Stream[] { noStream, noStream, noStream, noStream };

      Assert.IsTrue(new ChainStream(yesGroup).CanRead);
      Assert.IsFalse(new ChainStream(partialGroup).CanRead);
      Assert.IsFalse(new ChainStream(noGroup).CanRead);
    }

    /// <summary>
    ///   Verifies that the CanRead property is correctly determined by the stream chainer
    /// </summary>
    [Test]
    public void TestCanWrite() {
      MemoryStream yesStream = new MemoryStream();
      TestStream noStream = new TestStream(yesStream, true, false, true);

      Stream[] yesGroup = new Stream[] { yesStream, yesStream, yesStream, yesStream };
      Stream[] partialGroup = new Stream[] { yesStream, yesStream, noStream, yesStream };
      Stream[] noGroup = new Stream[] { noStream, noStream, noStream, noStream };

      Assert.IsTrue(new ChainStream(yesGroup).CanWrite);
      Assert.IsFalse(new ChainStream(partialGroup).CanWrite);
      Assert.IsFalse(new ChainStream(noGroup).CanWrite);
    }

    /// <summary>
    ///   Verifies that the CanSeek property is correctly determined by the stream chainer
    /// </summary>
    [Test]
    public void TestCanSeek() {
      MemoryStream yesStream = new MemoryStream();
      TestStream noStream = new TestStream(yesStream, true, true, false);

      Stream[] yesGroup = new Stream[] { yesStream, yesStream, yesStream, yesStream };
      Stream[] partialGroup = new Stream[] { yesStream, yesStream, noStream, yesStream };
      Stream[] noGroup = new Stream[] { noStream, noStream, noStream, noStream };

      Assert.IsTrue(new ChainStream(yesGroup).CanSeek);
      Assert.IsFalse(new ChainStream(partialGroup).CanSeek);
      Assert.IsFalse(new ChainStream(noGroup).CanSeek);
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the Seek() method is called on
    ///   a stream chainer with streams that do not support seeking
    /// </summary>
    [Test]
    public void TestThrowOnSeekWithUnseekableStream() {
      MemoryStream memoryStream = new MemoryStream();
      TestStream testStream = new TestStream(memoryStream, true, true, false);

      ChainStream chainer = new ChainStream(new Stream[] { testStream });
      Assert.Throws<NotSupportedException>(
        delegate() { chainer.Seek(123, SeekOrigin.Begin); }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the Position property is retrieved
    ///   on a stream chainer with streams that do not support seeking
    /// </summary>
    [Test]
    public void TestThrowOnGetPositionWithUnseekableStream() {
      MemoryStream memoryStream = new MemoryStream();
      TestStream testStream = new TestStream(memoryStream, true, true, false);

      ChainStream chainer = new ChainStream(new Stream[] { testStream });
      Assert.Throws<NotSupportedException>(
        delegate() { Console.WriteLine(chainer.Position); }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the Position property is set
    ///   on a stream chainer with streams that do not support seeking
    /// </summary>
    [Test]
    public void TestThrowOnSetPositionWithUnseekableStream() {
      MemoryStream memoryStream = new MemoryStream();
      TestStream testStream = new TestStream(memoryStream, true, true, false);

      ChainStream chainer = new ChainStream(new Stream[] { testStream });
      Assert.Throws<NotSupportedException>(
        delegate() { chainer.Position = 123; }
      );
    }

    /// <summary>
    ///   Tests whether an exception is thrown if the Length property is retrieved
    ///   on a stream chainer with streams that do not support seeking
    /// </summary>
    [Test]
    public void TestThrowOnGetLengthWithUnseekableStream() {
      MemoryStream memoryStream = new MemoryStream();
      TestStream testStream = new TestStream(memoryStream, true, true, false);

      ChainStream chainer = new ChainStream(new Stream[] { testStream });
      Assert.Throws<NotSupportedException>(
        delegate() { Assert.IsTrue(chainer.Length != chainer.Length); }
      );
    }

    /// <summary>
    ///   Tests whether the Seek() method of the stream chainer is working
    /// </summary>
    [Test]
    public void TestSeeking() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();

      Assert.AreEqual(7, chainer.Seek(-13, SeekOrigin.End));
      Assert.AreEqual(14, chainer.Seek(7, SeekOrigin.Current));
      Assert.AreEqual(11, chainer.Seek(11, SeekOrigin.Begin));
    }

    /// <summary>
    ///   Tests whether the stream behaves correctly if data is read from beyond its end
    /// </summary>
    [Test]
    public void TestReadBeyondEndOfStream() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();
      chainer.Seek(10, SeekOrigin.End);

      // This is how the MemoryStream behaves: it returns 0 bytes.
      int readByteCount = chainer.Read(new byte[1], 0, 1);
      Assert.AreEqual(0, readByteCount);
    }

    /// <summary>
    ///   Tests whether the Seek() method throws an exception if an invalid
    ///   reference point is provided
    /// </summary>
    [Test]
    public void TestThrowOnInvalidSeekReferencePoint() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();
      Assert.Throws<ArgumentException>(
        delegate() { chainer.Seek(1, (SeekOrigin)12345); }
      );
    }

    /// <summary>Verifies that the position property works correctly</summary>
    [Test]
    public void TestPositionChange() {
      ChainStream chainer = chainTwoStreamsOfTenBytes();

      chainer.Position = 7;
      Assert.AreEqual(chainer.Position, 7);
      chainer.Position = 14;
      Assert.AreEqual(chainer.Position, 14);
    }

    /// <summary>Tests the Flush() method of the stream chainer</summary>
    [Test]
    public void TestFlush() {
      MemoryStream firstStream = new MemoryStream();
      TestStream firstTestStream = new TestStream(firstStream, true, true, true);

      MemoryStream secondStream = new MemoryStream();
      TestStream secondTestStream = new TestStream(secondStream, true, true, true);

      ChainStream chainer = new ChainStream(
        new Stream[] { firstTestStream, secondTestStream }
      );

      Assert.AreEqual(0, firstTestStream.FlushCallCount);
      Assert.AreEqual(0, secondTestStream.FlushCallCount);

      chainer.Flush();

      Assert.AreEqual(1, firstTestStream.FlushCallCount);
      Assert.AreEqual(1, secondTestStream.FlushCallCount);
    }

    /// <summary>
    ///   Creates a stream chainer with two streams that each have a size of 10 bytes
    /// </summary>
    /// <returns>The new stream chainer with two chained 10-byte streams</returns>
    private static ChainStream chainTwoStreamsOfTenBytes() {
      MemoryStream firstStream = new MemoryStream(10);
      MemoryStream secondStream = new MemoryStream(10);

      firstStream.SetLength(10);
      secondStream.SetLength(10);

      return new ChainStream(
        new Stream[] { firstStream, secondStream }
      );
    }

  }

} // namespace Nuclex.Support.IO

#endif // UNITTEST
