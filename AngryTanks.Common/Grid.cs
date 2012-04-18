using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngryTanks.Common
{
    class Grid
    {
        #region Properties
        #endregion

        private List<IWorldObject> all_objects = new List<IWorldObject>(); 
        private Dictionary<GridLocation, List<IWorldObject>> grid 
                = new Dictionary<GridLocation, List<IWorldObject>>();
         
        private int min_world_x = -400;
        private int max_world_x =  400;
        private int min_world_y = -400;
        private int max_world_y =  400;
        private int cell_width =  50;
        private int cell_height = 50;

        public Grid(int world_height, int world_width, List<IWorldObject> all_objects)
        {
            this.all_objects = all_objects;
            this.min_world_y = -world_height / 2;
            this.min_world_x = -world_width / 2;
            this.max_world_y =  world_height / 2;
            this.max_world_x =  world_width / 2;
            cutIntoGrid();
            
        }
        public Grid(int world_height,
                    int world_width,
                    List<IWorldObject> all_objects,
                    int cell_height, 
                    int cell_width)
        {
            this.all_objects = all_objects;
            this.min_world_y = -world_height / 2;
            this.min_world_x = -world_width / 2;
            this.max_world_y =  world_height / 2;
            this.max_world_x =  world_width / 2;
            this.cell_height = cell_height;
            this.cell_width = cell_width;
            cutIntoGrid();
        }
        /* getCollidableObjects()
         * 
         * Requests the Grid to return a list of all objects which share a
         * GridLocation with the given object.
         * 
         */
        public List<IWorldObject> getCollidableObjects(IWorldObject w)
        {
            List<IWorldObject> collidables = new List<IWorldObject>();

            foreach (GridLocation g in Intersects(w)) //Find all Grid Locations that contain the object
            {
                //Compile a list of all objects contained in the found Grid Locations
                collidables.AddRange(getLocationObjectsOf(g)); 
            }
            return collidables;
        }

        /* getLocationObjectOf()
         * 
         * returns the objects associated with a given Grid Location
         * 
         */
        public List<IWorldObjects> getLocationObjectsOf(GridLocation g)
        {
            return grid[g];
        }

        /* cutIntoGrid()
         * 
         * Cuts the world up into GridLocations and associates objects with cells.
         */
        private void cutIntoGrid()
        {
            int X, Y; //Current grid coords

            //Start in the upper left corner
            X = min_world_x;
            Y = min_world_y;

            //Make appropriate number of grid locations
            //initialize Dictionary
            while (X + cell_width < max_world_x)
            {
                while (Y + cell_height < max_world_y)
                {
                    grid.Add(new GridLocation(X, Y, cell_width, cell_height), new List<IWorldObject>());
                    Y += cell_height;
                }
                //when finished with one column, reset Y and increment X
                Y = min_world_y;
                X += cell_width;
            }

            //Associate objects with their GridLocations
            foreach (IWorldObject o in all_objects)
            {
                foreach(GridLocation g in Intersects(o))
                {
                    grid[g].Add(o);
                }
            }


        }

        /* Intersects()
         * 
         * Returns a list of all grid loactions containing the IWorldObject 
         */
        public List<GridLocation> Intersects(IWorldObject o)
        {
            List<GridLocation> found = new List<GridLocation>();
            //List<GridLocation> missed = new List<GridLocation>();  - i forgot why we need this
            HashSet<GridLocation> to_be_tested = new HashSet<GridLocation>();

            //Determine the Grid Location of the object's center
            GridLocation has_center = new GridLocation((o.Position.X / cell_width) * cell_width,
                                                       (o.Position.Y / cell_height)* cell_height,
                                                                      cell_width, 
                                                                      cell_height);
            found.Add(has_center);//Add it to the found list.

            //Determine all surrounding GridLocation that contain the object
            to_be_tested.UnionWith(getSurrounding(has_center));

            while (to_be_tested.Count != 0)
            {
                foreach (GridLocation g in to_be_tested)
                {
                    //Not sure what to do with these variables sent back from Intersects()
                    Single overlap;
                    Vector2 collisionProjection;

                    if (o.RectangleBounds.Intersects(g.RectangleBounds, out overlap, out collisionProjection))
                    {
                        found.Add(g);
                        to_be_tested.UnionWith(getSurrounding(g));
                        to_be_tested.Remove(g);
                    }
                    else
                    {
                        //missed.Add(g) Why do we need this?
                    }
                }
            }
            return found;
        }

        /* getSurrounding()
         * 
         * Returns a list of the surrounding Grid Locations so long as they
         * exist in the world.
         */
        private List<GridLocation> getSurrounding(GridLocation g)
        {
            List<GridLocation> surrounding = new List<GridLocation>();

            surrounding.Add(new GridLocation(g.X - g.width, g.Y - g.height, g.width, g.height));
            surrounding.Add(new GridLocation(g.X - g.width, g.Y           , g.width, g.height));
            surrounding.Add(new GridLocation(g.X - g.width, g.Y + g.height, g.width, g.height));
            surrounding.Add(new GridLocation(g.X          , g.Y - g.height, g.width, g.height));
          //surrounding.Add(new GridLocation(g.X          , g.Y           , g.width, g.height)); - the cell itself
            surrounding.Add(new GridLocation(g.X          , g.Y + g.height, g.width, g.height));
            surrounding.Add(new GridLocation(g.X + g.width, g.Y - g.height, g.width, g.height));
            surrounding.Add(new GridLocation(g.X + g.width, g.Y           , g.width, g.height));
            surrounding.Add(new GridLocation(g.X + g.width, g.Y + g.height, g.width, g.height));

            //Check to make sure the surrounding cells are inside the world
            foreach (GridLocation s in surrounding)
            {                
                if (s.X < min_world_x && s.X > max_world_x &&
                    s.Y < min_world_y && s.Y > max_world_y)
                {
                    surrounding.Remove(s);
                }
            }

            return surrounding;
        }       
        
    }
    
    
    /* struct GridLocation
     * 
     * This structure represents one Grid block whose position coords
     * are measured from the center of the world in world units. 
     */
    private struct GridLocation
    {
        #region Properties

        //World unit coords of grid cell
        public readonly int X, Y, width, height;
        public readonly RotatedRectangle RectangleBounds;

        #endregion

        public GridLocation(int X, int Y, int cell_width, int cell_height)
        {
            this.X = X;
            this.Y = Y;
            width = cell_width;
            height = cell_height;
            RectangleBounds = new RotatedRectangle((float)X, (float)Y, (float)width, (float)height, 0.0f);    
        }

        public bool Equals(GridLocation g)
        {
            if(this.X == g.X && this.Y == g.Y)
                return true;
            return false;

        }

    }
}

