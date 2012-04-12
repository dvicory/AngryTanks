#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2007 Nuclex Development Labs

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

namespace Nuclex.Game.ContentCompressor {

  /// <summary>Compressed content files with the LZMA algorithm</summary>
  public static class LzmaContentCompressor {

    /// <summary>Compresses an individual content file</summary>
    /// <param name="path">Path of the content file that will be compressed</param>
    public static void CompressContentFile(string path) {

      // Change the asset's extension to .lzma to allow the normal content manage to live
      // side by side with the decompressing content manager
      string compressedFilename = Path.ChangeExtension(path, ".lzma");

      Console.WriteLine(
        string.Format(
          "Compressing {0} to {1}...",
          Path.GetFileName(path),
          Path.GetFileName(compressedFilename)
        )
      );

      Encoder lzmaEncoder = new Encoder();
      using(FileStream compressedFile = new FileStream(compressedFilename, FileMode.Create)) {
        using(FileStream uncompressedFile = new FileStream(path, FileMode.Open)) {
          BinaryWriter writer = new BinaryWriter(compressedFile);
          writer.Write((int)uncompressedFile.Length);

          lzmaEncoder.WriteCoderProperties(compressedFile);
          lzmaEncoder.Code(
            uncompressedFile, compressedFile,
            //uncompressedFile.Length, compressedFile.Length,
            0, 0, null
          );
        } // using uncompressedFile
      } // using compressedFile

      //File.Delete(path);
    }

  }

} // namespace Nuclex.Game.ContentCompressor
