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

        /// <summary>
        /// The first frame in the sprite sheet, depends on direction.
        /// </summary>
        public Point FirstFrame
        {
            get
            {
                if (Direction == SpriteSheetDirection.LeftToRight)
                    return Point.Zero;
                else if (Direction == SpriteSheetDirection.RightToLeft)
                    return new Point(maxFrames.X, 0);
                else
                    throw new ArgumentOutOfRangeException("direction", "Only left-to-right and right-to-left are supported at this time");
            }
        }

        /// <summary>
        /// The last frame in the sprite sheet, depends on direction.
        /// </summary>
        public Point LastFrame
        {
            get
            {
                if (Direction == SpriteSheetDirection.LeftToRight)
                    return MaxFrames;
                else if (Direction == SpriteSheetDirection.RightToLeft)
                    return new Point(0, MaxFrames.Y);
                else
                    throw new ArgumentOutOfRangeException();
            }
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

        private bool running = true;

        /// <summary>
        /// Determines if the animation is running.
        /// </summary>
        public bool Running
        {
            get { return running; }
            set
            {
                running = value;
                currentFrame = FirstFrame;
            }
        }

        /// <summary>
        /// Determines if the animation is done or not.
        /// </summary>
        public bool Done
        {
            get { return !Loop && (CurrentFrame == LastFrame); }
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

            this.currentFrame = FirstFrame;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Running)
                return;

            if (Direction == SpriteSheetDirection.LeftToRight)
            {
                if (!Loop && (CurrentFrame == LastFrame))
                    return;

                currentFrame.X++;

                // went past the right edge of the sprite sheet
                if (currentFrame.X > 0)
                {
                    currentFrame.X = 0;
                    currentFrame.Y++;

                    // did we go past the bottom edge?
                    if (currentFrame.Y > maxFrames.Y)
                    {
                        if (Loop)
                            currentFrame = FirstFrame;
                    }
                }
            }
            else if (Direction == SpriteSheetDirection.RightToLeft)
            {
                if (!Loop && (CurrentFrame == LastFrame))
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
                            currentFrame = FirstFrame;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Running)
                return;

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
