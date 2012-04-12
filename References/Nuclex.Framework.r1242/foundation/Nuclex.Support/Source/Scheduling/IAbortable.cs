#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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

namespace Nuclex.Support.Scheduling {

  /// <summary>Interface for abortable processes</summary>
  public interface IAbortable {

    /// <summary>Aborts the running process. Can be called from any thread.</summary>
    /// <remarks>
    ///   The receive should honor the abort request and stop whatever it is
    ///   doing as soon as possible. The method does not impose any requirement
    ///   on the timeliness of the reaction of the running process, but implementers
    ///   are advised to not ignore the abort request and urged to try and design
    ///   their code in such a way that it can be stopped in a reasonable time
    ///   (eg. within 1 second of the abort request).
    /// </remarks>
    void AsyncAbort();

  }

} // namespace Nuclex.Support.Scheduling
