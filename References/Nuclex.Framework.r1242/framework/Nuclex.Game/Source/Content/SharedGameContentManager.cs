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

using Microsoft.Xna.Framework.Content;

namespace Nuclex.Game.Content {

  /// <summary>
  ///   Implements the shared content service on top of the Game class' built-in
  ///   content manager for dependency injection frameworks.
  /// </summary>
  public class SharedGameContentManager : ISharedContentService {

    /// <summary>Initializes a new shared content manager adapter</summary>
    /// <param name="game">Game the content manager will be taken from</param>
    public SharedGameContentManager(Microsoft.Xna.Framework.Game game) {
      this.contentManager = game.Content;
    }

    /// <summary>Loads or accesses shared game content</summary>
    /// <typeparam name="AssetType">Type of the asset to be loaded or accessed</typeparam>
    /// <param name="assetName">Path and name of the requested asset</param>
    /// <returns>The requested asset from the the shared game content store</returns>
    public AssetType Load<AssetType>(string assetName) {
      return this.contentManager.Load<AssetType>(assetName);
    }

    /// <summary>The content manager this instance delegates to</summary>
    private ContentManager contentManager;
  
  }

} // namespace Nuclex.Game.Content
