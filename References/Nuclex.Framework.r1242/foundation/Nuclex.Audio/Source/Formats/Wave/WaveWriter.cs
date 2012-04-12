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

namespace Nuclex.Audio.Formats.Wave {

  /// <summary>Writes the data structures of windows-compatible wave files</summary>
  public class WaveWriter : IDisposable {

    /// <summary>Size of the wave file header ('RIFF' + filelength + 'WAVE')</summary>
    public const int FileHeaderSize = 4 + 4 + 4;

    /// <summary>Initializes a new wave writer</summary>
    /// <param name="stream">Stream the wave will be written into</param>
    public WaveWriter(Stream stream) {
      this.writer = new BinaryWriter(stream);
    }

    /// <summary>Immediately releases all resourced owned by the wave writer</summary>
    /// <remarks>
    ///   The behavior of the WaveWriter matches that of the other writers in the
    ///   .NET framework, meaning the Stream will be closed. If you don't need a writer
    ///   anymore but want to leave the stream open, just let go of the writer, do
    ///   not dispose it.
    /// </remarks>
    public void Dispose() {
      if(this.writer != null) {
        this.writer.Close();
        this.writer = null;
      }
    }

    /// <summary>Writes the global header of a wave file into the stream</summary>
    /// <param name="fileLength">Total length of the wave file</param>
    public void WriteFileHeader(int fileLength) {
      this.writer.Write(new byte[] { 0x52, 0x49, 0x46, 0x46 }); // 'RIFF'
      this.writer.Write((Int32)(fileLength - 8));
      this.writer.Write(new byte[] { 0x57, 0x41, 0x56, 0x45 }); // 'WAVE'
    }

    /// <summary>Write a format chunk using the old WaveFormat structure</summary>
    /// <param name="waveFormat">WaveFormat structure to write into the chunk</param>
    public void WriteChunk(ref WaveFormat waveFormat) {
      WaveFormat.Save(this.writer, ref waveFormat);
    }

    /// <summary>Write a format chunk using the WaveFormatEx structure</summary>
    /// <param name="waveFormatEx">
    ///   Extended WaveFormat structure to write into the chunk
    /// </param>
    public void WriteChunk(ref WaveFormatEx waveFormatEx) {
      WaveFormatEx.Save(this.writer, ref waveFormatEx);
    }

    /// <summary>Write a format chunk using the extensible WaveFormat structure</summary>
    /// <param name="waveFormatExtensible">
    ///   Extensible WaveFormat structure to write into the chunk
    /// </param>
    public void WriteChunk(ref WaveFormatExtensible waveFormatExtensible) {
      WaveFormatExtensible.Save(this.writer, ref waveFormatExtensible);
    }

    /// <summary>Write a data chunk consisting of 16 bit samples</summary>
    /// <param name="samples">Samples to write into the data chunk</param>
    public void WriteChunk(short[,] samples) {
      int sampleBytes = samples.GetLength(0) * samples.GetLength(1) * 2;

      writer.Write(new byte[] { 0x64, 0x61, 0x74, 0x61 }); // 'data'
      writer.Write((Int32)sampleBytes);

      for(int sample = 0; sample < samples.GetLength(1); ++sample) {
        for(int channel = 0; channel < samples.GetLength(0); ++channel) {
          writer.Write((Int16)samples[channel, sample]);
        }
      }
    }

    /// <summary>Writer used to write the wave file into the stream</summary>
    private BinaryWriter writer;

  }

} // namespace Nuclex.Audio.Formats.Wave
