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
        public readonly short X, Y;

        // TODO GridLocation probably shouldn't care about its size, keep it lean
        // public readonly Single Width, Height; -removed
        public readonly RotatedRectangle Bounds;

        #endregion

        public GridLocation(short X, short Y, Vector2 worldCellSize)
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
            return (this.X * 10 + this.Y);
        }

        public override bool Equals(System.Object o)
        {
            if (o == null || this.GetType() != o.GetType())
                return false;
            return (this.X == ((GridLocation)o).X && this.Y == ((GridLocation)o).Y);
        }

        public static bool operator ==(GridLocation g, GridLocation h)
        {
            if (h.X == g.X && h.Y == g.Y)
                return true;

            return false;
        }

        public static bool operator !=(GridLocation g, GridLocation h)
        {
            if (h.X != g.X || h.Y != g.Y)
                return true;

            return false;
        }
    }

    public class Grid
    {
        private List<IWorldObject> allObjects = new List<IWorldObject>();
        private Dictionary<GridLocation, List<IWorldObject>> grid
                = new Dictionary<GridLocation, List<IWorldObject>>();

        // Default Grid values may be overriden by Constructor
        private Vector2 worldCellSize = new Vector2(50, 50);//World dimensions of each grid cell        
        private Point minGrid = new Point(-8, -8);          //Upperleft-most coord of Grid
        private Point maxGrid = new Point(7, 7);            //Lowerright-most coord of Grid

        // Default Constructor applies a 16x16 Grid
        public Grid(Single worldWidth, Single worldHeight, List<IWorldObject> allObjects)
        {
            this.allObjects = allObjects;
            this.worldCellSize.X = worldWidth / 16;
            this.worldCellSize.Y = worldHeight / 16;

            CutIntoGrid();
        }

        //Accepts user-defined grid size
        public Grid(Single worldWidth, Single worldHeight, Point grid_size, List<IWorldObject> allObjects)
        {
            this.allObjects = allObjects;
            this.worldCellSize.X = worldWidth / grid_size.X;
            this.worldCellSize.Y = worldHeight / grid_size.Y;
            this.minGrid.X = -grid_size.X / 2;
            this.minGrid.Y = -grid_size.Y / 2;
            this.maxGrid.X = (grid_size.X / 2) - 1; //You must substract 1 to get the upperleft
            this.maxGrid.Y = (grid_size.X / 2) - 1; //corner of the lowerright-most grid-cell

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
            //STEP 1. Initialize the Dictionary
            // current grid coords
            short X, Y;

            // start in the upper left corner
            X = (short)(minGrid.X);
            Y = (short)(minGrid.Y);

            // make appropriate number of grid locations
            // initialize Dictionary
            while (X <= maxGrid.X)
            {
                while (Y <= maxGrid.Y)
                {
                    grid.Add(new GridLocation(X, Y, worldCellSize), new List<IWorldObject>());
                    Y++;
                }

                // when finished with one column, reset Y and increment X
                Y = (short)minGrid.Y;
                X++;
            }

            // STEP 2. Associate objects with their GridLocations
            foreach (IWorldObject worldObject in allObjects)
            {
                List<GridLocation> IntersectedGridCells = Intersects(worldObject);
                foreach (GridLocation gridLocation in IntersectedGridCells)
                {
                    grid[gridLocation].Add(worldObject);
                }
            }
            System.Diagnostics.Debug.WriteLine("CutIntoGrid is finished");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns>A list of all <see cref="GridLocation"/>s containing the <see cref="IWorldObject"/></returns>
        public List<GridLocation> Intersects(IWorldObject worldObject)
        {
            List<GridLocation> found = new List<GridLocation>();
            List<GridLocation> missed = new List<GridLocation>();
            UniqueList<GridLocation> toTest = new UniqueList<GridLocation>();
            UniqueList<GridLocation> newToBeTested = new UniqueList<GridLocation>();
            GridLocation hasCenter; //The GridLocation that contains the center of the object



            // STEP 1 - determine the GridLocation of the object's center
            short cellCoordX, cellCoordY;

            // Use integer division to find cell coordinates.
            // Since the grid cell coords are in the upperleft corner
            // 1 must be subtracted from the division if we are in the negative (left or up)
            // direction - effectively rounding up in absolute value.
            if (worldObject.Position.X < 0)
                cellCoordX = (short)(((int)worldObject.Position.X / (int)worldCellSize.X) - 1);
            else
                cellCoordX = (short)(((int)worldObject.Position.X / (int)worldCellSize.X));

            if (worldObject.Position.Y < 0)
                cellCoordY = (short)(((int)worldObject.Position.Y / (int)worldCellSize.Y) - 1);
            else
                cellCoordY = (short)(((int)worldObject.Position.Y / (int)worldCellSize.Y));

            //Check to makes sure these coordinates are in the world
            if (minGrid.X <= cellCoordX && cellCoordX <= maxGrid.X &&
                minGrid.Y <= cellCoordY && cellCoordY <= maxGrid.Y)
            {
                hasCenter = new GridLocation(cellCoordX,
                                             cellCoordY,
                                             worldCellSize);
            }
            else //If they are not in the world return an empty list.
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
                    // If a surrounding GridLocation interects the object do 4 things
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
                    // If a surrounding GridLocation DOES NOT interect the object add it to missed.    
                    else
                    {
                        missed.Add(gridLocation);
                    }

                }
                //Since we have checked everything in toTest flush it
                toTest.Clear();
                //Add the newly found surrounding candidates to toTest
                toTest.UnionWith(newToBeTested);
                newToBeTested.Clear(); //Flush the temp list;
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

            List<GridLocation> surrounding = new List<GridLocation>();

            surrounding.Add(new GridLocation((short)(g.X - 1), (short)(g.Y - 1), worldCellSize));
            surrounding.Add(new GridLocation((short)(g.X - 1), (short)g.Y, worldCellSize));
            surrounding.Add(new GridLocation((short)(g.X - 1), (short)(g.Y + 1), worldCellSize));
            surrounding.Add(new GridLocation((short)g.X, (short)(g.Y - 1), worldCellSize));
            //surrounding.Add(new GridLocation((short)g.X          , (short)g.Y             , worldCellSize)); - the cell itself
            surrounding.Add(new GridLocation((short)g.X, (short)(g.Y + 1), worldCellSize));
            surrounding.Add(new GridLocation((short)(g.X + 1), (short)(g.Y - 1), worldCellSize));
            surrounding.Add(new GridLocation((short)(g.X + 1), (short)g.Y, worldCellSize));
            surrounding.Add(new GridLocation((short)(g.X + 1), (short)(g.Y + 1), worldCellSize));

            // check to make sure the surrounding cells are inside the world
            List<GridLocation> outOfThisworld = new List<GridLocation>();
            foreach (GridLocation s in surrounding)
            {
                if (s.X < minGrid.X || s.X > maxGrid.X ||
                    s.Y < minGrid.Y || s.Y > maxGrid.Y)
                {
                    outOfThisworld.Add(s);
                }
            }
            foreach (GridLocation o in outOfThisworld)
            {
                surrounding.Remove(o);
            }

            return surrounding;
        }
    }
}
