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
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

namespace Nuclex.Graphics {

  /// <summary>Unit tests for the vertex element attribute</summary>
  [TestFixture]
  internal class VertexElementAttributeTest {

    #region struct TestVertex

    /// <summary>A vertex used to test the vertex element attribute</summary>
    private struct TestVertex {

      /// <summary>An element that has only its usage set</summary>
      [VertexElement(VertexElementUsage.Position)]
      public int UsageOnly;

      /// <summary>An element that has its usage and format set</summary>
      [VertexElement(VertexElementUsage.Color, VertexElementFormat.Byte4)]
      public int UsageAndFormat;

      /// <summary>An element that has its stream set to 123</summary>
      [VertexElement(VertexElementUsage.TessellateFactor, Stream = 123)]
      public int Stream123;

#if !XNA_4
      /// <summary>An element that has its method set to LookUp</summary>
      [
        VertexElement(
          VertexElementUsage.TextureCoordinate, Method = VertexElementMethod.LookUp
        )
      ]
      public int LookupMethod;
#endif

      /// <summary>An element that has the same usage as another one</summary>
      [VertexElement(VertexElementUsage.TextureCoordinate, UsageIndex = 1)]
      public int SecondUsage;

    }

    #endregion // struct TestVertex

    /// <summary>Tests whether the usage property is stored by the attribute</summary>
    [Test]
    public void TestUsageConstructor() {
      VertexElementAttribute attribute = getVertexElementAttribute<TestVertex>(
        "UsageOnly"
      );

      Assert.AreEqual(0, attribute.Stream);
      Assert.AreEqual(VertexElementUsage.Position, attribute.Usage);
      Assert.AreEqual(false, attribute.FormatProvided);
#if !XNA_4
      Assert.AreEqual(VertexElementMethod.Default, attribute.Method);
#endif
      Assert.AreEqual(0, attribute.UsageIndex);
    }

    /// <summary>
    ///   Tests whether the usage and format properties are stored by the attribute
    /// </summary>
    [Test]
    public void TestUsageAndFormatConstructor() {
      VertexElementAttribute attribute = getVertexElementAttribute<TestVertex>(
        "UsageAndFormat"
      );

      Assert.AreEqual(0, attribute.Stream);
      Assert.AreEqual(VertexElementUsage.Color, attribute.Usage);
      Assert.AreEqual(true, attribute.FormatProvided);
      Assert.AreEqual(VertexElementFormat.Byte4, attribute.Format);
#if !XNA_4
      Assert.AreEqual(VertexElementMethod.Default, attribute.Method);
#endif
      Assert.AreEqual(0, attribute.UsageIndex);
    }

    /// <summary>Tests whether the stream property is stored by the attribute</summary>
    [Test]
    public void TestStreamProperty() {
      VertexElementAttribute attribute = getVertexElementAttribute<TestVertex>(
        "Stream123"
      );

      Assert.AreEqual(123, attribute.Stream);
      Assert.AreEqual(VertexElementUsage.TessellateFactor, attribute.Usage);
      Assert.AreEqual(false, attribute.FormatProvided);
#if !XNA_4
      Assert.AreEqual(VertexElementMethod.Default, attribute.Method);
#endif
      Assert.AreEqual(0, attribute.UsageIndex);
    }

#if !XNA_4
    /// <summary>Tests whether the method property is stored by the attribute</summary>
    [Test]
    public void TestMethodProperty() {
      VertexElementAttribute attribute = getVertexElementAttribute<TestVertex>(
        "LookupMethod"
      );

      Assert.AreEqual(0, attribute.Stream);
      Assert.AreEqual(VertexElementUsage.TextureCoordinate, attribute.Usage);
      Assert.AreEqual(false, attribute.FormatProvided);
      Assert.AreEqual(VertexElementMethod.LookUp, attribute.Method);
      Assert.AreEqual(0, attribute.UsageIndex);
    }
#endif

    /// <summary>Tests whether the usage index property is stored by the attribute</summary>
    [Test]
    public void TestUsageIndexProperty() {
      VertexElementAttribute attribute = getVertexElementAttribute<TestVertex>(
        "SecondUsage"
      );

      Assert.AreEqual(0, attribute.Stream);
      Assert.AreEqual(VertexElementUsage.TextureCoordinate, attribute.Usage);
      Assert.AreEqual(false, attribute.FormatProvided);
#if !XNA_4
      Assert.AreEqual(VertexElementMethod.Default, attribute.Method);
#endif
      Assert.AreEqual(1, attribute.UsageIndex);
    }

    /// <summary>
    ///   Only serves to satisfy the compiler. Otherwise, warning CS0414 would occur
    ///   since the fields of the private TestVertex structure are never assigned to
    /// </summary>
    protected void EliminateCompilerWarnings() {
      TestVertex myVertex;
      myVertex.UsageOnly = 12;
      myVertex.UsageAndFormat = 34;
      myVertex.Stream123 = 56;
#if !XNA_4
      myVertex.LookupMethod = 78;
#endif
      myVertex.SecondUsage = 90;
    }

    /// <summary>
    ///   Retrieves the vertex element attribute assigned to a field in a structure
    /// </summary>
    /// <typeparam name="VertexType">
    ///   Structure that contains the field of which the vertex element attribute will
    ///   be retrieved
    /// </typeparam>
    /// <param name="fieldName">
    ///   Name of the field to retrieve the vertex element attribute for
    /// </param>
    /// <returns>The vertex element attribute of the requested field</returns>
    private static VertexElementAttribute getVertexElementAttribute<VertexType>(
      string fieldName
    ) where VertexType : struct {
      Type vertexType = typeof(VertexType);
      FieldInfo field = vertexType.GetField(
        fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
      );
      object[] attributes = field.GetCustomAttributes(
        typeof(VertexElementAttribute), false
      );

      // The docs state that if the requested attribute has not been applied to the field,
      // an array of length 0 will be returned.
      if(attributes.Length == 0) {
        throw new InvalidOperationException("Field does not have a VertexElementAttribute");
      }

      return (VertexElementAttribute)attributes[0];
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST