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

        #region GameKeeper Properties

        public List<Player> Players
        {
            get { return players.Values.ToList(); }
        }

        public Int16 PlayerCount
        {
            get
            {
                return (Int16)players.Count;
            }
        }

        #endregion

        private readonly NetServer server;

        public NetServer Server
        {
            get { return server; }
        }

        private readonly Byte[] rawWorld;

        public Byte[] RawWorld
        {
            get { return rawWorld; }
        }

        private Dictionary<Byte, Player> players = new Dictionary<Byte, Player>();

        private VariableDatabase VarDB = new VariableDatabase();

        public GameKeeper(NetServer server, Byte[] rawWorld)
        {
            this.server = server;
            this.rawWorld = rawWorld;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUpdate"></param>
        public void Update(DateTime lastUpdate)
        {
            foreach (Player player in Players)
                player.Update(lastUpdate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incomingMessage"></param>
        public void HandleIncomingData(NetIncomingMessage incomingMessage)
        {
            Player player = GetPlayerByConnection(incomingMessage.SenderConnection);

            if (player != null)
                player.HandleData(incomingMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incomingMessage"></param>
        public void HandleStatusChange(NetIncomingMessage incomingMessage)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="playerInfo"></param>
        public void AddPlayer(NetConnection connection, PlayerInformation playerInfo)
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
            players[slot] = new Player(this, slot, connection, playerInfo);

            // and tell everyone else about this awesome new player
            Log.DebugFormat("Sending MsgAddPlayer to everyone else about player #{0}", slot);

            NetOutgoingMessage packet = Server.CreateMessage();

            MsgAddPlayerPacket message = new MsgAddPlayerPacket(players[slot].PlayerInfo, false);

            packet.Write((Byte)message.MsgType);
            message.Write(packet);

            Log.DebugFormat("MsgAddPlayer Compiled ({0} bytes) and being sent to {1} recipients",
                            packet.LengthBytes, players.Count - 1);

            // send to everyone except our new player, we let Player itself decide when to send the state to the new guy
            Server.SendToAll(packet, connection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reason"></param>
        public void RemovePlayer(Player player, String reason)
        {
            Log.InfoFormat("Removing player #{0} ({1})", player.Slot, reason);

            // nuke player from the dictionary
            players.Remove(player.Slot);

            // now let's tell all the other players the dude left
            NetOutgoingMessage packet = Server.CreateMessage();

            MsgRemovePlayerPacket message =
                new MsgRemovePlayerPacket(player.Slot, reason);

            packet.Write((Byte)message.MsgType);
            message.Write(packet);

            // send to all
            Server.SendToAll(packet, null, NetDeliveryMethod.ReliableOrdered, 0);
            
            // disposing of player would be a good idea
            if (player.Connection != null)
                player.Connection.Disconnect(reason);
        }

        /// <summary>
        /// Gets the <see cref="Player"/> associated with a certain instance of <see cref="NetConnection"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns><see cref="Player"/>, if one found, otherwise null.</returns>
        public Player GetPlayerByConnection(NetConnection connection)
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

        /// <summary>
        /// Attempts to find a slot to allocate a <see cref="Player"/>.
        /// </summary>
        /// <param name="playerAdding"><see cref="PlayerInformation"/> about the player being added.</param>
        /// <param name="denyReason"><see cref="String"/> to be populated with a reason if a slot can not be allocated.</param>
        /// <returns><see cref="ProtocolInformation.DummySlot"/> if a slot can't be allocated, otherwise the slot.</returns>
        private Byte AllocateSlot(PlayerInformation playerAdding, out String denyReason)
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
    }
}
