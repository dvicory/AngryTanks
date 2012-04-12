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

  /// <summary>Retrieves or changes the protocol level of a CDDB connection</summary>
  internal class CddbProtocolLevelRequest :
    Request<CddbConnection.ServerProtocolLevel>, IAbortable, IProgressReporter {

    /// <summary>ASCII code for the space character</summary>
    private const char SP = ' ';

    /// <summary>
    ///   Delegate used to notify the receiver of the active protocol level
    /// </summary>
    /// <param name="activeLevel">Active protocol level on the server</param>
    public delegate void ProtocolLevelNotificationDelegate(int activeLevel);

    /// <summary>Triggered when the status of the process changes</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>
    ///   Initializes a new CDDB protocol level request for retrieval of
    ///   the current protocol level
    /// </summary>
    /// <param name="protocol">
    ///   Protocol used to communication with the CDDB server
    /// </param>
    public CddbProtocolLevelRequest(
      CddbProtocol protocol
    ) {
      this.protocol = protocol;
    }

    /// <summary>
    ///   Initializes a new CDDB protocol level request for changing of
    ///   the current protocol level
    /// </summary>
    /// <param name="protocol">
    ///   Protocol used to communication with the CDDB server
    /// </param>
    /// <param name="newLevel">New protocol level that will be set</param>
    /// <param name="callback">
    ///   Callback to be invoked as soon as the new protocol level is set
    /// </param>
    public CddbProtocolLevelRequest(
      CddbProtocol protocol, int newLevel, ProtocolLevelNotificationDelegate callback
    ) {
      this.protocol = protocol;
      this.newLevel = newLevel;
      this.callback = callback;
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
    protected override CddbConnection.ServerProtocolLevel GatherResults() {
      return this.protocolLevel;
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
          if(this.newLevel.HasValue) {
            changeProtocolLevel();
          } else {
            queryProtocolLevel();
          }
        }
        catch(Exception exception) {
          this.exception = exception;
        }
      }

      OnAsyncEnded();
    }

    /// <summary>Changes the CDDB server's active protocol level</summary>
    private void changeProtocolLevel() {

      // Issue the command to the CDDB server
      this.protocol.SendLine(string.Format("proto {0}", this.newLevel.Value), 5000);

      // Receive the reply from the server
      string statusLine = this.protocol.ReceiveLine(5000);

      // Obtain the status code returned by the server
      int statusCode = CddbProtocol.GetStatusCode(statusLine);

      // Decode the server reply
      switch(statusCode) {
        case 200: {
          this.protocolLevel = decodeDetailedProtocolLevel(statusLine);
          break;
        }
        case 201: {
          this.protocolLevel = decodeShortProtocolLevel(statusLine);
          break;
        }
        default: {
          this.exception = exceptionFromProtocolStatus(
            statusCode, statusLine.Substring((statusLine.Length >= 4) ? 4 : 3)
          );
          return;
        }
      }

      // Because we're pedantic, we'll check whether the server actually reported
      // the new protocol level we requested back to us.
      if(this.newLevel.Value != this.protocolLevel.ActiveProtocolLevel) {
        this.exception = new BadResponseException(
          "Server sent a positive response but didn't change the protocol level"
        );
      } else { // Everything went well
        this.callback(this.newLevel.Value);
      }

    }

    /// <summary>Queries the server for its active protocol level</summary>
    private void queryProtocolLevel() {

      // Issue the command to the CDDB server
      this.protocol.SendLine("proto", 5000);

      // Receive the reply from the server
      string statusLine = this.protocol.ReceiveLine(5000);

      // Obtain the status code returned by the server
      int statusCode = CddbProtocol.GetStatusCode(statusLine);

      // Decode the server reply
      switch(statusCode) {
        case 200: {
          this.protocolLevel = decodeDetailedProtocolLevel(statusLine);
          break;
        }
        case 201: {
          this.protocolLevel = decodeShortProtocolLevel(statusLine);
          break;
        }
        default: {
          this.exception = exceptionFromProtocolStatus(
            statusCode, statusLine.Substring((statusLine.Length >= 4) ? 4 : 3)
          );
          break;
        }
      }

    }

    /// <summary>Decodes a server response containing the detailed protocol level</summary>
    /// <param name="line">Status line received from the CDDB server</param>
    /// <returns>The decoded server protocol level</returns>
    private CddbConnection.ServerProtocolLevel decodeDetailedProtocolLevel(string line) {

      // Locate where in the string the current protocol level starts
      int currentIndex = -1;
      bool hasCurrent =
        (line.Length >= 5) &&
        (line[3] == SP) &&
        ((currentIndex = line.IndexOf("current ", 4)) != -1);

      if(!hasCurrent) {
        throw makeBadResponseException("missing current protocol level indication");
      }

      // Locate the end of the current protocol level number
      int currentNumberEndIndex = -1;
      bool hasCurrentNumber =
        (line.Length >= currentIndex + 9) &&
        ((currentNumberEndIndex = line.IndexOf(SP, currentIndex + 9)) != -1);

      if(!hasCurrentNumber) {
        throw makeBadResponseException("missing current protocol level number");
      }

      // Locate the position of the maximum supported protocol level in the string
      int supportedIndex = -1;
      bool hasSupported =
        (line.Length >= currentNumberEndIndex + 1) &&
        ((supportedIndex = line.IndexOf("supported ", currentNumberEndIndex + 1)) != -1);

      if(!hasSupported) {
        throw makeBadResponseException("missing supported protocol level indication");
      }

      // All positions are known, try to convert the levels into integers and return them
      return new CddbConnection.ServerProtocolLevel(
        Convert.ToInt32(
          line.Substring(currentIndex + 8, currentNumberEndIndex - currentIndex - 9)
        ),
        Convert.ToInt32(line.Substring(supportedIndex + 10))
      );

    }

    /// <summary>Decodes a server response containing the short protocol level</summary>
    /// <param name="line">Status line received from the CDDB server</param>
    /// <returns>The decoded server protocol level</returns>
    private CddbConnection.ServerProtocolLevel decodeShortProtocolLevel(string line) {

      // Locate where in the string the current protocol level starts
      int nowIndex = -1;
      bool hasNow =
        (line.Length >= 5) &&
        (line[3] == SP) &&
        ((nowIndex = line.IndexOf("now: ", 4)) != -1);

      if(!hasNow) {
        throw makeBadResponseException("missing current protocol level indication");
      }

      // We found the position of the protocol level, now try to convert it into
      // an integer and return it
      return new CddbConnection.ServerProtocolLevel(
        Convert.ToInt32(line.Substring(nowIndex + 5)),
        null
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
    ///   Generates an exception from the status code in the reply to
    ///   the genre list request
    /// </summary>
    /// <param name="statusCode">Status code provided by the server</param>
    /// <param name="message">Response returned by the server</param>
    /// <returns>
    ///   The exception for the server status code or null if the status code
    ///   indicates success
    /// </returns>
    private static Exception exceptionFromProtocolStatus(int statusCode, string message) {
      switch(statusCode) {
        case 501: {
          throw new Exceptions.IllegalProtocolLevelException(message);
        }
        case 502: {
          throw new Exceptions.ProtocolLevelAlreadySetException(message);
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
    private CddbProtocol protocol;
    /// <summary>Protocol level the CDDB server is currently using</summary>
    private CddbConnection.ServerProtocolLevel protocolLevel;
    /// <summary>Either null for query only or new protocol level to set</summary>
    private int? newLevel;
    /// <summary>Call to be invoked when the protocol level has changed</summary>
    private ProtocolLevelNotificationDelegate callback;

  }

} // namespace Nuclex.Audio.Metadata.Requests