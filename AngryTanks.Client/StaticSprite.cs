using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngryTanks.Client
{
    public class StaticSprite : Sprite
    {

        #region Properties

        protected override Vector2 Position
        {
            get
            {
                return Position;
            }
        }

        protected override Vector2 Size
        {
            get
            {
                return Size;
            }
        }

        protected override Single Rotation
        {
            get
            {
                return Rotation;
            }
        }

        #endregion

        public StaticSprite(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(texture, position, size, rotation)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            // TODO will we ever need Update in a StaticSprite?
        }

        public virtual void Draw(GameTime gameTime)
        {
            // TODO actually draw, revisit parameters
        }
    }
}
