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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects {

  /// <summary>Unit test for the static mesh graphics resource keeper</summary>
  [TestFixture]
  internal class StaticMeshTest {

    #region struct TestVertex

    /// <summary>
    ///   Vertex used to unit-test the static mesh graphics resource keepr
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct TestVertex
#if XNA_4
 : IVertexType
#endif
 {

      /// <summary>A vertex element of type Vector2</summary>
      [VertexElement(VertexElementUsage.Position)]
      public Vector3 Position;
      /// <summary>A vertex element of type Color</summary>
      [VertexElement(VertexElementUsage.Color)]
      public Color Color;

#if XNA_4

      /// <summary>Provides a declaration for this vertex type</summary>
      VertexDeclaration IVertexType.VertexDeclaration {
        get { return TestVertex.VertexDeclaration; }
      }

      /// <summary>Vertex declaration for this vertex structure</summary>
      public static readonly VertexDeclaration VertexDeclaration =
        new VertexDeclaration(VertexDeclarationHelper.BuildElementList<TestVertex>());

#else

      /// <summary>Size of one vertex</summary>
      public const int Stride = 16;

#endif

    }

    #endregion // struct TestVertex

    #region class TestStaticMesh

    /// <summary>Dummy static mesh class used for unit testing</summary>
    private class TestStaticMesh : StaticMesh<TestVertex> {

      /// <summary>
      ///   Initializes a new static mesh that automatically determines its vertex format
      /// </summary>
      /// <param name="graphicsDevice">Graphics device the static mesh lives on</param>
      /// <param name="vertexCount">Number of vertices in the static mesh</param>
      public TestStaticMesh(GraphicsDevice graphicsDevice, int vertexCount) :
        base(graphicsDevice, vertexCount) { }

#if !XNA_4

      /// <summary>
      ///   Initializes a new static mesh with an explicit vertex declaration
      /// </summary>
      /// <param name="graphicsDevice">Graphics device the static mesh lives on</param>
      /// <param name="vertexDeclaration">
      ///   Vertex declaration for the vertices of the static mesh
      /// </param>
      /// <param name="vertexCount">Number of vertices in the static mesh</param>
      public TestStaticMesh(
        GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount
      ) :
        base(graphicsDevice, vertexDeclaration, TestVertex.Stride, vertexCount) { }

#endif

      /// <summary>Selects the static meshes' vertex buffer</summary>
      public new void Select() {
        base.Select();
      }

      /// <summary>Vertex buffer containing the test meshes' vertices</summary>
      public new VertexBuffer VertexBuffer {
        get { return base.VertexBuffer; }
      }

    }

    #endregion // class TestStaticMesh

    /// <summary>
    ///   Verifies that the simple constructor of the static mesh class is working
    /// </summary>
    [Test]
    public void TestSimpleConstructor() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        using(
          TestStaticMesh test = new TestStaticMesh(
            mockGraphicsDeviceService.GraphicsDevice, 4
          )
        ) { }
      }
    }

#if !XNA_4

    /// <summary>
    ///   Verifies that the full constructor of the static mesh class is working
    /// </summary>
    [Test]
    public void TestFullConstructor() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        VertexElement[] elements = VertexDeclarationHelper.BuildElementList<TestVertex>();
        using(
          VertexDeclaration declaration = new VertexDeclaration(
            mockGraphicsDeviceService.GraphicsDevice, elements
          )
        ) {
          using(
            TestStaticMesh test = new TestStaticMesh(
              mockGraphicsDeviceService.GraphicsDevice, declaration, 4
            )
          ) { }
        }
      }
    }

#endif

    /// <summary>
    ///   Verifies that the constructor rolls back when an exception occurs in it
    /// </summary>
    [Test]
    public void TestThrowInConstructorRollback() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        Assert.Throws<ArgumentOutOfRangeException>(
          delegate() {
            using(
              TestStaticMesh test = new TestStaticMesh(
                mockGraphicsDeviceService.GraphicsDevice, -1
              )
            ) { }
          }
        );
      }
    }

    /// <summary>
    ///   Tests whether the static meshes' Select() method is implemented correctly
    /// </summary>
    [Test]
    public void TestSelect() {
      MockedGraphicsDeviceService mockGraphicsDeviceService =
        new MockedGraphicsDeviceService();

      using(IDisposable keeper = mockGraphicsDeviceService.CreateDevice()) {
        using(
          TestStaticMesh test = new TestStaticMesh(
            mockGraphicsDeviceService.GraphicsDevice, 4
          )
        ) {
          test.Select();
#if XNA_4
          Assert.AreSame(
            test.VertexBuffer,
            mockGraphicsDeviceService.GraphicsDevice.GetVertexBuffers()[0].VertexBuffer
          );
#else
          Assert.AreSame(
            test.VertexBuffer,
            mockGraphicsDeviceService.GraphicsDevice.Vertices[0].VertexBuffer
          );
#endif
        }
      }
    }

    /// <summary>
    ///   Only exists to prevent the compiler from complaining about unused fields
    /// </summary>
    protected void AvoidCompilerWarnings() {
      TestVertex v;
      v.Color = Color.Red;
      v.Position = Vector3.Zero;
    }

  }

} // namespace Nuclex.Graphics.SpecialEffects

#endif // UNITTEST
