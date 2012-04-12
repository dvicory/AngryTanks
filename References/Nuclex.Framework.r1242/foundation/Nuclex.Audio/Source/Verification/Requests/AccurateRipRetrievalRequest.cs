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
using System.IO;
using System.Net;
using System.Threading;

using Nuclex.Support.Tracking;
using Nuclex.Support.Scheduling;

namespace Nuclex.Audio.Verification.Requests {

  /// <summary>
  ///   Provides audio file verfication services using the AccurateRip database
  /// </summary>
  internal partial class AccurateRipRetrievalRequest :
    Request<AccurateRip.CdInfo[]>, IAbortable {

    /// <summary>Initializes a new request for handling an AccurateRip query</summary>
    /// <param name="discId1">
    ///   Disc id of the CD calculated using AccurateRip's primary algorithm
    /// </param>
    /// <param name="discId2">
    ///   Disc id of the CD calculated using AccurateRip's secondary algorithm
    /// </param>
    /// <param name="cddbDiscId">
    ///   Disc id of the CD calculated using the CDDB algorithm
    /// </param>
    /// <param name="trackCount">Number of tracks on the CD</param>
    public AccurateRipRetrievalRequest(
      int discId1, int discId2, int cddbDiscId, int trackCount
    ) {

      // Remember the IDs of the CD we asked for so we notice when data from
      // the wrong CD would be returned (not that this should happen, but for reasons
      // of robustness, we check for it)
      this.requestedDiscId1 = discId1;
      this.requestedDiscId2 = discId2;
      this.requestedCddbDiscId = cddbDiscId;
      this.requestedTrackCount = trackCount;

    }

    /// <summary>Starts the request</summary>
    /// <remarks>
    ///   The request will run asynchronously. If an error occurs during preparation
    ///   of the request, the AsyncEnded event will be triggered in the calling thread.
    /// </remarks>
    public void Start() {
      try {

        // Build a query string that can be sent to the AccurateRip server in order
        // to obtain the CRCs of the individual tracks
        //
        // Example queryString (found on the internet, no idea what CD it even is)
        //   c/5/8/dBAR-010-0012085c-009054b5-6a0b3d0a.bin
        //
        string queryString = string.Format(
          "{0:x1}/{1:x1}/{2:x1}/dBAR-{3:d3}-{4:x8}-{5:x8}-{6:x8}.bin",
          this.requestedDiscId1 & 0xF,
          (this.requestedDiscId1 >> 4) & 0xF,
          (this.requestedDiscId1 >> 8) & 0xF,
          this.requestedTrackCount,
          this.requestedDiscId1,
          this.requestedDiscId2,
          this.requestedCddbDiscId
        );

        // Build a request to the AccurateRip server using the prepared query URL
        this.request = WebRequest.Create(
          "http://www.accuraterip.com/accuraterip/" + queryString
        );
        request.Method = "GET";
        request.Timeout = 15000; // 15 seconds is plenty!

        // We run the request asynchronously so the user can do something else while
        // the request ist executing.
        request.BeginGetResponse(
          new AsyncCallback(accurateRipResponseReceived), null
        );

      }
      catch(Exception exception) {
        this.exception = exception;
        OnAsyncEnded();
      }
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

      // Special internal knowledge: We don't have to worry about the Start() method
      // still running because the AccurateRip class that's using this request will not
      // hand out the request to the user before Start() has completed. Thus we can
      // assume that the request and asyncResult fields are either valid or that the
      // request has already ended, in which case it is too late to abort.

      // Perform this inside of a lock in case the call arrives after 
      lock(this) {
        if(this.request != null) {
          this.aborting = true;
          this.request.Abort(); // Will invoke the Request callback!
        }
      } // lock(this)

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
    protected override AccurateRip.CdInfo[] GatherResults() {
      return this.results;
    }

    /// <summary>
    ///   Callback invoked when a response from the AccurateRip server has been received or
    ///   when the web request has been aborted
    /// </summary>
    /// <param name="asyncResult">
    ///   Asynchronous result handle of the completed or aborted web request
    /// </param>
    private void accurateRipResponseReceived(IAsyncResult asyncResult) {
      try {
        lock(this) {

          // If the request was aborted, avoid calling the EndGetResponse() method since
          // it will only trigger an exception. There have been some discussions about
          // whether .NET requires you to call EndGetResponse() even then, but the idea
          // is that WebRequest.Abort() replaces EndGetResponse() which I believe is the
          // design goal as this behavior is the same across many classes in the .NET
          // framework (compare implementation of Socket, SerialPort, etc.)
          if(this.aborting) {

            // Set the exception (instead of throwing it). This is slightly more efficient
            // and, most importantly, doesn't cause the debugger to break right here if the
            // user has configured it to stop on throw.
            this.exception = new AbortedException("Request has been aborted");

          } else { // Request was not aborted

            // At this point, We can now be sure that we've not been aborted yet and that
            // any abort calls happening in right now will have to wait before the lock and
            // thus, only get to continue when the nulled our 'request' field.
            WebResponse response = request.EndGetResponse(asyncResult);

            // Begin decoding the response data. The request might still be receiving data
            // from the server asynchronously but the header has been decoded at this point
            // and the BinaryReader will block until the required data has been received
            // when accessing the response stream. Blocking is not an issue since we can be
            // sure that we're running in a ThreadPool thread right now.
            Stream responseDataStream = response.GetResponseStream();
            this.results = AccurateRipParser.DecodeQueryResponse(
              new BinaryReader(responseDataStream)
            );
          }

          this.request = null;

        } // lock(this)
      }
      catch(Exception exception) {
        this.exception = exception;
      }

      OnAsyncEnded();
    }

    /// <summary>Exception that occured during the processing of the request</summary>
    private volatile Exception exception;
    /// <summary>Set to true to indicate that the request is being aborted</summary>
    private bool aborting;
    /// <summary>Results returned by the query to the AccurateRip database</summary>
    private AccurateRip.CdInfo[] results;

    /// <summary>
    ///   Disc id of the requested CD calculated using AccurateRip's primary algorithm
    /// </summary>
    private int requestedDiscId1;
    /// <summary>
    ///   Disc id of the requested CD calculated using AccurateRip's secondary algorithm
    /// </summary>
    private int requestedDiscId2;
    /// <summary>Disc id of the requested CD calculated using the CDDB algorithm</summary>
    private int requestedCddbDiscId;
    /// <summary>Number of tracks on the requested CD</summary>
    private int requestedTrackCount;
    /// <summary>Asynchronously running web request to the AccurateRip server</summary>
    private volatile WebRequest request;

  }

} // namespace Nuclex.Audio.Verification.Requests
