#pragma region CPL License
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
#pragma endregion

#pragma once

#include "RectanglePacker.h"

namespace Nuclex { namespace Fonts { namespace Content {

  /// <summary>Packer using a custom algorithm by Markus 'Cygon' Ewald</summary>
  /// <remarks>
  ///   <para>
  ///     Algorithm conceived by Markus Ewald (cygon at nuclex dot org), thought
  ///     I'm quite sure I'm not the first one to come up with it :)
  ///   </para>
  ///   <para>
  ///     The algorithm always places rectangles as low as possible in the packing
  ///     area. So, for any new rectangle that is to be added into the packing area,
  ///     the packer has to determine the X coordinate at which the rectangle can have
  ///     the lowest overall height without intersecting any other rectangles.
  ///   </para>
  ///   <para>
  ///     To quickly discover these locations, the packer uses a sophisticated
  ///     data structure that stores the upper silhouette of the packing area. When
  ///     a new rectangle needs to be added, only the silouette edges need to be
  ///     analyzed to find the position where the rectangle would achieve the lowest
  ///     placement possible in the packing area.
  ///   </para>
  /// </remarks>
  public ref class CygonRectanglePacker : RectanglePacker {

    #pragma region class SliceStartComparer

    /// <summary>Compares the starting position of height slices</summary>
    private: ref class SliceStartComparer : System::Collections::Generic::IComparer<
      Microsoft::Xna::Framework::Point
    > {

      /// <summary>Provides a default instance for the anchor rank comparer</summary>
      public: static SliceStartComparer ^Default = gcnew SliceStartComparer();

      /// <summary>Compares the starting position of two height slices</summary>
      /// <param name="left">Left slice start that will be compared</param>
      /// <param name="right">Right slice start that will be compared</param>
      /// <returns>The relation of the two slice starts ranks to each other</returns>
      public: virtual int Compare(
        Microsoft::Xna::Framework::Point left, Microsoft::Xna::Framework::Point right
      ) {
        return left.X - right.X;
      }

    };

    #pragma endregion

    /// <summary>Initializes a new rectangle packer</summary>
    /// <param name="packingAreaWidth">Maximum width of the packing area</param>
    /// <param name="packingAreaHeight">Maximum height of the packing area</param>
    public: CygonRectanglePacker(int packingAreaWidth, int packingAreaHeight);

    /// <summary>Tries to allocate space for a rectangle in the packing area</summary>
    /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
    /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
    /// <param name="placement">Output parameter receiving the rectangle's placement</param>
    /// <returns>True if space for the rectangle could be allocated</returns>
    public: virtual System::Boolean TryPack(
      int rectangleWidth, int rectangleHeight,
      [System::Runtime::InteropServices::Out] Microsoft::Xna::Framework::Point %placement
    ) override;

    /// <summary>Finds the best position for a rectangle of the given dimensions</summary>
    /// <param name="rectangleWidth">Width of the rectangle to find a position for</param>
    /// <param name="rectangleHeight">Height of the rectangle to find a position for</param>
    /// <param name="placement">Receives the best placement found for the rectangle</param>
    /// <returns>True if a valid placement for the rectangle could be found</returns>
    private: System::Boolean tryFindBestPlacement(
      int rectangleWidth, int rectangleHeight,
      [System::Runtime::InteropServices::Out] Microsoft::Xna::Framework::Point %placement
    );

    /// <summary>Integrates a new rectangle into the height slice table</summary>
    /// <param name="left">Position of the rectangle's left side</param>
    /// <param name="width">Width of the rectangle</param>
    /// <param name="bottom">Position of the rectangle's lower side</param>
    private: void integrateRectangle(int left, int width, int bottom);

    /// <summary>Stores the height silhouette of the rectangles</summary>
    private: System::Collections::Generic::List<
      Microsoft::Xna::Framework::Point
    > ^heightSlices;

  };

}}} // namespace Nuclex::Fonts::Content
