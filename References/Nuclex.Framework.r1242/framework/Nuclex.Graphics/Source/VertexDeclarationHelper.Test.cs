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

#if UNITTEST

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

namespace Nuclex.Graphics {

  /// <summary>Unit tests for the vertex element attribute</summary>
  [TestFixture]
  internal class VertexDeclarationHelperTest {

    #region struct TestVertex

    /// <summary>
    ///   A vertex used to unit-test the format auto-detection of
    ///   the vertex declaration helper
    /// </summary>
    private struct TestVertex {
      /// <summary>A vertex element of type Vector2</summary>
      [VertexElement(VertexElementUsage.TextureCoordinate)]
      public Vector2 TestVector2;
      /// <summary>A vertex element of type Vector3</summary>
      [VertexElement(VertexElementUsage.Position)]
      public Vector3 TestVector3;
      /// <summary>A vertex element of type Vector4</summary>
      [VertexElement(VertexElementUsage.Normal)]
      public Vector4 TestVector4;
      /// <summary>A vertex element of type Color</summary>
      [VertexElement(VertexElementUsage.Color)]
      public Color TestColor;
      /// <summary>A vertex element of type float</summary>
      [VertexElement(VertexElementUsage.BlendWeight)]
      public float TestSingle;
      /// <summary>A vertex element of type int</summary>
      [VertexElement(VertexElementUsage.Sample)]
      public int TestInt;
      /// <summary>A vertex element of type short</summary>
      [VertexElement(VertexElementUsage.Sample, UsageIndex = 1)]
      public short TestShort;
      /// <summary>A vertex element of type short</summary>
      [VertexElement(VertexElementUsage.BlendWeight, VertexElementFormat.Vector4)]
      public Matrix TestExplicitMatrix;
    }

    #endregion // struct TestVertex

    #region struct UnknownTypeVertex

    /// <summary>
    ///   A vertex containing a data type not recognized by the format auto-detection of
    ///   the vertex declaration helper
    /// </summary>
    private struct UnknownTypeVertex {

      /// <summary>A vertex element of type DateTime</summary>
      [VertexElement(VertexElementUsage.Sample, UsageIndex = 1)]
      public DateTime TestTimestamp;

    }

    #endregion // struct UnknownTypeVertex

    #region struct EmptyVertex

    /// <summary>An empty vertex type used to unit-test error behavior</summary>
    private struct EmptyVertex { }

    #endregion // struct EmptyVertex

    #region struct UnattributedFieldVertex

    /// <summary>
    ///   A vertex type containing a field without a vertex element attribute
    /// </summary>
    private struct UnattributedFieldVertex {

      /// <summary>A vertex element of type Vector3</summary>
      public Vector3 TestVector3;
      /// <summary>A vertex element of type Color</summary>
      public Color TestColor;

    }

    #endregion // struct UnattributedFieldVertex

    #region struct SecondStreamVertex

    /// <summary>
    ///   A vertex containing an additional data element to be used as second vertex stream
    /// </summary>
    private struct SecondStreamVertex {

      /// <summary>A vertex element of type DateTime</summary>
      [VertexElement(VertexElementUsage.BlendWeight, UsageIndex = 1)]
      public float BlendWeight;

    }

    #endregion // struct SecondStreamVertex

    #region struct GapVertex

    /// <summary>
    ///   A vertex in which not all elements are used by the shader
    /// </summary>
    private struct GapVertex {

      /// <summary>Position of the vertex</summary>
      [VertexElement(VertexElementUsage.Position)]
      public Vector3 Position;

      /// <summary>Velocity the vertex is moving at</summary>
      public Vector3 Velocity;

      /// <summary>Size of the vertex when rendered as a point sprite</summary>
      [VertexElement(VertexElementUsage.PointSize)]
      public float Size;

    }

    #endregion // struct GapVertex

    /// <summary>
    ///   Verifies that the stride of a vertex structure can be determined
    /// </summary>
    [Test]
    public void TestStrideDetermination() {
      Assert.AreEqual(114, VertexDeclarationHelper.GetStride<TestVertex>());
      Assert.AreEqual(4, VertexDeclarationHelper.GetStride<SecondStreamVertex>());
    }

    /// <summary>
    ///   Tests whether the vertex declaration helper fails is provieed with two
    ///   null references instead of two vertex element lists
    /// </summary>
    [Test]
    public void TestThrowOnCombineNull() {
      Assert.Throws<NullReferenceException>(
        delegate() { VertexDeclarationHelper.Combine(null, null); }
      );
    }

    /// <summary>
    ///   Tests whether the vertex declaration helper is able to combine two vertex element
    ///   lists into a single one
    /// </summary>
    [Test]
    public void TestCombine() {
      VertexElement[] firstElements = VertexDeclarationHelper.BuildElementList<TestVertex>(
#if !XNA_4
        0
#endif
      );

      VertexElement[] secondElements = VertexDeclarationHelper.BuildElementList<
        SecondStreamVertex
      >(
#if !XNA_4
        1
#endif
      );

      VertexElement[] combinedElements =
        VertexDeclarationHelper.Combine(firstElements, secondElements);

      Assert.AreEqual(firstElements.Length + secondElements.Length, combinedElements.Length);
      CollectionAssert.IsSubsetOf(firstElements, combinedElements);
      CollectionAssert.IsSubsetOf(secondElements, combinedElements);
    }

    /// <summary>
    ///   Tests whether the vertex declaration helper fails on empty vertices
    /// </summary>
    [Test]
    public void TestThrowOnEmptyVertex() {
      Assert.Throws<InvalidOperationException>(
        delegate() {
          VertexElement[] elements = VertexDeclarationHelper.BuildElementList<EmptyVertex>();
        }
      );
    }

    /// <summary>
    ///   Tests whether the vertex declaration helper fails when format auto-detection
    ///   is used on unknown data types
    /// </summary>
    [Test]
    public void TestThrowOnUnknownTypeVertex() {
      Assert.Throws<InvalidOperationException>(
        delegate() {
          VertexElement[] elements = VertexDeclarationHelper.BuildElementList<
            UnknownTypeVertex
          >();
        }
      );
    }

    /// <summary>
    ///   Tests whether the vertex declaration helper fails when no field of
    ///   a Vertex has have the vertex element attribute assigned to it
    /// </summary>
    [Test]
    public void TestThrowOnUnattributedFieldVertex() {
      Assert.Throws<InvalidOperationException>(
        delegate() {
          VertexElement[] elements = VertexDeclarationHelper.BuildElementList<
            UnattributedFieldVertex
          >();
        }
      );
    }

    /// <summary>Tests the format auto-detection of vertex elements</summary>
    [Test]
    public void TestFormatAutoDetection() {
      VertexElement[] elements = VertexDeclarationHelper.BuildElementList<TestVertex>();

      Assert.AreEqual(8, elements.Length);
      Assert.AreEqual(VertexElementFormat.Vector2, elements[0].VertexElementFormat);
      Assert.AreEqual(VertexElementFormat.Vector3, elements[1].VertexElementFormat);
      Assert.AreEqual(VertexElementFormat.Vector4, elements[2].VertexElementFormat);
      Assert.AreEqual(VertexElementFormat.Color, elements[3].VertexElementFormat);
      Assert.AreEqual(VertexElementFormat.Single, elements[4].VertexElementFormat);
      Assert.AreEqual(VertexElementFormat.Short4, elements[5].VertexElementFormat);
      Assert.AreEqual(VertexElementFormat.Short2, elements[6].VertexElementFormat);
      Assert.AreEqual(VertexElementFormat.Vector4, elements[7].VertexElementFormat);
      
      Assert.AreEqual(1, elements[6].UsageIndex);
    }

#if !XNA_4
    /// <summary>
    ///   Tests whether the vertex declaration helper correctly assigns the vertex
    ///   stream to all elements if one was specified
    /// </summary>
    [Test]
    public void TestVertexStreamAssignment() {
      VertexElement[] elements = VertexDeclarationHelper.BuildElementList<TestVertex>(123);

      Assert.AreEqual(8, elements.Length);
      Assert.AreEqual(123, elements[0].Stream);
      Assert.AreEqual(123, elements[1].Stream);
      Assert.AreEqual(123, elements[2].Stream);
      Assert.AreEqual(123, elements[3].Stream);
      Assert.AreEqual(123, elements[4].Stream);
      Assert.AreEqual(123, elements[5].Stream);
      Assert.AreEqual(123, elements[6].Stream);
      Assert.AreEqual(123, elements[7].Stream);
    }
#endif

    /// <summary>
    ///   Tests whether a vertex containing a field in its middle that isn't seen by
    ///   the vertex shader is processed correctly
    /// </summary>
    [Test]
    public void TestGapVertexElements() {
      VertexElement[] elements = VertexDeclarationHelper.BuildElementList<GapVertex>();

      Assert.AreEqual(2, elements.Length);
      Assert.Greater(elements[1].Offset, elements[0].Offset);
    }

    /// <summary>
    ///   Only serves to satisfy the compiler. Otherwise, warning CS0414 would occur
    ///   since the fields of the private vertex structures are never assigned to
    /// </summary>
    protected void EliminateCompilerWarnings() {
      TestVertex testVertex;
      testVertex.TestVector2 = Vector2.Zero;
      testVertex.TestVector3 = Vector3.Zero;
      testVertex.TestVector4 = Vector4.Zero;
      testVertex.TestColor = Color.White;
      testVertex.TestSingle = 0.0f;
      testVertex.TestInt = 0;
      testVertex.TestShort = 0;
      testVertex.TestExplicitMatrix = Matrix.Identity;

      UnknownTypeVertex unknownTypeVertex;
      unknownTypeVertex.TestTimestamp = DateTime.MinValue;

      UnattributedFieldVertex unattributedFieldVertex;
      unattributedFieldVertex.TestVector3 = Vector3.Zero;
      unattributedFieldVertex.TestColor = Color.White;

      SecondStreamVertex secondStreamVertex;
      secondStreamVertex.BlendWeight = 0.0f;
      
      GapVertex gapVertex;
      gapVertex.Position = Vector3.Zero;
      gapVertex.Velocity = Vector3.Zero;
      gapVertex.Size = 0.0f;
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
