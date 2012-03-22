using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

using AngryTanks.Common;

namespace AngryTanks.Server
{
    using MessageType = Protocol.MessageType;
    using TeamType    = Protocol.TeamType;

    class Player
    {
        public readonly Byte          ID;
        public readonly NetConnection Connection;
        public readonly String        Callsign, Tag;
        public readonly TeamType      Team;

        public Player(Byte id, NetIncomingMessage msg)
        {
            this.ID         = id;
            this.Connection = msg.SenderConnection;
            this.Team       = Protocol.TeamByteToType(msg.ReadByte());
            this.Callsign   = msg.ReadString();
            this.Tag        = msg.ReadString();
        }

        public Player(Byte id, NetConnection connection, String callsign, String tag, TeamType team)
        {
            this.ID         = id;
            this.Connection = connection;
            this.Callsign   = callsign;
            this.Tag        = tag;
            this.Team       = team;
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            return Program.Server.SendMessage(msg, Connection, method);
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequence)
        {
            return Program.Server.SendMessage(msg, Connection, method, sequence);
        }
    }
}
