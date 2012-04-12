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
using System.Reflection;

using Microsoft.Xna.Framework;

namespace Nuclex.Game.Serialization {

  /// <summary>Utility class for serializating objects into binary data</summary>
  public static class BinarySerializer {

    #region System.Collections.Generic.ICollection

    /// <summary>Loads a collection from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the collection</param>
    /// <param name="collection">Collection to be deserialized into</param>
    /// <remarks>
    ///   This method loads right into the collection and is not transactional.
    ///   If an error occurs during loading, the collection is left in
    ///   an intermediate state and no assumptions should be made as to its
    ///   contents. If you need transactional safety, create a temporary collection,
    ///   load into the temporary collection and then replace your actual
    ///   collection with it.
    /// </remarks>
    public static void Load<BinarySerializableType>(
      BinaryReader reader, ICollection<BinarySerializableType> collection
    ) where BinarySerializableType : IBinarySerializable {
      collection.Clear();

      // Read all the serialized objects
      int count = reader.ReadInt32();
      for(int index = 0; index < count; ++index) {

        // Try to create an instance from the serialized type name
        string qualifiedName = reader.ReadString();
        BinarySerializableType item = (BinarySerializableType)Activator.CreateInstance(
          Type.GetType(qualifiedName)
        );

        // Let the instance load its own data and add it to the collection
        ((IBinarySerializable)item).Load(reader);
        collection.Add(item);

      } // for
    }

    /// <summary>Serializes a collection of binary serializable objects</summary>
    /// <param name="writer">BinaryWriter to serialize the collection into</param>
    /// <param name="collection">Collection to be serialized</param>
    public static void Save<BinarySerializableType>(
      BinaryWriter writer, ICollection<BinarySerializableType> collection
    ) where BinarySerializableType : IBinarySerializable {

      // Serialize all the object in the collection
      writer.Write((int)collection.Count);
      foreach(BinarySerializableType item in collection) {

        // Save the type name of the object so we can recreate it later
        writer.Write(item.GetType().AssemblyQualifiedName);

        // Let the object save its own data
        ((IBinarySerializable)item).Save(writer);

      } // foreach

    }

    #endregion // System.Collections.Generic.ICollection

    #region Microsoft.Xna.Framework.Matrix

    /// <summary>Loads a matrix from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the matrix</param>
    /// <param name="matrix">Matrix to be deserialized</param>
    public static void Load(BinaryReader reader, out Matrix matrix) {
      matrix = new Matrix(
        reader.ReadSingle(), // m11
        reader.ReadSingle(), // m12
        reader.ReadSingle(), // m13
        reader.ReadSingle(), // m14

        reader.ReadSingle(), // m21
        reader.ReadSingle(), // m22
        reader.ReadSingle(), // m23
        reader.ReadSingle(), // m24

        reader.ReadSingle(), // m31
        reader.ReadSingle(), // m32
        reader.ReadSingle(), // m33
        reader.ReadSingle(), // m34

        reader.ReadSingle(), // m41
        reader.ReadSingle(), // m42
        reader.ReadSingle(), // m43
        reader.ReadSingle()  // m44
      );
    }

    /// <summary>Serializes a matrix into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the matrix into</param>
    /// <param name="matrix">Matrix to be serialized</param>
    public static void Save(BinaryWriter writer, ref Matrix matrix) {
      writer.Write(matrix.M11);
      writer.Write(matrix.M12);
      writer.Write(matrix.M13);
      writer.Write(matrix.M14);

      writer.Write(matrix.M21);
      writer.Write(matrix.M22);
      writer.Write(matrix.M23);
      writer.Write(matrix.M24);

      writer.Write(matrix.M31);
      writer.Write(matrix.M32);
      writer.Write(matrix.M33);
      writer.Write(matrix.M34);

      writer.Write(matrix.M41);
      writer.Write(matrix.M42);
      writer.Write(matrix.M43);
      writer.Write(matrix.M44);
    }

    #endregion // Microsoft.Xna.Framework.Matrix

    #region Microsoft.Xna.Framework.Vector2

    /// <summary>Loads a vector from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the vector</param>
    /// <param name="vector">Vector to be deserialized</param>
    public static void Load(BinaryReader reader, out Vector2 vector) {
      vector = new Vector2(
        reader.ReadSingle(),
        reader.ReadSingle()
      );
    }

    /// <summary>Serializes a vector into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the vector into</param>
    /// <param name="vector">Vector to be serialized</param>
    public static void Save(BinaryWriter writer, ref Vector2 vector) {
      writer.Write(vector.X);
      writer.Write(vector.Y);
    }

    #endregion // Microsoft.Xna.Framework.Vector2

    #region Microsoft.Xna.Framework.Vector3

    /// <summary>Loads a vector from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the vector</param>
    /// <param name="vector">Vector to be deserialized</param>
    public static void Load(BinaryReader reader, out Vector3 vector) {
      vector = new Vector3(
        reader.ReadSingle(),
        reader.ReadSingle(),
        reader.ReadSingle()
      );
    }

    /// <summary>Serializes a vector into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the vector into</param>
    /// <param name="vector">Vector to be serialized</param>
    public static void Save(BinaryWriter writer, ref Vector3 vector) {
      writer.Write(vector.X);
      writer.Write(vector.Y);
      writer.Write(vector.Z);
    }

    #endregion // Microsoft.Xna.Framework.Vector3

    #region Microsoft.Xna.Framework.Vector4

    /// <summary>Loads a vector from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the vector</param>
    /// <param name="vector">Vector to be deserialized</param>
    public static void Load(BinaryReader reader, out Vector4 vector) {
      // This is valid in C# (but order of evaluation would be undefined in C++)
      vector = new Vector4(
        reader.ReadSingle(),
        reader.ReadSingle(),
        reader.ReadSingle(),
        reader.ReadSingle()
      );
    }

    /// <summary>Serializes a vector into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the vector into</param>
    /// <param name="vector">Vector to be serialized</param>
    public static void Save(BinaryWriter writer, ref Vector4 vector) {
      writer.Write(vector.X);
      writer.Write(vector.Y);
      writer.Write(vector.Z);
      writer.Write(vector.W);
    }

    #endregion // Microsoft.Xna.Framework.Vector4

    #region Microsoft.Xna.Framework.Quaternion

    /// <summary>Loads a quaternion from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the quaternion</param>
    /// <param name="quaternion">Quaternion to be deserialized</param>
    public static void Load(BinaryReader reader, out Quaternion quaternion) {
      // This is valid in C# (but order of evaluation would be undefined in C++)
      quaternion = new Quaternion(
        reader.ReadSingle(),
        reader.ReadSingle(),
        reader.ReadSingle(),
        reader.ReadSingle()
      );
    }

    /// <summary>Serializes a quaternion into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the quaternion into</param>
    /// <param name="quaternion">Quaternion to be serialized</param>
    public static void Save(BinaryWriter writer, ref Quaternion quaternion) {
      writer.Write(quaternion.X);
      writer.Write(quaternion.Y);
      writer.Write(quaternion.Z);
      writer.Write(quaternion.W);
    }

    #endregion // Microsoft.Xna.Framework.Quaternion

    #region Microsoft.Xna.Framework.Curve

    /// <summary>Loads a curve from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the curve</param>
    /// <param name="curve">Curve to be deserialized</param>
    /// <remarks>
    ///   This method loads right into the curve and is not transactional.
    ///   If an error occurs during loading, the curve is left in
    ///   an intermediate state and no assumptions should be made as to its
    ///   contents. If you need transactional safety, create a temporary curve,
    ///   load into the temporary curve and then replace your actual
    ///   curve with it.
    /// </remarks>
    public static void Load(BinaryReader reader, Curve curve) {
      curve.Keys.Clear();

      // Load the curve's loop settings
      curve.PreLoop = (CurveLoopType)reader.ReadByte();
      curve.PostLoop = (CurveLoopType)reader.ReadByte();

      // Load the key frames defined for the curve
      int keyCount = reader.ReadInt32();
      for(int keyIndex = 0; keyIndex < keyCount; ++keyIndex) {
        float position = reader.ReadSingle();
        float value = reader.ReadSingle();
        float tangentIn = reader.ReadSingle();
        float tangentOut = reader.ReadSingle();
        CurveContinuity continuity = (CurveContinuity)reader.ReadByte();

        curve.Keys.Add(new CurveKey(position, value, tangentIn, tangentOut, continuity));
      } // for
    }

    /// <summary>Serializes a curve into a binary data stream</summary>
    /// <param name="writer">BinaryWriter to serialize the curve into</param>
    /// <param name="curve">Curve to be serialized</param>
    public static void Save(BinaryWriter writer, Curve curve) {

      // Save the curve's loop settings
      writer.Write((byte)curve.PreLoop);
      writer.Write((byte)curve.PostLoop);

      // Save the key frames contained in the curve
      writer.Write(curve.Keys.Count);
      for(int keyIndex = 0; keyIndex < curve.Keys.Count; ++keyIndex) {
        CurveKey key = curve.Keys[keyIndex];

        writer.Write(key.Position);
        writer.Write(key.Value);
        writer.Write(key.TangentIn);
        writer.Write(key.TangentOut);
        writer.Write((byte)key.Continuity);
      } // for

    }

    #endregion // Microsoft.Xna.Framework.Curve

  } // class BinarySerializer

} // namespace Nuclex.Game.Serialization
