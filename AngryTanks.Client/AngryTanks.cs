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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AngryTanks : Microsoft.Xna.Framework.Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static ServerLink serverLink;
        //public Map map;

        private World world;

        Texture2D tankTexture;
        PlayerControlledSprite playerSprite;

        public AngryTanks()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //serverLink = new ServerLink();
            //serverLink.Connect("localhost", 5150, MapLoaded);
            
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

            // intantiate a map
            //map = new Map(this);
            //map.Initialize(GraphicsDevice);
            world = new World(Services);

            tankTexture = Content.Load<Texture2D>("textures/tank");
            playerSprite = new PlayerControlledSprite(tankTexture,
                                                      new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2),
                                                      new Vector2(tankTexture.Width / 2, tankTexture.Height / 2), 0);

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
            //map.LoadContent(Content);
            world.LoadContent();
            world.LoadMap(new StreamReader("../../../Content/maps/pillbox.bzw"));
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

            // TODO: Add your update logic here
            //serverLink.Update();

            world.Update(gameTime);
            playerSprite.Update(gameTime);

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
            /*
            if (serverLink.GotWorld)
                map.Draw(gameTime);
            */

            world.Draw(gameTime);
            playerSprite.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        // TODO make this a more generic ServerLinkStateChanged
        private void MapLoaded(StreamReader map)
        {
            Log.Debug("AngryTanks.MapLoaded");

            //map.LoadMap(map);
        }
    }
}
