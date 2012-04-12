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

using Nuclex.Support;

namespace Nuclex.Networking.Http {

  /// <summary>Parses HTTP/1.1 requests</summary>
  /// <remarks>
  ///   <para>
  ///     This is a high-performance and low-garbage HTTP request parser that can be
  ///     fed with incoming data incrementally. The parser is designed to be bullet-proof
  ///     and will not run into an undefined state no matter what data it is given. It
  ///     will not look at a single byte more than the maximum allowed request header
  ///     size you specify, thereby also making it resilient to memory load attacks.
  ///   </para>
  ///   <para>
  ///     Parsing takes place directly on the data you provide the parser with, avoiding
  ///     expensive memory copies and string conversion/splitting operations. This makes
  ///     it ideal for usage in systems with limited memory or a cheaper implementation
  ///     of the .NET garbage collection and ensures good scalability on other systems.
  ///   </para>
  /// </remarks>
  internal class RequestParser : HttpLineParser {

    #region enum ParserState

    /// <summary>States the request parser can be in</summary>
    private enum ParserState {
      /// <summary>Waiting for the request line or a CR-LF to be sent</summary>
      AwaitingRequestLineOrCrLf,
      /// <summary>Waiting for the request line to be sent</summary>
      AwaitingRequestLine,
      /// <summary>Waiting for additional headers to be sent</summary>
      AwaitingRequestHeaderOrEnd,
      /// <summary></summary>
      AwaitingRequestData
    }

    #endregion // enum ParserState

    /// <summary>Initializes a new HTTP request parser</summary>
    /// <param name="maximumRequestHeaderSize">
    ///   Maximum length the HTTP request header is allowed to have
    /// </param>
    public RequestParser(int maximumRequestHeaderSize) :
      base(maximumRequestHeaderSize) {
      this.requestBuilder = new RequestBuilder();

      Reset();
    }

    /// <summary>Resets the parser to the initial state for a new request</summary>
    public new void Reset() {
      this.state = ParserState.AwaitingRequestLineOrCrLf;
      this.requestBuilder.Reset();
      base.Reset();
    }

    /// <summary>Instructs the parser to process the provided bytes</summary>
    /// <param name="buffer">Buffer containing the bytes that will be parsed</param>
    /// <param name="start">Index in the buffer at which to start reading</param>
    /// <param name="count">Number of bytes that will be parsed</param>
    /// <return>
    ///   The parsed HTTP request if a complete request could be constructed from the
    ///   provided bytes, null if more data is required
    /// </return>
    /// <remarks>
    ///   <para>
    ///     When a request is returned, there may be leftover data in the buffer that was
    ///     following the request. This data can either be another request sent by the
    ///     client or uploaded data for an HTTP POST request.
    ///   </para>
    ///   <para>
    ///     You should always extract the leftover data as soon as a request is returned
    ///     using the <see cref="GetRemainingData" /> method and buffer it until you know
    ///     (from the request header) what is to be done with it.If the request doesn't
    ///     involve any uploaded data, you can feed it back to the request parser after
    ///     calling the <see cref="Reset" /> method once (or creating a new parser).
    ///   </para>
    /// </remarks>
    public Request ProcessBytes(byte[] buffer, int start, int count) {

      // If the request is complete, assume all incoming data is additional data
      // appended to the request (eg. HTTP POST)
      if(this.state == ParserState.AwaitingRequestData) {
        return null;
      }

      // Hand the received data over to the underlying line parser
      SetReceivedData(buffer, start, count);

      for(; ; ) {

        // Let the line parser extract the next line from the data
        string line = base.ParseLine();

        // If we need more data to complete the current line, exit the loop and
        // wait for ProcessBytes() be called again
        if(line == null) {
          return null;
        }

        switch(this.state) {

          // We're awaiting either the request line of an empty line. The RFC says that
          // server should handle a single CR-LF being sent before the request line.
          case ParserState.AwaitingRequestLineOrCrLf: {
            if(line == string.Empty) {
              this.state = ParserState.AwaitingRequestLine;
              break;
            } else {
              goto case ParserState.AwaitingRequestLine;
            }
          }

          // we either received an empty line already or the first line sent to us was
          // non-empty, so now expect the client to follow with the request line.
          case ParserState.AwaitingRequestLine: {
            parseRequestLine(line);
            this.currentFieldName = null;
            this.state = ParserState.AwaitingRequestHeaderOrEnd;
            break;
          }

          // Request line has been received and we're now waiting for additional request
          // header lines or the end of the request header
          case ParserState.AwaitingRequestHeaderOrEnd: {
            if(line == string.Empty) {
              this.state = ParserState.AwaitingRequestData;
              return this.requestBuilder.BuildRequest();
            }
            parseHeaderLine(line);
            break;
          }

          // Honestly, this should not occur :)
          default: {
            throw new Exception("Internal error: invalid parser state");
          }

        }

      } // for(;;)

    }

    /// <summary>The remaining bytes the parser has not yet processed</summary>
    /// <remarks>
    ///   After a request is complete, normally, all bytes in the receive buffer should
    ///   have been processed. If the request was a HTTP POST request, however, the client
    ///   might begin sending the data immediately after the request header. In this case,
    ///   you will have to take back the remaining, unparsed bytes from the parser
    ///   after the complete request has been parsed.
    /// </remarks>
    public new ArraySegment<byte> GetRemainingData() {
      return base.GetRemainingData();
    }

    /// <summary>Parses the request line sent from the client</summary>
    /// <param name="requestLine">String containing the received request line</param>
    private void parseRequestLine(string requestLine) {

      // The RFC doesn't say that the request line must not contain any additional
      // spaces, so in the we will assume the first space terminates the method and the
      // last space terminates the URI.
      int uriDelimiterIndex = requestLine.IndexOf(' ');
      if(uriDelimiterIndex == -1) {
        throw Errors.BadRequest("Request-line is missing an URI");
      }

      // If there's only one space character, then the request is missing the version
      // of the HTTP protocol used.
      int versionDelimiterIndex = requestLine.LastIndexOf(' ');
      if(versionDelimiterIndex == uriDelimiterIndex) {
        throw Errors.BadRequest("Request-line does not specify HTTP version");
      }

      // Request seems to be at least be in the right layout. Extract the individual
      // components and pass them to the request container builder (validation of
      // the actual settings takes place once we have a complete request).
      requestBuilder.Method = requestLine.Substring(0, uriDelimiterIndex);
      requestBuilder.Uri = requestLine.Substring(
        uriDelimiterIndex + 1, versionDelimiterIndex - uriDelimiterIndex - 1
      );
      requestBuilder.Version = requestLine.Substring(versionDelimiterIndex + 1);

      // We expect HTTP/1.* to stay compatible with the general format of the request.
      // Any other version of the protocol may include major changes to the request
      // format, thus we only accept HTTP/1.*.
      if(!requestBuilder.Version.StartsWith("HTTP/1.")) {
        throw Errors.UnsupportedProtocolVersion();
      }

    }

    /// <summary>Parses a request header line sent from the client</summary>
    /// <param name="headerLine">String containing the received header line</param>
    private void parseHeaderLine(string headerLine) {

      // Find out whether this header line begins with whitespace. According to the
      // RFC, a message header can be broken into multiple lines by beginning the
      // next line with one or more whitespace characters (SP and HT)
      char firstCharacter = headerLine[0];
      bool startsWithWhitespace =
        (firstCharacter == ' ') ||
        (firstCharacter == '\t');

      // If the line starts with a whitespace, it is either a continuation of the
      // previous line or simply a broken request (or there is no previous line)
      if(startsWithWhitespace) {

        // If this is the first header field, the request is broken
        if(this.currentFieldName == null) {
          throw Errors.BadRequest("First message header is preceded by whitespace");
        }

        // Alright, this actually seems to be a valid field continuation
        parseHeaderFieldValue(headerLine, 1);

      } else { // Line doesn't begin with a whitespace 

        // Look for the delimiter character that ends the field name
        int valueDelimiterIndex = headerLine.IndexOf(':');
        if(valueDelimiterIndex == -1) { // No delimiter? Invalid request!
          throw Errors.BadRequest("Message header field omits value");
        }

        // Extract the field name from the line
        string fieldName = headerLine.Substring(0, valueDelimiterIndex);
        if(fieldName == string.Empty) { // Empty field name? Request broken!
          throw Errors.BadRequest("Message header contains unnamed field");
        }

        // There is no mention in the RFC that whitespace is allowed between the
        // header field name and the delimiter character, so we don't allow it.
        bool fieldNameEndsInWhitespace =
          (fieldName[fieldName.Length - 1] == ' ') ||
          (fieldName[fieldName.Length - 1] == '\t');

        if(fieldNameEndsInWhitespace) {
          throw Errors.BadRequest(
            "Message header field name is followed by whitespace"
          );
        }

        // Now that we know where the value begins, parse it!
        this.currentFieldName = fieldName;
        parseHeaderFieldValue(headerLine, valueDelimiterIndex + 1);

      }

    }

    /// <summary>Parses the field value of an HTTP header field</summary>
    /// <param name="headerLine">Line containing the field value</param>
    /// <param name="valueIndex">Index at which the field value begins</param>
    private void parseHeaderFieldValue(string headerLine, int valueIndex) {

      // Look for where the value starts (skip any whitespace)
      int firstNonWhitespaceIndex = StringHelper.IndexNotOfAny(
        headerLine, httpWhitespaces, valueIndex
      );

      // If there was no value (or the value consisted entirely of whitespace), we
      // add the header field as a field without value. A value might still follow
      // in the next line, and would be added then, but the RFC allows header fields
      // with only a name, so we have to add it now in case it is such a field.
      if(firstNonWhitespaceIndex == -1) {
        this.requestBuilder.AddHeader(this.currentFieldName);
        return;
      }

      // We scan the source string for the last non-whitespace character instead
      // of trimming the string because that would be less efficient for .NET's
      // immutable strings. We know that we'll find something since the forward
      // scan above has returned a valid index.
      int lastNonWhitespaceIndex = StringHelper.LastIndexNotOfAny(
        headerLine, httpWhitespaces
      );
      this.requestBuilder.AddHeader(
        this.currentFieldName,
        headerLine.Substring(
          firstNonWhitespaceIndex,
          lastNonWhitespaceIndex - firstNonWhitespaceIndex + 1
        )
      );

    }

    /// <summary>Characters considered as whitespace in the HTTP protocol</summary>
    private static readonly char[] httpWhitespaces = new char[] { ' ', '\t' };
    /// <summary>Collects data and constructs HTTP/1.1 request containers</summary>
    private RequestBuilder requestBuilder;
    /// <summary>Current state the parser is in</summary>
    private ParserState state;
    /// <summary>Field name of the last request header we parsed</summary>
    private string currentFieldName;

  }

} // namespace Nuclex.Networking.Http
