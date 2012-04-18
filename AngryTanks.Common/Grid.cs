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
    struct GridLocation
    {
        #region Properties

        // world unit coords of grid cell
        public readonly UInt16 X, Y;

        // TODO GridLocation probably shouldn't care about its size, keep it lean
        public readonly Single Width, Height;
        public readonly RotatedRectangle Bounds;

        #endregion

        public GridLocation(UInt16 X, UInt16 Y, Single cellWidth, Single cellHeight)
        {
            this.X = X;
            this.Y = Y;
            this.Width = cellWidth;
            this.Height = cellHeight;
            this.Bounds = new RotatedRectangle(X, Y, Width, Height, 0);
        }

        public bool Equals(GridLocation g)
        {
            if (this.X == g.X && this.Y == g.Y)
                return true;

            return false;
        }
    }

    class Grid
    {
        private List<IWorldObject> allObjects = new List<IWorldObject>();
        private Dictionary<GridLocation, List<IWorldObject>> grid
                = new Dictionary<GridLocation, List<IWorldObject>>();

        // TODO store as a RectangleF instead?
        private Vector2 minWorld = new Vector2(-400, -400);
        private Vector2 maxWorld = new Vector2(400, 400);
        private Vector2 cellSize = new Vector2(50, 50);

        // TODO configurable grid size, ie 16x16, 8x8, etc
        public Grid(Single worldHeight, Single worldWidth, List<IWorldObject> allObjects)
        {
            this.allObjects = allObjects;
            this.minWorld.X = -worldWidth  / 2;
            this.minWorld.Y = -worldHeight / 2;
            this.maxWorld.X =  worldWidth  / 2;
            this.maxWorld.Y =  worldHeight / 2;

            CutIntoGrid();
        }

        /// <summary>
        /// Requests the <see cref="Grid"/> to return a list of all <see cref="IWorldObject"/>s
        /// which share a GridLocation with the given object.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public List<IWorldObject> getCollidableObjects(IWorldObject worldObject)
        {
            List<IWorldObject> collidables = new List<IWorldObject>();

            // find all grid locations that contain the object
            foreach (GridLocation gridLocation in Intersects(worldObject))
            {
                // compile a list of all objects contained in the found Grid Locations
                collidables.AddRange(getLocationObjectsOf(gridLocation));
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
            // current grid coords
            UInt16 X, Y;

            // start in the upper left corner
            X = (UInt16)minWorld.X;
            Y = (UInt16)minWorld.Y;

            // make appropriate number of grid locations
            // initialize Dictionary
            while (X + cellSize.X < maxWorld.X)
            {
                while (Y + cellSize.Y < maxWorld.Y)
                {
                    grid.Add(new GridLocation(X, Y, cellSize.X, cellSize.Y), new List<IWorldObject>());
                    Y += (UInt16)cellSize.Y;
                }

                // when finished with one column, reset Y and increment X
                Y = (UInt16)minWorld.Y;
                X += (UInt16)cellSize.X;
            }

            // associate objects with their GridLocations
            foreach (IWorldObject worldObject in allObjects)
            {
                foreach(GridLocation gridLocation in Intersects(worldObject))
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
            List<GridLocation> found = new List<GridLocation>();
            //List<GridLocation> missed = new List<GridLocation>();  - i forgot why we need this
            HashSet<GridLocation> toTest = new HashSet<GridLocation>();

            // determine the GridLocation of the object's center
            GridLocation hasCenter = new GridLocation((UInt16)((worldObject.Position.X / cellSize.X) * cellSize.X),
                                                      (UInt16)((worldObject.Position.Y / cellSize.Y) * cellSize.Y),
                                                      cellSize.X, cellSize.Y);

            // add it to the found list
            found.Add(hasCenter);

            // determine all surrounding GridLocations that contain the object
            toTest.UnionWith(GetSurrounding(hasCenter));

            while (toTest.Count != 0)
            {
                foreach (GridLocation gridLocation in toTest)
                {
                    if (worldObject.Bounds.Intersects(gridLocation.Bounds))
                    {
                        found.Add(gridLocation);
                        toTest.UnionWith(GetSurrounding(gridLocation));
                        toTest.Remove(gridLocation);
                    }
                    else
                    {
                        // missed.Add(g) Why do we need this?
                    }
                }
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

            surrounding.Add(new GridLocation((UInt16)(g.X - g.Width), (UInt16)(g.Y - g.Height), g.Width, g.Height));
            surrounding.Add(new GridLocation((UInt16)(g.X - g.Width), (UInt16)g.Y             , g.Width, g.Height));
            surrounding.Add(new GridLocation((UInt16)(g.X - g.Width), (UInt16)(g.Y + g.Height), g.Width, g.Height));
            surrounding.Add(new GridLocation((UInt16)g.X            , (UInt16)(g.Y - g.Height), g.Width, g.Height));
            //surrounding.Add(new GridLocation((UInt16)g.X          , (UInt16)g.Y             , g.Width, g.Height)); - the cell itself
            surrounding.Add(new GridLocation((UInt16)g.X            , (UInt16)(g.Y + g.Height), g.Width, g.Height));
            surrounding.Add(new GridLocation((UInt16)(g.X + g.Width), (UInt16)(g.Y - g.Height), g.Width, g.Height));
            surrounding.Add(new GridLocation((UInt16)(g.X + g.Width), (UInt16)g.Y             , g.Width, g.Height));
            surrounding.Add(new GridLocation((UInt16)(g.X + g.Width), (UInt16)(g.Y + g.Height), g.Width, g.Height));

            // check to make sure the surrounding cells are inside the world
            foreach (GridLocation s in surrounding)
            {                
                if (s.X < minWorld.X && s.X > maxWorld.X &&
                    s.Y < minWorld.Y && s.Y > maxWorld.Y)
                {
                    surrounding.Remove(s);
                }
            }

            return surrounding;
        }
    }
}
