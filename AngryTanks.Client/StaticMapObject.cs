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
    /// This class represents non-moving, non-animated sprites by extending the Sprite Class. 
    /// ALL Animation-related parameters in the constructor are set to appropriate "do-nothing" 
    /// values. This way the Sprite class still supports animation and movement by default.
    /// 
    ///CLASS FIELD String type - this hold the name of the static object (box, pyramid)
    /// </summary>
    class StaticMapObject : Sprite
    {
        String type;

        public StaticMapObject(Texture2D textureImage, 
                               float rotation,
                               Vector2 position, 
                               Vector2 scale_size, 
                               int collisionOffset,
                               int millisecondsPerFrame,
                               string type)
            :base(textureImage, rotation, position, scale_size, /*frameSize*/ new Point(textureImage.Width, textureImage.Height),
        collisionOffset, /*currentFrame*/Point.Zero, /*sheetSize*/ Point.Zero, /*speed*/Vector2.Zero,
        millisecondsPerFrame)
        {
             this.type = type; 
        }
    }
}
