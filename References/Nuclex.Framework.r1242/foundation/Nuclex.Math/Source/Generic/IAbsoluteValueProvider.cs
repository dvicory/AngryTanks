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

namespace Nuclex.Math.Generic {

  /// <summary>Provides numbers with special meanings for a data type</summary>
  /// <typeparam name="ScalarType">Data type for which to provide special numbers</typeparam>
  public interface IAbsoluteValueProvider<ScalarType> {

    /// <summary>The number 0</summary>
    ScalarType Zero { get; }
    /// <summary>The number 1</summary>
    ScalarType One { get; }

    /// <summary>
    ///   The lower end of the value range that can be expressed by the data type
    /// </summary>
    ScalarType MinValue { get; }
    /// <summary>
    ///   The upper end of the value range that can be expressed by the data type
    /// </summary>
    ScalarType MaxValue { get; }

    /// <summary>
    ///   The smallest change in value that can be expressed with the data type
    /// </summary>
    ScalarType Epsilon { get; }

  }

} // namespace Nuclex.Math.Generic
