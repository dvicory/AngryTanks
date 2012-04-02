using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngryTanks.Client
{
    public abstract class Sprite
    {

        #region Properties

        protected virtual Texture2D Texture
        {
            get; set;
        }

        protected virtual Vector2 Position
        {
            get; set;
        }

        protected virtual Vector2 Size
        {
            get; set;
        }

        protected virtual Double Rotation
        {
            get; set;
        }

        protected virtual Color Color
        {
            get; set;
        }

        #endregion

        public Sprite(Texture2D texture, Vector2 position, Vector2 size, Double rotation)
        {
            this.Texture  = texture;
            this.Position = position;
            this.Size     = size;
            this.Rotation = rotation;
            this.Color    = Color.White;
        }

        public Sprite(Texture2D texture, Vector2 position, Vector2 size, Double rotation, Color color)
        {
            this.Texture  = texture;
            this.Position = position;
            this.Size     = size;
            this.Rotation = rotation;
            this.Color    = color;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // TODO actually draw, revisit parameters
        }

        /* 
         * drawStretched()
         * 
         * Call this function to draw sprites that should be stretched.           
         */
        public void drawStretched(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixelPosition = Position * World.worldToPixel;
            Vector2 pixelSize = Size * World.worldToPixel;

            spriteBatch.Draw(Texture,
                             new Rectangle((int)pixelPosition.X,
                                           (int)pixelPosition.Y,
                                           (int)pixelSize.X,
                                           (int)pixelSize.Y),
                             // for STRETCH MODE, set source relative to the texture's dimensions
                             new Rectangle(0, 0, (int)Texture.Width, (int)Texture.Height),
                             Color,
                             (float)Rotation,
                             // for STRETCH MODE, set the origin relative to the texture's dimensions
                             new Vector2(Texture.Width / 2, Texture.Height / 2),
                             SpriteEffects.None, 0);
        }

        /* 
         * drawTiled()
         * 
         * Call this function to draw sprites that should be tiled.           
         */
        public void drawTiled(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixelPosition = Position * World.worldToPixel;
            Vector2 pixelSize = Size * World.worldToPixel;

            spriteBatch.Draw(Texture,
                             new Rectangle((int)pixelPosition.X,
                                           (int)pixelPosition.Y,
                                           (int)pixelSize.X,
                                           (int)pixelSize.Y),
                             // for TILE MODE, set source relative to the Size's dimensions
                             new Rectangle(0, 0, (int)pixelSize.X, (int)pixelSize.Y),
                             Color,
                             (float)Rotation,
                             // for TILE MODE, set the origin relative to the Size's dimensions
                             new Vector2(pixelSize.X / 2, pixelSize.Y / 2),
                             SpriteEffects.None, 0);
        }

    }
}
