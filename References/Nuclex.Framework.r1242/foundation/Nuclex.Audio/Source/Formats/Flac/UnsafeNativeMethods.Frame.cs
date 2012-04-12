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
using System.Runtime.InteropServices;

namespace Nuclex.Audio.Formats.Flac {

#if ENABLE_PINVOKE_FLAC_DECODER

  internal static partial class UnsafeNativeMethods {

    /// <summary>An enumeration of the available subframe types.</summary>
    internal enum FLAC__SubframeType : int {
      /// <summary>Constant signal</summary>
      FLAC__SUBFRAME_TYPE_CONSTANT = 0,
      /// <summary>Uncompressed signal</summary>
      FLAC__SUBFRAME_TYPE_VERBATIM = 1,
      /// <summary>Fixed polynomial prediction</summary>
      FLAC__SUBFRAME_TYPE_FIXED = 2,
      /// <summary>Linear prediction</summary>
      FLAC__SUBFRAME_TYPE_LPC = 3
    };

    /// <summary>CONSTANT subframe.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FLAC__Subframe_Constant {
      /// <summary>The constant signal value.</summary>
      public Int32 Value;
    };

    /// <summary>VERBATIM subframe.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FLAC__Subframe_Verbatim {
      /// <summary>A pointer to verbatim signal.</summary>
      public IntPtr Data; // int32 *
    }

    /// <summary>FIXED subframe.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FLAC__Subframe_Fixed {

      /// <summary>The residual coding method.</summary>
      public FLAC__EntropyCodingMethod entropy_coding_method;

      /// <summary>The polynomial order.</summary>
      public uint order;

      /// <summary>Warmup samples to prime the predictor, length == order.</summary>
      public Int32[] warmup; //[FLAC__MAX_FIXED_ORDER];

      /// <summary>
      ///   The residual signal, length == (blocksize minus order) samples.
      /// </summary>
      public IntPtr Residual; // int32 *

    };

    /// <summary>LPC subframe.</summary>
    internal struct FLAC__Subframe_LPC {

      /// <summary>The residual coding method.</summary>
      public FLAC__EntropyCodingMethod entropy_coding_method;

      /// <summary>The FIR order.</summary>
      public uint order;

      /// <summary>Quantized FIR filter coefficient precision in bits.</summary>
      public uint qlp_coeff_precision;

      /// <summary>The qlp coeff shift needed.</summary>
      public int quantization_level;

      /// <summary>FIR filter coefficients.</summary>
      public Int32[] qlp_coeff; // [FLAC__MAX_LPC_ORDER];

      /// <summary>Warmup samples to prime the predictor, length == order.</summary>
      public Int32[] warmup; // [FLAC__MAX_LPC_ORDER];

      /// <summary>
      ///   The residual signal, length == (blocksize minus order) samples.
      /// </summary>
      public IntPtr Residual; // int32 *

    };

    /// <summary>FLAC subframe structure.</summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct FLAC__Subframe {
      [FieldOffset(0)]
      public FLAC__SubframeType Type;
      [FieldOffset(4)]
      public FLAC__Subframe_Constant constant;
      [FieldOffset(4)]
      public FLAC__Subframe_Fixed Fixed;
      [FieldOffset(4)]
      public FLAC__Subframe_LPC lpc;
      [FieldOffset(4)]
      public FLAC__Subframe_Verbatim verbatim;
      [FieldOffset(8)]
      public uint wasted_bits;
    };

    /// <summary>FLAC frame header structure.</summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct FLAC__FrameHeader {

      /// <summary>The number of samples per subframe.</summary>
      [FieldOffset(0)]
      public uint blocksize;

      /// <summary>The sample rate in Hz.</summary>
      [FieldOffset(4)]
      public uint sample_rate;

      /// <summary>The number of channels (== number of subframes).</summary>
      [FieldOffset(8)]
      public uint channels;

      /// <summary>The channel assignment for the frame.</summary>
      [FieldOffset(12)]
      public FLAC__ChannelAssignment channel_assignment;

      /// <summary>The sample resolution.</summary>
      [FieldOffset(16)]
      public uint bits_per_sample;

      /// <summary>The numbering scheme used for the frame.</summary>
      /// <remarks>
      ///   As a convenience, the decoder will always convert a frame number to a sample number
      ///   because the rules are complex.
      /// </remarks>
      [FieldOffset(20)]
      public FLAC__FrameNumberType number_type;

      /// <summary>
      ///   The frame number or sample number of first sample in frame; use the \a number_type
      ///   value to determine which to use.
      /// </summary>
      [FieldOffset(24)]
      public UInt32 frame_number;
      /// <summary>
      ///   The frame number or sample number of first sample in frame; use the \a number_type
      ///   value to determine which to use.
      /// </summary>
      [FieldOffset(24)]
      public UInt64 sample_number;

      /// <summary>
      ///   CRC-8 (polynomial = x^8 + x^2 + x^1 + x^0, initialized with 0) of the raw frame
      ///   header bytes, meaning everything before the CRC byte including the sync code.
      /// </summary>
      [FieldOffset(32)]
      public byte crc;

    };

    /// <summary>FLAC frame footer structure.</summary>
    internal struct FLAC__FrameFooter {
      /// <summary>
      ///   CRC-16 (polynomial = x^16 + x^15 + x^2 + x^0, initialized with 0) of the bytes
      ///   before the crc, back to and including the frame header sync code.
      /// </summary>
      public UInt16 crc;

    };

    /// <summary>FLAC frame structure.</summary>
    internal struct FLAC__Frame {
      FLAC__FrameHeader header;
      //FLAC__Subframe subframes[FLAC__MAX_CHANNELS];
      FLAC__FrameFooter footer;
    }

    /// <summary>Contents of a Rice partitioned residual</summary>
    internal struct FLAC__EntropyCodingMethod_PartitionedRiceContents {

      /// <summary>The Rice parameters for each context.</summary>
      public uint[] parameters;

      /// <summary>Widths for escape-coded partitions.</summary>
      /// <remarks>
      ///   Will be non-zero for escaped partitions and zero for unescaped partitions.
      /// </remarks>
      public uint[] raw_bits;

      /// <summary>
      ///   The capacity of the \a parameters and \a raw_bits arrays specified as an order,
      ///   i.e. the number of array elements allocated is 2 ^ \a capacity_by_order.
      /// </summary>
      public uint capacity_by_order;

    }

    /// <summary>Header for a Rice partitioned residual.</summary>
    internal struct FLAC__EntropyCodingMethod_PartitionedRice {

      /// <summary>The partition order, i.e. # of contexts = 2 ^ \a order.</summary>
      public uint order;

      /// <summary>The context's Rice parameters and/or raw bits.</summary>
      public FLAC__EntropyCodingMethod_PartitionedRiceContents[] contents;

    }

    /// <summary>Header for the entropy coding method.</summary>
    internal struct FLAC__EntropyCodingMethod {
      public FLAC__EntropyCodingMethodType type;
      //union {
      public FLAC__EntropyCodingMethod_PartitionedRice partitioned_rice;
      //} data;
    }

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
