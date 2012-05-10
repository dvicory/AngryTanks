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
using Nuclex.Game;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public class World : GraphicsDeviceDrawableComponent
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

        private ContentManager content;

        public ContentManager Content
        {
            get { return content; }
        }

        private ServerLink serverLink;

        public ServerLink ServerLink
        {
            get { return serverLink; }
        }

        public List<IWorldObject> MapObjects
        {
            get
            {
                List<Sprite> mapObjects = new List<Sprite>();
                mapObjects.AddRange(this.mapObjects["stretched"].ToList());
                mapObjects.AddRange(this.mapObjects["tiled"].ToList());

                return mapObjects.Cast<IWorldObject>().ToList();
            }
        }

        private Grid mapGrid;

        public Grid MapGrid
        {
            get { return mapGrid; }
        }

        private PlayerManager playerManager;

        public PlayerManager PlayerManager
        {
            get { return playerManager; }
        }

        #endregion

        public IServiceProvider IService;
        private SpriteBatch spriteBatch;

        // textures
        private Texture2D backgroundTexture, boxTexture, pyramidTexture, tankTexture;

        // A dictionary of lists of map objects, which are all static sprites
        // once initialized it contains two Lists.
        // Key "tiled" is a List of sprites to be tiled.
        // Key "stretched" is a List of sprites to be stretched.
        private Dictionary<String, List<Sprite>> mapObjects = new Dictionary<String, List<Sprite>>();
        private List<Sprite> tiled = new List<Sprite>();
        private List<Sprite> stretched = new List<Sprite>();

        // List of all Dynamic Sprites (tanks and flags), always drawn stretched.
        private List<Sprite> dynamicObjects = new List<Sprite>();

        private IAudioManager audioManager;

        private ScoreHUD scoreHUD;

        private Vector2     lastPlayerPosition; // TODO we shouldn't store this here

        public World(IServiceProvider iservice, ServerLink serverLink)
            : base(iservice)
        {
            this.IService = iservice;

            this.serverLink = serverLink;

            this.tiled = new List<Sprite>();
            this.stretched = new List<Sprite>();

            // initialize variable database
            // TODO need to get variables from server and stick them in this structure
            this.varDB = new VariableDatabase();

            // initialize player manager
            this.playerManager = new PlayerManager(this); 
           
            // initialize score HUD
            this.scoreHUD = new ScoreHUD(this.playerManager);

            ServerLink.MessageReceivedEvent += HandleReceivedMessage;
        }

        public override void Dispose()
        {
            playerManager.Dispose();
            playerManager = null;

            mapObjects.Clear();
            tiled.Clear();
            stretched.Clear();

            spriteBatch.Dispose();
            content.Unload();

            GraphicsDevice.DeviceReset -= GraphicsDeviceReset;
            ServerLink.MessageReceivedEvent -= HandleReceivedMessage;

            if(scoreHUD != null)
                scoreHUD.isActive = false; //Deactivate scoreHUD to prevent NullReferenceException

            base.Dispose();
        }

        private void GraphicsDeviceReset(object sender, EventArgs e)
        {
            camera.Viewport = GraphicsDevice.Viewport;
        }

        public override void Initialize()
        {
            // create our content manager
            content = new ContentManager(IService, "Content");

            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)IService.GetService(typeof(IGraphicsDeviceService));
            GraphicsDevice graphicsDevice = graphicsDeviceService.GraphicsDevice;

            // create our spritebatch
            spriteBatch = new SpriteBatch(graphicsDevice);

            // get the game console
            console = (IGameConsole)IService.GetService(typeof(IGameConsole));

            // get the audio manager
            audioManager = (IAudioManager)IService.GetService(typeof(IAudioManager));            

            // setup camera
            camera = new Camera(graphicsDevice.Viewport);
            camera.Limits = WorldUnitsToPixels(new RectangleF(-WorldSize / 2, -WorldSize / 2, WorldSize, WorldSize));
            camera.LookAt(Vector2.Zero);

            graphicsDevice.DeviceReset += GraphicsDeviceReset;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // load textures
            backgroundTexture = Content.Load<Texture2D>("textures/others/grass");
            boxTexture = Content.Load<Texture2D>("textures/bz/boxwall");
            pyramidTexture = Content.Load<Texture2D>("textures/bz/pyramid");
            tankTexture = Content.Load<Texture2D>("textures/tank_white");

            // let's make some test boxes - THESE ARE OVERRIDEN IF A MAP IS LOADED 
            List<Sprite> tiled = new List<Sprite>();
            tiled.Add(new Box(this, boxTexture, new Vector2(-10, 10), new Vector2(10, 10), 0, Color.Blue));
            tiled.Add(new Box(this, boxTexture, new Vector2(10, 10), new Vector2(10, 10), 0, Color.Purple));
            tiled.Add(new Box(this, boxTexture, new Vector2(10, -10), new Vector2(10, 10), 0, Color.Green));
            tiled.Add(new Box(this, boxTexture, new Vector2(-10, -10), new Vector2(10, 10), (Single)Math.PI / 4, Color.Red));
            tiled.Add(new Box(this, boxTexture, new Vector2(0, 0), new Vector2(10, 10), 0, Color.Yellow));
            mapObjects.Add("tiled", tiled);

            List<Sprite> stretched = new List<Sprite>();
            mapObjects.Add("stretched", stretched);

            // load grid
            mapGrid = new Grid(new Vector2(WorldSize, WorldSize), MapObjects);

            // load scoreHUD font
            scoreHUD.LoadContent(Content);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // handle any keys currently pressed
            HandleKeyDown(Keyboard.GetState());

            // update the players
            if (playerManager != null)
                playerManager.Update(gameTime);            
           
            // now finally track the tank (disregards any panning)
            // smoothstep helps smooth the camera if player gets stuck
            if (playerManager != null && playerManager.LocalPlayer != null)
            {
                lastPlayerPosition = playerManager.LocalPlayer.Position;
                camera.LookAt(World.WorldUnitsToPixels(Vector2.SmoothStep(lastPlayerPosition, playerManager.LocalPlayer.Position, 0.5f)));
                
                // activate score HUD
                scoreHUD.isActive = true;
                scoreHUD.Update();
            }

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
            if (Console.PromptActive)
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

            // audio (testing only)
            if (ks.IsKeyDown(Keys.B))
            {
                audioManager.Play("boom");                
            }
            if (ks.IsKeyDown(Keys.N))
            {
                audioManager.Play("boom", new Vector2(0, 0), new Vector2(400, 300));
            }
            if (ks.IsKeyDown(Keys.M))
            {                
                audioManager.playSong("Intergalactic");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // FIRST Draw pass: the background
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None,
                              camera.GetViewMatrix());

            // magic to tile
            GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            RectangleF bgRect = new RectangleF(camera.CameraPosition,
                                               new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));

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
            GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

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

            playerManager.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // FIFTH Draw pass: draw the HUD components.
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                              SpriteSortMode.BackToFront,
                              SaveStateMode.None);

            scoreHUD.Draw(spriteBatch);

            spriteBatch.End();
           
            base.Draw(gameTime);
        }

        private void HandleReceivedMessage(object sender, ServerLinkMessageEvent message)
        {
            switch (message.MessageType)
            {
                case MessageType.MsgWorld:
                    Console.WriteLine("Loading map...");

                    MsgWorldPacket worldPacket = (MsgWorldPacket)message.MessageData;

                    LoadMap(worldPacket.Map);

                    Console.WriteLine(String.Format("Map \"{0}\" loaded.", WorldName));

                    break;

                case MessageType.MsgDeath:
                    MsgDeathPacket deathPacket = (MsgDeathPacket)message.MessageData;

                    Player killee = PlayerManager.GetPlayerBySlot(deathPacket.Slot);
                    Player killer = PlayerManager.GetPlayerBySlot(deathPacket.Killer);

                    if (killee == null || killer == null)
                        return;

                    ConsoleMessageLine consoleMessage;

                    if (killer == PlayerManager.LocalPlayer)
                    {
                        consoleMessage =
                            new ConsoleMessageLine(
                                "You killed ", Color.White,
                                killee.Callsign, ProtocolHelpers.TeamTypeToColor(killer.Team));
                    }
                    else
                    {
                        consoleMessage =
                            new ConsoleMessageLine(
                                killee.Callsign, ProtocolHelpers.TeamTypeToColor(killee.Team),
                                " was killed by ", Color.White,
                                killer.Callsign, ProtocolHelpers.TeamTypeToColor(killer.Team));
                    }

                    Console.WriteLine(consoleMessage);

                    break;

                default:
                    break;
            }
        }

        public void LoadMap(StreamReader sr)
        {
            // construct the StaticSprites from the stream
            mapObjects = ParseMapFile(sr);

            // add the boundaries
            AddMapBoundaries();

            // now we can make our grid, make it 10% larger than actual size to get any objects near the world edge
            mapGrid = new Grid(new Vector2(WorldSize, WorldSize) * 1.1f, MapObjects);
        }

        private void AddMapBoundaries()
        {
            List<Sprite> tiled = mapObjects["tiled"];

            tiled.Add(new Box(this, boxTexture, new Vector2((-WorldSize / 2) - 5, 0), new Vector2(10, WorldSize + 20), 0));
            tiled.Add(new Box(this, boxTexture, new Vector2(( WorldSize / 2) + 5, 0), new Vector2(10, WorldSize + 20), 0));
            tiled.Add(new Box(this, boxTexture, new Vector2(0, (-WorldSize / 2) - 5), new Vector2(WorldSize + 20, 10), 0));
            tiled.Add(new Box(this, boxTexture, new Vector2(0, ( WorldSize / 2) + 5), new Vector2(WorldSize + 20, 10), 0));
        }

        /// <summary>
        /// <para>
        ///     Takes in a StreamReader <paramref name="sr"/> and returns a Dictionary mapping
        ///     the draw type of the world object to a List of <see cref="Sprite"/>s.
        /// </para>
        /// <para>
        ///     If there is no world data, this method will set the world name and size to
        ///     the default values of "No Name" and 800 world units, respectively.
        /// </para>
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        private Dictionary<String, List<Sprite>> ParseMapFile(StreamReader sr)
        {
            Dictionary<String, List<Sprite>> mapObjects = new Dictionary<String, List<Sprite>>();
            List<Sprite> stretched = new List<Sprite>();
            List<Sprite> tiled = new List<Sprite>();
            
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
