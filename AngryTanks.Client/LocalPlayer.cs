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

namespace AngryTanks.Client
{
    // TODO inherit from more generic Player
    public class LocalPlayer : DynamicSprite
    {
        private KeyboardState kb;

        private Single maxVelocity, velocityFactor;
        private Single maxAngularVelocity, angularVelocityFactor;

        public LocalPlayer(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(texture, position, size, rotation)
        {
            maxVelocity = 25f;
            maxAngularVelocity = (Single)Math.PI / 2;
        }

        public LocalPlayer(Texture2D texture, Vector2 position, Vector2 size, Single rotation, Color color)
            : base(texture, position, size, rotation, color)
        {
            maxVelocity = 25f;
            maxAngularVelocity = (Single)Math.PI / 2;
        }

        public override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();

            /*  Basing my calculations on this:
             *  Velocity.X = VelocityFactor * MaxVelocity.X * cos(Rotation)
             *  Velocity.Y = VelocityFactor * MaxVelocity.X * sin(Rotation)
             *  
             *  OldVelocity = Velocity;
             *  Position += (OldVelocity + Velocity) * 0.5 * dt;
             *  
             *  Fixed thanks to Daniel G.
             */

            if (kb.IsKeyDown(Keys.W))
                velocityFactor = 1;
            if (kb.IsKeyDown(Keys.S))
                velocityFactor = -1;
            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.S))
                velocityFactor = 0;

            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.S))
            {
                oldVelocity = newVelocity;

                newVelocity.X = velocityFactor * maxVelocity * (Single)Math.Cos(Rotation - Math.PI / 2);
                newVelocity.Y = velocityFactor * maxVelocity * (Single)Math.Sin(Rotation - Math.PI / 2);

                Velocity = (oldVelocity + newVelocity) * 0.5f;
                Position += Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.A))
                angularVelocityFactor = -1;
            if (kb.IsKeyDown(Keys.D))
                angularVelocityFactor = 1;
            if (kb.IsKeyDown(Keys.A) && kb.IsKeyDown(Keys.D))
                angularVelocityFactor = 0;

            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.D))
            {
                oldAngularVelocity = newAngularVelocity;

                newAngularVelocity = angularVelocityFactor * maxAngularVelocity;

                AngularVelocity = MathHelper.WrapAngle((Single)(newAngularVelocity + oldAngularVelocity) * 0.5f);
                Rotation += AngularVelocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawStretched(gameTime, spriteBatch);
        }
    }
}