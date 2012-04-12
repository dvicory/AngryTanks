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

    /// <summary>Signature for the read callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="buffer">
    ///   A pointer to a location for the callee to store data to be decoded.
    /// </param>
    /// <param name="bytes">
    ///   A pointer to the size of the buffer. On entry to the callback, it contains the maximum
    ///   number of bytes that may be stored in \a buffer. The callee must set it to the actual
    ///   number of bytes stored (0 in case of error or end-of-stream) before returning.
    /// </param>
    /// <param name="clientData">
    ///   The callee's client data set through FLAC__stream_decoder_init_*()
    /// </param>
    /// <returns>
    ///   The callee's return status. Note that the callback should return
    ///   \c FLAC__STREAM_DECODER_READ_STATUS_END_OF_STREAM if and only if zero bytes were read
    ///   and there is no more data to be read.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     A function pointer matching this signature must be passed to
    ///     FLAC__stream_decoder_init*_stream(). The supplied function will be called when the
    ///     decoder needs more input data. The address of the buffer to be filled is supplied,
    ///     along with the number of bytes the buffer can hold. The callback may choose to
    ///     supply less data and modify the byte count but must be careful not to overflow
    ///     the buffer. The callback then returns a status code chosen from
    ///     FLAC__StreamDecoderReadStatus.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate FLAC__StreamDecoderReadStatus FLAC__StreamDecoderReadCallback(
      IntPtr decoder,
      IntPtr buffer,
      ref int bytes,
      IntPtr clientData
    );

    /// <summary>Signature for the seek callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="absolute_byte_offset">
    ///   The offset from the beginning of the stream to seek to.
    /// </param>
    /// <param name="client_data">
    ///   The callee's client data set through FLAC__stream_decoder_init_*().
    /// </param>
    /// <returns>The callee's return status.</returns>
    /// <remarks>
    ///   <para>
    ///      A function pointer matching this signature may be passed to
    ///      FLAC__stream_decoder_init*_stream().  The supplied function will be called when
    ///      the decoder needs to seek the input stream. The decoder will pass the absolute
    ///      byte offset to seek to, 0 meaning the beginning of the stream.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate FLAC__StreamDecoderSeekStatus FLAC__StreamDecoderSeekCallback(
      IntPtr decoder,
      UInt64 absolute_byte_offset,
      IntPtr client_data
    );

    /// <summary>Signature for the tell callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="absolute_byte_offset">
    ///   A pointer to storage for the current offset from the beginning of the stream.
    /// </param>
    /// <param name="client_data">
    ///   The callee's client data set through FLAC__stream_decoder_init_*().
    /// </param>
    /// <returns>The callee's return status.</returns>
    /// <remarks>
    ///   <para>
    ///     A function pointer matching this signature may be passed to
    ///     FLAC__stream_decoder_init*_stream(). The supplied function will be called when
    ///     the decoder wants to know the current position of the stream. The callback should
    ///     return the byte offset from the beginning of the stream.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate FLAC__StreamDecoderTellStatus FLAC__StreamDecoderTellCallback(
      IntPtr decoder,
      ref UInt64 absolute_byte_offset,
      IntPtr client_data
    );

    /// <summary>Signature for the length callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="stream_length">
    ///   A pointer to storage for the length of the stream in bytes.
    /// </param>
    /// <param name="client_data">
    ///   The callee's client data set through FLAC__stream_decoder_init_*().
    /// </param>
    /// <returns>The callee's return status.</returns>
    /// <remarks>
    ///   <para>
    ///     A function pointer matching this signature may be passed to
    ///     FLAC__stream_decoder_init*_stream().  The supplied function will be called when
    ///     the decoder wants to know the total length of the stream in bytes.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate FLAC__StreamDecoderLengthStatus FLAC__StreamDecoderLengthCallback(
      IntPtr decoder,
      ref UInt64 stream_length,
      IntPtr client_data
    );

    /// <summary>Signature for the EOF callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="client_data">
    ///   The callee's client data set through FLAC__stream_decoder_init_*().
    /// </param>
    /// <returns>\c true if the currently at the end of the stream, else \c false.</returns>
    /// <remarks>
    ///   <para>
    ///     A function pointer matching this signature may be passed to
    ///     FLAC__stream_decoder_init*_stream().  The supplied function will be called when
    ///     the decoder needs to know if the end of the stream has been reached.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int FLAC__StreamDecoderEofCallback(
      IntPtr decoder,
      IntPtr client_data
    );

    /// <summary>Signature for the write callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="frame">
    ///   The description of the decoded frame. See FLAC__Frame.
    /// </param>
    /// <param name="buffer">
    ///   An array of pointers to decoded channels of data. Each pointer will point to an
    ///   array of signed samples of length \a frame->header.blocksize. Channels will be ordered
    ///   according to the FLAC specification; see the documentation for the frame header.
    /// </param>
    /// <param name="client_data">
    ///   The callee's client data set through FLAC__stream_decoder_init_*().
    /// </param>
    /// <returns>The callee's return status.</returns>
    /// <remarks>
    ///   <para>
    ///     A function pointer matching this signature must be passed to one of the
    ///     FLAC__stream_decoder_init_*() functions. The supplied function will be called when
    ///     the decoder has decoded a single audio frame. The decoder will pass the frame
    ///     metadata as well as an array of pointers (one for each channel) pointing to the
    ///     decoded audio.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate FLAC__StreamDecoderWriteStatus FLAC__StreamDecoderWriteCallback(
      IntPtr decoder,
      IntPtr frame,
      IntPtr[] buffer,
      IntPtr client_data
    );

    /// <summary>Signature for the metadata callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="metadata">The decoded metadata block.</param>
    /// <param name="client_data">
    ///   The callee's client data set through FLAC__stream_decoder_init_*().
    /// </param>
    /// <remarks>
    ///   <para>
    ///     A function pointer matching this signature must be passed to one of the
    ///     FLAC__stream_decoder_init_*() functions. The supplied function will be called when
    ///     the decoder has decoded a metadata block. In a valid FLAC file there will always be
    ///     one \c STREAMINFO block, followed by zero or more other metadata blocks. These will
    ///     be supplied by the decoder in the same order as they appear in the stream and
    ///     always before the first audio frame (i.e. write callback). The metadata block that
    ///     is passed in must not be modified, and it doesn't live beyond the callback, so you
    ///     should make a copy of it with FLAC__metadata_object_clone() if you will need it
    ///     elsewhere. Since metadata blocks can potentially be large, by default the decoder
    ///     only calls the metadata callback for the \c STREAMINFO block; you can instruct the
    ///     decoder to pass or filter other blocks with FLAC__stream_decoder_set_metadata_*()
    ///     calls.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void FLAC__StreamDecoderMetadataCallback(
      IntPtr decoder,
      IntPtr metadata,
      IntPtr client_data
    );

    /// <summary>Signature for the error callback.</summary>
    /// <param name="decoder">The decoder instance calling the callback.</param>
    /// <param name="status">The error encountered by the decoder.</param>
    /// <param name="client_data">
    ///   The callee's client data set through FLAC__stream_decoder_init_*().
    /// </param>
    /// <remarks>
    ///   <para>
    ///     A function pointer matching this signature must be passed to one of the
    ///     FLAC__stream_decoder_init_*() functions. The supplied function will be called
    ///     whenever an error occurs during decoding.
    ///   </para>
    ///   <para>
    ///     In general, FLAC__StreamDecoder functions which change the state should not be
    ///     called on the \a decoder while in the callback.
    ///   </para>
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void FLAC__StreamDecoderErrorCallback(
      IntPtr decoder,
      FLAC__StreamDecoderErrorStatus status,
      IntPtr client_data
    );

    /// <summary>Get the "MD5 signature checking" flag</summary>
    /// <param name="decoder">A decoder instance to query.</param>
    /// <returns>See remarks</returns>
    /// <remarks>
    ///   This is the value of the setting, not whether or not the decoder is currently checking
    ///   the MD5 (remember, it can be turned off automatically by a seek). When the decoder is
    ///   reset the flag will be restored to the value returned by this function.
    /// </remarks>
    [DllImport("flac.x86.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int FLAC__stream_decoder_get_md5_checking(IntPtr decoder);

    /// <summary>Sets the "MD5 signature checking" flag</summary>
    /// <param name="decoder">A decoder instance to set</param>
    /// <param name="value">Flag value (see remarks)</param>
    /// <returns>\c false if the decoder is already initialized, else \c true.</returns>
    /// <remarks>
    ///   <para>
    ///   If \c true, the decoder will compute the MD5 signature of the unencoded audio data
    ///   while decoding and compare it to the signature from the STREAMINFO block, if it
    ///   exists, during FLAC__stream_decoder_finish().
    ///   </para>
    ///   <para>
    ///     MD5 signature checking will be turned off (until the next
    ///     FLAC__stream_decoder_reset()) if there is no signature in the STREAMINFO block or
    ///     when a seek is attempted.
    ///   </para>
    ///   <para>
    ///     Clients that do not use the MD5 check should leave this off to speed up decoding.
    ///   </para>
    /// </remarks>
    [DllImport("flac.x86.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int FLAC__stream_decoder_set_md5_checking(IntPtr decoder, int value);

    /// <summary>Create a new stream decoder instance</summary>
    /// <returns>
    ///   \c NULL if there was an error allocating memory, else the new instance
    /// </returns>
    /// <remarks>
    ///   The instance is created with default settings; see the individual
    ///   FLAC__stream_decoder_set_*() functions for each setting's default.
    /// </remarks>
    [DllImport("flac.x86.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr FLAC__stream_decoder_new();

    /// <summary>Free a decoder instance</summary>
    /// <param name="decoder">A pointer to an existing decoder</param>
    /// <remarks>
    ///   Deletes the object pointed to by \a decoder.
    /// </remarks>
    [DllImport("flac.x86.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void FLAC__stream_decoder_delete(IntPtr decoder);

    /// <summary>Initialize the decoder instance to decode native FLAC streams</summary>
    /// <param name="decoder">An uninitialized decoder instance</param>
    /// <param name="read_callback">
    ///   See FLAC__StreamDecoderReadCallback. This pointer must not be \c NULL.
    /// </param>
    /// <param name="seek_callback">
    ///   See FLAC__StreamDecoderSeekCallback. This pointer may be \c NULL if seeking is not
    ///   supported. If \a seek_callback is not \c NULL then a \a tell_callback, \a
    ///   length_callback, and \a eof_callback must also be supplied. Alternatively, a dummy
    ///   seek callback that just returns \c FLAC__STREAM_DECODER_SEEK_STATUS_UNSUPPORTED
    ///   may also be supplied, all though this is slightly less efficient for the decoder. 
    /// </param>
    /// <param name="tell_callback">
    ///   See FLAC__StreamDecoderTellCallback. This pointer may be \c NULL if not supported
    ///   by the client. If \a seek_callback is not \c NULL then a \a tell_callback must also
    ///   be supplied. Alternatively, a dummy tell callback that just returns
    ///   \c FLAC__STREAM_DECODER_TELL_STATUS_UNSUPPORTED may also be supplied, all though this
    ///   is slightly less efficient for the decoder.
    /// </param>
    /// <param name="length_callback">
    ///   See FLAC__StreamDecoderLengthCallback.  This pointer may be \c NULL if not supported
    ///   by the client. If \a seek_callback is not \c NULL then a \a length_callback must also
    ///   be supplied. Alternatively, a dummy length callback that just returns
    ///   \c FLAC__STREAM_DECODER_LENGTH_STATUS_UNSUPPORTED may also be supplied, all though
    ///   this is slightly less efficient for the decoder.
    /// </param>
    /// <param name="eof_callback">
    ///   See FLAC__StreamDecoderEofCallback. This pointer may be \c NULL if not supported by
    ///   the client. If \a seek_callback is not \c NULL then a \a eof_callback must also be
    ///   supplied. Alternatively, a dummy length callback that just returns \c false may also
    ///   be supplied, all though this is slightly less efficient for the decoder.
    /// </param>
    /// <param name="write_callback">
    ///   See FLAC__StreamDecoderWriteCallback. This pointer must not be \c NULL.
    /// </param>
    /// <param name="metadata_callback">
    ///   See FLAC__StreamDecoderMetadataCallback. This pointer may be \c NULL if the callback
    ///   is not desired.
    /// </param>
    /// <param name="error_callback">
    ///   See FLAC__StreamDecoderErrorCallback. This pointer must not be \c NULL.
    /// </param>
    /// <param name="client_data">
    ///   This value will be supplied to callbacks in their \a client_data argument.
    /// </param>
    /// <returns>
    ///   \c FLAC__STREAM_DECODER_INIT_STATUS_OK if initialization was successful; see
    ///   FLAC__StreamDecoderInitStatus for the meanings of other return values.
    /// </returns>
    [DllImport("flac.x86.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int FLAC__stream_decoder_init_stream(
      IntPtr decoder,
      FLAC__StreamDecoderReadCallback read_callback,
      FLAC__StreamDecoderSeekCallback seek_callback,
      FLAC__StreamDecoderTellCallback tell_callback,
      FLAC__StreamDecoderLengthCallback length_callback,
      FLAC__StreamDecoderEofCallback eof_callback,
      FLAC__StreamDecoderWriteCallback write_callback,
      FLAC__StreamDecoderMetadataCallback metadata_callback,
      FLAC__StreamDecoderErrorCallback error_callback,
      IntPtr client_data
    );

    /// <summary>Decode until the end of the stream.</summary>
    /// <param name="decoder">An initialized decoder instance.</param>
    /// <returns>
    ///   \c false if any fatal read, write, or memory allocation error occurred (meaning
    ///   decoding must stop), else \c true; for more information about the decoder, check
    ///   the decoder state with FLAC__stream_decoder_get_state().
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This version instructs the decoder to decode from the current position and continue
    ///     until the end of stream (the read callback returns
    ///     \c FLAC__STREAM_DECODER_READ_STATUS_END_OF_STREAM), or until the callbacks return
    ///     a fatal error.
    ///   </para>
    ///   <para>
    ///     As the decoder needs more input it will call the read callback. As each metadata
    ///     block and frame is decoded, the metadata or write callback will be called with
    ///     the decoded metadata or frame.
    ///   </para>
    /// </remarks>
    [DllImport("flac.x86.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int FLAC__stream_decoder_process_until_end_of_stream(
      IntPtr decoder
    );

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
