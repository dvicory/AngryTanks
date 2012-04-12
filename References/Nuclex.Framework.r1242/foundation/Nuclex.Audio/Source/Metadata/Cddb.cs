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

  /// <summary>
  ///   Opens connections to a CDDB database and allows querying of CD titles
  /// </summary>
  public static class Cddb {

    #region struct Credentials

    /// <summary>Credentials with which to log into a CDDB server</summary>
    public struct Credentials {

      /// <summary>Initializes a new CDDB login credentials structure</summary>
      /// <param name="user">Login name of the user</param>
      /// <param name="hostName">Host name of the client</param>
      /// <param name="clientName">NAme of the connecting client software</param>
      /// <param name="version">Version number of the client software</param>
      public Credentials(
        string user, string hostName, string clientName, string version
      ) {
        this.User = user;
        this.HostName = hostName;
        this.ClientName = clientName;
        this.Version = version;
      }

      /// <summary>Login name of user. Example: johndoe</summary>
      public string User;
      /// <summary>Host name of client. Example: abc.fubar.com</summary>
      public string HostName;
      /// <summary>
      ///   The name of the connecting client. Example: xmcd, cda, EasyCD, et cetera.
      /// </summary>
      /// <remarks>
      ///   Do not use the name of another client which already	exists.
      /// </remarks>
      public string ClientName;
      /// <summary>Version number of client software. Example: v1.0PL0</summary>
      public string Version;

    }

    #endregion // struct Credentials

    #region struct DTitle

    /// <summary>Stores the contents of a CDDB DTITLE field</summary>
    public struct DTitle {

      /// <summary>Initializes a new CDDB DTITLE structure</summary>
      /// <param name="artist">Artist that released the song or the album</param>
      /// <param name="title">Name of the song or of the album</param>
      public DTitle(string artist, string title) {
        this.Artist = artist;
        this.Title = title;
      }

      /// <summary>Artist that released the song or the album</summary>
      public string Artist;
      /// <summary>Name of the song or of the album</summary>
      public string Title;

    }

    #endregion // struct DTitle

    #region struct Disc

    /// <summary>Informations about a disc that has been queried for</summary>
    public struct Disc {

      /// <summary>Initializes a new disc informations structure</summary>
      /// <param name="category">Category the disc is catalogued under</param>
      /// <param name="discId">CDDB disc id of the disc</param>
      /// <param name="artist">The artist of the disc</param>
      /// <param name="title">The title of the disc</param>
      public Disc(string category, int discId, string artist, string title) {
        this.Category = category;
        this.DiscId = discId;
        this.Artist = artist;
        this.Title = title;
      }

      /// <summary>Musical category of the CD, possibly wrong</summary>
      /// <remarks>
      ///   When CDDB came to existence, only 11 musical categories were defined with
      ///   a strong bias towards the creator's musical knowledge and/or taste.
      ///   Furthermore, CDDB servers a wary of changing these categories due to
      ///   crappy clients that are likely to break. Thus, today the categories are
      ///   mainly used to provide additional storage slots if duplicate disc ids occur.
      /// </remarks>
      public string Category;
      /// <summary>CDDB disc id of the disc</summary>
      public int DiscId;
      /// <summary>Artist of the CD</summary>
      public string Artist;
      /// <summary>Title of the CD</summary>
      public string Title;

    }

    #endregion // struct Disc

    #region struct DatabaseEntry

    /// <summary>Stores the contents of an XMCD database file</summary>
    public struct DatabaseEntry {

      /// <summary>Frame offsets of the individual tracks on the CD</summary>
      public int[] TrackFrameOffsets;
      /// <summary>Total length of the CD in seconds</summary>
      public int DiscLengthSeconds;
      /// <summary>Revision number of the database entry</summary>
      public int Revision;
      /// <summary>Application that has submitted the CDDB database entry</summary>
      public string Submitter;
      /// <summary>CDDB disc ids for this disc</summary>
      /// <remarks>
      ///   Storing multiple CDDB disc ids in a database entry has been deprecated,
      ///   normally you should only encounter entries with a single CDDB disc id.
      /// </remarks>
      public int[] DiscIds;
      /// <summary>Name of the artist who released this album</summary>
      /// <remarks>
      ///   If the CD is a sampler consisting of tracks from several artists, this
      ///   field will be set to 'Various'
      /// </remarks>
      public string Artist;
      /// <summary>Name of this album</summary>
      public string Album;
      /// <summary>Year that album was released in</summary>
      public int Year;
      /// <summary>Name of the genre the CD is categorized under</summary>
      public string Genre;
      /// <summary>Track titles of the individual tracks on the CD</summary>
      public DTitle[] Tracks;

    }

    #endregion // struct DatabaseEntry

    /// <summary>
    ///   Calculates the 'disc id' of a CD for submission or retrieval from a CDDB database
    /// </summary>
    /// <param name="discLengthSeconds">
    ///   Total length of the CD (from the beginning of the first track to the end of the
    ///   last track) in seconds
    /// </param>
    /// <param name="trackOffsetsSeconds">
    ///   Offsets of the individual tracks on the CD in seconds
    /// </param>
    /// <returns>The CDDB disc id of a CD with the provided characteristics</returns>
    /// <remarks>
    ///   Disc ids are not guaranteed to be unique. In fact, it is quite likely to find
    ///   duplicate ids all over the place. This is due to the CDDB disc id algorithm
    ///   which is severely flawed and has to be accepted. See the remarks on the
    ///   <see cref="CddbCategory" /> enumeration for how to get up to 11 CDs with the same
    ///   disc id enlisted in a CDDB database.
    /// </remarks>
    public static int CalculateDiscId(
      int discLengthSeconds, int[] trackOffsetsSeconds
    ) {
      int trackCount = trackOffsetsSeconds.Length;

      // First, the checksum needs to be calculated. This is done by calculating the
      // sum of digits for the starting offset (in seconds) of each track on the CD,
      // which are then added together and taken modulo 255 to arrive at the checksum.
      int offsetChecksum = 0;
      for(int trackIndex = 0; trackIndex < trackCount; ++trackIndex) {
        int trackOffsetSeconds = trackOffsetsSeconds[trackIndex];

        // Calculate the sum of digits for this track's starting offset
        int sumOfDigits = 0;
        while(trackOffsetSeconds > 0) {
          sumOfDigits += trackOffsetSeconds % 10;
          trackOffsetSeconds /= 10;
        }

        offsetChecksum += sumOfDigits;
      }
      offsetChecksum %= 255;

      // The rest of the CDDB disc id calculation is trivial: after the checksum (stored in
      // the highest byte), the next two bytes carry the total length of the CD in seconds
      // and the lowest byte is the number of tracks on the cD.
      int discId =
        (offsetChecksum << 24) |
        (discLengthSeconds << 8) |
        (trackCount & 0xFF);

      return discId;
    }

    /// <summary>Splits a CDDB DTITLE field into its artist and album name parts</summary>
    /// <param name="discTitle">
    ///   String containing a CDDB DTITLE field that will be split
    /// </param>
    /// <returns>Two strings containing the artist and the album name</returns>
    public static DTitle SplitDiscTitle(string discTitle) {
      int separatorIndex = discTitle.IndexOf(" / ");
      if(separatorIndex == -1) {
        return new DTitle(discTitle, discTitle);
      } else {
        return new DTitle(
          discTitle.Substring(0, separatorIndex),
          discTitle.Substring(separatorIndex + 3)
        );
      }
    }

    /// <summary>Establishes a connection to a public freedb database mirror</summary>
    /// <param name="credentials">Credentials by which to log in to the CDDB server</param>
    /// <returns>A request that provides the CDDB connection upon completion</returns>
    public static Request<CddbConnection> Connect(Credentials credentials) {
      return Connect("freedb.freedb.org", credentials);
    }

    /// <summary>
    ///   Establishes a connection to the specified CDDB compatible database server
    /// </summary>
    /// <param name="server">URL or IP address of the server to connect to</param>
    /// <param name="credentials">Credentials by which to log in to the CDDB server</param>
    /// <returns>A request that provides the CDDB connection upon completion</returns>
    public static Request<CddbConnection> Connect(string server, Credentials credentials) {
      Requests.CddbConnectionRequest connectionRequest =
        new Requests.CddbConnectionRequest(server, 8880, credentials);

      connectionRequest.Start();

      return connectionRequest;
    }

  }

} // namespace Nuclex.Audio.Metadata
