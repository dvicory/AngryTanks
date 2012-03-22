using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

using AngryTanks.Common;

namespace AngryTanks.Server
{
    class GameManager
    {
        private static SortedDictionary<Byte, Player> Players = new SortedDictionary<Byte,Player>();

        public static int AddPlayer(NetIncomingMessage msg)
        {
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

            return player_id;
        }

        public static Player GetPlayerByID(int id)
        {
            return Players[(byte)id];
        }
    }
}
