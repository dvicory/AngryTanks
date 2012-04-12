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

#if UNITTEST

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Game.ContentCompressor;
using Nuclex.Graphics;
using Nuclex.Testing.Xna;

namespace Nuclex.Game.Content {

  /// <summary>Unit test for the LZMA content manager class</summary>
  [TestFixture]
  internal class LzmaContentManagerTest {

    #region class TempDirectoryKeeper

    /// <summary>Creates and provides a temporary directory</summary>
    private class TempDirectoryKeeper : IDisposable {

      /// <summary>Initializes a new temp directory keeper</summary>
      public TempDirectoryKeeper() {
        this.tempFile = System.IO.Path.GetTempFileName();
        try {
          this.tempDir = System.IO.Path.ChangeExtension(this.tempFile, ".dir");
          Directory.CreateDirectory(this.tempDir);
        }
        catch(Exception) {
          File.Delete(this.tempFile);
        }
      }

      /// <summary>Immediately releases all resources owned by the instance</summary>
      public void Dispose() {
        if(this.tempDir != null) {
          Directory.Delete(this.tempDir, true);
          this.tempDir = null;
        }
        if(this.tempFile != null) {
          File.Delete(this.tempFile);
          this.tempFile = null;
        }
      }

      /// <summary>The full path to the temporary directory</summary>
      public string Path {
        get { return this.tempDir; }
      }

      /// <summary>Temporary file that has been created to get a unique name</summary>
      private string tempFile;
      /// <summary>Full path of the temporary directory</summary>
      private string tempDir;

    }

    #endregion // class TempDirectoryKeeper

    /// <summary>
    ///   Verifies that the LZMA content manager can load assets out of
    ///   compressed files
    /// </summary>
    [Test]
    public void TestCompressedContentLoading() {

      // Extract the .xnb test file for the unit test and compress it
      {
        string path = Path.Combine(this.tempDirectory.Path, "UnitTestEffect.xnb");
        writeBytesToFile(
          Resources.UnitTestResources.UnitTestEffect,
          path
        );
        LzmaContentCompressor.CompressContentFile(path);
        File.Delete(path);
      }

      // Now try to load the compressed file
      using(
        LzmaContentManager contentManager = new LzmaContentManager(
          GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
            this.mockedGraphicsDeviceService
          )
        )
      ) {
        contentManager.RootDirectory = this.tempDirectory.Path;

        Effect effect = contentManager.Load<Effect>("UnitTestEffect");
        Assert.IsNotNull(effect);
      }
    }

    /// <summary>
    ///   Verifies that the LZMA content manager uses an uncompressed asset if
    ///   it is available in parallel to the compressed one
    /// </summary>
    [Test]
    public void TestCompressedContentReplacement() {

      // If the replacement doesn't work, the test will load this file which,
      // as the observant programmer might notice, contains 'Crap' :-)
      writeBytesToFile(
        new byte[] { (byte)'C', (byte)'r', (byte)'a', (byte)'p' },
        Path.Combine(this.tempDirectory.Path, "UnitTestEffect.lzma")
      );

      // Write the real file the test should be loading
      writeBytesToFile(
        Resources.UnitTestResources.UnitTestEffect,
        Path.Combine(this.tempDirectory.Path, "UnitTestEffect.xnb")
      );

      using(
        LzmaContentManager contentManager = new LzmaContentManager(
          GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
            this.mockedGraphicsDeviceService
          )
        )
      ) {
        contentManager.RootDirectory = this.tempDirectory.Path;

        // The .lzma file contains 'Crap' (literally :D), so if the content manager
        // didn't see the replacement file, it would fall on its nose here!
        Effect effect = contentManager.Load<Effect>("UnitTestEffect");
        Assert.IsNotNull(effect);
      }
    }

    /// <summary>
    ///   Verifies that the LZMA content manager can load assets out of
    ///   compressed packages
    /// </summary>
    [Test]
    public void TestPackagedContentLoading() {

      // Extract the .xnb test file for the unit test and compress it
      string packagePath = Path.Combine(this.tempDirectory.Path, "UnitTestEffect.package");
      {
        string filePath = Path.Combine(this.tempDirectory.Path, "UnitTestEffect.xnb");
        writeBytesToFile(
          Resources.UnitTestResources.UnitTestEffect,
          filePath
        );

        LzmaPackageBuilder.Build(
          packagePath, new LzmaPackageBuilder.PackageFile(filePath, "UnitTestEffect")
        );

        File.Delete(filePath);
      }

      using(
        LzmaContentManager contentManager = new LzmaContentManager(
          GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
            this.mockedGraphicsDeviceService
          ),
          packagePath
        )
      ) {
        contentManager.RootDirectory = Path.GetDirectoryName(packagePath);
        Effect effect = contentManager.Load<Effect>("UnitTestEffect");
        Assert.IsNotNull(effect);
      }
    }

    /// <summary>
    ///   Verifies that the LZMA content manager uses an uncompressed asset if
    ///   it is available even when though no package exists
    /// </summary>
    [Test]
    public void TestPackagedContentReplacement() {

      // Extract the .xnb test file for the unit test and compress it
      string packagePath = Path.Combine(this.tempDirectory.Path, "UnitTestEffect.package");
      {
        string filePath = Path.Combine(this.tempDirectory.Path, "UnitTestEffect.xnb");

        // We will compress this nonsense file so that if the replacement isn't
        // honored, the content manager will fail.
        writeBytesToFile(
          new byte[] { (byte)'C', (byte)'r', (byte)'a', (byte)'p' }, filePath
        );
        LzmaPackageBuilder.Build(
          packagePath, new LzmaPackageBuilder.PackageFile(filePath, "UnitTestEffect")
        );
        File.Delete(filePath);

        // Now write the replacement, which actually contains valid data
        writeBytesToFile(
          Resources.UnitTestResources.UnitTestEffect,
          filePath
        );
      }

      using(
        LzmaContentManager contentManager = new LzmaContentManager(
          GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
            this.mockedGraphicsDeviceService
          ),
          packagePath
        )
      ) {
        contentManager.RootDirectory = this.tempDirectory.Path;

        // This only works if the content manager loads the replacement instead
        // of the packaged file
        Effect effect = contentManager.Load<Effect>("UnitTestEffect");
        Assert.IsNotNull(effect);
      }
    }

    /// <summary>
    ///   Verifies that the LZMA content manager uses an uncompressed asset if
    ///   it is available even when though no package exists
    /// </summary>
    [Test]
    public void TestPackagedContentReplacementWhenNoPackageExists() {
      string packagePath = Path.Combine(
        this.tempDirectory.Path, "PackagedUnitTestEffect.package"
      );
      writeBytesToFile(
        Resources.UnitTestResources.UnitTestEffect,
        Path.Combine(this.tempDirectory.Path, "UnitTestEffect.xnb")
      );

      using(
        LzmaContentManager contentManager = new LzmaContentManager(
          GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
            this.mockedGraphicsDeviceService
          ),
          packagePath
        )
      ) {
        contentManager.RootDirectory = this.tempDirectory.Path;

        // The package doesn't even exist, but because replacement is allowed,
        // no error should occur if the requested asset is available directly
        Effect effect = contentManager.Load<Effect>("UnitTestEffect");
        Assert.IsNotNull(effect);
      }
    }

    /// <summary>
    ///   Tests whether the LZMA content manager cleans up any open file handles
    ///   when an exception occurs while the package is opened
    /// </summary>
    [Test]
    public void TestCleanupAfterException() {
      string packagePath = Path.Combine(
        this.tempDirectory.Path, "PackagedUnitTestEffect.package"
      );
      writeBytesToFile(BitConverter.GetBytes(-1), packagePath);

      Assert.Throws<ArgumentOutOfRangeException>(
        delegate() {
          using(
            LzmaContentManager contentManager = new LzmaContentManager(
              GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
                this.mockedGraphicsDeviceService
              ),
              packagePath
            )
          ) { }
        }
      );

      // If the package is left open due to the exception, a follow-up error
      // will happen in the Teardown() method.
    }

    /// <summary>
    ///   Verifies that the right exception is thrown if a packaged does not exist
    /// </summary>
    [Test]
    public void TestThrowsOnMissingPackagedAsset() {
      string packagePath = Path.Combine(
        this.tempDirectory.Path, "PackagedUnitTestEffect.package"
      );
      using(
        LzmaContentManager contentManager = new LzmaContentManager(
          GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
            this.mockedGraphicsDeviceService
          ),
          packagePath
        )
      ) {
        contentManager.RootDirectory = this.tempDirectory.Path;

        Assert.Throws<ArgumentException>(
          delegate() {
            contentManager.Load<Effect>("DoesNotExist");
          }
        );
      }
    }

    /// <summary>
    ///   Verifies that the LZMA content manager uses an uncompressed asset if
    ///   it is available even when though no package exists
    /// </summary>
    [Test]
    public void TestThrowOnMissingCompressedAsset() {
      using(
        LzmaContentManager contentManager = new LzmaContentManager(
          GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
            this.mockedGraphicsDeviceService
          )
        )
      ) {
        contentManager.RootDirectory = this.tempDirectory.Path;

        Assert.Throws<ArgumentException>(
          delegate() {
            contentManager.Load<Effect>("DoesNotExist");
          }
        );
      }
    }

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService();
      this.mockedGraphicsDeviceService.CreateDevice();

      this.tempDirectory = new TempDirectoryKeeper();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void TearDown() {
      if(this.tempDirectory != null) {
        this.tempDirectory.Dispose();
        this.tempDirectory = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>Saves a bytes array into a file</summary>
    /// <param name="bytes">Byte array that will be written to a file</param>
    /// <param name="path">Name of the file the byte array will be written to</param>
    private void writeBytesToFile(byte[] bytes, string path) {
      using(
        FileStream stream = new FileStream(
          path, FileMode.Create, FileAccess.Write, FileShare.None
        )
      ) {
        stream.Write(bytes, 0, bytes.Length);
        stream.Flush();
      }
    }

    /// <summary>Mocked graphics device service used for unit testing</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>Creates a temp directory and keeps it alive until disposed</summary>
    private TempDirectoryKeeper tempDirectory;

  }

} // namespace Nuclex.Game.Content

#endif // UNITTEST
