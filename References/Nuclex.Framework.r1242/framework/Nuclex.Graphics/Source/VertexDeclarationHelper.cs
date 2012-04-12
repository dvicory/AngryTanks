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
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.Graphics {

  /// <summary>
  ///   Builds vertex declarations from vertex structures
  /// </summary>
  /// <remarks>
  ///   Based on ideas from Michael Popoloski's article on gamedev.net:
  ///   http://www.gamedev.net/reference/programming/features/xnaVertexElement/
  /// </remarks>
  public static class VertexDeclarationHelper {

    /// <summary>Combines two vertex element list into one single list</summary>
    /// <param name="left">First vertex element list that will be merged</param>
    /// <param name="right">Second vertex element list that will be merged</param>
    /// <returns>The combined vertex element list from both inputs</returns>
    /// <remarks>
    ///   <para>
    ///     No intelligence is applied to avoid duplicates or to adjust the usage index
    ///     of individual vertex elements. This method simply serves as a helper to merge
    ///     two vertex element lists from two structures that are used in seperate
    ///     vertex streams (but require a single vertex declaration containing the elements
    ///     of both streams).
    ///   </para>
    ///   <para>
    ///     <example>
    ///       This example shows how two vertex structures, each used in a different
    ///       vertex buffer, can be merged into a single vertex declaration that fetches
    ///       vertices from both vertex buffers, the positions from stream 0 and
    ///       the texture coordinates from stream 1
    ///       <code>
    ///         struct PositionVertex {
    ///           [VertexElement(VertexElementUsage.Position)]
    ///           public Vector3 Position;
    ///         }
    ///         struct TextureCoordinateVertex {
    ///           [VertexElement(VertexElementUsage.TextureCoordinate)]
    ///           public Vector2 TextureCoordinate;
    ///         }
    ///         
    ///         private VertexDeclaration buildVertexDeclaration() {
    ///           VertexDeclaration declaration = new VertexDeclaration(
    ///             graphicsDevice,
    ///             VertexDeclarationHelper.Combine(
    ///               VertexDeclarationHelper.BuildElementList&lt;PositionVertex&gt;(0),
    ///               VertexDeclarationHelper.BuildElementList&lt;TextureCoordinateVertex&gt;(1)
    ///             )
    ///           );
    ///         }
    ///       </code>
    ///     </example>
    ///   </para>
    /// </remarks>
    public static VertexElement[] Combine(VertexElement[] left, VertexElement[] right) {

      // Determine the total length the resulting array will have. If one of the arguments
      // is null, this line will intentionally trigger the NullReferenceException
      int totalLength = left.Length + right.Length;

      // Merge the two arrays
      VertexElement[] combined = new VertexElement[totalLength];
      Array.Copy(left, combined, left.Length);
      Array.Copy(right, 0, combined, left.Length, right.Length);

      // Done, no further processing required
      return combined;

    }

    /// <summary>
    ///   Builds a vertex element list that can be used to construct a vertex declaration
    ///   from a vertex structure that has the vertex element attributes applied to it
    /// </summary>
    /// <typeparam name="VertexType">
    ///   Vertex structure with vertex element attributes applied to it
    /// </typeparam>
    /// <returns>
    ///   A vertex element list that can be used to create a new vertex declaration matching
    ///   the provided vertex structure
    /// </returns>
    public static VertexElement[] BuildElementList<VertexType>() where VertexType : struct {
#if !XNA_4
      return BuildElementList<VertexType>(0);
    }

    /// <summary>
    ///   Builds a vertex element list that can be used to construct a vertex declaration
    ///   from a vertex structure that has the vertex element attributes applied to it
    /// </summary>
    /// <typeparam name="VertexType">
    ///   Vertex structure with vertex element attributes applied to it
    /// </typeparam>
    /// <param name="stream">
    ///   Index of the vertex buffer that will contain this element
    /// </param>
    /// <returns>
    ///   A vertex element list that can be used to create a new vertex declaration matching
    ///   the provided vertex structure
    /// </returns>
    /// <remarks>
    ///   Data for vertices can come from multiple vertex buffers. For example, you can store
    ///   the positions of your vertices in one vertex buffer and then store the texture
    ///   coordinates in an entirely different vertex buffer, only to combine them at rendering
    ///   time by using the two vertex buffers as two simultaneous vertex data streams.
    /// </remarks>
    public static VertexElement[] BuildElementList<VertexType>(short stream)
      where VertexType : struct {
#endif
      FieldInfo[] fields = getFields<VertexType>();

      int fieldOffset = 0;
      int elementCount = 0;

      // Set up an array for the vertex elements and fill it with the data we
      // gather directly from the vertex structure
      VertexElement[] elements = new VertexElement[fields.Length];
      for(int index = 0; index < fields.Length; ++index) {

        // Find out whether this field is used by the vertex shader. If so, add
        // it to the elements list so it ends up in the vertex shader.
        VertexElementAttribute attribute = getVertexElementAttribute(fields[index]);
        if(attribute != null) {
          buildVertexElement(fields[index], attribute, ref elements[elementCount]);

#if !(XBOX360 || WINDOWS_PHONE)
          fieldOffset = Marshal.OffsetOf(typeof(VertexType), fields[index].Name).ToInt32();
#endif
          elements[elementCount].Offset = (short)fieldOffset;
#if !XNA_4
          elements[elementCount].Stream = stream;
#endif

          ++elementCount;
        }

#if XBOX360
        fieldOffset += Marshal.SizeOf(fields[index].FieldType);
#endif
      }

      // If there isn't a single vertex element, this type would be completely useless
      // as a vertex. Probably the user forgot to add the VertexElementAttribute.
      if(elementCount == 0) {
        throw new InvalidOperationException(
          "No fields had the VertexElementAttribute assigned to them."
        );
      }

      Array.Resize(ref elements, elementCount);
      return elements;
    }
    
    /// <summary>Obtains the stride value for a vertex</summary>
    /// <typeparam name="VertexType">
    ///   Vertex structure the stride value will be obtained for
    /// </typeparam>
    /// <returns>The stride value for the specified vertex structure</returns>
    public static int GetStride<VertexType>() where VertexType : struct {
      FieldInfo[] fields = getFields<VertexType>();

      int fieldOffset = 0;
      for(int index = 0; index < fields.Length; ++index) {
        fieldOffset += Marshal.SizeOf(fields[index].FieldType);
      }

      return fieldOffset;
    }

    /// <summary>Retrieves the fields declared in a vertex structure</summary>
    /// <typeparam name="VertexType">Type the fields will be retrieved from</typeparam>
    /// <returns>The list of fields declared in the provided structure</returns>
    private static FieldInfo[] getFields<VertexType>() where VertexType : struct {
      Type vertexType = typeof(VertexType);

      // Obtain a list of all the fields (object member variables) in the vertex type
      FieldInfo[] fields = vertexType.GetFields(
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
      );
      if(fields.Length == 0) {
        throw new InvalidOperationException("Specified vertex type has no fields");
      }

      return fields;
    }

    /// <summary>Builds a vertex element from an attributed field in a structure</summary>
    /// <param name="fieldInfo">
    ///   Reflected data on the field for which a vertex element will be built
    /// </param>
    /// <param name="attribute">Vertex eelement attribute assigned to the field</param>
    /// <param name="element">
    ///   Output parameter the newly built vertex element is stored in
    /// </param>
    private static void buildVertexElement(
      FieldInfo fieldInfo, VertexElementAttribute attribute, ref VertexElement element
    ) {
#if !XNA_4
      element.VertexElementMethod = attribute.Method;
#endif
      element.VertexElementUsage = attribute.Usage;
      element.UsageIndex = attribute.UsageIndex;

      // Was an explicit data type provided for this field?
      if(attribute.FormatProvided == true) {
        element.VertexElementFormat = attribute.Format;
      } else { // Nope, try to auto-detect the data type
        if(fieldInfo.FieldType == typeof(Vector2)) {
          element.VertexElementFormat = VertexElementFormat.Vector2;
        } else if(fieldInfo.FieldType == typeof(Vector3)) {
          element.VertexElementFormat = VertexElementFormat.Vector3;
        } else if(fieldInfo.FieldType == typeof(Vector4)) {
          element.VertexElementFormat = VertexElementFormat.Vector4;
        } else if(fieldInfo.FieldType == typeof(Color)) {
          element.VertexElementFormat = VertexElementFormat.Color;
        } else if(fieldInfo.FieldType == typeof(float)) {
          element.VertexElementFormat = VertexElementFormat.Single;
        } else if(fieldInfo.FieldType == typeof(int)) {
          element.VertexElementFormat = VertexElementFormat.Short4;
        } else if(fieldInfo.FieldType == typeof(short)) {
          element.VertexElementFormat = VertexElementFormat.Short2;
        } else { // No success in auto-detection, give up
          throw new InvalidOperationException(
            "Unrecognized field type, please specify vertex format explicitly"
          );
        }
      }
    }

    /// <summary>
    ///   Retrieves the vertex element attribute assigned to a field in a structure
    /// </summary>
    /// <param name="fieldInfo">
    ///   Informations about the vertex element field the attribute is retrieved for
    /// </param>
    /// <returns>The vertex element attribute of the requested field</returns>
    private static VertexElementAttribute getVertexElementAttribute(FieldInfo fieldInfo) {
      object[] attributes = fieldInfo.GetCustomAttributes(
        typeof(VertexElementAttribute), false
      );

      // The docs state that if the requested attribute has not been applied to the field,
      // an array of length 0 will be returned.
      if(attributes.Length == 0) {
        return null;
      }

      return (VertexElementAttribute)attributes[0];
    }

  }

} // namespace Nuclex.Graphics
