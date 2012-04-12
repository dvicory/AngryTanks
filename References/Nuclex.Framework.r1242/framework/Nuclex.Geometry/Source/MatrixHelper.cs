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
using Microsoft.Xna.Framework;

namespace Nuclex.Geometry {

  /// <summary>Provides helper methods for working with matrices</summary>
  internal static class MatrixHelper {

    /// <summary>Creates a new matrix from the given directional vectors</summary>
    /// <param name="right">Vector that's pointing to the right in the matrix</param>
    /// <param name="up">Vector that's pointing upwards in the matrix</param>
    /// <param name="backward">Vector that's pointing backwards in the matrix</param>
    /// <returns>The new matrix</returns>
    public static Matrix Create(Vector3 right, Vector3 up, Vector3 backward) {
      return new Matrix(
        right.X, right.Y, right.Z, 0.0f,
        up.X, up.Y, up.Z, 0.0f,
        backward.X, backward.Y, backward.Z, 0.0f,
        0.0f, 0.0f, 0.0f, 1.0f
      );
    }

    /// <summary>
    ///   Creates a new matrix from the given directional vectors and a translational
    /// </summary>
    /// <param name="translation">Positional translation done by the matrix</param>
    /// <param name="right">Vector that's pointing to the right in the matrix</param>
    /// <param name="up">Vector that's pointing upwards in the matrix</param>
    /// <param name="backward">Vector that's pointing backwards in the matrix</param>
    /// <returns>The new matrix</returns>
    public static Matrix Create(
      Vector3 translation, Vector3 right, Vector3 up, Vector3 backward
    ) {
      return new Matrix(
        right.X, right.Y, right.Z, 0.0f,
        up.X, up.Y, up.Z, 0.0f,
        backward.X, backward.Y, backward.Z, 0.0f,
        translation.X, translation.Y, translation.Z, 1.0f
      );
    }

    /// <summary>Retrieves a row of the matrix as a vector</summary>
    /// <param name="matrix">Matrix of which to retrieve a row</param>
    /// <param name="row">Index of the row that will be retrieved</param>
    /// <returns>The vector built from the requested matrix row</returns>
    public static Vector3 Get(ref Matrix matrix, int row) {
      switch(row) {
        case 0: { return new Vector3(matrix.M11, matrix.M12, matrix.M13); }
        case 1: { return new Vector3(matrix.M21, matrix.M22, matrix.M23); }
        case 2: { return new Vector3(matrix.M31, matrix.M32, matrix.M33); }
        case 3: { return new Vector3(matrix.M41, matrix.M42, matrix.M43); }
        default: {
          throw new ArgumentOutOfRangeException("Matrix row index out of range");
        }
      }
    }

    /// <summary>Retrieves an element of the matrix by its column and row index</summary>
    /// <param name="matrix">Matrix of which to retrieve an element</param>
    /// <param name="row">Index of the row from which to retrieve the element</param>
    /// <param name="col">Index of the column to retrieve</param>
    /// <returns>The element at the given row and column</returns>
    public static float Get(ref Matrix matrix, int row, int col) {
      if((col < 0) || (col > 3)) {
        throw new ArgumentOutOfRangeException("col", "Matrix column index out of range");
      }

      switch(row << 2 | col) {
        case 0: { return matrix.M11; }
        case 1: { return matrix.M12; }
        case 2: { return matrix.M13; }
        case 3: { return matrix.M14; }

        case 4: { return matrix.M21; }
        case 5: { return matrix.M22; }
        case 6: { return matrix.M23; }
        case 7: { return matrix.M24; }

        case 8: { return matrix.M31; }
        case 9: { return matrix.M32; }
        case 10: { return matrix.M33; }
        case 11: { return matrix.M34; }

        case 12: { return matrix.M41; }
        case 13: { return matrix.M42; }
        case 14: { return matrix.M43; }
        case 15: { return matrix.M44; }

        default: {
          throw new ArgumentOutOfRangeException("row", "Matrix row index out of range");
        }
      }
    }

    /// <summary>Sets a row of the matrix from a vector</summary>
    /// <param name="matrix">Matrix in which to set a row</param>
    /// <param name="row">Index of the row that will be set</param>
    /// <param name="values">Vector containing the values to assign to the row</param>
    public static void Set(ref Matrix matrix, int row, Vector3 values) {
      switch(row) {
        case 0: {
          matrix.M11 = values.X;
          matrix.M12 = values.Y;
          matrix.M13 = values.Z;
          break;
        }
        case 1: {
          matrix.M21 = values.X;
          matrix.M22 = values.Y;
          matrix.M23 = values.Z;
          break;
        }
        case 2: {
          matrix.M31 = values.X;
          matrix.M32 = values.Y;
          matrix.M33 = values.Z;
          break;
        }
        case 3: {
          matrix.M41 = values.X;
          matrix.M42 = values.Y;
          matrix.M43 = values.Z;
          break;
        }
        default: {
          throw new ArgumentOutOfRangeException("Matrix row index out of range");
        }
      }
    }

    /// <summary>Set an element of the matrix by its column and row index</summary>
    /// <param name="matrix">Matrix in which to set an element</param>
    /// <param name="row">Index of the row in which to set an element</param>
    /// <param name="col">Index of the column to set</param>
    /// <param name="value">
    ///   Value to set to the matrix element at the specified row and column
    /// </param>
    public static void Set(ref Matrix matrix, int row, int col, float value) {
      if((col < 0) || (col > 3)) {
        throw new ArgumentOutOfRangeException("col", "Matrix column index out of range");
      }

      switch(row << 2 | col) {
        case 0: { matrix.M11 = value; break; }
        case 1: { matrix.M12 = value; break; }
        case 2: { matrix.M13 = value; break; }
        case 3: { matrix.M14 = value; break; }

        case 4: { matrix.M21 = value; break; }
        case 5: { matrix.M22 = value; break; }
        case 6: { matrix.M23 = value; break; }
        case 7: { matrix.M24 = value; break; }

        case 8: { matrix.M31 = value; break; }
        case 9: { matrix.M32 = value; break; }
        case 10: { matrix.M33 = value; break; }
        case 11: { matrix.M34 = value; break; }

        case 12: { matrix.M41 = value; break; }
        case 13: { matrix.M42 = value; break; }
        case 14: { matrix.M43 = value; break; }
        case 15: { matrix.M44 = value; break; }

        default: {
          throw new ArgumentOutOfRangeException("row", "Matrix row index out of range");
        }
      }
    }

  }

} // namespace Nuclex.Geometry
