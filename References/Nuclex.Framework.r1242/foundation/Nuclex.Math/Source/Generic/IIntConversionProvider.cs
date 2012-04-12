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

  /// <summary>Provides integer conversion operations for a data type</summary>
  /// <typeparam name="ScalarType">
  ///   Data type for which to provide conversion operations
  /// </typeparam>
  public interface IIntConversionProvider<ScalarType> {

    /// <summary>Converts an integer to the generic data type</summary>
    /// <param name="value">Value to convert</param>
    /// <returns>The input value as the generic data type</returns>
    ScalarType FromInteger(int value);

    /// <summary>Converts a generic data type into an integer</summary>
    /// <param name="value">Generic value to convert</param>
    /// <returns>The generic input value as an integer</returns>
    int ToInteger(ScalarType value);

  }

} // namespace Nuclex.Math.Generic
