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

  /// <summary>An enumeration of the available entropy coding methods.</summary>
  public enum EntropyCodingMethodType {

    /// <summary>
    ///   Residual is coded by partitioning into contexts, each with it's own 4-bit
    ///   Rice parameter.
    /// </summary>
    PartitionedRice = 0,

    /// <summary>
    ///   Residual is coded by partitioning into contexts, each with it's own 5-bit
    ///   Rice parameter.
    /// </summary>
    PartitionedRice2 = 1

  }

  /// <summary>Header for the entropy coding method.</summary>
  public struct EntropyCodingMethod {

    /// <summary>Indicates which entropy coding method is being used</summary>
    public EntropyCodingMethodType Type;

    /// <summary>Parameters for the partition rice entropy coding method</summary>
    public ParitionedRiceEntropyCodingMethod PartitionedRice;

  }

  /// <summary>Rice partitioned residual</summary>
  public struct ParitionedRiceEntropyCodingMethod {

    /// <summary>The partition order, i.e. # of contexts = 2 ^ \a order</summary>
    public int Order;

    /// <summary>The context's Rice parameters and/or raw bits</summary>
    public ParitionedRiceEntropyCodingMethodContents Contents;

  }

  /// <summary>Contents of a Rice partitioned residual</summary>
  public struct ParitionedRiceEntropyCodingMethodContents {

    /// <summary>The Rice parameters for each context</summary>
    public int[] Parameters;

    /// <summary>Widths for escape-coded partitions</summary>
    /// <remarks>
    ///   Will be non-zero for escaped partitions and zero for unescaped partitions.
    /// </remarks>
    public int[] RawBits;

    /// <summary>
    ///   The capacity of the \a parameters and \a raw_bits arrays specified as an order,
    ///   i.e. the number of array elements allocated is 2 ^ \a capacity_by_order
    /// </summary>
    public int CapacityByOrder;

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
