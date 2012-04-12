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
using System.Runtime.InteropServices;

namespace Nuclex.Audio.Formats.Flac {

#if ENABLE_PINVOKE_FLAC_DECODER

  internal static class FrameMarshalHelper {

    /// <summary>
    ///   Marshals a frame from unmanaged memory into the equivalent structure
    /// </summary>
    /// <param name="frameHandle">Handle to the unmanaged memory containing the frame</param>
    /// <returns>The marshaled frame structure</returns>
    internal static Frame MarshalFrame(IntPtr frameHandle) {
      Frame frame;

      frame.Header = marshalFrameHeader(frameHandle);

      // Discovered via Visual C++ 2008 Express SP1. The structure is 33 bytes without
      // alignment. 40 is unexpected unless a 'char' following a 'long long' uses 8 bytes.
      int offset = 40;

      frame.Subframes = new Subframe[Constants.MaximumChannelCount];
      for(int channelIndex = 0; channelIndex < Constants.MaximumChannelCount; ++channelIndex) {
        frame.Subframes[channelIndex] = marshalSubframe(frameHandle, offset);

        // Again, discovered via Visual C++ 2008 Express SP1. Without alignment, the entries
        // should be 284 bytes apart each.
        offset += 292;
      }

      // At a maximum channel count of 8, we should now be 40 + 2336 = 2376 bytes away from
      // the address we started at

      frame.Footer.Crc = (ushort)Marshal.ReadInt16(frameHandle, offset);

      return frame;
    }

    /// <summary>
    ///   Converts an integer into a member of the ChannelAssignment enumeration
    /// </summary>
    /// <param name="channelAssignment">
    ///   Integer that will be converted into a member of the ChannelAssignment enumeration
    /// </param>
    /// <returns>
    ///   The matching member of the ChannelAssignment enumeration for the specified channel
    /// </returns>
    internal static ChannelAssignment ChannelAssignmentFromInt(int channelAssignment) {
      switch(channelAssignment) {
        case 0: { return ChannelAssignment.Independent; }
        case 1: { return ChannelAssignment.LeftAndSide; }
        case 2: { return ChannelAssignment.RightAndSide; }
        case 3: { return ChannelAssignment.MidAndSide; }
        default: { throw new ArgumentException("Invalid channel assignment"); }
      }
    }

    /// <summary>
    ///   Converts an integer into a member of the SubframeType enumeration
    /// </summary>
    /// <param name="subframeType">
    ///   Integer that will be converted into a member of the SubframeType enumeration
    /// </param>
    /// <returns>
    ///   The matching member of the SubframeType enumeration for the specified type
    /// </returns>
    internal static SubframeType SubframeTypeFromInt(int subframeType) {
      switch(subframeType) {
        case 0: { return SubframeType.Constant; }
        case 1: { return SubframeType.Verbatim; }
        case 2: { return SubframeType.Fixed; }
        case 3: { return SubframeType.Lpc; }
        default: { throw new ArgumentException("Invalid subframe type"); }
      }
    }

    /// <summary>
    ///   Converts an integer into a member of the EntropyCodingMethodType enumeration
    /// </summary>
    /// <param name="entropyMethodCodingType">
    ///   Integer that will be converted into a member of
    ///   the EntropyCodingMethodType enumeration
    /// </param>
    /// <returns>
    ///   The matching member of the EntropyCodingMethodType enumeration for the
    ///   specified type
    /// </returns>
    internal static EntropyCodingMethodType EntropyCodingMethodTypeFromInt(
      int entropyMethodCodingType
    ) {
      switch(entropyMethodCodingType) {
        case 0: { return EntropyCodingMethodType.PartitionedRice; }
        case 1: { return EntropyCodingMethodType.PartitionedRice2; }
        default: { throw new ArgumentException("Invalid entropy coding method type"); }
      }
    }
    
    /// <summary>
    ///   Marshals the header of a frame from unmanaged memory into the equivalent structure
    /// </summary>
    /// <param name="frameHandle">Handle to unmanaged memory containing the frame</param>
    /// <returns>The marshaled frame header</returns>
    private static FrameHeader marshalFrameHeader(IntPtr frameHandle) {
      FrameHeader header;

      // The beginning of the structure is straightforward to marshal
      header.Blocksize = Marshal.ReadInt32(frameHandle, 0);
      header.SampleRate = Marshal.ReadInt32(frameHandle, 4);
      header.Channels = Marshal.ReadInt32(frameHandle, 8);
      header.ChannelAssignment = ChannelAssignmentFromInt(Marshal.ReadInt32(frameHandle, 12));
      header.BitsPerSample = Marshal.ReadInt32(frameHandle, 16);

      // Frame and sample number are actually a union. Which one is provided can be
      // determined by looking at the number_type field. We resolve this union here in
      // order to avoid explicit memory offsets in our managed structures
      int numberType = Marshal.ReadInt32(frameHandle, 20);
      switch(numberType) {
        case 0: {
          header.FrameNumber = Marshal.ReadInt32(frameHandle, 24);
          header.SampleNumber = -1;
          break;
        }
        case 1: {
          header.FrameNumber = -1;
          header.SampleNumber = Marshal.ReadInt64(frameHandle, 24);
          break;
        }
        default: {
          throw new ArgumentException("Invalid number type for the frame/sample number");
        }
      }

      // Finally, an 8 bit CRC follows the frame header
      header.Crc = Marshal.ReadByte(frameHandle, 32);

      return header;
    }

    /// <summary>Marhals a FLAC subframe</summary>
    /// <param name="frameHandle">Handle to unmanaged memory containing the frame</param>
    /// <param name="offset">Offset at which to begin reading this subframe</param>
    /// <returns>The marshaled subframe</returns>
    private static Subframe marshalSubframe(IntPtr frameHandle, int offset) {
      Subframe subframe;

      subframe.Type = SubframeTypeFromInt(Marshal.ReadInt32(frameHandle, offset));

      switch(subframe.Type) {
        case SubframeType.Constant: {
          subframe.Constant = marshalConstantSubframe(frameHandle, offset + 4);
          subframe.Fixed = null;
          subframe.Lpc = null;
          subframe.Verbatim = null;
          break;
        }
        case SubframeType.Fixed: {
          subframe.Constant = null;
          subframe.Fixed = marshalFixedSubframe(frameHandle, offset + 4);
          subframe.Lpc = null;
          subframe.Verbatim = null;
          break;
        }
        case SubframeType.Lpc: {
          subframe.Constant = null;
          subframe.Fixed = null;
          subframe.Lpc = marshalLpcSubframe(frameHandle, offset + 4);
          subframe.Verbatim = null;
          break;
        }
        case SubframeType.Verbatim: {
          subframe.Constant = null;
          subframe.Fixed = null;
          subframe.Lpc = null;
          subframe.Verbatim = marshalVerbatimSubframe(frameHandle, offset + 4);
          break;
        }
        default: {
          throw new ArgumentException("Invalid subframe type");
        }
      }

      subframe.WastedBits = Marshal.ReadInt32(frameHandle, offset + 288);

      return subframe;
    }

    /// <summary>Marhals a FLAC constant subframe</summary>
    /// <param name="frameHandle">Handle to unmanaged memory containing the frame</param>
    /// <param name="offset">Offset at which to begin reading this subframe</param>
    /// <returns>The marshaled constant subframe</returns>
    private static ConstantSubframe marshalConstantSubframe(IntPtr frameHandle, int offset) {
      ConstantSubframe constantSubframe;

      constantSubframe.Value = Marshal.ReadInt32(frameHandle, offset);

      return constantSubframe;
    }

    /// <summary>Marshals a FLAC fixed subframe</summary>
    /// <param name="frameHandle">Handle to unmanaged memory containing the frame</param>
    /// <param name="offset">Offset at which to begin reading this subframe</param>
    /// <returns>The marshaled constant subframe</returns>
    private static FixedSubframe marshalFixedSubframe(IntPtr frameHandle, int offset) {
      FixedSubframe fixedSubframe;

      fixedSubframe.EntropyCodingMethod = marshalEntropyCodingMethod(frameHandle, offset);
      fixedSubframe.Order = Marshal.ReadInt32(frameHandle, offset + 12);
      fixedSubframe.Warmup = new int[Constants.MaximumFixedOrder];
      for(int warmupIndex = 0; warmupIndex < Constants.MaximumFixedOrder; ++warmupIndex) {
        fixedSubframe.Warmup[warmupIndex] = Marshal.ReadInt32(
          frameHandle, offset + warmupIndex * 4 + 16
        );
      }

      fixedSubframe.Residual = null;

      return fixedSubframe;
    }

    /// <summary>Marshals a FLAC LPC subframe</summary>
    /// <param name="frameHandle">Handle to unmanaged memory containing the frame</param>
    /// <param name="offset">Offset at which to begin reading this subframe</param>
    /// <returns>The marshaled LPC subframe</returns>
    private static LpcSubframe marshalLpcSubframe(IntPtr frameHandle, int offset) {
      LpcSubframe lpcSubframe = new LpcSubframe();

      lpcSubframe.EntropyCodingMethod = marshalEntropyCodingMethod(frameHandle, offset);
      lpcSubframe.Order = Marshal.ReadInt32(frameHandle, offset + 12);
      lpcSubframe.QlpCoefficientPrecision = Marshal.ReadInt32(frameHandle, offset + 16);
      lpcSubframe.QuantizationLevel = Marshal.ReadInt32(frameHandle, offset + 20);

      offset += 24;

      // Marshal the array of QLP Coefficients
      lpcSubframe.QlpCoefficients = new int[Constants.MaximumLpcOrder];
      for(
        int index = 0; index < Constants.MaximumLpcOrder; ++index
      ) {
        lpcSubframe.QlpCoefficients[index] = Marshal.ReadInt32(
          frameHandle, offset + index * 4
        );
      }

      offset += Constants.MaximumLpcOrder * 4;

      // Marshal the Warmup values for the LPC decoder
      lpcSubframe.Warmup = new int[Constants.MaximumLpcOrder];
      for(
        int index = 0; index < Constants.MaximumLpcOrder; ++index
      ) {
        lpcSubframe.Warmup[index] = Marshal.ReadInt32(
          frameHandle, offset + index * 4
        );
      }

      offset += Constants.MaximumLpcOrder * 4;

      lpcSubframe.Residual = null;

      return lpcSubframe;
    }

    /// <summary>Marshals a FLAC verbatim subframe</summary>
    /// <param name="frameHandle">Handle to unmanaged memory containing the frame</param>
    /// <param name="offset">Offset at which to begin reading this subframe</param>
    /// <returns>The marshaled verbatim subframe</returns>
    private static VerbatimSubframe marshalVerbatimSubframe(IntPtr frameHandle, int offset) {
      VerbatimSubframe verbatimSubframe = new VerbatimSubframe();
      return verbatimSubframe;
    }

    /// <summary>Marshals a FLAC entropy coding method</summary>
    /// <param name="frameHandle">
    ///   Handle to unmanaged memory containing the entropy coding method
    /// </param>
    /// <param name="offset">
    ///   Offset at which to begin reading this entropy coding method
    /// </param>
    /// <returns>The marshaled entropy coding method</returns>
    private static EntropyCodingMethod marshalEntropyCodingMethod(
      IntPtr frameHandle, int offset
    ) {
      EntropyCodingMethod entropyCodingMethod = new EntropyCodingMethod();

      entropyCodingMethod.Type = EntropyCodingMethodTypeFromInt(
        Marshal.ReadInt32(frameHandle, offset)
      );
      switch(entropyCodingMethod.Type) {

        // Rice Partition Coding scheme 1
        case EntropyCodingMethodType.PartitionedRice: {
          entropyCodingMethod.PartitionedRice.Order = Marshal.ReadInt32(
            frameHandle, offset + 4
          );

          IntPtr contentsAddress = Marshal.ReadIntPtr(frameHandle, offset + 8);

          entropyCodingMethod.PartitionedRice.Contents.Parameters = null;
          entropyCodingMethod.PartitionedRice.Contents.RawBits = null;
          entropyCodingMethod.PartitionedRice.Contents.CapacityByOrder = Marshal.ReadInt32(
            contentsAddress, 8
          );
          break;
        }

        // Rice Partition Coding scheme 2
        case EntropyCodingMethodType.PartitionedRice2: {
          entropyCodingMethod.PartitionedRice.Order = Marshal.ReadInt32(
            frameHandle, offset + 4
          );

          IntPtr contentsAddress = Marshal.ReadIntPtr(frameHandle, offset + 8);

          entropyCodingMethod.PartitionedRice.Contents.Parameters = null;
          entropyCodingMethod.PartitionedRice.Contents.RawBits = null;
          entropyCodingMethod.PartitionedRice.Contents.CapacityByOrder = Marshal.ReadInt32(
            contentsAddress, 8
          );
          break;
        }

      }

      return entropyCodingMethod;
    }

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
