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

  /// <summary>Imported native methods</summary>
  internal static partial class UnsafeNativeMethods {

    /// <summary>Return values for the FLAC__StreamDecoder read callback</summary>
    internal enum FLAC__StreamDecoderReadStatus : int {

      /// <summary>The read was OK and decoding can continue.</summary>
      FLAC__STREAM_DECODER_READ_STATUS_CONTINUE,

      /// <summary>The read was attempted while at the end of the stream.</summary>
      /// <remarks>
      ///   Note that the client must only return this value when the read callback was called
      ///   when already at the end of the stream. Otherwise, if the read itself moves to the
      ///   end of the stream, the client should still return the data and
      ///   \c FLAC__STREAM_DECODER_READ_STATUS_CONTINUE, and then on the next read callback it
      ///   should return \c FLAC__STREAM_DECODER_READ_STATUS_END_OF_STREAM with a byte count
      ///   of \c 0.
      /// </remarks>
      FLAC__STREAM_DECODER_READ_STATUS_END_OF_STREAM,

      /// <summary>
      ///   An unrecoverable error occurred. The decoder will return from the process call.
      /// </summary>
      FLAC__STREAM_DECODER_READ_STATUS_ABORT

    }

    /// <summary>Return values for the FLAC__StreamDecoder seek callback.</summary>
    internal enum FLAC__StreamDecoderSeekStatus : int {

      /// <summary>The seek was OK and decoding can continue.</summary>
      FLAC__STREAM_DECODER_SEEK_STATUS_OK,

      /// <summary>
      ///   An unrecoverable error occurred.  The decoder will return from the process call.
      /// </summary>
      FLAC__STREAM_DECODER_SEEK_STATUS_ERROR,

      /// <summary>Client does not support seeking.</summary>
      FLAC__STREAM_DECODER_SEEK_STATUS_UNSUPPORTED

    }

    /// <summary>Return values for the FLAC__StreamDecoder tell callback.</summary>
    internal enum FLAC__StreamDecoderTellStatus : int {

      /// <summary>The tell was OK and decoding can continue.</summary>
      FLAC__STREAM_DECODER_TELL_STATUS_OK,

      /// <summary>
      ///   An unrecoverable error occurred.  The decoder will return from the process call.
      /// </summary>
      FLAC__STREAM_DECODER_TELL_STATUS_ERROR,

      /// <summary>Client does not support telling the position.</summary>
      FLAC__STREAM_DECODER_TELL_STATUS_UNSUPPORTED

    }

    /// <summary>Return values for the FLAC__StreamDecoder length callback.</summary>
    internal enum FLAC__StreamDecoderLengthStatus : int {

      /// <summary>The length call was OK and decoding can continue.</summary>
      FLAC__STREAM_DECODER_LENGTH_STATUS_OK,

      /// <summary>
      ///   An unrecoverable error occurred. The decoder will return from the process call.
      /// </summary>
      FLAC__STREAM_DECODER_LENGTH_STATUS_ERROR,

      /// <summary>Client does not support reporting the length.</summary>
      FLAC__STREAM_DECODER_LENGTH_STATUS_UNSUPPORTED

    }

    /// <summary>Return values for the FLAC__StreamDecoder write callback.</summary>
    internal enum FLAC__StreamDecoderWriteStatus : int {

      /// <summary>The write was OK and decoding can continue.</summary>
      FLAC__STREAM_DECODER_WRITE_STATUS_CONTINUE,

      /// <summary>
      ///   An unrecoverable error occurred. The decoder will return from the process call.
      /// </summary>
      FLAC__STREAM_DECODER_WRITE_STATUS_ABORT

    }

    /// <summary>
    ///   Possible values passed back to the FLAC__StreamDecoder error callback
    /// </summary>
    /// <remarks>
    ///   \c FLAC__STREAM_DECODER_ERROR_STATUS_LOST_SYNC is the generic catch-all. The rest
    ///   could be caused by bad sync (false synchronization on data that is not the start of
    ///   a frame) or corrupted data. The error itself is the decoder's best guess at what
    ///   happened assuming a correct sync. For example
    ///   \c FLAC__STREAM_DECODER_ERROR_STATUS_BAD_HEADER could be caused by a correct sync on
    ///   the start of a frame, but some data in the frame header was corrupted. Or it could be
    ///   the result of syncing on a point the stream that looked like the starting of a frame
    ///   but was not. \c FLAC__STREAM_DECODER_ERROR_STATUS_UNPARSEABLE_STREAM could be because
    ///   the decoder encountered a valid frame made by a future version of the encoder which
    ///   it cannot parse, or because of a false sync making it appear as though an encountered
    ///   frame was generated by a future encoder.
    /// </remarks>
    internal enum FLAC__StreamDecoderErrorStatus : int {

      /// <summary>
      ///   An error in the stream caused the decoder to lose synchronization.
      /// </summary>
      FLAC__STREAM_DECODER_ERROR_STATUS_LOST_SYNC,

      /// <summary>The decoder encountered a corrupted frame header.</summary>
      FLAC__STREAM_DECODER_ERROR_STATUS_BAD_HEADER,

      /// <summary>The frame's data did not match the CRC in the footer.</summary>
      FLAC__STREAM_DECODER_ERROR_STATUS_FRAME_CRC_MISMATCH,

      /// <summary>The decoder encountered reserved fields in use in the stream.</summary>
      FLAC__STREAM_DECODER_ERROR_STATUS_UNPARSEABLE_STREAM

    }

    /// <summary>
    ///   Possible return values for the FLAC__stream_decoder_init_*() functions.
    /// </summary>
    internal enum FLAC__StreamDecoderInitStatus : int {

      /// <summary>Initialization was successful.</summary>
      FLAC__STREAM_DECODER_INIT_STATUS_OK = 0,

      /// <summary>
      ///   The library was not compiled with support for the given container format.
      /// </summary>
      FLAC__STREAM_DECODER_INIT_STATUS_UNSUPPORTED_CONTAINER,

      /// <summary>A required callback was not supplied.</summary>
      FLAC__STREAM_DECODER_INIT_STATUS_INVALID_CALLBACKS,

      /// <summary>An error occurred allocating memory.</summary>
      FLAC__STREAM_DECODER_INIT_STATUS_MEMORY_ALLOCATION_ERROR,

      /// <summary>
      ///   fopen() failed in FLAC__stream_decoder_init_file() or
      ///   FLAC__stream_decoder_init_ogg_file().
      /// </summary>
      FLAC__STREAM_DECODER_INIT_STATUS_ERROR_OPENING_FILE,

      /// <summary>
      ///   FLAC__stream_decoder_init_*() was called when the decoder was already initialized,
      ///   usually because FLAC__stream_decoder_finish() was not called.
      /// </summary>
      FLAC__STREAM_DECODER_INIT_STATUS_ALREADY_INITIALIZED

    }

    /// <summary>An enumeration of the available channel assignments.</summary>
    internal enum FLAC__ChannelAssignment : int {

      /// <summary>Independent channels</summary>
      FLAC__CHANNEL_ASSIGNMENT_INDEPENDENT = 0,
      /// <summary>Left+side stereo</summary>
      FLAC__CHANNEL_ASSIGNMENT_LEFT_SIDE = 1,
      /// <summary>Right+side stereo</summary>
      FLAC__CHANNEL_ASSIGNMENT_RIGHT_SIDE = 2,
      /// <summary>Mid+side stereo</summary>
      FLAC__CHANNEL_ASSIGNMENT_MID_SIDE = 3

    }

    /// <summary>An enumeration of the possible frame numbering methods.</summary>
    internal enum FLAC__FrameNumberType : int {

      /// <summary>Number contains the frame number</summary>
      FLAC__FRAME_NUMBER_TYPE_FRAME_NUMBER,
      /// <summary>Number contains the sample number of first sample in frame</summary>
      FLAC__FRAME_NUMBER_TYPE_SAMPLE_NUMBER

    }

    /// <summary>An enumeration of the available entropy coding methods.</summary>
    internal enum FLAC__EntropyCodingMethodType : int {

      /// <summary>
      ///   Residual is coded by partitioning into contexts, each with it's own 4-bit
      ///   Rice parameter.
      /// </summary>
      FLAC__ENTROPY_CODING_METHOD_PARTITIONED_RICE = 0,

      /// <summary>
      ///   Residual is coded by partitioning into contexts, each with it's own 5-bit
      ///   Rice parameter.
      /// </summary>
      FLAC__ENTROPY_CODING_METHOD_PARTITIONED_RICE2 = 1

    }

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
