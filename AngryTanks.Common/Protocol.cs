using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            public static TeamType TeamByteToType(Byte team_byte)
            {
                TeamType team;
                switch (team_byte)
                {
                    case (Byte)TeamType.AutomaticTeam:
                        team = TeamType.AutomaticTeam;
                        break;
                    case (Byte)TeamType.RogueTeam:
                        team = TeamType.RogueTeam;
                        break;
                    case (Byte)TeamType.RedTeam:
                        team = TeamType.RedTeam;
                        break;
                    case (Byte)TeamType.GreenTeam:
                        team = TeamType.GreenTeam;
                        break;
                    case (Byte)TeamType.BlueTeam:
                        team = TeamType.BlueTeam;
                        break;
                    case (Byte)TeamType.PurpleTeam:
                        team = TeamType.PurpleTeam;
                        break;
                    case (Byte)TeamType.ObserverTeam:
                        team = TeamType.ObserverTeam;
                        break;
                    default: // WTF?
                        team = TeamType.NoTeam;
                        break;
                }

                return team;
            }
        }
    }
}
