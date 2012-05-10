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
            public static readonly UInt16 ProtocolVersion = 11;
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
            MsgSpawn,
            MsgShotBegin,
            MsgShotEnd
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
                switch (team)
                {
                    case TeamType.AutomaticTeam:
                        return Color.White;
                    case TeamType.RogueTeam:
                        return Color.Yellow;
                    case TeamType.RedTeam:
                        return Color.Red;
                    case TeamType.GreenTeam:
                        return Color.Green;
                    case TeamType.BlueTeam:
                        return Color.Blue;
                    case TeamType.PurpleTeam:
                        return Color.Purple;
                    case TeamType.ObserverTeam:
                        return Color.White;
                    default: // WTF?
                        throw new ArgumentOutOfRangeException("team", team, "team must be automatic, rogue, red, green, blue, purple, or observer team");
                }
            }

            public static String TeamTypeToName(TeamType team)
            {
                switch (team)
                {
                    case TeamType.AutomaticTeam:
                        return "automatic";
                    case TeamType.RogueTeam:
                        return "rogue";
                    case TeamType.RedTeam:
                        return "red";
                    case TeamType.GreenTeam:
                        return "green";
                    case TeamType.BlueTeam:
                        return "blue";
                    case TeamType.PurpleTeam:
                        return "purple";
                    case TeamType.ObserverTeam:
                        return "observer";
                    default: // WTF?
                        throw new ArgumentOutOfRangeException("team", team, "team must be automatic, rogue, red, green, blue, purple, or observer team");
                }
            }
        }
    }
}
