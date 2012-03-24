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

using log4net;

namespace AngryTanks.Client
{
    // TODO inherit from Sprite
    public class Map
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO fix this hack
        private static Game Game;
        private static GraphicsDevice GraphicsDevice;

        static String world_name;               //Not used yet
        static int world_size;                //Not used yet
        private static bool initialized = false;//Debugging
        private SpriteBatch spriteBatch;//Used to Draw StaticMapObjects

        private static Texture2D pyramid;   //Used to construct StaticMapObjects
        private static Texture2D box;       //Used to construct StaticMapObjects
        private static Texture2D background;//Used to tile a background (Not happening yet)
        List<StaticMapObject> static_layer = new List<StaticMapObject>();

        //Return the status of the Map
        public bool isInitialized()
        {
            return initialized;
        }

        public Map(Game game)
        {
            // dirty dirty hack
            Game = game;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            // dirty dirty hack
            GraphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public void LoadContent(ContentManager Content)
        {
            // load all the needed textures
            pyramid = Content.Load<Texture2D>("textures/bz/pyramid");
            box = Content.Load<Texture2D>("textures/bz/box");
            background = Content.Load<Texture2D>("textures/bz/std_ground");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
        }

        /// <summary>
        /// This is called when the Map should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            // TODO use matrix transformation to have a camera and handle proper scaling

            // 1) Draw world background
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            // trick to tile texture
            GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            spriteBatch.Draw(background,
                             new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height),
                             new Rectangle(0, 0, background.Width, background.Height),
                             Color.White,
                             0,
                             Vector2.Zero,
                             SpriteEffects.None,
                             1f);

            spriteBatch.End();

            // 2) Draw static layer
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            foreach (StaticMapObject mo in static_layer)
            {
                mo.Draw(gameTime, spriteBatch);
            }

            // 3) Draw mobile layer??

            spriteBatch.End();
        }

        public void LoadMap(StreamReader sr)
        {
            // construct the StaticMapObjects from the stream
            static_layer = parseMapFile(sr);
        }

        /* parseMapFile()
         * 
         * Takes in a StreamReader object and returns a LinkList of Strings
         * describing the world, boxes, and pyramids found in the stream. 
         * 
         * IF NO WORLD DATA is found, it sets default values.
         */
        private static List<StaticMapObject> parseMapFile(StreamReader sr)
        {
            List<StaticMapObject> map_objects = new List<StaticMapObject>();
            String line;
            bool inWorldBlock = false;
            bool inBlock = false;
            Vector2 position = Vector2.Zero;
            Vector2 size = Vector2.Zero;
            float rotation = 0;
            String type = ""; // Identifier to indicate which texture to construct
            int bad_objects = 0; // Counts object blocks that failed to load                              
            bool got_position = false;
            bool got_size = false;

            //Default values will be overidden if found in the file
            world_name = "No Name";
            world_size = 800;

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Equals("world"))
                {
                    inWorldBlock = true;
                    inBlock = false;
                }
                if (line.Equals("box") || line.Equals("pyramid"))
                {
                    inWorldBlock = false;
                    inBlock = true;
                    type = line.Split(' ')[0];
                }
                if (inWorldBlock)
                {
                    if (line.Contains("name"))
                    {
                        world_name = line.Trim().Substring(4, line.Length - 4).Trim();
                    }
                    if (line.Contains("size"))
                    {
                        world_size = (int)Convert.ToDecimal(line.Trim().Substring(4, line.Length - 4).Trim());
                    }
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
                        if (type.Equals("box"))
                            map_objects.Add(new StaticMapObject(box, rotation, position, size, 0, 60, type));
                        if (type.Equals("pyramid"))
                            map_objects.Add(new StaticMapObject(pyramid, rotation, position, size, 0, 60, type));
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