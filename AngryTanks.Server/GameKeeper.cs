using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;

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

            Int16 last_id;
            Int16 cur_id    = -1;
            Int16 player_id = -1;

            foreach (KeyValuePair<Byte, Player> entry in Players)
            {
                last_id = cur_id;
                cur_id  = (Int16)entry.Key;

                // did we hit the max? 
                if (cur_id >= Protocol.MaxPlayers)
                    return -1;

                // there was a gap between this current id and the last id
                if ((cur_id - last_id) > 1)
                {
                    // we now know that last id + 1 must be free
                    player_id = (Int16)(last_id + 1);
                    Players[(Byte)player_id] = new Player((Byte)player_id, msg);
                    break;
                }
            }

            // if cur_id never made it up to the max, then we know we can add a slot in at cur_id + 1
            if (cur_id < Protocol.MaxPlayers)
            {
                player_id = (Int16)(cur_id + 1);
                Players[(Byte)player_id] = new Player((Byte)player_id, msg);
            }

            return player_id;
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
