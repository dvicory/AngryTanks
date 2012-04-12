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
using System.Net.Sockets;

namespace Nuclex.Networking {

  /// <summary>Event argument container carrying a socket</summary>
  public class SocketEventArgs : EventArgs {

    /// <summary>Initializes a new socket event argument container</summary>
    /// <param name="socket">Socket to be provided to the event subscribers</param>
    public SocketEventArgs(Socket socket) {
      this.socket = socket;
    }

    /// <summary>The socket provided by the event</summary>
    public Socket Socket {
      get { return this.socket; }
    }

    /// <summary>Socket the event provides</summary>
    private Socket socket;

  }

} // namespace Nuclex.Networking
