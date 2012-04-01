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
    public class Shot : DynamicSprite
    {
        
        public Shot(Texture2D texture, Vector2 position, Vector2 size, Single rotation, Vector2 velocity)
            : base(texture, position, size, rotation)
        {
            // TODO: Construct any child components here
        }
        
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            // Shell should be constantly moving in the direction the tank was facing when it fired it 
            // until it hits an object

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