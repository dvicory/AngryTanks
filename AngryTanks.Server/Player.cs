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
    public class Player
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Player Properties

        public readonly Byte          Slot;
        public readonly NetConnection Connection;
        public readonly String        Callsign, Tag;
        public readonly TeamType      Team;

        private PlayerState playerState;
        public PlayerState State
        {
            get { return playerState; }
        }

        #endregion

        public Player(Byte slot, NetConnection connection, PlayerInformation playerInfo)
        {
            this.Slot       = slot;
            this.Connection = connection;
            this.Team       = playerInfo.Team;
            this.Callsign   = playerInfo.Callsign;
            this.Tag        = playerInfo.Tag;

            this.playerState = PlayerState.Joining;

            Log.InfoFormat("Player #{0} \"{1}\" <{2}> created and joined to {3}", Slot, Callsign, Tag, Team);
        }

        public void HandleData(NetIncomingMessage incomingMsg)
        {
            MessageType messageType = (MessageType)incomingMsg.ReadByte();

            switch (messageType)
            {
                case MessageType.MsgState:
                    SendState();
                    break;

                default:
                    break;
            }
        }

        private void SendState()
        {
            Log.DebugFormat("Sending state to #{0}...", Slot);

            // TODO we should clamp world size to no more than UInt16.MaxValue bytes large
            // first send the world
            NetOutgoingMessage worldMsg = Program.Server.CreateMessage(1 + 2 + (UInt16)Program.rawWorld.Length);
            worldMsg.Write((Byte)MessageType.MsgWorld);
            worldMsg.Write((UInt16)Program.rawWorld.Length);
            worldMsg.Write(Program.rawWorld);
            SendMessage(worldMsg, NetDeliveryMethod.ReliableOrdered, 0);

            // TODO send other state information... like flags

            // tell him about everyone else
            Log.DebugFormat("Sending MsgAddPlayer for each player to #{0}", Slot);

            foreach (Player otherPlayer in GameKeeper.Players)
            {
                // don't want to tell our player about himself...
                if (otherPlayer.Slot == this.Slot)
                    continue;

                /*
                NetOutgoingMessage msgAddPlayer = Program.Server.CreateMessage();
                msgAddPlayer.Write((Byte)MessageType.MsgAddPlayer);
                msgAddPlayer.Write(otherPlayer.Slot);
                msgAddPlayer.Write((Byte)otherPlayer.Team);
                msgAddPlayer.Write(otherPlayer.Callsign);
                msgAddPlayer.Write(otherPlayer.Tag);
                */

                NetOutgoingMessage packet = Program.Server.CreateMessage();

                MsgAddPlayerPacket message =
                    new MsgAddPlayerPacket(new PlayerInformation(otherPlayer.Slot, otherPlayer.Callsign, otherPlayer.Tag, otherPlayer.Team));

                packet.Write((Byte)message.MsgType);
                message.Write(packet);

                Log.DebugFormat("MsgAddPlayer Compiled ({0} bytes) for player #{1} and being sent to player #{2}",
                                packet.LengthBytes, otherPlayer.Slot, this.Slot);

                SendMessage(packet, NetDeliveryMethod.ReliableOrdered, 0);
            }
            
            // TODO send scores and such...

            // let them know we're ready to move on
            NetOutgoingMessage stateMsg = Program.Server.CreateMessage();
            stateMsg.Write((Byte)MessageType.MsgState);
            SendMessage(stateMsg, NetDeliveryMethod.ReliableOrdered, 0);

            // we're now ready to move to the spawn state
            this.playerState = PlayerState.Spawning;
        }

        #region Connection Helpers

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            return Program.Server.SendMessage(msg, Connection, method);
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
        {
            return Program.Server.SendMessage(msg, Connection, method, sequenceChannel);
        }

        #endregion
    }
}
