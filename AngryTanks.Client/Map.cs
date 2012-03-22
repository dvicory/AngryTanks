using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace AngryTanks.Client
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Map : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Map(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        //parseMapFile()//////////////////////////////////////////////////////////////////////
        //(and dummy Box and Pyramid private classes)
        //
        //Takes in a StreamReader object and returns a LinkList of Strings 
        //describing the world, boxes, and pyramids found in the stream.
        //
        //TO DO: 
        //1) Use world data to set this Map class's fields
        //2) replace dummy box and pyramid construtors with the real ones
        //3) Add support for finding rotation values
        //4) Create sub-functions to reduce code duplication in algorithm
        public static LinkedList<String> parseMapFile(StreamReader sr)
        {
            LinkedList<String> map_objects = new LinkedList<String>();
            String line = sr.ReadLine();
            bool no_corruption = true;

            while (!line.Trim().Contains("world"))
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    map_objects.AddLast("No world found");
                    return map_objects;
                }

            }
            map_objects.AddLast("Begin world data: ");

            while (!line.Trim().Contains("name"))
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    map_objects.AddLast("No world name found");
                    return map_objects;
                }

            }
            map_objects.AddLast("World Name: " + line.Trim().Substring(4, line.Length - 4).Trim());

            while (!line.Trim().Contains("size"))
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    map_objects.AddLast("No world size found");
                    return map_objects;
                }

            }
            map_objects.AddLast("World Size: " + line.Trim().Substring(4, line.Length - 4).Trim());

            int pos_x;
            int pos_y;
            int s_y;
            int s_x;

            //map_objects.AddLast("Starting Object search");
            while ((line = sr.ReadLine()) != null) //Reloads line to discard the 'end' statement of the world-block
            {
                //map_objects.AddLast("Top of object-search loop Line has: " + line);
                if (line.Contains("box"))
                {
                    no_corruption = true; //Assume all data is there unless it is found to be missing
                    //map_objects.AddLast("Found a Box line");
                    while (!line.Contains("position") && !line.Contains("size"))
                    {
                        //map_objects.AddLast("Searching for position or size of object");
                        line = sr.ReadLine();
                        if (line.Contains("end"))
                        {
                            map_objects.AddLast("A Box with corrupt or missing data!\n Searching for more objects");
                            no_corruption = false;
                            break;
                        }
                        if (line == null)
                        {
                            map_objects.AddLast("A Box with corrupt or missing data!\n End of File");
                            return map_objects;
                        }
                    }
                    //map_objects.AddLast("Exiting position Or size loop");
                    if (no_corruption)
                    {
                        //map_objects.AddLast("Found position or size line");
                        if (line.Contains("position"))
                        {
                            //map_objects.AddLast("Found a Position line");
                            String[] coords = line.Trim().Substring(9).Split(' ');

                            int.TryParse(coords[0].Trim(), out pos_x);
                            int.TryParse(coords[1].Trim(), out pos_y);

                            while (!line.Contains("size"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    map_objects.AddLast("A Box with corrupt or missing size! \nSearching for more objects");
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    map_objects.AddLast("A Box with corrupt or missing size!\n End of File");
                                    return map_objects;
                                }
                            }
                            if (no_corruption)
                            {
                                String[] coords_s = line.Trim().Substring(5).Split(' ');

                                int.TryParse(coords_s[0].Trim(), out s_x);
                                int.TryParse(coords_s[1].Trim(), out s_y);

                                map_objects.AddLast(new Box(pos_x, pos_y, s_x, s_y).toString());
                            }
                            else { } //If size cannot be found load nothing from this object to map_objects

                        }
                        else if (line.Contains("size"))
                        {
                            String[] coords = line.Trim().Substring(4).Split(' ');

                            int.TryParse(coords[0].Trim(), out s_x);
                            int.TryParse(coords[1].Trim(), out s_y);

                            while (!line.Contains("position"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    map_objects.AddLast("A Box with corrupt or missing position! \nSearching for more objects");
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    map_objects.AddLast("A Box with corrupt or missing position!\n End of File");
                                    return map_objects;
                                }
                            }
                            //map_objects.AddLast("Exiting Box position loop. Line has: " + line);
                            if (no_corruption)
                            {
                                String[] coords_s = line.Trim().Substring(8).Split(' ');

                                int.TryParse(coords_s[0].Trim(), out pos_x);
                                int.TryParse(coords_s[1].Trim(), out pos_y);

                                map_objects.AddLast(new Box(pos_x, pos_y, s_x, s_y).toString());
                            }
                            else { } //If size cannot be found load nothing from this object to map_objects

                        } //map_objects.AddLast("Exiting Box size loop. Line has: " + line);
                    } //map_objects.AddLast("Exiting Box position Or size loop. Line has: " + line);

                    //map_objects.AddLast("Exiting Box if-block. Line has: " + line);
                }
                else if (line.Contains("pyramid"))
                {
                    no_corruption = true; //Assume all data is there unless it is found missing
                    //map_objects.AddLast("Found a Pyramid line");
                    while (!line.Contains("position") && !line.Contains("size"))
                    {
                        //map_objects.AddLast("Searching for position or size of object");
                        line = sr.ReadLine();
                        if (line.Contains("end"))
                        {
                            map_objects.AddLast("A Pyramid with corrupt or missing data!\n Searching for more objects2");
                            no_corruption = false;
                            break;
                        }
                        if (line == null)
                        {
                            map_objects.AddLast("A Pyramid with corrupt or missing data!\n End of File");
                            return map_objects;
                        }
                    }
                    //map_objects.AddLast("Exiting pyramid position Or size loop: " + line);
                    if (no_corruption)
                    {
                        //map_objects.AddLast("Found pyramid position or size line");
                        if (line.Contains("position"))
                        {
                            //map_objects.AddLast("Found a pyramid Position line");
                            String[] coords = line.Trim().Substring(9).Split(' ');

                            int.TryParse(coords[0].Trim(), out pos_x);
                            int.TryParse(coords[1].Trim(), out pos_y);

                            while (!line.Contains("size"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    map_objects.AddLast("A Pyramid with corrupt or missing size!");
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    map_objects.AddLast("A Pyramid with corrupt or missing size!\n End of File");
                                    return map_objects;
                                }
                            }
                            if (no_corruption)
                            {
                                String[] coords_s = line.Trim().Substring(5).Split(' ');

                                int.TryParse(coords_s[0].Trim(), out s_x);
                                int.TryParse(coords_s[1].Trim(), out s_y);

                                map_objects.AddLast(new Pyramid(pos_x, pos_y, s_x, s_y).toString());
                            }
                            else { } //If size cannot be found load nothing from this object to map_objects

                        }
                        else if (line.Contains("size"))
                        {
                            String[] coords = line.Trim().Substring(4).Split(' ');

                            int.TryParse(coords[0].Trim(), out s_x);
                            int.TryParse(coords[1].Trim(), out s_y);

                            while (!line.Contains("position"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    map_objects.AddLast("A Pyramid with corrupt or missing position!");
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    map_objects.AddLast("A Pyramid with corrupt or missing position!\n End of File");
                                    return map_objects;
                                }
                            }
                            if (no_corruption)
                            {
                                String[] coords_s = line.Trim().Substring(8).Split(' ');

                                int.TryParse(coords_s[0].Trim(), out pos_x);
                                int.TryParse(coords_s[1].Trim(), out pos_y);

                                map_objects.AddLast(new Pyramid(pos_x, pos_y, s_x, s_y).toString());
                            }
                            else { } //If size cannot be found, load nothing from this object to map_objects
                        }
                    }

                }


            }
            //map_objects.AddLast("Object searching loop finished");   
            return map_objects;
        }
        private class Box
        {
            private int position_x;
            private int position_y;
            private int size_x;
            private int size_y;

            public Box(int p_x, int p_y, int s_x, int s_y)
            {
                position_x = p_x;
                position_y = p_y;
                size_x = s_x;
                size_y = s_y;
            }
            public String toString()
            {
                return ("A Box with height: " + size_y + "\n"
                     + "            width: " + size_x + "\n"
                     + "       at Y-coord: " + position_y + "\n"
                     + "       at X-coord: " + position_x + "\n");

            }
        }
        private class Pyramid
        {
            private int position_x;
            private int position_y;
            private int size_x;
            private int size_y;

            public Pyramid(int p_x, int p_y, int s_x, int s_y)
            {
                position_x = p_x;
                position_y = p_y;
                size_x = s_x;
                size_y = s_y;
            }
            public String toString()
            {
                return ("A Pyramid with height: " + size_y + "\n"
                     + "                width: " + size_x + "\n"
                     + "           at Y-coord: " + position_y + "\n"
                     + "           at X-coord: " + position_x + "\n");

            }
        }
    }
}