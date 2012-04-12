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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Nuclex.Game.Content {

  /// <summary>Interface for the shared content access service</summary>
  /// <remarks>
  ///   The shared content access service provides game-wide access to shared assets
  ///   (meaning assets that appear throughout the game, such as UI graphics, weapon
  ///   models and fonts for example). Once integrated into your game, jsut use this
  ///   service whenever you need to access any of the game's global assets.
  /// </remarks>
  public interface ISharedContentService {

    /// <summary>Loads or accesses shared game content</summary>
    /// <typeparam name="AssetType">Type of the asset to be loaded or accessed</typeparam>
    /// <param name="assetName">Path and name of the requested asset</param>
    /// <returns>The requested asset from the the shared game content store</returns>
    AssetType Load<AssetType>(string assetName);

  }

} // namespace Nuclex.Game.Content


