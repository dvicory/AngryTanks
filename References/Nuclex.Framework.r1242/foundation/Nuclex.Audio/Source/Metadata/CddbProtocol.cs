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
using System.Net.Sockets;
using System.Text;

using Nuclex.Networking;
using Nuclex.Networking.Exceptions;

namespace Nuclex.Audio.Metadata {

  /// <summary>
  ///   Handles communication following the CDDB protocol
  /// </summary>
  internal class CddbProtocol : IDisposable {

    /// <summary>ID of the ISO-8859-1 code page</summary>
    private const int ISO_8859_1 = 28591;

    #region class CddbLineParser

    /// <summary>Parses lines from CDDB replies</summary>
    private class CddbLineParser : LineParser {

      /// <summary>Initializes a new CDDB protocol line parser</summary>
      public CddbLineParser() : base(16384) { }

      /// <summary>
      ///   Called when the message is growing beyond the maximum message size
      /// </summary>
      /// <returns>
      ///   An exception that will be thrown to indicate the too large message
      /// </returns>
      protected override Exception HandleMessageTooLarge() {
        return new BadResponseException("Server response is too large");
      }

      /// <summary>
      ///   Called when the message contains a carriage return without a line feed
      /// </summary>
      /// <remarks>
      ///   It is safe to throw an exception here. The exception will travel up in
      ///   the call stack to the caller of the <see cref="LineParser.ParseLine" /> method.
      /// </remarks>
      protected override void HandleLoneCarriageReturn() { }

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
      ///     the call stack to the caller of the <see cref="LineParser.ParseLine" /> method.
      ///   </para>
      /// </remarks>
      protected override void VerifyPotentialLine(byte[] buffer, int start, int count) { }

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

    }

    #endregion // class CddbLineParser

    /// <summary>Initializes a new CDDB protocol handler</summary>
    /// <param name="connectedSocket">
    ///   Socket through which the CDDB can be reached
    /// </param>
    public CddbProtocol(Socket connectedSocket) {
      this.syncRoot = new object();
      this.socket = connectedSocket;
      this.lineParser = new CddbLineParser();
      this.buffer = new byte[256];
      updateEncoding();
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.socket != null) {
        this.socket.Shutdown(SocketShutdown.Both);
        this.socket.Close();
        this.socket = null;
      }
    }

    /// <summary>
    ///   Whether UTF-8 encoding is used to convert CDDB data between binary and text
    /// </summary>
    public bool UseUtf8 {
      get { return this.useUtf8; }
      set {
        if(value != this.useUtf8) {
          this.useUtf8 = value;
          updateEncoding();
        }
      }
    }

    /// <summary>
    ///   Synchronization root that can be used to ensure only one request communicates
    ///   with the CDDB server at a time
    /// </summary>
    public object SyncRoot {
      get { return this.syncRoot; }
    }

    /// <summary>Receives a single line from the CDDB server</summary>
    /// <param name="timeoutMilliseconds">
    ///   Timeout, in milliseconds, after which to stop waiting for incoming data
    /// </param>
    /// <returns>The received line</returns>
    public string ReceiveLine(int timeoutMilliseconds) {
      enforceAlive();

      // We might require any number of reads to complete a line if data is slowly
      // trickling into the socket
      for(; ; ) {

        // Try to extract a line from the data we received so far. If we had enough
        // data for a line, return it immediately.
        string line = this.lineParser.ParseLine();
        if(line != null) {
          return line;
        }

        // If this point is reached, we have not yet received a complete line from
        // the socket. Read all data from the socket's buffer (or wait for data
        // to arrive if the socket's buffer is empty)
        this.socket.ReceiveTimeout = timeoutMilliseconds;
        int receivedByteCount = this.socket.Receive(this.buffer, SocketFlags.None);
        if(receivedByteCount == 0) {
          throw new InvalidOperationException("Socket has been closed");
        }

        // Hand over the received data to the line parser. When the ParseLine() method
        // is called in the next loop cycle, the line parser will either decode
        // the line directly from the receive buffer or store the received data in
        // its own buffer to be able to complete the line with future incoming data.
        this.lineParser.SetReceivedData(this.buffer, 0, receivedByteCount);

      }
    }

    /// <summary>Sends a single line to the CDDB server</summary>
    /// <param name="line">Line that will be sent to the CDDB server</param>
    /// <param name="timeoutMilliseconds">
    ///   Timeout, in milliseconds, after which to stop waiting for the data to reach
    ///   the CDDB server
    /// </param>
    public void SendLine(string line, int timeoutMilliseconds) {
      enforceAlive();

      // Allocate an array large enough to hold at least the number of bytes the
      // line have when transformed into the target encoding
      int byteCount = this.encoding.GetMaxByteCount(line.Length);
      byte[] bytesToSend = new byte[byteCount + 2]; // + 2 for CR LF

      // Translate the line into a series bytes using the target encoding
      int actualByteCount = this.encoding.GetBytes(
        line, 0, line.Length, bytesToSend, 0
      );

      // Append a newline to the end of the line to terminate the command
      bytesToSend[actualByteCount] = 13;
      bytesToSend[actualByteCount + 1] = 10;

      // Reset the line parser for the reply (prevents the line parser from accumulating
      // the message size with each received line, eventually complaining that the received
      // message became too large)
      this.lineParser.Reset();

      // Everything is prepared, transmit the encoded line
      this.socket.SendTimeout = timeoutMilliseconds;
      this.socket.Send(bytesToSend, actualByteCount + 2, SocketFlags.None);
    }

    /// <summary>Extracts the status code from a server response</summary>
    /// <param name="line">Server response line expected to contain a status code</param>
    /// <returns>The status code as an integer</returns>
    public static int GetStatusCode(string line) {

      // If the line is shorter than 3 characters, it cannot contain a status code
      // and has to be considered invalid.
      if(line.Length < 3) {
        throw makeBadResponseException("response too short");
      }

      // Make sure the first three characters are numeric. If they aren't, we know it's
      // not a valid status code number and can report this error right away.
      bool allNumeric =
        char.IsNumber(line, 0) &&
        char.IsNumber(line, 1) &&
        char.IsNumber(line, 2);

      if(!allNumeric) {
        throw makeBadResponseException("status code is missing");
      }

      // There seems to be a number in the first three digits, parsing will likely
      // succeed. Return the status code to the caller as an integer.
      try {
        return Convert.ToInt32(line.Substring(0, 3));
      }
      catch(Exception exception) {
        throw makeBadResponseException("cannot parse status code", exception);
      }
    }

    /// <summary>Constructs a new BadResponseException</summary>
    /// <param name="detailedReason">
    ///   Detailed reason to show in the exception message in addition to the standard
    ///   bad response message
    /// </param>
    /// <returns>The newly constructed exception</returns>
    private static BadResponseException makeBadResponseException(string detailedReason) {
      return new BadResponseException(
        string.Format("Bad response from CDDB server ({0})", detailedReason)
      );
    }

    /// <summary>Constructs a new BadResponseException</summary>
    /// <param name="detailedReason">
    ///   Detailed reason to show in the exception message in addition to the standard
    ///   bad response message
    /// </param>
    /// <param name="innerException">
    ///   Inner exception that caused the response to be considered as malformed
    /// </param>
    /// <returns>The newly constructed exception</returns>
    private static BadResponseException makeBadResponseException(
      string detailedReason, Exception innerException
    ) {
      return new BadResponseException(
        string.Format("Bad response from CDDB server ({0})", detailedReason),
        innerException
      );
    }

    /// <summary>Updates the text encoding used in the protocol</summary>
    private void updateEncoding() {
      if(this.useUtf8) {
        this.encoding = Encoding.UTF8;
      } else {
        this.encoding = Encoding.GetEncoding(ISO_8859_1);
      }
    }

    /// <summary>Makes sure the object is still in a live, non-disposed state</summary>
    private void enforceAlive() {
      if(this.socket == null) {
        throw new ObjectDisposedException(
          "CddbProtocol", "The object has already been disposed"
        );
      }
    }

    /// <summary>Whether the protocol uses UTF-8 encoding for text</summary>
    /// <remarks>
    ///   Up to version 5 of CDDBP, ISO-8859-1 was used. This is the default encoding
    ///   that will be set when UseUTF8 is false. From version 6 onwards, the encoding
    ///   has been changed to UTF-8.
    /// </remarks>
    private volatile bool useUtf8;
    /// <summary>Encoding used to transform strings sent and received via CDDBP</summary>
    private volatile Encoding encoding;

    /// <summary>
    ///   Synchronization root used to ensure only one request communicates with
    ///   the CDDB server at a time
    /// </summary>
    private volatile object syncRoot;

    /// <summary>Socket by which we're connected to the CDDB server</summary>
    private volatile Socket socket;
    /// <summary>
    ///   Parser that chops binary server responses into one string per line
    /// </summary>
    private CddbLineParser lineParser;
    /// <summary>Receive buffer used to store incoming data from the socket</summary>
    private byte[] buffer;

  }

} // namespace Nuclex.Audio.Metadata
