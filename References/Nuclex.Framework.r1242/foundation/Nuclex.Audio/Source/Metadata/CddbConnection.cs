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

using Nuclex.Support.Tracking;

namespace Nuclex.Audio.Metadata {

  /// <summary>Connection to a CDDB compatible database server</summary>
  public class CddbConnection : IDisposable {

    #region struct ServerProtocolLevel

    /// <summary>
    ///   Stores the active and, if available, maximum supported protocol level
    ///   of a CDDB server
    /// </summary>
    public struct ServerProtocolLevel {

      /// <summary>Initializes a new CDDB server protocol level structure</summary>
      /// <param name="activeProtocolLevel">Active server protocol level to store</param>
      /// <param name="supportedProtocolLevel">
      ///   Maximum support server protocol level to store
      /// </param>
      public ServerProtocolLevel(
        int activeProtocolLevel,
        int? supportedProtocolLevel
      ) {
        this.ActiveProtocolLevel = activeProtocolLevel;
        this.SupportedProtocolLevel = supportedProtocolLevel;
      }

      /// <summary>Protocol level currently used by the CDDB server</summary>
      public int ActiveProtocolLevel;
      /// <summary>Maximum protocol level the CDDB server supports</summary>
      public int? SupportedProtocolLevel;

    }

    #endregion // struct ServerProtocolLevel

    /// <summary>Initializes a new CDDB server connection</summary>
    /// <param name="protocol">Protocol by which the CDDB server can be reached</param>
    /// <param name="hostname">Host name of the connected CDDB server</param>
    /// <param name="version">Version of the CDDB software running on the server</param>
    /// <param name="readOnly">Whether the server has restricted us to read access</param>
    internal CddbConnection(
      CddbProtocol protocol, string hostname, string version, bool readOnly
    ) {
      this.protocol = protocol;
      this.hostname = hostname;
      this.version = version;
      this.readOnly = readOnly;

      // This is not documented in the CDDB protocol specification, but all servers
      // I tested it with begin a new connection with protocol level 1. This also
      // is the only valid choice if the server wants to keep backward compatibility.
      this.activeProtocolLevel = 1;
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.protocol != null) {
        this.protocol.Dispose();
        this.protocol = null;
      }
    }

    /// <summary>Logs out from the server and closes the connection</summary>
    /// <returns>A request by which the log out process can be tracked</returns>
    /// <remarks>
    ///   You should call this method and wait for the request to finish before
    ///   disposing of the connection to gracefully leave the CDDB server.
    /// </remarks>
    public Request Quit() {
      Requests.CddbQuitRequest request = new Requests.CddbQuitRequest(
        this.protocol
      );
      request.Start();
      return request;
    }

    /// <summary>Lists the genre categories known to the CDDB server</summary>
    /// <returns>A request that will provide the genre list upon completion</returns>
    public Request<string[]> ListCategories() {
      Requests.CddbCategoryListRequest request = new Requests.CddbCategoryListRequest(
        this.protocol
      );
      request.Start();
      return request;
    }

    /// <summary>Retrieves the protocol level currently used by the CDDB server</summary>
    /// <returns>
    ///   A request that will provide the active protocol level upon completion
    /// </returns>
    public Request<ServerProtocolLevel> GetProtocolLevel() {
      Requests.CddbProtocolLevelRequest request = new Requests.CddbProtocolLevelRequest(
        this.protocol
      );
      request.Start();
      return request;
    }

    /// <summary>Changes the protocol level used by the CDDB server connection</summary>
    /// <param name="newLevel">New protocol level to switch to</param>
    /// <returns>
    ///   A request that will indicate when the protocol level has been changed
    /// </returns>
    public Request ChangeProtocolLevel(int newLevel) {
      Requests.CddbProtocolLevelRequest request = new Requests.CddbProtocolLevelRequest(
        this.protocol,
        newLevel,
        new Requests.CddbProtocolLevelRequest.ProtocolLevelNotificationDelegate(
          protocolLevelChanged
        )
      );
      request.Start();
      return request;
    }

    /// <summary>
    ///   Queries the CDDB server for informations about the specified disc
    /// </summary>
    /// <param name="discLengthSeconds">Total length of the CD in seconds</param>
    /// <param name="trackOffsetsSeconds">
    ///   Track offsets, in seconds, for each track of the CD
    /// </param>
    /// <returns>A request that will provide the query results upon completion</returns>
    public Request<Cddb.Disc[]> Query(
      int discLengthSeconds, int[] trackOffsetsSeconds
    ) {
      Requests.CddbQueryRequest request = new Requests.CddbQueryRequest(
        this.protocol, discLengthSeconds, trackOffsetsSeconds
      );
      request.Start();
      return request;
    }

    /// <summary>Reads the CDDB entry for the specified CD</summary>
    /// <param name="category">Category in which the CD's CDDB entry is stored</param>
    /// <param name="discId">Disc id of the CD whose CDDB entry will be read</param>
    /// <returns>A request that will provide the CDDB entry upon completion</returns>
    /// <remarks>
    ///   The CDDB specification requires you to first execute a query and only then
    ///   use the read command to retrieve a database file. Otherwise, the data being
    ///   returned is undefined.
    /// </remarks>
    public Request<Cddb.DatabaseEntry> Read(string category, int discId) {
      Requests.CddbReadRequest request = new Requests.CddbReadRequest(
        this.protocol, category, discId
      );
      request.Start();
      return request;
    }

    /// <summary>Hostname of the server to which the connection was made</summary>
    public string Hostname {
      get { return this.hostname; }
    }

    /// <summary>Version of the CDDB software running on the server</summary>
    public string Version {
      get { return this.version; }
    }

    /// <summary>
    ///   Whether the client has been restricted to read access by the server
    /// </summary>
    public bool ReadOnly {
      get { return this.readOnly; }
    }

    /// <summary>Currently active protocol level for the connection</summary>
    public int ProtocolLevel {
      get { return this.activeProtocolLevel; }
    }

    /// <summary>Called when the active protocol level has been changed</summary>
    /// <param name="newLevel">New protocol level the connection is using</param>
    private void protocolLevelChanged(int newLevel) {
      bool wasUtf = (this.activeProtocolLevel >= 6);
      bool isUtf = (newLevel >= 6);

      this.activeProtocolLevel = newLevel;

      if(wasUtf != isUtf) {
        this.protocol.UseUtf8 = isUtf;
      }
    }

    /// <summary>Protocol used to communicate with the CDDB server</summary>
    private CddbProtocol protocol;
    /// <summary>Hostname the server reported in the greeting string</summary>
    private string hostname;
    /// <summary>Version of the CDDB software supposedly running on the server</summary>
    private string version;
    /// <summary>Whether this client is in read-only mode</summary>
    private bool readOnly;
    /// <summary>Currently active protocol level for the connection</summary>
    private volatile int activeProtocolLevel;

  }

} // namespace Nuclex.Audio.Metadata
