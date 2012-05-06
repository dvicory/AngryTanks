using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    public class Score : IComparable, IEquatable<Score>
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

        public Int32 Overall
        {
            get { return Wins - Losses - Teamkills; }
        }

        #endregion

        public Score()
        {
            this.Wins      = 0;
            this.Losses    = 0;
            this.Teamkills = 0;
        }

        public static bool operator >(Score x, Score y)
        {
            if (x.Overall > y.Overall)
                return true;

            return false;
        }

        public static bool operator <(Score x, Score y)
        {
            return !(x > y);
        }

        public static bool operator ==(Score x, Score y)
        {
            if (x.Overall == y.Overall)
                return true;

            return false;
        }

        public static bool operator !=(Score x, Score y)
        {
            return !(x == y);
        }

        public override bool Equals(Object o)
        {
            if (o == null || this.GetType() != o.GetType())
                return false;

            return (this == (Score)o);
        }

        public virtual bool Equals(Score o)
        {
            if (o == null)
                return false;

            return (this == o);
        }

        public override int GetHashCode()
        {
            return this.Overall;
        }

        public int CompareTo(Object o)
        {
            if (o is Score)
            {
                Score temp = (Score)o;
                return this.Overall.CompareTo(temp.Overall);
            }

            throw new ArgumentException("object is not a Score");
        }
    }
}
