using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nuclex.Audio.Formats.Flac {

#if ENABLE_PINVOKE_FLAC_DECODER

  /// <summary>FLAC stream decoder</summary>
  public class StreamDecoder : IDisposable {

    /// <summary>Initializes a new FLAC stream decoder</summary>
    public StreamDecoder() {

      this.decoderHandle = UnsafeNativeMethods.FLAC__stream_decoder_new();
      if(this.decoderHandle == IntPtr.Zero) {
        throw new OutOfMemoryException("Could not allocate memory for FLAC stream decoder");
      }

    }

    /// <summary>Finalizer that will be called by the GC.</summary>
    ~StreamDecoder() {
      Dispose(false); // false == not being called by user code
    }

    /// <summary>Initializes the FLAC stream decoder</summary>
    /// <param name="stream">Stream that will be decoded</param>
    public void InitStream(FlacStream stream) {

      // Explanation for the GCHandle stuff: The only reference to the FlacStream instance
      // will very likely remain in unmanaged code. Since the garbage collector cannot
      // see this reference, given no other references to it exist, the FlacStream instance
      // would then become a candidate for garbage collection. The GCHandle avoids this.
      // It will ensure the FlacStream instance is kept alive until it the FLAC stream decoder
      // is destroyed (at which point we are responsible for invoking its Free() method)
      GCHandle streamHandle = GCHandle.Alloc(stream, GCHandleType.Normal);
      try {
        int result = UnsafeNativeMethods.FLAC__stream_decoder_init_stream(
          this.decoderHandle,
          stream.ReadCallbackDelegate,
          stream.SeekCallbackDelegate,
          stream.TellCallbackDelegate,
          stream.LengthCallbackDelegate,
          stream.EofCallbackDelegate,
          stream.WriteCallbackDelegate,
          stream.MetadataCallbackDelegate,
          stream.ErrorCallbackDelegate,
          GCHandle.ToIntPtr(streamHandle)
        );
        int goodResult = (int)UnsafeNativeMethods.FLAC__StreamDecoderInitStatus.
          FLAC__STREAM_DECODER_INIT_STATUS_OK;

        if(result != goodResult) {
          // TODO: Transform the error code into an appropriate exception
          throw new Exception("FLAC__stream_decoder_init_stream() has failed");
        }
      }
      catch(Exception) {
        streamHandle.Free();
        throw;
      }

    }

    /// <summary>Decodes data until the end of the stream has been reached</summary>
    public void ProcessUntilEndOfStream() {
      int result = UnsafeNativeMethods.FLAC__stream_decoder_process_until_end_of_stream(
        this.decoderHandle
      );
      if(result == 0) {
        throw new Exception("Stream processing has been aborted because of an error");
      }
    }

    /// <summary>The "MD5 signature checking" flag</summary>
    public bool EnableMd5Checking {
      get {
        int result = UnsafeNativeMethods.FLAC__stream_decoder_get_md5_checking(
          this.decoderHandle
        );
        return (result != 0);
      }
      set {
        int result = UnsafeNativeMethods.FLAC__stream_decoder_set_md5_checking(
          this.decoderHandle, value ? 1 : 0
        );
        if(result == 0) {
          throw new InvalidOperationException("FLAC stream decoder is already initialized");
        }
      }
    }

    /// <summary>Immediately releases all resources owned by the object.</summary>
    public void Dispose() {
      Dispose(true); // true == called by user code

      // We are finalized already, tell the GC that it doesn't
      // need to call the finalizer anymore
      GC.SuppressFinalize(this);
    }

    /// <summary>Immediately releases all resources owned by the object.</summary>
    /// <param name="calledByUser">
    ///   If true, the object is being disposed explicitely and can still access
    ///   the other managed objects it is referencing.
    /// </param>
    protected virtual void Dispose(bool calledByUser) {
      if(calledByUser) {
        // Perform finalization of managed objects here
      }

      if(this.decoderHandle != IntPtr.Zero) {
        UnsafeNativeMethods.FLAC__stream_decoder_delete(this.decoderHandle);
      }
    }

    /// <summary>
    ///   Handle of the native FLAC stream decoder being wrapped by the instance
    /// </summary>
    private IntPtr decoderHandle;

  }

#endif // ENABLE_PINVOKE_FLAC_DECODER

} // namespace Nuclex.Audio.Formats.Flac
