using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngryTanks.Common
{
    public class RectangleF
    {
        #region Properties

        public virtual Single X
        {
            get;
            set;
        }

        public virtual Single Y
        {
            get;
            set;
        }

        public virtual Single Width {
            get;
            set;
        }

        public virtual Single Height
        {
            get;
            set;
        }

        public Single Top
        {
            get { return Y; }
        }

        public Single Bottom
        {
            get { return Y + Height; }
        }

        public Single Left
        {
            get { return X; }
        }

        public Single Right
        {
            get { return X + Width; }
        }

        #endregion

        public RectangleF(Single x, Single y, Single width, Single height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public RectangleF(Vector2 position, Vector2 size)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

        public static explicit operator Rectangle(RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
    }
}
