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
    class Player
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Player Properties

        public readonly Byte          ID;
        public readonly NetConnection Connection;
        public readonly String        Callsign, Tag;
        public readonly TeamType      Team;

        #endregion

        public Player(Byte id, NetIncomingMessage msg)
        {
            this.ID         = id;
            this.Connection = msg.SenderConnection;
            this.Team       = ProtocolHelpers.TeamByteToType(msg.ReadByte());
            this.Callsign   = msg.ReadString();
            this.Tag        = msg.ReadString();

            Log.InfoFormat("Player #{0} '{1} <{2}>' created and joined to {3}", ID, Callsign, Tag, Team);

            // if we got this far, we can approve
            // TODO check impact of approving twice?
            //Connection.Approve();
        }

        // TODO reevaluate this overloaded constructor
        public Player(Byte id, NetConnection connection, String callsign, String tag, TeamType team)
        {
            this.ID         = id;
            this.Connection = connection;
            this.Callsign   = callsign;
            this.Tag        = tag;
            this.Team       = team;
        }

        public void HandleData(NetIncomingMessage incomingMsg)
        {
            Byte messageType = incomingMsg.ReadByte();

            switch (messageType)
            {
                case (byte)MessageType.MsgState:
                    SendState();
                    break;

                default:
                    break;
            }
        }

        private void SendState()
        {
            // TODO we should clamp world size to no more than UInt16.MaxValue bytes large
            NetOutgoingMessage worldMsg = Program.Server.CreateMessage(1 + 2 + (UInt16)Program.rawWorld.Length);
            worldMsg.Write((Byte)MessageType.MsgWorld);
            worldMsg.Write((UInt16)Program.rawWorld.Length);
            worldMsg.Write(Program.rawWorld);
            SendMessage(worldMsg, NetDeliveryMethod.ReliableOrdered, 0);

            // TODO send other state information

            NetOutgoingMessage stateMsg = Program.Server.CreateMessage(1);
            stateMsg.Write((Byte)MessageType.MsgState);
            SendMessage(stateMsg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        #region Connection Helpers

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            return Program.Server.SendMessage(msg, Connection, method);
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequence)
        {
            return Program.Server.SendMessage(msg, Connection, method, sequence);
        }

        #endregion
    }
}
