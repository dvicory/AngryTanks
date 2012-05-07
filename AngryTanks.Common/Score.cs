using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    public class Score : IComparable, IEquatable<Score>
    {
        #region Properties

        public Int32 Overall
        {
            get { return Wins - Losses - Teamkills; }
        }

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

        #endregion

        public Score()
        {
            this.Wins      = 0;
            this.Losses    = 0;
            this.Teamkills = 0;
        }

        public static bool operator >(Score a, Score b)
        {
            if (((Object)a == null) || ((Object)b == null))
                return false;

            return a.Overall > b.Overall;
        }

        public static bool operator <(Score a, Score b)
        {
            return !(a > b);
        }

        public static bool operator >=(Score a, Score b)
        {
            if (((Object)a == null) || ((Object)b == null))
                return false;

            return a.Overall >= b.Overall;
        }

        public static bool operator <=(Score a, Score b)
        {
            if (((Object)a == null) || ((Object)b == null))
                return false;

            return a.Overall <= b.Overall;
        }

        public static bool operator ==(Score a, Score b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((Object)a == null) || ((Object)b == null))
                return false;

            return a.Wins      == b.Wins &&
                   a.Losses    == b.Losses &&
                   a.Teamkills == b.Teamkills;
        }

        public static bool operator !=(Score a, Score b)
        {
            return !(a == b);
        }

        public override bool Equals(Object o)
        {
            if (o == null || this.GetType() != o.GetType())
                return false;

            return this == (Score)o;
        }

        public virtual bool Equals(Score o)
        {
            if (o == null)
                return false;

            return this == o;
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
