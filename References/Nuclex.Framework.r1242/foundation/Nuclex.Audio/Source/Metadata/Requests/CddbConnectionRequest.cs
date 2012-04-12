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
using Nuclex.Support.Tracking;
using Nuclex.Support.Scheduling;

namespace Nuclex.Audio.Metadata.Requests {

  /// <summary>Establishes a connection to a CDDB compatible server</summary>
  internal class CddbConnectionRequest :
    Request<CddbConnection>, IAbortable, IProgressReporter {

    /// <summary>ASCII code for the space character</summary>
    private const char SP = ' ';

    #region struct ServerGreeting

    /// <summary>Stores the informations returned by the server greeting</summary>
    internal struct ServerGreeting {

      /// <summary>Initializes a new server greeting structure</summary>
      /// <param name="hostname">Hostname of the connected server</param>
      /// <param name="version">
      ///   Version number of the CDDB software running on the server
      /// </param>
      /// <param name="date">Current server timestamp</param>
      public ServerGreeting(string hostname, string version, string date) {
        this.Hostname = hostname;
        this.Version = version;
        this.Date = date;
      }

      /// <summary>Server host name. Example: xyz.fubar.com</summary>
      public string Hostname;
      /// <summary>Version number of server software. Example: v1.0PL0</summary>
      public string Version;
      /// <summary>Current date and time.  Example: Wed Mar 13 00:41:34 1996</summary>
      public string Date;

    }

    #endregion // struct ServerGreeting

    /// <summary>Triggered when the status of the process changes</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new CDDB connection request</summary>
    /// <param name="serverUrl">
    ///   URL of the server a connection will be established to
    /// </param>
    /// <param name="port">Port to which to try to establish the CDDB connection</param>
    /// <param name="credentials">Credentials by which to log in to the CDDB server</param>
    public CddbConnectionRequest(
      string serverUrl, short port, Cddb.Credentials credentials
    ) {
      this.serverUrl = serverUrl;
      this.port = port;
      this.credentials = credentials;
    }

    /// <summary>Begins executing the CDDB connection request in the background</summary>
    public void Start() {
      this.socket = new Socket(
        AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp
      );
      socket.BeginConnect(
        this.serverUrl, this.port, onConnnected, null
      );
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
      if(this.socket != null) {
        this.exception = new AbortedException("Aborted on user request");
        closeSocket();
      }
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
    protected override CddbConnection GatherResults() {
      return this.connection;
    }

    /// <summary>Triggers the AsyncProgressChanged event</summary>
    /// <param name="progress">New progress to report to the event subscribers</param>
    protected virtual void OnProgressAchieved(float progress) {
      EventHandler<ProgressReportEventArgs> copy = AsyncProgressChanged;
      if(copy != null) {
        copy(this, new ProgressReportEventArgs(progress));
      }
    }

    /// <summary>Closes the socket in case an abort request has arrived</summary>
    private void closeSocket() {
      if(this.socket != null) {
        Socket theSocket = this.socket;
        this.socket = null;

        theSocket.Close();
      }
    }

    /// <summary>Called when a connection to the CDDB server has been established</summary>
    /// <param name="asyncResult">Result handle of the asynchronous operation</param>
    private void onConnnected(IAsyncResult asyncResult) {
      try {

        // Check if the callback was issued because the request has been aborted.
        // If so, set the exception to report to the user and stop right here.
        if(this.socket == null) {
          OnAsyncEnded();
          return;
        }

        // No abort request received, finish the connection
        this.socket.EndConnect(asyncResult);

        // A rough estimate. Depending on how long the CDDB server took to reply,
        // the request may have spent more than 90% of its time waiting for the connection
        // attempt to finish, but in general, this should be a solid estimate.
        OnProgressAchieved(0.4f);

        // Perform the remaining handshaking process asynchronously. If the socket finished
        // asynchronously, that means we're already executing in a ThreadPool thread which
        // we can use for the remainder of the handshaking process.
        if(asyncResult.CompletedSynchronously) {
          ThreadPool.QueueUserWorkItem(new WaitCallback(performClientServerHandshake));
        } else {
          performClientServerHandshake(null);
        }
      }
      catch(Exception exception) {
        this.exception = exception;
        OnAsyncEnded();
        return;
      }
    }

    /// <summary>
    ///   Performs the client/server handshake upon connecting to a CDDB server
    /// </summary>
    /// <param name="status">Not used</param>
    private void performClientServerHandshake(object status) {
      try {
        this.protocol = new CddbProtocol(this.socket);

        // Receive the server greeting string. This is sent by the CDDB server to
        // any client as soon as it connects and contains the server status and
        // software running on the server
        string greetingLine = this.protocol.ReceiveLine(5000);

        OnProgressAchieved(0.6f);

        // Obtain the status code returned from the server and convert it to
        // an exception if it indicates an error
        int greetingStatusCode = CddbProtocol.GetStatusCode(greetingLine);
        this.exception = exceptionFromGreetingStatus(
          greetingStatusCode, greetingLine.Substring((greetingLine.Length >= 4) ? 4 : 3)
        );

        // If no error has occured, decode the server greeting string
        if(this.exception == null) {
          ServerGreeting greeting = decodeServerGreeting(greetingLine);

          // The server greeting was decoded successfully, now let's identify ourselves
          // to the CDDB server
          sendHello();

          OnProgressAchieved(0.8f);

          // Receive the reply to our 'hello' from the server
          receiveHelloReply();

          // If everything went fine the CDDB connection is ready to enter normal service
          if(this.exception == null) {
            this.connection = new CddbConnection(
              protocol,
              greeting.Hostname,
              greeting.Version,
              (greetingStatusCode == 201) // Read only connection?
            );
          }
        }
      }
      catch(ObjectDisposedException exception) {
        if(!(this.exception is AbortedException)) {
          this.exception = exception;
        }
      }
      catch(Exception exception) {
        this.exception = exception;
      }

      OnAsyncEnded();
    }

    /// <summary>
    ///   Generates an exception from the status code in the server greeting,
    ///   if the greeting indicates a problem
    /// </summary>
    /// <param name="statusCode">Status code provided by the server</param>
    /// <param name="message">Response returned by the server</param>
    /// <returns>
    ///   The exception for the server status code or null if the status code
    ///   indicates success
    /// </returns>
    private static Exception exceptionFromGreetingStatus(int statusCode, string message) {
      switch(statusCode) {
        case 200: { return null; }
        case 201: { return null; }
        case 432: { return new Exceptions.PermissionDeniedException(message); }
        case 433: { return new Exceptions.TooManyUsersException(message); }
        case 434: { return new Exceptions.SystemLoadTooHighException(message); }
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

    /// <summary>Decodes the greeting received from a CDDB server</summary>
    /// <param name="line">Line containing the server greeting</param>
    /// <returns>A structure with the individual greeting elements</returns>
    private static ServerGreeting decodeServerGreeting(string line) {

      // Look for the server host name and save its position
      int hostnameEndIndex = -1;
      bool hasHostname =
        (line.Length >= 5) &&
        (line[3] == SP) &&
        ((hostnameEndIndex = line.IndexOf(SP, 4)) != -1);

      if(!hasHostname) {
        throw makeBadResponseException("missing hostname in greeting line");
      }

      // Make sure the greeting contains the correct server type
      bool hasServerType =
        (line.Length >= hostnameEndIndex + 14) &&
        (line.Substring(hostnameEndIndex + 1, 13) == "CDDBP server ");

      if(!hasServerType) {
        throw makeBadResponseException("missing server type in greeting line");
      }

      // Look for the server software version and save its position
      int versionEndIndex = -1;
      bool hasVersion =
        (line.Length > hostnameEndIndex + 15) &&
        ((versionEndIndex = line.IndexOf(SP, hostnameEndIndex + 15)) != -1);

      if(!hasVersion) {
        throw makeBadResponseException("missing server version in greeting line");
      }

      // Make sure the greeting contains the ready statement 
      bool hasReadyAt =
        (line.Length >= versionEndIndex + 9) &&
        (line.Substring(versionEndIndex + 1, 9) == "ready at ");

      if(!hasReadyAt) {
        throw makeBadResponseException("missing ready statement in greeting line");
      }

      // Look for the server time and save its position
      bool hasTime =
        (line.Length >= versionEndIndex + 10);

      if(!hasTime) {
        throw makeBadResponseException("missing server time in greeting line");
      }

      // The greeting line seems to be in order, extract the relevant pieces
      return new ServerGreeting(
        line.Substring(4, hostnameEndIndex - 4),
        line.Substring(hostnameEndIndex + 14, versionEndIndex - hostnameEndIndex - 14),
        line.Substring(versionEndIndex + 10)
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
        string.Format("Malformed greeting from CDDB server ({0})", detailedReason)
      );
    }

    /// <summary>Sends the 'hello' after the server has identified itself</summary>
    private void sendHello() {
      string hello = string.Format(
        "cddb hello {0} {1} {2} {3}",
        this.credentials.User,
        this.credentials.HostName,
        this.credentials.ClientName,
        this.credentials.Version
      );
      this.protocol.SendLine(hello, 5000);
    }

    /// <summary>Receives the repy to the 'hello' and validates it</summary>
    private void receiveHelloReply() {
      string replyLine = this.protocol.ReceiveLine(5000);

      int statusCode = CddbProtocol.GetStatusCode(replyLine);
      this.exception = exceptionFromHelloResponseStatus(
        statusCode, replyLine.Substring((replyLine.Length >= 4) ? 4 : 3)
      );
    }

    /// <summary>
    ///   Generates an exception from the status code returned in the response to
    ///   the 'hello' message
    /// </summary>
    /// <param name="statusCode">Status code provided by the server</param>
    /// <param name="message">Response returned by the server</param>
    /// <returns>
    ///   The exception for the server status code or null if the status code
    ///   indicates success
    /// </returns>
    private static Exception exceptionFromHelloResponseStatus(
      int statusCode, string message
    ) {
      switch(statusCode) {
        case 200: { return null; }
        case 402: { return new Exceptions.AlreadyShookHandsException(message); }
        case 431: { return new Exceptions.HandshakeFailedException(message); }
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

    /// <summary>URL of the CDDB server to which we connect</summary>
    private string serverUrl;
    /// <summary>Port on the CDDB server to which we connect</summary>
    private short port;
    /// <summary>Credentials by which to log in to the CDDB server</summary>
    private Cddb.Credentials credentials;

    /// <summary>Socket through which we're connected to the server</summary>
    private volatile Socket socket;
    /// <summary>Protocol handler used to communicate with the CDDB server</summary>
    private CddbProtocol protocol;
    /// <summary>
    ///   CDDB connected that's returned to the user upon successfully connecting
    /// </summary>
    private volatile CddbConnection connection;

    /// <summary>Exception that has occured during asynchronous processing</summary>
    private volatile Exception exception;

  }

} // namespace Nuclex.Audio.Metadata.Requests