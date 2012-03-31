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
            get;
            set;
        }

        #endregion

        public Sprite(Texture2D texture, Vector2 position, Vector2 size, Double rotation)
        {
            this.Texture  = texture;
            this.Position = position;
            this.Size     = size;
            this.Rotation = rotation;
            this.Color = Color.White;
        }

        public Sprite(Texture2D texture, Vector2 position, Vector2 size, Double rotation, Color color)
        {
            this.Texture = texture;
            this.Position = position;
            this.Size = size;
            this.Rotation = rotation;
            this.Color = color;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // TODO actually draw, revisit parameters
        }

        /* drawStretched()
         * 
         * Call this function to draw sprites that should be stretched.           
         */
        public void drawStretched(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixel_position = Position * World.worldToPixel;
            Vector2 pixel_size = Size * World.worldToPixel;
            spriteBatch.Draw(Texture,
                             new Rectangle((int)pixel_position.X,
                                           (int)pixel_position.Y,
                                           (int)pixel_size.X,
                                           (int)pixel_size.Y),
                //For STRETCH MODE Set source relative to the texture dimensions
                             new Rectangle(0, 0, (int)Texture.Width, (int)Texture.Height),
                             Color,
                             (float)Rotation,
                //For STRETCH MODE Set the origin relative to the texture's dimensions
                             new Vector2(Texture.Width / 2, Texture.Height / 2),
                             SpriteEffects.None, 0);
        }

        /* drawTiled()
         * 
         * Call this function to draw sprites that should be tiled.           
         */
        public void drawTiled(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixel_position = Position * World.worldToPixel;
            Vector2 pixel_size = Size * World.worldToPixel;
            spriteBatch.Draw(Texture,
                             new Rectangle((int)pixel_position.X,
                                           (int)pixel_position.Y,
                                           (int)pixel_size.X,
                                           (int)pixel_size.Y),
                //For TILE MODE Set source relative to the Size's dimensions
                             new Rectangle(0, 0, (int)pixel_size.X, (int)pixel_size.Y),
                             Color,
                             (float)Rotation,
                //For TILE MODE Set the origin relative to the Size's dimensions
                             new Vector2(pixel_size.X / 2, pixel_size.Y / 2),
                             SpriteEffects.None, 0);
        }

    }
}
