using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngryTanks.Common
{
    public interface IWorldObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        Vector2 Position
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        Vector2 Size
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        Single Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        RotatedRectangle Bounds
        {
            get;
        }

        #endregion
    }

    public class WorldObject : IWorldObject
    {
        #region Properties

        public Vector2 Position
        {
            get;
            set;
        }

        public Vector2 Size
        {
            get;
            set;
        }

        public Single Rotation
        {
            get;
            set;
        }
        public RotatedRectangle Bounds
        {
            get;
            set;
        }

        #endregion

        public WorldObject(Vector2 position, Vector2 size, Single rotation)
        {
            this.Position = position;
            this.Size = size;
            this.Rotation = rotation;
            this.Bounds = new RotatedRectangle(new RectangleF(this.Position - this.Size / 2, this.Size),
                                                        this.Rotation);
        }

        public void ReCalcBounds()
        {
            this.Bounds = new RotatedRectangle(new RectangleF(this.Position - this.Size / 2, this.Size),
                                                        this.Rotation);
        }
    }
}
