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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Nuclex.Graphics.Batching;

namespace Nuclex.Graphics.Debugging {

  /// <summary>Game component for overlaying debugging informations on the scene</summary>
  /// <remarks>
  ///   <para>
  ///     This game component is mainly intended to debugging purposes. It will
  ///     accept all kinds of geometry drawing commands whilst a frame is being drawn
  ///     and will overlay this debugging geometry on everything else within the scene
  ///     just at the end of the drawing queue.
  ///   </para>
  ///   <para>
  ///     The DebugDrawer uses the GameComponent's DrawingOrder property to let itself
  ///     be drawn last. If you cannot call base.Draw() in your Game class last
  ///     (that's the line that invokes the Draw() methods of all
  ///     DrawableGameComponents), you can also create a DebugDrawer without adding
  ///     it to the GameComponents collection and invoke its Draw() method yourself
  ///     at the very end of your rendering process.
  ///   </para>
  /// </remarks>
  public class DebugDrawer : Drawable, IDebugDrawingService {

    /// <summary>Maximum number of vertices allowed for debugging overlays</summary>
    /// <remarks>
    ///   Controls the size of the vertex buffer used for storing the vertices
    ///   of the DebugDrawer. If debugging overlays are drawn after this many
    ///   vertices have already been generated, the drawing operations will
    ///   silently fail and a short text message on the screen will show that
    ///   not all debugging overlays could be drawn.
    /// </remarks>
    internal const int MaximumDebugVertexCount = 8192;

    #region struct QueuedString

    /// <summary>Stores a string that is queued for rendering</summary>
    private struct QueuedString {
      /// <summary>Initialized a new queued string</summary>
      /// <param name="text">Text to be rendered</param>
      /// <param name="position">Position at which to render the text</param>
      /// <param name="color">Color of the text to render</param>
      public QueuedString(string text, Vector2 position, Color color) {
        this.Text = text;
        this.Position = position;
        this.Color = color;
      }

      /// <summary>Text to be rendered</summary>
      public string Text;
      /// <summary>Position the text should be rendered at</summary>
      public Vector2 Position;
      /// <summary>Color of the rendered text</summary>
      public Color Color;
    }

    #endregion // struct QueuedString

    /// <summary>Initializes a new debug drawer component</summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service the debug drawer will use for rendering
    /// </param>
    public DebugDrawer(IGraphicsDeviceService graphicsDeviceService) :
      base(graphicsDeviceService) {

      this.queuedVertices = new VertexPositionColor[MaximumDebugVertexCount];
      this.queuedStrings = new List<QueuedString>();

      Reset();
    }

    /// <summary>Resets the contents of the debug drawer</summary>
    /// <remarks>
    ///   Reset() will be called automatically after a frame has been rendered.
    ///   Only use this method if you actually plan to discard everything
    ///   added to the debug drawer so far inmidst of the drawing process.
    /// </remarks>
    public void Reset() {
      this.triangleIndex = 0;
      this.lineIndex = MaximumDebugVertexCount - 1;
      this.overflowed = false;
      this.queuedStrings.Clear();
    }

    /// <summary>Concatenated View and Projection matrices to use</summary>
    /// <remarks>
    ///   Update this once per frame to have your debug overlays appear in the
    ///   right places. Simply set it to (View * Projection) of your camera.
    /// </remarks>
    public Matrix ViewProjection {
      get { return this.viewProjection; }
      set { this.viewProjection = value; }
    }

    /// <summary>Loads the graphics resources of the component</summary>
    protected override void LoadContent() {
      this.contentManager = new ResourceContentManager(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(GraphicsDeviceService),
#if WINDOWS_PHONE
        Resources.Phone7DebugDrawerResources.ResourceManager
#else
        Resources.DebugDrawerResources.ResourceManager
#endif
      );

      // The effect will be managed by our content manager and only needs to be
      // reloaded when the graphics device has been totally shut down or is
      // starting up for the first time.
#if WINDOWS_PHONE
      this.fillEffect = new BasicEffect(GraphicsDevice);
#else
      this.fillEffect = this.contentManager.Load<Effect>("SolidColorEffect");
#endif
      this.drawContext = new EffectDrawContext(this.fillEffect);

      // Create the sprite batch we're using for text rendering
      this.font = this.contentManager.Load<SpriteFont>("LucidaSpriteFont");
      this.fontSpriteBatch = new SpriteBatch(GraphicsDevice);

      // Create a new vertex buffer and its matching vertex declaration for
      // holding the geometry data of our debug overlays.
#if XNA_4
      this.vertexDeclaration = VertexPositionColor.VertexDeclaration;
#else
      this.vertexDeclaration = new VertexDeclaration(
        GraphicsDevice, VertexPositionColor.VertexElements
      );
#endif
      this.batchDrawer = PrimitiveBatch<VertexPositionColor>.GetDefaultBatchDrawer(
#if XNA_4
        GraphicsDevice
#else
        GraphicsDevice, this.vertexDeclaration, VertexPositionColor.SizeInBytes
#endif
      );
    }

    /// <summary>Unloads the graphics resources of the component</summary>
    protected override void UnloadContent() {

      // Release the vertex buffer and vertex declaration. Because these
      // are not managed resources, we need to get rid of them manually even
      // when the graphics device is only being reset.
      IDisposable disposableBatchDrawer = this.batchDrawer as IDisposable;
      if(disposableBatchDrawer != null) {
        disposableBatchDrawer.Dispose();
        this.batchDrawer = null;
      }
#if !XNA_4
      if(this.vertexDeclaration != null) {
        this.vertexDeclaration.Dispose();
        this.vertexDeclaration = null;
      }
#endif
      // Release the font rendering sprite batch.
      this.fontSpriteBatch.Dispose();

      // If the device is about to be destroyed, there's no sense in keeping
      // even the managed resources, so unload the entire content manager
      // in that case.
      this.fillEffect = null;
      this.font = null;
      this.contentManager.Unload();

    }

    /// <summary>Draws a line from the starting point to the destination point</summary>
    /// <param name="from">Starting point of the line</param>
    /// <param name="to">Destination point the line will be drawn to</param>
    /// <param name="color">Desired color of the line</param>
    public void DrawLine(Vector3 from, Vector3 to, Color color) {
      const int VertexCount = 2;

      // If we would collide with the triangles in the array or there simply
      // isn't enough space left, set the overflow flag and silently skip drawing
      int proposedStart = this.lineIndex - (VertexCount - 1);
      if(proposedStart < this.triangleIndex) {
        this.overflowed = true;
        return;
      }

      // Append the line vertices to our vertex array
      this.queuedVertices[this.lineIndex].Position = to;
      this.queuedVertices[this.lineIndex].Color = color;
      this.queuedVertices[this.lineIndex - 1].Position = from;
      this.queuedVertices[this.lineIndex - 1].Color = color;

      this.lineIndex -= VertexCount;
    }

    /// <summary>Draws a wireframe triangle between three points</summary>
    /// <param name="a">First corner point of the triangle</param>
    /// <param name="b">Second corner point of the triangle</param>
    /// <param name="c">Third corner point of the triangle</param>
    /// <param name="color">Desired color of the line</param>
    public void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color) {
      const int VertexCount = WireframeTriangleVertexGenerator.VertexCount;

      // If we would collide with the triangles in the array or there simply
      // isn't enough space left, set the overflow flag and silently skip drawing
      int proposedStart = this.lineIndex - (VertexCount - 1);
      if(proposedStart < this.triangleIndex) {
        this.overflowed = true;
        return;
      }

      // Generate the vertices for box' wireframe
      WireframeTriangleVertexGenerator.Generate(
        this.queuedVertices, proposedStart, a, b, c, color
      );

      this.lineIndex -= VertexCount;
    }

    /// <summary>Draws a solid (filled) triangle between three points</summary>
    /// <param name="a">First corner point of the triangle</param>
    /// <param name="b">Second corner point of the triangle</param>
    /// <param name="c">Third corner point of the triangle</param>
    /// <param name="color">Desired color of the line</param>
    public void DrawSolidTriangle(Vector3 a, Vector3 b, Vector3 c, Color color) {
      const int VertexCount = SolidTriangleVertexGenerator.VertexCount;

      // If we would collide with the triangles in the array or there simply
      // isn't enough space left, set the overflow flag and silently skip drawing
      int proposedEnd = this.triangleIndex + VertexCount;
      if(proposedEnd > this.lineIndex) {
        this.overflowed = true;
        return;
      }

      // Generate the vertices for the faces of the box
      SolidTriangleVertexGenerator.Generate(
        this.queuedVertices, this.triangleIndex, a, b, c, color
      );

      this.triangleIndex += VertexCount;
    }

    /// <summary>Draws a wireframe box at the specified location</summary>
    /// <param name="min">Contains the coordinates of the box lesser corner</param>
    /// <param name="max">Contains the coordinates of the box greater corner</param>
    /// <param name="color">Color of the wireframe to draw</param>
    public void DrawBox(Vector3 min, Vector3 max, Color color) {
      const int VertexCount = WireframeBoxVertexGenerator.VertexCount;

      // If we would collide with the triangles in the array or there simply
      // isn't enough space left, set the overflow flag and silently skip drawing
      int proposedStart = this.lineIndex - (VertexCount - 1);
      if(proposedStart < this.triangleIndex) {
        this.overflowed = true;
        return;
      }

      // Generate the vertices for box' wireframe
      WireframeBoxVertexGenerator.Generate(
        this.queuedVertices, proposedStart, min, max, color
      );

      this.lineIndex -= VertexCount;
    }

    /// <summary>Draws a solid (filled) box at the specified location</summary>
    /// <param name="min">Contains the coordinates of the box lesser corner</param>
    /// <param name="max">Contains the coordinates of the box greater corner</param>
    /// <param name="color">Desired color for the box</param>
    public void DrawSolidBox(Vector3 min, Vector3 max, Color color) {
      const int VertexCount = SolidBoxVertexGenerator.VertexCount;

      // If we would collide with the triangles in the array or there simply
      // isn't enough space left, set the overflow flag and silently skip drawing
      int proposedEnd = this.triangleIndex + VertexCount;
      if((proposedEnd > this.lineIndex) || (proposedEnd > MaximumDebugVertexCount)) {
        this.overflowed = true;
        return;
      }

      // Generate the vertices for the faces of the box
      SolidBoxVertexGenerator.Generate(
        this.queuedVertices, this.triangleIndex, min, max, color
      );

      this.triangleIndex += VertexCount;
    }

    /// <summary>Draws a wireframe arrow into the scene to visualize a vector</summary>
    /// <param name="origin">
    ///   Location at which to draw the arrow (this will form the exact center of
    ///   the drawn arrow's base)
    /// </param>
    /// <param name="direction">
    ///   Direction the arrow is pointing into. The arrow's size is relative to
    ///   the length of this vector.
    /// </param>
    /// <param name="color">Color of the wireframe to draw</param>
    public void DrawArrow(Vector3 origin, Vector3 direction, Color color) {
      const int VertexCount = WireframeArrowVertexGenerator.VertexCount;

      // If we would collide with the triangles in the array or there simply
      // isn't enough space left, set the overflow flag and silently skip drawing
      int proposedStart = this.lineIndex - (VertexCount - 1);
      if(proposedStart < this.triangleIndex) {
        this.overflowed = true;
        return;
      }

      // Generate the vertices for box' wireframe
      WireframeArrowVertexGenerator.Generate(
        this.queuedVertices, proposedStart, origin, direction, color
      );

      this.lineIndex -= VertexCount;
    }

    /// <summary>Draws a solid arrow into the scene to visualize a vector</summary>
    /// <param name="origin">
    ///   Location at which to draw the arrow (this will form the exact center of
    ///   the drawn arrow's base)
    /// </param>
    /// <param name="direction">
    ///   Direction the arrow is pointing into. The arrow's size is relative to
    ///   the length of this vector.
    /// </param>
    /// <param name="color">Color of the arrow</param>
    public void DrawSolidArrow(Vector3 origin, Vector3 direction, Color color) {
      const int VertexCount = SolidArrowVertexGenerator.VertexCount;

      // If we would collide with the triangles in the array or there simply
      // isn't enough space left, set the overflow flag and silently skip drawing
      int proposedEnd = this.triangleIndex + VertexCount;
      if(proposedEnd > this.lineIndex) {
        this.overflowed = true;
        return;
      }

      // Generate the vertices for the faces of the box
      SolidArrowVertexGenerator.Generate(
        this.queuedVertices, this.triangleIndex, origin, direction, color
      );

      this.triangleIndex += VertexCount;
    }

    /// <summary>Draws text onto the screen at pixel coordinates</summary>
    /// <param name="position">
    ///   Location on the screen, in pixels, where the text should be drawn.
    /// </param>
    /// <param name="text">String to be drawn</param>
    /// <param name="color">Color the text should have</param>
    public void DrawString(Vector2 position, string text, Color color) {
      this.queuedStrings.Add(new QueuedString(text, position, color));
    }

    /// <summary>Draws the debug overlays queued for this frame</summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Draw(GameTime gameTime) {

      // Draw the lines and triangles we have queued in our vertex array
      drawGeometry();

      // If the user has attempted to render more debugging overlays than the
      // DebugDrawer can handle (due to the fixed vertex buffer size), we can at
      // least print out a warning...
      if(this.overflowed) {
        DrawString(
          new Vector2(0.5f, 0.5f),
          "Warning: DebugDrawer is omitting some overlays",
          Color.Orange
        );
      }

      // Draw the text on top of everything else
      drawStrings();

      // Remove the geometry we've drawn from the buffers again
      Reset();

    }

    /// <summary>Draws the geometry (lines and triangles) queued up for this frame</summary>
    private void drawGeometry() {

      // Upload the vertices to the GPU and select their vertex declaration
      this.batchDrawer.Select(this.queuedVertices, this.queuedVertices.Length);

      // Set the view * projection matrix to use for transforming the input
      // vertices into screen coordinates
#if WINDOWS_PHONE
      (this.fillEffect as BasicEffect).Projection = this.viewProjection;
#else
      this.fillEffect.Parameters["ViewProjection"].SetValue(this.viewProjection);
#endif
      // Draw all queued triangles     
      this.batchDrawer.Draw(
        0,
        this.triangleIndex,
        PrimitiveType.TriangleList,
        this.drawContext
      );

      // Draw all queued lines
      this.batchDrawer.Draw(
        this.lineIndex + 1,
        MaximumDebugVertexCount - 1 - this.lineIndex,
        PrimitiveType.LineList,
        drawContext
      );

    }

    /// <summary>Draws the strings queued up for this frame</summary>
    private void drawStrings() {
      this.fontSpriteBatch.Begin(
#if XNA_4
        SpriteSortMode.Deferred, BlendState.AlphaBlend
#else
        SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState
#endif
      );
      try {

        // Draw all strings that have been queued so far
        for(int index = 0; index < this.queuedStrings.Count; ++index) {
          this.fontSpriteBatch.DrawString(
            this.font,
            this.queuedStrings[index].Text,
            this.queuedStrings[index].Position,
            this.queuedStrings[index].Color
          );
        }

      }
      finally {
        this.fontSpriteBatch.End();
      }
    }

    /// <summary>Content manager used to load the debug drawer's effect file</summary>
    private ResourceContentManager contentManager;
    /// <summary>Effect used for drawing the debug overlays</summary>
    private Effect fillEffect;
    /// <summary>Drawing context used wit hthe batch drawer</summary>
    private EffectDrawContext drawContext;
    /// <summary>Buffer for constructing temporary vertices</summary>
    /// <remarks>
    ///   <para>
    ///     This array is filled from two sides: Triangles start at index 0 and
    ///     are appended as normal while lines start at MaximumDebugVertexCount
    ///     and are appended backwards. When the two list meet at the center
    ///     of the array, the vertex list is full.
    ///   </para>
    ///   <para>
    ///     We don't need a fancy drawing operation queue because the ordering of
    ///     the debugging primitives does not matter. We simply draw all of the
    ///     triangles and proceed with the lines, letting the z-buffer take care
    ///     of the rest.
    ///   </para>
    /// </remarks>
    private VertexPositionColor[] queuedVertices;
    /// <summary>The concatenated view and projection matrices</summary>
    private Matrix viewProjection;
    /// <summary>Vertex declaration for the vertices in the vertex buffer</summary>
    private VertexDeclaration vertexDeclaration;
    /// <summary>Drawer that sends the vertices to the GPU in the drawing phase</summary>
    private IBatchDrawer<VertexPositionColor> batchDrawer;
    /// <summary>Sprite font we're using for outputting strings to the screen</summary>
    private SpriteFont font;
    /// <summary>SpriteBatch used for font rendering</summary>
    private SpriteBatch fontSpriteBatch;
    /// <summary>Text queued to be rendered onto the scene</summary>
    private List<QueuedString> queuedStrings;
    /// <summary>Index for the next triangle in the vertex array</summary>
    /// <remarks>
    ///   Counts from 0 up to MaximumDebugVertexCount, increased in multiples of 3.
    /// </remarks>
    private int triangleIndex;
    /// <summary>Index for the next line in the vertex array</summary>
    /// <remarks> 
    ///   Counts in reverse from MaximumDebugVertexCount to 0,
    ///   decreased in multiples of 2.
    /// </remarks>
    private int lineIndex;
    /// <summary>
    ///   Flag indicating that the user has attempted to draw more primitives
    ///   than our vertex array can hold.
    /// </summary>
    private bool overflowed;

  }

} // namespace Nuclex.Graphics.Debugging
