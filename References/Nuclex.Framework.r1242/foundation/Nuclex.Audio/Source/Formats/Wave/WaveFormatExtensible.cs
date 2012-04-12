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

  /// <summary>Carries the informations stored in an extensible wave format chunk</summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct WaveFormatExtensible {

    /// <summary>FormatTag specifier for normal, uncompressed PCM audio</summary>
    public const ushort WAVEFORMATPCM = WaveFormatEx.WAVEFORMATPCM;

    /// <summary>FormatTag specifier for the WaveFormatExtensible chunk</summary>
    public const ushort WAVEFORMATEXTENSIBLE = 65534;

    /// <summary>Signature by which this chunk can be recognized</summary>
    public readonly static byte[] Signature = WaveFormatEx.Signature;

    /// <summary>Size of the WaveFormatExtensible structure</summary>
    public const int Size = 40;

    /// <summary>Sample data is stored in analog format (unknown)</summary>
    public static readonly Guid KSDATAFORMAT_SUBTYPE_ANALOG = new Guid(
      "6dba3190-67bd-11cf-a0f7-0020afd156e4"
    );

    /// <summary>Sample data is stored in uncompressed PCM format</summary>
    public static readonly Guid KSDATAFORMAT_SUBTYPE_PCM = new Guid(
      "00000001-0000-0010-8000-00aa00389b71"
    );

    /// <summary>Sample data is stored in uncompressed IEEE float format</summary>
    public static readonly Guid KSDATAFORMAT_SUBTYPE_IEEE_FLOAT = new Guid(
      "00000003-0000-0010-8000-00aa00389b71"
    );

#if false // Undocumented sample data formats defined by windows
    public static readonly Guid KSDATAFORMAT_SUBTYPE_DRM = new Guid(
      "00000009-0000-0010-8000-00aa00389b71"
    );

    public static readonly Guid KSDATAFORMAT_SUBTYPE_ALAW = new Guid(
      "00000006-0000-0010-8000-00aa00389b71"
    );

    public static readonly Guid KSDATAFORMAT_SUBTYPE_MULAW = new Guid(
      "00000007-0000-0010-8000-00aa00389b71"
    );

    public static readonly Guid KSDATAFORMAT_SUBTYPE_ADPCM = new Guid(
      "00000002-0000-0010-8000-00aa00389b71"
    );

    public static readonly Guid KSDATAFORMAT_SUBTYPE_MPEG = new Guid(
      "00000050-0000-0010-8000-00aa00389b71"
    );
#endif

    /// <summary>Old style wave format description</summary>
    public WaveFormatEx FormatEx;

    /// <summary>Number of valid bits in each sample</summary>
    /// <remarks>
    ///   <para>
    ///     The new wave format allows the number of valid bits in a sample to be less
    ///     than the number of bits per sample. This allows for easier handling of
    ///     non byte sized sample precisions because samples can be of a regular byte
    ///     size but the implementation doesn't have to use all of the bits.
    ///   </para>
    ///   <para>
    ///     Should be set to 0 (indicating it's not used) when all sample bits are
    ///     filled and the precision actually matches a byte size.
    ///   </para>
    ///   <para>
    ///     Can also contain the samples per block if the bits per sample field in the
    ///     wave format structure is 0.
    ///   </para>
    /// </remarks>
    public ushort ValidBitsPerSample;

    /// <summary>Bit flags indicating which audio channels have been provided</summary>
    public ChannelMaskFlags ChannelMask;

    /// <summary>Specifies the actual sample data format used</summary>
    public Guid SubFormat;

    /// <summary>Reads the WaveFormat information structure from a binary stream</summary>
    /// <param name="reader">Reader from which to read the WaveFormat structure</param>
    /// <param name="waveFormat">Wave format structure to be written to the file</param>
    /// <returns>The total size of the structure as indicated in the size field</returns>
    public static int Load(BinaryReader reader, out WaveFormatExtensible waveFormat) {
      int totalSize = WaveFormatEx.Load(reader, out waveFormat.FormatEx);

      waveFormat.ValidBitsPerSample = reader.ReadUInt16();
      waveFormat.ChannelMask = (ChannelMaskFlags)reader.ReadUInt32();

      byte[] subFormatGuidBytes = reader.ReadBytes(16);
      waveFormat.SubFormat = new Guid(subFormatGuidBytes);

      return totalSize;
    }

    /// <summary>Saves the WaveFormat information structure into a binary stream</summary>
    /// <param name="writer">Writer through which to write the WaveFormat structure</param>
    /// <param name="waveFormat">Wave format structure to be written to the file</param>
    public static void Save(BinaryWriter writer, ref WaveFormatExtensible waveFormat) {
      WaveFormatEx.Save(writer, WaveFormatExtensible.Size, ref waveFormat.FormatEx);

      writer.Write((UInt16)waveFormat.ValidBitsPerSample);
      writer.Write((UInt32)waveFormat.ChannelMask);

      byte[] subFormatGuidBytes = waveFormat.SubFormat.ToByteArray();
      writer.Write(subFormatGuidBytes, 0, 16);
    }

  }

} // namespace Nuclex.Audio.Formats.Wave
