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
            get;
            set;
        }

        protected virtual Vector2 Position
        {
            get;
            set;
        }

        protected virtual Vector2 Size
        {
            get;
            set;
        }

        protected virtual Single Rotation
        {
            get;
            set;
        }

        #endregion

        public Sprite(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
        {
            this.Texture  = texture;
            this.Position = position;
            this.Size     = size;
            this.Rotation = rotation;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // TODO actually draw, revisit parameters
        }
    }
}
