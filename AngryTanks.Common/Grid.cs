using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngryTanks.Common
{
    /// <summary>
    /// Structure that represents one grid block whose position coordinates
    /// are measured from the center of the world (in world units).
    /// </summary>
    public struct GridLocation
    {
        #region Properties

        // world unit coords of grid cell
        public readonly Int16 X, Y;

        // bounds of the grid cell
        public readonly RotatedRectangle Bounds;

        #endregion

        public GridLocation(Int16 X, Int16 Y, Vector2 worldCellSize)
        {
            this.X = X;
            this.Y = Y;
            this.Bounds = new RotatedRectangle(X * worldCellSize.X,
                                               Y * worldCellSize.Y,
                                                   worldCellSize.X,
                                                   worldCellSize.Y, 0);
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }

        public static bool operator ==(GridLocation a, GridLocation b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(GridLocation a, GridLocation b)
        {
            return !(a == b);
        }

        public override bool Equals(Object o)
        {
            if (o == null || this.GetType() != o.GetType())
                return false;

            return this == (GridLocation)o;
        }

        public bool Equals(GridLocation o)
        {
            if (o == null)
                return false;

            return this == o;
        }
    }

    public class Grid
    {
        private List<IWorldObject> allObjects = new List<IWorldObject>();

        public List<IWorldObject> AllObjects
        {
            get { return allObjects; }
        }

        private Dictionary<GridLocation, List<IWorldObject>> grid = new Dictionary<GridLocation, List<IWorldObject>>();

        // grid characteristics
        private Vector2 cellSize; // world dimensions of each grid cell        
        private Point gridSize;   // X-by-Y dimensions of grid
        private Point minGrid;    // upper left most coord of Grid
        private Point maxGrid;    // lower right most coord of Grid

        /// <summary>
        /// Constructs a default 16x16 <see cref="Grid"/>.
        /// </summary>
        /// <param name="worldSize"></param>
        /// <param name="allObjects"></param>
        public Grid(Vector2 worldSize, List<IWorldObject> allObjects)
            : this(worldSize, new Point(16, 16), allObjects)
        { }

        /// <summary>
        /// Constructs a user-defined size <see cref="Grid"/>.
        /// </summary>
        /// <param name="worldSize"></param>
        /// <param name="gridSize"></param>
        /// <param name="allObjects"></param>
        public Grid(Vector2 worldSize, Point gridSize, List<IWorldObject> allObjects)
        {
            this.allObjects = allObjects;

            this.cellSize.X = worldSize.X / gridSize.X;
            this.cellSize.Y = worldSize.Y / gridSize.Y;

            this.gridSize = gridSize;

            this.minGrid.X = -gridSize.X / 2;
            this.minGrid.Y = -gridSize.Y / 2;
            this.maxGrid.X = (gridSize.X / 2) - 1; // you must substract 1 to get the upper left
            this.maxGrid.Y = (gridSize.X / 2) - 1; // corner of the lower right-most grid cell

            CutIntoGrid();
        }

        /// <summary>
        /// Requests the <see cref="Grid"/> to return a list of all <see cref="IWorldObject"/>s
        /// which share a GridLocation with the given object.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public List<IWorldObject> PotentialIntersects(IWorldObject worldObject)
        {
            UniqueList<IWorldObject> collidables = new UniqueList<IWorldObject>();

            // find all grid locations that contain the object
            List<GridLocation> IntersectedGridCells = Intersects(worldObject);
            foreach (GridLocation gridLocation in IntersectedGridCells)
            {
                // compile a list of all objects contained in the found Grid Locations
                collidables.UnionWith(getLocationObjectsOf(gridLocation));
            }

            return collidables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridLocation"></param>
        /// <returns><see cref="IWorldObject"/>s associated with a given <paramref name="gridLocation"/></returns>
        public List<IWorldObject> getLocationObjectsOf(GridLocation gridLocation)
        {
            return grid[gridLocation];
        }

        /// <summary>
        /// Cuts the world up into <see cref="GridLocation"/>s and associates
        /// <see cref="IWorldObject"/>s with cells.
        /// </summary>
        private void CutIntoGrid()
        {
            // STEP 1. Initialize the Dictionary
            // current grid coords
            Int16 X, Y;

            // start in the upper left corner
            X = (Int16)minGrid.X;
            Y = (Int16)minGrid.Y;

            // make appropriate number of grid locations, filling the dictionary
            while (X <= maxGrid.X)
            {
                while (Y <= maxGrid.Y)
                {
                    grid.Add(new GridLocation(X, Y, cellSize), new List<IWorldObject>());
                    Y++;
                }

                // when finished with one column, reset Y and increment X
                Y = (Int16)minGrid.Y;
                X++;
            }

            // STEP 2. Associate objects with their GridLocations
            foreach (IWorldObject worldObject in allObjects)
            {
                List<GridLocation> intersectedGridCells = Intersects(worldObject);
                foreach (GridLocation gridLocation in intersectedGridCells)
                {
                    grid[gridLocation].Add(worldObject);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns>A list of all <see cref="GridLocation"/>s containing the <see cref="IWorldObject"/></returns>
        public List<GridLocation> Intersects(IWorldObject worldObject)
        {
            List<GridLocation> found = new List<GridLocation>(gridSize.X * gridSize.Y);
            List<GridLocation> missed = new List<GridLocation>(gridSize.X * gridSize.Y);
            UniqueList<GridLocation> toTest = new UniqueList<GridLocation>(gridSize.X * gridSize.Y);
            UniqueList<GridLocation> newToBeTested = new UniqueList<GridLocation>(gridSize.X * gridSize.Y);

            // the GridLocation that contains the center of the object
            GridLocation hasCenter;

            // STEP 1 - determine the GridLocation of the object's center
            Point cellCoord;

            // Use integer division to find cell coordinates.
            // Since the grid cell coords are in the upperleft corner
            // 1 must be subtracted from the division if we are in the negative (left or up)
            // direction - effectively rounding up in absolute value.
            if (worldObject.Position.X < 0)
                cellCoord.X = (Int16)(((Int16)worldObject.Position.X / (Int16)cellSize.X) - 1);
            else
                cellCoord.X = (Int16)(((Int16)worldObject.Position.X / (Int16)cellSize.X));

            if (worldObject.Position.Y < 0)
                cellCoord.Y = (Int16)(((Int16)worldObject.Position.Y / (Int16)cellSize.Y) - 1);
            else
                cellCoord.Y = (Int16)(((Int16)worldObject.Position.Y / (Int16)cellSize.Y));

            // check to makes sure these coordinates are in the world
            if (minGrid.X <= cellCoord.X && cellCoord.X <= maxGrid.X &&
                minGrid.Y <= cellCoord.Y && cellCoord.Y <= maxGrid.Y)
            {
                hasCenter = new GridLocation((Int16)cellCoord.X,
                                             (Int16)cellCoord.Y,
                                             cellSize);
            }
            // if they are not in the world, return the empty list
            else
            {
                return found;
            }

            // add it to the found list
            found.Add(hasCenter);

            // STEP 2 - determine all surrounding GridLocations that contain the object
            toTest.UnionWith(GetSurrounding(hasCenter));

            while (toTest.Count != 0)
            {
                foreach (GridLocation gridLocation in toTest)
                {
                    // if a surrounding GridLocation intersects the object do 4 things
                    if (worldObject.Bounds.Intersects(gridLocation.Bounds))
                    {
                        // 1. Add it to the found list
                        found.Add(gridLocation);

                        // 2. Get its surrounding GridLocations
                        newToBeTested.UnionWith(GetSurrounding(gridLocation));

                        // 3. Remove any that are already known to not contain the object
                        foreach (GridLocation g in missed)
                        {
                            newToBeTested.Remove(g);
                        }

                        // 4. Remove any that are already known to contain the object
                        foreach (GridLocation g in found)
                        {
                            newToBeTested.Remove(g);
                        }
                    }
                    // if a surrounding GridLocation DOES NOT intersect the object add it to missed
                    else
                    {
                        missed.Add(gridLocation);
                    }
                }

                // since we have checked everything in toTest flush it
                toTest.Clear();

                // add the newly found surrounding candidates to toTest
                toTest.UnionWith(newToBeTested);

                // flush the temp list
                newToBeTested.Clear();
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridLocation"></param>
        /// <returns>
        /// A list of the <see cref="GridLocation"/>s surrounding
        /// <paramref name="gridLocation"/> so long as they exist in the world.
        /// </returns>
        private List<GridLocation> GetSurrounding(GridLocation gridLocation)
        {
            GridLocation g = gridLocation;

            List<GridLocation> surrounding = new List<GridLocation>(8);

            surrounding.Add(new GridLocation((Int16)(g.X - 1), (Int16)(g.Y - 1), cellSize));
            surrounding.Add(new GridLocation((Int16)(g.X - 1), (Int16)g.Y,       cellSize));
            surrounding.Add(new GridLocation((Int16)(g.X - 1), (Int16)(g.Y + 1), cellSize));
            surrounding.Add(new GridLocation((Int16)g.X,       (Int16)(g.Y - 1), cellSize));
            //surrounding.Add(new GridLocation((Int16)g.X,     (Int16)g.Y,       cellSize)); - the cell itself
            surrounding.Add(new GridLocation((Int16)g.X,       (Int16)(g.Y + 1), cellSize));
            surrounding.Add(new GridLocation((Int16)(g.X + 1), (Int16)(g.Y - 1), cellSize));
            surrounding.Add(new GridLocation((Int16)(g.X + 1), (Int16)g.Y,       cellSize));
            surrounding.Add(new GridLocation((Int16)(g.X + 1), (Int16)(g.Y + 1), cellSize));

            // check to make sure the surrounding cells are inside the world, remove any that don't
            surrounding.RemoveAll(
                s => s.X < minGrid.X || s.X > maxGrid.X || s.Y < minGrid.Y || s.Y > maxGrid.Y
            );

            return surrounding;
        }
    }
}
