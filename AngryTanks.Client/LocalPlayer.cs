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
        private float maxVelocity, velocityFactor;
        private float maxAngularVelocity, angularVelocityFactor;

        private float oldAngularVelocity, newAngularVelocity;

        public LocalPlayer(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(texture, position, size, rotation)
        {
            maxVelocity = 25f;
            maxAngularVelocity = (float)Math.PI / 2;
        }

        public override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.W))
            {
                /*  Basing my calculations on this:
                 *  Velocity.X = VelocityFactor * MaxVelocity.X * cos(Rotation)
                 *  Velocity.Y = VelocityFactor * MaxVelocity.X * sin(Rotation)
                 *  
                 *  OldVelocity = Velocity;
                 *  Position += (OldVelocity + Velocity) * 0.5 * dt;
                 *  
                 *  Fixed thanks to Daniel G.
                 */

                oldVelocity = newVelocity;

                velocityFactor = -1;

                newVelocity.X = velocityFactor * maxVelocity * (float)Math.Cos(Rotation + Math.PI / 2);
                newVelocity.Y = velocityFactor * maxVelocity * (float)Math.Sin(Rotation + Math.PI / 2);

                Position += (oldVelocity + newVelocity) * 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.S))
            {
                oldVelocity = newVelocity;

                velocityFactor = 1;

                newVelocity.X = velocityFactor * maxVelocity * (float)Math.Cos(Rotation + Math.PI / 2);
                newVelocity.Y = velocityFactor * maxVelocity * (float)Math.Sin(Rotation + Math.PI / 2);

                Position += (oldVelocity + newVelocity) * 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.A))
            {
                oldAngularVelocity = newAngularVelocity;

                angularVelocityFactor = -1;

                newAngularVelocity = angularVelocityFactor * maxAngularVelocity;

                Rotation += (newAngularVelocity + oldAngularVelocity) * 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.D))
            {
                oldAngularVelocity = newAngularVelocity;

                angularVelocityFactor = 1;

                newAngularVelocity = angularVelocityFactor * maxAngularVelocity;

                Rotation += (newAngularVelocity + oldAngularVelocity) * 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            drawStretched(gameTime, spriteBatch);
        }
    }
}