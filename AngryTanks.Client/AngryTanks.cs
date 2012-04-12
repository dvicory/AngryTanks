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

using Nuclex.Input;
using log4net;

namespace AngryTanks.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AngryTanks : Microsoft.Xna.Framework.Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        GraphicsDeviceManager graphics;
        public static InputManager Input;
        SpriteBatch spriteBatch;

        private static ServerLink serverLink;
        public static ServerLink ServerLink
        {
            get { return serverLink; }
        }

        private static World world;
        public static World World
        {
            get { return world; }
        }

        private static GameConsole gameConsole;
        public static GameConsole Console
        {
            get { return gameConsole; }
        }


        private bool fullscreen = false;
        private Viewport lastViewport;

        public AngryTanks()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // instantiate server link
            serverLink = new ServerLink();

            // down with xna's input!
            Input = new InputManager(Services, Window.Handle);
            Components.Add(Input);
            Input.UpdateOrder = 100;

            // instantiate game console
            gameConsole = new GameConsole(this, new Vector2(0, 400), new Vector2(800, 200),
                                          new Vector2(10, 10), new Vector2(10, 10), new Color(255, 255, 255, 100));
            Components.Add(gameConsole);
            gameConsole.UpdateOrder = 1000;
            gameConsole.DrawOrder = 1000;

            // instantiate the world
            world = new World(this);
            Components.Add(world);
            world.UpdateOrder = 100;
            world.DrawOrder = 100;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            world.LoadMap(new StreamReader("Content/maps/ducati_style_random.bzw"));

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState kb = Keyboard.GetState();

            // TODO these keys are temporary

            if (kb.IsKeyDown(Keys.F1))
            {
                if (!fullscreen)
                {
                    graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
                    graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
                    graphics.IsFullScreen = true;
                    graphics.ApplyChanges();

                    fullscreen = !fullscreen;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 600;
                    graphics.IsFullScreen = false;
                    graphics.ApplyChanges();

                    fullscreen = !fullscreen;
                }
            }

            // connect to the server when you press C
            if (kb.IsKeyDown(Keys.C)
                && (serverLink.ServerLinkStatus == NetServerLinkStatus.None
                    || serverLink.ServerLinkStatus == NetServerLinkStatus.Disconnected))
            {
                if (world != null)
                {
                    world.Dispose();
                    world = null;
                }

                world = new World(this);
                Components.Add(world);
                world.UpdateOrder = 100;
                world.DrawOrder = 100;

                Console.WriteLine("Connecting to server...");
                serverLink.Connect("localhost", 5150);
            }

            //  disconnect when you press X
            if (kb.IsKeyDown(Keys.X)
                && serverLink.ServerLinkStatus == NetServerLinkStatus.Connected)
            {
                if (world != null)
                {
                    world.Dispose();
                    world = null;
                }

                world = new World(this);
                Components.Add(world);
                world.UpdateOrder = 100;
                world.DrawOrder = 100;

                Console.WriteLine("Disconnecting from server.");
                serverLink.Disconnect("player disconnected");
            }

            serverLink.Update();

            // TODO should probably have console do more advanced positioning that accounts for this...
            // do we need to change console's position and size?
            Viewport viewport = GraphicsDevice.Viewport;

            if ((viewport.Width != lastViewport.Width) || (viewport.Height != lastViewport.Height))
            {
                gameConsole.Position = new Vector2(0, viewport.Height - 200);
                gameConsole.Size = new Vector2(viewport.Width, 200);
            }

            lastViewport = viewport;

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
