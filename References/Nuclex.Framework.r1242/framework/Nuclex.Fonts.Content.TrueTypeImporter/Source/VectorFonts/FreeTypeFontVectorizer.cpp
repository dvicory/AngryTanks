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

#include "FreeTypeFontVectorizer.h"
#include "../clix.h"

#include <vector>

using namespace std;
using namespace clix;

using namespace System;
using namespace System::Collections::Generic;

using namespace Microsoft::Xna::Framework;
using namespace Microsoft::Xna::Framework::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  /// <summary>A vector of FreeType vertices</summary>
  typedef std::vector<FT_Vector> VertexVector;

  /// <summary>A vector of outline starting index / vertex count records</summary>
  typedef std::vector< std::pair<int, int> > OutlineVector;

  /// <summary>Internal structure used to collect a glyph's contour informations</summary>
  struct ContourSet {

    /// <summary>Initializes a new contour set</summary>
    public: ContourSet() : CurrentOutlineStartIndex(0) {}

    /// <summary>Vertices created for this font</summary>
    public: VertexVector Vertices;

    /// <summary>Contains the starting index and vertex count for each outline</summary>
    public: OutlineVector Outlines;

    /// <summary>Starting index of the current outline in the vertex array</summary>
    public: int CurrentOutlineStartIndex;

    /// <summary>Current position of the drawing pen</summary>
    public: FT_Vector PenPosition;

  }; // struct ContourSet

  // ------------------------------------------------------------------------------------------- //

  namespace { // anonymous

    /// <summary>Moves the pen position to the specified position</summary>
    /// <param name="to">Position to move the pen to</param>
    /// <param name="contourSetAddress">ContourSet to store the contour points in</param>
    int moveTo(const FT_Vector *to, void *contourSetAddress) {
      ContourSet &contourSet = *static_cast<ContourSet *>(contourSetAddress);

      // Only add a new contour if this isn't the initial moveTo() to start the character
      if(!contourSet.Vertices.empty()) {
        contourSet.Outlines.push_back(
          std::pair<int, int>(
            contourSet.CurrentOutlineStartIndex,
            contourSet.Vertices.size() - contourSet.CurrentOutlineStartIndex
          )
        );

        contourSet.CurrentOutlineStartIndex = contourSet.Vertices.size();
      }

      contourSet.PenPosition = *to;
      
      return 0;
    }

  } // anonymous namespace

  // ------------------------------------------------------------------------------------------- //

  namespace { // anonymous

    /// <summary>Draws a line to the specified position</summary>
    /// <param name="to">Position to draw a line to</param>
    /// <param name="contourSetAddress">ContourSet to store the contour points in</param>
    int lineTo(const FT_Vector *to, void *contourSetAddress) {
      ContourSet &contourSet = *static_cast<ContourSet *>(contourSetAddress);

      contourSet.Vertices.push_back(*to);
      contourSet.PenPosition = *to;

      return 0;
    }

  } // anonymous namespace

  // ------------------------------------------------------------------------------------------- //

  namespace { // anonymous

    /// <summary>Draws a conic bezier curve along the specified control point</summary>
    /// <param name="control">Control point for bezier curve's arc shape</param>
    /// <param name="to">Position to draw a conic bezier curve to</param>
    /// <param name="contourSetAddress">ContourSet to store the contour points in</param>
    int conicTo(
      const FT_Vector *control, const FT_Vector *to, void *contourSetAddress
    ) {
      ContourSet &contourSet = *static_cast<ContourSet *>(contourSetAddress);
      const FT_Vector &pen = contourSet.PenPosition;

      // Number of interpolated points to break the bezier curve down to. Maybe
      // this could be calculated by means of distance and sharpness of the bend,
      // but for now, a fixed number of segments is used.
      const int BezierStepCount = 3;

      // Create a matrix from the bezier curve's control points.
      float controlPoints[3][2] = {
        { static_cast<float>(pen.x), static_cast<float>(pen.y) },
        { static_cast<float>(control->x), static_cast<float>(control->y) },
        { static_cast<float>(to->x), static_cast<float>(to->y) }
      };

      // Break down the bezier curve into a series of line segments
      for(int i = 1; i <= BezierStepCount; ++i) {
        float bezierValues[2][2];

        // Interpolation point along the distance of the bezier curve
        float t = static_cast<float>(i) / static_cast<float>(BezierStepCount);

        // Perform the actual bezier interpolation
        bezierValues[0][0] = (1.0f - t) * controlPoints[0][0] + t * controlPoints[1][0];
        bezierValues[0][1] = (1.0f - t) * controlPoints[0][1] + t * controlPoints[1][1];
        bezierValues[1][0] = (1.0f - t) * controlPoints[1][0] + t * controlPoints[2][0];
        bezierValues[1][1] = (1.0f - t) * controlPoints[1][1] + t * controlPoints[2][1];
        bezierValues[0][0] = (1.0f - t) * bezierValues[0][0] + t * bezierValues[1][0];
        bezierValues[0][1] = (1.0f - t) * bezierValues[0][1] + t * bezierValues[1][1];
    
        // Add the interpolated point to the contour's list of points
        FT_Vector interpolatedPoint = { 
          static_cast<int>(bezierValues[0][0] + 0.5f),
          static_cast<int>(bezierValues[0][1] + 0.5f)
        };
        contourSet.Vertices.push_back(interpolatedPoint);
      }

      // Finally, move the cursor to the end of the curve
      contourSet.PenPosition = *to;

      return 0;
    }

  } // anonymous namespace

  // ------------------------------------------------------------------------------------------- //

  namespace { // anonymous

    /// <summary>Draws a cubic bezier curve with the specified control points</summary>
    /// <param name="control1">First control point for bezier curve's shape</param>
    /// <param name="control2">Second control point for bezier curve's shape</param>
    /// <param name="to">Position to draw a cubic bezier curve to</param>
    /// <param name="contourSetAddress">ContourSet to store the contour points in</param>
    int cubicTo(
      const FT_Vector *control1, const FT_Vector *control2,
      const FT_Vector *to, void *contourSetAddress
    ) {
      ContourSet &contourSet = *static_cast<ContourSet *>(contourSetAddress);
      const FT_Vector &pen = contourSet.PenPosition;

      // Number of interpolated points to break the bezier curve down to. Maybe
      // this could be calculated by means of distance and sharpness of the bend,
      // but for now, a fixed number of segments is used.
      const int BezierStepCount = 3;

      // Create a matrix from the bezier curve's control points.
      float controlPoints[4][2] = {
        { static_cast<float>(pen.x), static_cast<float>(pen.y) },
        { static_cast<float>(control1->x), static_cast<float>(control1->y) },
        { static_cast<float>(control2->x), static_cast<float>(control2->y) },
        { static_cast<float>(to->x), static_cast<float>(to->y) }
      };

      // Break down the bezier curve into a series of line segments
      for(int i = 1; i <= BezierStepCount; ++i) {
        float bezierValues[3][2];

        // Interpolation point along the distance of the bezier curve
        float t = static_cast<float>(i) / static_cast<float>(BezierStepCount);

        // Perform the actual bezier interpolation
        bezierValues[0][0] = (1.0f - t) * controlPoints[0][0] + t * controlPoints[1][0];
        bezierValues[0][1] = (1.0f - t) * controlPoints[0][1] + t * controlPoints[1][1];
        bezierValues[1][0] = (1.0f - t) * controlPoints[1][0] + t * controlPoints[2][0];
        bezierValues[1][1] = (1.0f - t) * controlPoints[1][1] + t * controlPoints[2][1];
        bezierValues[2][0] = (1.0f - t) * controlPoints[2][0] + t * controlPoints[3][0];
        bezierValues[2][1] = (1.0f - t) * controlPoints[2][1] + t * controlPoints[3][1];

        bezierValues[0][0] = (1.0f - t) * bezierValues[0][0] + t * bezierValues[1][0];
        bezierValues[0][1] = (1.0f - t) * bezierValues[0][1] + t * bezierValues[1][1];
        bezierValues[1][0] = (1.0f - t) * bezierValues[1][0] + t * bezierValues[2][0];
        bezierValues[1][1] = (1.0f - t) * bezierValues[1][1] + t * bezierValues[2][1];
        bezierValues[0][0] = (1.0f - t) * bezierValues[0][0] + t * bezierValues[1][0];
        bezierValues[0][1] = (1.0f - t) * bezierValues[0][1] + t * bezierValues[1][1];
    
        // Add the interpolated point to the contour's list of points
        FT_Vector interpolatedPoint = { 
          static_cast<int>(bezierValues[0][0] + 0.5f),
          static_cast<int>(bezierValues[0][1] + 0.5f)
        };
        contourSet.Vertices.push_back(interpolatedPoint);
      }

      // Finally, move the cursor to the end of the curve
      contourSet.PenPosition = *to;

      return 0;
    }

  } // anonymous namespace

  // ------------------------------------------------------------------------------------------- //

  namespace { // anonymous

    /// <summary>
    ///   Structure containing the decomposition function pointers for FreeType's
    ///   FT_Outline_Decompose() function.
    /// </summary>
    FT_Outline_Funcs decomposeFunctions = {
      moveTo,
      lineTo,
      conicTo,
      cubicTo,
      0,
      0
    };

  } // anonymous namespace

  // ------------------------------------------------------------------------------------------- //

  FreeTypeFontVectorizer::FreeTypeFontVectorizer(
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^fontDescription
  ) : FreeTypeFontProcessor(fontDescription) {}

  // ------------------------------------------------------------------------------------------- //

  VectorFontCharacterContent ^FreeTypeFontVectorizer::Vectorize(wchar_t character) {

    // Let FreeType load the character we want to be vectorized
    LoadCharacter(character);

    // Use FreeType's outline traversing function to decompose the glyph into
    // a series of closed shapes consisting of straight line segments only.
    ContourSet contourSet;
    FT_Error error = FT_Outline_Decompose(
      &this->freeTypeFace->glyph->outline, &decomposeFunctions, &contourSet
    );
    if(error)
      throw gcnew Exception("Error decomposing glyph into vectorial data");

    // Generate the final outline entry that was not yet closed by a moveTo() call
    if(!contourSet.Vertices.empty())
      contourSet.Outlines.push_back(
        std::pair<int, int>(
          contourSet.CurrentOutlineStartIndex,
          contourSet.Vertices.size() - contourSet.CurrentOutlineStartIndex
        )
      );

    VectorFontCharacterContent ^characterContent = gcnew VectorFontCharacterContent();

    // Now migrate the data into .NET data types that can be handled and stored by
    // the XNA framework classes.
    for(
      VertexVector::const_iterator vertex = contourSet.Vertices.begin();
      vertex != contourSet.Vertices.end();
      ++vertex
    ) {
      characterContent->Vertices->Add(
        Vector2(
          static_cast<float>(vertex->x) / 64.0f,
          static_cast<float>(vertex->y) / 64.0f
        )
      );
    }

    for(
      OutlineVector::const_iterator outline = contourSet.Outlines.begin();
      outline != contourSet.Outlines.end();
      ++outline
    ) {
      characterContent->Outlines->Add(
        VectorFontCharacterContent::Outline(outline->first, outline->second)
      );
    }

    // All done, we can hand out the Contours in a format .NET code actually understands!
    return characterContent;
   
  }

  // ------------------------------------------------------------------------------------------- //

  Microsoft::Xna::Framework::Vector2 FreeTypeFontVectorizer::GetAdvancement(wchar_t character) {

    // Prepare the character in FreeType
    LoadCharacter(character);

    // Return the bitmap offset informations stored in the glyph structure
    return Vector2(
      static_cast<float>(this->freeTypeFace->glyph->advance.x) / 64.0f,
      static_cast<float>(this->freeTypeFace->glyph->advance.y) / 64.0f
    );

  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
