using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    /// <summary>
    /// Status for a <see cref="ServerLink"/> instance
    /// </summary>
    public enum NetServerLinkStatus
    {
        /// <summary>
        /// No connection, or attempt, in place
        /// </summary>
        None,

        /// <summary>
        /// Connection in progress
        /// </summary>
        Connecting,

        /// <summary>
        /// Received MsgAccept and will begin to receive initial state
        /// </summary>
        Accepted,

        /// <summary>
        /// In the process of receiving state
        /// </summary>
        GettingState,

        /// <summary>
        /// Connected to server and received initial state
        /// </summary>
        Connected,

        /// <summary>
        /// In the process of disconnecting
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Disconnected from server
        /// </summary>
        Disconnected
    }

    public class ServerLinkStateChangedEvent : EventArgs
    {
        public readonly NetServerLinkStatus OldValue, NewValue;

        public ServerLinkStateChangedEvent(NetServerLinkStatus oldValue, NetServerLinkStatus newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class ServerLinkMessageEvent : EventArgs
    {
        public readonly MessageType MessageType;
        public readonly MsgBasePacket MessageData;
        public readonly NetServerLinkStatus ServerLinkStatus;
        public readonly GameTime Time;

        public ServerLinkMessageEvent(MessageType messageType, MsgBasePacket messageData, NetServerLinkStatus serverLinkStatus, GameTime gameTime)
        {
            this.MessageType = messageType;
            this.MessageData = messageData;
            this.ServerLinkStatus = serverLinkStatus;
            this.Time = gameTime;
        }
    }

    public class ServerLink
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Event hook to receive messages
        /// </summary>
        public event EventHandler<ServerLinkMessageEvent> MessageReceivedEvent;

        /// <summary>
        /// Event hook to receive changes to <see cref="ServerLinkState"/>
        /// </summary>
        public event EventHandler<ServerLinkStateChangedEvent> ServerLinkStateChanged;

        /// <summary>
        /// Configuration for <see cref="NetClient"/>
        /// </summary>
        private static NetPeerConfiguration Config;

        /// <summary>
        /// Reference to the <see cref="NetClient"/> instance
        /// </summary>
        private NetClient Client;

        /// <summary>
        /// Backs ServerLinkStatus property
        /// </summary>
        private NetServerLinkStatus serverLinkStatus = NetServerLinkStatus.None;

        /// <summary>
        /// Get the status of <see cref="ServerLink"/>
        /// </summary>
        public NetServerLinkStatus ServerLinkStatus
        {
            get { return serverLinkStatus; }
            private set
            {
                EventHandler<ServerLinkStateChangedEvent> handler = ServerLinkStateChanged;

                // prevent race condition
                if (handler != null)
                {
                    // notify delegates attached to event
                    ServerLinkStateChangedEvent e = new ServerLinkStateChangedEvent(serverLinkStatus, value);
                    handler(this, e);
                }

                serverLinkStatus = value;
            }
        }

        public ServerLink()
        {
            Client = new NetClient(SetupConfig());
            Client.Start();
        }

        private static NetPeerConfiguration SetupConfig()
        {
            Config = new NetPeerConfiguration("AngryTanks");

            // enable these default disabled message types
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            return Config;
        }

        public NetConnection Connect(String host, UInt16? port, String callsign, String tag, TeamType team)
        {
            NetOutgoingMessage hailMessage = Client.CreateMessage();

            hailMessage.Write((Byte)MessageType.MsgEnter);
            hailMessage.Write(ProtocolInformation.ProtocolVersion);
            hailMessage.Write((Byte)team);
            hailMessage.Write(callsign);
            hailMessage.Write((tag != null ? tag : ""));

            // we are now initiating the connect, so change status
            ServerLinkStatus = NetServerLinkStatus.Connecting;

            if (!port.HasValue)
                port = 5150;

            return Client.Connect(host, port.Value, hailMessage);
        }

        public void Disconnect(string byeMessage)
        {
            Client.Disconnect(byeMessage);

            // we don't want to change our status to disconnecting if we know we can't get out of it
            if (ServerLinkStatus != NetServerLinkStatus.None
                && ServerLinkStatus != NetServerLinkStatus.Disconnecting
                && ServerLinkStatus != NetServerLinkStatus.Disconnected)
                ServerLinkStatus = NetServerLinkStatus.Disconnecting;
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            return Client.SendMessage(msg, method);
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
        {
            return Client.SendMessage(msg, method, sequenceChannel);
        }

        public NetOutgoingMessage CreateMessage()
        {
            return Client.CreateMessage();
        }

        public void Update(GameTime gameTime)
        {
            NetIncomingMessage msg;

            // are there any messages for us to read?
            while ((msg = Client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.WarningMessage:
                        Log.Warn(msg.ReadString());
                        break;

                    case NetIncomingMessageType.ErrorMessage:
                        Log.Error(msg.ReadString());
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        Log.Debug(msg.ReadString());
                        break;

                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        Log.InfoFormat("Connection latency: {0}", msg.ReadSingle());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                                ServerLinkStatus = NetServerLinkStatus.Accepted;
                                break;

                            case NetConnectionStatus.Disconnected:
                                ServerLinkStatus = NetServerLinkStatus.Disconnected;
                                break;

                            default:
                                break;
                        }

                        string reason = msg.ReadString();
                        Log.DebugFormat("New status: {0} ({1})", status, reason);

                        break;

                    case NetIncomingMessageType.Data:
                        HandleData(gameTime, msg);
                        break;

                    default:
                        Log.DebugFormat("Hit default message handler for {0}", msg.MessageType);
                        break;
                }

                // reduce GC pressure by recycling
                Client.Recycle(msg);
            }

            // let server know we're ready to start receiving state if we've been accepted
            if (ServerLinkStatus == NetServerLinkStatus.Accepted)
            {
                NetOutgoingMessage msgReady = Client.CreateMessage();

                msgReady.Write((Byte)MessageType.MsgState);

                Client.SendMessage(msgReady, NetDeliveryMethod.ReliableOrdered, 0);

                // we now move to getting initial state
                ServerLinkStatus = NetServerLinkStatus.GettingState;
            }
        }

        private void HandleData(GameTime gameTime, NetIncomingMessage msg)
        {
            MessageType messageType = (MessageType)msg.ReadByte();

            switch (messageType)
            {
                case MessageType.MsgState:
                    {
                        Log.DebugFormat("Got MsgState ({0} bytes)", msg.LengthBytes);

                        // we finished receiving state at this point
                        ServerLinkStatus = NetServerLinkStatus.Connected;

                        break;
                    }

                case MessageType.MsgAddPlayer:
                    {
                        Log.DebugFormat("Got MsgAddPlayer ({0} bytes)", msg.LengthBytes);
                        MsgAddPlayerPacket packet = MsgAddPlayerPacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                case MessageType.MsgRemovePlayer:
                    {
                        Log.DebugFormat("Got MsgRemovePlayer ({0} bytes)", msg.LengthBytes);
                        MsgRemovePlayerPacket packet = MsgRemovePlayerPacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                case MessageType.MsgDeath:
                    {
                        Log.DebugFormat("Got MsgDeath ({0} bytes)", msg.LengthBytes);
                        MsgDeathPacket packet = MsgDeathPacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                case MessageType.MsgSpawn:
                    {
                        Log.DebugFormat("Got MsgSpawn ({0} bytes)", msg.LengthBytes);
                        MsgSpawnPacket packet = MsgSpawnPacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                case MessageType.MsgScore:
                    {
                        Log.DebugFormat("Got MsgScore ({0} bytes)", msg.LengthBytes);
                        MsgScorePacket packet = MsgScorePacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                case MessageType.MsgWorld:
                    {
                        Log.DebugFormat("Got MsgWorld ({0} bytes)", msg.LengthBytes);

                        UInt16 mapLength = msg.ReadUInt16();
                        Byte[] rawWorld = new Byte[mapLength];
                        msg.ReadBytes(mapLength, out rawWorld);

                        MemoryStream ms = new MemoryStream(rawWorld);
                        StreamReader sr = new StreamReader(ms);

                        MsgWorldPacket msgWorldEventData = new MsgWorldPacket(mapLength, sr);

                        FireMessageEvent(gameTime, msgWorldEventData);

                        break;
                    }

                case MessageType.MsgPlayerServerUpdate:
                    {
                        MsgPlayerServerUpdatePacket packet = MsgPlayerServerUpdatePacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                case MessageType.MsgBeginShot:
                    {
                        MsgBeginShotPacket packet = MsgBeginShotPacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                case MessageType.MsgEndShot:
                    {
                        MsgEndShotPacket packet = MsgEndShotPacket.Read(msg);
                        FireMessageEvent(gameTime, packet);
                        break;
                    }

                default:
                    // if we get anything else we should fail
                    // protocol version should protect us from unknowns
                    break;

            }
        }

        private void FireMessageEvent(GameTime gameTime, MsgBasePacket msgData)
        {
            EventHandler<ServerLinkMessageEvent> handler = MessageReceivedEvent;

            // prevent race condition
            if (handler != null)
            {
                // notify delegates attached to event
                ServerLinkMessageEvent e = new ServerLinkMessageEvent(msgData.MsgType, msgData, ServerLinkStatus, gameTime);
                handler(this, e);
            }
        }
    }
}
