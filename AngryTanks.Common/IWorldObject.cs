using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngryTanks.Client;

namespace AngryTanks.Common
{
    interface IWorldObject
    {
        #region Properties

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
        public virtual RotatedRectangle RectangleBounds
        {
            get;
            protected set;
        }

        #endregion
    }
}
