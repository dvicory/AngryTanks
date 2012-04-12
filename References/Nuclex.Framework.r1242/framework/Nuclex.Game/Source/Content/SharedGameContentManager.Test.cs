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

using Nuclex.Graphics;
using Nuclex.Testing.Xna;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Nuclex.Game.Content {

  /// <summary>Unit test for the embedded content manager class</summary>
  [TestFixture]
  internal class GameSharedContentManagerTest {

    /// <summary>
    ///   Verifies that the adapter indeed uses the game's content manager
    /// </summary>
    [Test]
    public void TestContentAccess() {
      using(XnaGame game = new XnaGame()) {
        MockedGraphicsDeviceService service = new MockedGraphicsDeviceService();
        using(service.CreateDevice()) {
          game.Services.AddService(typeof(IGraphicsDeviceService), service);

          string assetPath = writeBytesToTempFile(
            Resources.UnitTestResources.UnitTestEffect
          );
          try {
            game.Content.RootDirectory = Path.GetDirectoryName(assetPath);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
  
            SharedGameContentManager adapter = new SharedGameContentManager(game);
            
            Effect loadedFromGame = game.Content.Load<Effect>(assetName);
            Effect loadedFromShared = adapter.Load<Effect>(assetName);
            
            Assert.AreSame(loadedFromGame, loadedFromShared);
          }
          finally {
            File.Delete(assetPath);
          }
        } // using graphics device
      } // using game
    }

    /// <summary>Saves a byte array into a temporary file</summary>
    /// <param name="bytes">Byte array that will be written to a file</param>
    /// <returns>Path to the file containing the temporary byte array</returns>
    private string writeBytesToTempFile(byte[] bytes) {
      string tempFile = Path.GetTempFileName();
      try {
        string outputFile = Path.ChangeExtension(tempFile, ".xnb");
        writeBytesToFile(bytes, outputFile);

        return outputFile;
      }
      finally {
        File.Delete(tempFile);
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

  }

} // namespace Nuclex.Game.Content

#endif // UNITTEST
