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

  /// <summary>Scales and unscales generic number</summary>
  /// <typeparam name="ScalarType">Data type of the numbers to be scaled</typeparam>
  public interface IScalingProvider<ScalarType> {

    /// <summary>Scales a value by an arbitrary factor</summary>
    /// <param name="number">Value to be scaled</param>
    /// <param name="factor">Scaling factor</param>
    /// <returns>The scaled value</returns>
    ScalarType Scale(ScalarType number, double factor);

    /// <summary>Unscales a value by an arbitrary factor</summary>
    /// <param name="number">Value to be unscaled</param>
    /// <param name="divisor">Unscaling factor</param>
    /// <returns>The unscaled value</returns>
    ScalarType Unscale(ScalarType number, double divisor);

  }

} // namespace Nuclex.Math.Generic
