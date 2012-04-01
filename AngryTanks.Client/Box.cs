using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using log4net;

namespace AngryTanks.Client
{
    class Box : StaticSprite
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Box(Texture2D texture, Vector2 position, Vector2 size, Double rotation)
            : base(texture, position, size, rotation)
        {
        }

        public Box(Texture2D texture, Vector2 position, Vector2 size, Double rotation, Color color)
            : base(texture, position, size, rotation)
        {
        }        

        /*
         * TODO:
         * 1) check to make sure this is the correct box model
         *  
         * ** problem 2) is FIXED - I think**
         * 2) currently the source rectangle must be the full size (you can shift it so it starts in negative
         *    but it still must be the full size). another issue is that you must be in Immediate SpriteSortMode
         *    and Wrap TextureAddressMode. this means that your textures tile if the size is greater than the
         *    texture size. this is undesirable for pretty much everything besides box. we already have a nice
         *    base texture that we don't want to tile. obviously tanks and shots should not tile either. these
         *    should all stretch. there must be a way to remedy this, but I'm afraid the camera may be at fault.
         *    possibly, with enough playing with the rectangles, one could devise a way to allow for stretching.
         * 3) scaling world units -> pixels. this should probably be done in camera (if it is kept) and/or
         *    some resolution helper class.
         * 4) decide how to store rotations, in the map it is degrees. but here we need a float. do we convert
         *    at parsing?
         * 5) should probably allow colors and other options for objects at instantiation and/or draw time
         * 
         * This is how Box should be drawn
         * 
         *    <--Size.X-->    |
         *  ^ *----------*    y
         *  | |          |    |
         *  S |          |    |
         *  i |          |    |
         *  z |    *     |    |
         *  e |     \    |    |
         *  . |      \   |    |
         *  Y |       \  |    |
         *  | |        \ |    |
         *  ^ *---------\*    |
         *               \    |
         *                \   |
         *    Position --> \  |
         *                  \ |
         *                   \|
         * <- -x -------------------------- x ->
         *                    |
         *                    |
         *                   -y
         *                    |
         * 
         */
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            drawTiled(gameTime, spriteBatch);
        }        
    }
}
