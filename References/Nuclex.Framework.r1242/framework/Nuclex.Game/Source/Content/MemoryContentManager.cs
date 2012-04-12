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
#if !XBOX360
using System.Security.Cryptography;
using System.Text;
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Nuclex.Graphics;

namespace Nuclex.Game.Content {

  /// <summary>Content manager for loading content from in-memory arrays</summary>
  /// <remarks>
  ///   This is not much different from the resource content manager, since resources
  ///   are always loaded into a process and are "in-memory" as well, but this
  ///   content manager allows you to load content directly out of byte arrays.
  /// </remarks>
  public class MemoryContentManager : ContentManager {

    /// <summary>
    ///   Initializes a new embedded content manager using a directly specified
    ///   graphics device service for the resources.
    /// </summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service to load the content asset in
    /// </param>
    public MemoryContentManager(IGraphicsDeviceService graphicsDeviceService) :
      this(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(graphicsDeviceService)
      ) { }

    /// <summary>
    ///   Initializes a new embedded content manager using the provided game services
    ///   container for providing services for the loaded asset.
    /// </summary>
    /// <param name="services">
    ///   Service container containing the services the asset may access
    /// </param>
    public MemoryContentManager(IServiceProvider services) :
      base(services) { }

#if !(XBOX360 || WINDOWS_PHONE)
    /// <summary>Loads the asset the embedded content manager was created for</summary>
    /// <typeparam name="AssetType">Type of the asset to load</typeparam>
    /// <param name="content">Content that will be loaded as an asset</param>
    /// <returns>The loaded asset</returns>
    /// <remarks>
    ///   <para>
    ///     To mirror the behavior of the ResourceContentManager class, this method
    ///     calculates the SHA-1 of the provided array. Otherwise, you could request
    ///     the same asset two times and when you dispose one, the other requested
    ///     instance would still work, which does not match the behavior of
    ///     the ResourceContentManager.
    ///   </para>
    ///   <para>
    ///     It is recommended that you use the named LoadAsset method or control
    ///     asset lifetimes yourself by using the ReadAsset() method.
    ///   </para>
    /// </remarks>
    public AssetType Load<AssetType>(byte[] content) {
      return Load<AssetType>(content, getSha1(content));
    }
#endif

    /// <summary>Loads the asset the embedded content manager was created for</summary>
    /// <typeparam name="AssetType">Type of the asset to load</typeparam>
    /// <param name="content">Content that will be loaded as an asset</param>
    /// <param name="uniqueName">Unique name of the resource</param>
    /// <returns>The loaded asset</returns>
    /// <remarks>
    ///   This class avoids the SHA-1 calculation under the promise that the caller
    ///   will provide a name that is unique for each loaded asset.
    /// </remarks>
    public AssetType Load<AssetType>(byte[] content, string uniqueName) {
      lock(this) {
        this.content = content;
        try {
          return base.Load<AssetType>(uniqueName);
        }
        finally {
          if(this.memoryStream != null) {
            this.memoryStream.Dispose();
            this.memoryStream = null;
          }
          this.content = null;
        }
      } // lock(this)
    }

    /// <summary>Loads an asset from the provided byte array</summary>
    /// <typeparam name="AssetType">Type of the asset to load</typeparam>
    /// <param name="content">Content that will be loaded as an asset</param>
    /// <returns>The loaded asset</returns>
    public AssetType ReadAsset<AssetType>(byte[] content) {
      lock(this) {
        using(this.memoryStream = new MemoryStream(content, false)) {
          return base.ReadAsset<AssetType>("null", null);
        }
      } // lock(this)
    }

    /// <summary>Opens a stream for reading the specified asset</summary>
    /// <param name="assetName">The name of the asset to be read</param>
    /// <returns>The opened stream for the asset</returns>
    protected override Stream OpenStream(string assetName) {
      if(this.memoryStream == null) {
        this.memoryStream = new MemoryStream(this.content, false);
      }

      return this.memoryStream;
    }

#if !(XBOX360 || WINDOWS_PHONE)
    /// <summary>Calculates the SHA-1 hash of the provided byte array</summary>
    /// <param name="data">Data that will be hashed</param>
    /// <returns>A string containing the SHA-1 sum of the byte array</returns>
    private string getSha1(byte[] data) {
      if(this.sha1HashProvider == null) {
        this.sha1HashProvider = SHA1.Create();
      }
      byte[] hashCode = this.sha1HashProvider.ComputeHash(data);
      
      StringBuilder builder = new StringBuilder(hashCode.Length * 2);
      for(int index = 0; index < hashCode.Length; ++index) {
        builder.AppendFormat("{0:x2}", hashCode[index]);
      }

      return builder.ToString();
    }
#endif

    /// <summary>Content that will be loaded by the embedded content manager</summary>
    private MemoryStream memoryStream;
    /// <summary>Content which is currently being loaded</summary>
    private byte[] content;
#if !(XBOX360 && !WINDOWS_PHONE)
    /// <summary>
    ///   SHA-1 hash provider used to calculate SHA-1 sums of asset data
    /// </summary>
    private SHA1 sha1HashProvider;
#endif

  }

} // namespace Nuclex.Game.Content
