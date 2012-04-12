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

namespace Nuclex.Game.Space {

  /// <summary>Two-dimensional spatial index</summary>
  /// <typeparam name="ItemType">Type of the items being indexed</typeparam>
  /// <remarks>
  ///   <para>
  ///     This class serves as the base class for spatial indexes that allow for
  ///     efficient searches in large sets of two-dimensional objects.
  ///   </para>
  /// </remarks>
  public abstract class SpatialIndex2<ItemType> {

    /// <summary>Queries the spatial database for all items in a region</summary>
    /// <param name="region">Region of which the items will be returned</param>
    /// <returns>An enumerator for all items in the queried region</returns>
    public virtual IEnumerable<ItemType> Query(BoundingRectangle region) {
      List<ItemType> items = new List<ItemType>();
      Query(region, items);

      for(int index = 0; index < items.Count; ++index) {
        yield return items[index];
      }
    }

    /// <summary>Queries the spatial database for all objects in a region</summary>
    /// <param name="region">Region of which the items will be returned</param>
    /// <param name="items">
    ///   Collection that will receive all items in the query region
    /// </param>
    /// <remarks>
    ///   Use this method to avoid generating garbage by reusing the collection
    ///   the queried items are stored in.
    /// </remarks>
    public abstract void Query(BoundingRectangle region, ICollection<ItemType> items);

    /// <summary>Inserts an item into the spatial database</summary>
    /// <param name="itemToAdd">Item that will be inserted</param>
    public abstract void Insert(ItemType itemToAdd);

    /// <summary>Removes an item from the spatial database</summary>
    /// <param name="itemToRemove">Item that will be removed</param>
    public abstract bool Remove(ItemType itemToRemove);

  }

} // namespace Nuclex.Game.Space
