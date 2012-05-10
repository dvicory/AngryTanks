using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AngryTanks.Common;

namespace AngryTanks.Client
{
    public abstract class Sprite : IWorldObject
    {
        #region Properties

        protected World World
        {
            get;
            set;
        }

        protected virtual Texture2D Texture
        {
            get;
            set;
        }

        public virtual Vector2 Position
        {
            get;
            set;
        }

        public virtual Vector2 Size
        {
            get;
            set;
        }

        public virtual Single Rotation
        {
            get;
            set;
        }

        public virtual Color Color
        {
            get;
            set;
        }

        public virtual RotatedRectangle Bounds
        {
            get;
            protected set;
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

            this.Bounds = new RotatedRectangle(new RectangleF(this.Position - this.Size / 2, this.Size), this.Rotation);
        }

        public Sprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation, Color color)
        {
            this.World    = world;
            this.Texture  = texture;
            this.Position = position;
            this.Size     = size;
            this.Rotation = rotation;
            this.Color    = color;

            this.Bounds = new RotatedRectangle(new RectangleF(this.Position - this.Size / 2, this.Size), this.Rotation);
        }

        /// <summary>
        /// Tests for an intersection with <paramref name="sprite"/>.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public virtual bool Intersects(IWorldObject sprite)
        {
            Single overlap;
            Vector2 collisionProjection;
            return Bounds.Intersects(sprite.Bounds, out overlap, out collisionProjection);
        }

        /// <summary>
        /// Tests for an intersection with <paramref name="sprite"/>.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="overlap"></param>
        /// <param name="collisionProjection"></param>
        /// <returns></returns>
        public virtual bool Intersects(IWorldObject sprite, out Single overlap, out Vector2 collisionProjection)
        {
            return Bounds.Intersects(sprite.Bounds, out overlap, out collisionProjection);
        }

        /// <summary>
        /// Finds the nearest collision out of <paramref name="collisionObjects"/>.
        /// </summary>
        /// <param name="collisionObjects">List of objects to test against.</param>
        /// <returns></returns>
        public virtual bool FindNearestCollision(List<IWorldObject> collidableObjects)
        {
            Single overlap;
            Vector2 collisionProjection;
            IWorldObject collidingObject;

            return FindNearestCollision(collidableObjects, out overlap, out collisionProjection, out collidingObject);
        }

        /// <summary>
        /// Finds the nearest collision out of <paramref name="collisionObjects"/>.
        /// </summary>
        /// <param name="collisionObjects">List of objects to test against.</param>
        /// <param name="collidingObject"></param>
        /// <returns></returns>
        public virtual bool FindNearestCollision(List<IWorldObject> collidableObjects, out IWorldObject collidingObject)
        {
            Single overlap;
            Vector2 collisionProjection;

            return FindNearestCollision(collidableObjects, out overlap, out collisionProjection, out collidingObject);
        }

        /// <summary>
        /// Finds the nearest collision out of <paramref name="collisionObjects"/>.
        /// </summary>
        /// <param name="collisionObjects">List of objects to test against.</param>
        /// <param name="overlap"></param>
        /// <param name="collisionProjection"></param>
        /// <returns></returns>
        public virtual bool FindNearestCollision(List<IWorldObject> collidableObjects, out Single overlap, out Vector2 collisionProjection)
        {
            IWorldObject collidingObject;

            return FindNearestCollision(collidableObjects, out overlap, out collisionProjection, out collidingObject);
        }

        /// <summary>
        /// Finds the nearest collision out of <paramref name="collisionObjects"/>.
        /// </summary>
        /// <param name="collisionObjects">List of objects to test against.</param>
        /// <param name="overlap"></param>
        /// <param name="collisionProjection"></param>
        /// <param name="collidingObject"></param>
        /// <returns></returns>
        public virtual bool FindNearestCollision(List<IWorldObject> collidableObjects, out Single overlap, out Vector2 collisionProjection, out IWorldObject collidingObject)
        {
            Single largestOverlap = 0;
            Vector2 largestCollisionProjection = Vector2.Zero;
            IWorldObject foundCollidingObject = null;

            foreach (IWorldObject collidableObject in collidableObjects)
            {
                if (!Intersects(collidableObject, out overlap, out collisionProjection))
                    continue;

                if (overlap > largestOverlap)
                {
                    largestOverlap = overlap;
                    largestCollisionProjection = collisionProjection;
                    foundCollidingObject = collidableObject;
                }
            }

            // we found no collisions
            if (largestOverlap == 0)
            {
                overlap = 0;
                collisionProjection = Vector2.Zero;
                collidingObject = null;
                return false;
            }

            // we did find a collision otherwise, so assign variables
            overlap = largestOverlap;
            collisionProjection = largestCollisionProjection;
            collidingObject = foundCollidingObject;

            return true;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // TODO actually draw, revisit parameters
        }

        /// <summary>
        /// Draws <see cref="Sprite"/>'s Texture at the current Position and Size in stretched mode.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
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

        /// <summary>
        /// Draws <see cref="Sprite"/>'s Texture at the current Position and Size in tiled mode.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void DrawTiled(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pixelPosition = World.WorldUnitsToPixels(Position);
            Vector2 pixelSize = World.WorldUnitsToPixels(Size);

            spriteBatch.Draw(Texture,
                             (Rectangle)new RectangleF(pixelPosition, pixelSize),
                             // for TILE MODE, set source relative to the Size's dimensions
                             (Rectangle)new RectangleF(Vector2.Zero, pixelSize),
                             Color,
                             Rotation,
                             // for TILE MODE, set the origin relative to the Size's dimensions
                             new Vector2(pixelSize.X / 2, pixelSize.Y / 2),
                             SpriteEffects.None, 0);
        }

    }
}
