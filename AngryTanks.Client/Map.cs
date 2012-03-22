using System;
using System.IO;
using System.Collections.Generic;
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
    /// This represents the map.
    /// 
    /// (Constructor)
    /// It uses the game's content manager to load the textures needed for the static map objects
    /// and to tile a background.
    /// It reads in a mapfile and constructs a collection "static_layer" of StaticMapObjects.
    /// 
    /// (Initialize
    /// It creates its own spritebatch.
    /// 
    /// (Draw)
    /// It calls the Draw method on each of the StaticMapObjects in the "static_layer".
    /// </summary>
    public class Map : Microsoft.Xna.Framework.GameComponent
    {
        static String world_name;               //Not used yet
        static float world_size;                //Not used yet
        private static bool initialized = false;//Debugging
        private static SpriteBatch spritebatch;//Used to Draw StaticMapObjects

        private static Texture2D pyramid;   //Used to construct StaticMapObjects
        private static Texture2D box;       //Used to construct StaticMapObjects
        private static Texture2D background;//Used to tile a background (Not happening yet)
        LinkedList<StaticMapObject> static_layer = new LinkedList<StaticMapObject>();

        public Map(Game game, StreamReader mapfile)
            : base(game)
        {
            //Load all the needed textures
            pyramid = game.Content.Load<Texture2D>("textures/bz/pyramid");
            box = game.Content.Load<Texture2D>("textures/bz/caution");
            background = game.Content.Load<Texture2D>("textures/bz/std_ground");

            //Construct the StaticMapObjects from the mapfile
            static_layer = parseMapFile(mapfile);
        }

        //Return the status of the Map
        public bool isInitialized()
        {
            return initialized;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            spritebatch = new SpriteBatch(Game.GraphicsDevice);
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
        /// This is called when the Map should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Draw(GameTime gameTime)
        {
            spritebatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            //1)Draw world background

            //2)Draw static layer
            foreach (StaticMapObject mo in static_layer)
            {
                mo.Draw(gameTime, spritebatch);
            }

            //3)Draw mobile layer??

            spritebatch.End();
        }

        //parseMapFile()//////////////////////////////////////////////////////////////////////
        //
        //Takes in a StreamReader object and returns a LinkList of Strings 
        //describing the world, boxes, and pyramids found in the stream.
        //
        //TO DO: 
        //1) DONE Use world data to set this Map class's fields
        //2) DONE replace dummy box and pyramid construtors with the real ones
        //3) Add support for finding rotation values
        //4) Create sub-functions to reduce code duplication in algorithm
        private static LinkedList<StaticMapObject> parseMapFile(StreamReader sr)
        {
            LinkedList<StaticMapObject> map_objects = new LinkedList<StaticMapObject>();
            String line = sr.ReadLine();
            bool no_corruption = true;
            int bad_objects = 0;


            while (!line.Trim().Contains("name"))
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    initialized = false;
                    return map_objects;
                }

            }
            world_name = line.Trim().Substring(4, line.Length - 4).Trim();

            while (!line.Trim().Contains("size"))
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    initialized = false;
                    return map_objects;
                }

            }
            world_size = (float)Convert.ToDecimal(line.Trim().Substring(4, line.Length - 4).Trim());

            Vector2 position;
            Vector2 size;


            while ((line = sr.ReadLine()) != null)
            {

                if (line.Contains("box"))
                {
                    no_corruption = true; //Assume all data is there unless it is found to be missing

                    while (!line.Contains("position") && !line.Contains("size"))
                    {

                        line = sr.ReadLine();
                        if (line.Contains("end"))
                        {
                            bad_objects++;
                            no_corruption = false;
                            break;
                        }
                        if (line == null)
                        {
                            bad_objects++;
                            return map_objects;
                        }
                    }

                    if (no_corruption)
                    {

                        if (line.Contains("position"))
                        {

                            String[] coords_p = line.Trim().Substring(9).Split(' ');

                            position.X = (float)Convert.ToDecimal(coords_p[0].Trim());
                            position.Y = (float)Convert.ToDecimal(coords_p[1].Trim());

                            while (!line.Contains("size"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    bad_objects++;
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    bad_objects++;
                                    return map_objects;
                                }
                            }
                            if (no_corruption)
                            {
                                String[] coords_s = line.Trim().Substring(5).Split(' ');

                                size.X = (float)Convert.ToDecimal(coords_s[0].Trim());
                                size.Y = (float)Convert.ToDecimal(coords_s[1].Trim());

                                map_objects.AddLast(new StaticMapObject(box,
                                                                        0.0f,
                                                                        position,
                                                                        size,
                                                                        5,
                                                                        60,
                                                                        "Box"));
                            }
                            else { } //If size cannot be found load nothing from this object to map_objects

                        }
                        else if (line.Contains("size"))
                        {
                            String[] coord_s = line.Trim().Substring(5).Split(' ');

                            size.X = (float)Convert.ToDecimal(coord_s[0].Trim());
                            size.Y = (float)Convert.ToDecimal(coord_s[1].Trim());

                            while (!line.Contains("position"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    bad_objects++;
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    bad_objects++;
                                    return map_objects;
                                }
                            }
                            //map_objects.AddLast("Exiting Box position loop. Line has: " + line);
                            if (no_corruption)
                            {
                                String[] coords_s = line.Trim().Substring(9).Split(' ');

                                position.X = (float)Convert.ToDecimal(coord_s[0].Trim());
                                position.Y = (float)Convert.ToDecimal(coord_s[1].Trim());

                                map_objects.AddLast(new StaticMapObject(box,
                                                                        0.0f,
                                                                        position,
                                                                        size,
                                                                        5,
                                                                        60,
                                                                        "Box"));
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
                            bad_objects++;
                            no_corruption = false;
                            break;
                        }
                        if (line == null)
                        {
                            bad_objects++;
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

                            position.X = (float)Convert.ToDecimal(coords[0].Trim());
                            position.Y = (float)Convert.ToDecimal(coords[1].Trim());

                            while (!line.Contains("size"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    bad_objects++;
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    bad_objects++;
                                    return map_objects;
                                }
                            }
                            if (no_corruption)
                            {
                                String[] coord_s = line.Trim().Substring(5).Split(' ');

                                size.X = (float)Convert.ToDecimal(coord_s[0].Trim());
                                size.Y = (float)Convert.ToDecimal(coord_s[1].Trim());

                                map_objects.AddLast(new StaticMapObject(pyramid,
                                                                        0.0f,
                                                                        position,
                                                                        size,
                                                                        5,
                                                                        60,
                                                                        "Pyramid"));
                            }
                            else { } //If size cannot be found load nothing from this object to map_objects

                        }
                        else if (line.Contains("size"))
                        {
                            String[] coords_s = line.Trim().Substring(5).Split(' ');

                            size.X = (float)Convert.ToDecimal(coords_s[0].Trim());
                            size.Y = (float)Convert.ToDecimal(coords_s[1].Trim());

                            while (!line.Contains("position"))
                            {
                                line = sr.ReadLine();
                                if (line.Contains("end"))
                                {
                                    bad_objects++;
                                    no_corruption = false;
                                    break;
                                }
                                if (line == null)
                                {
                                    bad_objects++;
                                    return map_objects;
                                }
                            }
                            if (no_corruption)
                            {
                                String[] coordss_s = line.Trim().Substring(9).Split(' ');

                                position.X = (float)Convert.ToDecimal(coordss_s[0].Trim());
                                position.Y = (float)Convert.ToDecimal(coordss_s[1].Trim());

                                map_objects.AddLast(new StaticMapObject(pyramid,
                                                                        0.0f,
                                                                        position,
                                                                        size,
                                                                        5,
                                                                        60,
                                                                        "Pyramid"));
                            }
                            else { } //If size cannot be found, load nothing from this object to map_objects
                        }
                    }

                }


            }
            //map_objects.AddLast("Object searching loop finished");   
            return map_objects;
        }
    }
}