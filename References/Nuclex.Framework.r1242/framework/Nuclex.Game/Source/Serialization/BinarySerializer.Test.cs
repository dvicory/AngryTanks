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
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;

using NUnit.Framework;

namespace Nuclex.Game.Serialization {

  /// <summary>Ensures that the binary serializer is working correctly</summary>
  [TestFixture]
  internal class BinarySerializerTest {

    #region class TestSerializable

    /// <summary>Serializable class used to test the serialization code</summary>
    private class TestSerializable : IBinarySerializable {
      /// <summary>Loads the state of the object from binary data</summary>
      /// <param name="reader">Reader containing the binary state of the object</param>
      public void Load(BinaryReader reader) { this.Dummy = reader.ReadInt32(); }
      /// <summary>Saves the state of the object as binary data</summary>
      /// <param name="writer">
      ///   Writer into which the binary state of the object will be written
      /// </param>
      public void Save(BinaryWriter writer) { writer.Write(this.Dummy); }
      /// <summary>Serialized value representing the state of this object</summary>
      public int Dummy;
    }

    #endregion // class TestSerializable

    #region class CurveComparer

    /// <summary>Compares two curves against each other</summary>
    private class CurveComparer : IComparer<Curve>, IComparer {

      /// <summary>Initializes a new curve comparer</summary>
      private CurveComparer() { }

      /// <summary>The one and only instance of the curve comparer</summary>
      public static CurveComparer Instance {
        get { return instance; }
      }

      /// <summary>Compares two curves against each other</summary>
      /// <param name="left">Compared curve in the left side</param>
      /// <param name="right">Compared curve on the right side</param>
      /// <returns>The relation of the provided curves to each other</returns>
      public int Compare(Curve left, Curve right) {
        // Compare IsConstant flag (bool)
        if(left.IsConstant != right.IsConstant) {
          if(left.IsConstant) {
            return -1;
          } else {
            return 1;
          }
        }

        // Compare PreLoop (enum)
        int preLoopDifference = (int)right.PreLoop - (int)left.PreLoop;
        if(preLoopDifference != 0) {
          return preLoopDifference;
        }

        // Compare keys (collection)
        int keysDifference = compareKeys(left.Keys, right.Keys);
        if(keysDifference != 0) {
          return keysDifference;
        }

        // Compare PostLoop (enum)
        int postLoopDifference = (int)right.PostLoop - (int)left.PostLoop;
        if(postLoopDifference != 0) {
          return postLoopDifference;
        }

        return 0;
      }

      /// <summary>Compares two curves against each other</summary>
      /// <param name="left">Compared curve in the left side</param>
      /// <param name="right">Compared curve on the right side</param>
      /// <returns>The relation of the provided curves to each other</returns>
      public int Compare(object left, object right) {
        if(!(left is Curve)) {
          throw new ArgumentException("Left argument is not a curve", "left");
        }
        if(!(right is Curve)) {
          throw new ArgumentException("Right argument is not a curve", "right");
        }

        return Compare((Curve)left, (Curve)right);
      }

      /// <summary>Compares the key collections of two curves against each other</summary>
      /// <param name="leftKeys">Keys in the key collection to the left side</param>
      /// <param name="rightKeys">Keys in the key collection to the right side</param>
      /// <returns>The rleation of the two provided key collections to each other</returns>
      private int compareKeys(CurveKeyCollection leftKeys, CurveKeyCollection rightKeys) {

        // Compare number of keys (int)
        int keyCountDifference = (int)rightKeys.Count - (int)leftKeys.Count;
        if(keyCountDifference != 0) {
          return keyCountDifference;
        }

        for(int index = 0; index < leftKeys.Count; ++index) {

          // Compare Continuity (enum)
          int continuityDifference =
            (int)rightKeys[index].Continuity - (int)leftKeys[index].Continuity;
          if(continuityDifference != 0) {
            return continuityDifference;
          }

          // Compare TangentIn (float)
          if(leftKeys[index].TangentIn > rightKeys[index].TangentIn) {
            return +1;
          } else if(leftKeys[index].TangentIn < rightKeys[index].TangentIn) {
            return -1;
          }

          // Compare Position (float)
          if(leftKeys[index].Position > rightKeys[index].Position) {
            return +1;
          } else if(leftKeys[index].Position < rightKeys[index].Position) {
            return -1;
          }

          // Compare Value (float)
          if(leftKeys[index].Value > rightKeys[index].Value) {
            return +1;
          } else if(leftKeys[index].Value < rightKeys[index].Value) {
            return -1;
          }

          // Compare TangentOut (float)
          if(leftKeys[index].TangentOut > rightKeys[index].TangentOut) {
            return +1;
          } else if(leftKeys[index].TangentOut < rightKeys[index].TangentOut) {
            return -1;
          }
        }

        // No differences found
        return 0;

      }

      /// <summary>The one and only instance of the curve comparer</summary>
      private static CurveComparer instance = new CurveComparer();

    }

    #endregion // class CurveComparer

    /// <summary>
    ///   Tests wether a simple collection can be successfully saved and loaded again
    /// </summary>
    [Test]
    public void TestSimpleCollection() {
      MemoryStream buffer = new MemoryStream();

      // Fill and save
      {
        List<TestSerializable> serializables = new List<TestSerializable>();

        serializables.Add(new TestSerializable());
        serializables.Add(new TestSerializable());
        serializables[0].Dummy = 123;
        serializables[1].Dummy = 456;

        BinarySerializer.Save(new BinaryWriter(buffer), serializables);
        buffer.Position = 0;
      }

      // Load and validate
      {
        List<TestSerializable> serializables = new List<TestSerializable>();

        BinarySerializer.Load(new BinaryReader(buffer), serializables);

        Assert.AreEqual(2, serializables.Count);
        Assert.AreEqual(123, serializables[0].Dummy);
        Assert.AreEqual(456, serializables[1].Dummy);
      }
    }

    /// <summary>
    ///   Verifies that matrices can be serialized and restored again
    /// </summary>
    [Test]
    public void TestMatrixSerialization() {
      MemoryStream buffer = new MemoryStream();

      Matrix testMatrix = new Matrix(
        1.1f, 1.2f, 1.3f, 1.4f,
        2.1f, 2.2f, 2.3f, 2.4f,
        3.1f, 3.2f, 3.3f, 3.4f,
        4.1f, 4.2f, 4.3f, 4.4f
      );

      // Save
      {
        BinarySerializer.Save(new BinaryWriter(buffer), ref testMatrix);
        buffer.Position = 0;
      }

      // Load and validate
      {
        Matrix loadedMatrix;

        BinarySerializer.Load(new BinaryReader(buffer), out loadedMatrix);

        Assert.AreEqual(testMatrix, loadedMatrix);
      }
    }

    /// <summary>
    ///   Verifies that a Vector2 can be serialized and restored again
    /// </summary>
    [Test]
    public void TestVector2Serialization() {
      MemoryStream buffer = new MemoryStream();

      Vector2 testVector = new Vector2(1.1f, 2.2f);

      // Save
      {
        BinarySerializer.Save(new BinaryWriter(buffer), ref testVector);
        buffer.Position = 0;
      }

      // Load and validate
      {
        Vector2 loadedVector;

        BinarySerializer.Load(new BinaryReader(buffer), out loadedVector);

        Assert.AreEqual(testVector, loadedVector);
      }
    }

    /// <summary>
    ///   Verifies that a Vector3 can be serialized and restored again
    /// </summary>
    [Test]
    public void TestVector3Serialization() {
      MemoryStream buffer = new MemoryStream();

      Vector3 testVector = new Vector3(1.1f, 2.2f, 3.3f);

      // Save
      {
        BinarySerializer.Save(new BinaryWriter(buffer), ref testVector);
        buffer.Position = 0;
      }

      // Load and validate
      {
        Vector3 loadedVector;

        BinarySerializer.Load(new BinaryReader(buffer), out loadedVector);

        Assert.AreEqual(testVector, loadedVector);
      }
    }

    /// <summary>
    ///   Verifies that a Vector4 can be serialized and restored again
    /// </summary>
    [Test]
    public void TestVector4Serialization() {
      MemoryStream buffer = new MemoryStream();

      Vector4 testVector = new Vector4(1.1f, 2.2f, 3.3f, 4.4f);

      // Save
      {
        BinarySerializer.Save(new BinaryWriter(buffer), ref testVector);
        buffer.Position = 0;
      }

      // Load and validate
      {
        Vector4 loadedVector;

        BinarySerializer.Load(new BinaryReader(buffer), out loadedVector);

        Assert.AreEqual(testVector, loadedVector);
      }
    }

    /// <summary>
    ///   Verifies that a quaternion can be serialized and restored again
    /// </summary>
    [Test]
    public void TestQuaternionSerialization() {
      MemoryStream buffer = new MemoryStream();

      Quaternion testQuaternion = new Quaternion(1.1f, 2.2f, 3.3f, 4.4f);

      // Save
      {
        BinarySerializer.Save(new BinaryWriter(buffer), ref testQuaternion);
        buffer.Position = 0;
      }

      // Load and validate
      {
        Quaternion loadedQuaternion;

        BinarySerializer.Load(new BinaryReader(buffer), out loadedQuaternion);

        Assert.AreEqual(testQuaternion, loadedQuaternion);
      }
    }

    /// <summary>
    ///   Verifies that a curve can be serialized and restored again
    /// </summary>
    [Test]
    public void TestCurveSerialization() {
      MemoryStream buffer = new MemoryStream();

      Curve testCurve = new Curve();
      testCurve.PreLoop = CurveLoopType.Cycle;
      testCurve.PostLoop = CurveLoopType.Oscillate;
      testCurve.Keys.Add(new CurveKey(1.2f, 3.4f, 5.6f, 7.8f));
      testCurve.Keys[0].Continuity = CurveContinuity.Smooth;
      testCurve.Keys.Add(new CurveKey(8.7f, 6.5f, 4.3f, 2.1f));
      testCurve.Keys[1].Continuity = CurveContinuity.Step;

      // Save
      {
        BinarySerializer.Save(new BinaryWriter(buffer), testCurve);
        buffer.Position = 0;
      }

      // Load and validate
      {
        Curve loadedCurve = new Curve();

        BinarySerializer.Load(new BinaryReader(buffer), loadedCurve);

        Assert.That(
          testCurve, Is.EqualTo(loadedCurve).Using<Curve>(CurveComparer.Instance)
        );
      }
    }

  }

} // namespace Nuclex.Game.Serialization

#endif // UNITTEST
