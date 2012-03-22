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
        //3) DONE Add support for finding rotation values
        //4) DONE Create sub-functions to reduce code duplication in algorithm
        private static LinkedList<StaticMapObject> parseMapFile(StreamReader sr)
        {
            LinkedList<StaticMapObject> map_objects = new LinkedList<StaticMapObject>();
            String line = sr.ReadLine();
            bool inBlock = false;
            Vector2 position = Vector2.Zero;
            Vector2 size = Vector2.Zero;
            float rotation = 0;
            String type = "";

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

            //This is not a critical step
            line = sr.ReadLine();//Flush the 'end' line of the world block (for debugging) 
            
            bool got_position = false;
            bool got_size = false;            
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Equals("box") || line.Equals("pyramid"))
                {
                    inBlock = true;
                    type = line.Split(' ')[0];
                }

                if (inBlock)
                {
                    if (line.Contains("position"))
                    {
                        position = new Vector2(0, 0);
                        String[] coords = line.Trim().Substring(9).Split(' ');
                        position.X = (float)Convert.ToDecimal(coords[0].Trim());
                        position.Y = (float)Convert.ToDecimal(coords[1].Trim());
                        got_position = true;
                    }
                    if (line.Contains("size"))
                    {
                        size = new Vector2(0, 0);
                        String[] coords = line.Trim().Substring(5).Split(' ');
                        size.X = (float)Convert.ToDecimal(coords[0].Trim());
                        size.Y = (float)Convert.ToDecimal(coords[1].Trim());
                        got_size = true;
                    }
                    if (line.Contains("rotation"))
                    {
                        String[] coords = line.Trim().Substring(9).Split(' ');
                        rotation = (float)Convert.ToDecimal(coords[0].Trim());
                    }
                }
                if (line.Equals("end"))
                {
                    inBlock = false;
                    if (got_position && got_size)
                    {
                        if(type.Equals("box"))
                            map_objects.AddLast(new StaticMapObject(box, rotation, position, size, 0, 60, type));
                        if (type.Equals("pyramid"))
                            map_objects.AddLast(new StaticMapObject(pyramid, rotation, position, size, 0, 60, type));
                    }
                    else
                    {
                        bad_objects++;
                    }
                    got_position = false;
                    got_size = false;
                    rotation = 0;
                    
                }

            }
            return map_objects;
        }
    }
}