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

using AngryTanks.Common.Protocol;
using AngryTanks.Common.Messages;

namespace AngryTanks.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AngryTanks : Microsoft.Xna.Framework.Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static ServerLink serverLink;

        private World world;

        public AngryTanks()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            serverLink = new ServerLink();
            serverLink.MessageReceivedEvent += ReceiveMessage;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // instantiate the world
            world = new World(Services);

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

            // TODO: use this.Content to load your game content here
            world.LoadContent();
            world.LoadMap(new StreamReader("Content/maps/ducati_style_random.bzw"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
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

                world = new World(Services);
                world.LoadContent();
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

                serverLink.Disconnect("player initiated disconnect");
            }

            serverLink.Update();

            if (world != null)
                world.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            if (world != null)
                world.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void ReceiveMessage(object sender, ServerLinkMessageEvent message)
        {
            Log.Debug("AngryTanks.ReceiveMessage");

            switch (message.MessageType)
            {
                case MessageType.MsgWorld:
                    Log.Debug("Received MsgWorld");

                    MsgWorldData msgWorldData = (MsgWorldData)message.MessageData;

                    if (world != null)
                        world.LoadMap(msgWorldData.Map);

                    break;

                default:
                    break;
            }
        }
    }
}
