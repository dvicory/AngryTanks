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

  /// <summary>An enumeration of the available channel assignments</summary>
  public enum ChannelAssignment {

    /// <summary>Independent channels</summary>
    Independent = 0,

    /// <summary>Left+side stereo</summary>
    LeftAndSide = 1,

    /// <summary>Right+side stereo</summary>
    RightAndSide = 2,

    /// <summary>Mid+side stereo</summary>
    MidAndSide = 3

  }

  /// <summary>An enumeration of the available subframe types</summary>
  public enum SubframeType {

    /// <summary>Constant signal</summary>
    Constant = 0,

    /// <summary>Uncompressed signal</summary>
    Verbatim = 1,

    /// <summary>Fixed polynomial prediction</summary>
    Fixed = 2,

    /// <summary>Linear prediction</summary>
    Lpc = 3

  }

  /// <summary>FLAC frame header structure</summary>
  public struct FrameHeader {

    /// <summary>Number of samples per subframe</summary>
    public int Blocksize;

    /// <summary>Sample rate in Hz</summary>
    public int SampleRate;

    /// <summary>Number of channels (== number of subframes)</summary>
    /// <remarks>
    ///   <list type="table">
    ///     <item>
    ///       <term>1 channel</term>
    ///       <description>mono</description>
    ///     </item>
    ///     <item>
    ///       <term>2 channel</term>
    ///       <description>left, right</description>
    ///     </item>
    ///     <item>
    ///       <term>3 channel</term>
    ///       <description>left, right, center</description>
    ///     </item>
    ///     <item>
    ///       <term>4 channel</term>
    ///       <description>left, right, back left, back right</description>
    ///     </item>
    ///     <item>
    ///       <term>5 channel</term>
    ///       <description>
    ///         left, right, center, back/surround left, back/surround right
    ///       </description>
    ///     </item>
    ///     <item>
    ///       <term>6 channel</term>
    ///       <description>
    ///         left, right, center, LFE, back/surround left, back/surround right
    ///       </description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public int Channels;

    /// <summary>Channel assignment for the frame</summary>
    public ChannelAssignment ChannelAssignment;

    /// <summary>The sample resolution.</summary>
    public int BitsPerSample;

    /// <summary>Frame number of first sample in frame (if provided)</summary>
    /// <remarks>
    ///   Either a FrameNumber or a SampleNumber will be provided. The omitted element
    ///   will be set to -1.
    /// </remarks>
    public int FrameNumber;

    /// <summary>Sample number of first sample in frame (if provided)</summary>
    /// <remarks>
    ///   Either a FrameNumber or a SampleNumber will be provided. The omitted element
    ///   will be set to -1.
    /// </remarks>
    public long SampleNumber;

    /// <summary>CRC-8 of the raw frame header bytes</summary>
    /// <remarks>
    ///   Polynomial = x^8 + x^2 + x^1 + x^0, initialized with 0. Includes everything before
    ///   the CRC byte including the sync code.
    /// </remarks>
    public byte Crc;

  }

  /// <summary>FLAC frame footer structure</summary>
  public struct FrameFooter {

    /// <summary>CRC-16 of the bytes before the crc</summary>
    /// <remarks>
    ///   Polynomial = x^16 + x^15 + x^2 + x^0, initialized with 0. Goes back to and includes
    ///   the frame header sync code.
    /// </remarks>
    public UInt16 Crc;

  }

  /// <summary>FLAC frame structure</summary>
  public struct Frame {

    /// <summary>Header providing informations that describe the frame</summary>
    public FrameHeader Header;

    /// <summary>Subframes for the decoded data of the individual channels</summary>
    public Subframe[] Subframes;

    /// <summary>Footer containing data to verify the frame</summary>
    public FrameFooter Footer;

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
