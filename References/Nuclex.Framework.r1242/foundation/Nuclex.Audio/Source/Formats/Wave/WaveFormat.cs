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

  /// <summary>Carries the informations stored in a wave format chunk</summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct WaveFormat {

    /// <summary>FormatTag specifier for normal, uncompressed PCM audio</summary>
    public const ushort WAVEFORMATPCM = 1;

    /// <summary>Size of the WaveFormat structure</summary>
    /// <remarks>
    ///   The chunk can change size to encompass additional data that may be added
    ///   to the wave format by Microsoft or whoever is responsible for the wave file
    ///   format. The number of bytes on disk will be this size plus 8 additional
    ///   bytes from the chunk's header.
    /// </remarks>
    public const int Size = 14;

    /// <summary>Signature by which this chunk can be recognized</summary>
    public readonly static byte[] Signature = new byte[] { 0x66, 0x6D, 0x74, 0x20 };

    /// <summary>Wave format, mainly used to indicate compression scheme</summary>
    public ushort FormatTag;
    /// <summary>Number of audio channels in the file</summary>
    public ushort ChannelCount;
    /// <summary>Playback sample frames per second</summary>
    public uint SamplesPerSecond;
    /// <summary>Average number of bytes per second when playing back the file</summary>
    /// <remarks>
    ///   <para>
    ///     This value should include padding and alignment. It is intended for systems
    ///     to estimate whether they can play back the audio file without any skips or
    ///     pauses on their hardware.
    ///   </para>
    ///   <para>
    ///     Can be calculated using the formula <tt>SamplesPerSecond * BlockAlignment</tt>
    ///     rounded up to the nearest whole number.
    ///   </para>
    /// </remarks>
    public uint AverageBytesPerSecond;
    /// <summary>Size of a single sample frame</summary>
    /// <remarks>
    ///   <para>
    ///     A sample frame is a single sample for each channels. Audio data in a wave file
    ///     is stored in interleaved form, eg. the first frame contains sample 0 for all
    ///     channels, the next frame then contains sample 1 for all channels.
    ///   </para>
    ///   <para>
    ///     On platforms with a byte size of 8 bits, the block alignment can be calculated
    ///     using the formular <tt>ChannelCount * (BitsPerSample / 8)</tt>
    ///   </para>
    /// </remarks>
    public ushort BlockAlignment;

    /// <summary>Reads the WaveFormat information structure from a binary stream</summary>
    /// <param name="reader">Reader from which to read the WaveFormat structure</param>
    /// <param name="waveFormat">Wave format structure to be written to the file</param>
    /// <returns>The total size of the structure as indicated in the size field</returns>
    public static int Load(BinaryReader reader, out WaveFormat waveFormat) {
      byte[] actualSignature = reader.ReadBytes(4);

      bool signatureIsValid =
        (actualSignature[0] == WaveFormat.Signature[0]) && // 'f'
        (actualSignature[1] == WaveFormat.Signature[1]) && // 'm'
        (actualSignature[2] == WaveFormat.Signature[2]) && // 't'
        (actualSignature[3] == WaveFormat.Signature[3]); // ' '

      if(!signatureIsValid) {
        throw new IOException(
          "Offset mismatch or binary data does not contain a wave format structure"
        );
      }

      // Obtain the total length of the chunk
      // The old style format info for uncompressed audio was always 16 bytes long
      int chunkLength = reader.ReadInt32();

      // Read the remainder of the wave format info structure
      waveFormat = new WaveFormat();
      waveFormat.FormatTag = reader.ReadUInt16();
      waveFormat.ChannelCount = reader.ReadUInt16();
      waveFormat.SamplesPerSecond = reader.ReadUInt32();
      waveFormat.AverageBytesPerSecond = reader.ReadUInt32();
      waveFormat.BlockAlignment = reader.ReadUInt16();

      // Done, return the chunk's indicated length, the caller might be interested
      // in this number to either skip additional data or process it
      return chunkLength;
    }

    /// <summary>Saves the WaveFormat information structure into a binary stream</summary>
    /// <param name="writer">Writer through which to write the WaveFormat structure</param>
    /// <param name="waveFormat">Wave format structure to be written to the file</param>
    /// <remarks>
    ///   The default header has a total size of 24 bytes, 16 bytes are taken up by
    ///   the actual chunk and 8 additional bytes by the header that specifies the
    ///   chunk's signature and size.
    /// </remarks>
    public static void Save(BinaryWriter writer, ref WaveFormat waveFormat) {
      Save(writer, WaveFormat.Size, ref waveFormat);
    }

    /// <summary>Saves the WaveFormat information structure into a binary stream</summary>
    /// <param name="writer">Writer through which to write the WaveFormat structure</param>
    /// <param name="chunkSize">Total size of the format info chunk</param>
    /// <param name="waveFormat">Wave format structure to be written to the file</param>
    /// <remarks>
    ///   Note that this writes chunkSize + 8 bytes. The additional 8 bytes result from
    ///   the chunk's header that provides the chunk signature and size you indicate.
    /// </remarks>
    public static void Save(BinaryWriter writer, int chunkSize, ref WaveFormat waveFormat) {
      writer.Write(Signature); // 'f', 'm', 't', ' '
      writer.Write((Int32)chunkSize);

      writer.Write((UInt16)waveFormat.FormatTag);
      writer.Write((UInt16)waveFormat.ChannelCount);
      writer.Write((UInt32)waveFormat.SamplesPerSecond);
      writer.Write((UInt32)waveFormat.AverageBytesPerSecond);
      writer.Write((UInt16)waveFormat.BlockAlignment);
    }

  }

} // namespace Nuclex.Audio.Formats.Wave
