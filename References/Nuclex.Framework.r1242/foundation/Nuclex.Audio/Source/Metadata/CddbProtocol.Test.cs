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

  /// <summary>Unit Test for the CDDB protocol class</summary>
  [TestFixture]
  public class CddbProtocolTest {

    /// <summary>
    ///   Validates that the protocol can extract the status code form a server response
    /// </summary>
    [Test]
    public void TestStatusCodeRetrieval() {
      Assert.AreEqual(
        123, CddbProtocol.GetStatusCode("123 One two three")
      );
    }

    /// <summary>
    ///   Verifies that the status code retrieval throws an exception if the server
    ///   response is to short to contain a valid status code.
    /// </summary>
    [Test]
    public void TestThrowOnStatusCodeFromTooShortLine() {
      Assert.Throws<BadResponseException>(
        delegate() { CddbProtocol.GetStatusCode("12"); }
      );
    }

    /// <summary>
    ///   Verifies that the status code retrieval throws an exception if the server
    ///   response contains invalid characters
    /// </summary>
    [Test]
    public void TestThrowOnInvalidCharactersInStatusCode() {
      Assert.Throws<BadResponseException>(
        delegate() { CddbProtocol.GetStatusCode("12b This is not valid"); }
      );
    }

    /*
        /// <summary>
        ///   Tests whether the CDDB protocol can decode the shortest possible greeting
        /// </summary>
        [Test]
        public void TestDecodeServerGreeting() {
          CddbProtocol.ServerGreeting greeting = CddbProtocol.DecodeServerGreeting(
            "200 X CDDBP server 1 ready at 0"
          );
          Assert.AreEqual("X", greeting.Hostname);
          Assert.AreEqual("1", greeting.Version);
          Assert.AreEqual("0", greeting.Date);
        }
    */
  }

} // namespace Nuclex.Audio.Metadata

#endif // UNITTEST
