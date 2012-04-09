using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Server
{
    public class GameKeeper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SortedDictionary<Byte, Player> players = new SortedDictionary<Byte, Player>();

        #region GameKeeper Properties

        public static List<Player> Players
        {
            get { return players.Values.ToList(); }
        }

        public static Int16 PlayerCount
        {
            get
            {
                return (Int16)players.Count;
            }
        }

        #endregion

        public static void HandleIncomingData(NetIncomingMessage incomingMessage)
        {
            Player player = GetPlayerByConnection(incomingMessage.SenderConnection);

            if (player != null)
                player.HandleData(incomingMessage);
        }

        public static void HandleStatusChange(NetIncomingMessage incomingMessage)
        {
            NetConnectionStatus status = (NetConnectionStatus)incomingMessage.ReadByte();

            switch (status)
            {
                case NetConnectionStatus.Disconnecting:
                case NetConnectionStatus.Disconnected:
                    {
                        Player player = GetPlayerByConnection(incomingMessage.SenderConnection);

                        if (player != null && player.State != PlayerState.Leaving)
                            RemovePlayer(player, incomingMessage.ReadString());

                        break;
                    }
            }
        }

        public static void AddPlayer(NetConnection connection, PlayerInformation playerInfo)
        {
            Byte slot = FindSlot();

            // game is full if FindSlot returns dummy slot (255)
            if (slot == ProtocolInformation.DummySlot)
            {
                Log.InfoFormat("Player \"{0}\" from {1} tried to join, but the game was full.",
                               playerInfo.Callsign, connection);
                connection.Deny("the game is currently full");
                return;
            }

            // we can now approve the player if we get here
            connection.Approve();

            // add player to our list
            players[slot] = new Player(slot, connection, playerInfo);

            // and tell everyone else about this awesome new player
            Log.DebugFormat("Sending MsgAddPlayer to everyone else about player #{0}", slot);

            /*
            NetOutgoingMessage msgAddPlayer = Program.Server.CreateMessage();
            msgAddPlayer.Write((Byte)MessageType.MsgAddPlayer);
            msgAddPlayer.Write(slot);
            msgAddPlayer.Write((Byte)playerInfo.Team);
            msgAddPlayer.Write(playerInfo.Callsign);
            msgAddPlayer.Write(playerInfo.Tag);
            */

            NetOutgoingMessage packet = Program.Server.CreateMessage();

            MsgAddPlayerPacket message =
                new MsgAddPlayerPacket(new PlayerInformation(slot, playerInfo.Callsign, playerInfo.Tag, playerInfo.Team));

            packet.Write((Byte)message.MsgType);
            message.Write(packet);

            Log.DebugFormat("MsgAddPlayer Compiled ({0} bytes) and being sent to {1} recipients",
                            packet.LengthBytes, players.Count - 1);

            // send to everyone except our new player
            Program.Server.SendToAll(packet, connection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public static void RemovePlayer(Player player, String reason)
        {
            Log.InfoFormat("Removing player #{0} ({1})", player.Slot, reason);

            // nuke player from the dictionary
            players.Remove(player.Slot);

            // now let's tell all the other players the dude left
            /*
            NetOutgoingMessage msgRemovePlayer = Program.Server.CreateMessage();
            msgRemovePlayer.Write((Byte)MessageType.MsgRemovePlayer);
            msgRemovePlayer.Write(player.Slot);
            msgRemovePlayer.Write(reason);
            */

            NetOutgoingMessage packet = Program.Server.CreateMessage();

            MsgRemovePlayerPacket message =
                new MsgRemovePlayerPacket(player.Slot, reason);

            packet.Write((Byte)message.MsgType);
            message.Write(packet);

            // send to all
            Program.Server.SendToAll(packet, null, NetDeliveryMethod.ReliableOrdered, 0);
            
            // disposing of player would be a good idea
            if (player.Connection != null)
                player.Connection.Disconnect(reason);
        }

        private static Byte FindSlot()
        {
            // TODO check for duplicate callsigns
            Int16 lastSlot;
            Int16 curSlot    = -1;
            Int16 playerSlot = -1;

            foreach (KeyValuePair<Byte, Player> entry in players)
            {
                lastSlot = curSlot;
                curSlot  = (Int16)entry.Key;

                // did we hit the max? 
                if (curSlot >= ProtocolInformation.MaxPlayers)
                    return ProtocolInformation.DummySlot;

                // there was a gap between this current slot and the last slot
                if ((curSlot - lastSlot) > 1)
                {
                    // we now know that last Slot + 1 must be free
                    playerSlot = (Byte)(lastSlot + 1);
                    return (Byte)playerSlot;
                }
            }

            // if curSlot never made it up to the max, then we know we can add a slot in at curSlot + 1
            if (curSlot < ProtocolInformation.MaxPlayers)
                playerSlot = (Byte)(curSlot + 1);

            return (Byte)playerSlot;
        }

        public static Player GetPlayerBySlot(Byte Slot)
        {
            return players[Slot];
        }

        /// <summary>
        /// Gets the <see cref="Player"/> associated with a certain instance of <see cref="NetConnection"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns><see cref="Player"/>, if one found, otherwise null.</returns>
        public static Player GetPlayerByConnection(NetConnection connection)
        {
            try
            {
                return players.Values.First(player => player.Connection == connection);
            }
            // that connection doesn't exist...
            catch (InvalidOperationException e)
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
                return null;
            }
        }
    }
}
