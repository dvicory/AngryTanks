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

using Nuclex.Audio.Metadata;
using Nuclex.Support.Tracking;

namespace Nuclex.Audio.Verification {

  /// <summary>
  ///   Provides audio file verfication services using the AccurateRip database
  /// </summary>
  public static class AccurateRip {

    #region struct TrackInfo

    /// <summary>
    ///   Informations about a track returned by a query to the AccurateRip database
    /// </summary>
    public struct TrackInfo {
      /// <summary>CRC32 checksum of the uncompressed PCM data of the track</summary>
      public int Crc32;
      /// <summary>
      ///   Confidence (eg. number of people with the same checksum) that this checksum
      ///   indicates a proper rip of the track
      /// </summary>
      public int Confidence;
    }

    #endregion // struct TrackInfo

    #region struct CdInfo

    /// <summary>
    ///   Informations about a CD returned by a query to the AccurateRip database
    /// </summary>
    public struct CdInfo {
      /// <summary>Disc ID for the CD calculated using the CDDB disc id algorithm</summary>
      public int CddbDiscId;
      /// <summary>
      ///   Disc ID for the CD calculated using the primary AccurateRip algorithm
      /// </summary>
      public int DiscId1;
      /// <summary>
      ///   Disc ID for the CD calculated using the secondary AccurateRip algorithm
      /// </summary>
      public int DiscId2;
      /// <summary>Informations about the individual tracks on this CD</summary>
      public TrackInfo[] TrackInfos;
    }

    #endregion // struct CdInfo

    /// <summary>
    ///   Queries the AccurateRip database for the CRCs of the tracks on the specified CD
    /// </summary>
    /// <param name="totalLengthSeconds">
    ///   Total length of the CD (from the beginning of the first track to the end of the
    ///   last track) in seconds
    /// </param>
    /// <param name="trackOffsetsSeconds">
    ///   Offsets of the individual tracks on the CD in seconds
    /// </param>
    /// <returns></returns>
    public static Request<CdInfo[]> QueryDatabase(
      int totalLengthSeconds, int[] trackOffsetsSeconds
    ) {
      try {

        // Calculate the three IDs required by a query to the AccurateRip database. AccurateRip
        // improves (marginally) on the flawed CDDB disc id by providing two additional ids,
        // however, this doesn't hurt AccurateRip nearly as much as it would a CDDB service.
        int discId1 = CalculateDiscId1(trackOffsetsSeconds);
        int discId2 = CalculateDiscId2(trackOffsetsSeconds);
        int cddbDiscId = Cddb.CalculateDiscId(totalLengthSeconds, trackOffsetsSeconds);
        int trackCount = trackOffsetsSeconds.Length;

        return QueryDatabase(discId1, discId2, cddbDiscId, trackCount);

      }
      catch(Exception exception) {
        return Request<CdInfo[]>.CreateFailedDummy(exception);
      }
    }

    /// <summary>
    ///   Queries the AccurateRip database for the CRCs of the tracks on the specified CD
    /// </summary>
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
    /// <returns></returns>
    public static Request<CdInfo[]> QueryDatabase(
      int discId1, int discId2, int cddbDiscId, int trackCount
    ) {
      try {

        // Set up a request to retrieve the checksums from the AccurateRip database
        Requests.AccurateRipRetrievalRequest retrievalRequest =
          new Requests.AccurateRipRetrievalRequest(
            discId1, discId2, cddbDiscId, trackCount
          );
        retrievalRequest.Start();

        // No exceptions until this point, the request is now running asynchronously and
        // the user can either wait for it to finish or register a callback
        return retrievalRequest;

      }
      catch(Exception exception) {
        return Request<CdInfo[]>.CreateFailedDummy(exception);
      }
    }

    /// <summary>Calculates a disc id using AccurateRip's primary id algorithm</summary>
    /// <param name="trackOffsetsSeconds">
    ///   Offsets of the individual tracks on the CD in seconds
    /// </param>
    /// <returns>The disc id calculated using AccurateRip's primary id algorithm</returns>
    public static int CalculateDiscId1(int[] trackOffsetsSeconds) {
      int discId = 0;

      for(int trackIndex = 0; trackIndex < trackOffsetsSeconds.Length; ++trackIndex) {
        discId += trackOffsetsSeconds[trackIndex];
      }

      return discId;
    }

    /// <summary>Calculates a disc id using AccurateRip's secondary id algorithm</summary>
    /// <param name="trackOffsetsSeconds">
    ///   Offsets of the individual tracks on the CD in seconds
    /// </param>
    /// <returns>The disc id calculated using AccurateRip's secondary id algorithm</returns>
    public static int CalculateDiscId2(int[] trackOffsetsSeconds) {
      int discId = 0;

      for(int trackIndex = 0; trackIndex < trackOffsetsSeconds.Length; ++trackIndex) {
        int trackOffsetSeconds = Math.Max(trackOffsetsSeconds[trackIndex], 1);
        discId += trackOffsetSeconds * (trackIndex + 1);
      }

      return discId;
    }

  }

} // namespace Nuclex.Audio.Verification
