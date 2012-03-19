using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace AngryTanks.Client
{
    class ServerLink : NetClient
    {
        public readonly static Byte ProtocolVersion = 1;

        enum MessageType
        {
            MsgEnter,
            MsgWorld
        }

        enum TeamType
        {
            RogueTeam,
            RedTeam,
            GreenTeam,
            BlueTeam,
            PurpleTeam,
            ObserverTeam
        }

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

        // TODO: be able to change callsign/player
        new public NetConnection Connect(string host, int port)
        {
            NetOutgoingMessage hailMessage = CreateMessage();

            hailMessage.Write((byte)MessageType.MsgEnter);
            hailMessage.Write((byte)ProtocolVersion);
            hailMessage.Write((byte)TeamType.RogueTeam);
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
            return;
        }
    }
}
