using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;

namespace AngryTanks.Client
{
    using MessageType = Protocol.MessageType;
    using TeamType    = Protocol.TeamType;

    class ServerLink : NetClient
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static NetPeerConfiguration Config;

        // TODO make this more robust... need connection state
        private bool got_world = false;

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
            hailMessage.Write((byte)MessageType.MsgEnter);
            hailMessage.Write(Protocol.ProtocolVersion);
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
                    case NetIncomingMessageType.Data:
                        Log.Debug("Got Data");
                        HandleData(msg);
                        break;
                    default:
                        Log.DebugFormat("Got Default: {0}", msg.MessageType);
                        break;
                }

                // reduce GC pressure by recycling
                Recycle(msg);
            }

            // TODO we should check state and see what to do, this is hacked in
            if (!got_world)
            {
                NetOutgoingMessage map_request = CreateMessage();

                map_request.Write((byte)MessageType.MsgWorld);

                SendMessage(map_request, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        // TODO actually handle data
        private void HandleData(NetIncomingMessage msg)
        {
            Byte message_type = msg.ReadByte();

            switch (message_type)
            {
                case (byte)MessageType.MsgWorld:
                    // TODO get the world to parser
                    // call map class directly? use callback? decisions...
                    Log.Debug("Got MsgWorld");

                    UInt16 map_len = msg.ReadUInt16();

                    byte[] raw_world = new byte[map_len];
                    raw_world = msg.ReadBytes((int)map_len);

                    got_world = true;

                    /*
                    MemoryStream ms = new MemoryStream(raw_world);
                    StreamReader sr = new StreamReader(ms);
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        System.Diagnostics.Debug.Write(line);
                    }
                    */

                    break;
                default:
                    // if we get anything else we should fail
                    // protocol version should protect us from unknowns
                    break;
            }
        }
    }
}
