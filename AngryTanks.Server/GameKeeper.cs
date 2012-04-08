using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Server
{
    class GameKeeper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SortedDictionary<Byte, Player> Players = new SortedDictionary<Byte, Player>();

        public static Int16 PlayerCount
        {
            get
            {
                return (Int16)Players.Count;
            }
        }

        public static Int16 AddPlayer(NetIncomingMessage msg)
        {
            // TODO check for duplicate callsigns

            Int16 lastID;
            Int16 curID    = -1;
            Int16 playerID = -1;

            foreach (KeyValuePair<Byte, Player> entry in Players)
            {
                lastID = curID;
                curID  = (Int16)entry.Key;

                // did we hit the max? 
                if (curID >= ProtocolInformation.MaxPlayers)
                    return -1;

                // there was a gap between this current id and the last id
                if ((curID - lastID) > 1)
                {
                    // we now know that last id + 1 must be free
                    playerID = (Int16)(lastID + 1);
                    Players[(Byte)playerID] = new Player((Byte)playerID, msg);
                    break;
                }
            }

            // if curID never made it up to the max, then we know we can add a slot in at curID + 1
            if (curID < ProtocolInformation.MaxPlayers)
            {
                playerID = (Int16)(curID + 1);
                Players[(Byte)playerID] = new Player((Byte)playerID, msg);
            }

            return playerID;
        }

        // TODO be able to remove player

        public static Player GetPlayerByID(Byte id)
        {
            return Players[id];
        }

        public static Player GetPlayerByConnection(NetConnection conn)
        {
            return Players.Values.First(player => player.Connection == conn);
        }
    }
}
