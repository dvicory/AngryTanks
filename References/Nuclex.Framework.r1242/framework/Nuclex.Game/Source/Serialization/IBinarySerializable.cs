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

namespace Nuclex.Game.Serialization {

  /// <summary>Interface for objects able to serialize themselfes into a binary format</summary>
  /// <remarks>
  ///   Sometimes, the limitations of XML serialization are too strict, especially
  ///   in the context of a game where you might need to serialize larger chunks of
  ///   binary data or in cases where you do not wish to expose a default constructor
  ///   in your classes. This interface defines two simple methods that can be
  ///   used to load and save an object's state in a simple manner.
  /// </remarks>
  public interface IBinarySerializable {

    /// <summary>Loads the object's state from its serialized representation</summary>
    /// <param name="reader">Reader to use for reading the object's state</param>
    void Load(BinaryReader reader);
    
    /// <summary>Saves the object's state into a serialized representation</summary>
    /// <param name="writer">Writer to use for writing the object's state</param>
    void Save(BinaryWriter writer);

  }

} // namespace Nuclex.Game.Serialization
