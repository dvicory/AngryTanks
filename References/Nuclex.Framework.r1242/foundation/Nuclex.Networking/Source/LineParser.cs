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

namespace Nuclex.Networking {

  /// <summary>Parses lines from binary data</summary>
  /// <remarks>
  ///   <para>
  ///     To use this parser, derive a class from it to set up your own filtering rules
  ///     for invalid characters and text encoding format.
  ///   </para>
  ///   <para>
  ///     Next, create an instance of your new parser and feed it a chunk of data (with
  ///     arbitrary length, either containing the complete message or only a fraction of
  ///     it) using the <see cref="SetReceivedData" /> method. Then let it chop the chunk
  ///     down into lines by calling the <see cref="ParseLine" /> method repeatedly until
  ///     it returns null (meaning it requires more data to continue) or throws
  ///     an exception.
  ///   </para>
  ///   <para>
  ///     If you're implementing a combined text/binary protocol like HTTP you can also,
  ///     at any time, call <see cref="GetRemainingData" /> to obtain any data that has
  ///     not been parsed yet to obtain the beginning of binary data the followed the
  ///     textual header of a request.
  ///   </para>
  ///   <para>
  ///     You can reuse the same parser for multiple requests by calling its
  ///     <see cref="Reset" /> method, which will restore it to the state it was in when
  ///     it had just been created.
  ///   </para>
  ///   <para>
  ///     Your will find several references to "the RFC" in the comments within the code.
  ///     This is because this parser, while generic in purpose, relies on the RFC for
  ///     the HTTP protocol (RFC-2616) for any decisions on how to proceed. One this class'
  ///     design goals is to be usable as the fundament for a HTTP protocol parser.
  ///   </para>
  /// </remarks>
  public abstract class LineParser {

    /// <summary>ASCII code for the carriage return character</summary>
    private const byte CR = 13;
    /// <summary>ASCII code for the line feed character</summary>
    private const byte LF = 10;

    /// <summary>Initializes a new line parser</summary>
    public LineParser() : this(1024) { }

    /// <summary>Initializes a new line parser</summary>
    /// <param name="maximumMessageSize">
    ///   Maximum size the entire message is allowed to have in bytes
    /// </param>
    public LineParser(int maximumMessageSize) {
      this.maximumMessageSize = maximumMessageSize;
      this.storedBytes = new byte[64];
    }

    /// <summary>Assigns a new chunk of received data for parsing</summary>
    /// <param name="bytes">Array containing the bytes that will be parsed</param>
    /// <param name="start">Index in the array at which to begin parsing</param>
    /// <param name="count">Number of bytes to parse</param>
    /// <remarks>
    ///   This method has to be called before the ParseHeaderLine() method can be used.
    /// </remarks>
    public void SetReceivedData(byte[] bytes, int start, int count) {
      this.receivedBytes = bytes;
      this.receivedByteIndex = start;
      this.receivedByteCount = count;
    }

    /// <summary>
    ///   Returns the remaining (still unparsed) data in the received buffer
    /// </summary>
    /// <returns>The remaining data from the receive buffer</returns>
    public ArraySegment<byte> GetRemainingData() {
      try {
        return new ArraySegment<byte>(
          this.receivedBytes, this.receivedByteIndex, this.receivedByteCount
        );
      }
      finally {
        this.receivedByteCount = 0;
      }
    }

    /// <summary>Resets the parser to its initial state</summary>
    public void Reset() {
      this.accumulatedRequestSize = 0;
      this.storedByteCount = 0;
      this.receivedByteCount = 0;
      this.storedBytesEndWithCR = false;
    }

    /// <summary>Attempts to parse a complete line from the received data</summary>
    /// <returns>The complete line or null if more data is required</returns>
    /// <remarks>
    ///   Before calling this method, you have to assign the data to be parsed using
    ///   the <see cref="SetReceivedData" /> method. The idea is to call
    ///   <see cref="SetReceivedData" /> once and then keep calling this method until
    ///   it returns null (meaning it ran out of data), at which point you can call
    ///   <see cref="SetReceivedData" /> and continue parsing lines or call
    ///   <see cref="GetRemainingData" /> instead to retrieve the still unparsed
    ///   bytes following the most recently parsed line.
    /// </remarks>
    public string ParseLine() {

      // Find out how many bytes remain to be parsed. If we run out of bytes,
      // tell this to the caller by returning null
      if(this.receivedByteCount == 0) {
        return null;
      }

      // If the data we parsed in the last run ended with a CR, it might be a line
      // break that got split in two packets.
      if(this.storedBytesEndWithCR) {

        // Find out whether a line feed follows
        bool isLineFeed = (this.receivedBytes[this.receivedByteIndex] == LF);

        // Advance to the next character. We don't have to care about request length
        // here because the request will be rejected when it's clear that it cannot
        // complete before exceeding the maximum length.
        ++this.accumulatedRequestSize;
        ++this.receivedByteIndex;
        --this.receivedByteCount;

        // Ensure the store buffer is cleared, whatever happens
        try {

          // If the next received byte is not an LF, that means there's a lone CR in
          // the stream, which the RFC says isn't allowed
          if(!isLineFeed) {
            HandleLoneCarriageReturn();
          }

          // Transform the received bytes into a string. No verification needed at
          // this point since all characters in the buffer are verified already
          return TransformToString(this.storedBytes, 0, this.storedByteCount - 1);

        }
        finally {
          this.storedBytesEndWithCR = false;
          this.storedByteCount = 0;
        }

      } // if storedBytesEndWithCR

      return internalScanForLineEnding();

    }

    /// <summary>
    ///   Called when the message is growing beyond the maximum message size
    /// </summary>
    /// <returns>
    ///   An exception that will be thrown to indicate the too large message
    /// </returns>
    protected abstract Exception HandleMessageTooLarge();

    /// <summary>
    ///   Called when the message contains a carriage return without a line feed
    /// </summary>
    /// <remarks>
    ///   It is safe to throw an exception here. The exception will travel up in
    ///   the call stack to the caller of the <see cref="ParseLine" /> method.
    /// </remarks>
    protected abstract void HandleLoneCarriageReturn();

    /// <summary>
    ///   Called to scan the bytes of a potential line for invalid characters
    /// </summary>
    /// <param name="buffer">
    ///   Array containing the bytes that to can for invalid characters
    /// </param>
    /// <param name="start">Index in the array at which to begin reading</param>
    /// <param name="count">Number of bytes from the array to scan</param>
    /// <remarks>
    ///   <para>
    ///     This method is used to check for invalid characters even before a complete
    ///     line has been received. It will be called with incomplete lines (for example,
    ///     when the received data ends before a CR LF is encountered) to allow for early
    ///     rejection of data containing characters not allowed by a protocol.
    ///   </para>
    ///   <para>
    ///     It is safe to throw an exception here. The exception will travel up in
    ///     the call stack to the caller of the <see cref="ParseLine" /> method.
    ///   </para>
    /// </remarks>
    protected abstract void VerifyPotentialLine(byte[] buffer, int start, int count);

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
    protected abstract string TransformToString(byte[] buffer, int start, int count);

    /// <summary>Internal method that scans the received data for a header line</summary>
    /// <returns>
    ///   The header line if enough data for at least one complete line was available,
    ///   null if more data is required
    /// </returns>
    private string internalScanForLineEnding() {

      // Find out how many bytes we can access before exceeding the maximum
      // request size defined by the user
      int safeByteCount = Math.Min(
        this.receivedByteCount, this.maximumMessageSize - this.accumulatedRequestSize
      );

      // Look for the next carriage return in the stream
      int crIndex = Array.IndexOf<byte>(
        this.receivedBytes,
        CR,
        this.receivedByteIndex,
        safeByteCount
      );

      // No line terminator found? Assume all received data belongs to a single line.
      if(crIndex == -1) {

        // First scan the received data for invalid characters. If the request is too
        // large, we still do this because we want to mime a byte-by-byte parser that
        // would encounter the invalid character before noticing the request is too large.
        VerifyPotentialLine(this.receivedBytes, this.receivedByteIndex, safeByteCount);

        // We know there's no CR in the scanned data. If the data comes to within 1
        // byte of the maximum header size, a valid request line is not possible
        // anymore. This also catches the case when safeByteCount was capped.
        int totalSize = this.accumulatedRequestSize + safeByteCount;
        if(totalSize >= (this.maximumMessageSize - 1)) {
          throw HandleMessageTooLarge();
        }

        // Copy all remaining characters into our temporary line buffer so we can use them
        // later when the request line will (hopefully) be terminated. We do not have to
        // care that this takes over receive buffer because a too large message header
        // would have been caught in the previous if already.
        internalTakeOverReceiveBuffer();
        return null;

      } else { // Line terminator found

        // At this point, only three outcomes are possible: We either run out of data,
        // obtain a complete line or discover that the CR is not followed by an LF,
        // meaning the request line is invalid.
        return internalParsePotentialLine(crIndex);

      }

    }

    /// <summary>Parses a potential request line for final LF character</summary>
    /// <param name="crIndex">Index of the CR character in the received data</param>
    /// <returns>
    ///   A string containing the parsed line or null if more data is required
    /// </returns>
    private string internalParsePotentialLine(int crIndex) {

      // Find out how many bytes in the receive buffer we skipped from the start
      // of the current line to the index of the CR we discovered
      int skippedBytes = crIndex - this.receivedByteIndex;

      // Make sure the received bytes are valid characters. As before, we try to mime
      // a byte-by-byte parser, so we will check for this first before handling
      // potential errors at the CR we just discovered.
      VerifyPotentialLine(this.receivedBytes, this.receivedByteIndex, skippedBytes);

      // If the CR is at the end of the receive buffer, the request might be invalid
      // in case the CR is exactly at the edge of the allowed header length. Just
      // one byte too large, but we care about precision here!
      int totalSize = this.accumulatedRequestSize + skippedBytes;
      if(totalSize >= (this.maximumMessageSize - 1)) {
        throw HandleMessageTooLarge();
      }

      // Find out whether we can safely take at least one more byte from the receive
      // buffer. If that's the case, we might be able to avoid a buffer copy. If,
      // on the other hand, the CR was the final byte we were provided with, we have
      // to stop here and tell the caller to give us more data.
      bool oneMoreByteAvailable = ((skippedBytes + 1) < this.receivedByteCount);
      if(!oneMoreByteAvailable) {
        internalTakeOverReceiveBuffer();
        this.storedBytesEndWithCR = true;
        return null;
      }

      // Find out whether the character that follows is a line feed.
      bool isLineFeed = (this.receivedBytes[crIndex + 1] == LF);

      // Make sure the buffer pointers are updated whatever happens next.
      try {

        // If this is not a line feed character, we have found a lone CR character
        // and thus, the request is invalid.
        if(!isLineFeed) {
          HandleLoneCarriageReturn();
        }

        // Optimization: If the complete line is in the receive buffer, we do not need
        // to waste time copying data to the store buffer
        if(this.storedByteCount == 0) {
          return TransformToString(
            this.receivedBytes, this.receivedByteIndex, skippedBytes
          );
        } else { // Line is split between store buffer and receive buffer
          ensureAdditionalStoreCapacity(skippedBytes);
          Array.Copy(
            this.receivedBytes,
            this.receivedByteIndex,
            this.storedBytes,
            this.storedByteCount,
            skippedBytes
          );
          return TransformToString(
            this.storedBytes, 0, this.storedByteCount + skippedBytes
          );
        }

      }
      finally {
        skippedBytes += 2;
        this.storedByteCount = 0;
        this.receivedByteIndex += skippedBytes;
        this.receivedByteCount -= skippedBytes;
        this.accumulatedRequestSize += skippedBytes;
      }

    }

    /// <summary>Takes over the current receive buffer into the store buffer</summary>
    private void internalTakeOverReceiveBuffer() {

      // Make sure the store buffer is large enough to take the received data
      ensureAdditionalStoreCapacity(this.receivedByteCount);

      // Done, now append all received data to the store buffer
      try {
        this.accumulatedRequestSize += this.receivedByteCount;
        Array.Copy(
          this.receivedBytes,
          this.receivedByteIndex,
          this.storedBytes,
          this.storedByteCount,
          this.receivedByteCount
        );
      }
      finally {
        this.storedByteCount += this.receivedByteCount;
        this.receivedByteCount = 0;
      }

    }

    /// <summary>
    ///   Makes sure that the line buffer has enough capacity to fit the specified
    ///   amount of additional characters in it
    /// </summary>
    /// <param name="additionalSize">Number of required additional characters</param>
    private void ensureAdditionalStoreCapacity(int additionalSize) {

      // See whether we need to do anything at all
      bool needsExpansion =
        ((this.storedByteCount + additionalSize) > this.storedBytes.Length);

      // If we need to expand, resize the store buffer to the next highest power of 2
      if(needsExpansion) {
        int newSize = IntegerHelper.NextPowerOf2(
          this.storedByteCount + additionalSize
        );

        byte[] newStoredBytes = new byte[newSize];
        Array.Copy(this.storedBytes, newStoredBytes, this.storedByteCount);
        this.storedBytes = newStoredBytes;
      }

    }

    /// <summary>Maximum size the request header is allowed to reach</summary>
    private int maximumMessageSize;
    /// <summary>Buffer containing the received bytes while they're processed</summary>
    private byte[] receivedBytes;
    /// <summary>Current index in the received bytes the parser is working at</summary>
    private int receivedByteIndex;
    /// <summary>Number of received bytes left to process</summary>
    private int receivedByteCount;
    /// <summary>
    ///   Stores received data if it needs to be remembered between two parse runs
    /// </summary>
    private byte[] storedBytes;
    /// <summary>Number of bytes in the temporary store buffer</summary>
    private int storedByteCount;
    /// <summary>Whether the final byte in the temporary store buffer is a CR</summary>
    private bool storedBytesEndWithCR;
    /// <summary>Total size of the request</summary>
    private int accumulatedRequestSize;

  }

} // namespace Nuclex.Networking
