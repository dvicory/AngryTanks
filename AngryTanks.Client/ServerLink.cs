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

    public class ServerLinkStateChangedEvent<T> : EventArgs
    {
        public readonly T OldValue, NewValue;

        public ServerLinkStateChangedEvent(T oldValue, T newValue)
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

        public ServerLinkMessageEvent(MessageType messageType, MsgBasePacket messageData, NetServerLinkStatus serverLinkStatus)
        {
            MessageType      = messageType;
            MessageData      = messageData;
            ServerLinkStatus = serverLinkStatus;
        }
    }

    public class ServerLink
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            set
            {
                // TODO fire event
                serverLinkStatus = value;
            }
        }

        /// <summary>
        /// Event hook to receive messages
        /// </summary>
        public event EventHandler<ServerLinkMessageEvent> MessageReceivedEvent;

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

        public NetConnection Connect(string host, UInt16 port)
        {
            NetOutgoingMessage hailMessage = Client.CreateMessage();

            // TODO be able to change callsign/tag
            hailMessage.Write((Byte)MessageType.MsgEnter);
            hailMessage.Write(ProtocolInformation.ProtocolVersion);
            hailMessage.Write((Byte)TeamType.RogueTeam);
            hailMessage.Write("Player Callsign");
            hailMessage.Write("Player Tag");

            // we are now initiating the connect, so change status
            ServerLinkStatus = NetServerLinkStatus.Connecting;
            
            return Client.Connect(host, port, hailMessage);
        }

        public void Disconnect(string byeMessage)
        {
            Client.Disconnect(byeMessage);

            // in the processing of disconnecting... now
            ServerLinkStatus = NetServerLinkStatus.Disconnecting;
        }

        public void Update()
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
                        HandleData(msg);
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

        private void HandleData(NetIncomingMessage msg)
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

                        MsgAddPlayerPacket message = MsgAddPlayerPacket.Read(msg);

                        FireEvent(message);

                        break;
                    }

                case MessageType.MsgRemovePlayer:
                    {
                        Log.DebugFormat("Got MsgRemovePlayer ({0} bytes)", msg.LengthBytes);

                        MsgRemovePlayerPacket message = MsgRemovePlayerPacket.Read(msg);

                        FireEvent(message);

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

                        FireEvent(msgWorldEventData);

                        break;
                    }

                default:
                    // if we get anything else we should fail
                    // protocol version should protect us from unknowns
                    break;

            }
        }

        private void FireEvent(MsgBasePacket msgData)
        {
            EventHandler<ServerLinkMessageEvent> handler = MessageReceivedEvent;

            // prevent race condition
            if (handler != null)
            {
                // notify delegates attached to event
                ServerLinkMessageEvent e = new ServerLinkMessageEvent(msgData.MsgType, msgData, ServerLinkStatus);
                handler(this, e);
            }
        }
    }
}
