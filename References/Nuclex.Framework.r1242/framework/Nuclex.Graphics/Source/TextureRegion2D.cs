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

namespace Nuclex.Graphics {

  /// <summary>Represents a limited region of a texture</summary>
  /// <remarks>
  ///   This is similar to .NET's ArraySegment class, only that it applies to
  ///   a two-dimensional texture instead of a one-dimensional array.
  /// </remarks>
  public struct TextureRegion2D {

    /// <summary>Initializes a new texture region using a full texture</summary>
    /// <param name="texture">Texture that will be used in full</param>
    public TextureRegion2D(Texture2D texture) :
      this(texture, Vector2.Zero, Vector2.One) { }

    /// <summary>Initializes a new texture region using part of a texture</summary>
    /// <param name="texture">Texture of which a region will be used</param>
    /// <param name="minX">Minimum X texture coordinate (normalized) of the region</param>
    /// <param name="minY">Minimum Y texture coordinate (normalized) of the region</param>
    /// <param name="maxX">Maximum X texture coordinate (normalized) of the region</param>
    /// <param name="maxY">Maximum Y texture coordinate (normalized) of the region</param>
    public TextureRegion2D(
      Texture2D texture, float minX, float minY, float maxX, float maxY
    ) :
      this(texture, new Vector2(minX, minY), new Vector2(maxX, maxY)) { }

    /// <summary>Initializes a new texture region using part of a texture</summary>
    /// <param name="texture">Texture of which a region will be used</param>
    /// <param name="min">Minimum texture coordinates (normalized) of the region</param>
    /// <param name="max">Maximum texture coordinates (normalized) of the region</param>
    public TextureRegion2D(Texture2D texture, Vector2 min, Vector2 max) {
      this.Texture = texture;
      this.Min = min;
      this.Max = max;
    }

    /// <summary>Initializes a new texture region using part of a texture</summary>
    /// <param name="texture">Texture of which a region will be used</param>
    /// <param name="min">Minimum texture coordinates of the region</param>
    /// <param name="max">Maximum texture coordinates of the region</param>
    public static TextureRegion2D FromTexels(Texture2D texture, Point min, Point max) {
      float width = (float)texture.Width;
      float height = (float)texture.Height;

      return new TextureRegion2D(
        texture, min.X / width, min.Y / height, max.X / width, max.Y / height
      );
    }

    /// <summary>Initializes a new texture region using part of a texture</summary>
    /// <param name="texture">Texture of which a region will be used</param>
    /// <param name="minX">Minimum X texture coordinate of the region</param>
    /// <param name="minY">Minimum Y texture coordinate of the region</param>
    /// <param name="maxX">Maximum X texture coordinate of the region</param>
    /// <param name="maxY">Maximum Y texture coordinate of the region</param>
    public static TextureRegion2D FromTexels(
      Texture2D texture, int minX, int minY, int maxX, int maxY
    ) {
      float width = (float)texture.Width;
      float height = (float)texture.Height;

      return new TextureRegion2D(
        texture, minX / width, minY / height, maxX / width, maxY / height
      );
    }

    /// <summary>Returns the hash code for the current instance</summary>
    /// <returns>A 32-bit signed integer hash code</returns>
    public override int GetHashCode() {
      return
        this.Texture.GetHashCode() ^
        this.Min.GetHashCode() ^
        this.Max.GetHashCode();
    }

    /// <summary>
    ///   Determines whether the specified object is equal to the current instance
    /// </summary>
    /// <returns>
    ///   True if the specified object is a <see cref="TextureRegion2D" /> structure and is
    ///   equal to the current instance; otherwise, false
    /// </returns>
    /// <param name="other">The object to be compared with the current instance</param>
    public override bool Equals(object other) {
      return
        (other is TextureRegion2D) &&
        this.Equals((TextureRegion2D)other);
    }

    /// <summary>
    ///   Determines whether the specified <see cref="TextureRegion2D" /> structure is equal
    ///   to the current instance
    /// </summary>
    /// <returns>
    ///   True if the specified <see cref="TextureRegion2D" /> structure is equal to the
    ///   current instance; otherwise, false
    /// </returns>
    /// <param name="other">
    ///   The <see cref="TextureRegion2D" /> structure to be compared with
    ///   the current instance
    /// </param>
    public bool Equals(TextureRegion2D other) {
      return
        (other.Texture == this.Texture) &&
        (other.Min == this.Min) &&
        (other.Max == this.Max);
    }

    /// <summary>
    ///   Indicates whether two <see cref="TextureRegion2D" /> structures are equal
    /// </summary>
    /// <returns>True if a is equal to b; otherwise, false</returns>
    /// <param name="left">
    ///   The <see cref="TextureRegion2D" /> structure on the left side of the
    ///   equality operator
    /// </param>
    /// <param name="right">
    ///   The <see cref="TextureRegion2D" /> structure on the right side of the
    ///   equality operator
    /// </param>
    public static bool operator ==(TextureRegion2D left, TextureRegion2D right) {
      return
        (left.Texture == right.Texture) &&
        (left.Min == right.Min) &&
        (left.Max == right.Max);
    }

    /// <summary>
    ///   Indicates whether two <see cref="TextureRegion2D" /> structures are unequal
    /// </summary>
    /// <returns>True if a is not equal to b; otherwise, false</returns>
    /// <param name="left">
    ///   The <see cref="TextureRegion2D" /> structure on the left side of the
    ///   inequality operator
    /// </param>
    /// <param name="right">
    ///   The <see cref="TextureRegion2D" /> structure on the right side of the
    ///   inequality operator
    /// </param>
    public static bool operator !=(TextureRegion2D left, TextureRegion2D right) {
      return
        (left.Texture != right.Texture) ||
        (left.Min != right.Min) ||
        (left.Max != right.Max);
    }

    /// <summary>Returns a string representation of the string segment</summary>
    /// <returns>The string representation of the string segment</returns>
    public override string ToString() {
      string textureString = this.Texture.ToString();
      string minString = this.Min.ToString();
      string maxString = this.Max.ToString();

      StringBuilder stringBuilder = new StringBuilder(
        textureString.Length + 2 + minString.Length + 3 + maxString.Length + 1
      );
      
      stringBuilder.Append(textureString);
      stringBuilder.Append(" {");
      stringBuilder.Append(minString);
      stringBuilder.Append(" - ");
      stringBuilder.Append(maxString);
      stringBuilder.Append('}');
      
      return stringBuilder.ToString();
    }

    /// <summary>Texture from which a region is used</summary>
    public Texture2D Texture;

    /// <summary>Minimum coordinates of the region on the texture</summary>
    /// <remarks>
    ///   These coordinates are normalized: 0,0 is the lower left corner and
    ///   1,1 is the upper right corner.
    /// </remarks>
    public Vector2 Min;
    /// <summary>Maximum coordinates of the region on the texture</summary>
    /// <remarks>
    ///   These coordinates are normalized: 0,0 is the lower left corner and
    ///   1,1 is the upper right corner.
    /// </remarks>
    public Vector2 Max;

  }

} // namespace Nuclex.Graphics
