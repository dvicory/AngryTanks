// --------------------------------------------------------------------------------------------- //
// SolidColor Effect
// --------------------------------------------------------------------------------------------- //
//
// Fills any polygons drawn with it in a solid color
//

/// <summary>Concatenated view and projection matrix</summary>
float4x4 ViewProjection : VIEWPROJECTION;

// --------------------------------------------------------------------------------------------- //
// Supporting Structures
// --------------------------------------------------------------------------------------------- //

/// <summary>Vertex shader output values. These are sent to the pixel shader.</summary>
struct VertexShaderOutput {
  /// <summary>Position in screen coordinates</summary>
  float4 Position : POSITION0;
  /// <summary>Color of the vertex</summary>
  float4 Color : COLOR0;
};

// --------------------------------------------------------------------------------------------- //
// Vertex Shader
// --------------------------------------------------------------------------------------------- //

/// <summary>Transforms a vertex into view space</summary>
/// <param name="position">Position of the vertex in world coordinates</param>
/// <param name="color">Color of the vertex</param>
/// <returns>The transformed vertex</returns>
VertexShaderOutput VertexShaderFunction(float4 position : POSITION0, float4 color : COLOR0) {
  VertexShaderOutput output;
  
  output.Position = mul(position, ViewProjection);
  output.Color = color;

  return output;
}

// --------------------------------------------------------------------------------------------- //
// Pixel Shader
// --------------------------------------------------------------------------------------------- //

/// <summary>Calculates the color of a pixel</summary>
/// <param name="color">Color the pixel will have</param>
/// <returns>The calculated color of the pixel</returns>
float4 PixelShaderFunction(float4 color : COLOR0) : COLOR0 {
  return color;
};

// --------------------------------------------------------------------------------------------- //
// Techniques
// --------------------------------------------------------------------------------------------- //
technique SolidColorTechnique {
  pass FillPass {
    CullMode = None;

    ZEnable = false;
    ZWriteEnable = false;

    AlphaBlendEnable = true;
    SrcBlend = SrcAlpha;
    DestBlend = InvSrcAlpha;

    VertexShader = compile vs_2_0 VertexShaderFunction();
    PixelShader  = compile ps_2_0 PixelShaderFunction();
  }
}
