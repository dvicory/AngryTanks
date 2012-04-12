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

  // Flaws:
  //   - Unsplittable nodes are not correctly identified. If ten nodes are outside
  //     of the tree's corners, these are counted as unsplittable. Add more items
  //     to the same quadrant, and it will be split. However, the firstSplittableItem
  //     will point past the items in the corner, causing items outside of the tree
  //     to end up in an inner (non-border) node
  //   - Far too complicated to remain efficient

  /// <summary>Tree that breaks down space into quadrants</summary>
  /// <typeparam name="ItemType">Type of the items being indexed by the tree</typeparam>
  public partial class QuadTree<ItemType> : SpatialIndex2<ItemType>
    where ItemType : IBoundingRectangleProvider {

    #region class Node

    /// <summary>A node in a quad-tree</summary>
    private class Node {

      /// <summary>Initializes the quad-tree node</summary>
      /// <param name="boundingRectangle">Bounding rectangle of the node</param>
      /// <param name="capacity">Item capacity for the node</param>
      public Node(BoundingRectangle boundingRectangle, int capacity) {
        this.BoundingRectangle = boundingRectangle;
        this.Center = new Vector2(
          (boundingRectangle.Min.X + boundingRectangle.Max.X) / 2.0f,
          (boundingRectangle.Min.Y + boundingRectangle.Max.Y) / 2.0f
        );
        this.Items = new ItemType[capacity];
      }

      /// <summary>Bounding rectangle of this node</summary>
      public BoundingRectangle BoundingRectangle;

      /// <summary>The center point of the node</summary>
      public Vector2 Center;

      /// <summary>The top-left child of this node</summary>
      public Node TL;
      /// <summary>The top-right child of this node</summary>
      public Node TR;
      /// <summary>The bottom-left child of this node</summary>
      public Node BL;
      /// <summary>The bottom-right child of this node</summary>
      public Node BR;

      /// <summary>Items contained in this node</summary>
      public ItemType[] Items;
      /// <summary>Number of items stored in the item array</summary>
      public int ItemCount;
      /// <summary>Index of the first item that is suitable for splitting</summary>
      public int FirstSplittableItemIndex;

    }

    #endregion // class Node

    /// <summary>Initializes a new quadtree</summary>
    /// <param name="bounds">
    ///   Limits of the quad-tree. Setting this too small will make it impossible
    ///   for the tree to work.
    /// </param>
    public QuadTree(BoundingRectangle bounds) :
      this(bounds, EqualityComparer<ItemType>.Default) { }

    /// <summary>Initializes a new quadtree</summary>
    /// <param name="bounds">
    ///   Limits of the quad-tree. Setting this too small will make it impossible
    ///   for the tree to work.
    /// </param>
    /// <param name="comparer">
    ///   Comparer that will be used to recognize items when they're accessed
    /// </param>
    public QuadTree(BoundingRectangle bounds, IEqualityComparer<ItemType> comparer) {
      this.MinItemsPerNode = 32;
      this.MaxItemsPerNode = 64;

      this.root = new Node(bounds, this.MaxItemsPerNode);
      this.comparer = comparer;
    }

    /// <summary>Maximum number of items per node</summary>
    public int MaxItemsPerNode;
    /// <summary>Minimum number of items per node after collapsing to collapse</summary>
    public int MinItemsPerNode;

    /// <summary>The root node of the quad-tree</summary>
    private Node root;
    /// <summary>Used to compare items to each other</summary>
    private IEqualityComparer<ItemType> comparer;

  }

} // namespace Nuclex.Game.Space

#endif // ENABLE_QUADTREES
