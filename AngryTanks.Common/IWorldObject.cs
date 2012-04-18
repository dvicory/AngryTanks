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
}
