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

namespace Nuclex.Audio.Verification {

  /// <summary>Parses replies from an AccurateRip server</summary>
  internal static class AccurateRipParser {

    /// <summary>
    ///   Decodes a response to a checksum query sent to the AccurateRip server
    /// </summary>
    /// <param name="reader">Reader through which to read the response data</param>
    /// <returns>The decoded AccurateRip response</returns>
    public static AccurateRip.CdInfo[] DecodeQueryResponse(BinaryReader reader) {
      List<AccurateRip.CdInfo[]> cdInfos = new List<AccurateRip.CdInfo[]>();

      for(;;) {

        // Try to decode the header. Since the stream does not supported seeking and thus,
        // cannot provide its length, we have to use the special case ReadByte() method to
        // see whether we have reached the end of the stream.
        int trackCountOrEndOfStream = reader.BaseStream.ReadByte();
        if(trackCountOrEndOfStream == -1) {
          break;
        }

        // Read the file header containing the disc ids and track count
        AccurateRip.CdInfo cdInfo;
        cdInfo.CddbDiscId = reader.ReadInt32();
        cdInfo.DiscId1 = reader.ReadInt32();
        cdInfo.DiscId2 = reader.ReadInt32();
        cdInfo.TrackInfos = decodeQueryResponseTracks(reader, trackCountOrEndOfStream);

      }
      
      return null;
    }

    /// <summary>Decodes the response from the AccurateRip server</summary>
    /// <param name="reader">Reader the server's response can be read from</param>
    /// <param name="trackCount">Number of tracks to decode</param>
    private static AccurateRip.TrackInfo[] decodeQueryResponseTracks(
      BinaryReader reader, int trackCount
    ) {
      AccurateRip.TrackInfo[] trackInfos = new AccurateRip.TrackInfo[trackCount];
      for(int trackIndex = 0; trackIndex < trackCount; ++trackIndex) {
        trackInfos[trackIndex].Confidence = reader.ReadByte();
        trackInfos[trackIndex].Crc32 = reader.ReadInt32();

        // No idea what these four bytes contain!
        reader.BaseStream.ReadByte();
        reader.BaseStream.ReadByte();
        reader.BaseStream.ReadByte();
        reader.BaseStream.ReadByte();
      }

      return trackInfos;
    }

  }

} // namespace Nuclex.Audio.Verification
