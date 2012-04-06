using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngryTanks.Client
{
    public class StaticSprite : Sprite
    {
        public StaticSprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(world, texture, position, size, rotation)
        {
        }

        public StaticSprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation, Color color)
            : base(world, texture, position, size, rotation, color)
        {
        }
    }
}
