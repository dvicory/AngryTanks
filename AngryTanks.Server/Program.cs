using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
                            msg.ReadByte();

                            // spit out info
                            Console.WriteLine("PROTO VERSION: " + msg.ReadByte());
                            Console.WriteLine("TEAM: " + msg.ReadByte());
                            Console.WriteLine("CALLSIGN: " + msg.ReadString());
                            Console.WriteLine("TAG: " + msg.ReadString());

                            // TODO here's where we should store the client and fire off sending the world

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
