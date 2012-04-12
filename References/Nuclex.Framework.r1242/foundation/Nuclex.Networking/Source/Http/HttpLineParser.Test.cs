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

namespace Nuclex.Networking.Http {

  /// <summary>Unit tests for the HTTP request parser</summary>
  [TestFixture]
  public class HttpLineParserTest {

    #region class TestParser

    /// <summary>Dummy parser for testing the HTTP request line parser</summary>
    private class TestParser : HttpLineParser {

      /// <summary>Initializes a new test parser</summary>
      /// <param name="maximumHeaderSize">
      ///   Maximum size the HTTP request header is allowed to have
      /// </param>
      public TestParser(int maximumHeaderSize) : base(maximumHeaderSize) { }

      /// <summary>Adds bytes to be parsed</summary>
      /// <param name="bytes">Array containing the bytes to be parsed</param>
      /// <param name="start">Index at which to begin reading the array</param>
      /// <param name="count">Number of bytes to take from the array</param>
      /// <returns>True if more data is required</returns>
      public void AddBytes(byte[] bytes, int start, int count) {
        SetReceivedData(bytes, start, count);

        for(; ; ) {
          string line = ParseLine();
          if(line == null) {
            break;
          }
        }
      }

    }

    #endregion // class TestParser

    /// <summary>
    ///   Tests whether a buffer overflow in a single adds is handled correctly
    /// </summary>
    [Test]
    public void TestBufferOverflowSingleRun() {
      TestParser parser = new TestParser(128);

      Assert.Throws<Exceptions.RequestEntityTooLargeException>(
        delegate() { parser.AddBytes(createWhiteSpaceArray(129), 0, 129); }
      );
    }

    /// <summary>
    ///   Tests whether a buffer overflow built over multiple adds is handled correctly
    /// </summary>
    [Test]
    public void TestBufferOverflowMultipleRuns() {
      TestParser parser = new TestParser(128);

      parser.AddBytes(createWhiteSpaceArray(100), 0, 100);
      Assert.Throws<Exceptions.RequestEntityTooLargeException>(
        delegate() { parser.AddBytes(createWhiteSpaceArray(29), 0, 29); }
      );
    }

    /// <summary>
    ///   Tests whether a line containing invalid characters is rejected
    /// </summary>
    [Test]
    public void TestInvalidCharactersInRequest() {
      TestParser parser = new TestParser(128);

      byte[] requestData = Encoding.ASCII.GetBytes("GET /something HTTP/1.1\r\n");
      requestData[6] = 10;
      requestData[9] = 13;

      Assert.Throws<Exceptions.BadRequestException>(
        delegate() { parser.AddBytes(requestData, 0, requestData.Length); }
      );
    }

    /// <summary>
    ///   Tests whether a line end is recognized when it is split into two adds
    /// </summary>
    /// <remarks>
    ///   If the line end is not correctly recognized, a BadRequestException will result
    /// </remarks>
    [Test]
    public void TestSplitLineEndRecognition() {
      TestParser parser = new TestParser(128);

      byte[] requestData = Encoding.ASCII.GetBytes(exampleRequest);
      int crIndex = Array.IndexOf<byte>(requestData, 13);
      System.Diagnostics.Trace.Assert(crIndex != -1);

      parser.AddBytes(requestData, 0, crIndex + 1);
      Assert.Throws<Exceptions.RequestEntityTooLargeException>(
        delegate() {
          parser.AddBytes(requestData, crIndex + 1, requestData.Length - crIndex);
        }
      );
    }

    /// <summary>Make sure the parser correctly handles the array start index</summary>
    [Test]
    public void TestExampleRequestWithStartIndex() {
      TestParser parser = new TestParser(512);

      byte[] requestData = Encoding.ASCII.GetBytes(exampleRequest);
      byte[] paddedRequestData = createWhiteSpaceArray(requestData.Length + 512);
      Array.Copy(requestData, 0, paddedRequestData, 512, requestData.Length);

      parser.AddBytes(paddedRequestData, 512, requestData.Length);
    }

    /// <summary>Tests whether the parser can handle a normal HTTP request</summary>
    [Test]
    public void TestExampleRequest() {
      TestParser parser = new TestParser(512);

      byte[] requestData = Encoding.ASCII.GetBytes(exampleRequest);

      parser.AddBytes(requestData, 0, requestData.Length);
    }

    /// <summary>Creates a byte array of whitespace characters</summary>
    /// <param name="length">Number of whitespace characters to put in the array</param>
    /// <returns>The array of whitespace characters</returns>
    public static byte[] createWhiteSpaceArray(int length) {
      return Encoding.ASCII.GetBytes(new string(' ', length));
    }

    /// <summary>An example HTTP/1.1 request</summary>
    private static readonly string exampleRequest =
      "GET / HTTP/1.1\r\n" +
      "Host: localhost\r\n" +
      "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.16)" +
        "Gecko/20080702 Firefox/2.0.0.16\r\n" +
      "Accept: text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;" +
        "q=0.8,image/png,*/*;q=0.5\r\n" +
      "Accept-Language: de-de,de;q=0.8,en-us;q=0.5,en;q=0.3\r\n" +
      "Accept-Encoding: gzip,deflate\r\n" +
      "Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7\r\n" +
      "Keep-Alive: 300\r\n" +
      "Connection: keep-alive\r\n" +
      "Cache-Control: max-age=0\r\n" +
      "\r\n";
  }

} // namespace Nuclex.Networking.Http

#endif // UNITTEST
