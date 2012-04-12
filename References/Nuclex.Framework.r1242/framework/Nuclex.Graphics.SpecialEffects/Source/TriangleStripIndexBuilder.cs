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

namespace Nuclex.Graphics.SpecialEffects {

  /// <summary>Builds index lists for drawing vertex grids as triangle strips</summary>
  public static class TriangleStripIndexBuilder {

    /// <summary>
    ///   Calculates the number of vertex indices required to draw an alternating
    ///   triangle strip of the requested size
    /// </summary>
    /// <param name="segmentsX">
    ///   Number of segments the strip should have on the X axis
    /// </param>
    /// <param name="segmentsZ">
    ///   Number of segments the strip should have on the Y axis
    /// </param>
    /// <returns>The number of indices required for the strip</returns>
    public static int CountAlternatingStripIndices(int segmentsX, int segmentsZ) {
      int indicesPerRow = (segmentsX + 1) * 2 - 1;

      return segmentsZ * indicesPerRow + 1;
    }

    /// <summary>
    ///   Builds the index buffer for a grid of vertices drawn as a triangle strip
    /// </summary>
    /// <param name="segmentsX">Number of horizontal subdivisions in the plane</param>
    /// <param name="segmentsZ">Number of vertical subdivisions in the plane</param>
    /// <remarks>
    ///   <para>
    ///     This method builds indices to draw a grid of vertices with alternating
    ///     split diagonals for each line of quads.
    ///   </para>
    ///   <code>
    ///     k-l-m-n-o
    ///     |/|/|/|/|
    ///     f-g-h-i-j
    ///     |\|\|\|\|
    ///     a-b-c-d-e
    ///     |/|/|/|/|
    ///     5-6-7-8-9
    ///     |\|\|\|\|
    ///     0-1-2-3-4
    ///   </code>
    ///   <para>
    ///     Rows are drawn alternating diretion between left to right and 
    ///     right to left. Only a single, small degenerate triangle is required
    ///     between two rows to move into the next row, resulting in (segmentsZ - 1)
    ///     degenerate triangles for the whole grid.
    ///   </para>
    ///   <para>
    ///     This method is ideal for graphics cards with a limited vertex cache
    ///     because it only requires space for (segmentsX + 1) vertices in
    ///     the vertex cache to reach optimum caching performance.
    ///   </para>
    /// </remarks>
    public static short[] BuildAlternatingStrip(int segmentsX, int segmentsZ) {
      if(segmentsX < 1) {
        throw new ArgumentException("Too few segments in X direction", "segmentsX");
      }
      if(segmentsZ < 1) {
        throw new ArgumentException("Too few segments in Z direction", "segmentsX");
      }

      // Calculate the total number of indices we'll have
      int totalIndices = CountAlternatingStripIndices(segmentsX, segmentsZ);

      // Number of vertices in one row
      int verticesX = segmentsX + 1;

      // Set up the index array
      short[] indices = new short[totalIndices];
      int indicesIndex = 1; // Current index in the vertex index array :)

      // This is how the indices will be generated:
      //
      //  x      0          1          2          3
      //
      //                                      k l m n o
      //                                      |/|/|/|/|
      //                backwards  f g h i j  f g h i  
      //                           |\|\|\|\|
      //      forwards  a b c d e    b c d e  backwards
      //                |/|/|/|/|
      //     5 6 7 8 9  5 6 7 8     forwards
      //     |\|\|\|\|      
      //  0    1 2 3 4
      //
      for(int row = 0; row < segmentsZ; ++row) {
        bool evenRow = ((row & 1) == 0);

        // Even row: Build sawtooths from left to right
        if(evenRow) {

          int rowStartIndex = row * verticesX + 1;
          int rowEndIndex = rowStartIndex + segmentsX;
          for(int index = rowStartIndex; index < rowEndIndex; ++index) {
            indices[indicesIndex++] = (short)(index + segmentsX);
            indices[indicesIndex++] = (short)(index);
          }
          indices[indicesIndex++] = (short)(rowEndIndex + segmentsX);

        } else { // Odd row: Build sawtooths from right to left

          int rowEndIndex = row * verticesX + verticesX;
          int rowStartIndex = rowEndIndex + segmentsX;
          for(int index = rowStartIndex; index > rowEndIndex; --index) {
            indices[indicesIndex++] = (short)(index);
            indices[indicesIndex++] = (short)(index - verticesX - 1);
          }
          indices[indicesIndex++] = (short)(rowEndIndex);

        }
      }

      return indices;
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects
