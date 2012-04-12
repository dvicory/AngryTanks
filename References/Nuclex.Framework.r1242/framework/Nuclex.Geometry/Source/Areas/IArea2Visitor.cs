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
using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Areas {

  /// <summary>Base class for area visitors</summary>
  /// <remarks>
  ///   See the visitor pattern. 
  ///   <para>
  ///     If you need to perform work on a area whose type you do not know, you could
  ///     of course just create a set of if(area is ...) queries. However, you would
  ///     have to manually track down all these queries should a future version of this
  ///     library provide a new type of volume.
  ///   </para>
  ///   <para>
  ///     Your other option is to create a special class that does your work which derives
  ///     from this AreaVisitor class. It will elegantly resolve the area type by
  ///     calling into the area (resolving the type via the vtable) which then calls
  ///     the visitor's distinctive method for the exact kind of area. If a new area
  ///     is introduced to the library, it will be added to the AreaVisitor class and
  ///     your compiler will point out to you where you need to extend your code for the
  ///     new kind of area because the abstract method will not yet be implemented there.
  ///   </para>
  /// </remarks>
  public interface IArea2Visitor {

    /// <summary>Visit an axis aligned rectangle</summary>
    /// <param name="rectangle">Axis aligned rectangle to visit</param>
    void Visit(AxisAlignedRectangle2 rectangle);

    /// <summary>Visit a rectangle</summary>
    /// <param name="rectangle">Rectangle to visit</param>
    void Visit(Rectangle2 rectangle);

    /// <summary>Visit a triangle</summary>
    /// <param name="triangle">Triangle to visit</param>
    void Visit(Triangle2 triangle);

    /// <summary>Visit a disc</summary>
    /// <param name="disc">Disc to visit</param>
    void Visit(Disc2 disc);

  }

} // namespace Nuclex.Geometry.Areas
