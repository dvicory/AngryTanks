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
  ///   Tells the CDDB server that the connection will be closed and closes it
  /// </summary>
  internal class CddbQuitRequest : Request {

    /// <summary>Initializes a new CDDB quit request</summary>
    /// <param name="protocol">
    ///   Protocol used to communication with the CDDB server
    /// </param>
    public CddbQuitRequest(CddbProtocol protocol) {
      this.protocol = protocol;
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

    /// <summary>Called asynchronously to execute the request</summary>
    /// <param name="state">Not used</param>
    private void execute(object state) {
      lock(this.protocol.SyncRoot) {
        try {
          this.protocol.SendLine("quit", 5000);

          // The first reply will indicate the status of the request.
          string statusLine = this.protocol.ReceiveLine(5000);
          int statusCode = CddbProtocol.GetStatusCode(statusLine);

          switch(statusCode) {
            case 230: {
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
          this.exception = exception;
        }

        // Close the connection, whether an error occured or not
        this.protocol.Dispose();
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
        case 530: {
          throw new Exception(message);
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

  }

} // namespace Nuclex.Audio.Metadata.Requests