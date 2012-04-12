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

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Audio.Formats.Wave {

  /// <summary>Unit Test for the WaveFormatEx structure</summary>
  [TestFixture]
  public class WaveFormatExTest {

    /// <summary>Verifies that the WaveFormatEx load and save routines match up</summary>
    [Test]
    public void TestLoadVersusSave() {
      WaveFormatEx original;
      original.WaveFormat.FormatTag = 0x1A2B;
      original.WaveFormat.ChannelCount = 42;
      original.WaveFormat.SamplesPerSecond = 123456789;
      original.WaveFormat.AverageBytesPerSecond = 987654321;
      original.WaveFormat.BlockAlignment = 1928;
      original.BitsPerSample = 9182;
      original.ExtraInformationSize = 22222;

      using(MemoryStream memoryStream = new MemoryStream()) {
        {
          BinaryWriter writer = new BinaryWriter(memoryStream);
          WaveFormatEx.Save(writer, ref original);
        }

        memoryStream.Position = 0;

        {
          WaveFormatEx restored;

          BinaryReader reader = new BinaryReader(memoryStream);
          int restoredFileLength = WaveFormatEx.Load(reader, out restored);

          Assert.AreEqual(WaveFormatEx.Size, restoredFileLength);
          Assert.AreEqual(
            original.WaveFormat.FormatTag, restored.WaveFormat.FormatTag
          );
          Assert.AreEqual(
            original.WaveFormat.ChannelCount, restored.WaveFormat.ChannelCount
          );
          Assert.AreEqual(
            original.WaveFormat.SamplesPerSecond, restored.WaveFormat.SamplesPerSecond
          );
          Assert.AreEqual(
            original.WaveFormat.AverageBytesPerSecond, restored.WaveFormat.AverageBytesPerSecond
          );
          Assert.AreEqual(
            original.WaveFormat.BlockAlignment, restored.WaveFormat.BlockAlignment
          );
          Assert.AreEqual(original.BitsPerSample, restored.BitsPerSample);
          Assert.AreEqual(original.ExtraInformationSize, restored.ExtraInformationSize);
        }
      }
    }

    /// <summary>
    ///   Verifies that the right exception is thrown if Load() is used on arbitrary data
    /// </summary>
    [Test]
    public void TestLoadNonWaveData() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        {
          byte[] testData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
          memoryStream.Write(testData, 0, testData.Length);
        }

        memoryStream.Position = 0;

        {
          WaveFormatEx restored;

          BinaryReader reader = new BinaryReader(memoryStream);
          Assert.Throws<IOException>(
            delegate() { WaveFormatEx.Load(reader, out restored); }
          );
        }
      }
    }

    /// <summary>
    ///   Verifies that the WaveFormatEx chunk of an actual wave file can be loaded
    /// </summary>
    [Test]
    public void TestLoadStandardWaveFile() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        {
          byte[] testData = new byte[] {
            0x66, 0x6D, 0x74, 0x20, 0x32, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x02, 0x00, 0x22, 0x56, 0x00, 0x00,
            0x27, 0x57, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00,
            0x20, 0x00//, 0xF4, 0x03, 0x07
          };
          memoryStream.Write(testData, 0, testData.Length);
        }

        memoryStream.Position = 0;

        {
          WaveFormatEx restored;

          BinaryReader reader = new BinaryReader(memoryStream);
          int restoredFileLength = WaveFormatEx.Load(reader, out restored);

          Assert.AreEqual(50, restoredFileLength);
          Assert.AreEqual(2, restored.WaveFormat.FormatTag);
          Assert.AreEqual(2, restored.WaveFormat.ChannelCount);
          Assert.AreEqual(22050, restored.WaveFormat.SamplesPerSecond);
          Assert.AreEqual(22311, restored.WaveFormat.AverageBytesPerSecond);
          Assert.AreEqual(1024, restored.WaveFormat.BlockAlignment);
          Assert.AreEqual(4, restored.BitsPerSample);
          Assert.AreEqual(32, restored.ExtraInformationSize);
        }
      }

    }

  }

} // namespace Nuclex.Audio.Formats.Wave

#endif // UNITTEST
