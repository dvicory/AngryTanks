using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AngryTanks.Common;

namespace AngryTanks.Client
{
    public abstract class Sprite
    {
        #region Properties

        protected World World
        {
            get; set;
        }

        protected virtual Texture2D Texture
        {
            get; set;
        }

        public virtual Vector2 Position
        {
            get; set;
        }

        public virtual Vector2 Size
        {
            get; set;
        }

        public virtual Single Rotation
        {
            get; set;
        }

        public virtual Color Color
        {
            get; set;
        }

        public virtual RotatedRectangle RectangleBounds
        {
            get; protected set;
        }

        public virtual bool Collided
        {
            get; set;
        }

        #endregion

        public Sprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation)
        {
            this.World    = world;
            this.Texture  = texture;
            this.Position = position;
            this.Size     = size;
            this.Rotation = rotation;
            this.Color    = Color.White;

            this.RectangleBounds = new RotatedRectangle(new RectangleF(this.Position - this.Size / 2, this.Size),
                                                        this.Rotation);
        }

        public Sprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation, Color color)
        {
            this.World    = world;
            this.Texture  = texture;
            this.Position = position;
            this.Size     = size;
            this.Rotation = rotation;
            this.Color    = color;

            this.RectangleBounds = new RotatedRectangle(new RectangleF(this.Position - this.Size / 2, this.Size),
                                                        this.Rotation);
        }

        public virtual bool Intersects(Sprite sprite)
        {
            Single overlap;
            Vector2 collisionProjection;
            return RectangleBounds.Intersects(sprite.RectangleBounds, out overlap, out collisionProjection);
        }

        public virtual bool Intersects(Sprite sprite, out Single overlap, out Vector2 collisionProjection)
        {
            return RectangleBounds.Intersects(sprite.RectangleBounds, out overlap, out collisionProjection);
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
        public void DrawStretched(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixelPosition = World.WorldUnitsToPixels(Position);
            Vector2 pixelSize = World.WorldUnitsToPixels(Size);

            spriteBatch.Draw(Texture,
                             (Rectangle)new RectangleF(pixelPosition, pixelSize),
                             // for STRETCH MODE, set source relative to the texture's dimensions
                             new Rectangle(0, 0, (int)Texture.Width, (int)Texture.Height),
                             Color,
                             Rotation,
                             // for STRETCH MODE, set the origin relative to the texture's dimensions
                             new Vector2(Texture.Width / 2, Texture.Height / 2),
                             SpriteEffects.None, 0);
        }

        /* 
         * drawTiled()
         * 
         * Call this function to draw sprites that should be tiled.           
         */
        public void DrawTiled(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixelPosition = World.WorldUnitsToPixels(Position);
            Vector2 pixelSize = World.WorldUnitsToPixels(Size);

            spriteBatch.Draw(Texture,
                             (Rectangle)new RectangleF(pixelPosition, pixelSize),
                             // for TILE MODE, set source relative to the Size's dimensions
                             new Rectangle(0, 0, (int)pixelSize.X, (int)pixelSize.Y),
                             Color,
                             Rotation,
                             // for TILE MODE, set the origin relative to the Size's dimensions
                             new Vector2(pixelSize.X / 2, pixelSize.Y / 2),
                             SpriteEffects.None, 0);
        }

    }
}
