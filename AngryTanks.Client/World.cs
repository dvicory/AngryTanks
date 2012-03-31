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
    public class World
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // allows us to get a reference to things like IGraphicsDeviceService
        protected readonly IServiceProvider IService;

        // world manages the camera
        private Camera camera;

        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private ContentManager contentManager;

        // background texture
        static private Texture2D backgroundTexture, boxTexture, pyramidTexture;

        // the world data;
        public static String world_name;
        public static int world_size;

        // world-unit to pixel conversion factor
        public static int worldToPixel = 10;

        // list of map objects, which are all static sprites
        private List<StaticSprite> mapObjects = new List<StaticSprite>();

        public World(IServiceProvider iservice)
        {
            IService = iservice;

            // TODO what happens if the viewport changes?
            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)IService.GetService(typeof(IGraphicsDeviceService));

            graphicsDevice = graphicsDeviceService.GraphicsDevice;
            contentManager = new ContentManager(IService, "Content");

            camera = new Camera(graphicsDevice.Viewport);
            camera.LookAt(Vector2.Zero);
        }

        public virtual void LoadContent()
        {
            // create our spritebatch
            spriteBatch = new SpriteBatch(graphicsDevice);

            // load textures
            backgroundTexture = contentManager.Load<Texture2D>("textures/selfmade/b");
            boxTexture = contentManager.Load<Texture2D>("textures/bz/boxwall");
            pyramidTexture = contentManager.Load<Texture2D>("textures/bz/pyramid");


            // let's make some test boxes - THESE ARE OVERRIDEN IF A MAP IS LOADED                       
            mapObjects.Add(new Box(boxTexture, new Vector2(-100, 100), new Vector2(100, 100), 0, Color.Blue));
            mapObjects.Add(new Box(boxTexture, new Vector2(100, 100), new Vector2(100, 100), 0, Color.Purple));            
            mapObjects.Add(new Box(boxTexture, new Vector2(100, -100), new Vector2(100, 100), 0, Color.Green));
            mapObjects.Add(new Box(boxTexture, new Vector2(-100, -100), new Vector2(100, 100), Math.PI/4, Color.Red));
            mapObjects.Add(new Box(boxTexture, new Vector2(0, 0), new Vector2(512, 512), 0, Color.Yellow));            
        }

        public virtual void Update(GameTime gameTime)
        {
            // TODO make sure to move to UserInput helper class when available
            KeyboardState ks = Keyboard.GetState();

            // move the camera
            if (ks.IsKeyDown(Keys.Up))
                camera.Move(new Vector2(0, -25), true);
            if (ks.IsKeyDown(Keys.Down))
                camera.Move(new Vector2(0, 25), true);
            if (ks.IsKeyDown(Keys.Left))
                camera.Move(new Vector2(-25, 0), true);
            if (ks.IsKeyDown(Keys.Right))
                camera.Move(new Vector2(25, 0), true);
            if (ks.IsKeyDown(Keys.Home))
                camera.LookAt(Vector2.Zero);

            // TODO later test zooming also with scrollwheel
            // zoom in with pgup/pgdwn
            if (ks.IsKeyDown(Keys.PageUp))
                camera.Zoom *= 1.01f;
            if (ks.IsKeyDown(Keys.PageDown))
                camera.Zoom *= 0.99f;
            if (ks.IsKeyDown(Keys.Home))
                camera.Zoom = 1;

            // rotation (testing only)
            if (ks.IsKeyDown(Keys.LeftShift))
                camera.Rotation += 0.01f;
            if (ks.IsKeyDown(Keys.RightShift))
                camera.Rotation -= 0.01f;
            if (ks.IsKeyDown(Keys.Home))
                camera.Rotation = 0;
        }

        public virtual void Draw(GameTime gameTime)
        {
            // TODO fix, breaks stuff, so keep it at 1
            float backgroundZoomFactor = 1f;

            // immediate mode is required for tiling
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None,
                              camera.GetViewMatrix(backgroundZoomFactor, true));

            // magic to tile
            graphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            graphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            Rectangle source = new Rectangle((int)camera.Position.X, (int)camera.Position.Y,
                                             graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            Rectangle destination = new Rectangle((int)camera.Position.X, (int)camera.Position.Y,
                                                  graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            /*
             * following works for infinite scrolling background IF Camera.Origin = Vector2.Zero
            source.Width += Math.Abs((int)(GraphicsDevice.Viewport.Width / Camera.Zoom));
            source.Height += Math.Abs((int)(GraphicsDevice.Viewport.Height / Camera.Zoom));
            destination.Width += Math.Abs((int)(GraphicsDevice.Viewport.Width / Camera.Zoom));
            destination.Height += Math.Abs((int)(GraphicsDevice.Viewport.Height / Camera.Zoom));
            */

            /*
             * works but I think the math is wrong resulting in drawing more than required
            source.X -= Math.Abs((int)(Camera.Origin.X / Camera.Zoom));
            source.Y -= Math.Abs((int)(Camera.Origin.Y / Camera.Zoom));
            destination.X -= Math.Abs((int)(Camera.Origin.X / Camera.Zoom));
            destination.Y -= Math.Abs((int)(Camera.Origin.Y / Camera.Zoom));

            source.Width += Math.Abs(2 * (int)(Camera.Origin.X / Camera.Zoom));
            source.Height += Math.Abs(2 * (int)(Camera.Origin.Y / Camera.Zoom));
            destination.Width += Math.Abs(2 * (int)(Camera.Origin.X / Camera.Zoom));
            destination.Height += Math.Abs(2 * (int)(Camera.Origin.Y / Camera.Zoom));
            */

            // essentially the same as above commented out code, just stuck in variables to be faster
            // TODO someone much smarter than me check the math on this. it works, but I'm sure it's overdrawing
            int resizeByX = Math.Abs((int)(camera.Origin.X / (backgroundZoomFactor * camera.Zoom)));
            int resizeByY = Math.Abs((int)(camera.Origin.Y / (backgroundZoomFactor * camera.Zoom)));

            source.X -= resizeByX;
            source.Y -= resizeByY;
            destination.X -= resizeByX;
            destination.Y -= resizeByY;

            source.Width += 2 * resizeByX;
            source.Height += 2 * resizeByY;
            destination.Width += 2 * resizeByX;
            destination.Height += 2 * resizeByY;

            // draw the grass background
            spriteBatch.Draw(backgroundTexture, destination, source, Color.White);

            spriteBatch.End();

            //Immediate mode and the Wrapping are no longer used for this Draw pass.
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                              SpriteSortMode.BackToFront, //Returned to BackToFront
                              SaveStateMode.None,
                              camera.GetViewMatrix());

            //No longer using Wrapping
            //graphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            //graphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            foreach (Sprite mapObject in mapObjects)
                mapObject.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public void LoadMap(StreamReader sr)
        {
            // construct the StaticMapObjects from the stream
            mapObjects = parseMapFile(sr);
        }

        /* parseMapFile()
         * 
         * Takes in a StreamReader object and returns a List of StaticSprites
         * corresponding to the boxes and pyramids which have a zero Z-position
         * found in the stream.
         * 
         * IF THERE IS NO WORLD DATA this function will set the world name and size to default
         * values 'No Name' and 800.
         * 
         */
        private static List<StaticSprite> parseMapFile(StreamReader sr)
        {
            List<StaticSprite> map_objects = new List<StaticSprite>();
            String line = "";
            
            Vector2 position = Vector2.Zero;
            Vector2 size = Vector2.Zero;
            double rotation = 0;
            String type = "";    // Internal identifier to indicate which texture to construct
            int bad_objects = 0; // Counts object blocks that failed to load  

            //Control flags
            bool inWorldBlock = false;
            bool inBlock = false;             
            bool got_position = false;
            bool got_size = false;

            //Default values for world data will be overidden if found in the file
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

                        //Only load objects with a zero Z-position
                        if (coords.Length == 3)
                        {
                            if (coords[2].Trim().Equals("0"))
                            {
                                got_position = true;
                            }
                            else
                            {
                                got_position = false;
                            }
                        }
                        else
                        {
                            got_position = true;
                        }
                        
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
                        rotation = (double)Convert.ToDecimal(coords[0].Trim());
                    }
                }
                if (line.Equals("end"))
                {
                    inBlock = false;
                    if (got_position && got_size)
                    {
                        if (type.Equals("box"))
                            map_objects.Add(new Box(boxTexture, position/2, size, rotation * (Math.PI / 180)));
                        if (type.Equals("pyramid"))
                            map_objects.Add(new Box(pyramidTexture, position/2, size, rotation * (Math.PI / 180)));
                    }
                    else
                    {
                        bad_objects++;
                    }
                    //When finished with one block clear all variables 
                    got_position = false;
                    got_size = false;
                    rotation = 0;
                }

            }
            return map_objects;
        }

    }
}
