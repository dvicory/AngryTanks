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

  /// <summary>Unit Test for the WaveFormat structure</summary>
  [TestFixture]
  public class WaveFormatTest {

    /// <summary>Verifies that the WaveFormat load and save routines match up</summary>
    [Test]
    public void TestLoadVersusSave() {
      WaveFormat original;
      original.FormatTag = 0x1A2B;
      original.ChannelCount = 42;
      original.SamplesPerSecond = 123456789;
      original.AverageBytesPerSecond = 987654321;
      original.BlockAlignment = 1928;

      using(MemoryStream memoryStream = new MemoryStream()) {
        {
          BinaryWriter writer = new BinaryWriter(memoryStream);
          WaveFormat.Save(writer, ref original);
        }

        memoryStream.Position = 0;

        {
          WaveFormat restored;

          BinaryReader reader = new BinaryReader(memoryStream);
          int restoredFileLength = WaveFormat.Load(reader, out restored);

          Assert.AreEqual(WaveFormat.Size, restoredFileLength);
          Assert.AreEqual(original.FormatTag, restored.FormatTag);
          Assert.AreEqual(original.ChannelCount, restored.ChannelCount);
          Assert.AreEqual(original.SamplesPerSecond, restored.SamplesPerSecond);
          Assert.AreEqual(original.AverageBytesPerSecond, restored.AverageBytesPerSecond);
          Assert.AreEqual(original.BlockAlignment, restored.BlockAlignment);
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
          WaveFormat restored;

          BinaryReader reader = new BinaryReader(memoryStream);
          Assert.Throws<IOException>(
            delegate() { WaveFormat.Load(reader, out restored); }
          );
        }
      }
    }

    /// <summary>
    ///   Verifies that the WaveFormat chunk of an actual wave file can be loaded
    /// </summary>
    [Test]
    public void TestLoadStandardWaveFile() {
      using(MemoryStream memoryStream = new MemoryStream()) {
        {
          byte[] testData = new byte[] {
            0x66, 0x6D, 0x74, 0x20, 0x32, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x02, 0x00, 0x22, 0x56, 0x00, 0x00,
            0x27, 0x57, 0x00, 0x00, 0x00, 0x04
          };
          memoryStream.Write(testData, 0, testData.Length);
        }

        memoryStream.Position = 0;

        {
          WaveFormat restored;

          BinaryReader reader = new BinaryReader(memoryStream);
          int restoredFileLength = WaveFormat.Load(reader, out restored);

          Assert.AreEqual(50, restoredFileLength);
          Assert.AreEqual(2, restored.FormatTag);
          Assert.AreEqual(2, restored.ChannelCount);
          Assert.AreEqual(22050, restored.SamplesPerSecond);
          Assert.AreEqual(22311, restored.AverageBytesPerSecond);
          Assert.AreEqual(1024, restored.BlockAlignment);
        }
      }


    }

  }

} // namespace Nuclex.Audio.Formats.Wave

#endif // UNITTEST
