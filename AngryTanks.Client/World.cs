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

using AngryTanks.Common;

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

        // textures
        private Texture2D backgroundTexture, boxTexture, pyramidTexture, tankTexture;

        // world information
        private String worldName;

        public String WorldName
        {
            get { return worldName; }
        }

        private Single worldSize = 800;

        public Single WorldSize
        {
            get { return worldSize; }
        }

        // world-unit to pixel conversion factor
        // TODO make some helper methods (place in Resolution class?) that converts many different things back and forth
        public static int worldToPixel = 10;

        // A dictionary of lists of map objects, which are all static sprites
        // once initialized it contains two Lists.
        // Key "tiled" is a List of sprites to be tiled.
        // Key "stretched" is a List of sprites to be stretched.
        private Dictionary<String, List<StaticSprite>> mapObjects = new Dictionary<String, List<StaticSprite>>();
        private List<StaticSprite> tiled = new List<StaticSprite>();
        private List<StaticSprite> stretched = new List<StaticSprite>();

        // List of all Dynamic Sprites (tanks and flags), always drawn stretched.
        private List<StaticSprite> dynamicObjects = new List<StaticSprite>();

        private LocalPlayer localPlayer;

        public World(IServiceProvider iservice)
        {
            IService = iservice;

            // TODO if resolution changes on us, this will fail miserably
            // we need a way to detect if it changes and to then update camera's viewprt
            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)IService.GetService(typeof(IGraphicsDeviceService));

            graphicsDevice = graphicsDeviceService.GraphicsDevice;
            contentManager = new ContentManager(IService, "Content");

            camera = new Camera(graphicsDevice.Viewport);
            camera.Limits = WorldUnitsToPixels(new RectangleF(-WorldSize / 2, -WorldSize / 2, WorldSize, WorldSize));
            camera.LookAt(Vector2.Zero);
        }

        public virtual void LoadContent()
        {
            // create our spritebatch
            spriteBatch = new SpriteBatch(graphicsDevice);

            // load textures
            backgroundTexture = contentManager.Load<Texture2D>("textures/others/grass");
            boxTexture = contentManager.Load<Texture2D>("textures/bz/boxwall");
            pyramidTexture = contentManager.Load<Texture2D>("textures/bz/pyramid");
            tankTexture = contentManager.Load<Texture2D>("textures/tank_white");

            // let's make some test boxes - THESE ARE OVERRIDEN IF A MAP IS LOADED 
            List<StaticSprite> tiled = new List<StaticSprite>();
            tiled.Add(new Box(boxTexture, new Vector2(-100, 100), new Vector2(100, 100), 0, Color.Blue));
            tiled.Add(new Box(boxTexture, new Vector2(100, 100), new Vector2(100, 100), 0, Color.Purple));
            tiled.Add(new Box(boxTexture, new Vector2(100, -100), new Vector2(100, 100), 0, Color.Green));
            tiled.Add(new Box(boxTexture, new Vector2(-100, -100), new Vector2(100, 100), (Single)Math.PI / 4, Color.Red));
            tiled.Add(new Box(boxTexture, new Vector2(0, 0), new Vector2(512, 512), 0, Color.Yellow));
            mapObjects.Add("tiled", tiled);

            // 2.8 width and 6 length are bzflag defaults
            // our tank, however, is a different ratio... it's much fatter. this means some maps may not work so well.
            localPlayer = new LocalPlayer(tankTexture, Vector2.Zero, new Vector2(4.86f, 6), 0);
        }

        public virtual void Update(GameTime gameTime)
        {
            // TODO make sure to move to a better input solution
            KeyboardState ks = Keyboard.GetState();

            // pan the camera
            if (ks.IsKeyDown(Keys.Up))
                camera.Pan(new Vector2(0, -25), true);
            if (ks.IsKeyDown(Keys.Down))
                camera.Pan(new Vector2(0, 25), true);
            if (ks.IsKeyDown(Keys.Left))
                camera.Pan(new Vector2(-25, 0), true);
            if (ks.IsKeyDown(Keys.Right))
                camera.Pan(new Vector2(25, 0), true);

            // TODO later test zooming also with scrollwheel
            // zoom in with pgup/pgdwn
            if (ks.IsKeyDown(Keys.PageUp))
                camera.Zoom *= 1.01f;
            if (ks.IsKeyDown(Keys.PageDown))
                camera.Zoom *= 0.99f;

            // rotation (testing only)
            if (ks.IsKeyDown(Keys.LeftShift))
                camera.Rotation += 0.01f;
            if (ks.IsKeyDown(Keys.RightShift))
                camera.Rotation -= 0.01f;

            // reset back to tank
            if (ks.IsKeyDown(Keys.Home))
            {
                camera.Zoom = 1;
                camera.Rotation = 0;
                camera.PanPosition = Vector2.Zero;
            }

            // now finally track the tank (disregards any panning)
            camera.LookAt(localPlayer.Position * worldToPixel);

            // update the local player
            localPlayer.Update(gameTime);

            // check for collisions
            mapObjects.TryGetValue("stretched", out stretched);
            foreach (Sprite mapObject in stretched)
            {
                if (localPlayer.Intersects(mapObject))
                {
                    if (!mapObject.Collided)
                    {
                        Log.DebugFormat("tank collided with object at {0}", localPlayer.Position);
                        Log.DebugFormat("tank rectangle bounds: {0}", localPlayer.RectangleBounds);
                        Log.DebugFormat("object rectangle bounds: {0}", mapObject.RectangleBounds);
                    }
                    mapObject.Color = Color.Red;
                    mapObject.Collided = true;
                }
                else
                    mapObject.Color = Color.White;
            }

            mapObjects.TryGetValue("tiled", out tiled);
            foreach (Sprite mapObject in tiled)
            {
                if (localPlayer.Intersects(mapObject))
                {
                    if (!mapObject.Collided)
                    {
                        Log.DebugFormat("tank collided with object at {0}", localPlayer.Position);
                        Log.DebugFormat("tank rectangle bounds: {0}", localPlayer.RectangleBounds);
                        Log.DebugFormat("object rectangle bounds: {0}", mapObject.RectangleBounds);
                    }
                    mapObject.Color = Color.Red;
                    mapObject.Collided = true;
                }
                else
                    mapObject.Color = Color.White;
            }

        }

        public virtual void Draw(GameTime gameTime)
        {
            // FIRST Draw pass: the background
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None,
                              camera.GetViewMatrix());

            // magic to tile
            graphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            graphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            RectangleF source, destination;
            source = destination = new RectangleF(camera.CameraPosition, new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));

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
            Single resizeByX = Math.Abs(camera.Origin.X / camera.Zoom);
            Single resizeByY = Math.Abs(camera.Origin.Y / camera.Zoom);

            source.X -= resizeByX;
            source.Y -= resizeByY;
            destination.X -= resizeByX;
            destination.Y -= resizeByY;

            source.Width += 2 * resizeByX;
            source.Height += 2 * resizeByY;
            destination.Width += 2 * resizeByX;
            destination.Height += 2 * resizeByY;

            // draw the grass background
            spriteBatch.Draw(backgroundTexture, (Rectangle)destination, (Rectangle)source, Color.White);

            spriteBatch.End();

            // SECOND Draw pass: draw the stretched map objects.
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                              SpriteSortMode.BackToFront,
                              SaveStateMode.None,
                              camera.GetViewMatrix());
            
            mapObjects.TryGetValue("stretched", out stretched);
            foreach (Sprite mapObject in stretched)
                mapObject.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // THIRD Draw pass: draw the tiled map objects.
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None,
                              camera.GetViewMatrix());

            // using wrapping
            graphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            graphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            mapObjects.TryGetValue("tiled", out tiled);
            foreach (Sprite mapObject in tiled)
                mapObject.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // FOURTH Draw pass: draw the dynamic objects (stretched).
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                              SpriteSortMode.BackToFront,
                              SaveStateMode.None,
                              camera.GetViewMatrix());
            
            foreach (Sprite sprite in dynamicObjects)
                sprite.Draw(gameTime, spriteBatch);

            localPlayer.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public void LoadMap(StreamReader sr)
        {
            // construct the StaticSprites from the stream
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
        private Dictionary<String, List<StaticSprite>> parseMapFile(StreamReader sr)
        {
            Dictionary<String, List<StaticSprite>> mapObjects = new Dictionary<String, List<StaticSprite>>();
            List<StaticSprite> stretched = new List<StaticSprite>();
            List<StaticSprite> tiled = new List<StaticSprite>();
            
            Vector2? position = null;
            Vector2? size = null;
            Single rotation = 0;
            String currentType = ""; // internal identifier to indicate which texture to construct
            int badObjects = 0; // counts object blocks that failed to load

            // control flags
            bool inWorldBlock = false;
            bool inBlock = false;

            // default values for world data will be overidden if found in the file
            worldName = "No Name";
            worldSize = 800;

            String line = "";

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith("world", StringComparison.InvariantCultureIgnoreCase))
                {
                    inWorldBlock = true;
                    inBlock = false;
                }

                if (line.StartsWith("box", StringComparison.InvariantCultureIgnoreCase)
                    || line.StartsWith("pyramid", StringComparison.InvariantCultureIgnoreCase))
                {
                    inWorldBlock = false;
                    inBlock = true;
                    currentType = line.Split(' ')[0];
                }

                if (inWorldBlock)
                {
                    if (line.StartsWith("name", StringComparison.InvariantCultureIgnoreCase))
                    {
                        worldName = line.Trim().Substring(4).Trim();
                    }
                    if (line.StartsWith("size", StringComparison.InvariantCultureIgnoreCase))
                    {
                        worldSize = (UInt16)Convert.ToUInt16(line.Trim().Substring(4).Trim());
                    }
                }

                if (inBlock)
                {
                    if (line.StartsWith("position", StringComparison.InvariantCultureIgnoreCase)
                        || line.StartsWith("pos", StringComparison.InvariantCultureIgnoreCase))
                    {
                        List<String> rawArgs = line.Trim().Substring(9).Split(' ').ToList();
                        List<Single> coords = new List<Single>();

                        rawArgs.ForEach(v => coords.Add((Single)Convert.ToSingle(v)));

                        // only load objects with at least x, y and a zero z-position
                        if (coords.Count == 2 || ((coords.Count == 3) && (Math.Abs(coords[2]) <= Single.Epsilon)))
                        {
                            position = new Vector2(coords[0], coords[1]);
                        }
                    }
                    else if (line.StartsWith("size", StringComparison.InvariantCultureIgnoreCase))
                    {
                        List<String> rawArgs = line.Trim().Substring(5).Split(' ').ToList();
                        List<Single> coords = new List<Single>();

                        rawArgs.ForEach(v => coords.Add((Single)Convert.ToSingle(v)));

                        // only load objects with at least x and y size
                        if (coords.Count >= 2)
                        {
                            size = new Vector2(coords[0], coords[1]);
                        }
                    }
                    else if (line.StartsWith("rotation", StringComparison.InvariantCultureIgnoreCase)
                             || line.StartsWith("rot", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String[] coords = line.Trim().Substring(9).Split(' ');
                        rotation = Convert.ToSingle(coords[0].Trim());
                    }
                }

                if (line.Equals("end"))
                {
                    if (position.HasValue && size.HasValue)
                    {
                        if (currentType.Equals("box"))
                            tiled.Add(new Box(boxTexture, position.Value, size.Value * 2, MathHelper.ToRadians(rotation)));
                        if (currentType.Equals("pyramid"))
                            stretched.Add(new Pyramid(pyramidTexture, position.Value, size.Value * 2, MathHelper.ToRadians(rotation)));
                    }
                    else
                    {
                        badObjects++;
                    }

                    // when finished with one block clear all variables
                    inBlock = false;
                    position = null;
                    size = null;
                    rotation = 0;
                }

            }

            mapObjects.Add("tiled", tiled);
            mapObjects.Add("stretched", stretched);

            return mapObjects;
        }

        public static Vector2 WorldUnitsToPixels(Vector2 vector)
        {
            return vector * worldToPixel;
        }

        public static RectangleF WorldUnitsToPixels(RectangleF rectangle)
        {
            return new RectangleF(rectangle.X * worldToPixel, rectangle.Y * worldToPixel,
                                  rectangle.Width * worldToPixel, rectangle.Height * worldToPixel);
        }
    }
}
