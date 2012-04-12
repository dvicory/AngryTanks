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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using Nuclex.Networking.Exceptions;
using Nuclex.Support;
using Nuclex.Support.Tracking;
using Nuclex.Support.Scheduling;

namespace Nuclex.Audio.Metadata.Requests {

  /// <summary>
  ///   Queries the CDDB server for entries matching the provided informations
  /// </summary>
  internal class CddbQueryRequest : Request<Cddb.Disc[]>, IAbortable, IProgressReporter {

    /// <summary>ASCII code for the space character</summary>
    private const char SP = ' ';

    /// <summary>
    ///   Delegate used to notify the receiver of the active protocol level
    /// </summary>
    /// <param name="activeLevel">Active protocol level on the server</param>
    public delegate void ProtocolLevelNotificationDelegate(int activeLevel);

    /// <summary>Triggered when the status of the process changes</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new CDDB disc query request</summary>
    /// <param name="protocol">
    ///   Protocol used to communication with the CDDB server
    /// </param>
    /// <param name="discLengthSeconds">
    ///   Total length of the CD (from the beginning of the first track to the end of the
    ///   last track) in seconds
    /// </param>
    /// <param name="trackOffsetsSeconds">
    ///   Offsets of the individual tracks on the CD in seconds
    /// </param>
    public CddbQueryRequest(
      CddbProtocol protocol, int discLengthSeconds, int[] trackOffsetsSeconds
    ) {
      this.protocol = protocol;
      this.discLengthSeconds = discLengthSeconds;
      this.trackOffsetsSeconds = trackOffsetsSeconds;
    }

    /// <summary>Aborts the running process. Can be called from any thread.</summary>
    /// <remarks>
    ///   The receiver should honor the abort request and stop whatever it is
    ///   doing as soon as possible. The method does not impose any requirement
    ///   on the timeliness of the reaction of the running process, but implementers
    ///   are advised to not ignore the abort request and urged to try and design
    ///   their code in such a way that it can be stopped in a reasonable time
    ///   (eg. within 1 second of the abort request).
    /// </remarks>
    public void AsyncAbort() {
      if(this.protocol != null) {
        CddbProtocol theProtocol = this.protocol;
        this.protocol = null;

        this.exception = new AbortedException("Aborted on user request");

        theProtocol.Dispose();
      }
    }

    /// <summary>Starts the asynchronous execution of the request</summary>
    public void Start() {
      ThreadPool.QueueUserWorkItem(new WaitCallback(execute));
    }

    /// <summary>
    ///   Allows the specific request implementation to re-throw an exception if
    ///   the background process finished unsuccessfully
    /// </summary>
    protected override void ReraiseExceptions() {
      if(this.exception != null) {
        throw this.exception;
      }
    }

    /// <summary>
    ///   Allows the specific request implementation to re-throw an exception if
    ///   the background process finished unsuccessfully
    /// </summary>
    protected override Cddb.Disc[] GatherResults() {
      return this.results;
    }

    /// <summary>Triggers the AsyncProgressChanged event</summary>
    /// <param name="progress">New progress to report to the event subscribers</param>
    protected virtual void OnProgressAchieved(float progress) {
      EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
      if(copy != null) {
        copy(this, new ProgressReportEventArgs(progress));
      }
    }

    /// <summary>Called asynchronously to execute the request</summary>
    /// <param name="state">Not used</param>
    private void execute(object state) {
      lock(this.protocol.SyncRoot) {
        try {
          sendQuery();

          // The first reply will indicate the status of the request.
          string statusLine = this.protocol.ReceiveLine(5000);
          int statusCode = CddbProtocol.GetStatusCode(statusLine);

          switch(statusCode) {
            case 200: { // Exact match found
              this.results = new Cddb.Disc[1] {
                decodeDiscFromStatusLine(statusLine)
              };
              break;
            }
            case 202: { // No matches found
              this.results = new Cddb.Disc[0];
              break;
            }
            case 211: { // Inexact matches found
              this.results = receiveDiscs();
              break;
            }
            default: {
              this.exception = exceptionFromQueryStatus(
                statusCode, statusLine.Substring((statusLine.Length >= 4) ? 4 : 3)
              );
              break;
            }
          }

        }
        catch(Exception exception) {
          if(!(this.exception is AbortedException)) {
            this.exception = exception;
          }
        }
      }

      OnAsyncEnded();
    }

    /// <summary>Sends the query to the CDDB server</summary>
    private void sendQuery() {
      StringBuilder builder = new StringBuilder(192);

      // Build the initial query string consisting of the command, CDDB disc id
      // and the number of tracks on the CD
      builder.AppendFormat(
        "cddb query {0:x8} {1} ",
        Cddb.CalculateDiscId(this.discLengthSeconds, this.trackOffsetsSeconds),
        this.trackOffsetsSeconds.Length
      );

      // Append the start offsets 
      for(int index = 0; index < this.trackOffsetsSeconds.Length; ++index) {
        builder.AppendFormat("{0} ", this.trackOffsetsSeconds[index] * 75);
      }

      builder.Append(this.discLengthSeconds);

      this.protocol.SendLine(builder.ToString(), 5000);
    }

    /// <summary>Decodes disc informations directly from the status line</summary>
    /// <param name="statusLine">Status line containing disc informations</param>
    /// <returns>The decoded disc informations</returns>
    private Cddb.Disc decodeDiscFromStatusLine(string statusLine) {
      return decodeDisc(statusLine, 4);
    }

    /// <summary>Receives the disc informations being sent by the server</summary>
    /// <returns>All disc information records the server has transmitted</returns>
    private Cddb.Disc[] receiveDiscs() {
      List<Cddb.Disc> discList = new List<Cddb.Disc>();

      for(; ; ) {
        string line = this.protocol.ReceiveLine(5000);
        if(line == ".") {
          break;
        }

        discList.Add(decodeDisc(line, 0));
      }

      // All genres received, convert the list into an array that can be returned
      // to the owner of the request.
      return discList.ToArray();
    }

    /// <summary>Decodes the disc informations sent by the CDDB server</summary>
    /// <param name="line">Line containing the disc informations</param>
    /// <param name="startIndex">
    ///   Characer Index at which the disc informations begin
    /// </param>
    /// <returns>The decoded CDDB disc informations</returns>
    private Cddb.Disc decodeDisc(string line, int startIndex) {

      // Locate where in the string the current protocol level starts
      int categoryEndIndex = -1;
      bool hasCategory =
        (line.Length >= startIndex) &&
        ((categoryEndIndex = line.IndexOf(SP, startIndex)) != -1);

      if(!hasCategory) {
        throw makeBadResponseException("missing category name in query result");
      }

      // Locate where in the string the current protocol level starts
      int discIdEndIndex = -1;
      bool hasDiscId =
        (line.Length >= categoryEndIndex + 1) &&
        ((discIdEndIndex = line.IndexOf(SP, categoryEndIndex + 1)) != -1);

      if(!hasDiscId) {
        throw makeBadResponseException("missing disc id in query result");
      }

      bool hasDiscTitle =
        (line.Length >= discIdEndIndex + 1) &&
        (line[discIdEndIndex] == SP);

      if(!hasDiscTitle) {
        throw makeBadResponseException("missing disc title in query result");
      }

      string discId = line.Substring(
        categoryEndIndex + 1, discIdEndIndex - categoryEndIndex - 1
      );
      Cddb.DTitle artistAndAlbum = Cddb.SplitDiscTitle(
        line.Substring(discIdEndIndex + 1)
      );
      return new Cddb.Disc(
        line.Substring(startIndex, categoryEndIndex - startIndex),
        Convert.ToInt32(discId, 16),
        artistAndAlbum.Artist,
        artistAndAlbum.Title
      );

    }

    /// <summary>Constructs a new BadResponseException</summary>
    /// <param name="detailedReason">
    ///   Detailed reason to show in the exception message in addition to the standard
    ///   bad response message
    /// </param>
    /// <returns>The newly constructed exception</returns>
    private static BadResponseException makeBadResponseException(string detailedReason) {
      return new BadResponseException(
        string.Format("Malformed response from CDDB server ({0})", detailedReason)
      );
    }

    /// <summary>
    ///   Generates an exception from the status code in the reply to a disc query
    /// </summary>
    /// <param name="statusCode">Status code provided by the server</param>
    /// <param name="message">Response returned by the server</param>
    /// <returns>
    ///   The exception for the server status code or null if the status code
    ///   indicates success
    /// </returns>
    private static Exception exceptionFromQueryStatus(int statusCode, string message) {
      switch(statusCode) {
        case 403: {
          return new Exceptions.DatabaseEntryCorruptException(message);
        }
        case 409: {
          return new Exceptions.NoHandshakeException(message);
        }
        default: {
          return new BadResponseException(
            string.Format(
              "Bad response from CDDB server: invalid status code '{0}', " +
              "server message is '{1}'",
              statusCode, message
            )
          );
        }
      }
    }

    /// <summary>Exception that has occured during asynchronous processing</summary>
    private volatile Exception exception;
    /// <summary>Protocol used to communicate with the CDDB server</summary>
    private volatile CddbProtocol protocol;

    /// <summary>Total length of the CD in seconds</summary>
    private int discLengthSeconds;
    /// <summary>Offsets of the tracks on the CD in seconds</summary>
    private int[] trackOffsetsSeconds;

    /// <summary>Results returned by the CDDB server</summary>
    private volatile Cddb.Disc[] results;

  }

} // namespace Nuclex.Audio.Metadata.Requests