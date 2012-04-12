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

  /// <summary>Constructs compressed LZMA packages</summary>
  public static class LzmaPackageBuilder {

    #region struct PackageFile

    /// <summary>Stores informations about a file that is to be packaged</summary>
    public struct PackageFile {

      /// <summary>Initializes a new package file</summary>
      /// <param name="path">Absolute path of the file to be packaged</param>
      /// <param name="name">Name to assign to the file inside the package</param>
      public PackageFile(string path, string name) {
        this.Path = path;
        this.Name = name;
      }

      /// <summary>The absolute path of the file to package</summary>
      public string Path;
      /// <summary>Name the file will have inside the package</summary>
      public string Name;

    }

    #endregion // struct PackageFile

    #region struct PackageFileHeader

    /// <summary>Stores informations about a file that is to be packaged</summary>
    private struct PackageFileHeader {

      /// <summary>Name of the asset</summary>
      public string Name;
      /// <summary>Where in the LZMA package the file's data is stored</summary>
      public long DataOffset;
      /// <summary>Length the data has in uncompressed form</summary>
      public int UncompressedLength;
      /// <summary>Length of the compressed data</summary>
      public int CompressedLength;

      /// <summary>Writes the header of a package file into a binary writer</summary>
      /// <param name="writer">Binary writer the package file header is written to</param>
      public void Write(BinaryWriter writer) {
        writer.Write(this.Name);
        writer.Write(this.DataOffset);
        writer.Write(this.CompressedLength);
        writer.Write(this.UncompressedLength);
      }

    }

    #endregion // struct PackageFileHeader

    /// <summary>Adds one or more files to the list of files to be packed</summary>
    /// <param name="packagePath">Path to the package that assets are read from</param>
    /// <param name="packageFiles">Enumerable list with the paths of the files to add</param>
    public static void Build(string packagePath, params PackageFile[] packageFiles) {
      Build(packagePath, (IEnumerable<PackageFile>)packageFiles);
    }

    /// <summary>Adds one or more files to the list of files to be packed</summary>
    /// <param name="packagePath">Path to the package that assets are read from</param>
    /// <param name="packageFiles">Enumerable list with the paths of the files to add</param>
    public static void Build(string packagePath, IEnumerable<PackageFile> packageFiles) {
      List<PackageFileHeader> packageFileHeaders = setupPackageFileHeaders(packageFiles);

      using(
        FileStream package = new FileStream(
          packagePath, FileMode.Create, FileAccess.Write, FileShare.None
        )
      ) {
        BinaryWriter packageWriter = new BinaryWriter(package);
        packageWriter.Write(packageFileHeaders.Count);

        // Write the preliminary headers to the package (we don't know some informations,
        // like the compressed file sizes, yet). The headers will be written a second
        // time after the data offsets and compressed file sizes have been filled in.
        foreach(PackageFileHeader packageFileHeader in packageFileHeaders)
          packageFileHeader.Write(packageWriter);

        // Compress all files and write them into the package, filling the missing fields
        // in the package file headers in the process.
        int index = 0;
        foreach(PackageFile packageFile in packageFiles) {
          using(
            FileStream asset = new FileStream(
              packageFile.Path, FileMode.Open, FileAccess.Read, FileShare.Read
            )
          ) {
            PackageFileHeader header = packageFileHeaders[index];
            header.DataOffset = package.Position;
            header.CompressedLength = compress(package, asset);
            packageFileHeaders[index] = header;
          }

          ++index;
        }

        // Second run writing the package file headers, this time we know all the
        // informations. This relies on the headers not changing in size, so any
        // strings contained in the headers must not be modified.
        package.Position = 4;
        foreach(PackageFileHeader packageFileHeader in packageFileHeaders)
          packageFileHeader.Write(packageWriter);
      }
    }

    /// <summary>
    ///   Compresses data in a stream and writes the compressed data into another stream
    /// </summary>
    /// <param name="destination">Destination stream the data is written to</param>
    /// <param name="source">Stream from which the data to be compressed is taken</param>
    /// <returns>The number of bytes that were written into the destination stream</returns>
    private static int compress(Stream destination, Stream source) {
      long startingPosition = destination.Position;

      Encoder lzmaEncoder = new Encoder();
      lzmaEncoder.WriteCoderProperties(destination);
      lzmaEncoder.Code(
        source, destination,
        0, 0, null
      );

      return (int)(destination.Position - startingPosition);
    }

    /// <summary>
    ///   Sets up a list of package file headers for the provided file names.
    /// </summary>
    /// <param name="packageFiles">File name to set up the package file headers for</param>
    /// <returns>A list of package file headers for the provided file names</returns>
    private static List<PackageFileHeader> setupPackageFileHeaders(
      IEnumerable<PackageFile> packageFiles
    ) {
      List<PackageFileHeader> packageFileHeaders = new List<PackageFileHeader>();

      foreach(PackageFile packageFile in packageFiles) {
        PackageFileHeader packageFileHeader;
        packageFileHeader.Name = packageFile.Name;
        packageFileHeader.DataOffset = 0; // filled in later
        packageFileHeader.UncompressedLength = (int)(new FileInfo(packageFile.Path).Length);
        packageFileHeader.CompressedLength = 0; // filled in later

        packageFileHeaders.Add(packageFileHeader);
      }

      return packageFileHeaders;
    }

  }

} // namespace Nuclex.Game.ContentCompressor
