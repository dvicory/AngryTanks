using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngryTanks.Common
{
    namespace Protocol
    {
        public static class ProtocolInformation
        {
            public static readonly UInt16 ProtocolVersion = 9;
            public static readonly Byte MaxPlayers = 100;
            public static readonly Byte DummySlot = 255;
            public static readonly Byte MaxShots = 20;
            public static readonly Byte DummyShot = Byte.MaxValue;
        }

        public enum MessageType
        {
            MsgEnter,
            MsgGameInformation,
            MsgState,
            MsgSetVariable,
            MsgAddPlayer,
            MsgRemovePlayer,
            MsgWorld,
            MsgPlayerClientUpdate, // from client to server
            MsgPlayerServerUpdate, // from server to client
            MsgDeath,
            MsgSpawn
        }

        public enum GamePlayType
        {
            FreeForAll,
            CaptureTheFlag,
            RabbitHunt
        }

        public enum TeamType
        {
            AutomaticTeam = 240,
            RogueTeam,
            RedTeam,
            GreenTeam,
            BlueTeam,
            PurpleTeam,
            ObserverTeam,
            NoTeam
        }

        public enum PlayerState
        {
            None,
            Joining,
            Leaving,
            Exploding,
            Dead,
            Alive,
            Spawning
        }

        public static class ProtocolHelpers
        {
            public static Color TeamTypeToColor(TeamType team)
            {
                Color color;

                switch (team)
                {
                    case TeamType.AutomaticTeam:
                        color = Color.White;
                        break;
                    case TeamType.RogueTeam:
                        color = Color.DarkGray;
                        break;
                    case TeamType.RedTeam:
                        color = Color.Red;
                        break;
                    case TeamType.GreenTeam:
                        color = Color.Green;
                        break;
                    case TeamType.BlueTeam:
                        color = Color.Blue;
                        break;
                    case TeamType.PurpleTeam:
                        color = Color.Purple;
                        break;
                    case TeamType.ObserverTeam:
                        color = Color.White;
                        break;
                    default: // WTF?
                        throw new ArgumentOutOfRangeException("team", team, "team must be automatic, rogue, red, green, blue, purple, or observer team");
                        break;
                }

                return color;
            }

            public static String TeamTypeToName(TeamType team)
            {
                String name;

                switch (team)
                {
                    case TeamType.AutomaticTeam:
                        name = "automatic";
                        break;
                    case TeamType.RogueTeam:
                        name = "rogue";
                        break;
                    case TeamType.RedTeam:
                        name = "red";
                        break;
                    case TeamType.GreenTeam:
                        name = "green";
                        break;
                    case TeamType.BlueTeam:
                        name = "blue";
                        break;
                    case TeamType.PurpleTeam:
                        name = "purple";
                        break;
                    case TeamType.ObserverTeam:
                        name = "observer";
                        break;
                    default: // WTF?
                        throw new ArgumentOutOfRangeException("team", team, "team must be automatic, rogue, red, green, blue, purple, or observer team");
                        break;
                }

                return name;
            }
        }
    }
}
