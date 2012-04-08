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
    public class World : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Public Properties

        private bool disposed = false;

        /// <summary>
        /// True if Dispose() has been called
        /// </summary>
        public bool Disposed
        {
            get { return disposed; }
        }

        // world-unit to pixel conversion factor
        public static int WorldToPixel = 10;

        private String worldName;

        /// <summary>
        /// Gets the name of the world
        /// </summary>
        public String WorldName
        {
            get { return worldName; }
        }

        private Single worldSize = 800;

        /// <summary>
        /// Gets the size of the world
        /// </summary>
        public Single WorldSize
        {
            get { return worldSize; }
        }

        private VariableDatabase varDB;

        /// <summary>
        /// Holds the variables in play for <see cref="World"/>
        /// </summary>
        public VariableDatabase VarDB
        {
            get { return varDB; }
        }

        #endregion

        /// <summary>
        /// Allows us to get a reference to various services
        /// </summary>
        protected readonly IServiceProvider IService;

        private Camera camera;

        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private ContentManager contentManager;

        // textures
        private Texture2D backgroundTexture, boxTexture, pyramidTexture, tankTexture;

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
        private Vector2     lastPlayerPosition; // TODO we shouldn't store this here

        public World(IServiceProvider iservice)
        {
            IService = iservice;

            tiled = new List<StaticSprite>();
            stretched = new List<StaticSprite>();

            // initialize variable database
            // TODO need to get variables from server and stick them in this structure
            varDB = new VariableDatabase();

            // TODO if resolution changes on us, this will fail miserably
            // we need a way to detect if it changes and to then update camera's viewprt
            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)IService.GetService(typeof(IGraphicsDeviceService));

            graphicsDevice = graphicsDeviceService.GraphicsDevice;
            contentManager = new ContentManager(IService, "Content");

            camera = new Camera(graphicsDevice.Viewport);
            camera.Limits = WorldUnitsToPixels(new RectangleF(-WorldSize / 2, -WorldSize / 2, WorldSize, WorldSize));
            camera.LookAt(Vector2.Zero);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Disposed)
                return;

            mapObjects.Clear();
            tiled.Clear();
            stretched.Clear();

            localPlayer = null;

            spriteBatch.Dispose();
            contentManager.Unload();

            disposed = true;
        }

        ~World()
        {
            Dispose(false);
        }

        public virtual void LoadContent()
        {
            if (Disposed)
                throw new ObjectDisposedException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

            // create our spritebatch
            spriteBatch = new SpriteBatch(graphicsDevice);

            // load textures
            backgroundTexture = contentManager.Load<Texture2D>("textures/others/grass");
            boxTexture = contentManager.Load<Texture2D>("textures/bz/boxwall");
            pyramidTexture = contentManager.Load<Texture2D>("textures/bz/pyramid");
            tankTexture = contentManager.Load<Texture2D>("textures/tank_white");

            // let's make some test boxes - THESE ARE OVERRIDEN IF A MAP IS LOADED 
            List<StaticSprite> tiled = new List<StaticSprite>();
            tiled.Add(new Box(this, boxTexture, new Vector2(-10, 10), new Vector2(10, 10), 0, Color.Blue));
            tiled.Add(new Box(this, boxTexture, new Vector2(10, 10), new Vector2(10, 10), 0, Color.Purple));
            tiled.Add(new Box(this, boxTexture, new Vector2(10, -10), new Vector2(10, 10), 0, Color.Green));
            tiled.Add(new Box(this, boxTexture, new Vector2(-10, -10), new Vector2(10, 10), (Single)Math.PI / 4, Color.Red));
            tiled.Add(new Box(this, boxTexture, new Vector2(0, 0), new Vector2(10, 10), 0, Color.Yellow));
            mapObjects.Add("tiled", tiled);

            List<StaticSprite> stretched = new List<StaticSprite>();
            mapObjects.Add("stretched", stretched);

            // 2.8 width and 6 length are bzflag defaults
            // our tank, however, is a different ratio... it's much fatter. this means some maps may not work so well.
            localPlayer = new LocalPlayer(this, tankTexture, Vector2.Zero, new Vector2(4.86f, 6), 0);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Disposed)
                throw new ObjectDisposedException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

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

            // update local player
            List<StaticSprite> collisionObjects = new List<StaticSprite>();
            collisionObjects.AddRange(stretched.ToList());
            collisionObjects.AddRange(tiled.ToList());

            // update the local player
            lastPlayerPosition = localPlayer.Position;
            localPlayer.Update(gameTime, collisionObjects);

            // now finally track the tank (disregards any panning)
            // smoothstep helps smooth the camera if player gets stuck
            camera.LookAt(WorldUnitsToPixels(Vector2.SmoothStep(lastPlayerPosition, localPlayer.Position, 0.5f)));
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (Disposed)
                throw new ObjectDisposedException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

            // FIRST Draw pass: the background
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None,
                              camera.GetViewMatrix());

            // magic to tile
            graphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            graphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            RectangleF bgRect = new RectangleF(camera.CameraPosition,
                                               new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));

            Single resizeByX = Math.Abs(camera.Origin.X / camera.Zoom);
            Single resizeByY = Math.Abs(camera.Origin.Y / camera.Zoom);

            bgRect.X -= resizeByX;
            bgRect.Y -= resizeByY;

            bgRect.Width += 2 * resizeByX;
            bgRect.Height += 2 * resizeByY;

            // draw the grass background
            spriteBatch.Draw(backgroundTexture, (Rectangle)bgRect, (Rectangle)bgRect, Color.White);

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
            if (Disposed)
                throw new ObjectDisposedException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

            // construct the StaticSprites from the stream
            mapObjects = ParseMapFile(sr);
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
        private Dictionary<String, List<StaticSprite>> ParseMapFile(StreamReader sr)
        {
            if (Disposed)
                throw new ObjectDisposedException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

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
                        worldSize = (Single)Convert.ToSingle(line.Trim().Substring(4).Trim());
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
                            tiled.Add(new Box(this, boxTexture, position.Value, size.Value * 2, MathHelper.ToRadians(rotation)));
                        if (currentType.Equals("pyramid"))
                            stretched.Add(new Pyramid(this, pyramidTexture, position.Value, size.Value * 2, MathHelper.ToRadians(rotation)));
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
            return vector * WorldToPixel;
        }

        public static RectangleF WorldUnitsToPixels(RectangleF rectangle)
        {
            return new RectangleF(rectangle.X * WorldToPixel, rectangle.Y * WorldToPixel,
                                  rectangle.Width * WorldToPixel, rectangle.Height * WorldToPixel);
        }
    }
}
