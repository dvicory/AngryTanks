using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using log4net;
using NDesk.Options;
using Lidgren.Network;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Server
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO fixme
        public static NetServer Server;
        public static byte[] rawWorld;

        static void Main(string[] args)
        {
            UInt16 port = 5150;
            int verbosity = 0;
            bool showHelp = false;
            string worldFilePath = "";

            OptionSet p = new OptionSet()
            {
                { "p|port",
                    "sets the port to run the server on",
                    (UInt16 v) => port = v
                    },
                { "w|world=",
                    "sets the world file to serve",
                    v => worldFilePath = v
                    },
                { "v",
                    "increases verbosity level",
                    v => { if (v != null) ++verbosity; }
                    },
                { "h|?|help",
                    "shows this message and exits",
                    v => showHelp = v != null
                    },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Log.Fatal(e.Message);
                ShowHelp(p, args);
                return;
            }

            // set logging level
            log4net.Core.Level loggingLevel;

            if (verbosity == 0)
                loggingLevel = log4net.Core.Level.Error;
            else if (verbosity == 1)
                loggingLevel = log4net.Core.Level.Warn;
            else if (verbosity == 2)
                loggingLevel = log4net.Core.Level.Info;
            else
                loggingLevel = log4net.Core.Level.Debug;

            ((log4net.Repository.Hierarchy.Logger)Log.Logger).Level = loggingLevel;

            // do we need to show help?
            if (showHelp)
            {
                ShowHelp(p, args);
                return;
            }

            // do we have a world file?
            if (worldFilePath.Length == 0)
            {
                Log.Fatal("A world file is required");
                ShowHelp(p, args);
                return;
            }

            // and is it valid?
            // TODO actually check if a valid world file?
            if (!File.Exists(worldFilePath))
            {
                Log.FatalFormat("A world file does not exist at '{0}'", worldFilePath);
                ShowHelp(p, args);
                return;
            }

            // let's read the world now and save it
            rawWorld = ReadWorld(worldFilePath);

            NetPeerConfiguration config = new NetPeerConfiguration("AngryTanks");

            // we need to enable these default disabled message types
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            // use configured port
            config.Port = port;

            // start server
            Server = new NetServer(config);
            Server.Start();

            // go to main loop
            AppLoop();
        }

        private static void ShowHelp(OptionSet p, string[] args)
        {
            Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " [OPTIONS]");
            Console.WriteLine("The Angry Tanks server, implementing protocol version " + ProtocolInformation.ProtocolVersion);
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private static byte[] ReadWorld(string worldPath)
        {
            FileStream worldStream = new FileStream(worldPath, FileMode.Open, FileAccess.Read);

            byte[] bytes = new byte[worldStream.Length];

            int bytesToRead = (int)worldStream.Length;
            int bytesAlreadyRead = 0;

            while (bytesToRead > 0)
            {
                int n = worldStream.Read(bytes, bytesAlreadyRead, bytesToRead);

                // we reached the end of the file
                if (n == 0)
                    break;

                bytesAlreadyRead += n;
                bytesToRead -= n;
            }

            return bytes;
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
                        case NetIncomingMessageType.WarningMessage:
                            Log.Warn(msg.ReadString());
                            break;

                        case NetIncomingMessageType.ErrorMessage:
                            Log.Error(msg.ReadString());
                            break;

                        case NetIncomingMessageType.DebugMessage:
                            Log.Debug(msg.ReadString());
                            break;

                        case NetIncomingMessageType.DiscoveryRequest:
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            {
                                // we're not interested in status changes on the server yet
                                if (msg.SenderConnection == null)
                                    break;

                                GameKeeper.HandleStatusChange(msg);

                                break;
                            }

                        case NetIncomingMessageType.ConnectionApproval:
                            {
                                // chop off header
                                MessageType messageType = (MessageType)msg.ReadByte();

                                // WTF?
                                if (messageType != MessageType.MsgEnter)
                                {
                                    String rejection = String.Format("message type not as expected (expected {0}, you sent {1})",
                                                                     MessageType.MsgEnter, messageType);
                                    msg.SenderConnection.Deny(rejection);
                                    break;
                                }

                                UInt16 clientProtoVersion = msg.ReadUInt16();

                                if (clientProtoVersion != ProtocolInformation.ProtocolVersion)
                                {
                                    String rejection = String.Format("protocol versions do not match (server is {0}, you are {1})",
                                                                     ProtocolInformation.ProtocolVersion, clientProtoVersion);
                                    msg.SenderConnection.Deny(rejection);
                                    break;
                                }

                                TeamType team = ProtocolHelpers.TeamByteToType(msg.ReadByte());
                                String callsign = msg.ReadString();
                                String tag = msg.ReadString();

                                PlayerInformation playerInfo = new PlayerInformation(ProtocolInformation.DummySlot, callsign, tag, team);

                                GameKeeper.AddPlayer(msg.SenderConnection, playerInfo);

                                break;
                            }

                        case NetIncomingMessageType.Data:
                            GameKeeper.HandleIncomingData(msg);
                            break;

                        default:
                            // welp... what shall we do?
                            break;
                    }

                    // reduce GC pressure by recycling
                    Server.Recycle(msg);
                }

                // we must sleep otherwise we will lock everything up
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
