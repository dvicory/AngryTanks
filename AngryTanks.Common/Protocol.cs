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
        }
    }
}
