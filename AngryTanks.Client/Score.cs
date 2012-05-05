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
            get {return Wins - Losses - TeamKills;}            
        }        

        #endregion
    }
}
