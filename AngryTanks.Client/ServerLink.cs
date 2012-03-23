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

        private Action<StreamReader> MapLoadedCallback;

        // TODO make this more robust... need connection state
        private bool got_world = false;
        public bool GotWorld
        {
            get
            {
                return got_world;
            }
        }

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

        public NetConnection Connect(string host, int port, Action<StreamReader> mapLoadedCallback)
        {
            MapLoadedCallback = mapLoadedCallback; 

            NetOutgoingMessage hailMessage = CreateMessage();

            // TODO be able to change callsign/tag
            hailMessage.Write((Byte)MessageType.MsgEnter);
            hailMessage.Write(Protocol.ProtocolVersion);
            hailMessage.Write((Byte)TeamType.RogueTeam);
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

                map_request.Write((Byte)MessageType.MsgWorld);

                SendMessage(map_request, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        // TODO actually handle data
        private void HandleData(NetIncomingMessage msg)
        {
            Byte messageType = msg.ReadByte();

            switch (messageType)
            {
                case (Byte)MessageType.MsgWorld:
                    Log.Debug("Got MsgWorld");

                    UInt16 map_len = msg.ReadUInt16();

                    Byte[] raw_world = new Byte[map_len];
                    raw_world = msg.ReadBytes(map_len);

                    got_world = true;

                    MemoryStream ms = new MemoryStream(raw_world);
                    StreamReader sr = new StreamReader(ms);

                    MapLoadedCallback(sr);

                    break;

                default:
                    // if we get anything else we should fail
                    // protocol version should protect us from unknowns
                    break;

            }
        }
    }
}
