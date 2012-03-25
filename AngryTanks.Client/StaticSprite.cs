using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngryTanks.Client
{
    public class StaticSprite : Sprite
    {
        public StaticSprite(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(texture, position, size, rotation)
        {
        }
    }
}
