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
            String denyReason;
            Byte slot = AllocateSlot(playerInfo, out denyReason);

            // could not allocate the player if AllocateSlot returns DummySlot
            if (slot == ProtocolInformation.DummySlot)
            {
                Log.InfoFormat("Player \"{0}\" from {1} tried to join, but was rejected ({2}).",
                               playerInfo.Callsign, connection, denyReason);
                connection.Deny(denyReason);
                return;
            }

            // we can now approve the player if we get here
            connection.Approve();

            // add player to our list
            players[slot] = new Player(slot, connection, playerInfo);

            // and tell everyone else about this awesome new player
            Log.DebugFormat("Sending MsgAddPlayer to everyone else about player #{0}", slot);

            NetOutgoingMessage packet = Program.Server.CreateMessage();

            MsgAddPlayerPacket message = new MsgAddPlayerPacket(players[slot].PlayerInfo);

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

        private static Byte AllocateSlot(PlayerInformation playerAdding, out String denyReason)
        {
            Player player;
            Byte earliestSlot = ProtocolInformation.DummySlot;

            for (Byte i = 0; i < ProtocolInformation.MaxPlayers; ++i)
            {
                // we found a player at this slot
                if (players.TryGetValue(i, out player))
                {
                    // check if they're the same callsign
                    if (playerAdding.Callsign == player.Callsign)
                    {
                        // found a duplicate callsign, no good
                        denyReason = "callsign is already in use";
                        return ProtocolInformation.DummySlot;
                    }
                }
                else
                {
                    // we didn't find a player at i, so let's save this slot in case we make it out of this loop
                    if (i < earliestSlot)
                        earliestSlot = i;
                }
            }

            // we didn't find an open slot since it's still at dummy slot...
            if (earliestSlot == ProtocolInformation.DummySlot)
            {
                denyReason = "the game is full";
                return ProtocolInformation.DummySlot;
            }

            denyReason = null;
            return earliestSlot;
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
