using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Client
{
    public class Score
    {
        #region Properties

        public byte Wins
        {
            get;
            set;
        }

        public byte Losses
        {
            get;
            set;
        }

        public byte TeamKills
        {
            get;
            set;
        }

        public Int32 Score
        {
            get { return Wins - Losses - TeamKills; }
        }

        #endregion

        public Score()
        {
            Wins = 0;
            Losses = 0;
            TeamKills = 0;
        }

        public bool operator >(Score x, Score y)
        {
            if (x.Score > y.Score)
                return true;
            return false;
        }

        public bool operator <(Score x, Score y)
        {
            if (x.Score < y.Score)
                return true;
            return false;
        }

        public bool operator ==(Score x, Score y)
        {
            if (x.Score == y.Score)
                return true;
            return false;
        }

        public bool operator !=(Score x, Score y)
        {
            if (x.Score != y.Score)
                return true;
            return false;
        }

        public override bool Equals(System.Object o)
        {
            if (o == null || this.GetType() != o.GetType())
                return false;
            return (this.Score == ((Score)o).Score);
        }

        public override int GetHashCode()
        {
            return (this.Score);
        }
    }


}
