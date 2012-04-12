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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Nuclex.Audio.Formats.Flac {

#if ENABLE_PINVOKE_FLAC_DECODER

  /// <summary>Errors that can occur when a FLAC file is decoded</summary>
  public enum StreamDecoderError {

    /// <summary>
    ///   An error in the stream caused the decoder to lose synchronization.
    /// </summary>
    LostSync,

    /// <summary>The decoder encountered a corrupted frame header.</summary>
    BadHeader,

    /// <summary>The frame's data did not match the CRC in the footer.</summary>
    FrameCrcMismatch,

    /// <summary>The decoder encountered reserved fields in use in the stream.</summary>
    UnparseableStream

  };

  /// <summary>Adapter that wraps a managed stream into a FLAC stream</summary>
  public abstract class FlacStream {

    /// <summary>Initializes a new FLAC stream adapter</summary>
    protected FlacStream() {
      this.ReadCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderReadCallback(
        readCallback
      );
      this.SeekCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderSeekCallback(
        seekCallback
      );
      this.TellCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderTellCallback(
        tellCallback
      );
      this.LengthCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderLengthCallback(
        lengthCallback
      );
      this.EofCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderEofCallback(
        eofCallback
      );
      this.WriteCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderWriteCallback(
        writeCallback
      );
      this.MetadataCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderMetadataCallback(
        metadataCallback
      );
      this.ErrorCallbackDelegate = new UnsafeNativeMethods.FLAC__StreamDecoderErrorCallback(
        errorCallback
      );
    }

    /// <summary>
    ///   Reads data from the stream at the current position of the file pointer
    /// </summary>
    /// <param name="buffer">Buffer to store the read bytes in</param>
    /// <param name="byteCount">Number of bytes to read at most</param>
    /// <returns>
    ///   The number of bytes that were actually read and stored in the buffer
    /// </returns>
    protected abstract int Read(byte[] buffer, int byteCount);

    /// <summary>Moves the file pointer within the stream</summary>
    /// <param name="offset">New offset to move the file pointer to</param>
    protected abstract void Seek(long offset);

    /// <summary>Retrieves the current position of the file pointer in the stream</summary>
    /// <returns>The file pointer's current position</returns>
    protected abstract long Position { get; }

    /// <summary>Total length of the stream in bytes</summary>
    protected abstract long Length { get; }

    /// <summary>Determines whether the end of the stream has been reached</summary>
    protected abstract bool IsEndOfStreamReached { get; }

    /// <summary>Writes decoded data into the stream</summary>
    /// <param name="frame">Informations about the decoded frame</param>
    /// <param name="buffers">Buffers containing the decoded data of each channel</param>
    protected abstract void Write(int frame, byte[][] buffers);

    /// <summary>Processes decoded metadata informations</summary>
    /// <param name="metadata">Decoded metadata</param>
    protected abstract void ProcessMetadata(int metadata);

    /// <summary>Informs the stream that an error has occured decoding the stream</summary>
    /// <param name="error">Error that has occured</param>
    protected abstract void HandleError(StreamDecoderError error);

    /// <summary>
    ///   Reads data from the stream at the current position of the file pointer
    /// </summary>
    /// <param name="decoder">FLAC stream decoder issuing the read request</param>
    /// <param name="bufferHandle">Buffer to store the read bytes in</param>
    /// <param name="byteCount">
    ///   Number of bytes to read at most and, upon return, contains the number
    ///   of bytes that were actually read
    /// </param>
    /// <param name="clientData">Not used</param>
    /// <returns>Whether the read request was completed successfully</returns>
    private UnsafeNativeMethods.FLAC__StreamDecoderReadStatus readCallback(
      IntPtr decoder,
      IntPtr bufferHandle,
      ref int byteCount,
      IntPtr clientData
    ) {
      try {
        byte[] buffer = new byte[byteCount];
        byteCount = Read(buffer, byteCount);
        Marshal.Copy(buffer, 0, bufferHandle, byteCount);
      }
      catch(EndOfStreamException) {
        return UnsafeNativeMethods.FLAC__StreamDecoderReadStatus.
          FLAC__STREAM_DECODER_READ_STATUS_END_OF_STREAM;
      }
      catch(Exception) {
        return UnsafeNativeMethods.FLAC__StreamDecoderReadStatus.
          FLAC__STREAM_DECODER_READ_STATUS_ABORT;
      }

      return UnsafeNativeMethods.FLAC__StreamDecoderReadStatus.
        FLAC__STREAM_DECODER_READ_STATUS_CONTINUE;
    }

    /// <summary>Moves the file pointer within the stream</summary>
    /// <param name="decoder">FLAC stream decoder issuing the seek request</param>
    /// <param name="absolute_byte_offset">New offset to move the file pointer to</param>
    /// <param name="client_data">Not used</param>
    /// <returns>Whether the file pointer was successfully moved</returns>
    private UnsafeNativeMethods.FLAC__StreamDecoderSeekStatus seekCallback(
      IntPtr decoder,
      UInt64 absolute_byte_offset,
      IntPtr client_data
    ) {
      try {
        Seek((long)absolute_byte_offset);
      }
      catch(NotSupportedException) {
        return UnsafeNativeMethods.FLAC__StreamDecoderSeekStatus.
          FLAC__STREAM_DECODER_SEEK_STATUS_UNSUPPORTED;
      }
      catch(Exception) {
        return UnsafeNativeMethods.FLAC__StreamDecoderSeekStatus.
          FLAC__STREAM_DECODER_SEEK_STATUS_ERROR;
      }

      return UnsafeNativeMethods.FLAC__StreamDecoderSeekStatus.
        FLAC__STREAM_DECODER_SEEK_STATUS_OK;
    }

    /// <summary>Returns the current position of the file pointer within the stream</summary>
    /// <param name="decoder">FLAC stream decoder issuing the seek request</param>
    /// <param name="absolute_byte_offset">
    ///   Output parameter that will receive the current file pointer position
    /// </param>
    /// <param name="client_data">Not used</param>
    /// <returns>Whether the file pointer position was successfully determined</returns>
    private UnsafeNativeMethods.FLAC__StreamDecoderTellStatus tellCallback(
      IntPtr decoder,
      ref UInt64 absolute_byte_offset,
      IntPtr client_data
    ) {
      try {
        absolute_byte_offset = (UInt64)Position;
      }
      catch(NotSupportedException) {
        return UnsafeNativeMethods.FLAC__StreamDecoderTellStatus.
          FLAC__STREAM_DECODER_TELL_STATUS_UNSUPPORTED;
      }
      catch(Exception) {
        return UnsafeNativeMethods.FLAC__StreamDecoderTellStatus.
          FLAC__STREAM_DECODER_TELL_STATUS_ERROR;
      }

      return UnsafeNativeMethods.FLAC__StreamDecoderTellStatus.
        FLAC__STREAM_DECODER_TELL_STATUS_OK;
    }

    /// <summary>Returns the length of the stream</summary>
    /// <param name="decoder">FLAC stream decoder issuing the length request</param>
    /// <param name="stream_length">
    ///   Output parameter that will receive the stream's length
    /// </param>
    /// <param name="client_data">Not used</param>
    /// <returns>Whether the length of the stream was successfully determined</returns>
    private UnsafeNativeMethods.FLAC__StreamDecoderLengthStatus lengthCallback(
      IntPtr decoder,
      ref UInt64 stream_length,
      IntPtr client_data
    ) {
      try {
        stream_length = (UInt64)Length;
      }
      catch(NotSupportedException) {
        return UnsafeNativeMethods.FLAC__StreamDecoderLengthStatus.
          FLAC__STREAM_DECODER_LENGTH_STATUS_UNSUPPORTED;
      }
      catch(Exception) {
        return UnsafeNativeMethods.FLAC__StreamDecoderLengthStatus.
          FLAC__STREAM_DECODER_LENGTH_STATUS_ERROR;
      }
      return UnsafeNativeMethods.FLAC__StreamDecoderLengthStatus.
        FLAC__STREAM_DECODER_LENGTH_STATUS_OK;
    }

    /// <summary>Determines whether the end of the stream has been reached</summary>
    /// <param name="decoder">FLAC stream decoder issuing the eof determination request</param>
    /// <param name="client_data">Not used</param>
    /// <returns>True when the end of the stream was reached, false otherwise</returns>
    private int eofCallback(
      IntPtr decoder,
      IntPtr client_data
    ) {
      try {
        return IsEndOfStreamReached ? 1 : 0;
      }
      catch(Exception exception) {
        Trace.TraceError(
          "FLAC stream adapter received an exception from IsEndOfStreamReached:\n{0}",
          exception
        );
      }

      return 1;
    }

    /// <summary>Writes data into the stream</summary>
    /// <param name="decoder">FLAC stream decoder issuing the write request</param>
    /// <param name="frameHandle">A description of the decoded frame</param>
    /// <param name="buffer">Pointers to the decoded data of the individual channels</param>
    /// <param name="client_data">Not used</param>
    /// <returns>
    ///   Whether the decoded data has been written into the stream successfully
    /// </returns>
    private UnsafeNativeMethods.FLAC__StreamDecoderWriteStatus writeCallback(
      IntPtr decoder,
      IntPtr frameHandle,
      IntPtr[] buffer,
      IntPtr client_data
    ) {
      try {
        Frame frame = FrameMarshalHelper.MarshalFrame(frameHandle);
        Write(0, null); // TODO
      }
      catch(Exception) {
        return UnsafeNativeMethods.FLAC__StreamDecoderWriteStatus.
          FLAC__STREAM_DECODER_WRITE_STATUS_ABORT;
      }

      return UnsafeNativeMethods.FLAC__StreamDecoderWriteStatus.
        FLAC__STREAM_DECODER_WRITE_STATUS_CONTINUE;
    }

    /// <summary>Processes a decoded metadata block</summary>
    /// <param name="decoder">
    ///   FLAC stream decoder issuing the metadata processing request
    /// </param>
    /// <param name="metadata">Pointer to the decoded metadata block</param>
    /// <param name="client_data">Not used</param>
    private void metadataCallback(
      IntPtr decoder,
      IntPtr metadata,
      IntPtr client_data
    ) {
      try {
        ProcessMetadata(123); // TODO
      }
      catch(Exception exception) {
        Trace.TraceError(
          "FLAC stream adapter received an exception from ProcessMetadata():\n{0}",
          exception
        );
      }
    }

    /// <summary>Responds to an error that has occured during decoding</summary>
    /// <param name="decoder">FLAC stream decoder issuing the error notification</param>
    /// <param name="status">Error that has been encountered by the decoder</param>
    /// <param name="client_data">Not used</param>
    private void errorCallback(
      IntPtr decoder,
      UnsafeNativeMethods.FLAC__StreamDecoderErrorStatus status,
      IntPtr client_data
    ) {
      try {
        HandleError(streamDecoderErrorFromFlacStreamDecoderErrorStatus(status));
      }
      catch(Exception exception) {
        Trace.TraceError(
          "FLAC stream adapter received an exception from HandleError():\n{0}",
          exception
        );
      }
    }

    /// <summary>
    ///   Converts a FLAC stream decoder error status into a value from the
    ///   StreamDecoderErrors enumeration
    /// </summary>
    /// <param name="errorStatus">
    ///   FLAC stream decoder error status that will be converted
    /// </param>
    /// <returns>The equivalent entry in the StreamDecoderErrors enumeration</returns>
    private static StreamDecoderError streamDecoderErrorFromFlacStreamDecoderErrorStatus(
      UnsafeNativeMethods.FLAC__StreamDecoderErrorStatus errorStatus
    ) {
      switch(errorStatus) {
        case UnsafeNativeMethods.FLAC__StreamDecoderErrorStatus.
          FLAC__STREAM_DECODER_ERROR_STATUS_BAD_HEADER: {
          return StreamDecoderError.BadHeader;
        }
        case UnsafeNativeMethods.FLAC__StreamDecoderErrorStatus.
          FLAC__STREAM_DECODER_ERROR_STATUS_FRAME_CRC_MISMATCH: {
          return StreamDecoderError.FrameCrcMismatch;
        }
        case UnsafeNativeMethods.FLAC__StreamDecoderErrorStatus.
          FLAC__STREAM_DECODER_ERROR_STATUS_LOST_SYNC: {
          return StreamDecoderError.LostSync;
        }
        case UnsafeNativeMethods.FLAC__StreamDecoderErrorStatus.
          FLAC__STREAM_DECODER_ERROR_STATUS_UNPARSEABLE_STREAM: {
          return StreamDecoderError.UnparseableStream;
        }
        default: {
          throw new ArgumentException("Invalid error status");
        }
      }
    }

    // These delegates are required!
    //
    // When a delegate is passed to native code, the .NET runtime generates a small thunk of
    // executable code tailored for the delegate instance. This thunk performs the call back
    // into the method the delegate is pointing to. It also counts as a reference for the GC
    // not to collect the object whose method the delegate points to.
    //
    // Now if native code stores the adress of this generated thunk for later use, the GC has
    // no way of knowing about it and will see the delegate as a candidate for garbage
    // collection. Should a collection happen and the native code tries to call the function
    // pointer, the result is most likely an access violation.
    //
    // Thus, the FlacStream keeps its delegates alive as long as it is alive itself by
    // referencing them here. The FlacStream then is prevented from becoming a candidate for
    // garbage collection by a GCHandle that's stored as the 'client_data' pointer by
    // unmanaged code.
    //
    // Yep, P/Invoke isn't always as clean and straightforward as you might think :)

    /// <summary>Delegate for the readCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderReadCallback ReadCallbackDelegate;
    /// <summary>Delegate for the seekCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderSeekCallback SeekCallbackDelegate;
    /// <summary>Delegate for the tellCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderTellCallback TellCallbackDelegate;
    /// <summary>Delegate for the lengthCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderLengthCallback LengthCallbackDelegate;
    /// <summary>Delegate for the eofCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderEofCallback EofCallbackDelegate;
    /// <summary>Delegate for the writeCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderWriteCallback WriteCallbackDelegate;
    /// <summary>Delegate for the metadataCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderMetadataCallback MetadataCallbackDelegate;
    /// <summary>Delegate for the errorCallback() method</summary>
    internal UnsafeNativeMethods.FLAC__StreamDecoderErrorCallback ErrorCallbackDelegate;

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
