using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using AngryTanks.Common;


namespace AngryTanks.Tests.GridTesting
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<IWorldObject> allObjects = new List<IWorldObject>();
        List<IWorldObject> collidables = new List<IWorldObject>();
        List<IWorldObject> nonCollidables = new List<IWorldObject>();

        WorldObject UpperLeft, UpperRight, LowerLeft, LowerRight, mouse;
        Point gridSize = new Point(8, 8);
        Grid grid;


        Texture2D RectangleTexture;
        SpriteFont MyFont;
        Color drawColor = Color.White;

        MouseState ms;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            RectangleTexture = Content.Load<Texture2D>("Square");
            MyFont = Content.Load<SpriteFont>("MyFont");

            UpperLeft = new WorldObject(new Vector2(100, 150), new Vector2(75, 75), (Single)0.0f);
            UpperRight = new WorldObject(new Vector2(500, 100), new Vector2(75, 75), (Single)0.0f);
            LowerLeft = new WorldObject(new Vector2(200, 450), new Vector2(100, 100), (Single)0.0f);
            LowerRight = new WorldObject(new Vector2(550, 480), new Vector2(100, 100), (Single)0.0f);
            mouse = new WorldObject(new Vector2(100, 590), new Vector2(50, 50), (Single)Math.PI / 4);

            allObjects.Add(UpperLeft);
            allObjects.Add(UpperRight);
            allObjects.Add(LowerLeft);
            allObjects.Add(LowerRight);

            //Without a camera only the first quadrant will be visible - so the world is 4x(800,600)
            grid = new Grid(new Vector2(1600, 1200), gridSize, allObjects);
            System.Diagnostics.Debug.WriteLine("The Grid is constructed");
            System.Diagnostics.Debug.WriteLine("LoadContent is finshed");
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

            //Move mouse around
            ms = Mouse.GetState();
            mouse.Position = new Vector2(ms.X, ms.Y);
            mouse.ReCalcBounds();

            //Find all WorldObjects that share a GridLocation with the mouse Sprite           
            collidables = grid.PotentialIntersects(mouse);

            //Find all WorldObjects that DO NOT share a Grid Location with the mouse
            nonCollidables.Clear();
            foreach (WorldObject w in allObjects)
            {
                nonCollidables.Add(w);
            }
            foreach (WorldObject w in collidables)
            {
                nonCollidables.Remove(w);
            }
            System.Diagnostics.Debug.WriteLine("Update is finshed");
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
            spriteBatch.Begin();

            //Draw Gridbounds  

            //Draw Vertical lines
            for (int i = 0; i <= (gridSize.X / 2) - 1; i++)
            {
                spriteBatch.Draw(RectangleTexture, new Rectangle((800 / (gridSize.X / 2)) * i,
                                                             0,
                                                             1,
                                                             600), Color.Yellow);
            }
            //Draw Horizontal line
            for (int j = 0; j <= (gridSize.Y / 2) - 1; j++)
            {
                spriteBatch.Draw(RectangleTexture, new Rectangle(0,
                                                             (600 / (gridSize.Y / 2)) * j,
                                                             800,
                                                             1),
                                                             Color.Yellow);
            }


            //Draw Collidables
            foreach (WorldObject w in collidables)
            {
                spriteBatch.Draw(RectangleTexture, new Rectangle((int)(w.Position.X - w.Size.X / 2),
                                                                 (int)(w.Position.Y - w.Size.Y / 2),
                                                                 (int)w.Size.X,
                                                                 (int)w.Size.Y), Color.Red);
            }

            //Draw Non-collidables
            foreach (WorldObject w in nonCollidables)
            {
                spriteBatch.Draw(RectangleTexture, new Rectangle((int)(w.Position.X - w.Size.X / 2),
                                                                 (int)(w.Position.Y - w.Size.Y / 2),
                                                                 (int)w.Size.X,
                                                                 (int)w.Size.Y), Color.Green);
            }

            //Draw the Mouse
            spriteBatch.Draw(RectangleTexture, new Rectangle((int)(mouse.Position.X),
                                                             (int)(mouse.Position.Y),
                                                             (int)mouse.Size.X,
                                                             (int)mouse.Size.Y),
                                               new Rectangle(0, 0, RectangleTexture.Width,
                                                                 RectangleTexture.Height),
                                                              Color.Blue,
                                                              mouse.Rotation,
                                                              new Vector2(RectangleTexture.Width / 2,
                                                                          RectangleTexture.Height / 2),
                                                              SpriteEffects.None,
                                                              0.0f);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
