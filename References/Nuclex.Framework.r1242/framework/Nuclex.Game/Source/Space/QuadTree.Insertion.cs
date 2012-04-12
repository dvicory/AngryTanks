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

    /// <summary>Inserts an item into the spatial database</summary>
    /// <param name="itemToAdd">Item that will be inserted</param>
    public override void Insert(ItemType itemToAdd) {
      BoundingRectangle rectangle = itemToAdd.BoundingRectangle;

      // Find the smallest node the item can be added to. Items outside of the
      // quad-tree will be placed in the smallest node at the border of the tree
      // to provide at least some degree of indexing for these items.
      Node insertNode = recursiveFindSmallestContainingNode(this.root, ref rectangle);

      // Now see if this node is becoming too populated and needs to be split
      while((insertNode.TL == null) && (insertNode.ItemCount >= this.MaxItemsPerNode)) {
        if(!splitNode(insertNode)) {
          break;
        }

        insertNode = recursiveFindSmallestContainingNode(insertNode, ref rectangle);
      }

      // If the node's item list overflows, extend its capacity. This does not normally
      // happen, but if many items cluster along a quad-tree's bad spots, we are
      // forced to keep more than maxItemsPerNode items in a node.
      if(insertNode.ItemCount == insertNode.Items.Length) {
        increaseNodeCapacity(insertNode);
      }

      // Add the item to the item list of the target node. There will always be
      // at least one free slot in a node.
      insertNode.Items[insertNode.ItemCount] = itemToAdd;
      ++insertNode.ItemCount;
    }

    /// <summary>
    ///   Searches the items in the node for the first item that would benefit
    ///   from splitting the node into smaller nodes
    /// </summary>
    /// <param name="node">Node whose items will be scanned</param>
    /// <returns>
    ///   The index of the first item that would benefit from a split or -1 if
    ///   splitting the node wouldn't result in any improvement
    /// </returns>
    private int findFirstSplittableItem(Node node) {
      BoundingRectangle rectangle;

      // Check all items that remain unchecked whether they would benefit
      // from splitting this node
      for(int index = node.FirstSplittableItemIndex; index < node.ItemCount; ++index) {
        rectangle = node.Items[index].BoundingRectangle;

        // See if this item would fit into a child node and thus be
        // a benefactor of a split
        bool benefitsFromSplit =
          (rectangle.Max.X < node.Center.X) ||
          (rectangle.Min.X > node.Center.X) ||
          (rectangle.Max.Y < node.Center.Y) ||
          (rectangle.Min.Y > node.Center.Y);

        if(benefitsFromSplit) {
          bool outsideCorner =
            (
              (rectangle.Max.X < this.root.BoundingRectangle.Min.X) ||
              (rectangle.Min.X > this.root.BoundingRectangle.Max.X)
            )
            &&
            (
              (rectangle.Max.Y < this.root.BoundingRectangle.Min.Y) ||
              (rectangle.Min.Y > this.root.BoundingRectangle.Max.Y)
            );

          if(!outsideCorner) {
            node.FirstSplittableItemIndex = index;
            return index;
          }
        }
      }

      // No items would benefit from splitting this node
      node.FirstSplittableItemIndex = node.ItemCount;
      return -1;
    }

    /// <summary>Splits a node into child nodes if there's any benefit</summary>
    /// <param name="node">Node that will be split if there's a benefit</param>
    /// <returns>True if the node has been split</returns>
    private bool splitNode(Node node) {

      // Before splitting, scan the nodes until we reach an item that would benefit
      // from being moved into a child node. If none are found, do not split.
      // The items would have to be processed in any case, so we essentially get this
      // check without any additional performance cost.
      int index = findFirstSplittableItem(node);

      // If no items would benefit from splitting the node, we can stop right here
      if(index == -1) {
        return false;
      }

      // Create child nodes for this node
      createChildNodes(node);

      // Now move all items into the subnodes they belong to
      BoundingRectangle rectangle;
      while(index < node.ItemCount) {
        rectangle = node.Items[index].BoundingRectangle;

        // Locate the node this item should be moved to. If it touches any of the center
        // axes, we cannot move it and it has to stay in this node.
        Node targetNode = null;
        if(rectangle.Max.X < node.Center.X) {
          if(rectangle.Max.Y < node.Center.Y) {
            targetNode = node.TL;
          } else if(rectangle.Min.Y > node.Center.Y) {
            targetNode = node.BL;
          }
        } else if(rectangle.Min.X > node.Center.X) {
          if(rectangle.Max.Y < node.Center.Y) {
            targetNode = node.TR;
          } else if(rectangle.Min.Y > node.Center.Y) {
            targetNode = node.BR;
          }
        }

        // If the item should be moved to a child node, adjust the arrays here.
        // Otherwise, skip it and try the next node.
        if(targetNode != null) {
          //if(targetNode.ItemCount == targetNode.Items.Length) {
          //  increaseNodeCapacity(targetNode);
          //}

          targetNode.Items[targetNode.ItemCount] = node.Items[index];
          ++targetNode.ItemCount;

          node.Items[index] = node.Items[node.ItemCount - 1];
          --node.ItemCount;
          node.Items[node.ItemCount] = default(ItemType); // kill zombie objects
        } else {
          ++index;
        }
      }

      // We just did a split of all possible items, so we know any remaining items
      // are not suitable for splitting
      node.FirstSplittableItemIndex = node.ItemCount;
      return true;

    }

    /// <summary>Creates child nodes for the node</summary>
    /// <param name="node">Node in which child nodes will be created</param>
    private void createChildNodes(Node node) {
      node.TL = new Node(
        new BoundingRectangle(
          node.BoundingRectangle.Min,
          node.Center
        ),
        this.MaxItemsPerNode
      );
      node.TR = new Node(
        new BoundingRectangle(
          node.Center.X, node.BoundingRectangle.Min.Y,
          node.BoundingRectangle.Max.X, node.Center.Y
        ),
        this.MaxItemsPerNode
      );
      node.BL = new Node(
        new BoundingRectangle(
          node.BoundingRectangle.Min.X, node.Center.Y,
          node.Center.X, node.BoundingRectangle.Max.Y
        ),
        this.MaxItemsPerNode
      );
      node.BR = new Node(
        new BoundingRectangle(
          node.Center,
          node.BoundingRectangle.Max
        ),
        this.MaxItemsPerNode
      );
    }

    /// <summary>
    ///   Recursively finds the smallest node that entirely contains the specified rectangle
    /// </summary>
    /// <param name="startNode">Node at which searching will begin</param>
    /// <param name="rectangle">Rectangle that needs to be contained in the node</param>
    /// <returns>
    ///   The smallest child node of the provided start node that can entirely contain
    ///   the specified rectangle
    /// </returns>
    private static Node recursiveFindSmallestContainingNode(
      Node startNode, ref BoundingRectangle rectangle
    ) {

      // If this node doesn't have any children, stop searching
      if(startNode.TL == null) {
        return startNode;
      }

      // We already know that the rectangle is completely contained in the starting
      // node, so we only have to check which side of the center the item is on.
      // If it crosses any of the center axes, it cannot go any lower.
      if(rectangle.Max.X < startNode.Center.X) {
        if(rectangle.Max.Y < startNode.Center.Y) {
          return recursiveFindSmallestContainingNode(startNode.TL, ref rectangle);
        } else if(rectangle.Min.Y >= startNode.Center.Y) {
          return recursiveFindSmallestContainingNode(startNode.BL, ref rectangle);
        }
      } else if(rectangle.Min.X >= startNode.Center.X) {
        if(rectangle.Max.Y < startNode.Center.Y) {
          return recursiveFindSmallestContainingNode(startNode.TR, ref rectangle);
        } else if(rectangle.Min.Y >= startNode.Center.Y) {
          return recursiveFindSmallestContainingNode(startNode.BR, ref rectangle);
        }
      }

      // The node must be crossing either the X or Y axis of this node's center,
      // so it cannot be contained in any of our child nodes
      return startNode;

    }

    /// <summary>Increases the item storage capacity of a node</summary>
    /// <param name="insertNode">
    ///   Node whose item storage capacity will be increased
    /// </param>
    private static void increaseNodeCapacity(Node insertNode) {
      ItemType[] items = new ItemType[insertNode.ItemCount * 2];
      Array.Copy(insertNode.Items, items, insertNode.ItemCount);
      insertNode.Items = items;
    }

  }

} // namespace Nuclex.Game.Space

#endif // ENABLE_QUADTREES
