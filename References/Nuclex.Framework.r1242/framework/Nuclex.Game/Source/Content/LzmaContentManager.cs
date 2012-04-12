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

using SevenZip.Compression.LZMA;

using Microsoft.Xna.Framework.Content;

namespace Nuclex.Game.Content {

  /// <summary>Content manager that can read LZMA-compressed game assets</summary>
  public class LzmaContentManager : ContentManager {

    #region struct CompressedFileInfo

    /// <summary>Stores the informations of a compressed file</summary>
    private struct CompressedFileInfo {
      /// <summary>Where in the LZMA package the file is stored</summary>
      public long Offset;
      /// <summary>Length of the compressed data</summary>
      public int CompressedLength;
      /// <summary>Length the data will have when it is uncompressed</summary>
      public int UncompressedLength;
    }

    #endregion // struct CompressedFileInfo

    /// <summary>Initializes a new LZMA-decompressing content manager</summary>
    /// <param name="serviceProvider">
    ///   Service provider to use for accessing additional services that the
    ///   individual content readers require for creating their resources.
    /// </param>
    public LzmaContentManager(IServiceProvider serviceProvider) :
      this(serviceProvider, true) { }

    /// <summary>Initializes a new LZMA-decompressing content manager</summary>
    /// <param name="serviceProvider">
    ///   Service provider to use for accessing additional services that the
    ///   individual content readers require for creating their resources.
    /// </param>
    /// <param name="allowReplacement">
    ///   Is the user allowed to replace individual files by placing the .xnb files
    ///   in the game's folders as Visual Studio would normally do?
    /// </param>
    /// <remarks>
    ///   It is recommended to leave the allowReplacement argument set to true at least
    ///   during development. Otherwise you will be forced to rerun the content compression
    ///   utility each time you modify one of your assets. Forgetting to run the utility
    ///   might further lead to strange problems because the game would then be working
    ///   with an older version of the assets than you think it does.
    /// </remarks>
    public LzmaContentManager(IServiceProvider serviceProvider, bool allowReplacement) :
      base(serviceProvider) {
      this.replacementAllowed = allowReplacement;
    }

    /// <summary>Initializes a new LZMA-decompressing content manager</summary>
    /// <param name="packagePath">
    ///   Path of the package to read content asset files from. This package needs to
    ///   be created using the Nuclex content compression utility.
    /// </param>
    /// <param name="serviceProvider">
    ///   Service provider to use for accessing additional services that the
    ///   individual content readers require for creating their resources.
    /// </param>
    public LzmaContentManager(IServiceProvider serviceProvider, string packagePath) :
      this(serviceProvider, packagePath, true) { }

    /// <summary>Initializes a new LZMA-decompressing content manager</summary>
    /// <param name="serviceProvider">
    ///   Service provider to use for accessing additional services that the
    ///   individual content readers require for creating their resources.
    /// </param>
    /// <param name="packagePath">
    ///   Path of the package to read content asset files from. This package needs to
    ///   be created using the Nuclex content compression utility.
    /// </param>
    /// <param name="allowReplacement">
    ///   Is the user allowed to replace individual files by placing the .xnb files
    ///   in the game's folders as Visual Studio would normally do?
    /// </param>
    /// <remarks>
    ///   It is recommended to leave the allowReplacement argument set to true at least
    ///   during development. Otherwise you will be forced to rerun the Nuclex content
    ///   compression utility each time you modify one of your assets. Forgetting to run
    ///   the utility might further lead to strange problems because the game would then
    ///   be working with an older version of the assets than you think it does.
    /// </remarks>
    public LzmaContentManager(
      IServiceProvider serviceProvider, string packagePath, bool allowReplacement
    ) :
      this(serviceProvider, allowReplacement) {
      openPackage(packagePath);
    }

    /// <summary>Immediately releases all resources used the content manager</summary>
    /// <param name="calledByUser">Whether the call was initiated by user code</param>
    /// <remarks>
    ///   If the call wasn't initiated by user code, the call comes from the .NET
    ///   garbage collector, meaning the content manager must not access any other
    ///   classes for risk of them having been reclaimed already.
    /// </remarks>
    protected override void Dispose(bool calledByUser) {
      if(calledByUser) {
        if(this.lzmaPackageStream != null) {
          this.lzmaPackageStream.Dispose();
          this.lzmaPackageStream = null;
        }
      }

      base.Dispose(calledByUser);
    }

    /// <summary>Opens a stream to the named asset</summary>
    /// <param name="assetName">Asset to open a stream for</param>
    /// <returns>The opened stream</returns>
    protected override Stream OpenStream(string assetName) {

      // If overriding resources is allowed, check whether a normal file with
      // the .xnb extension already exists. If it does, we will use the .xnb file
      // instead of the compressed content. This prevents nasty surprises when people
      // run the content compressor on their build directory and then continue working.
      if(this.replacementAllowed) {
        string xnbAssetFilename = Path.ChangeExtension(assetName, ".xnb");
        string xnbAssetPath = Path.Combine(RootDirectory, xnbAssetFilename);
        if(File.Exists(xnbAssetPath)) {
          return new FileStream(
            xnbAssetPath, FileMode.Open, FileAccess.Read, FileShare.Read
          );
        }
      }

      // Are we configured to use a package?
      if(this.files != null) {
        return uncompressFromPackage(assetName);
      } else { // Nope, look for an individually compressed file!
        return uncompressIndividualFile(assetName);
      }

    }

    /// <summary>Uncompresses an LZMA-compressed file from an LZMA package</summary>
    /// <param name="assetName">Name of the packaged file to decompress</param>
    /// <returns>A stream by which the uncompressed data can be read</returns>
    private Stream uncompressFromPackage(string assetName) {

      // Look for the compressed asset in the LZMA package
      CompressedFileInfo fileInfo;
      if(!this.files.TryGetValue(assetName.ToLower(), out fileInfo)) {
        throw new ArgumentException(
          string.Format("Asset '{0}' not found in package", assetName), "assetName"
        );
      }

      // We have found the entry, jump to the indicated position and
      // uncompress the asset
      lock(this.lzmaPackageStream) {
        this.lzmaPackageStream.Position = fileInfo.Offset;
        return uncompress(
          this.lzmaPackageStream, fileInfo.CompressedLength, fileInfo.UncompressedLength
        );
      }

    }

    /// <summary>Uncompresses an invididually compressed LZMA file</summary>
    /// <param name="assetName">Name of the asset to uncompress as an LZMA file</param>
    /// <returns>A stream by which the uncompressed data can be read</returns>
    private Stream uncompressIndividualFile(string assetName) {

      // Look for a compressed .lzma asset
      string lzmaAssetFilename = Path.ChangeExtension(assetName, ".lzma");
      string lzmaAssetPath = Path.Combine(RootDirectory, lzmaAssetFilename);
      if(!File.Exists(lzmaAssetPath)) {
        throw new ArgumentException(
          string.Format("Asset '{0}' not found", assetName), "assetName"
        );
      }

      // Try to open and decompress the .lzma asset as an individual file
      using(
        FileStream compressedFile = new FileStream(
          lzmaAssetPath, FileMode.Open, FileAccess.Read, FileShare.Read
        )
      ) {
        BinaryReader reader = new BinaryReader(compressedFile);
        int uncompressedLength = reader.ReadInt32();

        return uncompress(
          compressedFile, (int)(compressedFile.Length - 4), uncompressedLength
        );
      }

    }

    /// <summary>Uncompresses a stream of LZMA-compressed data into a memory stream</summary>
    /// <param name="source">Source stream containing the LZMA-compressed data</param>
    /// <param name="compressedLength">Length of the compressed data</param>
    /// <param name="uncompressedLength">Length the uncompressed data will have</param>
    /// <returns>A memory stream containing the uncompressed data</returns>
    private Stream uncompress(Stream source, int compressedLength, int uncompressedLength) {
      BinaryReader reader = new BinaryReader(source);

      // Build a memory chunk we can uncompress into
      MemoryStream uncompressedMemory = new MemoryStream(uncompressedLength);

      // Set up the LZMA decoder and decode the compressed asset into the memory chunk
      Decoder decoder = new Decoder();
      decoder.SetDecoderProperties(reader.ReadBytes(5));
      decoder.Code(
        source, uncompressedMemory,
        compressedLength - 5, uncompressedLength,
        null
      );

      // Done, set the file pointer to the beginning of the memory chunk and return it
      uncompressedMemory.Position = 0;
      return uncompressedMemory;
    }

    /// <summary>Opens the specified package for access by the LZMA content manager</summary>
    /// <param name="packagePath">Path of the package that will be opened</param>
    private void openPackage(string packagePath) {

      // If file replacement is allowed, there might not even be a package and all the
      // assets are stored uncompressed in their respective folders.
      if(this.replacementAllowed) {
        if(!File.Exists(packagePath)) {
          this.files = new Dictionary<string, CompressedFileInfo>();
          return;
        }
      }

      FileStream lzmaPackageStream = new FileStream(
        packagePath, FileMode.Open, FileAccess.Read, FileShare.Read
      );
      try {
        BinaryReader reader = new BinaryReader(lzmaPackageStream);

        // Obtain the number of assets that are stored in this package
        int fileCount = reader.ReadInt32();
        this.files = new Dictionary<string, CompressedFileInfo>(fileCount);

        // Read the file headers and all assets stored in the LZMA package
        for(int fileIndex = 0; fileIndex < fileCount; ++fileIndex) {
          string name = reader.ReadString();

          CompressedFileInfo fileInfo;
          fileInfo.Offset = reader.ReadInt64();
          fileInfo.CompressedLength = reader.ReadInt32();
          fileInfo.UncompressedLength = reader.ReadInt32();

          this.files.Add(name.ToLower(), fileInfo);
        }
      }
      catch(Exception) {
        lzmaPackageStream.Dispose();
        throw;
      }
      
      // Successfully parsed, take over the package stream
      this.lzmaPackageStream = lzmaPackageStream;

    }

    /// <summary>
    ///   Whether compressed files can be overridden by placing an uncompressed file
    ///   at the same location the compressed file is in.
    /// </summary>
    private bool replacementAllowed;
    /// <summary>File stream for the LZMA package opened by this content manager</summary>
    private FileStream lzmaPackageStream;
    /// <summary>Starting offsets for the files contained</summary>
    private Dictionary<string, CompressedFileInfo> files;

  }

} // namespace Nuclex.Game.Content
