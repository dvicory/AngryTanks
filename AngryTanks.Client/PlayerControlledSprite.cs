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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PlayerControlledSprite : DynamicSprite
    {
        KeyboardState kb;
        float maxVelocity, velocityFactor;
        

        public PlayerControlledSprite(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(texture, position, size, rotation)
        {
            this.Size = new Vector2(Texture.Width / 2, Texture.Height / 2);
            
            NewVelocity = position;
            maxVelocity = 1.0f;
        }

       

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.W))
            {
                /*  Basing my calcualtion on this:
                 *  Velocity.X = VelocityFactor * MaxVelocity.X * cos(Rotation)
                 *  Velocity.Y = VelocityFactor * MaxVelocity.X * sin(Rotation)
                 *  
                 *  OldVelocity = Velocity;
                 *  Position += (OldVelocity + Velocity) * 0.5 * dt;
                 *  
                 *  But.. the tanks move to the right, not forward.
                 */

                velocityFactor = 1;
                NewVelocity.X = velocityFactor * maxVelocity * (float)Math.Cos(Rotation);
                NewVelocity.Y = velocityFactor * maxVelocity * (float)Math.Sin(Rotation);

                Position += (OldVelocity + NewVelocity) * 0.5f;
                OldVelocity = NewVelocity;
            }
            //else if (kb.IsKeyDown(Keys.S))
                


            if (kb.IsKeyDown(Keys.A))
                Rotation -= 0.1f;
            else if (kb.IsKeyDown(Keys.D))
                Rotation += 0.1f;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            spriteBatch.Draw(Texture,
                                Position,
                                null,
                                Color.White,
                                Rotation,
                                Size,
                                1f,
                                SpriteEffects.None,
                                0f
                                );

            spriteBatch.End();
        }
    }
}