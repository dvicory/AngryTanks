using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics.SpecialEffects.Terrain {

#if false
  /// <summary>Manages the rendering of terrain based on a regular grid</summary>
  public class GridTerrain<VertexType> where VertexType : struct {

    #region interface IVertexSetter

    /// <summary>
    ///   Used to assign values to specific components of the grid's vertices
    /// </summary>
    public interface IVertexSetter {
      /// <summary>Sets the position of a vertex</summary>
      /// <param name="vertex">Vertex whose position will be set</param>
      /// <param name="position">Position that will be assigned to the vertex</param>
      void SetPosition(ref VertexType vertex, Vector3 position);
      /// <summary>Sets the texture coordinates of a vertex</summary>
      /// <param name="vertex">Vertex whose texture coordinates will be set</param>
      /// <param name="textureCoordinate">
      ///   Texture coordinates that will be assigned to the vertex
      /// </param>
      void SetTextureCoordinate(ref VertexType vertex, Vector2 textureCoordinate);
    }

    #endregion // interface IVertexSetter

    /// <summary>Initializes a new grid-based terrain</summary>
    /// <param name="graphicsDevice">Graphics device the terrain is rendered on</param>
    /// <param name="vertexElements">Vertex elements of the provided vertex type</param>
    /// <param name="size">Number of quadrilaterals in the terrain</param>
    public GridTerrain(
      GraphicsDevice graphicsDevice, VertexElement[] vertexElements, Point size
    ) {

    }


    /// <summary>Vertex declaration for the vertices in the grid</summary>
    private VertexDeclaration vertexDeclaration;
    /// <summary>Vertex buffer containing the grid vertices</summary>
    private DynamicVertexBuffer vertexBuffer;
    /// <summary>Index buffer containing the indices to the vertex buffer</summary>
    private IndexBuffer indexBuffer;
    /// <summary>Graphics device the grid will be rendered with</summary>
    private GraphicsDevice graphicsDevice;

  }
#endif

} // namespace Nuclex.Graphics.SpecialEffects.Terrain
