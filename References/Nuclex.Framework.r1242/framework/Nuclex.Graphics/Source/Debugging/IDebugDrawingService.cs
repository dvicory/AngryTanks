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

namespace Nuclex.Graphics.Debugging {

  /// <summary>Service for overlaying debugging informations on the scene</summary>
  public interface IDebugDrawingService {

    /// <summary>Concatenated View and Projection matrices to use</summary>
    /// <remarks>
    ///   Update this once per frame to have your debug overlays appear in the
    ///   right places. Simply set it to (View * Projection) of your camera.
    /// </remarks>
    Matrix ViewProjection { get; set; }

    /// <summary>Draws a line from the starting point to the destination point</summary>
    /// <param name="from">Starting point of the line</param>
    /// <param name="to">Destination point the line will be drawn to</param>
    /// <param name="color">Desired color of the line</param>
    void DrawLine(Vector3 from, Vector3 to, Color color);

    /// <summary>Draws a wireframe triangle between three points</summary>
    /// <param name="a">First corner point of the triangle</param>
    /// <param name="b">Second corner point of the triangle</param>
    /// <param name="c">Third corner point of the triangle</param>
    /// <param name="color">Desired color of the line</param>
    void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color);

    /// <summary>Draws a solid (filled) triangle between three points</summary>
    /// <param name="a">First corner point of the triangle</param>
    /// <param name="b">Second corner point of the triangle</param>
    /// <param name="c">Third corner point of the triangle</param>
    /// <param name="color">Desired color of the line</param>
    void DrawSolidTriangle(Vector3 a, Vector3 b, Vector3 c, Color color);

    /// <summary>Draws a wireframe box at the specified location</summary>
    /// <param name="min">Contains the coordinates of the box lesser corner</param>
    /// <param name="max">Contains the coordinates of the box greater corner</param>
    /// <param name="color">Color of the wireframe to draw</param>
    void DrawBox(Vector3 min, Vector3 max, Color color);

    /// <summary>Draws a solid (filled) box at the specified location</summary>
    /// <param name="min">Contains the coordinates of the box lesser corner</param>
    /// <param name="max">Contains the coordinates of the box greater corner</param>
    /// <param name="color">Desired color for the box</param>
    void DrawSolidBox(Vector3 min, Vector3 max, Color color);

    /// <summary>Draws a wireframe arrow into the scene to visualize a vector</summary>
    /// <param name="origin">Location at which to draw the arrow</param>
    /// <param name="direction">Direction the arrow is pointing into</param>
    /// <param name="color">Color of the wireframe to draw</param>
    void DrawArrow(Vector3 origin, Vector3 direction, Color color);

    /// <summary>Draws an arrow into the scene to visualize a vector</summary>
    /// <param name="origin">Location from which the arrow originates</param>
    /// <param name="direction">Direction into which the arrow is pointing</param>
    /// <param name="color">Color of the arrow</param>
    void DrawSolidArrow(Vector3 origin, Vector3 direction, Color color);

    /// <summary>Draws text onto the screen at pixel coordinates</summary>
    /// <param name="position">
    ///   Location on the screen, in pixels, where the text should be drawn.
    /// </param>
    /// <param name="text">String to be drawn</param>
    /// <param name="color">Color the text should have</param>
    void DrawString(Vector2 position, string text, Color color);

  }

} // namespace Nuclex.Graphics.Debugging
