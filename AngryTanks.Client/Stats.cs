using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Client
{
    public class Stats : IComparable, IEquatable<Stats>
    {
        #region Properties

        public Int32 Wins
        {
            get;
            set;
        }

        public Int32 Losses
        {
            get;
            set;
        }

        public Int32 Teamkills
        {
            get;
            set;
        }

        public Int32 Score
        {
            get { return Wins - Losses - Teamkills; }
        }

        #endregion

        public Stats()
        {
            this.Wins      = 0;
            this.Losses    = 0;
            this.Teamkills = 0;
        }

        public static bool operator >(Stats x, Stats y)
        {
            if (x.Score > y.Score)
                return true;

            return false;
        }

        public static bool operator <(Stats x, Stats y)
        {
            return !(x > y);
        }

        public static bool operator ==(Stats x, Stats y)
        {
            if (x.Score == y.Score)
                return true;

            return false;
        }

        public static bool operator !=(Stats x, Stats y)
        {
            return !(x == y);
        }

        public override bool Equals(Object o)
        {
            if (o == null || this.GetType() != o.GetType())
                return false;

            return (this == (Stats)o);
        }

        public virtual bool Equals(Stats o)
        {
            if (o == null)
                return false;

            return (this == o);
        }

        public override int GetHashCode()
        {
            return this.Score;
        }

        public int CompareTo(Object o)
        {
            if (o is Stats)
            {
                Stats temp = (Stats)o;
                return this.Score.CompareTo(temp.Score);
            }

            throw new ArgumentException("object is not a Stats");
        }
    }
}
