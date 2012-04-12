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

using System;
using System.Collections.Generic;
using System.Text;

using Nuclex.Support;

namespace Nuclex.Networking.Http {

  /// <summary>Parses lines in HTTP/1.1 request headers</summary>
  /// <remarks>
  ///   <para>
  ///     In order to keep the parsing process clean and readable, the HTTP request parsing
  ///     has been split into two parts. This is the low level part, a simple line parser
  ///     that efficiently extracts individual lines from an HTTP request while honoring
  ///     the guidelines from the RFC describing the HTTP protocol. The parser is designed
  ///     to be used when implementing more complex parsers and has a specialized interface
  ///     intended for the parser writers.
  ///   </para>
  ///   <para>
  ///     The correct usage is to feed it a chunk of data (with arbitrary length) using the
  ///     <see cref="LineParser.SetReceivedData" /> method and then let it chop the chunk
  ///     down into lines by calling the <see cref="LineParser.ParseLine" /> method
  ///     repeatedly until it returns null (meaning it requires more data to continue) or
  ///     throws an exception.
  ///   </para>
  ///   <para>
  ///     You should not try to continue parsing once the <see cref="LineParser.ParseLine" />
  ///     method has thrown an exception. Providing the parser with new data is also only
  ///     allowed when it has explicitely asked for more data by returning null from the
  ///     <see cref="LineParser.ParseLine" /> method. Calling
  ///     <see cref="LineParser.SetReceivedData" /> any earlier will result in
  ///     the unprocessed data in the receive buffer to not be seen by the parser.
  ///   </para>
  ///   <para>
  ///     This is so because the parser works directly on your buffer. In order to achieve
  ///     maximum efficiency, it will not copy the into a local buffer unless required to
  ///     when it has to bridge lines that are split between two chunks provided by the
  ///     <see cref="LineParser.SetReceivedData" /> method.
  ///   </para>
  /// </remarks>
  public class HttpLineParser : LineParser {

    // These constants use abbreviations to match the RFC descriptions.

    /// <summary>ASCII code for the space character</summary>
    private const byte SP = 32;
    /// <summary>ASCII code for the horizontal tab character</summary>
    private const byte HT = 9;
    /// <summary>ASCII code for the delete character</summary>
    private const byte DEL = 127;

    /// <summary>ID of the ISO-8859-1 code page</summary>
    private const int ISO_8859_1 = 28591;

    /// <summary>Initializes a new HTTP/1.1 request parser</summary>
    public HttpLineParser() : base() { }

    /// <summary>Initializes a new HTTP/1.1 request parser</summary>
    /// <param name="maximumRequestHeaderSize">
    ///   Maximum size the request header is allowed to have
    /// </param>
    public HttpLineParser(int maximumRequestHeaderSize) :
      base(maximumRequestHeaderSize) {
      this.maximumRequestHeaderSize = maximumRequestHeaderSize;
    }

    /// <summary>
    ///   Called when the message is growing beyond the maximum message size
    /// </summary>
    /// <returns>
    ///   An exception that will be thrown to indicate the too large message
    /// </returns>
    protected override Exception HandleMessageTooLarge() {
      return Errors.RequestEntityTooLarge(this.maximumRequestHeaderSize);
    }

    /// <summary>
    ///   Called when the message contains a carriage return without a line feed
    /// </summary>
    protected override void HandleLoneCarriageReturn() {
      throw Errors.BadRequest("Invalid character in request header");
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

      // Make sure the line does not contain any characters which are considered
      // invalid by the RFC
      for(int index = start; index < count; ++index) {

        // First, find out whether this is a control character. All but 2 control
        // characters are disallowed by the RFC
        bool isControlCharacter =
          (buffer[index] < 32) ||
          (buffer[index] == DEL);

        // If it Is a control character, we need to do another check to see whether
        // the characters is one of the two allowed control characters
        if(isControlCharacter) {
          bool isValidControlCharacter =
            (buffer[index] == SP) ||
            (buffer[index] == HT);

          // It's not one of the two allowed control characters, let's complain
          if(!isValidControlCharacter) {
            throw Errors.BadRequest("Invalid character in request header");
          }
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
      return Encoding.GetEncoding(ISO_8859_1).GetString(buffer, start, count);
    }

    /// <summary>Maximum size the request header is allowed to have</summary>
    private int maximumRequestHeaderSize;

  }

} // namespace Nuclex.Networking.Http
