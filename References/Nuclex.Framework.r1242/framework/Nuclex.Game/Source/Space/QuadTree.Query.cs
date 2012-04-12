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

#if ENABLE_QUADTREES

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Nuclex.Game.Space {

  partial class QuadTree<ItemType> {

    #if false
    /// <summary>Queries the spatial database for all items in a region</summary>
    /// <param name="region">Region of which the items will be returned</param>
    /// <returns>An enumerator for all items in the queried region</returns>
    public override IEnumerable<ItemType> Query(BoundingRectangle region) {
      throw new NotImplementedException();
    }
    #endif

    /// <summary>Queries the spatial database for all objects in a region</summary>
    /// <param name="region">Region of which the items will be returned</param>
    /// <param name="items">
    ///   Collection that will receive all items in the query region
    /// </param>
    /// <remarks>
    ///   Use this method to avoid generating garbage by reusing the collection
    ///   the queried items are stored in.
    /// </remarks>
    public override void Query(BoundingRectangle region, ICollection<ItemType> items) {
      recursiveQueryRegion(
        this.root,
        new Vector2(float.NegativeInfinity),
        new Vector2(float.PositiveInfinity),
        ref region,
        items
      );
    }

    /// <summary>Processes a region query in a partially intersected node</summary>
    /// <param name="startNode">Node at which to begin the search</param>
    /// <param name="min">Minimum coordinates of the node's bounding rectangle</param>
    /// <param name="max">Maxumum coordinates of the node's bounding rectangle</param>
    /// <param name="region">Region of which all items will be returned</param>
    /// <param name="items">
    ///   Collection into which all discovered items will be written
    /// </param>
    private static void recursiveQueryRegion(
      Node startNode, Vector2 min, Vector2 max, //BoundingRectangle nodeBounds,
      ref BoundingRectangle region, ICollection<ItemType> items
    ) {
      System.Diagnostics.Debug.WriteLine(
        "Checking " + startNode.BoundingRectangle.ToString()
      );
    
      // We assume the starting node is only partially covered by the region. This is
      // useful because, when a new query is made, the first node will be the root
      // node and thus contain all items in problematic spots.
      for(int index = 0; index < startNode.ItemCount; ++index) {
        bool result;
        startNode.Items[index].BoundingRectangle.Intersects(ref region, out result);

        if(result) {
          items.Add(startNode.Items[index]);
        }
      }

      if(startNode.TL != null) {

        // Determine which quadrants have been touched by the region
        bool leftTouched = (region.Min.X < startNode.Center.X);
        bool rightTouched = (region.Max.X > startNode.Center.X);
        bool topTouched = (region.Min.Y < startNode.Center.Y);
        bool bottomTouched = (region.Max.Y > startNode.Center.Y);

        //bool leftContained = rightTouched && (region.Min.X < min.X);
        //bool topContained = bottomTouched && (region.Min.Y < min.Y);
        //bool rightContained = leftTouched && (region.Max.X > max.X);
        //bool bottomContained = topTouched && (region.Max.Y > max.Y);

        // Blindly add all quadrants which are entirely enclosed by the region and
        // proceed to recursively scan those which are only touched by it.
        #region Today's entry for the ugly code competition

        // This achieves a cyclomatic complexity of 23 in a single method! Yay!
        if(leftTouched) {
          bool leftContained = rightTouched && (region.Min.X < min.X);

          if(leftContained) {
            if(topTouched) {
              bool topContained = bottomTouched && (region.Min.Y < min.Y);

              if(topContained) { // All TL
                recursiveAddRegion(startNode.TL, items);
              } else { // Some TL
                recursiveQueryRegion(
                  startNode.TL, min, startNode.Center, ref region, items
                );
              }
            }
            if(bottomTouched) {
              bool bottomContained = topTouched && (region.Max.Y > max.Y);

              if(bottomContained) { // All BL
                recursiveAddRegion(startNode.BL, items);
              } else { // Some BL
                Vector2 blMin = new Vector2(min.X, startNode.Center.Y);
                Vector2 blMax = new Vector2(startNode.Center.X, max.X);
                recursiveQueryRegion(startNode.BL, blMin, blMax, ref region, items);
              }
            }
          } else {
            if(topTouched) { // Some TL
              recursiveQueryRegion(
                startNode.TL, min, startNode.Center, ref region, items
              );
            }
            if(bottomTouched) { // Some BL
              Vector2 blMin = new Vector2(min.X, startNode.Center.Y);
              Vector2 blMax = new Vector2(startNode.Center.X, max.X);
              recursiveQueryRegion(startNode.BL, blMin, blMax, ref region, items);
            }
          }
        }
        if(rightTouched) {
          bool rightContained = leftTouched && (region.Max.X > max.X);

          if(rightContained) {
            if(topTouched) {
              bool topContained = bottomTouched && (region.Min.Y < min.Y);

              if(topContained) { // All TR
                recursiveAddRegion(startNode.TR, items);
              } else { // Some TR
                Vector2 brMin = new Vector2(startNode.Center.X, min.Y);
                Vector2 brMax = new Vector2(max.X, startNode.Center.Y);
                recursiveQueryRegion(startNode.TR, brMin, brMax, ref region, items);
              }
            }
            if(bottomTouched) {
              bool bottomContained = topTouched && (region.Max.Y > max.Y);

              if(bottomContained) { // All BR
                recursiveAddRegion(startNode.BR, items);
              } else { // Some BR
                recursiveQueryRegion(
                  startNode.BR, startNode.Center, max, ref region, items
                );
              }
            }
          } else {
            if(topTouched) { // Some TR
              Vector2 brMin = new Vector2(startNode.Center.X, min.Y);
              Vector2 brMax = new Vector2(max.X, startNode.Center.Y);
              recursiveQueryRegion(startNode.TR, brMin, brMax, ref region, items);
            }
            if(bottomTouched) { // Some BR
              recursiveQueryRegion(
                startNode.BR, startNode.Center, max, ref region, items
              );
            }
          }
        }

        #endregion // Today's entry for the ugly code competition
      }
    }

    /// <summary>Adds all items in a node and its subnodes to a collection</summary>
    /// <param name="startNode">Node at which to begin adding items</param>
    /// <param name="items">Collection into which the items will be added</param>
    private static void recursiveAddRegion(
      Node startNode, ICollection<ItemType> items
    ) {
      for(int index = 0; index < startNode.ItemCount; ++index) {
        items.Add(startNode.Items[index]);
      }

      if(startNode.TL != null) {
        recursiveAddRegion(startNode.TL, items);
        recursiveAddRegion(startNode.TR, items);
        recursiveAddRegion(startNode.BL, items);
        recursiveAddRegion(startNode.BR, items);
      }
    }

  }

} // namespace Nuclex.Game.Space

#endif // ENABLE_QUADTREES
