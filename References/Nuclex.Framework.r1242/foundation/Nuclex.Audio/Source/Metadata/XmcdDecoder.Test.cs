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

#if UNITTEST

using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Nuclex.Networking.Exceptions;

namespace Nuclex.Audio.Metadata {

  /// <summary>Unit Test for the XMCD decoder class</summary>
  [TestFixture]
  public class XmcdDecoderTest {

    /// <summary>Example file from the XMCD specification</summary>
    private static readonly string[] exampleFile = new string[] {
      "# xmcd",
      "#",
      "# Track frame offsets:",
      "#	150",
      "#	210627",
      "#",
      "# Disc length: 2952 seconds",
      "#",
      "# Revision: 1",
      "# Submitted via: xmcd 2.0",
      "#",
      "DISCID=270b8617",
      "DTITLE=Franske Stemninger / Con Spirito",
      "DYEAR=1981",
      "DGENRE=Classical",
      "TTITLE0=Mille regretz de vous abandonner",
      "TTITLE22=L'arche de no",
      "EXTD=Copyright (c) 1981 MCA Records Inc.\nManufactured f",
      "EXTD=or MCA Records Inc.",
      "EXTT0=Des Prez\nYez",
      "EXTT22=Schmitt: A contre-voix \n(excerpt)",
      "PLAYORDER="
    };

    /// <summary>
    ///   Validates that the XMCD decoder is able to decode the example file
    /// </summary>
    [Test]
    public void TestXmcdFileParsing() {
      XmcdDecoder decoder = new XmcdDecoder();
      for(int index = 0; index < exampleFile.Length; ++index) {
        decoder.ProcessLine(exampleFile[index]);
      }

      Cddb.DatabaseEntry entry = decoder.ToDatabaseEntry();

      Assert.AreEqual(2, entry.TrackFrameOffsets.Length);
      Assert.AreEqual(150, entry.TrackFrameOffsets[0]);
      Assert.AreEqual(210627, entry.TrackFrameOffsets[1]);

      Assert.AreEqual(2952, entry.DiscLengthSeconds);

      Assert.AreEqual(1, entry.Revision);

      Assert.AreEqual("xmcd 2.0", entry.Submitter);

      Assert.AreEqual(1, entry.DiscIds.Length);
      Assert.AreEqual(0x270b8617, entry.DiscIds[0]);

      Assert.AreEqual("Franske Stemninger", entry.Artist);
      Assert.AreEqual("Con Spirito", entry.Album);

      Assert.AreEqual(1981, entry.Year);

      Assert.AreEqual("Classical", entry.Genre);

      Assert.AreEqual(23, entry.Tracks.Length);
      Assert.AreEqual("Mille regretz de vous abandonner", entry.Tracks[0].Title);
      Assert.AreEqual("L'arche de no", entry.Tracks[22].Title);
    }

  }

} // namespace Nuclex.Audio.Metadata

#endif // UNITTEST


