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
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public class World : DrawableGameComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region World Properties

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

        private Camera camera;

        public Camera Camera
        {
            get { return camera; }
        }

        private IGameConsole console;

        public IGameConsole Console
        {
            get { return console; }
        }

        #endregion

        private GraphicsDevice graphicsDevice;
        private ContentManager contentManager;
        private SpriteBatch spriteBatch;

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

        public World(Game game)
            : base(game)
        {
            this.tiled = new List<StaticSprite>();
            this.stretched = new List<StaticSprite>();

            // initialize variable database
            // TODO need to get variables from server and stick them in this structure
            this.varDB = new VariableDatabase();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                mapObjects.Clear();
                tiled.Clear();
                stretched.Clear();

                localPlayer = null;

                spriteBatch.Dispose();
                contentManager.Unload();

                graphicsDevice.DeviceReset -= GraphicsDeviceReset;
                AngryTanks.ServerLink.MessageReceivedEvent -= ReceiveMessage;
            }

            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            // get our graphics device
            graphicsDevice = Game.GraphicsDevice;

            // create our content manager
            contentManager = new ContentManager(Game.Services, Game.Content.RootDirectory);

            // create our spritebatch
            spriteBatch = new SpriteBatch(graphicsDevice);

            // get the game console
            console = (IGameConsole)Game.Services.GetService(typeof(IGameConsole));

            // setup camera
            camera = new Camera(graphicsDevice.Viewport);
            camera.Limits = WorldUnitsToPixels(new RectangleF(-WorldSize / 2, -WorldSize / 2, WorldSize, WorldSize));
            camera.LookAt(Vector2.Zero);

            graphicsDevice.DeviceReset += GraphicsDeviceReset;
            AngryTanks.ServerLink.MessageReceivedEvent += ReceiveMessage;

            base.Initialize();
        }

        private void GraphicsDeviceReset(object sender, EventArgs e)
        {
            camera.Viewport = graphicsDevice.Viewport;
        }

        protected override void LoadContent()
        {
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

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            HandleKeyDown(Keyboard.GetState());

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

            base.Update(gameTime);
        }

        protected void HandleKeyDown(KeyboardState ks)
        {
            // reset back to tank
            if (ks.IsKeyDown(Keys.Home))
            {
                camera.Zoom = 1;
                camera.Rotation = 0;
                camera.PanPosition = Vector2.Zero;
            }

            // the above is the only thing allowed when console prompt is active
            if (console.PromptActive)
                return;

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
            if (ks.IsKeyDown(Keys.LeftShift) && ks.IsKeyDown(Keys.LeftAlt))
                camera.Rotation += 0.01f;
            if (ks.IsKeyDown(Keys.RightShift) && ks.IsKeyDown(Keys.RightAlt))
                camera.Rotation -= 0.01f;
        }

        public override void Draw(GameTime gameTime)
        {
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

            base.Draw(gameTime);
        }

        private void ReceiveMessage(object sender, ServerLinkMessageEvent message)
        {
            switch (message.MessageType)
            {
                case MessageType.MsgAddPlayer:
                    {
                        MsgAddPlayerPacket packet = (MsgAddPlayerPacket)message.MessageData;

                        if (message.ServerLinkStatus == NetServerLinkStatus.Connected)
                            console.WriteLine(String.Format("{0} has joined the {1}",
                                                  packet.Player.Callsign, packet.Player.Team));
                        else if (message.ServerLinkStatus == NetServerLinkStatus.GettingState)
                            console.WriteLine(String.Format("{0} is on the {1}",
                                                  packet.Player.Callsign, packet.Player.Team));

                        break;
                    }

                case MessageType.MsgRemovePlayer:
                    {
                        MsgRemovePlayerPacket packet = (MsgRemovePlayerPacket)message.MessageData;

                        console.WriteLine(String.Format("Player {0} has left the server ({1})", packet.Slot, packet.Reason));

                        break;
                    }

                case MessageType.MsgWorld:
                    console.WriteLine("Loading map...");

                    MsgWorldPacket msgWorldData = (MsgWorldPacket)message.MessageData;
                    
                    LoadMap(msgWorldData.Map);

                    console.WriteLine(String.Format("Map \"{0}\" loaded.", WorldName));

                    break;

                default:
                    break;
            }
        }

        public void LoadMap(StreamReader sr)
        {
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
