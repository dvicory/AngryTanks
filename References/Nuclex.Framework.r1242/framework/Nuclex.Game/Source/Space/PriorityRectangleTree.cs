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

#if ENABLE_RTREES

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Nuclex.Game.Space {

  /// <summary>Priority Rectangle Tree (PR-Tree) for two-dimensional data</summary>
  public partial class PriorityRectangleTree<ItemType> : RectangleTree<ItemType>
    where ItemType : IBoundingRectangleProvider {

    /// <summary>Queries the spatial database for all objects in a region</summary>
    /// <param name="region">Region of which the objects will be returned</param>
    /// <returns>All objects in the queried region</returns>
    public override IEnumerable<ItemType> Query(BoundingRectangle region) {
      throw new NotImplementedException();
    }

    /// <summary>Inserts an object into the spatial database</summary>
    /// <param name="itemToAdd">Item that will be inserted</param>
    public override void Insert(ItemType itemToAdd) {
      throw new NotImplementedException();
    }

    /// <summary>Removes an object from the spatial database</summary>
    /// <param name="itemToRemove">Item that will be removed</param>
    public override void Remove(ItemType itemToRemove) {
      throw new NotImplementedException();
    }

    /// <summary>Updates the position of an object in the spatial database</summary>
    /// <param name="itemToUpdate">Item whose bounding rectangle has changed</param>
    public override void Update(ItemType itemToUpdate) {
      throw new NotImplementedException();
    }

  }

} // namespace Nuclex.Game.Space

#endif // ENABLE_RTREES
