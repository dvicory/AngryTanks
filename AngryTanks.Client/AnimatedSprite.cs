using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AngryTanks.Common;

namespace AngryTanks.Client
{
    public enum SpriteSheetDirection
    {
        LeftToRight,
        RightToLeft
    }

    public class AnimatedSprite : StaticSprite
    {
        private Point currentFrame;

        /// <summary>
        /// The current animation frame.
        /// </summary>
        public Point CurrentFrame
        {
            get { return currentFrame; }
        }

        private Point maxFrames;

        /// <summary>
        /// The maximum number of frames in the sprite sheet.
        /// </summary>
        public Point MaxFrames
        {
            get { return maxFrames; }
        }

        private Point frameSize;

        /// <summary>
        /// The size of each frame in the sprite sheet.
        /// </summary>
        public Point FrameSize
        {
            get { return frameSize; }
        }

        private SpriteSheetDirection direction;

        public SpriteSheetDirection Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// Determines whether the animation loops or not.
        /// </summary>
        public bool Loop
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if the animation is done or not.
        /// </summary>
        public bool Done
        {
            get { return !Loop && (CurrentFrame == MaxFrames); }
        }

        public AnimatedSprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation, Point maxFrames, Point frameSize, SpriteSheetDirection direction, bool loop)
            : this(world, texture, position, size, rotation, Color.White, maxFrames, frameSize, direction, loop)
        { }

        public AnimatedSprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation, Color color, Point maxFrames, Point frameSize, SpriteSheetDirection direction, bool loop)
            : base(world, texture, position, size, rotation, color)
        {
            this.maxFrames = maxFrames;
            this.frameSize = frameSize;
            this.direction = direction;
            this.Loop = loop;

            if (Direction == SpriteSheetDirection.LeftToRight)
                this.currentFrame = Point.Zero;
            else if (Direction == SpriteSheetDirection.RightToLeft)
                this.currentFrame = new Point(maxFrames.X, 0);
            else
                throw new ArgumentOutOfRangeException("direction", "Only left-to-right and right-to-left are supported at this time");
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Direction == SpriteSheetDirection.LeftToRight)
            {
                throw new NotImplementedException();
            }
            else if (Direction == SpriteSheetDirection.RightToLeft)
            {
                if (!Loop && (CurrentFrame == new Point(0, MaxFrames.Y)))
                    return;

                currentFrame.X--;

                // went past the left edge of the sprite sheet
                if (currentFrame.X < 0)
                {
                    currentFrame.X = maxFrames.X;
                    currentFrame.Y++;

                    // did we go past the bottom edge?
                    if (currentFrame.Y > maxFrames.Y)
                    {
                        if (Loop)
                        {
                            currentFrame.X = maxFrames.X;
                            currentFrame.Y = 0;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixelPosition = World.WorldUnitsToPixels(Position);
            Vector2 pixelSize = World.WorldUnitsToPixels(Size);

            spriteBatch.Draw(Texture,
                             (Rectangle)new RectangleF(pixelPosition, pixelSize),
                             // for STRETCH MODE, set source relative to the texture's dimensions
                             new Rectangle(
                                 CurrentFrame.X * FrameSize.X,
                                 CurrentFrame.Y * FrameSize.Y,
                                 FrameSize.X, FrameSize.Y),
                             Color,
                             Rotation,
                             // for STRETCH MODE, set the origin relative to the texture's dimensions
                             new Vector2(FrameSize.X / 2, FrameSize.Y / 2),
                             SpriteEffects.None, 0);
        }
    }
}
