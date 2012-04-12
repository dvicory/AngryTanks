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

namespace Nuclex.Audio.Metadata {

  /// <summary>Unit Test for the CDDB access and utility class</summary>
  [TestFixture]
  public class CddbTest {

    /// <summary>
    ///   Verifies that the constructor of the credentials structure works correctly
    /// </summary>
    [Test]
    public void TestCredentialsConstructor() {
      const string testUser = "john doe";
      const string testHost = "foobar";
      const string testClient = "sure";
      const string testVersion = "what version?";

      Cddb.Credentials credentials = new Cddb.Credentials(
        testUser, testHost, testClient, testVersion
      );
      Assert.AreEqual(testUser, credentials.User);
      Assert.AreEqual(testHost, credentials.HostName);
      Assert.AreEqual(testClient, credentials.ClientName);
      Assert.AreEqual(testVersion, credentials.Version);
    }

    /// <summary>
    ///   Verifies that the constructor of the dtitle structure works correctly
    /// </summary>
    [Test]
    public void TestDTitleConstructor() {
      const string testArtist = "R. Tist";
      const string testTitle = "Yes";

      Cddb.DTitle discTitle = new Cddb.DTitle(testArtist, testTitle);

      Assert.AreEqual(testArtist, discTitle.Artist);
      Assert.AreEqual(testTitle, discTitle.Title);
    }

    /// <summary>
    ///   Verifies that the constructor of the disc structure works correctly
    /// </summary>
    [Test]
    public void TestDiscConstructor() {
      const string testCategory = "rock";
      const int testDiscId = 0x12345678;
      const string testArtist = "R. Tist";
      const string testTitle = "Yes";

      Cddb.Disc disc = new Cddb.Disc(
        testCategory, testDiscId, testArtist, testTitle
      );

      Assert.AreEqual(testCategory, disc.Category);
      Assert.AreEqual(testDiscId, disc.DiscId);
      Assert.AreEqual(testArtist, disc.Artist);
      Assert.AreEqual(testTitle, disc.Title);
    }

    /// <summary>Validates the disc id calculation algorithm is correct</summary>
    [Test]
    public void TestDiscIdCalculation() {

      // Example given on wikipedia: CD with one track starting at 2 seconds
      Assert.AreEqual(
        0x020e1a01,
        Cddb.CalculateDiscId(3610, new int[] { 2 })
      );

      // Another example I've found on the 'net. It seems to be quite hard to track
      // down any test data to verify a given implementation of a CDDB disc id calculator
      Assert.AreEqual(
        0x2e0da505,
        Cddb.CalculateDiscId(
          3493,
          new int[] {
            182 / 75, // 2
            19527 / 75, // 260
            39015 / 75, // 520
            132282 / 75, // 1763
            189270 / 75 // 2523
          }
        )
      );

    }

    /// <summary>
    ///   Ensures that the SplitDiscTitle() method correctly splits a disc title
    ///   not containing a " / " sequence, meaning the artist and album name are equal.
    /// </summary>
    [Test]
    public void TestSplitSameArtistAlbum() {
      Cddb.DTitle artistAndAlbum = Cddb.SplitDiscTitle("Ensiferum");

      Assert.AreEqual("Ensiferum", artistAndAlbum.Artist);
      Assert.AreEqual("Ensiferum", artistAndAlbum.Title);
    }

    /// <summary>
    ///   Ensures that the SplitDiscTitle() method correctly splits a disc title
    ///   at the " / " sequence which delimits the artist and the album name.
    /// </summary>
    [Test]
    public void TestSplitDifferentArtistAlbum() {
      Cddb.DTitle artistAndAlbum = Cddb.SplitDiscTitle(
        "Catamenia / VIII: The Time Unchained"
      );

      Assert.AreEqual("Catamenia", artistAndAlbum.Artist);
      Assert.AreEqual("VIII: The Time Unchained", artistAndAlbum.Title);
    }

  }

} // namespace Nuclex.Audio.Metadata

#endif // UNITTEST
