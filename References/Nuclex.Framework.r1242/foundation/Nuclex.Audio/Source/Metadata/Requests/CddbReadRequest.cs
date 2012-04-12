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

  /// <summary>Queries the CDDB server for the XMCD file of a CD</summary>
  internal class CddbReadRequest :
    Request<Cddb.DatabaseEntry>, IAbortable, IProgressReporter {

    /// <summary>ASCII code for the space character</summary>
    private const char SP = ' ';

    /// <summary>Triggered when the status of the process changes</summary>
    public event EventHandler<ProgressReportEventArgs> AsyncProgressChanged;

    /// <summary>Initializes a new CDDB database read request</summary>
    /// <param name="protocol">
    ///   Protocol used to communication with the CDDB server
    /// </param>
    /// <param name="category">Category in which the CD's CDDB entry is stored</param>
    /// <param name="discId">Disc id of the CD whose CDDB entry will be read</param>
    public CddbReadRequest(
      CddbProtocol protocol, string category, int discId
    ) {
      this.protocol = protocol;
      this.category = category;
      this.discId = discId;
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
    protected override Cddb.DatabaseEntry GatherResults() {
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
          this.protocol.SendLine(
            string.Format("cddb read {0} {1:x8} ", this.category, this.discId),
            5000
          );

          // The first reply will indicate the status of the request.
          string statusLine = this.protocol.ReceiveLine(5000);
          int statusCode = CddbProtocol.GetStatusCode(statusLine);

          switch(statusCode) {
            case 210: {
              this.results = receiveDatabaseFile();
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
        case 401: {
          throw new Exceptions.EntryNotFoundException(message);
        }
        case 402: {
          throw new Exceptions.ServerErrorException(message);
        }
        case 403: {
          throw new Exceptions.DatabaseEntryCorruptException(message);
        }
        case 409: {
          throw new Exceptions.NoHandshakeException(message);
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

    /// <summary>Retrieves the XMCD database file for the CD</summary>
    /// <returns>An array of strings containing the lines of the database file</returns>
    private Cddb.DatabaseEntry receiveDatabaseFile() {
      XmcdDecoder decoder = new XmcdDecoder();

      for(; ; ) {
        string line = this.protocol.ReceiveLine(5000);
        if(line == ".") {
          break;
        }

        decoder.ProcessLine(line);
      }

      // All genres received, convert the list into an array that can be returned
      // to the owner of the request.
      return decoder.ToDatabaseEntry();
    }

    /// <summary>Exception that has occured during asynchronous processing</summary>
    private volatile Exception exception;
    /// <summary>Protocol used to communicate with the CDDB server</summary>
    private CddbProtocol protocol;

    /// <summary>Category in which the CD's CDDB entry is stored</summary>
    private string category;
    /// <summary>Disc id of the CD whose CDDB entry will be read</summary>
    private int discId;

    /// <summary>Results returned by the CDDB server</summary>
    private Cddb.DatabaseEntry results;

  }

} // namespace Nuclex.Audio.Metadata.Requests