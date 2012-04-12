#if ENABLE_RTREES

using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Game.Space {

  /// <summary>Variants of R-Tree behaviors this implementation can assume</summary>
  public enum RTreeVariants {

    /// <summary>
    ///   Insertions and deletions take linear time at the cost of degrading the
    ///   tree's overall performance.
    /// </summary>
    /// <remarks>
    ///   Finds the two bounding boxes with the greatest normalized separation
    ///   along both axes, and split along this axis. The remaining bounding boxes
    ///   in the node are assigned to the nodes whose covering bounding box is
    ///   increased the least by the addition [Gutt84]. This method takes linear time.
    /// </remarks>
    Linear,

    /// <summary>
    ///   Insertions and deletions take quadratic time while keeping the tree's
    ///   overall performance at a reasonable level.
    /// </summary>
    /// <remarks>
    ///   Examines all the children of the overflowing node and find the pair of
    ///   bounding boxes that would waste the most area were they to be inserted
    ///   in the same node. This is determined by subtracting the sum of the areas
    ///   of the two bounding boxes from the area of the covering bounding box.
    ///   These two bounding boxes are placed in separate nodes, say j and k.
    ///   The set of remaining bounding boxes are examined and the bounding box i
    ///   whose addition maximizes the difference in coverage between the bounding
    ///   boxes associated with j and k is added to the node whose coverage
    ///   is minimized by the addition. This process is reapplied to the
    ///   remaining bounding boxes [Gutt84]. This method takes quadratic time.
    /// </remarks>
    Quadratic,

    /// <summary>
    ///   Insertions and deletions vary in performance but the tree's overall
    ///   performance is kept high.
    /// </summary>
    /// <remarks>
    ///   The R*-tree [Beck90c] is a name given to a variant of the R-tree which
    ///   makes use of the most complex of the node splitting algorithms. The
    ///   algorithm differs from the other algorithms as it attempts to reduce
    ///   both overlap and coverage. In particular, the primary focus is on
    ///   reducing overlap with ties broken by favoring the splits that reduce
    ///   the coverage by using the splits that minimize the perimeter of the
    ///   bounding boxes of the resulting nodes. In addition, when a node 'a'
    ///   overflows, instead of immediately splitting 'a', an attempt is made
    ///   first to see if some of the objects in 'a' could possibly be more suited
    ///   to being in another node. This is achieved by reinserting a fraction
    ///   (30% has been found to yield good performance [Beck90c]) of these
    ///   objects in the tree (termed 'forced reinsertion'). The node is only split
    ///   if it has been found to overflow after reinsertion has taken place.
    ///   This method is quite complex.
    /// </remarks>
    RStar

  }

} // namespace Nuclex.Game.Space

#endif // ENABLE_RTREES
