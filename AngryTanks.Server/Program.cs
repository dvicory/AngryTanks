using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using AngryTanks.Common;
using Lidgren.Network;

namespace AngryTanks.Server
{
    class Program
    {
        public static NetServer Server;

        static void Main(string[] args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("AngryTanks");

            // we need to enable these default disabled message types
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            config.Port = 5150;

            Server = new NetServer(config);
            Server.Start();

            AppLoop();
        }

        private static void AppLoop()
        {
            NetIncomingMessage msg;

            while (true)
            {
                if ((msg = Server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                            Console.WriteLine("DEBUG: " + msg.ReadString());
                            break;
                        // TODO implement
                        case NetIncomingMessageType.DiscoveryRequest:
                            break;
                        case NetIncomingMessageType.ConnectionApproval:

                            // chop off header until we understand what it is
                            byte messageType = msg.ReadByte();

                            // WTF?
                            if (messageType != (byte)Protocol.MessageType.MsgEnter)
                            {
                                String rejection = String.Format("message type not as expected (expected {0}, you sent {1})",
                                                                 Protocol.MessageType.MsgEnter, messageType);
                                msg.SenderConnection.Deny(rejection);
                                break;
                            }

                            byte clientProtoVersion = msg.ReadByte();

                            if (clientProtoVersion != Protocol.ProtocolVersion)
                            {
                                String rejection = String.Format("protocol versions do not match (server is {0}, you are {1})",
                                                                 Protocol.ProtocolVersion, clientProtoVersion);
                                msg.SenderConnection.Deny(rejection);
                                break;
                            }

                            // spit out info
                            Console.WriteLine("PROTO VERSION: " + clientProtoVersion);
                            Console.WriteLine("TEAM: " + msg.ReadByte());
                            Console.WriteLine("CALLSIGN: " + msg.ReadString());
                            Console.WriteLine("TAG: " + msg.ReadString());

                            // TODO here's where we should store the client and fire off sending the world
                            msg.SenderConnection.Approve();

                            break;
                        default:
                            // welp... what do we do?
                            break;
                    }
                }

                // we must sleep otherwise we will lock everything up
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
