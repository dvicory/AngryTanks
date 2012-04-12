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

#include "CygonRectanglePacker.h"

using namespace Microsoft::Xna::Framework;
using namespace System::Collections::Generic;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  CygonRectanglePacker::CygonRectanglePacker(
    int packingAreaWidth, int packingAreaHeight
  ) : RectanglePacker(packingAreaWidth, packingAreaHeight) {

    this->heightSlices = gcnew List<Point>();

    // At the beginning, the packing area is a single slice of height 0
    this->heightSlices->Add(Point(0, 0));

  }

  // ------------------------------------------------------------------------------------------- //

  System::Boolean CygonRectanglePacker::TryPack(
    int rectangleWidth, int rectangleHeight,
    [System::Runtime::InteropServices::Out] Microsoft::Xna::Framework::Point %placement
  ) {
    // If the rectangle is larger than the packing area in any dimension,
    // it will never fit!
    if(
      (rectangleWidth > PackingAreaWidth) || (rectangleHeight > PackingAreaHeight)
    ) {
      placement = Point::Zero;
      return false;
    }

    // Determine the placement for the new rectangle
    bool fits = tryFindBestPlacement(rectangleWidth, rectangleHeight, placement);

    // If a place for the rectangle could be found, update the height slice table to
    // mark the region of the rectangle as being taken.
    if(fits)
      integrateRectangle(placement.X, rectangleWidth, placement.Y + rectangleHeight);

    return fits;
  }

  // ------------------------------------------------------------------------------------------- //

  System::Boolean CygonRectanglePacker::tryFindBestPlacement(
    int rectangleWidth, int rectangleHeight,
    [System::Runtime::InteropServices::Out] Microsoft::Xna::Framework::Point %placement
  ) {

    // Slice index, vertical position and score of the best placement we could find
    int bestSliceIndex = -1; // Slice index where the best placement was found
    int bestSliceY = 0; // Y position of the best placement found
    int bestScore = PackingAreaWidth * PackingAreaHeight; // lower == better!

    // This is the counter for the currently checked position. The search works by
    // skipping from slice to slice, determining the suitability of the location for the
    // placement of the rectangle.
    int leftSliceIndex = 0;

    // Determine the slice in which the right end of the rectangle is located
    int rightSliceIndex = this->heightSlices->BinarySearch(
      Point(rectangleWidth, 0), SliceStartComparer::Default
    );
    if(rightSliceIndex < 0)
      rightSliceIndex = ~rightSliceIndex;

    while(rightSliceIndex <= this->heightSlices->Count) {

      // Determine the highest slice within the slices covered by the rectangle at
      // its current placement. We cannot put the rectangle any lower than this without
      // overlapping the other rectangles.
      int highest = this->heightSlices[leftSliceIndex].Y;
      for(int index = leftSliceIndex + 1; index < rightSliceIndex; ++index)
        if(this->heightSlices[index].Y > highest)
          highest = this->heightSlices[index].Y;

      // Only process this position if it doesn't leave the packing area
      if((highest + rectangleHeight < PackingAreaHeight)) {
        int score = highest;

        if(score < bestScore) {
          bestSliceIndex = leftSliceIndex;
          bestSliceY = highest;
          bestScore = score;
        }
      }

      // Advance the starting slice to the next slice start
      ++leftSliceIndex;
      if(leftSliceIndex >= this->heightSlices->Count)
        break;

      // Advance the ending slice until we're on the proper slice again, given the new
      // starting position of the rectangle.
      int rightRectangleEnd = this->heightSlices[leftSliceIndex].X + rectangleWidth;
      for(; rightSliceIndex <= this->heightSlices->Count; ++rightSliceIndex) {
        int rightSliceStart;
        if(rightSliceIndex == this->heightSlices->Count)
          rightSliceStart = PackingAreaWidth;
        else
          rightSliceStart = this->heightSlices[rightSliceIndex].X;

        // Is this the slice we're looking for?
        if(rightSliceStart > rightRectangleEnd)
          break;
      }

      // If we crossed the end of the slice array, the rectangle's right end has left
      // the packing area, and thus, our search ends.
      if(rightSliceIndex > this->heightSlices->Count)
        break;

    } // while rightSliceIndex <= this->heightSlices->Count

    // Return the best placement we found for this rectangle. If the rectangle
    // didn't fit anywhere, the slice index will still have its initialization value
    // of -1 and we can report that no placement could be found.
    if(bestSliceIndex == -1) {
      placement = Point::Zero;
      return false;
    } else {
      placement = Point(this->heightSlices[bestSliceIndex].X, bestSliceY);
      return true;
    }

  }

  // ------------------------------------------------------------------------------------------- //

  void CygonRectanglePacker::integrateRectangle(int left, int width, int bottom) {

    // Find the first slice that is touched by the rectangle
    int startSlice = this->heightSlices->BinarySearch(
      Point(left, 0), SliceStartComparer::Default
    );
    int firstSliceOriginalHeight;

    // Did we score a direct hit on an existing slice start?
    if(startSlice >= 0) {

      // We scored a direct hit, so we can replace the slice we have hit
      firstSliceOriginalHeight = this->heightSlices[startSlice].Y;
      this->heightSlices[startSlice] = Point(left, bottom);

    } else { // No direct hit, slice starts inside another slice

      // Add a new slice after the slice in which we start
      startSlice = ~startSlice;
      firstSliceOriginalHeight = this->heightSlices[startSlice - 1].Y;
      this->heightSlices->Insert(startSlice, Point(left, bottom));

    }

    int right = left + width;
    ++startSlice;

    // Special case, the rectangle started on the last slice, so we cannot
    // use the start slice + 1 for the binary search and the possibly already
    // modified start slice height now only remains in our temporary
    // firstSliceOriginalHeight variable
    if(startSlice >= this->heightSlices->Count) {

      // If the slice ends within the last slice (usual case, unless it has the
      // exact same width the packing area has), add another slice to return to
      // the original height at the end of the rectangle.
      if(right < PackingAreaWidth)
        this->heightSlices->Add(Point(right, firstSliceOriginalHeight));

    } else { // The rectangle doesn't start on the last slice

      int endSlice = this->heightSlices->BinarySearch(
        startSlice, this->heightSlices->Count - startSlice,
        Point(right, 0), SliceStartComparer::Default
      );

      // Another direct hit on the final slice's end?
      if(endSlice > 0) {

        this->heightSlices->RemoveRange(startSlice, endSlice - startSlice);

      } else { // No direct hit, rectangle ends inside another slice

        // Make index from negative BinarySearch() result
        endSlice = ~endSlice;

        // Find out to which height we need to return at the right end of
        // the rectangle
        int returnHeight;
        if(endSlice == startSlice)
          returnHeight = firstSliceOriginalHeight;
        else
          returnHeight = this->heightSlices[endSlice - 1].Y;

        // Remove all slices covered by the rectangle and begin a new slice at its end
        // to return back to the height of the slice on which the rectangle ends.
        this->heightSlices->RemoveRange(startSlice, endSlice - startSlice);
        if(right < PackingAreaWidth)
          this->heightSlices->Insert(startSlice, Point(right, returnHeight));

      } // if endSlice > 0

    } // if startSlice >= this.heightSlices.Count

  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
