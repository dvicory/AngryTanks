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
using System.Text;

using NUnit.Framework;

namespace Nuclex.Networking {

  /// <summary>Unit tests for the line parser</summary>
  [TestFixture]
  public class LineParserTest {

    #region class TestParser

    /// <summary>Dummy parser for testing the line parser</summary>
    private class TestParser : LineParser {

      /// <summary>Initializes a new test parser</summary>
      public TestParser() : base() { }

      /// <summary>Initializes a new test parser</summary>
      /// <param name="maximumMessageSize">
      ///   Maximum size the message is allowed to have
      /// </param>
      public TestParser(int maximumMessageSize) : base(maximumMessageSize) { }

      /// <summary>Adds bytes to be parsed</summary>
      /// <param name="bytes">Array containing the bytes to be parsed</param>
      /// <param name="start">Index at which to begin reading the array</param>
      /// <param name="count">Number of bytes to take from the array</param>
      /// <returns>True if more data is required</returns>
      public void ProcessBytes(byte[] bytes, int start, int count) {
        SetReceivedData(bytes, start, count);

        for(; ; ) {
          string line = ParseLine();
          if(line == null) {
            break;
          }

          ++parsedLineCount;
        }
      }

      /// <summary>Number of lines the parser has parsed so far</summary>
      public int ParsedLineCount {
        get { return this.parsedLineCount; }
      }

      /// <summary>
      ///   Called when the message is growing beyond the maximum message size
      /// </summary>
      /// <returns>
      ///   An exception that will be thrown to indicate the too large message
      /// </returns>
      protected override Exception HandleMessageTooLarge() {
        return new InsufficientMemoryException("Message too large");
      }

      /// <summary>
      ///   Called when the message contains a carriage return without a line feed
      /// </summary>
      protected override void HandleLoneCarriageReturn() {
        throw new FormatException("Lone carriage return encountered");
      }

      /// <summary>
      ///   Called to scan the bytes of a potential line for invalid characters
      /// </summary>
      /// <param name="buffer">
      ///   Array containing the bytes that to can for invalid characters
      /// </param>
      /// <param name="start">Index in the array at which to begin reading</param>
      /// <param name="count">Number of bytes from the array to scan</param>
      protected override void VerifyPotentialLine(byte[] buffer, int start, int count) {
        for(int index = start; index < start + count; ++index) {
          if(buffer[index] >= 128) {
            throw new FormatException("Invalid character encountered");
          }
        }
      }

      /// <summary>
      ///   Called to transform a received series of bytes into a string
      /// </summary>
      /// <param name="buffer">Buffer containing the bytes to be transformed</param>
      /// <param name="start">Index of the first byte to transform</param>
      /// <param name="count">Number of bytes to transform into a string</param>
      /// <returns>The string produced from the bytes in the specified buffer</returns>
      /// <remarks>
      ///   This method allows you to use your own encoding for transforming the bytes
      ///   in a line into a string. Always called to transform an entire line in one
      ///   piece, excluding the CR LF characters at the line's end.
      /// </remarks>
      protected override string TransformToString(byte[] buffer, int start, int count) {
        return Encoding.ASCII.GetString(buffer, start, count);
      }

      /// <summary>Number of lines that line parser has parsed</summary>
      private int parsedLineCount;

    }

    #endregion // class TestParser

    /// <summary>Verifies that the default constructor of the line parser works</summary>
    [Test]
    public void TestDefaultConstructor() {
      new TestParser();
    }

    /// <summary>
    ///   Tests whether the line parser works on a single line
    /// </summary>
    [Test]
    public void TestSingleLineParsing() {
      TestParser parser = new TestParser(128);

      byte[] requestData = Encoding.ASCII.GetBytes("This is a boring test string\r\n");
      parser.ProcessBytes(requestData, 0, requestData.Length);

      Assert.AreEqual(1, parser.ParsedLineCount);
    }

    /// <summary>
    ///   Tests whether the line parser can cope with an incomplete line
    /// </summary>
    [Test]
    public void TestIncompleteLineParsing() {
      TestParser parser = new TestParser(128);

      byte[] requestData = Encoding.ASCII.GetBytes("This is a boring test string");
      parser.ProcessBytes(requestData, 0, requestData.Length);

      Assert.AreEqual(0, parser.ParsedLineCount);
    }

    /// <summary>
    ///   Tests whether the GetRemainingData() method returns the still unparsed data
    ///   correctly
    /// </summary>
    [Test]
    public void TestRemainingDataRetrieval() {
      byte[] binaryData = new byte[9] {
        0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88
      };

      byte[] requestData = Encoding.ASCII.GetBytes("This is a test\r\n123456789");
      for(int index = 0; index < 9; ++index) {
        requestData[index + 16] = binaryData[index];
      }

      TestParser parser = new TestParser(128);
      parser.SetReceivedData(requestData, 0, requestData.Length);

      Assert.AreEqual("This is a test", parser.ParseLine());
      Assert.IsTrue(
        arraySegmentContentsAreEqual(
          new ArraySegment<byte>(binaryData, 0, 9),
          parser.GetRemainingData()
        )
      );
    }

    /// <summary>
    ///   Verifies that the Reset() fully resets the parser 
    /// </summary>
    [Test]
    public void TestReset() {
      byte[] beginning = Encoding.ASCII.GetBytes("This is a test\r");
      byte[] ending = Encoding.ASCII.GetBytes("\nA real test\r\n");

      TestParser parser = new TestParser(128);
      parser.SetReceivedData(beginning, 0, beginning.Length);

      Assert.IsNull(parser.ParseLine());
      parser.Reset();

      // If the Reset() didn't work, the parser would now complain that it discovered
      // a lone carriage return inmidst of the data stream
      parser.SetReceivedData(beginning, 0, beginning.Length);
      Assert.IsNull(parser.ParseLine());

      parser.SetReceivedData(ending, 0, ending.Length);
      Assert.AreEqual("This is a test", parser.ParseLine());
      Assert.AreEqual("A real test", parser.ParseLine());
      Assert.IsNull(parser.ParseLine());
    }

    /// <summary>
    ///   Tests whether a lone carriage return at the end of a split buffer is
    ///   detected correctly.
    /// </summary>
    [Test]
    public void TestLoneCarriageReturnInSplitStream() {
      byte[] badData = Encoding.ASCII.GetBytes("This is a test\r");

      TestParser parser = new TestParser(128);

      // Parse a chunk of data that leaves an open-ended carriage return. No exception
      // should be thrown here because the next chunk the parser get might start
      // with a line feed, thus forming a valid line.
      try {
        parser.SetReceivedData(badData, 0, badData.Length);
        Assert.IsNull(parser.ParseLine());
      }
      catch(FormatException) {
        Assert.Fail("Line parser complained about what could yet become a valid line");
      }

      // Parse the same thing again. Now the parser should notice that the carriage
      // return wasn't followed by a line feed and legitimately complain.
      parser.SetReceivedData(badData, 0, badData.Length);
      Assert.Throws<FormatException>(
        delegate() { parser.ParseLine(); }
      );
    }

    /// <summary>
    ///   Tests parsing of a message that is just by one byte larger than the maximum
    ///   allowed message size
    /// </summary>
    [Test]
    public void TestSlightlyTooLargeMessage() {
      byte[] slightlyTooLargeData = Encoding.ASCII.GetBytes(
        new string(' ', 63) + "\r\n"
      );

      TestParser parser = new TestParser(64);
      parser.SetReceivedData(slightlyTooLargeData, 0, slightlyTooLargeData.Length);
      Assert.Throws<InsufficientMemoryException>(
        delegate() { Console.WriteLine(parser.ParseLine()); }
      );
    }

    /// <summary>
    ///   Tests parsing of a message that is just by one byte larger than the maximum
    ///   allowed message size, split into multiple lines
    /// </summary>
    [Test]
    public void TestSlightlyTooLargeSplitMessage() {
      byte[] slightlyTooLargeData = Encoding.ASCII.GetBytes(
        new string(' ', 30) + "\r\n" +
        new string(' ', 30) + "\r\n" +
        new string(' ', 31) + "\r\n"
      );

      TestParser parser = new TestParser(96);
      parser.SetReceivedData(slightlyTooLargeData, 0, slightlyTooLargeData.Length);

      Assert.AreEqual(new string(' ', 30), parser.ParseLine());
      Assert.AreEqual(new string(' ', 30), parser.ParseLine());
      Assert.Throws<InsufficientMemoryException>(
        delegate() { Console.WriteLine(parser.ParseLine()); }
      );
    }

    /// <summary>
    ///   Tests parsing of a message that is way too large to fit in the receive buffer
    /// </summary>
    [Test]
    public void TestFarTooLargeSplitMessage() {
      byte[] farTooLargeData = Encoding.ASCII.GetBytes(new string(' ', 128));

      TestParser parser = new TestParser(64);
      parser.SetReceivedData(farTooLargeData, 0, farTooLargeData.Length);

      Assert.Throws<InsufficientMemoryException>(
        delegate() { Console.WriteLine(parser.ParseLine()); }
      );
    }

    /// <summary>
    ///   Tests parsing of a message that barely fits in the receive buffer
    /// </summary>
    [Test]
    public void TestBarelyFittingMessage() {
      byte[] barelyFittingData = Encoding.ASCII.GetBytes(
        new string(' ', 62) + "\r\n"
      );

      TestParser parser = new TestParser(64);
      parser.SetReceivedData(barelyFittingData, 0, barelyFittingData.Length);
      Assert.AreEqual(new string(' ', 62), parser.ParseLine());
      Assert.IsNull(parser.ParseLine());
    }

    /// <summary>
    ///   Tests parsing of a message that contains a lone carriage return character
    /// </summary>
    [Test]
    public void TestLoneCarriageReturn() {
      byte[] badData = Encoding.ASCII.GetBytes("First line\r\nThis is a \r test");

      TestParser parser = new TestParser(64);
      parser.SetReceivedData(badData, 0, badData.Length);

      Assert.AreEqual("First line", parser.ParseLine());
      Assert.Throws<FormatException>(
        delegate() { parser.ParseLine(); }
      );
    }

    /// <summary>
    ///   Tests parsing of a line split into multiple messages
    /// </summary>
    [Test]
    public void TestSplitLine() {
      byte[] firstPart = Encoding.ASCII.GetBytes("This ");
      byte[] secondPart = Encoding.ASCII.GetBytes("is a ");
      byte[] thirdPart = Encoding.ASCII.GetBytes("test\r\n");

      TestParser parser = new TestParser(64);

      parser.SetReceivedData(firstPart, 0, firstPart.Length);
      Assert.IsNull(parser.ParseLine());

      parser.SetReceivedData(secondPart, 0, secondPart.Length);
      Assert.IsNull(parser.ParseLine());

      parser.SetReceivedData(thirdPart, 0, thirdPart.Length);
      Assert.AreEqual("This is a test", parser.ParseLine());
      Assert.IsNull(parser.ParseLine());
    }

    /// <summary>
    ///   Tests parsing of a message that requires the store buffer to be enlarged
    /// </summary>
    [Test]
    public void TestLargeMessage() {
      byte[] firstPart = Encoding.ASCII.GetBytes(new string(' ', 128));
      byte[] secondPart = Encoding.ASCII.GetBytes("This is a test\r\n");

      TestParser parser = new TestParser(192);

      parser.SetReceivedData(firstPart, 0, firstPart.Length);
      Assert.IsNull(parser.ParseLine());

      parser.SetReceivedData(secondPart, 0, secondPart.Length);
      Assert.AreEqual(new string(' ', 128) + "This is a test", parser.ParseLine());
      Assert.IsNull(parser.ParseLine());
    }

    /// <summary>Compares two array segments based on their contents</summary>
    /// <param name="firstSegment">First array segment that will be compared</param>
    /// <param name="secondSegment">Second array segment that will be compared</param>
    /// <returns>
    ///   True if the contents in both array segments are identical, otherwise false
    /// </returns>
    private static bool arraySegmentContentsAreEqual(
      ArraySegment<byte> firstSegment, ArraySegment<byte> secondSegment
    ) {

      // If the lengths to not match, there's no point in doing any comparison
      if(firstSegment.Count != secondSegment.Count) {
        return false;
      }

      // Compare the contents of both array segments byte by byte
      for(int index = 0; index < firstSegment.Count; ++index) {
        byte leftByte = firstSegment.Array[firstSegment.Offset + index];
        byte rightByte = secondSegment.Array[secondSegment.Offset + index];
        if(leftByte != rightByte) {
          return false;
        }
      }

      // No differences found, the contents of both segments must be identical
      return true;

    }

  }

}

#endif // UNITTEST
