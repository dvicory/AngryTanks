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
using System.Runtime.InteropServices;

namespace Nuclex.Audio.Formats.Wave {

  /// <summary>Carries the informations stored in an extended wave format chunk</summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct WaveFormatEx {

    /// <summary>FormatTag specifier for normal, uncompressed PCM audio</summary>
    public const ushort WAVEFORMATPCM = WaveFormat.WAVEFORMATPCM;

    /// <summary>Size of the WaveFormatEx structure</summary>
    /// <remarks>
    ///   The chunk can change size to encompass additional data that may be added
    ///   to the wave format by Microsoft or whoever is responsible for the wave file
    ///   format. The number of bytes on disk will be this size plus 8 additional
    ///   bytes from the chunk's header.
    /// </remarks>
    public const int Size = 18;

    /// <summary>Signature by which this chunk can be recognized</summary>
    public readonly static byte[] Signature = WaveFormat.Signature;

    /// <summary>The basic WaveFormat structure being extended</summary>
    public WaveFormat WaveFormat;

    /// <summary>Number of bits for a single sample</summary>
    public ushort BitsPerSample;
    /// <summary>Number of bytes in extra informations appended to this structure</summary>
    /// <remarks>
    ///   Also reflected in the structure size, but redundantly specific here for
    ///   additional programmer nuisance
    /// </remarks>
    public ushort ExtraInformationSize;

    /// <summary>Reads the WaveFormat information structure from a binary stream</summary>
    /// <param name="reader">Reader from which to read the WaveFormat structure</param>
    /// <param name="waveFormatEx">Wave format structure to be written to the file</param>
    /// <returns>The total size of the structure as indicated in the size field</returns>
    public static int Load(BinaryReader reader, out WaveFormatEx waveFormatEx) {
      int chunkLength = WaveFormat.Load(reader, out waveFormatEx.WaveFormat);

      waveFormatEx.BitsPerSample = reader.ReadUInt16();
      waveFormatEx.ExtraInformationSize = reader.ReadUInt16();

      // Done, return the chunk's indicated length, the caller might be interested
      // in this number to either skip additional data or process it
      return chunkLength;
    }

    /// <summary>Saves the WaveFormat information structure into a binary stream</summary>
    /// <param name="writer">Writer through which to write the WaveFormat structure</param>
    /// <param name="waveFormatEx">Wave format structure to be written to the file</param>
    /// <remarks>
    ///   The default header has a total size of 26 bytes, 18 bytes are taken up by
    ///   the actual chunk and 8 additional bytes by the header that specifies the
    ///   chunk's signature and size.
    /// </remarks>
    public static void Save(BinaryWriter writer, ref WaveFormatEx waveFormatEx) {
      Save(writer, WaveFormatEx.Size, ref waveFormatEx);
    }

    /// <summary>Saves the WaveFormat information structure into a binary stream</summary>
    /// <param name="writer">Writer through which to write the WaveFormat structure</param>
    /// <param name="chunkSize">Total size of the format info chunk</param>
    /// <param name="waveFormatEx">Wave format structure to be written to the file</param>
    /// <remarks>
    ///   Note that this writes chunkSize + 8 bytes. The additional 8 bytes result from
    ///   the chunk's header that provides the chunk signature and size you indicate.
    /// </remarks>
    public static void Save(
      BinaryWriter writer, int chunkSize, ref WaveFormatEx waveFormatEx
    ) {
      WaveFormat.Save(writer, chunkSize, ref waveFormatEx.WaveFormat);

      writer.Write((UInt16)waveFormatEx.BitsPerSample);
      writer.Write((UInt16)waveFormatEx.ExtraInformationSize);
    }

  }

} // namespace Nuclex.Audio.Formats.Wave
