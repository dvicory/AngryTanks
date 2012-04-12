﻿#region CPL License
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

namespace Nuclex.Game.Space {

  /// <summary>
  ///   Interface for geometrical objects providing a bounding rectangle
  /// </summary>
  public interface IBoundingRectangleProvider {

    /// <summary>Bounding rectangle of the geometrical object</summary>
    BoundingRectangle BoundingRectangle { get; }

  }

} // namespace Nuclex.Game.Space
