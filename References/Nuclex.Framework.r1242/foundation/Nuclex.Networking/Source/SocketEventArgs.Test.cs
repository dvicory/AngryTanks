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
using System.Net.Sockets;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Networking {

  /// <summary>Unit Test for the socket event argument container</summary>
  [TestFixture]
  public class SocketEventArgsTest {

    /// <summary>
    ///   Tests whether an argument can be stored in the argument container
    /// </summary>
    [Test]
    public void TestArgumentPassing() {
      Socket testSocket = new Socket(
        AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp
      );

      SocketEventArgs arguments = new SocketEventArgs(testSocket);
      Assert.AreSame(testSocket, arguments.Socket);
    }

  }

} // namespace Nuclex.Networking

#endif // UNITTEST
