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
        
        public LocalPlayer(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(texture, position, size, rotation)
        {
            newVelocity = position;
            maxVelocity = 1.0f;
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

                velocityFactor = -1;
                newVelocity.X = velocityFactor * maxVelocity * (float)Math.Cos(Rotation + Math.PI / 2);
                newVelocity.Y = velocityFactor * maxVelocity * (float)Math.Sin(Rotation + Math.PI / 2);

                oldVelocity = newVelocity;
                Position += (oldVelocity + newVelocity) * 0.5f;
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                velocityFactor = 1;
                newVelocity.X = velocityFactor * maxVelocity * (float)Math.Cos(Rotation + Math.PI / 2);
                newVelocity.Y = velocityFactor * maxVelocity * (float)Math.Sin(Rotation + Math.PI / 2);

                oldVelocity = newVelocity;
                Position += (oldVelocity + newVelocity) * 0.5f;
            }
            
            if (kb.IsKeyDown(Keys.A))
                Rotation -= 0.1f;
            if (kb.IsKeyDown(Keys.D))
                Rotation += 0.1f;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            spriteBatch.Draw(Texture,
                                Position,
                                null,
                                Color,
                                (float)Rotation,
                                Size,
                                1f,
                                SpriteEffects.None,
                                0f
                                );

            spriteBatch.End();
        }
    }
}