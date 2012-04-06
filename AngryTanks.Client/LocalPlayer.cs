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

namespace AngryTanks.Client
{
    // TODO inherit from more generic Player
    public class LocalPlayer : DynamicSprite
    {
        private KeyboardState kb;

        private Single velocityFactor;
        private Single angularVelocityFactor;

        public LocalPlayer(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(world, texture, position, size, rotation)
        {
        }

        public LocalPlayer(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation, Color color)
            : base(world, texture, position, size, rotation, color)
        {
        }

        public override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();

            Single maxVelocity = (Single)World.VarDB["tankSpeed"].Value;
            Single maxAngularVelocity = (Single)World.VarDB["tankAngVel"].Value;

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
                oldPosition = newPosition;

                newVelocity.X = velocityFactor * maxVelocity * (Single)Math.Cos(Rotation - Math.PI / 2);
                newVelocity.Y = velocityFactor * maxVelocity * (Single)Math.Sin(Rotation - Math.PI / 2);

                Velocity = (oldVelocity + newVelocity) * 0.5f;

                newPosition += Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
                Position = newPosition;
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
                oldRotation = newRotation;

                newAngularVelocity = angularVelocityFactor * maxAngularVelocity;

                AngularVelocity = MathHelper.WrapAngle((Single)(newAngularVelocity + oldAngularVelocity) * 0.5f);

                newRotation += AngularVelocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
                Rotation = newRotation;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawStretched(gameTime, spriteBatch);
        }
    }
}