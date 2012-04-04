using System;
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

using AngryTanks.Common;

namespace AngryTanks.Tests.CollisionDetection
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RotatedRectangle RectangleA;
        RotatedRectangle RectangleB;

        SpriteFont MyFont;
        Texture2D RectangleTexture;

        bool IsRectangleASelected = true;
        bool IsCollisionDetected = false;

        KeyboardState PreviousKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
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
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            RectangleTexture = Content.Load<Texture2D>("Square");
            MyFont = Content.Load<SpriteFont>("MyFont");

            RectangleA = new RotatedRectangle(new Rectangle(100, 200, RectangleTexture.Width, RectangleTexture.Height), 0.0f);
            RectangleB = new RotatedRectangle(new Rectangle(300, 200, 130, 390), 0.0f);
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
            SwitchActiveRectangle();
            MoveRectangle();
            IsCollisionDetected = RectangleA.Intersects(RectangleB);
            base.Update(gameTime);
        }

        private void SwitchActiveRectangle()
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();
            if (aCurrentKeyboardState.IsKeyDown(Keys.Tab) && PreviousKeyboardState.IsKeyDown(Keys.Tab) == false)
            {
                IsRectangleASelected = !IsRectangleASelected;
            }
            PreviousKeyboardState = aCurrentKeyboardState;
        }

        private void MoveRectangle()
        {
            if (IsRectangleASelected)
            {
                MoveRectangleA();
            }
            else
            {
                MoveRectangleB();
            }
        }

        private void MoveRectangleA()
        {
            KeyboardState CurrentKeyboard = Keyboard.GetState();

            if (CurrentKeyboard.IsKeyDown(Keys.Up))
            {
                RectangleA.ChangePosition(0, -2);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Down))
            {
                RectangleA.ChangePosition(0, 2);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Left))
            {
                RectangleA.ChangePosition(-2, 0);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Right))
            {
                RectangleA.ChangePosition(2, 0);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Space))
            {
                RectangleA.Rotation += 0.01f;
            }
        }

        private void MoveRectangleB()
        {
            KeyboardState CurrentKeyboard = Keyboard.GetState();

            if (CurrentKeyboard.IsKeyDown(Keys.Up))
            {
                RectangleB.ChangePosition(0, -2);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Down))
            {
                RectangleB.ChangePosition(0, 2);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Left))
            {
                RectangleB.ChangePosition(-2, 0);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Right))
            {
                RectangleB.ChangePosition(2, 0);
            }

            if (CurrentKeyboard.IsKeyDown(Keys.Space))
            {
                RectangleB.Rotation += 0.01f;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Color aColor = Color.Blue;
            if (IsCollisionDetected)
            {
                aColor = Color.Red;
            }

            spriteBatch.Begin();

            spriteBatch.DrawString(MyFont, "Move the Rectangle using the Arrow Keys", new Vector2(60, 30), Color.White);
            spriteBatch.DrawString(MyFont, "Rotate the Rectangle using the Spacebar", new Vector2(60, 70), Color.White);
            spriteBatch.DrawString(MyFont, "Change the selected Rectangle using the Tab key", new Vector2(60, 110), Color.White);

            Rectangle aPositionAdjusted = new Rectangle(RectangleA.X + (RectangleA.Width / 2), RectangleA.Y + (RectangleA.Height / 2), RectangleA.Width, RectangleA.Height);
            spriteBatch.Draw(RectangleTexture, aPositionAdjusted, new Rectangle(0, 0, 2, 6), aColor, RectangleA.Rotation, new Vector2(2 / 2, 6 / 2), SpriteEffects.None, 0);

            aPositionAdjusted = new Rectangle(RectangleB.X + (RectangleB.Width / 2), RectangleB.Y + (RectangleB.Height / 2), RectangleB.Width, RectangleB.Height);
            spriteBatch.Draw(RectangleTexture, aPositionAdjusted, new Rectangle(0, 0, 2, 6), aColor, RectangleB.Rotation, new Vector2(2 / 2, 6 / 2), SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
