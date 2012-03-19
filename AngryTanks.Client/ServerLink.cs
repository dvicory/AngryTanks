using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AngryTanks.Common;
using Lidgren.Network;

namespace AngryTanks.Client
{
    class ServerLink : NetClient
    {
        private static NetPeerConfiguration Config;

        public ServerLink()
            : base(SetupConfig())
        {
            Start();
        }

        private static NetPeerConfiguration SetupConfig()
        {
            Config = new NetPeerConfiguration("AngryTanks");

            // enable these default disabled message types
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            return Config;
        }

        new public NetConnection Connect(string host, int port)
        {
            NetOutgoingMessage hailMessage = CreateMessage();

            // TODO be able to change callsign/tag
            hailMessage.Write((byte)Protocol.MessageType.MsgEnter);
            hailMessage.Write((byte)Protocol.ProtocolVersion);
            hailMessage.Write((byte)Protocol.TeamType.RogueTeam);
            hailMessage.Write("Player Callsign");
            hailMessage.Write("Player Tag");

            return base.Connect(host, port, hailMessage);
        }

        public void Update()
        {
            NetIncomingMessage msg;

            // are there any messages for us to read?
            while ((msg = ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        // TODO do we need a flag to prevent from sending anything until we are approved?
                        break;
                    case NetIncomingMessageType.Data:
                        HandleData(msg);
                        break;
                    default:
                        break;
                }
            }
        }

        // TODO actually handle data
        private void HandleData(NetIncomingMessage msg)
        {
            byte messageType = msg.ReadByte();

            switch (messageType)
            {
                case (byte)Protocol.MessageType.MsgWorld:
                    // TODO get the world to parser
                    // call map class directly? use callback? decisions...
                    break;
                default:
                    // if we get anything else we should fail
                    // protocol version should protect us from unknowns
                    break;
            }
        }
    }
}
