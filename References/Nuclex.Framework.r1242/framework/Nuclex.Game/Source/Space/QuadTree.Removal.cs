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

    /// <summary>Removes an item from the spatial database</summary>
    /// <param name="itemToRemove">Item that will be removed</param>
    public override bool Remove(ItemType itemToRemove) {
      return false;
    }

#if false
    /// <summary>
    ///   Recursively tries to remove an item from the quad-tree
    /// </summary>
    /// <param name="startNode">Node at which to begin the search</param>
    /// <param name="rectangle">Rectangle of the item that will be removed</param>
    /// <param name="item">Item that will be removed if found</param>
    /// <param name="found">Set to true if the item was found and removed</param>
    /// <returns>Whether the node can be collapsed</returns>
    /// <remarks>
    ///   Here's how the removal works:
    ///   <list type="number">
    ///     <item><description>
    ///       Recursively go down to the node that should contain the item.
    ///     </description></item>
    ///     <item><description>
    ///       If not found, return false (all the way up the recursion chain).
    ///       If found, see if the node should be collapsed. Requires deep tree scan.
    ///     </description></item>
    ///     <item><description>
    ///       Up in the recursion, if nested call returned false, return false as well.
    ///       If nested call returned true, deep-check whether collapse should be
    ///       performed, then collapse and return true if so.
    ///     </description></item>
    ///     <item><description>
    ///       Highest level is direct call, so root node will never be collapsed.
    ///     </description></item>
    ///   </list>
    /// </remarks>
    private bool recursiveRemoveItem(
      Node startNode, ref BoundingRectangle rectangle, ItemType item, out bool found
    ) {
      /*
        bool descended = false;
        bool collapsable = false;

        if(startNode.TL != null) {
          bool collapsed = false;

          // We already know that the rectangle is completely contained in the starting
          // node, so we only have to check which side of the center the item is on.
          // If it crosses any of the center axes, it cannot go any lower.
          if(rectangle.Max.X < startNode.Center.X) {
            if(rectangle.Max.Y < startNode.Center.Y) {
              descended = true;
              collapsed = recursiveRemoveItem(startNode.TL, ref rectangle, item, out found);
            } else if(rectangle.Min.Y >= startNode.Center.Y) {
              descended = true;
              collapsed = recursiveRemoveItem(startNode.BL, ref rectangle, item, out found);
            }
          } else if(rectangle.Min.X >= startNode.Center.X) {
            if(rectangle.Max.Y < startNode.Center.Y) {
              descended = true;
              collapsed = recursiveRemoveItem(startNode.TR, ref rectangle, item, out found);
            } else if(rectangle.Min.Y >= startNode.Center.Y) {
              collapsed = recursiveRemoveItem(startNode.BR, ref rectangle, item, out found);
            }
          }

        }
        
        if(!descended) {

  */
      /*
                    int collapsedItemCount =
                      startNode.ItemCount +
                      startNode.TL.ItemCount +
                      startNode.TR.ItemCount +
                      startNode.BL.ItemCount +
                      startNode.BR.ItemCount;

                    return (collapsedItemCount <= this.minItemsPerNode);
      */
      /*
                  }
                }
              }

            }
      */
      found = false;
      return false;
    }


    /// <summary>Removes an item from a node</summary>
    /// <param name="node">Node the item will be removed from</param>
    /// <param name="item">Item that will be removed from the node</param>
    /// <returns>True if the item was found and has been removed</returns>
    private bool removeItemFromNode(Node node, ItemType item) {

      // The node must be crossing either the X or Y axis of this node's center,
      // so it cannot be contained in any of our child nodes
      for(int index = 0; index < node.ItemCount; ++index) {
        if(this.comparer.Equals(node.Items[index], item)) {
          node.Items[index] = node.Items[node.ItemCount - 1];
          --node.ItemCount;
          node.Items[node.ItemCount] = default(ItemType);

          return true;
        }
      }

      return false;
    }
#endif
  }

} // namespace Nuclex.Game.Space

#endif // ENABLE_QUADTREES
