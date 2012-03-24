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
    /*
     * Dummy class until Sprite refactor is complete
     * 
     */
    class StaticMapObject
    {
        String type;

        public StaticMapObject(Texture2D textureImage, 
                               float rotation,
                               Vector2 position, 
                               Vector2 scale_size, 
                               int collisionOffset,
                               int millisecondsPerFrame,
                               string type)
        {
             this.type = type;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            return;
        }
    }
}
