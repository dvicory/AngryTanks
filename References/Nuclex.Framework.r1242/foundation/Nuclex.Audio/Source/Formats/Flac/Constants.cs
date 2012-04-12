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

namespace Nuclex.Audio.Formats.Flac {

#if ENABLE_PINVOKE_FLAC_DECODER

  /// <summary>Constants used in the FLAC decoder</summary>
  public static partial class Constants {

    /// <summary>Maximum number of channels permitted by the format</summary>
    public const int MaximumChannelCount = 8;

    /// <summary>Maximum LPC order permitted by the format.</summary>
    public const int MaximumLpcOrder = 32;

    /// <summary>Maximum Rice partition order permitted by the format.</summary>
    public const int MaximumRicePartitionOrder = 15;

    /// <summary>
    ///   Maximum order of the fixed predictors permitted by the format.
    /// </summary>
    public const int MaximumFixedOrder = 4;

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac