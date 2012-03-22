using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;

namespace AngryTanks.Server
{
    using MessageType = Protocol.MessageType;
    using TeamType    = Protocol.TeamType;

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
            this.Team       = Protocol.TeamByteToType(msg.ReadByte());
            this.Callsign   = msg.ReadString();
            this.Tag        = msg.ReadString();

            Log.InfoFormat("Player #{0} '{1} <{2}>' created and joined to {3}", ID, Callsign, Tag, Team);

            // if we got this far, we can approve
            // TODO check impact of approving twice?
            Connection.Approve();
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

        public void HandleData(NetIncomingMessage incoming_msg)
        {
            Byte message_type = incoming_msg.ReadByte();

            switch (message_type)
            {
                case (byte)MessageType.MsgWorld:
                    // TODO we should clamp world size to no more than UInt16.Max bytes large
                    NetOutgoingMessage world_msg = Program.Server.CreateMessage(1 + 2 + Program.raw_world.Length);
                    world_msg.Write((byte)MessageType.MsgWorld);
                    world_msg.Write((UInt16)Program.raw_world.Length);
                    world_msg.Write(Program.raw_world);

                    Program.Server.SendMessage(world_msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);

                    break;

                default:
                    break;
            }
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
