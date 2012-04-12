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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics.Effects {

#if false

  /// <summary>Caches bitmaps on large textures to reduce texture switches</summary>
  internal class TextureCache : IDisposable {

    #region struct Entry

    /// <summary>Used to return a looked up cache entry</summary>
    public struct Entry {

      /// <summary>Initializes a cache entry</summary>
      /// <param name="texture">Texture on which the cached bitmap is located</param>
      /// <param name="upperLeft">Upper left texture coordinates of the cached bitmap</param>
      /// <param name="lowerRight">Lower right texture coordinates of the cached bitmap</param>
      public Entry(Texture2D texture, Vector2 upperLeft, Vector2 lowerRight) {
        this.Texture = texture;
        this.UpperLeft = upperLeft;
        this.LowerRight = lowerRight;
      }

      /// <summary>Texture containing the cached bitmap</summary>
      public Texture2D Texture;
      /// <summary>Upper left texture coordinate of the bitmap</summary>
      public Vector2 UpperLeft;
      /// <summary>Lower right texture coordinate of the bitmap</summary>
      public Vector2 LowerRight;

    }

    #endregion

    #region class SharedTexture

    /// <summary>
    ///   A texture which is shared by multiple smaller texture to increase
    ///   rendering efficiency
    /// </summary>
    private class SharedTexture {

      /// <summary>Initializes a new shared texture</summary>
      /// <param name="graphicsDevice">Graphics device on which to create the texture</param>
      /// <param name="resolution">Desired size of the shared texture</param>
      public SharedTexture(GraphicsDevice graphicsDevice, Point resolution) {

        this.Texture = new Texture2D(
          graphicsDevice,
          resolution.X,
          resolution.Y,
          1,
          TextureUsage.None,
          SurfaceFormat.Color
        );

      }

      /// <summary>Allocates space within the shared texture</summary>
      /// <param name="size">Amount of space to be allocated</param>
      /// <returns>The location of the new sub texture</returns>
      public Point AllocateSpace(Point size) {
        if((size.X > this.Texture.Width) || (size.Y > this.Texture.Height))
          throw new ArgumentException("Region is too large for the shared texture");

        // Do we have to start a new line ?
        if(this.column + size.X > this.Texture.Width) {
          this.currentLine += this.lineHeight;
          this.lineHeight = 0;
          this.column = 0;
        }

        // If it doesn't fit vertically now, the packing area is full
        if(this.currentLine + size.Y > this.Texture.Height)
          throw new InvalidOperationException(
            "Shared texture does not have enough space available"
          );

        Point Location = new Point(this.column, this.currentLine);

        this.column += size.X; // Can be larger than cache width till next run

        if(size.Y > this.lineHeight)
          this.lineHeight = size.Y;

        return Location;
      }

      /// <summary>XNA texture storing the cached bitmaps</summary>
      public Texture2D Texture;
      /// <summary>Current packing line</summary>
      private int currentLine;
      /// <summary>Height of the current packing line</summary>
      private int lineHeight;
      /// <summary>Current column in the current packing line</summary>
      private int column;

    }

    #endregion

    /// <summary>Initializes a new glyph bitmap cache</summary>
    /// <param name="graphicsDevice">Graphics device to use for rendering</param>
    public TextureCache(GraphicsDevice graphicsDevice) {
      this.graphicsDevice = graphicsDevice;

      // Look up the maximum texture size reported to be supported by the device
      this.textureResolution = new Point(
        graphicsDevice.GraphicsDeviceCapabilities.MaxTextureWidth,
        graphicsDevice.GraphicsDeviceCapabilities.MaxTextureHeight
      );

      // Cap to 4 MB per cache texture so we don't become a memory hog for
      // modern graphics cards that support insane texture resoutions.
      this.textureResolution.X = Math.Min(this.textureResolution.X, 1024);
      this.textureResolution.Y = Math.Min(this.textureResolution.Y, 1024);

      // Create the collections used to store and look up glyphs
      this.locations = new Dictionary<object, Entry>();
      this.cacheTextures = new List<SharedTexture>();

      // Set up the first cache texture
      addCacheTexture();
    }

    /// <summary>Explicitely releases all resources used by an instance</summary>
    public void Dispose() {

      // Release all the textures
      if(this.cacheTextures != null) {
        foreach(SharedTexture sharedTexture in this.cacheTextures)
          sharedTexture.Texture.Dispose();

        // Make sure nobody can try to use this instance afterwards
        this.cacheTextures = null;
        this.locations = null;
      }

    }
/*
    /// <summary>Looks up a cached glyph or puts it into   the cache</summary>
    /// <param name="glyph">Glyph to be looked up</param>
    /// <returns>A cache entry with the requested glyph bitmap</returns>
    public Entry LookUp(GlyphBitmap glyph) {
      Entry theEntry;
      
      // Check if we already got this one cached. If so, just return it.
      if(this.locations.TryGetValue(glyph, out theEntry))
        return theEntry;

      // First, we need to find a location within the cache where the surface can be stored
      Point location;
      try {
        location = this.currentCacheTexture.AllocateSpace(new Point(glyph.Width, glyph.Height));
      }
      catch(InvalidOperationException) {
        addCacheTexture();
        location = this.currentCacheTexture.AllocateSpace(new Point(glyph.Width, glyph.Height));
      }

#if XBOX360

      // First, I wanted to use NoOverwrite flag to tell the graphics driver I'm only
      // updating before unused regions on the texture. The call failed. Then I simply
      // tried setting a region of the texture. The call worked on the PC but failed
      // on the XBox. Obviously the XBox can not update smaller regions within a
      // texture and you are forced to modify the whole damn thing. This makes the entire
      // texture cache idea become contra-productive and slow.
      {
        for(int line = 0; line < glyph.Height; ++line) {
          Array.Copy(
            glyph.Bitmap, line * glyph.Width * 4,
            this.currentCacheTexture.TextureData,
            ((location.Y + line) * this.currentCacheTexture.Texture.Width + location.X) * 4,
            glyph.Width * 4
          );
        }

        this.currentCacheTexture.Texture.SetData<byte>(this.currentCacheTexture.TextureData);
      }

#else

      // Copy the glyph bitmap into the cache texture.
      this.currentCacheTexture.Texture.SetData<byte>(
        0,
        new Rectangle(location.X, location.Y, glyph.Width, glyph.Height),
        glyph.Bitmap,
        0,
        glyph.Width * glyph.Height * 4,
        SetDataOptions.None// NoOverwrite
      );

#endif

      // Prepare the cache entry
      theEntry = new Entry(
        this.currentCacheTexture.Texture,
        new Vector2(
          (float)location.X / (float)this.textureResolution.X,
          (float)location.Y / (float)this.textureResolution.Y
        ),
        new Vector2(
          (float)(location.X + glyph.Width) / (float)this.textureResolution.X,
          (float)(location.Y + glyph.Height) / (float)this.textureResolution.Y
        )
      );
      this.locations.Add(glyph, theEntry);

      // All done, hand out the cache entry
      return theEntry;
    }
*/
    /// <summary>Adds a caching texture to the cache</summary>
    private void addCacheTexture() {
      this.cacheTextures.Add(new SharedTexture(this.graphicsDevice, this.textureResolution));
      this.currentCacheTexture = this.cacheTextures[this.cacheTextures.Count - 1];
    }

    /// <summary>The graphics device on which the cache operates</summary>
    private GraphicsDevice graphicsDevice;
    /// <summary>Locations of the cached bitmaps on the cache texture</summary>
    private Dictionary<object, Entry> locations;
    /// <summary>The optimal resolution of cache textures</summary>
    private Point textureResolution;
    /// <summary>The intermediate textures used by the cache</summary>
    private List<SharedTexture> cacheTextures;
    /// <summary>The current cache texture</summary>
    private SharedTexture currentCacheTexture;
  }

#endif

} // namespace Nuclex.SpecialFx
