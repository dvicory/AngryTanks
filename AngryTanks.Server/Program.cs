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

namespace AngryTanks.Server
{
    using MessageType = Protocol.MessageType;
    using TeamType    = Protocol.TeamType;

    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO fixme
        public static NetServer Server;
        public static byte[] raw_world;

        static void Main(string[] args)
        {
            UInt16 port = 5150;
            int verbosity = 0;
            bool show_help = false;
            string world_file_path = "";

            OptionSet p = new OptionSet()
            {
                { "p|port",
                    "sets the port to run the server on",
                    (UInt16 v) => port = v
                    },
                { "w|world=",
                    "sets the world file to serve",
                    v => world_file_path = v
                    },
                { "v",
                    "increases verbosity level",
                    v => { if (v != null) ++verbosity; }
                    },
                { "h|?|help",
                    "shows this message and exits",
                    v => show_help = v != null
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
            log4net.Core.Level logging_level;

            if (verbosity == 0)
                logging_level = log4net.Core.Level.Error;
            else if (verbosity == 1)
                logging_level = log4net.Core.Level.Warn;
            else if (verbosity == 2)
                logging_level = log4net.Core.Level.Info;
            else
                logging_level = log4net.Core.Level.Debug;

            ((log4net.Repository.Hierarchy.Logger)Log.Logger).Level = logging_level;

            // do we need to show help?
            if (show_help)
            {
                ShowHelp(p, args);
                return;
            }

            // do we have a world file?
            if (world_file_path.Length == 0)
            {
                Log.Fatal("A world file is required");
                ShowHelp(p, args);
                return;
            }

            // and is it valid?
            // TODO actually check if a valid world file?
            if (!File.Exists(world_file_path))
            {
                Log.FatalFormat("A world file does not exist at '{0}'", world_file_path);
                ShowHelp(p, args);
                return;
            }

            // let's read the world now and save it
            raw_world = ReadWorld(world_file_path);

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
            Console.WriteLine("The Angry Tanks server, implementing protocol version " + Protocol.ProtocolVersion);
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private static byte[] ReadWorld(string world_path)
        {
            FileStream world_stream = new FileStream(world_path, FileMode.Open, FileAccess.Read);

            byte[] bytes = new byte[world_stream.Length];

            int bytes_to_read = (int)world_stream.Length;
            int bytes_already_read = 0;

            while (bytes_to_read > 0)
            {
                int n = world_stream.Read(bytes, bytes_already_read, bytes_to_read);

                // we reached the end of the file
                if (n == 0)
                    break;

                bytes_already_read += n;
                bytes_to_read -= n;
            }

            return bytes;
        }

        private static void AppLoop()
        {
            NetIncomingMessage msg;

            Byte message_type;

            while (true)
            {
                if ((msg = Server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                            using (log4net.NDC.Push("Lidgren"))
                            {
                                Log.Debug(msg.ReadString());
                            }
                            break;
                        // TODO implement local LAN discovery
                        case NetIncomingMessageType.DiscoveryRequest:
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            // chop off header
                            message_type = msg.ReadByte();

                            // WTF?
                            if (message_type != (byte)Protocol.MessageType.MsgEnter)
                            {
                                String rejection = String.Format("message type not as expected (expected {0}, you sent {1})",
                                                                 Protocol.MessageType.MsgEnter, message_type);
                                msg.SenderConnection.Deny(rejection);
                                break;
                            }

                            UInt16 client_proto_version = msg.ReadUInt16();

                            if (client_proto_version != Protocol.ProtocolVersion)
                            {
                                String rejection = String.Format("protocol versions do not match (server is {0}, you are {1})",
                                                                 Protocol.ProtocolVersion, client_proto_version);
                                msg.SenderConnection.Deny(rejection);
                                break;
                            }

                            Int16 id = GameKeeper.AddPlayer(msg);

                            // game is full if AddPlayer returns -1
                            if (id < 0)
                            {
                                // TODO be able to get callsign, etc here without duplicating packet reading code
                                Log.InfoFormat("Player from {0} tried to join, but the game was full", msg.SenderConnection);
                                msg.SenderConnection.Deny("the game is currently full");
                                break;
                            }

                            Player joined_player = GameKeeper.GetPlayerByID((Byte)id);

                            joined_player.Connection.Approve();

                            break;

                        case NetIncomingMessageType.Data:
                            GameKeeper.GetPlayerByConnection(msg.SenderConnection).HandleData(msg);
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
